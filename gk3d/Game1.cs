using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GK3D
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Camera camera;
        private Matrix worldMatrix;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private TimeSpan frameTime;
        private DateTime lastFrameTimeUpdate;

        int planetoidSphereRadius;

        HalfCylinder halfCylinder;
        private Model robotModel;
        private Model rocketModel;
        private Model revolverModel;

        Sphere planetoidSphere;
        HalfSphere halfSphere;
        HalfSphere halfSphereTwo;

        private Model appleModel;

        private Vector3 appleStelliteOneTransaltion;
        private Vector3 appleStelliteTwoTransaltion;


        private Effect phongEffect;
        private Effect phongEffectForSphere;
        private Effect phongEffectForModels;
        private Vector3 viewVector;
        private Matrix[] appleModelTransforms;
        private Matrix[] robotModelTransforms;
        private Matrix[] rocketModelTransforms;
        private Matrix[] revolverModelTransforms;

        private Lights lights;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            lights = new Lights();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // TODO: Add your drawing code here

            //CAMERA MATRIX
            camera = new Camera();

            viewMatrix = Matrix.CreateLookAt(
                camera.CameraPosition,  camera.CameraTarget, camera.CameraUpVector);


            //WORLD MATRIX
            worldMatrix = Matrix.CreateWorld(camera.CameraTarget, Vector3.
                Forward, Vector3.Up);

            //PROJECTION MATRIX
            // We want the aspect ratio of our display to match
            // the entire screen's aspect ratio:
            float aspectRatio =
                graphics.PreferredBackBufferWidth / (float) graphics.PreferredBackBufferHeight;
            // Field of view measures how wide of a view our camera has.
            // Increasing this value means it has a wider view, making everything
            // on screen smaller. This is conceptually the same as "zooming out".
            // It also 
            float fieldOfView = MathHelper.ToRadians(45f);
            // Anything closer than this will not be drawn (will be clipped)
            float nearClipPlane = 1f;
            // Anything further than this will not be drawn (will be clipped)
            float farClipPlane = 1000f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView, aspectRatio, nearClipPlane, farClipPlane);


            //basicEffect = new BasicEffect(graphics.GraphicsDevice);
            //basicEffect.Alpha = 1f;
            //this.basicEffect.VertexColorEnabled = true;
            //basicEffect.GraphicsDevice.Clear(Color.CornflowerBlue);

            planetoidSphereRadius = 15;
            planetoidSphere = new Sphere(planetoidSphereRadius);

            halfSphere = new HalfSphere(planetoidSphereRadius, Color.Black);
            halfSphere.RotateZ(180);
            halfSphere.Translate(new Vector3(0, -75, 0));
            halfSphere.Scale(0.2f);

            halfSphereTwo = new HalfSphere(planetoidSphereRadius, Color.Magenta);
            halfSphereTwo.Translate(new Vector3(0, -104, 0));
            halfSphereTwo.Scale(0.2f);

            halfCylinder = new HalfCylinder(4, 8, Color.Blue);
            halfCylinder.Scale(0.5f);
            halfCylinder.RotateY(180);
            halfCylinder.Translate(new Vector3(0, -24.1f, 0));

            appleStelliteOneTransaltion = new Vector3(25, 25, 0);
            appleStelliteTwoTransaltion = new Vector3(-25, 0, 0);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here


            appleModel = Content.Load<Model>("apple");
            appleModelTransforms = new Matrix[appleModel.Bones.Count];
            appleModel.CopyAbsoluteBoneTransformsTo(appleModelTransforms);

            robotModel = Content.Load<Model>("robot");
            robotModelTransforms = new Matrix[robotModel.Bones.Count];
            robotModel.CopyAbsoluteBoneTransformsTo(robotModelTransforms);

            rocketModel = Content.Load<Model>("rocket");
            rocketModelTransforms = new Matrix[rocketModel.Bones.Count];
            rocketModel.CopyAbsoluteBoneTransformsTo(rocketModelTransforms);

            revolverModel = Content.Load<Model>("Revolver");
            revolverModelTransforms = new Matrix[revolverModel.Bones.Count];
            revolverModel.CopyAbsoluteBoneTransformsTo(revolverModelTransforms);

            phongEffect = Content.Load<Effect>("Phong");
            phongEffectForSphere = Content.Load<Effect>("SpherePhongSpotlight");
            phongEffectForModels = Content.Load<Effect>("PhongModel");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            frameTime = DateTime.Now - lastFrameTimeUpdate;
            if (frameTime.TotalMilliseconds == 0) return;
            var keyboardState = Keyboard.GetState();

            camera.Update(keyboardState, frameTime);
            viewMatrix = Matrix.CreateLookAt(camera.CameraPosition, camera.CameraPosition - camera.CameraForward,
                camera.CameraUpVector);

            lastFrameTimeUpdate = DateTime.Now;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            InitializePhongEffectForGrid();
            InitializePhongEffectForSphere();
            InitializePhongEffectForModel();
            DrawSphereWithEffect();
            DrawHalfSphereWithEffect();
            DrawHalfSphereTwoWithEffect();
            DrawHalfCylinderWithEffect();

            DrawAppleSatteliteOne();
            DrawAppleSatteliteTwo();
            DrawRocket();
            DrawRevolver();

            base.Draw(gameTime);
        }

        private void InitializePhongEffectForGrid()
        {
            phongEffect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            phongEffect.GraphicsDevice.Clear(Color.CornflowerBlue);
            phongEffect.GraphicsDevice.RasterizerState = new RasterizerState() {FillMode = FillMode.Solid };
            viewVector = camera.CameraTarget - camera.CameraPosition;
            viewVector.Normalize();

            phongEffect.Parameters["World"].SetValue(worldMatrix);
            phongEffect.Parameters["View"].SetValue(viewMatrix);
            phongEffect.Parameters["Projection"].SetValue(projectionMatrix);

            phongEffect.Parameters["World"].SetValue(worldMatrix);
            phongEffect.Parameters["View"].SetValue(viewMatrix);
            phongEffect.Parameters["Projection"].SetValue(projectionMatrix);

            phongEffect.Parameters["AmbientColor"].SetValue(lights.DirectionalLightAmbientColor);
            phongEffect.Parameters["AmbientIntensity"].SetValue(0.02f);

            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(worldMatrix));
            phongEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);

            phongEffect.Parameters["DirectionalLightDirection"].SetValue(lights.DirectionalLightDirection);
            phongEffect.Parameters["DiffuseColor"].SetValue(lights.DirectionalLightDiffuseColor);
            phongEffect.Parameters["DiffuseIntensity"].SetValue(0.75f);

            phongEffect.Parameters["Shininess"].SetValue(100f);
            phongEffect.Parameters["SpecularColor"].SetValue(lights.DirectionalLightSpecularColor);
            phongEffect.Parameters["SpecularIntensity"].SetValue(0.5f);

            phongEffect.Parameters["ViewVector"].SetValue(viewVector);
        }

        private void InitializePhongEffectForSphere()
        {
            phongEffectForSphere.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            phongEffectForSphere.GraphicsDevice.Clear(Color.CornflowerBlue);
            phongEffectForSphere.GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid };
            viewVector = camera.CameraTarget - camera.CameraPosition;
            viewVector.Normalize();

            phongEffectForSphere.Parameters["World"].SetValue(worldMatrix);
            phongEffectForSphere.Parameters["View"].SetValue(viewMatrix);
            phongEffectForSphere.Parameters["Projection"].SetValue(projectionMatrix);


            phongEffectForSphere.Parameters["AmbientColor"].SetValue(lights.DirectionalLightAmbientColor);
            phongEffectForSphere.Parameters["AmbientIntensity"].SetValue(0.02f);

            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(worldMatrix));
            phongEffectForSphere.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);

            phongEffectForSphere.Parameters["DirectionalLightDirection"].SetValue(lights.DirectionalLightDirection);
            phongEffectForSphere.Parameters["DiffuseColor"].SetValue(lights.DirectionalLightDiffuseColor);
            phongEffectForSphere.Parameters["DiffuseIntensity"].SetValue(0.75f);

            phongEffectForSphere.Parameters["Shininess"].SetValue(100f);
            phongEffectForSphere.Parameters["SpecularColor"].SetValue(lights.DirectionalLightSpecularColor);
            phongEffectForSphere.Parameters["SpecularIntensity"].SetValue(0.5f);

            phongEffectForSphere.Parameters["ViewVector"].SetValue(viewVector);


            phongEffectForSphere.Parameters["SpotlightOneLightPosition"].SetValue(lights.SpotlightOneLightPosition);
            phongEffectForSphere.Parameters["SpotlightOneSpotDirection"].SetValue(lights.SpotlightOneSpotDirection);
            phongEffectForSphere.Parameters["SpotlightOneLightRadius"].SetValue(50f);
            phongEffectForSphere.Parameters["SpotlightOneSpotDecayExponent"].SetValue(5f);
            phongEffectForSphere.Parameters["SpotlightOneSpotLightAngleCosine"].SetValue((float)Math.Cos(MathHelper.ToRadians(10)));
            phongEffectForSphere.Parameters["SpotlightOneDiffuseColor"].SetValue(lights.SpotlightOneDiffuseColor);
            phongEffectForSphere.Parameters["SpotlightOneSpecularColor"].SetValue(lights.SpotlightOneSpecularColor);


            phongEffectForSphere.Parameters["SpotlightTwoLightPosition"].SetValue(lights.SpotlightTwoLightPosition);
            phongEffectForSphere.Parameters["SpotlightTwoSpotDirection"].SetValue(lights.SpotlightTwoSpotDirection);
            phongEffectForSphere.Parameters["SpotlightTwoLightRadius"].SetValue(50f);
            phongEffectForSphere.Parameters["SpotlightTwoSpotDecayExponent"].SetValue(5f);
            phongEffectForSphere.Parameters["SpotlightTwoSpotLightAngleCosine"].SetValue((float)Math.Cos(MathHelper.ToRadians(20)));
            phongEffectForSphere.Parameters["SpotlightTwoDiffuseColor"].SetValue(lights.SpotlightTwoDiffuseColor);
            phongEffectForSphere.Parameters["SpotlightTwoSpecularColor"].SetValue(lights.SpotlightTwoSpecularColor);


        }

        private void InitializePhongEffectForModel()
        {
            phongEffectForModels.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            phongEffectForModels.GraphicsDevice.Clear(Color.CornflowerBlue);
            phongEffectForModels.GraphicsDevice.RasterizerState = new RasterizerState() {FillMode = FillMode.Solid };
            viewVector = camera.CameraTarget - camera.CameraPosition;
            viewVector.Normalize();

            phongEffectForModels.Parameters["World"].SetValue(worldMatrix);
            phongEffectForModels.Parameters["View"].SetValue(viewMatrix);
            phongEffectForModels.Parameters["Projection"].SetValue(projectionMatrix);

            phongEffectForModels.Parameters["AmbientColor"].SetValue(lights.DirectionalLightAmbientColor);
            phongEffectForModels.Parameters["AmbientIntensity"].SetValue(0.02f);

            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(worldMatrix));
            phongEffectForModels.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);

            phongEffectForModels.Parameters["DirectionalLightDirection"].SetValue(lights.DirectionalLightDirection);
            phongEffectForModels.Parameters["DiffuseColor"].SetValue(lights.DirectionalLightDiffuseColor);
            phongEffectForModels.Parameters["DiffuseIntensity"].SetValue(0.75f);

            phongEffectForModels.Parameters["Shininess"].SetValue(100f);
            phongEffectForModels.Parameters["SpecularColor"].SetValue(lights.DirectionalLightSpecularColor);
            phongEffectForModels.Parameters["SpecularIntensity"].SetValue(0.5f);

            phongEffectForModels.Parameters["ViewVector"].SetValue(viewVector);
        }


        private void DrawSphereWithEffect()
        {
            foreach (EffectPass pass in phongEffectForSphere.CurrentTechnique.Passes)
            {
                pass.Apply();
                phongEffectForSphere.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
                    PrimitiveType.TriangleList,
                    planetoidSphere.vertices, 0,
                    planetoidSphere.vertices.Length, planetoidSphere.indices, 0, planetoidSphere.indices.Length / 3);
            }
        }

        private void DrawHalfSphereWithEffect()
        {
            foreach (EffectPass pass in phongEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                phongEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
                    PrimitiveType.TriangleList,
                    halfSphere.vertices, 0,
                    halfSphere.vertices.Length, halfSphere.indices, 0, halfSphere.indices.Length / 3);
            }
        }

        private void DrawHalfSphereTwoWithEffect()
        {
            foreach (EffectPass pass in phongEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                phongEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
                    PrimitiveType.TriangleList,
                    halfSphereTwo.vertices, 0,
                    halfSphereTwo.vertices.Length, halfSphereTwo.indices, 0, halfSphereTwo.indices.Length / 3);
            }
        }

        private void DrawHalfCylinderWithEffect()
        {
            foreach (EffectPass pass in phongEffect.CurrentTechnique.Passes)
            {
                pass.Apply();


                phongEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
                    PrimitiveType.TriangleList,
                    halfCylinder.vertices.ToArray(), 0,
                    halfCylinder.vertices.Count, halfCylinder.indices.ToArray(), 0, halfCylinder.indices.Count / 3);
            }
        }

        private void DrawAppleSatteliteOne()
        {
            foreach (ModelMesh mesh in appleModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    phongEffectForModels.Parameters["ModelColor"].SetValue(Color.Red.ToVector4());
                    phongEffectForModels.Parameters["World"].SetValue(appleModelTransforms[mesh.ParentBone.Index] *
                                                                      Matrix.CreateTranslation(
                                                                          appleStelliteOneTransaltion) * worldMatrix);
                    part.Effect = phongEffectForModels;
                }
                mesh.Draw();
            }
        }

        private void DrawAppleSatteliteTwo()
        {
            foreach (ModelMesh mesh in appleModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    phongEffectForModels.Parameters["ModelColor"].SetValue(Color.Red.ToVector4());
                    phongEffectForModels.Parameters["World"].SetValue(appleModelTransforms[mesh.ParentBone.Index] *
                                                                      Matrix.CreateTranslation(
                                                                          appleStelliteTwoTransaltion) * worldMatrix);

                    part.Effect = phongEffectForModels;
                }
                mesh.Draw();
            }
        }

        private void DrawRocket()
        {
            foreach (ModelMesh mesh in rocketModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    phongEffectForModels.Parameters["ModelColor"].SetValue(Color.Brown.ToVector4());
                    phongEffectForModels.Parameters["World"].SetValue(rocketModelTransforms[mesh.ParentBone.Index] *
                                                                      Matrix.CreateRotationX(MathHelper.ToRadians(-45)) *
                                                                      Matrix.CreateScale(new Vector3(0.02f, 0.02f, 0.02f)) *
                                                                      Matrix.CreateTranslation(
                                                                          new Vector3(0f, 25f, -25f)));
                    part.Effect = phongEffectForModels;
                }
                mesh.Draw();
            }
        }

        private void DrawRevolver()
        {
            foreach (ModelMesh mesh in revolverModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    phongEffectForModels.Parameters["ModelColor"].SetValue(Color.Orange.ToVector4());
                    phongEffectForModels.Parameters["World"].SetValue(revolverModelTransforms[mesh.ParentBone.Index] *
                                                                      Matrix.CreateScale(new Vector3(0.01f, 0.01f, 0.01f)) *
                                                                      Matrix.CreateRotationY(MathHelper.ToRadians(90)) *
                                                                      //Matrix.CreateRotationZ(MathHelper.ToRadians(-5)) *
                                                                      Matrix.CreateTranslation(
                                                                          new Vector3(-40f, 0f, 0f)));
                    part.Effect = phongEffectForModels;
                }
                mesh.Draw();
            }
        }
    }
}