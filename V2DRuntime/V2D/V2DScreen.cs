using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Box2DX.Common;
using Box2DX.Dynamics;

using DDW.Display;
using DDW.V2D.Serialization;
using DDW.Input;
using V2DRuntime.Components;
using V2DRuntime.V2D;
using Box2DX.Collision;
using V2DRuntime.Enums;
using V2DRuntime.Attributes;

namespace DDW.V2D
{
    [XmlRoot]
    public class V2DScreen : Screen
    {
        public World world;

        public float worldScale = 15;
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
		public float WorldScale = 15;
		public Vec2 Gravity = new Vec2(0.0f, 10.0f); 
        
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
 
		protected override void RemoveInstanceByName(string name)
		{
			Body bd = GetBodyByName(name);

			if (bd != null)
			{
				if (bodyMap.ContainsKey(name))
				{
					bodyMap.Remove(name);

					List<Joint> relatedJoints = new List<Joint>();
					for (int j = joints.Count - 1; j >= 0; j--)
					{
						if (joints[j].GetBody1() == bd || joints[j].GetBody2() == bd)
						{
							joints.RemoveAt(j);
							relatedJoints.Add(joints[j]);
						}
					}

					for (int j = relatedJoints.Count - 1; j >= 0; j--)
					{
						world.DestroyJoint(relatedJoints[j]);
					}

					DestroyBody(bd);
				}
				bd.SetUserData(null);

			}

			base.RemoveInstanceByName(name);
		}
		public override void Initialize()
		{
			base.Initialize();

			V2DDefinition def = v2dWorld.GetDefinitionByName(this.definitionName);
			if (def != null)
			{
				for (int i = 0; i < def.Joints.Count; i++)
				{
					AddJoint(def.Joints[i], this.X, this.Y);
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
						Gravity = new Vec2(a.gravityX, a.gravityY);
						world.Gravity = Gravity;
					}
				}
			}
		}
		private void CreateWorld()
		{
			if (world == null)
			{
				bool doSleep = true;
				ClientSize = new Vector2(v2dWorld.Width, v2dWorld.Height);
				float w = ClientSize.X / WorldScale;
				float h = ClientSize.Y / WorldScale;
				AABB worldAABB = new AABB();
				worldAABB.LowerBound.Set(0 - 500, (-h / 2) - 500);
				worldAABB.UpperBound.Set((w / 2) + 500, (h / 2) + 500);

				world = new World(worldAABB, Gravity, doSleep);
				bodyMap.Add(V2DGame.ROOT_NAME, world.GetGroundBody());
				if (instanceName != V2DGame.ROOT_NAME)
				{
					bodyMap.Add(this.instanceName, world.GetGroundBody());
				}
			}
		}
		private void DestroyWorld()
		{
			// clear box2d
			Body b = world.GetBodyList();
			while (b != null)
			{
				DestroyBody(b);
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

        protected Joint AddJoint(V2DJoint joint, float offsetX, float offsetY)
        {
            Joint jnt = null;
            Body targ0 = this.bodyMap[joint.Body1];
            Body targ1 = this.bodyMap[joint.Body2];
            Vector2 pt0 = new Vector2(joint.X + offsetX, joint.Y + offsetY);

            string name = joint.Name;
            float scale = WorldScale;

            Vec2 anchor0 = new Vec2();
            anchor0.Set(pt0.X / scale, pt0.Y / scale);
            Vec2 anchor1 = new Vec2();

            switch (joint.Type)
            {
                case V2DJointKind.Distance:
                    Vec2 pt1 = new Vec2(joint.X2 + offsetX, joint.Y2 + offsetY);
                    anchor1.Set(pt1.X / scale, pt1.Y / scale);

                    DistanceJointDef dj = new DistanceJointDef();
                    dj.Initialize(targ0, targ1, anchor0, anchor1);
                    dj.CollideConnected = joint.CollideConnected;
                    dj.DampingRatio = joint.DampingRatio;
                    dj.FrequencyHz = joint.FrequencyHz;
                    if (joint.Length != -1)
                    {
                        dj.Length = joint.Length / scale;
                    }

                    jnt = this.world.CreateJoint(dj);
                    break;

                case V2DJointKind.Revolute:
                    float rot0 = joint.Min; //(typeof(joint["min"]) == "string") ? parseFloat(joint["min"]) / 180 * Math.PI : joint["min"];
                    float rot1 = joint.Max; //(typeof(joint["max"]) == "string") ? parseFloat(joint["max"]) / 180 * Math.PI : joint["max"];

                    RevoluteJointDef rj = new RevoluteJointDef();
                    rj.Initialize(targ0, targ1, anchor0);
                    rj.LowerAngle = rot0;
                    rj.UpperAngle = rot1;

                    rj.EnableLimit = rot0 != 0 && rot1 != 0;
                    rj.MaxMotorTorque = joint.MaxMotorTorque;
                    rj.MotorSpeed = joint.MotorSpeed;
                    rj.EnableMotor = joint.EnableMotor;

                    jnt = this.world.CreateJoint(rj);
                    break;

                case V2DJointKind.Prismatic:
                    float axisX = joint.AxisX;
                    float axisY = joint.AxisY;
                    float min = joint.Min;
                    float max = joint.Max;

                    PrismaticJointDef pj = new PrismaticJointDef();
                    Vec2 worldAxis = new Vec2();
                    worldAxis.Set(axisX, axisY);
                    pj.Initialize(targ0, targ1, anchor0, worldAxis);
                    pj.LowerTranslation = min / scale;
                    pj.UpperTranslation = max / scale;

                    pj.EnableLimit = joint.EnableLimit;
                    pj.MaxMotorForce = joint.MaxMotorTorque;
                    pj.MotorSpeed = joint.MotorSpeed;
                    pj.EnableMotor = joint.EnableMotor;

                    jnt = this.world.CreateJoint(pj);
                    break;

                case V2DJointKind.Pully:
                    Vector2 pt2 = new Vector2(joint.X2 + offsetX, joint.Y2 + offsetY);
                    anchor1.Set(pt2.X / scale, pt2.Y / scale);

                    Vec2 groundAnchor0 = new Vec2();
                    groundAnchor0.Set(joint.GroundAnchor1X / scale, joint.GroundAnchor1Y / scale);

                    Vec2 groundAnchor1 = new Vec2();
                    groundAnchor1.Set(joint.GroundAnchor2X / scale, joint.GroundAnchor2Y / scale);

                    float max0 = joint.MaxLength1;
                    float max1 = joint.MaxLength2;

                    float rat = joint.Ratio;

                    PulleyJointDef puj = new PulleyJointDef();
                    puj.Initialize(targ0, targ1, groundAnchor0, groundAnchor1, anchor0, anchor1, rat);
                    puj.MaxLength1 = (max0 + max1) / scale;
                    puj.MaxLength2 = (max0 + max1) / scale;

                    puj.CollideConnected = joint.CollideConnected;

                    jnt = this.world.CreateJoint(puj);
                    break;

                case V2DJointKind.Gear:
                    GearJointDef gj = new GearJointDef();
                    gj.Body1 = targ0;
                    gj.Body2 = targ1;
                    gj.Joint1 = GetFirstGearableJoint(targ0.GetJointList());
                    gj.Joint2 = GetFirstGearableJoint(targ1.GetJointList());
                    gj.Ratio = joint.Ratio;
                    jnt = this.world.CreateJoint(gj);
                    break;
            }

            if (jnt != null)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict["name"] = name;
                jnt.UserData = dict;
                this.joints.Add(jnt);

				SetJointWithReflection(name, jnt);
            }


            return jnt;
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
                if((string)joints[i].UserData == name)
                {
                    RemoveJoint(joints[i]);
                    break;    
                }
            }	  
        }

        protected Joint GetFirstGearableJoint(JointEdge je)
        {
            Joint result = je.Joint;
            while (result != null && !(result is PrismaticJoint || result is RevoluteJoint))
            {
                je = je.Next;
                result = je.Joint;
                break;
            }
            return result;
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
		public Body CreateBody(BodyDef bodyDef)
		{
			return world.CreateBody(bodyDef);
		}
		public void DestroyBody(Body b)
		{
			if (world.Contains(b))
			{
				world.DestroyBody(b);
				this.bodies.Remove(b);
			}
			else
			{
			}
		}
		public void SetGravity(Vec2 v2)
		{
			world.Gravity = v2;
		}
		public void SetContactListener(ContactListener contactListener)
		{
			world.SetContactListener(contactListener);
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

		public override void SetBounds(float x, float y, float w, float h)
		{
			foreach (Body b in boundsBodies)
			{
				if (b != null && world.Contains(b))
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

		protected Regex lastDigits = new Regex(@"^([a-zA-Z$_]*)([0-9]+)$", RegexOptions.Compiled);
		public virtual void SetJointWithReflection(string instName, Joint jnt)
		{
			Type t = this.GetType();

			int index = -1;
			FieldInfo fi = t.GetField(instName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (fi == null)
			{
				Match m = lastDigits.Match(instName);
				if (m.Groups.Count > 2 && t.GetField(instName) == null)
				{
					instName = m.Groups[1].Value;
					index = int.Parse(m.Groups[2].Value, System.Globalization.NumberStyles.None);
					fi = t.GetField(instName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				}
			}

			if (fi != null)
			{
				Type ft = fi.FieldType;
				
				if (ft.IsArray)
				{
					object array = fi.GetValue(this);
					Type elementType = ft.GetElementType();
					if (array == null)
					{
						int arrayLength = GetJointArrayLength(instName);
						array = Array.CreateInstance(elementType, arrayLength);
						fi.SetValue(this, array);
					}

					MethodInfo mi = array.GetType().GetMethod("SetValue", new Type[] { elementType, index.GetType() });
					mi.Invoke(array, new object[] { jnt, index });
				}
				else if (typeof(System.Collections.ICollection).IsAssignableFrom(ft))
				{
					Type[] genTypes = ft.GetGenericArguments();
					if (genTypes.Length == 1) // only support single type generics (eg List<>) for now
					{
						Type gt = genTypes[0];
						object collection = fi.GetValue(this);
						if (collection == null) // ensure list created
						{
							ConstructorInfo ci = ft.GetConstructor(new Type[] { });
							collection = ci.Invoke(new object[] { });
							fi.SetValue(this, collection);
						}

						PropertyInfo cm = collection.GetType().GetProperty("Count");
						int cnt = (int)cm.GetValue(collection, new object[] { });

						// pad with nulls if needs to skip indexes (order is based on flash depth, not index)
						while (index > cnt)
						{
							MethodInfo mia = collection.GetType().GetMethod("Add");
							mia.Invoke(collection, new object[] { null });
							cnt = (int)cm.GetValue(collection, new object[] { });
						}

						if (index < cnt)
						{
							MethodInfo mia = collection.GetType().GetMethod("RemoveAt");
							mia.Invoke(collection, new object[] { index });
						}

						MethodInfo mi = collection.GetType().GetMethod("Insert");
						mi.Invoke(collection, new object[] { index, jnt });
					}
				}
				else if (ft.Equals(typeof(Joint)) || ft.IsSubclassOf(typeof(Joint)))
				{
					fi.SetValue(this, jnt);
				}
				else
				{
					throw new ArgumentException("Not supported field type. " + ft.ToString() + " " + instName);
				}


				// apply attributes
				System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fi);  // reflection

				foreach (System.Attribute attr in attrs)
				{
					if (jnt is DistanceJoint && attr is DistanceJointAttribute)
					{
						((DistanceJointAttribute)attr).ApplyAttribtues((DistanceJoint)jnt);
					}
					else if (jnt is GearJoint && attr is GearJointAttribute)
					{
						((GearJointAttribute)attr).ApplyAttribtues((GearJoint)jnt);
					}
					else if (jnt is LineJoint && attr is LineJointAttribute)
					{
						((LineJointAttribute)attr).ApplyAttribtues((LineJoint)jnt);
					}
					else if (jnt is PrismaticJoint && attr is PrismaticJointAttribute)
					{
						((PrismaticJointAttribute)attr).ApplyAttribtues((PrismaticJoint)jnt);
					}
					else if (jnt is PulleyJoint && attr is PulleyJointAttribute)
					{
						((PulleyJointAttribute)attr).ApplyAttribtues((PulleyJoint)jnt);
					}
					else if (jnt is RevoluteJoint && attr is RevoluteJointAttribute)
					{
						((RevoluteJointAttribute)attr).ApplyAttribtues((RevoluteJoint)jnt);
					}
				}
			}
		}

		private int GetJointArrayLength(string instName)
		{
			int result = 1; // will always be at least one, allows dopping index in def for single arrays
			V2DDefinition def = screen.v2dWorld.GetDefinitionByName(definitionName);
			if (def != null)
			{
				foreach (V2DJoint vi in def.Joints)
				{
					if (vi.Name.StartsWith(instName))
					{
						string s = vi.Name.Substring(instName.Length);
						int val = 0;
						try
						{
							val = int.Parse(s, System.Globalization.NumberStyles.None);
						}
						catch (Exception)
						{
						}
						result = System.Math.Max(val + 1, result);
					}
				}
			}
			return result;
		}



		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (V2DGame.instance.IsActive && isActive)
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

				world.SetWarmStarting(enableWarmStarting > 0);
				world.SetContinuousPhysics(enableTOI > 0);

				world.Step(timeStep, velocityIterations, positionIterations);

				world.Validate();

				Body b = world.GetBodyList();
				while (b != null)
				{
					if (b.GetUserData() is V2DSprite)
					{
						V2DSprite s = (V2DSprite)b.GetUserData();
						Vector2 offset = s.Parent.GetGlobalOffset(Vector2.Zero); //Vector2.Zero;// 
						s.X = (int)(b.GetPosition().X * WorldScale - offset.X);
						s.Y = (int)(b.GetPosition().Y * WorldScale - offset.Y);
						s.Rotation = b.GetAngle();
					}
					b = b.GetNext();
				}
			}
		}
    }
}
