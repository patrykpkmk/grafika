using System;
using System.Collections.Generic;
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
        SpriteFont spriteFont;
        BasicEffect basicEffect;

        private Camera camera;
        private Matrix worldMatrix;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private TimeSpan frameTime;
        private DateTime lastFrameTimeUpdate;

        int planetoidSphereRadius;

        HalfCylinder halfCylinder;
        private Model rocketModel;
        private Model revolverModel;

        Sphere planetoidSphere;
        HalfSphere halfSphere;
        HalfSphere halfSphereTwo;

        private Model appleModel;

        private Vector3 appleStelliteOneTransaltion;
        private Vector3 appleStelliteTwoTransaltion;


        private Effect phongEffectForGrids;
        private Effect phongEffectForModels;
        private Vector3 viewVector;
        private Matrix[] appleModelTransforms;
        private Matrix[] robotModelTransforms;
        private Matrix[] rocketModelTransforms;
        private Matrix[] revolverModelTransforms;
        private Matrix[] untexturedSphereForReflectionModelTransforms;

        private Lights lights;

        //Textures
        private Texture2D textureChess;
        Skybox skybox;

        //EnvMapping reflection
        private Model untexturedSphereForEnvMapReflectionModel;
        private Effect effectForEnvMapReflection;
        private TextureCube textureCubeUsedForSkyBox;

        //Vector3 cameraPosition;
        float angle = 0;
        float distance = 20;


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
                camera.CameraPosition, camera.CameraTarget, camera.CameraUpVector);


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

            halfSphere = new HalfSphere(planetoidSphereRadius, Color.LightCoral);
            halfSphere.RotateZ(180);
            halfSphere.Translate(new Vector3(0, -75, 0));
            halfSphere.Scale(0.2f);

            halfSphereTwo = new HalfSphere(planetoidSphereRadius, Color.Magenta);
            halfSphereTwo.Translate(new Vector3(0, -104, 0));
            halfSphereTwo.Scale(0.2f);

            halfCylinder = new HalfCylinder(4, 8, Color.Turquoise);
            halfCylinder.Scale(0.5f);
            halfCylinder.RotateY(0);
            halfCylinder.RotateX(90);
            halfCylinder.Translate(new Vector3(0, -22.8f, 0));

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
            spriteFont = Content.Load<SpriteFont>("Font");

            basicEffect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };
            // TODO: use this.Content to load your game content here


            appleModel = Content.Load<Model>("apple");
            appleModelTransforms = new Matrix[appleModel.Bones.Count];
            appleModel.CopyAbsoluteBoneTransformsTo(appleModelTransforms);

            rocketModel = Content.Load<Model>("rocket");
            rocketModelTransforms = new Matrix[rocketModel.Bones.Count];
            rocketModel.CopyAbsoluteBoneTransformsTo(rocketModelTransforms);

            revolverModel = Content.Load<Model>("Revolver");
            revolverModelTransforms = new Matrix[revolverModel.Bones.Count];
            revolverModel.CopyAbsoluteBoneTransformsTo(revolverModelTransforms);


            phongEffectForGrids = Content.Load<Effect>("PhongGrid");
            phongEffectForModels = Content.Load<Effect>("PhongModel");

            //Texturing and skybox
            textureChess = Content.Load<Texture2D>("chess");
            string skyboxTextureName = "Sunset";
            skybox = new Skybox(skyboxTextureName, Content);

            //EnvMap Reflection 
            textureCubeUsedForSkyBox = Content.Load<TextureCube>(skyboxTextureName);
            untexturedSphereForEnvMapReflectionModel = Content.Load<Model>("UntexturedSphere");
            untexturedSphereForReflectionModelTransforms =
                new Matrix[untexturedSphereForEnvMapReflectionModel.Bones.Count];
            untexturedSphereForEnvMapReflectionModel.CopyAbsoluteBoneTransformsTo(
                untexturedSphereForReflectionModelTransforms);
            effectForEnvMapReflection = Content.Load<Effect>("ReflectionEnvMap");
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
            //MSAA
            if (keyboardState.IsKeyDown(Keys.M))
            {
                //graphics.GraphicsProfile = GraphicsProfile.HiDef;
                graphics.PreferMultiSampling = !graphics.PreferMultiSampling;

                graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            var perlins = new List<float>();
            PerlinNoiseGenerator.Reseed();
            perlins.Add(PerlinNoiseGenerator.Noise(4.0f,2.1f));
            perlins.Add(PerlinNoiseGenerator.Noise(2.0f,1.1f));

            InitializePhongEffectForGrids();
            InitializePhongEffectForModel();
            DrawSphereWithEffect();
            DrawHalfSphereWithEffect();
            DrawHalfSphereTwoWithEffect();
            DrawHalfCylinderWithEffect();

            DrawAppleSatteliteOne();
            DrawAppleSatteliteTwo();
            DrawRocket();
            DrawRevolver();

            DrawSphereModelWithEnvMapReflectionEffect(untexturedSphereForEnvMapReflectionModel, worldMatrix, viewMatrix,
                projectionMatrix);
            DrawSkyBox();

            DrawBillboards();
            DrawBillboards2();


            base.Draw(gameTime);
        }

        private void DrawBillboards()
        {
            Vector3 textPosition = new Vector3(0, 0, -60);

            basicEffect.World = Matrix.CreateConstrainedBillboard(textPosition, textPosition + camera.CameraForward,
                Vector3.Down, null, null);
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            const string message = "GRAFIKA 3D";
            Vector2 textOrigin = spriteFont.MeasureString(message) / 2;
            const float textSize = 0.25f;

            spriteBatch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullNone, basicEffect);

            spriteBatch.DrawString(spriteFont, message, Vector2.Zero, Color.White, 0, textOrigin, textSize, 0, 0);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }


        private void DrawBillboards2()
        {
            Vector3 textPosition = new Vector3(60, 0, 0);

            basicEffect.World = Matrix.CreateConstrainedBillboard(textPosition, textPosition + camera.CameraForward,
                Vector3.Down, null, null);
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            const string message = "2018";
            Vector2 textOrigin = spriteFont.MeasureString(message) / 2;
            const float textSize = 0.5f;

            spriteBatch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullNone, basicEffect);

            spriteBatch.DrawString(spriteFont, message, Vector2.Zero, Color.Black, 0, textOrigin, textSize, 0, 0);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }


        private void DrawSkyBox()
        {
            RasterizerState pom = spriteBatch.GraphicsDevice.RasterizerState;
            RasterizerState r = new RasterizerState();
            r.CullMode = CullMode.CullClockwiseFace;
            spriteBatch.GraphicsDevice.RasterizerState = r;
            skybox.Draw(viewMatrix, projectionMatrix, camera.CameraPosition);
            spriteBatch.GraphicsDevice.RasterizerState = pom;
        }

        private void InitializePhongEffectForGrids()
        {
            phongEffectForGrids.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            phongEffectForGrids.GraphicsDevice.Clear(Color.CornflowerBlue);
            phongEffectForGrids.GraphicsDevice.RasterizerState = new RasterizerState() {FillMode = FillMode.Solid};
            viewVector = camera.CameraTarget - camera.CameraPosition;
            viewVector.Normalize();

            phongEffectForGrids.Parameters["World"].SetValue(worldMatrix);
            phongEffectForGrids.Parameters["View"].SetValue(viewMatrix);
            phongEffectForGrids.Parameters["Projection"].SetValue(projectionMatrix);


            phongEffectForGrids.Parameters["AmbientColor"].SetValue(lights.DirectionalLightAmbientColor);
            phongEffectForGrids.Parameters["AmbientIntensity"].SetValue(0.02f);

            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(worldMatrix));
            phongEffectForGrids.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);

            phongEffectForGrids.Parameters["DirectionalLightDirection"].SetValue(lights.DirectionalLightDirection);
            phongEffectForGrids.Parameters["DiffuseColor"].SetValue(lights.DirectionalLightDiffuseColor);
            phongEffectForGrids.Parameters["DiffuseIntensity"].SetValue(0.9f);

            phongEffectForGrids.Parameters["Shininess"].SetValue(100f);
            phongEffectForGrids.Parameters["SpecularColor"].SetValue(lights.DirectionalLightSpecularColor);
            phongEffectForGrids.Parameters["SpecularIntensity"].SetValue(0.9f);

            phongEffectForGrids.Parameters["ViewVector"].SetValue(viewVector);


            phongEffectForGrids.Parameters["SpotlightOneLightPosition"].SetValue(lights.SpotlightOneLightPosition);
            phongEffectForGrids.Parameters["SpotlightOneSpotDirection"].SetValue(lights.SpotlightOneSpotDirection);
            phongEffectForGrids.Parameters["SpotlightOneLightRadius"].SetValue(50f);
            phongEffectForGrids.Parameters["SpotlightOneSpotDecayExponent"].SetValue(5f);
            phongEffectForGrids.Parameters["SpotlightOneSpotLightAngleCosine"].SetValue(
                (float) Math.Cos(MathHelper.ToRadians(10)));
            phongEffectForGrids.Parameters["SpotlightOneDiffuseColor"].SetValue(lights.SpotlightOneDiffuseColor);
            phongEffectForGrids.Parameters["SpotlightOneSpecularColor"].SetValue(lights.SpotlightOneSpecularColor);


            phongEffectForGrids.Parameters["SpotlightTwoLightPosition"].SetValue(lights.SpotlightTwoLightPosition);
            phongEffectForGrids.Parameters["SpotlightTwoSpotDirection"].SetValue(lights.SpotlightTwoSpotDirection);
            phongEffectForGrids.Parameters["SpotlightTwoLightRadius"].SetValue(50f);
            phongEffectForGrids.Parameters["SpotlightTwoSpotDecayExponent"].SetValue(5f);
            phongEffectForGrids.Parameters["SpotlightTwoSpotLightAngleCosine"].SetValue(
                (float) Math.Cos(MathHelper.ToRadians(20)));
            phongEffectForGrids.Parameters["SpotlightTwoDiffuseColor"].SetValue(lights.SpotlightTwoDiffuseColor);
            phongEffectForGrids.Parameters["SpotlightTwoSpecularColor"].SetValue(lights.SpotlightTwoSpecularColor);
        }

        private void InitializePhongEffectForModel()
        {
            phongEffectForModels.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            phongEffectForModels.GraphicsDevice.Clear(Color.CornflowerBlue);
            phongEffectForModels.GraphicsDevice.RasterizerState = new RasterizerState() {FillMode = FillMode.Solid};
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
            phongEffectForModels.Parameters["DiffuseIntensity"].SetValue(0.9f);

            phongEffectForModels.Parameters["Shininess"].SetValue(100f);
            phongEffectForModels.Parameters["SpecularColor"].SetValue(lights.DirectionalLightSpecularColor);
            phongEffectForModels.Parameters["SpecularIntensity"].SetValue(0.9f);

            phongEffectForModels.Parameters["ViewVector"].SetValue(viewVector);


            phongEffectForModels.Parameters["SpotlightOneLightPosition"].SetValue(lights.SpotlightOneLightPosition);
            phongEffectForModels.Parameters["SpotlightOneSpotDirection"].SetValue(lights.SpotlightOneSpotDirection);
            phongEffectForModels.Parameters["SpotlightOneLightRadius"].SetValue(50f);
            phongEffectForModels.Parameters["SpotlightOneSpotDecayExponent"].SetValue(5f);
            phongEffectForModels.Parameters["SpotlightOneSpotLightAngleCosine"].SetValue(
                (float) Math.Cos(MathHelper.ToRadians(10)));
            phongEffectForModels.Parameters["SpotlightOneDiffuseColor"].SetValue(lights.SpotlightOneDiffuseColor);
            phongEffectForModels.Parameters["SpotlightOneSpecularColor"].SetValue(lights.SpotlightOneSpecularColor);


            phongEffectForModels.Parameters["SpotlightTwoLightPosition"].SetValue(lights.SpotlightTwoLightPosition);
            phongEffectForModels.Parameters["SpotlightTwoSpotDirection"].SetValue(lights.SpotlightTwoSpotDirection);
            phongEffectForModels.Parameters["SpotlightTwoLightRadius"].SetValue(50f);
            phongEffectForModels.Parameters["SpotlightTwoSpotDecayExponent"].SetValue(5f);
            phongEffectForModels.Parameters["SpotlightTwoSpotLightAngleCosine"].SetValue(
                (float) Math.Cos(MathHelper.ToRadians(20)));
            phongEffectForModels.Parameters["SpotlightTwoDiffuseColor"].SetValue(lights.SpotlightTwoDiffuseColor);
            phongEffectForModels.Parameters["SpotlightTwoSpecularColor"].SetValue(lights.SpotlightTwoSpecularColor);

            phongEffectForModels.Parameters["ModelTexture"].SetValue(textureChess);
        }


        private void DrawSphereWithEffect()
        {
            foreach (EffectPass pass in phongEffectForGrids.CurrentTechnique.Passes)
            {
                pass.Apply();
                phongEffectForGrids.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
                    PrimitiveType.TriangleList,
                    planetoidSphere.vertices, 0,
                    planetoidSphere.vertices.Length, planetoidSphere.indices, 0, planetoidSphere.indices.Length / 3);
            }
        }

        private void DrawHalfSphereWithEffect()
        {
            foreach (EffectPass pass in phongEffectForGrids.CurrentTechnique.Passes)
            {
                pass.Apply();
                phongEffectForGrids.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
                    PrimitiveType.TriangleList,
                    halfSphere.vertices, 0,
                    halfSphere.vertices.Length, halfSphere.indices, 0, halfSphere.indices.Length / 3);
            }
        }

        private void DrawHalfSphereTwoWithEffect()
        {
            foreach (EffectPass pass in phongEffectForGrids.CurrentTechnique.Passes)
            {
                pass.Apply();
                phongEffectForGrids.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
                    PrimitiveType.TriangleList,
                    halfSphereTwo.vertices, 0,
                    halfSphereTwo.vertices.Length, halfSphereTwo.indices, 0, halfSphereTwo.indices.Length / 3);
            }
        }

        private void DrawHalfCylinderWithEffect()
        {
            foreach (EffectPass pass in phongEffectForGrids.CurrentTechnique.Passes)
            {
                pass.Apply();


                phongEffectForGrids.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
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
                    part.Effect.CurrentTechnique = part.Effect.Techniques["Textured"];
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
                    part.Effect.CurrentTechnique = part.Effect.Techniques["Textured"];
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
                    phongEffectForModels.Parameters["ModelColor"].SetValue(Color.Yellow.ToVector4());
                    phongEffectForModels.Parameters["World"].SetValue(rocketModelTransforms[mesh.ParentBone.Index] *
                                                                      Matrix.CreateRotationX(MathHelper.ToRadians(-45)) *
                                                                      Matrix.CreateScale(new Vector3(0.02f, 0.02f, 0.02f)) *
                                                                      Matrix.CreateTranslation(
                                                                          new Vector3(0f, 25f, -25f)));
                    part.Effect.CurrentTechnique = part.Effect.Techniques["NotTextured"];
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
                    part.Effect.CurrentTechnique = part.Effect.Techniques["NotTextured"];
                    part.Effect = phongEffectForModels;
                }
                mesh.Draw();
            }
        }

        private void DrawSphereModelWithEnvMapReflectionEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effectForEnvMapReflection;
                    effectForEnvMapReflection.Parameters["World"].SetValue(
                        untexturedSphereForReflectionModelTransforms[mesh.ParentBone.Index] *
                        Matrix.CreateScale(new Vector3(3f, 3f, 3f)) * Matrix.CreateTranslation(
                            new Vector3(-40f, 20f, 0f)));
                    effectForEnvMapReflection.Parameters["View"].SetValue(view);
                    effectForEnvMapReflection.Parameters["Projection"].SetValue(projection);
                    effectForEnvMapReflection.Parameters["SkyboxTexture"].SetValue(textureCubeUsedForSkyBox);
                    effectForEnvMapReflection.Parameters["CameraPosition"].SetValue(camera.CameraPosition);
                    effectForEnvMapReflection.Parameters["WorldInverseTranspose"].SetValue(
                        Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));
                }
                mesh.Draw();
            }
        }
    }
}