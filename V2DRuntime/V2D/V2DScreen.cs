using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Box2D.XNA;

using DDW.Display;
using DDW.V2D.Serialization;
using DDW.Input;
using V2DRuntime.Components;
using V2DRuntime.V2D;
using V2DRuntime.Enums;
using V2DRuntime.Attributes;

namespace DDW.V2D
{
    [XmlRoot]
	public class V2DScreen : Screen, IJointable
    {
        public World world;
		protected V2DScreen v2dScreen;
		public Body groundBody;

        public Dictionary<string, Body> bodyMap = new Dictionary<string, Body>();
        public List<Body> bodies = new List<Body>();
        public List<Joint> joints = new List<Joint>();
		//private V2DInstance rootInstance;

		Cursor cursor;
		internal MouseJoint _mouseJoint;
		public float hz = 60.0f;
		public bool singleStep = false;
		public int velocityIterations = 10;
		public int positionIterations = 8;
		public int enableWarmStarting = 1;
		public int enableTOI = 1;
		public int Iterations = 10;
		public float TimeStep = 1 / 30;
		public Vector2 Gravity = new Vector2(0.0f, 10.0f); 
        
        public V2DScreen()
        {
			CreateWorld();
        }
        public V2DScreen(SymbolImport symbolImport) : base(symbolImport)
        {
			this.SymbolImport = symbolImport;
			CreateWorld();
        }
        public V2DScreen(V2DContent v2dContent) : base(v2dContent)
        {
			CreateWorld();
        }

		private float worldScale = 15;
		public float WorldScale { get { return worldScale; } set { worldScale = value; } }
		public V2DScreen VScreen { get { return v2dScreen; } set { v2dScreen = value; } }

		public override Sprite CreateDefaultObject(Texture2D texture, V2DInstance inst)
		{
			Sprite result;
			if (parent is V2DSprite || parent is V2DScreen)
			{
				result = new V2DSprite(texture, inst);
			}
			else
			{
				result = new Sprite(texture, inst);
			}
			return result;
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
		public override void RemoveChild(DisplayObject obj)
		{
			base.RemoveChild(obj);
			if (obj is V2DSprite)
			{
				V2DSprite sp = (V2DSprite)obj;
				if(sp.body != null)
				{
					DestroyBody(sp.body, obj.InstanceName);
					sp.body.SetUserData(null);
				}
			}
		}
		public override void Initialize()
		{
			base.Initialize();

			V2DDefinition def = v2dWorld.GetDefinitionByName(this.definitionName);
			if (def != null)
			{
				for (int i = 0; i < def.Joints.Count; i++)
				{
					this.AddJoint(def.Joints[i], this.X, this.Y);
				}
			}
			
			System.Reflection.MemberInfo inf = this.GetType();			
			System.Attribute[] attrs = System.Attribute.GetCustomAttributes(inf);  // reflection
			foreach (System.Attribute attr in attrs)
			{
				if (attr is V2DScreenAttribute)
				{
					V2DScreenAttribute a = (V2DScreenAttribute)attr;
					if (a.gravityX != 0 | a.gravityY != 10)
					{
						Gravity = new Vector2(a.gravityX, a.gravityY);
						world.Gravity = Gravity;
					}
				}
			}
		}
		private Box2D.XNA.TestBed.Framework.DebugDraw _debugDraw;
		private void CreateWorld()
		{
			if (world == null)
			{
				bool doSleep = true;
				ClientSize = new Vector2(v2dWorld.Width, v2dWorld.Height);

				//float w = ClientSize.X / WorldScale;
				//float h = ClientSize.Y / WorldScale;
				//float t = (-h / 2) - 500f;
				//float l = -500f;
				//float b = (h / 2f) + 500f;
				//float r = (w / 2f) + 500f;

				world = new World(Gravity, doSleep);
				BodyDef groundBodyDef = new BodyDef();
				groundBodyDef.position = new Vector2(0f, 0f);
				groundBody = world.CreateBody(groundBodyDef);

				bodyMap.Add(V2DGame.ROOT_NAME, groundBody);
				if (instanceName != V2DGame.ROOT_NAME)
				{
					bodyMap.Add(this.instanceName, groundBody);
				}
			}
		}
		private void DestroyWorld()
		{
			// clear box2d
			Body b = world.GetBodyList();
			while (b != null)
			{
				DestroyBody(b, "");
				b = b.GetNext();
			}
			joints.Clear();
			bodyMap.Clear();
			world = null;
		}
		public override void Added(EventArgs e)
		{
			base.Added(e);
			if (V2DGame.instance.HasCursor)
			{
				cursor = V2DGame.instance.GetCursor();
				cursor.MouseDown += MouseDown;
				cursor.MouseMove += MouseMove;
				cursor.MouseUp += MouseUp;
			}
		}
        public override void Removed(EventArgs e)
        {
            base.Removed(e);
			if (V2DGame.instance.HasCursor)
			{
				cursor = V2DGame.instance.GetCursor();
				cursor.MouseDown -= MouseDown;
				cursor.MouseMove -= MouseMove;
				cursor.MouseUp -= MouseUp;
			}
        }

        public void  RemoveJoint(Joint joint)
        {	
            if(joints.Contains(joint))
            {
                joints.Remove(joint);
                world.DestroyJoint(joint);
            }
        }
        protected void  RemoveJointByName(string name)
        {				
            for(int i = joints.Count - 1; i >= 0; i--)
            {
                if((string)joints[i].GetUserData() == name)
                {
                    RemoveJoint(joints[i]);
                    break;    
                }
            }	  
        }
        public virtual Body GetBodyByName(string name)
        {
            Body result = null;
            for (int i = 0; i < bodies.Count; i++)
		    {
                object o = bodies[i].GetUserData();
                if(o is DisplayObject)
                {
                    if ( ((DisplayObject)o).InstanceName == name)
                    {
                        result = bodies[i];
                        break;
                    }
                }
		    }
            return result;
        }

		public override void DestroyElement(DisplayObject obj)
		{
			base.DestroyElement(obj);
			if (obj is V2DSprite)
			{
				DestroyBody(((V2DSprite)obj).body, obj.InstanceName);
			}
		}
		public Body CreateBody(BodyDef bodyDef)
		{
			return world.CreateBody(bodyDef);
		}
		public void DestroyBody(Body b, string name)
		{
			if (bodyMap.ContainsKey(name))//world.Contains(b))
			{
				//List<Joint> relatedJoints = new List<Joint>();
				//for (int j = 0; j < joints.Count; j++)
				//{
				//    if (joints[j].GetBodyA() == b || joints[j].GetBodyB() == b)
				//    {
				//        //joints.RemoveAt(j);
				//        relatedJoints.Add(joints[j]);
				//    }
				//}

				//for (int j = relatedJoints.Count - 1; j >= 0; j--)
				//{
				//    if (joints.Contains(relatedJoints[j]))
				//    {
				//        world.DestroyJoint(relatedJoints[j]);
				//        joints.Remove(relatedJoints[j]);
				//    }
				//}

				world.DestroyBody(b);
				this.bodies.Remove(b);

				//if (bodyMap.ContainsKey(name))
				//{
					bodyMap.Remove(name);
				//}
			}
		}
		public void SetGravity(Vector2 v2)
		{
			world.Gravity = v2;
		}
		public void SetContactListener(IContactListener contactListener)
		{
			world.ContactListener = contactListener;
		}
		public void ClearContactListener()
		{
			world.ContactListener = null;
		}

		public virtual void MouseDown(Vector2 p)
		{
			if (_mouseJoint != null)
			{
				return;
			}

			p /= worldScale;
			// Make a small box.
			AABB aabb;
			Vector2 d = new Vector2(0.001f, 0.001f);
			aabb.lowerBound = p - d;
			aabb.upperBound = p + d;

			Fixture _fixture = null;

			// Query the world for overlapping shapes.
			world.QueryAABB(
				(fixture) =>
				{
					Body body = fixture.GetBody();
					if (body.GetType() == BodyType.Dynamic)
					{
						bool inside = fixture.TestPoint(p);
						if (inside)
						{
							_fixture = fixture;

							// We are done, terminate the query.
							return false;
						}
					}

					// Continue the query.
					return true;
				}, ref aabb);

			if (_fixture != null)
			{
				Body body = _fixture.GetBody();
				MouseJointDef md = new MouseJointDef();
				md.bodyA = groundBody;
				md.bodyB = body;
				md.target = p;
				md.maxForce = 1000.0f * body.GetMass();
				_mouseJoint = (MouseJoint)world.CreateJoint(md);
				body.SetAwake(true);
			}
		}
		public virtual void MouseUp(Vector2 p)
		{
			if (_mouseJoint != null)
			{
				world.DestroyJoint(_mouseJoint);
				_mouseJoint = null;
			}
		}
		public void MouseMove(Vector2 p)
		{
			p /= worldScale;
			if (_mouseJoint != null)
			{
				_mouseJoint.SetTarget(p);
			}
		}

		Body[] boundsBodies = new Body[4];

		protected void ClearBoundsBodies()
		{
			foreach (Body b in boundsBodies)
			{
				if (b != null)// && world.Contains(b))
				{
					world.DestroyBody(b);
				}
			}
		}
		public override void SetBounds(float x, float y, float w, float h)
		{
			ClearBoundsBodies();
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
			bodyDef.position = new Vector2(x, y);

			PolygonShape polyShape = new PolygonShape();
			polyShape._vertexCount = 4;
			polyShape._vertices[0] = new Vector2(0, 0);
			polyShape._vertices[1] = new Vector2(w, 0);
			polyShape._vertices[2] = new Vector2(w, h);
			polyShape._vertices[3] = new Vector2(0, h);

			Body body = world.CreateBody(bodyDef);
			body.CreateFixture(polyShape);

			MassData md;
			body.GetMassData(out md);
			body.SetMassData(ref md);

			return body;
		}
		public override void DrawDebugData(SpriteBatch batch)
		{
			base.DrawDebugData(batch);

			simpleColorEffect.Begin();
			simpleColorEffect.Techniques[0].Passes[0].Begin();
			_debugDraw.FinishDrawShapes();
			simpleColorEffect.Techniques[0].Passes[0].End();
			simpleColorEffect.End();
		}
		public override void Update(GameTime gameTime)
		{
			if (isActive)
			{
				base.Update(gameTime);
				float timeStep = hz > 0.0f ? 1.0f / hz : 0.0f;

				if (stage.pause)
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

				world.WarmStarting = enableWarmStarting > 0;
				world.ContinuousPhysics = enableTOI > 0;

				world.Step(timeStep, velocityIterations, positionIterations);
				world.DrawDebugData();

				//world.Validate();

				//Body b = world.GetBodyList();
				//while (b != null)
				//{
				//    if (b.GetUserData() is V2DSprite)
				//    {
				//        V2DSprite s = (V2DSprite)b.GetUserData();
				//        Vector2 offset = s.Parent.GetGlobalOffset(Vector2.Zero); //Vector2.Zero;// 
				//        s.X = (int)(b.GetPosition().X * WorldScale - offset.X);
				//        s.Y = (int)(b.GetPosition().Y * WorldScale - offset.Y);
				//        s.Rotation = b.GetAngle();
				//    }
				//    b = b.GetNext();
				//}
			}
		}
		private BasicEffect simpleColorEffect;
		private bool firstTime = true;
		public override void Draw(SpriteBatch batch)
		{
			if (isActive)
			{
				base.Draw(batch);
				if (firstTime)
				{
					_debugDraw = new Box2D.XNA.TestBed.Framework.DebugDraw();
					_debugDraw.AppendFlags(DebugDrawFlags.AABB | DebugDrawFlags.CenterOfMass | DebugDrawFlags.Joint | DebugDrawFlags.Pair | DebugDrawFlags.Shape);
					world.DebugDraw = _debugDraw;
					simpleColorEffect = new BasicEffect(batch.GraphicsDevice, null);
					simpleColorEffect.VertexColorEnabled = true;
					simpleColorEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographicOffCenter(0, ClientSize.X / WorldScale, ClientSize.Y / WorldScale, 0, -1, 1));

					Box2D.XNA.TestBed.Framework.DebugDraw._batch = batch;
					Box2D.XNA.TestBed.Framework.DebugDraw._device = batch.GraphicsDevice;
					firstTime = false;
				}
			}
		}
    }
}
