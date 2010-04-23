using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using Box2D.XNA;
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
			Joint result = null;
			JointDef jointDef = null;
			//Body targ0 = ithis.VScreen.bodyMap[joint.Body1];
			//Body targ1 = ithis.VScreen.bodyMap[joint.Body2];
			Body targ0 = GetBody(ithis, joint.Body1);
			Body targ1 = GetBody(ithis, joint.Body2);

			// gears need the first body static
			if (targ0 != null && targ1 != null && targ1.GetType() == BodyType.Static && targ0.GetType() != BodyType.Static)
			{
				Body temp = targ0;
				targ0 = targ1;
				targ1 = temp;
			}

			Vector2 pt0 = new Vector2(joint.X + offsetX, joint.Y + offsetY);

			string name = joint.Name;

			Vector2 anchor0 = new Vector2(pt0.X / V2DScreen.WorldScale, pt0.Y / V2DScreen.WorldScale);
			Vector2 anchor1 = new Vector2();

			switch (joint.Type)
			{
				case V2DJointKind.Distance:
					Vector2 pt1 = new Vector2(joint.X2 + offsetX, joint.Y2 + offsetY);
					anchor1 = new Vector2(pt1.X / V2DScreen.WorldScale, pt1.Y / V2DScreen.WorldScale);

					DistanceJointDef dj = new DistanceJointDef();
					dj.Initialize(targ0, targ1, anchor0, anchor1);
					dj.collideConnected = joint.CollideConnected;
					dj.dampingRatio = joint.DampingRatio;
					dj.frequencyHz = joint.FrequencyHz;
					if (joint.Length != -1)
					{
						dj.length = joint.Length / V2DScreen.WorldScale;
					}

					jointDef = dj;
					break;

				case V2DJointKind.Revolute:
					float rot0 = joint.Min; //(typeof(joint["min"]) == "string") ? parseFloat(joint["min"]) / 180 * Math.PI : joint["min"];
					float rot1 = joint.Max; //(typeof(joint["max"]) == "string") ? parseFloat(joint["max"]) / 180 * Math.PI : joint["max"];

					RevoluteJointDef rj = new RevoluteJointDef();
					rj.Initialize(targ0, targ1, anchor0);
					rj.lowerAngle = rot0;
					rj.upperAngle = rot1;

					rj.enableLimit = rot0 != 0 && rot1 != 0;
					rj.maxMotorTorque = joint.MaxMotorTorque;
					rj.motorSpeed = joint.MotorSpeed;
					rj.enableMotor = joint.EnableMotor;

					jointDef = rj;
					break;

				case V2DJointKind.Prismatic:
					float axisX = joint.AxisX;
					float axisY = joint.AxisY;
					float min = joint.Min;
					float max = joint.Max;

					PrismaticJointDef pj = new PrismaticJointDef();
					Vector2 worldAxis = new Vector2(axisX, axisY);
					pj.Initialize(targ0, targ1, anchor0, worldAxis);
					pj.lowerTranslation = min / V2DScreen.WorldScale;
					pj.upperTranslation = max / V2DScreen.WorldScale;

					pj.enableLimit = joint.EnableLimit;
					pj.maxMotorForce = joint.MaxMotorTorque;
					pj.motorSpeed = joint.MotorSpeed;
					pj.enableMotor = joint.EnableMotor;

					jointDef = pj;
					break;

				case V2DJointKind.Pully:
					Vector2 pt2 = new Vector2(joint.X2 + offsetX, joint.Y2 + offsetY);
					anchor1 = new Vector2(pt2.X / V2DScreen.WorldScale, pt2.Y / V2DScreen.WorldScale);

					Vector2 groundAnchor0 = new Vector2(joint.GroundAnchor1X / V2DScreen.WorldScale, joint.GroundAnchor1Y / V2DScreen.WorldScale);

					Vector2 groundAnchor1 = new Vector2(joint.GroundAnchor2X / V2DScreen.WorldScale, joint.GroundAnchor2Y / V2DScreen.WorldScale);

					float max0 = joint.MaxLength1;
					float max1 = joint.MaxLength2;

					float rat = joint.Ratio;

					PulleyJointDef puj = new PulleyJointDef();
					puj.Initialize(targ0, targ1, groundAnchor0, groundAnchor1, anchor0, anchor1, rat);
					puj.maxLengthA = (max0 + max1) / V2DScreen.WorldScale;
					puj.maxLengthB = (max0 + max1) / V2DScreen.WorldScale;

					puj.collideConnected = joint.CollideConnected;

					jointDef = puj;
					break;

				case V2DJointKind.Gear:
					GearJointDef gj = new GearJointDef();
					gj.bodyA = targ0;
					gj.bodyB = targ1;
					gj.joint1 = GetFirstGearableJoint(targ0.GetJointList());
					gj.joint2 = GetFirstGearableJoint(targ1.GetJointList());
					gj.ratio = joint.Ratio;
					jointDef = gj;
					break;
			}

			if (jointDef != null)
			{
				result = SetJointWithReflection(ithis, name, jointDef);

				if (result != null)
				{
					Dictionary<string, string> dict = new Dictionary<string, string>();
					dict["name"] = name;
					result.SetUserData(dict);
				}
			}

			return result;
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
		public static Joint SetJointWithReflection(this IJointable ithis, string instName, JointDef jointDef)
		{
			Joint result = null;
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

				// apply attributes
				System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fi);  // reflection

				foreach (System.Attribute attr in attrs)
				{
					if (jointDef is DistanceJointDef && attr is DistanceJointAttribute)
					{
						((DistanceJointAttribute)attr).ApplyAttribtues((DistanceJointDef)jointDef);
					}
					else if (jointDef is GearJointDef && attr is GearJointAttribute)
					{
						((GearJointAttribute)attr).ApplyAttribtues((GearJointDef)jointDef);
					}
					else if (jointDef is LineJointDef && attr is LineJointAttribute)
					{
						((LineJointAttribute)attr).ApplyAttribtues((LineJointDef)jointDef);
					}
					else if (jointDef is PrismaticJointDef && attr is PrismaticJointAttribute)
					{
						((PrismaticJointAttribute)attr).ApplyAttribtues((PrismaticJointDef)jointDef);
					}
					else if (jointDef is PulleyJointDef && attr is PulleyJointAttribute)
					{
						((PulleyJointAttribute)attr).ApplyAttribtues((PulleyJointDef)jointDef);
					}
					else if (jointDef is RevoluteJointDef && attr is RevoluteJointAttribute)
					{
						((RevoluteJointAttribute)attr).ApplyAttribtues((RevoluteJointDef)jointDef);
					}
				}
				result = ithis.VScreen.world.CreateJoint(jointDef);

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
					mi.Invoke(array, new object[] { result, index });
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
						mi.Invoke(collection, new object[] { index, result });
					}
				}
				else if (ft.Equals(typeof(Joint)) || ft.IsSubclassOf(typeof(Joint)))
				{
					fi.SetValue(ithis, result);
				}
				else
				{
					throw new ArgumentException("Not supported field type. " + ft.ToString() + " " + instName);
				}
			}
			else
			{
				result = ithis.VScreen.world.CreateJoint(jointDef);
			}

			return result;
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
