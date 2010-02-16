using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DDW.Display;
using Microsoft.Xna.Framework;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using V2DRuntime.V2D;
using V2DRuntime.Attributes;

namespace DDW.V2D
{
    public class V2DSprite : Sprite
    {
		public Body body;
        protected List<Joint> jointRefs = new List<Joint>();
        protected float worldScale;
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


        public V2DSprite(Texture2D texture, V2DInstance instance) : base(texture)
        {
            this.texture = texture;
            this.instanceDefinition = instance;
            //this.groupIndex = groupIndexCounter++;

            if (texture != null)
            {
                this.sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            }

            ResetInstanceProperties();
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
                    Shape s = body.GetShapeList();
                    while (s != null)
                    {
                        s.Density = 0;
                        s = s.GetNext();
                    }

                    body.SetMassFromShapes();
                }
            }
		}
		public void SetMaskAndCategoryBits(ushort mask, ushort category)
		{
			if (body != null)
			{
				Shape sh = body.GetShapeList();
				while (sh != null)
				{
					// note: can't change variables of structs that are properties
					FilterData fd = new FilterData();
					fd.GroupIndex = sh.FilterData.GroupIndex;
					fd.MaskBits = mask;
					fd.CategoryBits = category;
					sh.FilterData = fd;

					sh = sh.GetNext();
				}
			}
		}
        public void SetGroupIndex(short value)
        {
            this.groupIndex = value;

            if (body != null)
            {
                Shape sh = body.GetShapeList();
                while (sh != null)
                {
                    // note: can't change variables of structs that are properties
                    FilterData fd = new FilterData();
                    fd.GroupIndex = value;
                    fd.MaskBits = sh.FilterData.MaskBits;
                    fd.CategoryBits = sh.FilterData.CategoryBits;
                    sh.FilterData = fd;

                    sh = sh.GetNext();
                }
            }
        }

		public void SetB2DPosition(float x, float y)
		{
			body.SetXForm(new Vec2(x / worldScale, y / worldScale), Rotation);
		}

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

        public void RemoveBodyInstanceFromRuntime()
        {
			if(screen is V2DScreen)
			{
				V2DScreen vscreen = (V2DScreen)screen;
                for (int i = 0; i < jointRefs.Count; i++)
                {
					vscreen.RemoveJoint(jointRefs[i]);
                }

				if (body != null && vscreen.bodyMap.ContainsKey(this.instanceName))
                {
					vscreen.DestroyBody(body, this.instanceName);
                }

                body = null;
                jointRefs.Clear();
            }
        }
        public virtual Body AddBodyInstanceToRuntime()
        {
			if(screen is V2DScreen)
			{
				V2DScreen vscreen = (V2DScreen)screen;
				this.worldScale = vscreen.worldScale;

                // box2D body
                if (this.polygons.Count > 0)
                {
                    BodyDef bodyDef = new BodyDef();
					float localX = 0;
					float localY = 0;
					LocalToGlobal(ref localX, ref localY);
					bodyDef.Position.Set(localX / worldScale, localY / worldScale);
					bodyDef.Angle = this.rotation;
					bodyDef.FixedRotation = this.fixedRotation;
					bodyDef.AngularDamping = this.angularDamping;
                    bodyDef.LinearDamping = this.linearDamping;

                    if (!fixedRotation &&
                        rotation != 0 && 
                        this.transforms != null && 
                        this.transforms.Length > 0 && 
                        this.transforms[0].Rotation == this.rotation)
                    {
                        for (int i = 0; i < transforms.Length; i++)
                        {
                            this.transforms[0].Rotation -= rotation;
                        }
                    }

					if (attributeProperties != null)
					{
						attributeProperties.ApplyAttribtues(bodyDef);
					}

					body = vscreen.CreateBody(bodyDef);
					vscreen.bodies.Add(body);
                    
                    for (int i = 0; i < this.polygons.Count; i++)
                    {
                        AddPoly(body, this.polygons[i]);
                    }

                    if (groupIndex != 0)
                    {
                        SetGroupIndex(groupIndex);
                    }

					if(isStatic != false)
					{
						IsStatic = isStatic;
					}

                    body.SetMassFromShapes();
                    body.SetUserData(this);
                }

                if (body != null)
                {
					vscreen.bodyMap.Add(this.instanceName, body);
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
            ShapeDef sd;
            if (polygon.IsCircle)
            {
                CircleDef circDef = new CircleDef();
                circDef.Radius = polygon.Radius / (worldScale * scale.X);
                Vec2 lp = new Vec2();
                lp.Set(polygon.CenterX / worldScale, polygon.CenterY / worldScale);
                circDef.LocalPosition = lp;
                sd = circDef;
            }
            else
            {
                float[] pts = polygon.Data;
                PolygonDef polyDef = new PolygonDef();
                sd = polyDef;
                polyDef.VertexCount = (int)(pts.Length / 2);

                for (int i = 0; i < polyDef.VertexCount; i++)
                {
                    float px = pts[i * 2];
                    float py = pts[i * 2 + 1];

                    polyDef.Vertices[i].Set(
                        px / worldScale * scale.X,
                        py / worldScale * scale.Y);
                }
            }

            if (instanceName.IndexOf("s_") == 0)
            {
                isStatic = true;
                sd.Density = 0.0F;
            }
            else
            {
                sd.Density = this.density;
            }
            sd.Friction = this.friction;
            sd.Restitution = this.restitution;

            if (groupIndex != 0)
            {
                sd.Filter.GroupIndex = groupIndex;
                // note: can't change variables of structs that are properties
                FilterData fd = new FilterData();
                fd.GroupIndex = groupIndex;
                fd.MaskBits = sd.Filter.MaskBits;
                fd.CategoryBits = sd.Filter.CategoryBits;
                sd.Filter = fd;
			}

			if (attributeProperties != null)
			{
				attributeProperties.ApplyAttribtues(sd);
			}

            body.CreateShape(sd);
        }
        protected void ResetInstanceProperties()
        {
            if (instanceDefinition != null)
            {
                if (texture != null)
                {
                    //this.destinationRectangle = new V2DRectangle(0, 0, texture.Width, texture.Height);
                    this.destinationRectangle = new V2DRectangle((int)instanceDefinition.X, (int)instanceDefinition.Y, texture.Width, texture.Height);
                }
                else
                {
                    //this.destinationRectangle = new V2DRectangle(0, 0, 0, 0);
                    this.destinationRectangle = new V2DRectangle((int)instanceDefinition.X, (int)instanceDefinition.Y, 0, 0);
                }
                this.origin = new Vector2(-instanceDefinition.Definition.OffsetX, -instanceDefinition.Definition.OffsetY);
				this.InstanceName = instanceDefinition.InstanceName;
                this.DefinitionName = instanceDefinition.DefinitionName;
                this.rotation = instanceDefinition.Rotation;
                this.scale = new Vector2(instanceDefinition.ScaleX, instanceDefinition.ScaleY);
                this.alpha = instanceDefinition.Alpha;
                this.visible = instanceDefinition.Visible;
                this.Depth = instanceDefinition.Depth;

                // normalize all transforms to base position
                this.transforms = new V2DTransform[instanceDefinition.Transforms.Length];
                float ox = instanceDefinition.X;
                float oy = instanceDefinition.Y;
                for (int i = 0; i < instanceDefinition.Transforms.Length; i++)
                {
                    this.transforms[i] = instanceDefinition.Transforms[i].Clone();
                    this.transforms[i].TranslationX -= ox;
                    this.transforms[i].TranslationY -= oy;
                }
                this.polygons = instanceDefinition.Definition.V2DShapes;
                this.density = instanceDefinition.Density;
                this.friction = instanceDefinition.Friction;
                this.restitution = instanceDefinition.Restitution;
                this.StartFrame = instanceDefinition.StartFrame;
                this.EndFrame = instanceDefinition.EndFrame;
            }
        }

		public override void RemoveChild(DisplayObject o)
		{
			base.RemoveChild(o);
			RemoveBodyInstanceFromRuntime();
		}
    }
}