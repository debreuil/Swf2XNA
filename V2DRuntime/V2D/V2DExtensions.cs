using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using Box2DX.Dynamics;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Text.RegularExpressions;
using V2DRuntime.Attributes;
using DDW.Display;
using DDW.V2D.Serialization;

namespace V2DRuntime.V2D
{
	public static class V2DExtensions
	{
		public static void AddJoints(this IJointable ithis)
		{
			V2DDefinition def = ithis.VScreen.v2dWorld.GetDefinitionByName(ithis.DefinitionName);
			if (def != null)
			{
				for (int i = 0; i < def.Joints.Count; i++)
				{
					AddJoint(ithis, def.Joints[i], ithis.X, ithis.Y);
				}
			}
		}

		public static Body GetBody(this IJointable ithis, string name)
		{
			Body result = (name == V2DWorld.ROOT_NAME) ? ithis.VScreen.groundBody : null;
			if (result == null)
			{
				DisplayObject obj = ithis.GetChildByName(name);
				if(obj != null && obj is V2DSprite)
				{
					result = ((V2DSprite)obj).body;
				}
			}
			return result;
		}

		public static Joint AddJoint(this IJointable ithis, V2DJoint joint, float offsetX, float offsetY)
		{
			Joint jnt = null;
			//Body targ0 = ithis.VScreen.bodyMap[joint.Body1];
			//Body targ1 = ithis.VScreen.bodyMap[joint.Body2];
			Body targ0 = GetBody(ithis, joint.Body1);
			Body targ1 = GetBody(ithis, joint.Body2);
			Vector2 pt0 = new Vector2(joint.X + offsetX, joint.Y + offsetY);

			string name = joint.Name;
			float scale = ithis.WorldScale;

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

					jnt = ithis.VScreen.world.CreateJoint(dj);
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

					jnt = ithis.VScreen.world.CreateJoint(rj);
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

					jnt = ithis.VScreen.world.CreateJoint(pj);
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

					jnt = ithis.VScreen.world.CreateJoint(puj);
					break;

				case V2DJointKind.Gear:
					GearJointDef gj = new GearJointDef();
					gj.Body1 = targ0;
					gj.Body2 = targ1;
					gj.Joint1 = GetFirstGearableJoint(targ0.GetJointList());
					gj.Joint2 = GetFirstGearableJoint(targ1.GetJointList());
					gj.Ratio = joint.Ratio;
					jnt = ithis.VScreen.world.CreateJoint(gj);
					break;
			}

			if (jnt != null)
			{
				Dictionary<string, string> dict = new Dictionary<string, string>();
				dict["name"] = name;
				jnt.UserData = dict;
				ithis.VScreen.joints.Add(jnt);

				SetJointWithReflection(ithis, name, jnt);
			}


			return jnt;
		}

		private static Joint GetFirstGearableJoint(JointEdge je)
		{
			while (je != null && !(je.Joint is PrismaticJoint || je.Joint is RevoluteJoint))
			{
				je = je.Next;
			}
			if (je == null)
			{
				throw (new Exception("missing gear joint target"));
			}
			return je.Joint;
		}
		//private static Joint GetFirstGearableJoint(JointEdge je)
		//{
		//    Joint result = je.Joint;
		//    while (result != null && !(result is PrismaticJoint || result is RevoluteJoint))
		//    {
		//        je = je.Next;
		//        result = je.Joint;
		//        break;
		//    }
		//    return result;
		//}

		private static Regex lastDigits = new Regex(@"^([a-zA-Z$_]*)([0-9]+)$", RegexOptions.Compiled);
		public static void SetJointWithReflection(this IJointable ithis, string instName, Joint jnt)
		{
			Type t = ithis.GetType();

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
					object array = fi.GetValue(ithis);
					Type elementType = ft.GetElementType();
					if (array == null)
					{
						int arrayLength = GetJointArrayLength(ithis, instName);
						array = Array.CreateInstance(elementType, arrayLength);
						fi.SetValue(ithis, array);
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
						object collection = fi.GetValue(ithis);
						if (collection == null) // ensure list created
						{
							ConstructorInfo ci = ft.GetConstructor(new Type[] { });
							collection = ci.Invoke(new object[] { });
							fi.SetValue(ithis, collection);
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
					fi.SetValue(ithis, jnt);
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

		private static int GetJointArrayLength(this IJointable ithis, string instName)
		{
			int result = 1; // will always be at least one, allows dopping index in def for single arrays
			V2DDefinition def = ithis.VScreen.v2dWorld.GetDefinitionByName(ithis.DefinitionName);
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

	}
}
