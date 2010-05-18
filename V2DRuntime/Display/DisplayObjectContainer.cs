using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DDW.Input;
using System.Text.RegularExpressions;
using System.Reflection;
using DDW.V2D;
using V2DRuntime.Shaders;
using V2DRuntime.Attributes;
using V2DRuntime.Display;

namespace DDW.Display
{  
    public class DisplayObjectContainer : DisplayObject
    {
        public event AnimationEvent PlayheadWrap;

		protected List<DisplayObject> children = new List<DisplayObject>();
		public uint FrameCount = 1;
		public uint CurFrame = 0;
		public float CurFrameTime = 0;
		public bool isPlaying = false;//true;//

		public DisplayObjectContainer()
        {
        }
		public DisplayObjectContainer(Texture2D texture, V2DInstance inst) : base(texture, inst)
        {
			this.FrameCount = (inst.Definition == null) ? 1 : inst.Definition.FrameCount;
        }

        public int NumChildren
        {
            get
            {
                return children.Count;
            }
        }
        public override float VisibleWidth
        {
            get
            {
                float max = (texture == null) ? 0 : texture.Width;
                float temp = 0;
                for (int i = 0; i < children.Count; i++)
                {
                    temp = children[i].VisibleWidth;
                    if (temp > max)
                    {
                        max = temp;
                    }
                }
                return max;
            }
        }
        public override float VisibleHeight
        {
            get
            {
                float max = (texture == null) ? 0 : texture.Height;
                float temp = 0;
                for (int i = 0; i < children.Count; i++)
                {
                    temp = children[i].VisibleHeight;
                    if (temp > max)
                    {
                        max = temp;
                    }
                }
                return max;
            }
        }
	    public virtual void AddChild(DisplayObject o)
        {
            o.Parent = this;
			children.Add(o);
			if (o.State.EndFrame > LastChildFrame)
            {
				LastChildFrame = o.State.EndFrame;
            }
            children.Sort(DisplayObject.CompareTo);

			o.SetStageAndScreen();

			if(o is DisplayObjectContainer)
			{
				((DisplayObjectContainer)o).EnsureInstancesCreated();
			}

            o.Added(EventArgs.Empty);
	    }
		private void EnsureInstancesCreated()
		{
			if (!isInitialized)
			{
				V2DDefinition def = screen.v2dWorld.GetDefinitionByName(definitionName);
				if (def != null)
				{
					for (int i = 0; i < def.Instances.Count; i++)
					{
						AddInstance(def.Instances[i], this);
					}
				}
			}
		}

        public virtual bool Contains(DisplayObject o)
	    {
		    return children.Contains(o);
	    }
        public virtual Rectangle GetBounds()
        {
            return Rectangle.Empty;
        }
        public virtual DisplayObject GetChildAt(int index)
        {
            DisplayObject result = null;
            if(index > 0 && index < children.Count)
            {
		        result = children[index];
            }
            return result;
        }
        public virtual DisplayObject GetChildByName(string name)
        {
            return children.Find(n => n.InstanceName == name);
            //DisplayObject result = null;
            //for (int i = 0; i < children.Count; i++)
            //{
            //    if (children[i].InstanceName == name)
            //    {
            //        result = children[i];
            //        break;
            //    }
            //}
            //return result;
        }
        public virtual int GetChildIndex(DisplayObject o)
        {
		    return children.IndexOf(o);
        }
        public virtual void RemoveChild(DisplayObject o)
        {
		    children.Remove(o);
			if (o.State.EndFrame >= LastChildFrame)
            {
			    LastChildFrame = children.Count == 0 ? 0 : children.Max(ef => ef.State.EndFrame);
            }

            o.Removed(EventArgs.Empty);
			o.Parent = null;
        }
        public virtual void RemoveChildAt(int index)
        {
            if (index > 0 && index < children.Count)
            {
                RemoveChild(children[index]); // todo: display objects may need to compare on depth
            }
        }
        public virtual void ClearChildren()
        {
            children.ForEach(RemoveChild);
        }

        public virtual void SetChildIndex(int index, DisplayObject o)
        {
            int curIndex = GetChildIndex(o);
            if (curIndex != -1 && curIndex != index && index > 0 && index < children.Count)
            {
                children.RemoveAt(curIndex);
                children.Insert(index, o);
            }
        }
        public virtual void SwapChildren(DisplayObject a, DisplayObject b)
        {
            int index0 = children.IndexOf(a);
            int index1 = children.IndexOf(b);
            SwapChildrenAt(index0, index1);
        }
        public virtual void SwapChildrenAt(int index0, int index1)
        {
            if (index0 > 0 && index0 < children.Count && index1 > 0 && index1 < children.Count)
            {
                DisplayObject a = children[index0];
                DisplayObject b = children[index1];

                children.RemoveAt(index0);
                children.RemoveAt(index1);

                if (index0 < index1)
                {
                    children.Insert(index0, a);
                }
                else
                {
                    children.Insert(index1, b);
                }
            }
        }

        public virtual void Play()
        {
            isPlaying = true;
        }
        public virtual void PlayAll()
        {
            this.Play();
            foreach (DisplayObject d in children)
            {
                if (d is DisplayObjectContainer)
                {
                    ((DisplayObjectContainer)d).PlayAll();
                }
            }
        }
        public virtual void Stop()
        {
            isPlaying = false;
        }
        public virtual void StopAll()
        {
            this.Stop();
            foreach (DisplayObject d in children)
            {
                if (d is DisplayObjectContainer)
                {
                    ((DisplayObjectContainer)d).StopAll();
                }
            }
        }
        public virtual void GotoAndPlay(uint frame)
        {
            CurChildFrame = frame < 0 ? 0 : frame > LastChildFrame ? LastChildFrame : frame;
            isPlaying = true;
        }
        public virtual void GotoAndStop(uint frame)
        {
            CurChildFrame = frame < 0 ? 0 : frame > LastChildFrame ? LastChildFrame : frame;
            isPlaying = false;
        }

        protected List<DisplayObject> DisplayList = new List<DisplayObject>();
        public uint CurChildFrame;
        public uint LastChildFrame;

		protected override void OnAddToStageComplete()
		{
			base.OnAddToStageComplete();
			foreach (DisplayObject d in children)
			{
				if (d is DisplayObjectContainer)
				{
					((DisplayObjectContainer)d).OnAddToStageComplete();
				}
				else
				{
					d.Initialize();
				}
			}
		}
		//public override void Initialize()
		//{
		//    base.Initialize();
		//    foreach (DisplayObject d in children)
		//    {
		//        d.Initialize();
		//    }
		//}
        public override void Added(EventArgs e)
		{
            foreach (DisplayObject d in children)
            {
                d.Added(e);
            }
            base.Added(e);
        }
        public override void Removed(EventArgs e)
        {
            base.Removed(e);
            foreach (DisplayObject d in children)
            {
                d.Removed(e);
            }
            //children.Clear();
        }
        public override void RemovedFromStage(EventArgs e)
        {
            base.RemovedFromStage(e);
            for (int i = 0; i < children.Count; i++)
            {
                children[i].RemovedFromStage(e);
            }
        }

		#region Instance Management
		public V2DInstance CreateInstanceDefinition(string definitionName, string instName)
		{
			V2DInstance result = null;

			V2DDefinition def = screen.v2dWorld.GetDefinitionByName(definitionName);
			if (def != null)
			{
				result = new V2DInstance();
				result.Definition = def;
				result.DefinitionName = def.Name;
				result.InstanceName = instName;
				result.Transforms = new V2DGenericTransform[1];
                result.Transforms[0] = new V2DGenericTransform(0, 0, 1, 1, 0, 0, 0, 1);
				result.Visible = true;
			}

			return result;
		}
		public DisplayObject CreateInstance(string definitionName, string instanceName, float x, float y, float rot)
		{
			return CreateInstanceAt(definitionName, instanceName, x, y, rot, this.children.Count);
		}
		public DisplayObject CreateInstanceAt(string definitionName, string instanceName, float x, float y, float rot, int depth)
		{
			DisplayObject result = null;
			if (definitionName == null || definitionName == "")
			{
				V2DInstance v2dInst = new V2DInstance();
				v2dInst.Depth = depth;
				v2dInst.InstanceName = instanceName;
				v2dInst.X = x;
				v2dInst.Y = y;
				v2dInst.Rotation = rot;
				v2dInst.Transforms = new V2DGenericTransform[1];
                v2dInst.Transforms[0] = new V2DGenericTransform(0, 0, 1, 1, 0, 0, 0, 1); // image
				v2dInst.Visible = true;
				result = AddInstance(v2dInst, this);
			}
			else
			{
				V2DDefinition def = screen.v2dWorld.GetDefinitionByName(definitionName);
				if (def != null)
				{
					V2DInstance v2dInst = new V2DInstance();
					v2dInst.Depth = depth;
					v2dInst.Definition = def;
					v2dInst.DefinitionName = def.Name;
					v2dInst.InstanceName = instanceName;
					v2dInst.X = x; // body
					v2dInst.Y = y;
					v2dInst.Rotation = rot;
                    v2dInst.Transforms = new V2DGenericTransform[1];
                    v2dInst.Transforms[0] = new V2DGenericTransform(0, 0, 1, 1, 0, 0, 0, 1); // image
					v2dInst.Visible = true;

					result = AddInstance(v2dInst, this);
				}
			}
			return result;
		}
		public virtual DisplayObject AddInstance(V2DInstance inst, DisplayObjectContainer parent)
		{
			DisplayObject result = null;
			if (inst != null)
			{
				V2DDefinition def = screen.v2dWorld.GetDefinitionByName(inst.DefinitionName);
				if (def != null)
				{
					Texture2D texture = screen.GetTexture(def.LinkageName);
					inst.Definition = def;

					if (inst.InstanceName == V2DGame.currentRootName)
					{
						result = this;
					}
					else
					{
						result = parent.SetFieldWithReflection(texture, inst);

						if (result == null)
						{
							result = screen.CreateDefaultObject(texture, inst);
						}
						parent.AddChild(result);
					}
				}
				else
				{
					result = parent.SetFieldWithReflection(texture, inst);
					if (result == null)
					{
						result = screen.CreateDefaultObject(texture, inst);
					}
					parent.AddChild(result);
				}
			}

			return result;
		}
		public virtual bool RemoveInstance(DisplayObject obj)
		{
			bool result = false;
			if (this.children.Contains(obj))
			{
				this.RemoveChild(obj);
				result = true;
			}
			else 
			{
				for (int i = 0; i < children.Count; i++)
				{
					if (children[i] is DisplayObjectContainer)
					{
						DisplayObjectContainer doc = (DisplayObjectContainer)children[i];
						if (doc.RemoveInstance(obj))
						{
							result = true;
							break;							
						}
					}
				}
			}
			return result;
		}
		//protected virtual void RemoveInstanceByName(string name)
		//{
		//    DisplayObject d = this.GetChildByName(name);
		//    if (d != null && d.Parent != null)
		//    {
		//        d.Parent.RemoveChild(d);
		//        d.Parent = null;
		//    }
		//}

        private DisplayObject[] tempChildrenArray;
        public override void DestroyView()
        {
            base.DestroyView();
            tempChildrenArray = children.ToArray();
            foreach (DisplayObject d in tempChildrenArray)
            {
                DestroyElement(d);
            }
            tempChildrenArray = null;
        }
        public override void CreateView()
        {
            base.CreateView();
            SetStageAndScreen();
            EnsureInstancesCreated();
        }
		/// <summary>
		/// Destroys element, any attached bodies, and children
		/// </summary>
		public virtual void DestroyElement(DisplayObject obj)
		{
            obj.DestroyView();
			this.RemoveInstance(obj);
		}
		#endregion

		protected Regex lastDigits = new Regex(@"^([a-zA-Z$_]*)([0-9]+)$", RegexOptions.Compiled);
		public virtual DisplayObject SetFieldWithReflection( Texture2D texture, V2DInstance inst)
		{
			DisplayObject result = null;

			Type t = this.GetType();
			string instName = inst.InstanceName;
			int index = -1;

			Match m = lastDigits.Match(instName);
			if (m.Groups.Count > 2 && t.GetField(instName) == null)
			{
				instName = m.Groups[1].Value;
				index = int.Parse(m.Groups[2].Value, System.Globalization.NumberStyles.None);
			}

			FieldInfo fi = t.GetField(instName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			if (fi != null)
			{
				Type ft = fi.FieldType;

				if (ft.BaseType.Name == typeof(V2DRuntime.Components.Group<>).Name && // IsSubclassOf etc doesn't work on generics?
					 ft.BaseType.Namespace == typeof(V2DRuntime.Components.Group<>).Namespace)
				{ 
					// eg ButtonTabGroup
					if (fi.GetValue(this) == null)
					{
						ConstructorInfo ci = ft.GetConstructor(new Type[] { typeof(Texture2D), typeof(V2DInstance) });
						result = (DisplayObject)ci.Invoke(new object[] { texture, inst });
						fi.SetValue(this, result);
					}
				}
				else if (ft.IsArray)
				{
					object array = fi.GetValue(this);
					Type elementType = ft.GetElementType();
					if (array == null)
					{
						int arrayLength = GetArrayLength(instName);
						array = Array.CreateInstance(elementType, arrayLength);
						fi.SetValue(this, array);
					}
					// add element
					ConstructorInfo elementCtor = elementType.GetConstructor(new Type[] { typeof(Texture2D), typeof(V2DInstance) });
					result = (DisplayObject)elementCtor.Invoke(new object[] { texture, inst });

					MethodInfo mi = array.GetType().GetMethod("SetValue", new Type[] { elementType, index.GetType() });
					mi.Invoke(array, new object[] { result, index });
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

						// add element
						ConstructorInfo elementCtor = gt.GetConstructor(new Type[] { typeof(Texture2D), typeof(V2DInstance) });
						result = (DisplayObject)elementCtor.Invoke(new object[] { texture, inst });

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
				//else if (ft.Equals(typeof(TextBox)) || ft.IsSubclassOf(typeof(TextBox)))
				//{
				//    ConstructorInfo ci = ft.GetConstructor(new Type[] { });
				//    result = (DisplayObject)ci.Invoke(new object[] { });
				//    fi.SetValue(this, result);
				//}
				else if (ft.Equals(typeof(DisplayObject)) || ft.IsSubclassOf(typeof(DisplayObject)))
				{
					ConstructorInfo ci = ft.GetConstructor(new Type[] { typeof(Texture2D), typeof(V2DInstance) });
					result = (DisplayObject)ci.Invoke(new object[] { texture, inst });
					fi.SetValue(this, result);
				}
				else
				{
					throw new ArgumentException("Not supported field type. " + ft.ToString() + " " + instName);
				}
			}

			if (result != null)
			{
				result.Index = index; // set for all object, -1 if not in collection

				// apply attributes
				System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fi);  // reflection

				foreach (System.Attribute attr in attrs)
				{
					if (attr is SpriteAttribute)
					{
						SpriteAttribute a = (SpriteAttribute)attr;
						result.DepthGroup = a.depthGroup;
					}

					if (attr is V2DSpriteAttribute)
					{
						if (result is V2DSprite)
						{
							V2DSpriteAttribute a = (V2DSpriteAttribute)attr;
							V2DSprite sp = (V2DSprite)result;
							sp.attributeProperties = a;
							sp.SetGroupIndex(a.groupIndex);
							sp.IsStatic = a.isStatic;
						}
					}
				}

				// need to do this separately to ensure the depth group is set in previous step
				if (this is Screen)
				{
					Screen scr = (Screen)this;
					// field attirbutes
					foreach (System.Attribute attr in attrs)
					{
						if (attr is V2DShaderAttribute && !scr.shaderMap.ContainsKey(result.DepthGroup))
						{
							V2DShaderAttribute vsa = (V2DShaderAttribute)attr;
							float[] parameters = new float[] { };
							ConstructorInfo ci = vsa.shaderType.GetConstructor(new Type[] { parameters.GetType() });
							scr.shaderMap.Add(
								result.DepthGroup,  
								(V2DShader)ci.Invoke(new object[] { new float[]{vsa.param0, vsa.param1, vsa.param2, vsa.param3, vsa.param4} })
								);
						}
					}
				}

			}
			return result;
		}

		private int GetArrayLength(string instName)
		{
			int result = 1; // will always be at least one, allows dopping index in def for single arrays
			V2DDefinition def = screen.v2dWorld.GetDefinitionByName(definitionName);
			if (def != null)
			{
				foreach (V2DInstance vi in def.Instances)
				{
					if (vi.InstanceName.StartsWith(instName))
					{
						string s = vi.InstanceName.Substring(instName.Length);
						int val = 0;
						try
						{
							val = int.Parse(s, System.Globalization.NumberStyles.None);
						}
						catch(Exception)
						{
						}
						// xbox doesn't support TryParse
						//if (int.TryParse(s, out val))
						//{
						//    result = System.Math.Max(val + 1, result);
						//}
						result = System.Math.Max(val + 1, result);
					}
				}
			}
			return result;
		}

        public override bool OnPlayerInput(int playerIndex, Move move, TimeSpan time)
        {
            bool result = base.OnPlayerInput(playerIndex, move, time);
			if (result)
			{
				foreach (DisplayObject d in children)
				{
					result = d.OnPlayerInput(playerIndex, move, time);
					if (!result)
					{
						break;
					}
				}
			}
			return result;
        }
        public override void Update(GameTime gameTime)
        {

            if (isPlaying && LastChildFrame > 0)
            {
                CurFrameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                CurChildFrame = ((uint)(CurFrameTime / mspf));
                if (CurChildFrame > LastChildFrame)
                {
                    CurChildFrame = 0;
                    CurFrameTime %= (mspf * LastChildFrame);
                    if (PlayheadWrap != null)
                    {
                        PlayheadWrap(this);
                    }
                }
            }

            base.Update(gameTime);

            foreach (DisplayObject d in children)
            {
				if (d.Visible && d.Alpha > 0)
				{
					if (CurChildFrame >= d.State.StartFrame && CurChildFrame <= d.State.EndFrame)
					{
						d.Update(gameTime);
					}
				}
            }
		}
        public override void Draw(SpriteBatch batch)
		{			
			foreach (DisplayObject d in children)
			{
				if (d.Visible && d.Alpha > 0 && d.DepthGroup < 0)
				{
					if (CurChildFrame >= d.State.StartFrame && CurChildFrame <= d.State.EndFrame)
					{
						DrawChild(d, batch);						
					}
				}
			}
			base.Draw(batch);
			foreach (DisplayObject d in children)
			{
				if (d.Visible && d.Alpha > 0 && d.DepthGroup >= 0)
				{
					if (CurChildFrame >= d.State.StartFrame && CurChildFrame <= d.State.EndFrame)
					{
						DrawChild(d, batch);						
					}
				}
			}
        }
		protected virtual void DrawChild(DisplayObject d, SpriteBatch batch)
		{
			d.Draw(batch);
		}
    }
}
