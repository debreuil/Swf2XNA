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
using Box2D.XNA.TestBed.Framework;

namespace DDW.V2D
{
    [XmlRoot]
	public class V2DScreen : Screen, IJointable, IContactListener
	{
		public static float WorldScale = 15;
        public World world;
		protected V2DScreen v2dScreen;
		public Body groundBody;

		Cursor cursor;
		public float hz = 60.0f;
		public bool singleStep = false;
		public int velocityIterations = 8;
		public int positionIterations = 10;
		public int enableWarmStarting = 1;
		public int enableTOI = 1;
		public Vector2 Gravity = new Vector2(0.0f, 10.0f);

		public bool useDebugDraw = false;
		private bool firstTime = true;
//#if !XBOX
		internal MouseJoint _mouseJoint;
		private Box2D.XNA.TestBed.Framework.DebugDraw _debugDraw;
		private BasicEffect simpleColorEffect;
//#endif
     
        public V2DScreen()
        {
			CreateWorld();
        }
        public V2DScreen(SymbolImport symbolImport) : base(symbolImport)
        {
			CreateWorld();
        }
        public V2DScreen(V2DContent v2dContent) : base(v2dContent)
        {
			CreateWorld();
        }

		public V2DScreen VScreen { get { return v2dScreen; } set { v2dScreen = value; } }

		public override Sprite CreateDefaultObject(Texture2D texture, V2DInstance inst)
		{
			Sprite result;
			if (inst.Definition.V2DShapes.Count > 0 || inst.Definition.Instances.Count > 0)
			{
				result = new V2DSprite(texture, inst);
			}
			else
			{
				result = new Sprite(texture, inst);
			}
			return result;
		}

		#region Creation
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

					useDebugDraw = a.debugDraw;
				}
			}
		}
		public override void Added(EventArgs e)
		{
			base.Added(e);
			if (V2DGame.instance.HasCursor)
			{
				cursor = V2DGame.instance.GetCursor();
#if !XBOX
				cursor.MouseDown += MouseDown;
				cursor.MouseMove += MouseMove;
				cursor.MouseUp += MouseUp;
#endif
            }
		}
        public override void Removed(EventArgs e)
        {
            base.Removed(e);
			if (V2DGame.instance.HasCursor)
			{
				cursor = V2DGame.instance.GetCursor();
#if !XBOX
				cursor.MouseDown -= MouseDown;
				cursor.MouseMove -= MouseMove;
                cursor.MouseUp -= MouseUp;
#endif
            }
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
					DestroyBody(sp.body);
					sp.body.SetUserData(null);
				}
			}
		}
        //public override void DestroyElement(DisplayObject obj)
        //{
        //    base.DestroyElement(obj);
        //    if (obj is V2DSprite)
        //    {
        //        DestroyBody(((V2DSprite)obj).body);                
        //    }
        //}
		#endregion
		#region world
		private void CreateWorld()
		{
			if (world == null)
			{
				bool doSleep = true;
				ClientSize = new Vector2(v2dWorld.Width, v2dWorld.Height);

				world = new World(Gravity, doSleep);
				world.ContactListener = this;
				BodyDef groundBodyDef = new BodyDef();
				groundBodyDef.position = new Vector2(0f, 0f);
				groundBody = world.CreateBody(groundBodyDef);
			}
		}
		private void DestroyWorld()
		{
			// clear box2d
			world.ContactListener = null;
			Body b = world.GetBodyList();
			while (b != null)
			{
				DestroyBody(b);
				b = b.GetNext();
			}
			world = null;
		}
		#endregion
		#region Body
		public Body CreateBody(BodyDef bodyDef)
		{
			return world.CreateBody(bodyDef);
		}
		public void DestroyBody(Body b)
		{
			if (b != null && b.GetWorld() != null && world.BodyCount > 0)
			{
                // avoid bodies getting removed twice by accident
                if (b.GetFixtureList() != null)
                {
                    world.DestroyBody(b);
                }
			}
		}
		#endregion
		#region Joint
		public void  RemoveJoint(Joint joint)
        {
			Joint j = world.GetJointList();
			while (j != null)
			{
				if (j == joint)
				{
					world.DestroyJoint(joint);
					break;
				}
				j = j.GetNext();
			}
        }
        protected void  RemoveJointByName(string name)
		{
			Joint j = world.GetJointList();
			while (j != null)
			{
				if (j.GetUserData() != null && (string)j.GetUserData() == name)
				{
					world.DestroyJoint(j);
					break;
				}
				j = j.GetNext();
			}
		}
		#endregion

		#region Mouse
#if !XBOX
		public virtual void MouseDown(Vector2 p)
		{
			if (_mouseJoint != null)
			{
				return;
			}

			p /= V2DScreen.WorldScale;
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
			p /= V2DScreen.WorldScale;
			if (_mouseJoint != null)
			{
				_mouseJoint.SetTarget(p);
			}
		}
#endif
        #endregion
        #region Bounds
        Body[] boundsBodies = new Body[4];
		protected void ClearBoundsBodies()
		{
            for (int i = 0; i < boundsBodies.Length; i++)
            {
				if (boundsBodies[i] != null)// && world.Contains(b))
				{
					world.DestroyBody(boundsBodies[i]);
                    boundsBodies[i] = null;
				}
                
            }
		}

        protected void ClearBoundsBody(EdgeName edge)
        {
            for (int i = 0; i < boundsBodies.Length; i++)
            {
                if (boundsBodies[i] != null && (EdgeName)boundsBodies[i].GetUserData() == edge)
                {
                    world.DestroyBody(boundsBodies[i]);
                    boundsBodies[i] = null;
                    break;
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
			bodyDef.position = new Vector2(x + w / 2f, y + h / 2f);
			bodyDef.type = BodyType.Static;
			Body body = world.CreateBody(bodyDef);
			
			PolygonShape polyShape = new PolygonShape();
			polyShape.SetAsBox(w / 2f, h / 2f);
			//polyShape._vertexCount = 4;
			//polyShape._vertices[0] = new Vector2(0, 0);
			//polyShape._vertices[1] = new Vector2(w, 0);
			//polyShape._vertices[2] = new Vector2(w, h);
			//polyShape._vertices[3] = new Vector2(0, h);

			FixtureDef fd = new FixtureDef();
			fd.density = 1;
			fd.shape = polyShape;
			fd.filter.groupIndex = 1;
			fd.filter.categoryBits = 1;
			fd.filter.maskBits = 0xFFFF;
			Fixture f = body.CreateFixture(fd);			

			return body;
		}
		#endregion
		#region Contact
		internal int _pointCount;
		public static int k_maxContactPoints = 2048;
		internal ContactPoint[] _points = new ContactPoint[k_maxContactPoints];
		public virtual void BeginContact(Contact contact) { }
		public virtual void EndContact(Contact contact) { }
		public virtual void PreSolve(Contact contact, ref Manifold oldManifold)
		{
			// call this in the base class if points are needed
			// PreSolveCalcPoints(contact, ref oldManifold);
		}
		public virtual void PostSolve(Contact contact, ref ContactImpulse impulse) { }
		protected void PreSolveCalcPoints(Contact contact, ref Manifold oldManifold)
		{
			Manifold manifold;
			contact.GetManifold(out manifold);

			if (manifold._pointCount == 0)
			{
				return;
			}

			Fixture fixtureA = contact.GetFixtureA();
			Fixture fixtureB = contact.GetFixtureB();

			FixedArray2<PointState> state1, state2;
			Collision.GetPointStates(out state1, out state2, ref oldManifold, ref manifold);

			WorldManifold worldManifold;
			contact.GetWorldManifold(out worldManifold);

			for (int i = 0; i < manifold._pointCount && _pointCount < k_maxContactPoints; ++i)
			{
				if (fixtureA == null)
				{
					_points[i] = new ContactPoint();
				}
				ContactPoint cp = _points[_pointCount];
				cp.fixtureA = fixtureA;
				cp.fixtureB = fixtureB;
				cp.position = worldManifold._points[i];
				cp.normal = worldManifold._normal;
				cp.state = state2[i];
				_points[_pointCount] = cp;
				++_pointCount;
			}
		}
		#endregion

//#if !XBOX
		public override void DrawDebugData(SpriteBatch batch)
		{
			base.DrawDebugData(batch);

			if (useDebugDraw)
			{
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

				simpleColorEffect.Begin();
				simpleColorEffect.Techniques[0].Passes[0].Begin();
				_debugDraw.FinishDrawShapes();
				simpleColorEffect.Techniques[0].Passes[0].End();
				simpleColorEffect.End();
			}
		}
//#endif
        public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (isActive)
			{
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

				if (timeStep != 0)
				{
					world.Step(timeStep, velocityIterations, positionIterations);
				}

			}
		}
		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
//#if !XBOX
			if(useDebugDraw)
			{
				world.DrawDebugData();
			}
//#endif
        }
    }
}
