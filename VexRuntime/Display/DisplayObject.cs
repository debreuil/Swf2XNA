using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using VexRuntime.V2D;
using DDW.V2D;

namespace DDW.Display
{
    public class DisplayObject : IDrawable
    {
        protected Texture2D texture; // texture evetually will be a 'graphics' generic class
        public string instanceName;
        public string definitonName;

        public uint FrameCount = 1;
        public uint StartFrame = 0;
        public uint TotalFrames = 1;
        public uint CurFrame = 0;
        public float CurFrameTime = 0;
        public int totalMilliseconds = (int)(1000f / 12f);
        protected bool isPlaying = false;

        protected Vector2 origin;
        protected Rectangle sourceRectangle;
        protected V2DRectangle destinationRectangle;
        protected float rotation = 0;
        protected Vector2 scale = Vector2.One;
        protected Color color = Color.White;
        protected float layerDepth;
        protected SpriteEffects spriteEffects = SpriteEffects.None;

        protected float alpha = 1;
        protected bool visible = true;
        protected V2DTransform[] transforms;
        protected DisplayObjectContainer parent;
        protected Stage stage;
        protected Screen screen;
        protected float mspf;

        private int id;
        private static int idCounter = int.MinValue;
        private bool isOnStage = false;

        #region Properties
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
                    destinationRectangle = new V2DRectangle(0, 0, texture.Width, texture.Height);
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
                return definitonName;
            }
            set
            {
                definitonName = value;
            }
        }
        public Vector2 Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
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
        public float X
        {
            get
            {
                return destinationRectangle.X;
            }
            set
            {
                destinationRectangle.X = value;
            }
        }
        public float Y
        {
            get
            {
                return destinationRectangle.Y;
            }
            set
            {
                destinationRectangle.Y = value;
            }
        }
        public float Width
        {
            get
            {
                return destinationRectangle.Width;
            }
            set
            {
                destinationRectangle.Width = value;
            }
        }
        public float Height
        {
            get
            {
                return destinationRectangle.Height;
            }
            set
            {
                destinationRectangle.Height = value;
            }
        }
        public V2DRectangle DestinationRectangle
        {
            get
            {
                return destinationRectangle;
            }
            set
            {
                destinationRectangle = value;
            }
        }
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }
        public Vector2 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
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
        public float LayerDepth
        {
            get
            {
                return layerDepth;
            }
            set
            {
                layerDepth = value;
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
                return alpha;
            }
            set
            {
                alpha = value;
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
        #endregion

        public DisplayObject()
        {
            id = idCounter++;
        }

        public virtual void Initialize()
        {
        }
        public virtual DisplayObject Clone()
        {
            DisplayObject result = new DisplayObject();
            result.texture = texture;
            result.InstanceName = InstanceName;
            result.DefinitionName = DefinitionName;
            result.origin = origin;
            result.sourceRectangle = sourceRectangle;
            result.destinationRectangle = destinationRectangle;
            result.rotation = rotation;
            result.scale = scale;
            result.color = color;
            result.layerDepth = layerDepth;
            result.spriteEffects = spriteEffects;
            result.alpha = alpha;
            result.visible = visible;
            result.transforms = transforms;
            result.parent = parent;
            result.stage = stage;
            result.id = idCounter++;
            return result;
        }

        private Stage GetStage()
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
        private Screen GetScreen()
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
                result = null;
            }
            return result;
        }
        protected Screen GetContainerScreen(DisplayObject obj)
        {
            Screen result = null;
            while (obj.parent != null && !(obj.parent is Screen) && obj.parent is DisplayObjectContainer)
            {
                obj = (DisplayObjectContainer)obj.parent;
            }

            if (obj.parent is Screen)
            {
                result = (Screen)obj.parent;
            }
            return result;
        }

        public virtual void Play()
        {
            isPlaying = true;
        }
        public virtual void Stop()
        {
            isPlaying = false;
        }
        public virtual void GotoAndPlay(uint frame)
        {
            CurFrame = frame < 1 ? 1 : frame > TotalFrames ? TotalFrames : frame;
            isPlaying = true;
        }
        public virtual void GotoAndStop(uint frame)
        {
            CurFrame = frame < 1 ? 1 : frame > TotalFrames ? TotalFrames : frame;
            isPlaying = false;
        }

        public virtual void Added(EventArgs e)
        {
            if (!isOnStage)
            {
                stage = GetStage();
                screen = GetScreen();
                mspf = (screen == null) ? stage.MillisecondsPerFrame : screen.MillisecondsPerFrame;
                this.AddedToStage(e);
                isOnStage = true;
            }
        }
        public virtual void Removed(EventArgs e)
        {
            if (isOnStage)
            {
                this.RemovedFromStage(e);
                isOnStage = false;
                stage = null;
            }
            this.parent = null;
        }
        public virtual void AddedToStage(EventArgs e)
        {
            stage.ObjectAddedToStage(this);
        }
        public virtual void RemovedFromStage(EventArgs e)
        {
           stage.ObjectRemovedFromStage(this);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (parent != null && parent.isPlaying)
            {
                CurFrameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                CurFrameTime %= totalMilliseconds;
                CurFrame = ((uint)(CurFrameTime / mspf)) % (TotalFrames);
                Rotation = this.transforms[CurFrame].Rotation;
            }
        }
        public virtual void Draw(SpriteBatch batch)
        {
                if (CurFrame > 1)
                {
                    int x = 5;
                }
            if (texture != null)
            {
                Vector2 gOffset = GetGlobalOffset(Vector2.Zero);
                float gRotation = GetGlobalRotation(rotation);
                batch.Draw(texture, gOffset, sourceRectangle, color,
                    gRotation, origin, scale, spriteEffects, layerDepth);
            }
        }

        public void LocalToGlobal(ref float x, ref float y)
        {
            if (parent != null)
            {
                parent.LocalToGlobal(ref x, ref y);
            }
            x += this.X;
            y += this.Y;
        }
        public Vector2 LocalToGlobal(Vector2 p)
        {
            if (parent != null)
            {
                p = parent.LocalToGlobal(p);
            }
            p.X += this.X;
            p.Y += this.Y;
            return p;
        }
        public Vector2 GetGlobalOffset(Vector2 offset)
        {
            if (parent != null)
            {
                offset = parent.GetGlobalOffset(offset);
            }
            offset.X += this.destinationRectangle.X;
            offset.Y += this.destinationRectangle.Y;
            return offset;
        }
        public float GetGlobalRotation(float rot)
        {
            if (parent != null)
            {
                rot = parent.GetGlobalRotation(rot);
            }
            rot += this.rotation;
            return rot;
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
