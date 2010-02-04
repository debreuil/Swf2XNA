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
        
        public V2DScreen()
        {
        }
        public V2DScreen(SymbolImport symbolImport) : base(symbolImport)
        {
        }
        public V2DScreen(V2DContent v2dContent) : base(v2dContent)
        {
        }

        public void Activate(World world)
        {
			base.Activate();

            bodyMap.Clear();
            bodyMap.Add(V2DGame.currentRootName, world.GetGroundBody());
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
		}
        public override void Removed(EventArgs e)
        {
            base.Removed(e);

			// clear box2d
			Body b = world.GetBodyList();
			while (b != null)
			{
				DestroyBody(b);
				b = b.GetNext();
			}
			//for (int i = 0; i < bodies.Count; i++)
			//{
			//    world.DestroyBody(bodies[i]);
			//}

			//for (int i = 0; i < joints.Count; i++)
			//{
			//    world.DestroyJoint(joints[i]);
			//}

			joints.Clear();
			bodyMap.Clear();
        }

        protected Joint AddJoint(V2DJoint joint, float offsetX, float offsetY)
        {
            Joint jnt = null;
            Body targ0 = this.bodyMap[joint.Body1];
            Body targ1 = this.bodyMap[joint.Body2];
            Vector2 pt0 = new Vector2(joint.X + offsetX, joint.Y + offsetY);

            string name = joint.Name;
            float scale = V2DStage.GetInstance().WorldScale;

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
    }
}
