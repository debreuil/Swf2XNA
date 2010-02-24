#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
#endregion

namespace DDW.Input
{
    public class Cursor : DrawableGameComponent
    {
        public delegate void MouseEvent(Vector2 position);
        public event MouseEvent MouseDown;
        public event MouseEvent MouseUp;
        public event MouseEvent MouseMove;

        public bool isInitalized = false;
        public bool isMouseDown = false;

        const float CursorSpeed = 250.0f;

        SpriteBatch spriteBatch;
        Texture2D cursorTexture;
        Vector2 textureCenter;

        private Vector2 previousPosition = Vector2.Zero;

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
        }

        public Cursor(Game game) : base(game) 
        {
        }

        // on Xbox360, initialize is overriden so that we can center the cursor once we
        // know how big the viewport will be.
#if XBOX360
        public override void Initialize()
        {            
            base.Initialize();

            Viewport vp = GraphicsDevice.Viewport;

            position.X = vp.X + (vp.Width / 2);
            position.Y = vp.Y + (vp.Height / 2);
        }
#endif

        protected override void LoadContent()
        {
            cursorTexture = Game.Content.Load<Texture2D>("cursor");
            textureCenter = new Vector2(0, 0); //cursorTexture.Width / 2, cursorTexture.Height / 2);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            isInitalized = true;

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (isInitalized)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(cursorTexture, Position, null, Color.White, 0.0f,
                    textureCenter, 1.0f, SpriteEffects.None, 0.0f);

                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            // we'll create a vector2, called delta, which will store how much the
            // cursor position should change.
            Vector2 delta = currentState.ThumbSticks.Left;

            // down on the thumbstick is -1. however, in screen coordinates, values
            // increase as they go down the screen. so, we have to flip the sign of the
            // y component of delta.
            delta.Y *= -1;

            // check the dpad: if any of its buttons are pressed, that will change delta
            // as well.
            if (currentState.DPad.Up == ButtonState.Pressed)
            {
                delta.Y = -1;
            }
            if (currentState.DPad.Down == ButtonState.Pressed)
            {
                delta.Y = 1;
            }
            if (currentState.DPad.Left == ButtonState.Pressed)
            {
                delta.X = -1;
            }
            if (currentState.DPad.Right == ButtonState.Pressed)
            {
                delta.X = 1;
            }

            // normalize delta so that we know the cursor can't move faster than
            // CursorSpeed.
            if (delta != Vector2.Zero)
            {
                delta.Normalize();
            }

#if XBOX360
            // modify position using delta, the CursorSpeed constant defined above, and
            // the elapsed game time.
            position += delta * CursorSpeed *
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            // clamp the cursor position to the viewport, so that it can't move off the
            // screen.
            Viewport vp = GraphicsDevice.Viewport;
            position.X = MathHelper.Clamp(position.X, vp.X, vp.X + vp.Width);
            position.Y = MathHelper.Clamp(position.Y, vp.Y, vp.Y + vp.Height);
#else
            MouseState mouseState = Mouse.GetState();
            position.X = mouseState.X;
            position.Y = mouseState.Y;

            if (Game.IsActive && isInitalized)
            {
                // modify position using delta, the CursorSpeed constant defined above,
                // and the elapsed game time, only if the cursor is on the screen
                Viewport vp = GraphicsDevice.Viewport;
                if ((vp.X <= position.X) && (position.X <= (vp.X + vp.Width)) &&
                    (vp.Y <= position.Y) && (position.Y <= (vp.Y + vp.Height)))
                {
                    position += delta * CursorSpeed *
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
                    position.X = MathHelper.Clamp(position.X, vp.X, vp.X + vp.Width);
                    position.Y = MathHelper.Clamp(position.Y, vp.Y, vp.Y + vp.Height);
                }
                else if (delta.LengthSquared() > 0f)
                {
                    position.X = vp.X + vp.Width / 2;
                    position.Y = vp.Y + vp.Height / 2;
                }

                // set the new mouse position using the combination of mouse and gamepad
                // data.
                Mouse.SetPosition((int)position.X, (int)position.Y);
            }

            if (!isMouseDown && mouseState.LeftButton == ButtonState.Pressed)
            {
                isMouseDown = true;
				if (MouseDown != null)
				{
					MouseDown.Invoke(position);
				}
            }
            else if (isMouseDown && mouseState.LeftButton == ButtonState.Released)
            {
                isMouseDown = false;
				if (MouseDown != null)
				{
					MouseUp.Invoke(position);
				}
            }
            else if (isMouseDown && position != previousPosition)
            {
				if (MouseMove != null)
				{
					MouseMove.Invoke(position);
				}
                previousPosition = position;
            }
#endif


            base.Update(gameTime);
        }


        // CalculateCursorRay Calculates a world space ray starting at the camera's
        // "eye" and pointing in the direction of the cursor. Viewport.Unproject is used
        // to accomplish this. see the accompanying documentation for more explanation
        // of the math behind this function.
        public Ray CalculateCursorRay(Matrix projectionMatrix, Matrix viewMatrix)
        {
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector3 nearSource = new Vector3(Position, 0f);
            Vector3 farSource = new Vector3(Position, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }
    }
}
