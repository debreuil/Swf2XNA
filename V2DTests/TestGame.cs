using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DDW.Display;
using Microsoft.Xna.Framework.Input;

namespace V2DTest
{
    public class TestGame : V2DGame
    {
		public bool show3D = false;

        public override bool HasCursor { get { return true; } }

        public TestGame()
        {
            this.Content.RootDirectory = "V2DTestsContent";
        }
		protected override void CreateScreens()
        {
			stage.AddScreen(new DistanceJointDemo(new SymbolImport("DistanceJoint")));
            stage.AddScreen(new RevoluteJointDemo(new SymbolImport("RevoluteJoint")));
            stage.AddScreen(new GearJointDemo(new SymbolImport("GearJoint")));
            stage.AddScreen(new V2DScreen(new SymbolImport("PrismaticJoint")));
            stage.AddScreen(new PulleyJointDemo(new SymbolImport("PulleyJoint")));

            stage.AddScreen(new AnimationDemo(new SymbolImport("germs")));
            stage.AddScreen(new SpinnerDemo(new SymbolImport("Scene3Data")));
            stage.AddScreen(new V2DScreen(new SymbolImport("Scene1Data")));
            stage.AddScreen(new V2DScreen(new SymbolImport("Scene2Data")));

        }
        protected override void Initialize()
        {
            base.Initialize();
            camPos = new Vector3(0f, -250f, 300.0f);
            camTarget = new Vector3(0, -100, 0);

            float aspectRatio = 640.0f / 480.0f;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80F), aspectRatio, 10F, 1000F);
            view = Matrix.CreateLookAt(camPos, camTarget, Vector3.Up);

            rotations = new Vector3[MAX_PLAYERS];
            translations = new Vector3[MAX_PLAYERS];
            mX = new Matrix[MAX_PLAYERS];
            mY = new Matrix[MAX_PLAYERS];
            mZ = new Matrix[MAX_PLAYERS];
            mT = new Matrix[MAX_PLAYERS];

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                rotations[i] = new Vector3();
                translations[i] = new Vector3();
            }
            progress = new float[] { 0, .2F, .4F, .6F, .8F };
        }
        protected override void LoadContent()
        {
			base.LoadContent();

			FontManager.Instance.AddFont("Arial", V2DGame.contentManager.Load<SpriteFont>(@"Arial"));

			players = new Model[MAX_PLAYERS];
            players[0] = contentManager.Load<Model>(@"ss0");
            players[1] = contentManager.Load<Model>(@"ss1");
            players[2] = contentManager.Load<Model>(@"ss2");
            players[3] = contentManager.Load<Model>(@"ss3");
            players[4] = contentManager.Load<Model>(@"ss4");
        }
        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
			
			KeyboardState ks = Keyboard.GetState();
			if (!keyDown && ks.IsKeyDown(Keys.Left))
			{
				keyDown = true;
				stage.PreviousScreen();
			}
			else if (!keyDown && ks.IsKeyDown(Keys.Right))
			{
				keyDown = true;
				stage.NextScreen();
			}
			else if (!keyDown && ks.IsKeyDown(Keys.Space))
			{
				keyDown = true;
                show3D = !show3D;
			}
			else if (   keyDown && 
                        ks.IsKeyUp(Keys.Left) && 
                        ks.IsKeyUp(Keys.Right) && 
                        ks.IsKeyUp(Keys.Space) )
			{
				keyDown = false;
			}

            if (show3D)
            {
                UpdateShips(gameTime);
            }
        }
        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            //GraphicsDevice.RenderState.DepthBufferEnable = true;
            if (show3D)
            {
                DrawShips();
            }
        }
        public const int MAX_PLAYERS = 5;
        Vector3 camPos;
        Vector3 camTarget;
        Matrix projection;
        Matrix view;

        Model[] players;
        Matrix[] mX;
        Matrix[] mY;
        Matrix[] mZ;
        Matrix[] mT;

        Vector3[] rotations;
        Vector3[] translations;
        float[] progress;
        private void DrawShips()
        {
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                Model m = players[i];
                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        effect.View = view;
                        effect.Projection = projection;
                        effect.World = mX[i] * mY[i] * mZ[i] * mesh.ParentBone.Transform * mT[i];
                    }
                    mesh.Draw();
                }
            }
        }

        private void UpdateShips(GameTime gameTime)
        {
            float loopDur = 5;
            float t = (float)gameTime.TotalGameTime.TotalMilliseconds / (loopDur * 1000);
            t = t - (int)t;

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                float pt = t + progress[i];
                pt = pt - (int)pt;
                float pt2p = (float)(pt * System.Math.PI * 2);

                Matrix.CreateRotationX((float)System.Math.PI, out mX[i]);

                float rollPoint = .35F;
                float rollDur = .25F;
                if (pt > rollPoint && pt < rollPoint + rollDur)
                {
                    float rt = (pt - rollPoint) * 1 / rollDur;
                    Matrix.CreateRotationY(rt * (float)System.Math.PI * 2, out mY[i]);
                }
                else
                {
                    Matrix.CreateRotationY(0F, out mY[i]);
                }

                Matrix.CreateRotationZ((float)(System.Math.PI * 2) - pt2p + (float)(System.Math.PI * 1.5) - .3F, out mZ[i]);

                translations[i].X = (float)(System.Math.Sin(pt2p) * 450);
                translations[i].Y = (float)(System.Math.Cos(pt2p) * 200);
                Matrix.CreateTranslation(ref translations[i], out mT[i]);
            }
        }
    }
}
