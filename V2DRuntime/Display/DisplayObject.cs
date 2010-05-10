using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using V2DRuntime.V2D;
using DDW.V2D;
using DDW.Input;

namespace DDW.Display
{
    public class DisplayObject : IDrawable
    {
		protected Texture2D texture; // texture evetually will be a 'graphics' generic class
		protected V2DInstance instanceDefinition;

		protected string instanceName;
		protected string definitionName;
        public static uint DepthCounter;
        public int Index = -1;
		public int DepthGroup = 0;

        protected float mspf;

        public int id;
		public bool isInitialized = false;
        private static int idCounter = 0;//int.MinValue;
		private bool isOnStage = false;

		public DisplayObject()
		{
			id = idCounter++;
			this.instanceName = "_inst" + id;
			transforms = new V2DTransform[]{State};
		}
		public DisplayObject(Texture2D texture, V2DInstance inst)
		{
			id = idCounter++;

			Texture = texture;
			instanceDefinition = inst;

			this.X = (int)inst.X;
			this.Y = (int)inst.Y;

			ResetInstanceProperties();
		}

        #region Properties

		public V2DTransform State = new V2DTransform(0, 1, 1, 1, 0, 0, 0, 1);
		public V2DTransform CurrentState = new V2DTransform(0, 1, 1, 1, 0, 0, 0, 1);

        protected Rectangle sourceRectangle;

        protected Color color = Color.White;

        protected V2DTransform[] transforms;

        protected float depth;
        protected SpriteEffects spriteEffects = SpriteEffects.None;

        protected bool visible = true;
        protected DisplayObjectContainer parent;
		protected Stage stage;
		protected Screen screen;

		public bool IsOnStage { get { return isOnStage; } }
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
                if (texture != null)
				{
                    sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
					State.Position = new Vector2(0, 0);
                }
                    
            }
        }
        public string InstanceName
        {
            get
            {
                return instanceName;
            }
            set
            {
				instanceName = value;
            }
        }
        public string DefinitionName
        {
            get
            {
				return definitionName;
            }
            set
            {
                definitionName = value;
            }
        }
        public Vector2 Origin
        {
            get
            {
				return State.Origin;
            }
            set
            {
				State.Origin = value;
            }
        }
        public Rectangle SourceRectangle
        {
            get
            {
                return sourceRectangle;
            }
            set
            {
                sourceRectangle = value;
            }
        }
        public virtual Vector2 Position
        {
            get
            {
				return State.Position;
            }
            set
            {
				State.Position = value;
            }
        }
        public virtual float X
        {
            get
            {
				return State.Position.X + transforms[transformIndex].Position.X;
            }
            set
            {
				State.Position.X = value;
            }
        }
		public virtual float Y
        {
            get
            {
				return State.Position.Y + transforms[transformIndex].Position.Y;
            }
            set
            {
				State.Position.Y = value;
            }
        }
        public float Width
        {
            get
            {
				return (texture == null) ? 0 : texture.Width;
            }
        }
        public float Height
        {
            get
            {
				return (texture == null) ? 0 : texture.Height;
            }
        }
        public virtual float VisibleWidth
        {
            get
            {
				return (texture == null) ? 0 : texture.Width;
            }
        }
        public virtual float VisibleHeight
        {
            get
            {
				return (texture == null) ? 0 : texture.Height;
            }
        }
        public virtual float Rotation
        {
            get
            {
				return State.Rotation + transforms[transformIndex].Rotation;
            }
            set
            {
				State.Rotation = value;
            }
        }
        public Vector2 Scale
        {
            get
            {
				return State.Scale;
            }
            set
            {
				State.Scale = value;
            }
        }
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        public float Depth
        {
            get
            {
                return depth;
            }
            set
            {
                depth = value;
            }
        }
        public SpriteEffects SpriteEffects
        {
            get
            {
                return spriteEffects;
            }
            set
            {
                spriteEffects = value;
            }
        }
        public float Alpha
        {
            get
            {
                return State.Alpha;
            }
            set
            {
				State.Alpha = value;
				color.A = (byte)(State.Alpha * 255);
            }
        }
        public virtual bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }
        public V2DTransform[] Transforms
        {
            get
            {
                return transforms;
            }
            set
            {
                transforms = value;
            }
        }
        public DisplayObjectContainer Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }
        public Stage Stage
        {
            get
            {
                return stage;
            }
            set
            {
                stage = value;
            }
        }
		public Screen Screen 
		{			
            get
            {
                return screen;
            }
            set
            {
				screen = value;
            }
		}
        #endregion
		public virtual void Initialize()
        {
			isInitialized = true;
        }
		protected virtual void OnAddToStageComplete()
		{
			this.Initialize();
		}
        public virtual DisplayObject Clone()
        {
            DisplayObject result = new DisplayObject();
            result.texture = texture;
            result.instanceDefinition = instanceDefinition;
			result.State.Origin = State.Origin;
            result.sourceRectangle = sourceRectangle;
			result.State.Position = State.Position;
			result.State.Rotation = State.Rotation;
			result.State.Scale = State.Scale;
            result.color = color;
            result.depth = depth;
            result.spriteEffects = spriteEffects;
			result.State.Alpha = State.Alpha;
            result.visible = visible;
            result.transforms = (V2DTransform[])transforms.Clone();
            result.parent = parent;
            result.stage = stage;
            result.id = idCounter++;
            return result;
        }

        protected Stage GetStage()
        {
            Stage result = null;
            if (parent != null)
            {
                result = parent.GetStage();
            }
            else
            {
                result = (this is Stage) ? (Stage)this : null;
            }
            return result;
        }
		protected Screen GetScreen()
        {
            Screen result = null;
            if (this is Screen)
            {
                result = (Screen)this;
            }
            else if (parent != null)
            {
                result = parent.GetScreen();
            }
            else
            {
                result = screen;
            }
            return result;
        }
		//protected Screen GetContainerScreen(DisplayObject obj)
		//{
		//    Screen result = null;
		//    while (obj.parent != null && !(obj.parent is Screen) && obj.parent is DisplayObjectContainer)
		//    {
		//        obj = (DisplayObjectContainer)obj.parent;
		//    }

		//    if (obj.parent is Screen)
		//    {
		//        result = (Screen)obj.parent;
		//    }
		//    return result;
		//}

		public virtual void SetStageAndScreen()
		{
			if (!isOnStage)
			{
				stage = GetStage();
				screen = GetScreen();
			}
		}
		/// <summary>
		/// When object is added to a parent object. Parent isn't neccesarily on stage.
		/// </summary>
        public virtual void Added(EventArgs e)
        {
            if (!isOnStage && stage != null)
            {
				stage.ObjectAddedToStage(this);
            }
        }
		/// <summary>
		/// When object is removed to a parent object. Parent isn't neccesarily on stage.
		/// </summary>
        public virtual void Removed(EventArgs e)
		{
            if (isOnStage)
            {
                this.RemovedFromStage(e);
            }
            //this.parent = null;
        }
		/// <summary>
		/// When this object, or a parent object is added to stage.
		/// </summary>
        public virtual void AddedToStage(EventArgs e)
		{
			mspf = (screen == null) ? stage.MillisecondsPerFrame : screen.MillisecondsPerFrame;
			isOnStage = true;
			if (parent.isInitialized)
			{
				OnAddToStageComplete();
			}
        }
		/// <summary>
		/// When this object, or a parent object is removed from stage.
		/// </summary>
        public virtual void RemovedFromStage(EventArgs e)
        {
            if (stage != null)
            {
				stage.ObjectRemovedFromStage(this);
            }
			isOnStage = false;
			stage = null;
        }

		//public void LocalToGlobal(ref float x, ref float y)
		//{
		//    if (parent != null)
		//    {
		//        parent.LocalToGlobal(ref x, ref y);
		//    }
		//    x += this.X;
		//    y += this.Y;
		//}
		//public Vector2 LocalToGlobal(Vector2 p)
		//{
		//    if (parent != null)
		//    {
		//        p = parent.LocalToGlobal(p);
		//    }
		//    p.X += this.X;
		//    p.Y += this.Y;
		//    return p;
		//}

        public Vector2 GetGlobalOffset(Vector2 offset)
        {
            if (parent != null)
            {
                offset = parent.GetGlobalOffset(offset);
                if (transforms != null)
                {
                    V2DTransform t = transforms.FirstOrDefault(tr =>
                        tr.StartFrame <= (parent.CurChildFrame) && tr.EndFrame >= (parent.CurChildFrame));
					offset.X += t.Position.X + State.Position.X;
					offset.Y += t.Position.Y + State.Position.Y;
                }
                else
                {
					offset.X += State.Position.X;
					offset.Y += State.Position.Y;
                }
            }
            return offset;
        }
        public float GetGlobalRotation(float rot)
        {
            if (parent != null)
            {
                rot = parent.GetGlobalRotation(rot);
                if (transforms != null)
                {
                    V2DTransform t = transforms.First(tr =>
                        tr.StartFrame <= (parent.CurChildFrame) && tr.EndFrame >= (parent.CurChildFrame));
					rot += t.Rotation + State.Rotation;//180f * 3.14159265f;
                }
                else
                {
					rot += State.Rotation;
                }
            }
            return rot;
        }
        public Vector2 GetGlobalScale(Vector2 sc)
        {
            if (parent != null)
            {
                sc = parent.GetGlobalScale(sc);
                if (transforms != null)
                {
                    V2DTransform t = transforms.First(tr =>
                        tr.StartFrame <= (parent.CurChildFrame) && tr.EndFrame >= (parent.CurChildFrame));
                    sc.X *= t.Scale.X;
                    sc.Y *= t.Scale.Y;
                }
                else
                {
                }
            }
            return sc;
        }

		public virtual void DestroyAfterUpdate()
		{
			if (screen != null)
			{
				screen.DestroyAfterUpdate(this);
			}
		}

        public virtual void DestroyView()
        {
            this.isInitialized = false;
        }
        public virtual void CreateView()
        {
        }
        public virtual void ReplaceView(string definitionName)
        {
            DestroyView();
            this.DefinitionName = definitionName;
            CreateView();
        }

		protected virtual void ResetInstanceProperties()
		{
			if (instanceDefinition != null)
			{
				this.InstanceName = instanceDefinition.InstanceName;
				this.DefinitionName = instanceDefinition.DefinitionName;

				this.Position = new Vector2((int)instanceDefinition.X, (int)instanceDefinition.Y);
				this.Rotation = instanceDefinition.Rotation;
				this.Scale = new Vector2(instanceDefinition.ScaleX, instanceDefinition.ScaleY);
				this.State.Origin = instanceDefinition.Definition == null ? 
					Vector2.Zero :
					new Vector2(-instanceDefinition.Definition.OffsetX, -instanceDefinition.Definition.OffsetY);

				this.Alpha = instanceDefinition.Alpha;
				this.Visible = instanceDefinition.Visible;
				this.Depth = instanceDefinition.Depth;

				transforms = V2DTransform.GetV2DTransformArray(instanceDefinition.Transforms);
				//// normalize all transforms to base position
				//this.transforms = new V2DTransform[instanceDefinition.Transforms.Length];
				//float ox = instanceDefinition.X;
				//float oy = instanceDefinition.Y;
				//for (int i = 0; i < instanceDefinition.Transforms.Length; i++)
				//{
				//    transforms[i] = instanceDefinition.Transforms[i].Clone();
				//    //transforms[i].Position.X -= ox;
				//    //transforms[i].Position.Y -= oy;
				//}
				this.State.StartFrame = instanceDefinition.StartFrame;
				this.State.EndFrame = instanceDefinition.EndFrame;
			}
		}
        public virtual bool OnPlayerInput(int playerIndex, Move move, TimeSpan time)
		{
			return true;
        }
        protected int transformIndex = 0;
        Vector2 absPosition;
        float curRot;
        //protected Matrix m = Matrix.Identity;
		protected virtual void SetCurrentState()
		{
			for (int i = 0; i < transforms.Length; i++)
			{
				if(transforms[i].StartFrame <= parent.CurChildFrame && transforms[i].EndFrame >= (parent.CurChildFrame))
				{
					transformIndex = i;
					break;
				}
			}
			V2DTransform t = transforms[transformIndex];

            //m = parent.m;
            
            //Matrix sm = Matrix.CreateScale(State.Scale.X * t.Scale.X, State.Scale.Y * t.Scale.Y, 0f);
            //Matrix rm = Matrix.CreateRotationZ(State.Rotation + t.Rotation);
            //Matrix tm = Matrix.CreateTranslation(State.Position.X + t.Position.X, State.Position.Y + t.Position.Y, 0f);
            //Matrix om = Matrix.CreateTranslation(Origin.X, Origin.Y, 0f);

            //Matrix.Multiply(ref sm, ref m, out m); // scale
            //Matrix.Subtract(ref om, ref m, out m);
            //Matrix.Multiply(ref rm, ref m, out m); // rotate
            //Matrix.Add(ref om, ref m, out m);
            //Matrix.Add(ref tm, ref m, out m); // translate

            //Vector3 s;
            //Quaternion r;
            //Vector3 tr;
            //m.Decompose(out s, out r, out tr);
            Vector2 relativePosition = State.Position + t.Position;
            relativePosition *= parent.CurrentState.Scale;

			CurrentState.Scale = parent.CurrentState.Scale * State.Scale * t.Scale;
            CurrentState.Rotation = parent.CurrentState.Rotation + State.Rotation + t.Rotation;
            float tempX = relativePosition.X;
            float cosRot = (float)Math.Cos(parent.CurrentState.Rotation);
            float sinRot = (float)Math.Sin(parent.CurrentState.Rotation);
            relativePosition.X = cosRot * tempX - sinRot * relativePosition.Y;
            relativePosition.Y = sinRot * tempX + cosRot * relativePosition.Y;

            CurrentState.Position = parent.CurrentState.Position + relativePosition;


            //absPosition = parent.CurrentState.Position + State.Position + t.Position;
            //CurrentState.Position.X = (absPosition.X - State.Origin.X) * (1f/CurrentState.Scale.X);
            //CurrentState.Position.Y = absPosition.Y - State.Origin.Y * (1f/CurrentState.Scale.Y);

            //CurrentState.Scale = parent.CurrentState.Scale * State.Scale * t.Scale;
            //absPosition = parent.CurrentState.Position + State.Position + t.Position;
            //CurrentState.Position.X = (absPosition.X - State.Origin.X) * (1f/CurrentState.Scale.X);
            //CurrentState.Position.Y = absPosition.Y - State.Origin.Y * (1f/CurrentState.Scale.Y);
            
            //curRot = State.Rotation + t.Rotation;
            //CurrentState.Rotation = parent.CurrentState.Rotation + State.Rotation + t.Rotation;

            //absPosition = (Origin) * (1f / CurrentState.Scale.X);
            //CurrentState.Position.X += (float)(Math.Cos(curRot) * absPosition.X - Math.Sin(curRot) * absPosition.Y);            
            //CurrentState.Position.Y += (float)(Math.Sin(curRot) * absPosition.X + Math.Cos(curRot) * absPosition.Y);
            
            //curRot = State.Rotation + t.Rotation;
            //float parRot = parent.CurrentState.Rotation;
            //CurrentState.Rotation = parent.CurrentState.Rotation + curRot;

            //CurrentState.Position.X = parent.CurrentState.Position.X + (float)(Math.Cos(curRot) * curPos.X - Math.Sin(curRot) * curPos.Y);
            //CurrentState.Position.Y = parent.CurrentState.Position.Y + (float)(Math.Sin(curRot) * curPos.X + Math.Cos(curRot) * curPos.Y);



            /*
			CurrentState.Scale = parent.CurrentState.Scale * State.Scale * t.Scale;
			//CurrentState.Rotation = parent.CurrentState.Rotation + State.Rotation + t.Rotation;
			//CurrentState.Position = parent.CurrentState.Position + State.Position + t.Position;
			CurrentState.Origin = State.Origin;

            if (!(this is DisplayObjectContainer) || ((DisplayObjectContainer)this).NumChildren > 0)
            {
                CurrentState.Position = parent.CurrentState.Position + State.Position + t.Position;
                CurrentState.Rotation = parent.CurrentState.Rotation + State.Rotation + t.Rotation;
            }
            else
            {
                curPos = State.Position + t.Position;
                curRot = parent.CurrentState.Rotation + State.Rotation + t.Rotation;
			    CurrentState.Rotation = parent.CurrentState.Rotation + State.Rotation + t.Rotation;
                CurrentState.Position = parent.CurrentState.Position + curPos; 
                // todo: this is needed for b2d elements, investigate
                // CurrentState.Position.X = parent.CurrentState.Position.X + (float)(Math.Cos(curRot) * curPos.X - Math.Sin(curRot) * curPos.Y);
                // CurrentState.Position.Y = parent.CurrentState.Position.Y + (float)(Math.Sin(curRot) * curPos.X + Math.Cos(curRot) * curPos.Y);
            }
            */
           
		}
		protected Rectangle destRect;
		protected SpriteEffects se = SpriteEffects.None;
        public virtual void Update(GameTime gameTime)
		{
			SetCurrentState();
			se = SpriteEffects.None;

			float xdif = 0;
			float ydif = 0;
			if (CurrentState.Scale.X < 0)
			{
				se |= SpriteEffects.FlipHorizontally;
				CurrentState.Scale.X = Math.Abs(CurrentState.Scale.X);
				xdif = Width - (Width - State.Origin.X) * 2;
				CurrentState.Origin.X += xdif;
			}
			if (CurrentState.Scale.Y < 0)
			{
				se |= SpriteEffects.FlipVertically;
				CurrentState.Scale.Y = Math.Abs(CurrentState.Scale.Y);
				ydif = Height - (Height - State.Origin.Y) * 2;
				CurrentState.Origin.Y -= ydif;
			}

			destRect = new Rectangle(
				(int)CurrentState.Position.X,
                (int)CurrentState.Position.Y,
				(int)Math.Round(CurrentState.Scale.X * Width),
                (int)Math.Round(CurrentState.Scale.Y * Height));

        }
        public virtual void Draw(SpriteBatch batch)
		{
            if (texture != null)
			{
				//batch.Draw(texture, destRect, sourceRectangle, color, CurrentState.Rotation, CurrentState.Origin, se, 1f / DepthCounter++);
				//batch.Draw(texture, destRect, sourceRectangle, color, CurrentState.Rotation, Origin, se, 1f / DepthCounter++);
                batch.Draw(texture, destRect, sourceRectangle, color, CurrentState.Rotation, Origin, se, 1f / DepthCounter++);
            }


        }

		public static int CompareTo(DisplayObject a, DisplayObject b)
		{
			int result = a.DepthGroup.CompareTo(b.DepthGroup);
			if (result == 0)
			{
				result = a.Depth.CompareTo(b.Depth);
			}
			return result;
		}
        public override bool Equals(object obj)
        {
            bool result = false;
            if(obj is DisplayObject && ((DisplayObject)obj).id == id)
            {
                result = true;
            }
            return result;
        }
        public override int GetHashCode()
        {
            return id;
        }
    }
}






