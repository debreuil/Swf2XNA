using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DDW.Display;
using Microsoft.Xna.Framework;
using Box2D.XNA;
using V2DRuntime.V2D;
using V2DRuntime.Attributes;

namespace DDW.V2D
{
    public class V2DSprite : Sprite, IJointable
	{
		V2DScreen v2dScreen;

		public Body body;
        protected List<Joint> jointRefs = new List<Joint>();
        protected float density;
        protected float friction;
        protected float restitution;
        protected float linearDamping;
        protected float angularDamping;
        protected bool isStatic;
        protected short groupIndex = 0;
        protected bool fixedRotation;
        protected List<V2DShape> polygons = new List<V2DShape>();

        protected static short groupIndexCounter = 1;
		public V2DSpriteAttribute attributeProperties;


        public V2DSprite(Texture2D texture, V2DInstance instance) : base(texture, instance)
        {
        }

		private float worldScale = 15;
		public float WorldScale { get { return worldScale; } set { worldScale = value; } }
		public V2DScreen VScreen { get { return v2dScreen; } set { v2dScreen = value; } }

		public override void Initialize()
		{
			base.Initialize();
			this.AddJoints();
		}
		public override void SetStageAndScreen()
		{
			base.SetStageAndScreen();
			if (screen == null || !(screen is V2DScreen))
			{
				throw new Exception("V2D can only be added to V2DScreens");
			}
			v2dScreen = (V2DScreen)screen;
		}
        public bool IsStatic
        {
            get
            {
                return isStatic;
            }
            set
            {
                isStatic = value;
                if (value && body != null)
				{
					Fixture fl = body.GetFixtureList();
                    while (fl != null)
                    {
                        fl.SetDensity(0);
                        fl = fl.GetNext();
                    }

                    //body.SetMassFromShapes();
                }
            }
		}
		public void SetMaskAndCategoryBits(ushort mask, ushort category)
		{
			if (body != null)
			{
				Fixture fl = body.GetFixtureList();
				while (fl != null)
				{
					// note: can't change variables of structs that are properties
					Filter fd;
					fl.GetFilterData(out fd);
					fd.maskBits = mask;
					fd.categoryBits = category;
					fl.SetFilterData(ref fd);

					fl = fl.GetNext();
				}
			}
		}
        public void SetGroupIndex(short value)
        {
            this.groupIndex = value;

            if (body != null)
			{
				Fixture fl = body.GetFixtureList();
				while (fl != null)
				{
					// note: can't change variables of structs that are properties
					Filter fd;
					fl.GetFilterData(out fd);
                    fd.groupIndex = value;
					fl.SetFilterData(ref fd);

					fl = fl.GetNext();
				}
            }
        }

		//public void SetB2DPosition(float x, float y)
		//{
		//    Vector2 go = parent.GetGlobalOffset(Vector2.Zero);
		//    XForm xf = body.GetXForm();
		//    body.SetXForm(new Vector2((x + go.X) / worldScale, (y + go.Y) / worldScale), xf.R.GetAngle());
		//    //body.SetXForm(new Vector2(x / worldScale, y / worldScale), Rotation);
		//}

        public override void Play()
        {
            if (isWrappedB2DObject())
            {
                ((DisplayObjectContainer)children[0]).isPlaying = true;
            }
            else
            {
                isPlaying = true;
            }
        }
        public override void Stop()
        {
            if (isWrappedB2DObject())
            {
                ((DisplayObjectContainer)children[0]).isPlaying = false;
            }
            else
            {
                isPlaying = false;
            }
        }
        public override void GotoAndPlay(uint frame)
        {
            if (isWrappedB2DObject())
            {
                DisplayObjectContainer doc = ((DisplayObjectContainer)children[0]);
                doc.CurChildFrame = frame < 0 ? 0 : frame > doc.LastChildFrame ? doc.LastChildFrame : frame;
                doc.isPlaying = true;
            }
            else
            {
                ((DisplayObjectContainer)children[0]).isPlaying = true;
                CurChildFrame = frame < 0 ? 0 : frame > LastChildFrame ? LastChildFrame : frame;
            }
        }
        public override void GotoAndStop(uint frame)
        {
            if (isWrappedB2DObject())
            {
                DisplayObjectContainer doc = ((DisplayObjectContainer)children[0]);
                doc.CurChildFrame = frame < 0 ? 0 : frame > doc.LastChildFrame ? doc.LastChildFrame : frame;
                doc.isPlaying = false;
            }
            else
            {
                ((DisplayObjectContainer)children[0]).isPlaying = false;
                CurChildFrame = frame < 0 ? 0 : frame > LastChildFrame ? LastChildFrame : frame;
            }
        }
        private bool isWrappedB2DObject()
        {
            return polygons.Count > 0 && LastChildFrame == 0 && children.Count == 1 && children[0] is DisplayObjectContainer;
        }

		public override void DestroyElement(DisplayObject obj)
		{
			base.DestroyElement(obj);
			if (obj is V2DSprite)
			{
				v2dScreen.DestroyBody(((V2DSprite)obj).body, obj.InstanceName);
			}
		}

        public virtual Body AddBodyInstanceToRuntime()
        {
			this.worldScale = v2dScreen.WorldScale;

            // box2D body
            if (this.polygons.Count > 0)
            {
                BodyDef bodyDef = new BodyDef();
				Vector2 pos = GetGlobalOffset(Vector2.Zero);
				bodyDef.position = new Vector2(pos.X / worldScale, pos.Y / worldScale);
				bodyDef.angle = GetGlobalRotation(0);
				bodyDef.fixedRotation = this.fixedRotation;
				bodyDef.angularDamping = this.angularDamping;
                bodyDef.linearDamping = this.linearDamping;

				//// todo: this needs to allow for nested levels
				//if (!fixedRotation)
				//{
				//    for (int i = 0; i < transforms.Length; i++)
				//    {
				//        transforms[i].Position = transforms[i].Position - pos + State.Position;
				//        transforms[i].Rotation = transforms[i].Rotation - bodyDef.Angle + State.Rotation;
				//        //this.transforms[i].Scale /= bodyDef.Scale;
				//    }
				//}

				if (attributeProperties != null)
				{
					attributeProperties.ApplyAttribtues(bodyDef);
				}
				body = v2dScreen.CreateBody(bodyDef);
				v2dScreen.bodies.Add(body);
                
                for (int i = 0; i < this.polygons.Count; i++)
                {
                    AddPoly(body, this.polygons[i]);
                }

                if (groupIndex != 0)
                {
                    SetGroupIndex(groupIndex);
                }

				IsStatic = isStatic;
				// todo: add kinematic
				if (isStatic)
				{
					body.SetType(BodyType.Static);
				}
				else
				{
					body.SetType(BodyType.Dynamic);
				}

				body.ResetMassData();
                body.SetUserData(this);
            }

            if (body != null)
            {
				//v2dScreen.bodyMap.Add(this.instanceName, body);
            }
            return body;
        }

        public void AddJointReference(Joint j)
        {
            jointRefs.Add(j);
        }
        public void RemoveJointReference(Joint j)
        {
            if (jointRefs.Contains(j))
            {
                jointRefs.Remove(j);
            }
        }
        public void ClearJointReferences()
        {
            jointRefs.Clear();
        }

        protected void AddPoly(Body body2Body, V2DShape polygon)
        {
            Shape shape;
            if (polygon.IsCircle)
            {
				CircleShape circDef = new CircleShape();
				circDef._radius = polygon.Radius / (worldScale * State.Scale.X);
                Vector2 lp = new Vector2(polygon.CenterX / worldScale, polygon.CenterY / worldScale);
                circDef._p = lp;
                shape = circDef;
            }
            else
            {
                float[] pts = polygon.Data;
				PolygonShape polyDef = new PolygonShape();
                shape = polyDef;
                polyDef._vertexCount = (int)(pts.Length / 2);

                for (int i = 0; i < polyDef._vertexCount; i++)
                {
                    float px = pts[i * 2];
                    float py = pts[i * 2 + 1];

                    polyDef._vertices[i] = new Vector2(
						px / worldScale * State.Scale.X,
						py / worldScale * State.Scale.Y);
                }
            }

			FixtureDef fd = new FixtureDef();
			fd.shape = shape;

            if (instanceName.IndexOf("s_") == 0)
            {
                isStatic = true;
				fd.density = 0.0f;
            }
            else
            {
				fd.density = density;
            }
            fd.friction = friction;
            fd.restitution = restitution;

            if (groupIndex != 0)
			{
				fd.filter.groupIndex = groupIndex;
			}

			if (attributeProperties != null)
			{
				attributeProperties.ApplyAttribtues(fd);
			}

            body.CreateFixture(fd);
        }
        protected override void ResetInstanceProperties()
        {
			base.ResetInstanceProperties();
            if (instanceDefinition != null)
            {
                this.polygons = instanceDefinition.Definition.V2DShapes;
                this.density = instanceDefinition.Density;
                this.friction = instanceDefinition.Friction;
                this.restitution = instanceDefinition.Restitution;
            }
        }

		public override void RemoveChild(DisplayObject o)
		{
			base.RemoveChild(o);
		}
		public override float X
		{
			get
			{
				return base.X;
			}
			set
			{
				hasXChange = true;

				//if (parent != null)
				//{
				//    float dif = X;
				//    base.X = value;
				//    dif = X - dif;
				//    foreach (DisplayObject obj in children)
				//    {
				//        obj.X += dif;
				//    }
				//}
				//else
				//{
					base.X = value;
				//}
			}
		}
		public override float Y
		{
			get
			{
				return base.Y;
			}
			set
			{
				base.Y = value;
				hasYChange = true;
			}
		}
		public override float Rotation
		{
			get
			{
				return base.Rotation;
			}
			set
			{
				base.Rotation = value;
				hasRChange = true;
			}
		}
		private bool hasXChange = true;
		private bool hasYChange = true;
		private bool hasRChange = true;
		public void UpdateTransform()
		{
			if (body != null && parent != null)
			{
				if (hasXChange || hasYChange || hasRChange)
				{
					V2DTransform t = transforms[transformIndex];

					if (hasXChange || hasYChange)
					{
						Vector2 newPos = body.GetPosition();
						if (hasXChange)
						{
							//newPos.X = (State.Position.X + parent.CurrentState.Position.X + t.Position.X) / worldScale;
							newPos.X = (State.Position.X + parent.CurrentState.Position.X) / worldScale;
						}
						if (hasYChange)
						{
							//newPos.Y = (State.Position.Y + parent.CurrentState.Position.Y + t.Position.Y) / worldScale;
							newPos.Y = (State.Position.Y + parent.CurrentState.Position.Y) / worldScale;
						}
						body.Position = newPos;
					}

					if (hasRChange)
					{
						body.Rotation = State.Rotation - parent.CurrentState.Rotation + t.Rotation;
					}


					//Vector2 v = new Vector2(newPos.X, newPos.Y);
					//State.Position = (v *  worldScale) - parent.CurrentState.Position - t.Position;
					//State.Scale = CurrentState.Scale;
					//State.Rotation = rot - parent.CurrentState.Rotation;
					//State.Origin = CurrentState.Origin;

					//foreach (DisplayObject obj in children)
					//{
					//    if (obj is V2DSprite)
					//    {
					//        V2DSprite sp = (V2DSprite)obj;
					//        sp.hasRChange = hasRChange;
					//        sp.hasXChange = hasXChange;
					//        sp.hasYChange = hasYChange;
					//    }
					//}

					hasXChange = false;
					hasYChange = false;
					hasRChange = false;
				}
			}
		}
		//protected void NeedsTransformUpdate()
		//{
		//    if (!v2dScreen.transformList.Contains(this))
		//    {
		//       v2dScreen.transformList.Add(this);
		//    }
		//}
		protected override void SetCurrentState()
		{
			if (body == null)
			{
				base.SetCurrentState();
			}
			else// 
			{
				for (int i = 0; i < transforms.Length; i++)
				{
					if (transforms[i].StartFrame <= parent.CurChildFrame && transforms[i].EndFrame >= (parent.CurChildFrame))
					{
						transformIndex = i;
						break;
					}
				}
				V2DTransform t = transforms[transformIndex];

				if (hasRChange || hasXChange || hasYChange)
				{
					UpdateTransform();
				}

				Vector2 bPosition = new Vector2((int)(body.GetPosition().X * WorldScale), (int)(body.GetPosition().Y * WorldScale));
				float br = body.GetAngle();

				CurrentState.Position = bPosition;// -parent.CurrentState.Position;// +t.Position;
				CurrentState.Scale = parent.CurrentState.Scale * State.Scale * t.Scale;
				CurrentState.Rotation = br;// -parent.CurrentState.Rotation + t.Rotation;
				CurrentState.Origin = State.Origin;

				//State.Position = CurrentState.Position - parent.CurrentState.Position - t.Position;
				State.Position = CurrentState.Position - parent.CurrentState.Position;
				//State.Scale = CurrentState.Scale;
				State.Rotation = CurrentState.Rotation - parent.CurrentState.Rotation;
				//State.Origin = CurrentState.Origin;
			}
		}

		//public override void Update(GameTime gameTime)
		//{
		//    base.Update(gameTime);
		//    if (hasRChange || hasXChange || hasYChange)
		//    {
		//        NeedsTransformUpdate();
		//    }
		//}
    }
}