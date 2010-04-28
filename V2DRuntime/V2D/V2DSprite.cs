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
        public override void CreateView()
        {
            base.CreateView();
            if (this.body == null)
            {
                this.AddBodyInstanceToRuntime();
            }
        }
        public virtual Body AddBodyInstanceToRuntime()
        {
            // box2D body
            if (this.polygons.Count > 0)
            {
                BodyDef bodyDef = new BodyDef();
				Vector2 pos = GetGlobalOffset(Vector2.Zero);
				bodyDef.position = new Vector2(pos.X / V2DScreen.WorldScale, pos.Y / V2DScreen.WorldScale);
				bodyDef.angle = GetGlobalRotation(0);
				bodyDef.fixedRotation = this.fixedRotation;
				bodyDef.angularDamping = this.angularDamping;
                bodyDef.linearDamping = this.linearDamping;

				IsStatic = isStatic;
				// todo: add kinematic
				if (isStatic)
				{
					bodyDef.type = BodyType.Static;
				}
				else
				{
					bodyDef.type = BodyType.Dynamic;
				}

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
                body.SetUserData(this);
                
                for (int i = 0; i < this.polygons.Count; i++)
                {
                    AddPoly(body, this.polygons[i]);
				}
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
				circDef._radius = polygon.Radius / (V2DScreen.WorldScale * State.Scale.X);
				Vector2 lp = new Vector2(polygon.CenterX / V2DScreen.WorldScale, polygon.CenterY / V2DScreen.WorldScale);
                circDef._p = lp;
                shape = circDef;
            }
            else
            {
                float[] pts = polygon.Data;
				PolygonShape polyDef = new PolygonShape();
                shape = polyDef;
                int len= (int)(pts.Length / 2);
				Vector2[] v2s = new Vector2[len];

                for (int i = 0; i < len; i++)
                {
                    float px = pts[i * 2];
                    float py = pts[i * 2 + 1];

					v2s[i] = new Vector2(
						px / V2DScreen.WorldScale * State.Scale.X,
						py / V2DScreen.WorldScale * State.Scale.Y);
                }
				polyDef.Set(v2s, len);
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
							newPos.X = (State.Position.X + parent.CurrentState.Position.X) / V2DScreen.WorldScale;
						}
						if (hasYChange)
						{
							//newPos.Y = (State.Position.Y + parent.CurrentState.Position.Y + t.Position.Y) / worldScale;
							newPos.Y = (State.Position.Y + parent.CurrentState.Position.Y) / V2DScreen.WorldScale;
						}
						body.Position = newPos;
					}

					if (hasRChange)
					{
						body.Rotation = State.Rotation - parent.CurrentState.Rotation + t.Rotation;
					}

					hasXChange = false;
					hasYChange = false;
					hasRChange = false;
				}
			}
		}
		protected override void SetCurrentState()
		{
			if (body == null)
			{
				base.SetCurrentState();
			}
			else
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

				Vector2 bPosition = new Vector2((int)(body.GetPosition().X * V2DScreen.WorldScale), (int)(body.GetPosition().Y * V2DScreen.WorldScale));
				float br = body.GetAngle();

				CurrentState.Position = bPosition;// -parent.CurrentState.Position;// +t.Position;
				CurrentState.Scale = parent.CurrentState.Scale * State.Scale * t.Scale;
				CurrentState.Rotation = br;// -parent.CurrentState.Rotation + t.Rotation;
				CurrentState.Origin = State.Origin;

				//State.Position = CurrentState.Position - parent.CurrentState.Position - t.Position;
				State.Position = CurrentState.Position - parent.CurrentState.Position;
				//State.Scale = CurrentState.Scale;
				State.Rotation = CurrentState.Rotation - parent.CurrentState.Rotation;
				State.Origin = CurrentState.Origin;
			}
		}
    }
}
