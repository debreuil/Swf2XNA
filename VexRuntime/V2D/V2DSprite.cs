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
using VexRuntime.V2D;

namespace DDW.V2D
{
    public class V2DSprite : Sprite
    {
        protected V2DInstance instance;
        public Body body;
        protected List<Joint> jointRefs = new List<Joint>();
        protected float worldScale;
        protected float density;
        protected float friction;
        protected float restitution;
        protected bool isStatic;
        protected short groupIndex = 0;
        protected bool fixedRotation;
        protected List<V2DShape> polygons = new List<V2DShape>();

        protected static short groupIndexCounter = 1;


        public V2DSprite(Texture2D texture, V2DInstance instance) : base(texture)
        {
            this.texture = texture;
            this.instance = instance;
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

        public void RemoveInstanceFromRuntime()
        {
            Screen scr = GetContainerScreen(this);
            if (scr != null && scr is V2DScreen)
            {
                V2DScreen screen = (V2DScreen)scr;

                for (int i = 0; i < jointRefs.Count; i++)
                {
                    screen.RemoveJoint(jointRefs[i]);
                }

                if (body != null)
                {
                    screen.world.DestroyBody(body);
                    screen.bodyMap.Remove(this.instanceName);
                }

                body = null;
                jointRefs.Clear();
            }
        }
        public Body AddInstanceToRuntime()
        {
            Screen scr = GetContainerScreen(this);
            if (scr != null && scr is V2DScreen)
            {
                V2DScreen screen = (V2DScreen)scr;
                this.worldScale = screen.worldScale;

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

                    body = screen.world.CreateBody(bodyDef);
                    screen.bodies.Add(body);

                    for (int i = 0; i < this.polygons.Count; i++)
                    {
                        AddPoly(body, this.polygons[i]);
                    }
                    if (groupIndex != 0)
                    {
                        SetGroupIndex(groupIndex);
                    }
                    body.SetMassFromShapes();
                    body.SetUserData(this);
                }
                else
                {
                    //view.X = this.X;
                    //view.Y = this.Y;
                    //view.Rotation = (float)(this.rotation / System.Math.PI * 180);
                }

                if (body != null)
                {
                    screen.bodyMap.Add(this.instanceName, body);
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
                    polyDef.Vertices[i].Set(
                        pts[i * 2] / worldScale * scale.X,
                        pts[i * 2 + 1] / worldScale * scale.Y);
                }
            }

            if (instanceName.IndexOf("s_") == 0)
            {
                isStatic = true;
                sd.Density = 0.0F;
            }
            else
            {
                isStatic = false;
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
            body.CreateShape(sd);
        }
        protected void ResetInstanceProperties()
        {
            if (instance != null)
            {
                if (texture != null)
                {
                    this.destinationRectangle = new V2DRectangle((int)instance.X, (int)instance.Y, texture.Width, texture.Height);
                }
                else
                {
                    this.destinationRectangle = new V2DRectangle((int)instance.X, (int)instance.Y, 0, 0);
                }
                this.origin = new Vector2(-instance.Definition.OffsetX, -instance.Definition.OffsetY);
                this.instanceName = instance.InstanceName;
                this.definitonName = instance.DefinitionName;
                this.rotation = instance.Rotation;
                this.scale = new Vector2(instance.ScaleX, instance.ScaleY);
                this.alpha = instance.Alpha;
                this.visible = instance.Visible;
                this.transforms = instance.Transforms;
                this.polygons = instance.Definition.V2DShapes;
                this.density = instance.Density;
                this.friction = instance.Friction;
                this.restitution = instance.Restitution;
                this.StartFrame = instance.StartFrame;
                this.TotalFrames = instance.TotalFrames;
            }
        }
    }
}
