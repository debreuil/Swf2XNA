using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Dynamics;
using DDW.V2D.Serialization;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Common;
using Microsoft.Xna.Framework.Graphics;
using DDW.Display;
using Microsoft.Xna.Framework.Input;
using DDW.Input;
using V2DRuntime.Enums;

namespace DDW.V2D
{
    public class V2DStage : Stage
    {
        private World world;
        public V2DWorld v2dWorld;
        public int Iterations = 10;
        public float TimeStep = 1 / 30;
        public float WorldScale = 15;
		public Vec2 Gravity = new Vec2(0.0f, 10.0f);
        public Vector2 ClientSize = new Vector2(400, 300);

		public float hz = 60.0f;
		public int velocityIterations = 10;
		public int positionIterations = 8;
		public int enableWarmStarting = 1;
		public int enableTOI = 1;
        public bool pause = false;
        public bool singleStep = false;	        
        Cursor cursor;
        internal MouseJoint _mouseJoint;

        protected V2DStage()
        {
        }

        private static V2DStage stageInstance;
        public static V2DStage GetInstance()
        {
            if (stageInstance == null)
            {
                stageInstance = new V2DStage();
            }
            return stageInstance;
        }

        public override void Initialize()
        {
            base.Initialize();

            bool doSleep = true;
            float w = ClientSize.X / WorldScale;
            float h = ClientSize.Y / WorldScale;
            AABB worldAABB = new AABB();
            worldAABB.LowerBound.Set(0 - 500, (-h / 2) - 500);
            worldAABB.UpperBound.Set((w / 2) + 500, (h / 2) + 500);

            world = new World(worldAABB, Gravity, doSleep);
            SetBounds(0, 0, ClientSize.X, ClientSize.Y);

            if (V2DGame.instance.HasCursor)
            {
                cursor = V2DGame.instance.GetCursor();
                cursor.MouseDown += MouseDown;
                cursor.MouseMove += MouseMove;
                cursor.MouseUp += MouseUp;
            }
        }

        public void MouseDown(Vector2 position)
        {
            if (_mouseJoint != null)
            {
                return;
            }

            Vec2 p = new Vec2(position.X / WorldScale, position.Y / WorldScale);
            // Make a small box.
            AABB aabb = new AABB();
            Vec2 d = new Vec2();
            d.Set(0.001f, 0.001f);
            aabb.LowerBound = p - d;
            aabb.UpperBound = p + d;

            // Query the world for overlapping shapes.
            int k_maxCount = 10;
            Shape[] shapes = new Shape[k_maxCount];
            int count = world.Query(aabb, shapes, k_maxCount);
            Body bd = null;
            for (int i = 0; i < count; ++i)
            {
                Body shapeBody = shapes[i].GetBody();
                if (shapeBody.IsStatic() == false && shapeBody.GetMass() > 0.0f)
                {
                    bool inside = shapes[i].TestPoint(shapeBody.GetXForm(), p);
                    if (inside)
                    {
                        bd = shapes[i].GetBody();
                        break;
                    }
                }
            }

            if (bd != null)
            {
                MouseJointDef md = new MouseJointDef();
                md.Body1 = world.GetGroundBody();
                md.Body2 = bd;
                md.Target = p;
                md.MaxForce = 1000.0f * bd.GetMass();
                _mouseJoint = (MouseJoint)world.CreateJoint(md);
                bd.WakeUp();
            }
        }
        public void MouseUp(Vector2 position)
        {
            if (_mouseJoint != null)
            {
                world.DestroyJoint(_mouseJoint);
                _mouseJoint = null;
            }
        }
        public void MouseMove(Vector2 position)
        {
            if (_mouseJoint != null)
            {
                Vec2 p = new Vec2(position.X / WorldScale, position.Y / WorldScale);
                _mouseJoint.SetTarget(p);
            }
        }

        Body[] boundsBodies = new Body[4];
        public void SetBounds(float x, float y, float w, float h)
        {
			foreach (Body b in boundsBodies)
			{
				if (world.Contains(b))
				{
					world.DestroyBody(b);
				}
			}            

            float overlap = 10;
            float thickness = 100;

            boundsBodies[0] = CreateBox(-overlap, -thickness, w + overlap * 2, thickness);
            boundsBodies[1] = CreateBox(-overlap, h, w + overlap * 2, thickness);
            boundsBodies[2] = CreateBox(-thickness, -overlap, thickness, h + overlap * 2);
            boundsBodies[3] = CreateBox(w, -overlap, thickness, h + overlap * 2);

			boundsBodies[0].SetUserData(EdgeName.Top);
			boundsBodies[1].SetUserData(EdgeName.Bottom);
			boundsBodies[2].SetUserData(EdgeName.Left);
			boundsBodies[3].SetUserData(EdgeName.Right);

        }
        protected Body CreateBox(float x, float y, float w, float h)
        {
            x /= WorldScale;
            y /= WorldScale;
            w /= WorldScale;
            h /= WorldScale;

            BodyDef bodyDef = new BodyDef();
            bodyDef.Position.Set(x, y);

            PolygonDef polyDef = new PolygonDef();
            polyDef.VertexCount = 4;
            polyDef.Vertices[0].Set(0, 0);
            polyDef.Vertices[1].Set(w, 0);
            polyDef.Vertices[2].Set(w, h);
            polyDef.Vertices[3].Set(0, h);

            Body body = world.CreateBody(bodyDef);
            body.CreateShape(polyDef);
            body.SetMassFromShapes();

            return body;
		}

        public override void Update(GameTime gameTime)
        {
            float timeStep = hz > 0.0f ? 1.0f / hz : 0.0f;
            if (pause)
            {
                if (singleStep)
                {
                    singleStep = false;
                }
                else
                {
                    timeStep = 0.0f;
                }
            }

            world.SetWarmStarting(enableWarmStarting > 0);
            world.SetContinuousPhysics(enableTOI > 0);

            world.Step(timeStep, velocityIterations, positionIterations);

            world.Validate();

            Body b = world.GetBodyList();
            while (b != null)
            {
                if(b.GetUserData() is V2DSprite)
                {
                    V2DSprite s = (V2DSprite)b.GetUserData();
                    Vector2 offset = s.Parent.GetGlobalOffset(Vector2.Zero); //Vector2.Zero;// 
                    s.X = (int)(b.GetPosition().X * WorldScale - offset.X);
                    s.Y = (int)(b.GetPosition().Y * WorldScale - offset.Y);
                    s.Rotation = b.GetAngle();
                }
                b = b.GetNext();
            }

            base.Update(gameTime);
        }

		public override void AddChild(DisplayObject o)
		{
			if (o is V2DScreen)
			{
				((V2DScreen)o).world = world;
			}
			base.AddChild(o);
		}
        internal override void ObjectAddedToStage(DisplayObject o)
        {
            if (o is V2DScreen)
            {
                V2DScreen scr = (V2DScreen)o;
                scr.Activate(world);
            }
            else if (o is V2DSprite)
            {
                V2DSprite sp = (V2DSprite)o;
                sp.AddBodyInstanceToRuntime();
            }

            base.ObjectAddedToStage(o);
        }
        internal override void ObjectRemovedFromStage(DisplayObject o)
        {
            base.ObjectRemovedFromStage(o);

            if (o is V2DScreen)
            {
                V2DScreen scr = (V2DScreen)o;
                scr.Deactivate();
            }
			//else if (o is V2DSprite)
			//{
			//    if (o.Parent != null)
			//    {
			//        V2DSprite sp = (V2DSprite)o;
			//        sp.RemoveBodyInstanceFromRuntime();
			//    }
			//}
        }
    }
}
