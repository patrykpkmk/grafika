using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GK3D
{
    public class Camera
    {
        public Camera()
        {
            CameraPosition = new Vector3(0f, 0f, 75f);
            CameraTarget = Vector3.Zero;
            CameraUpVector = new Vector3(0, 1, 0);
            CameraForward = new Vector3(0, 0, 1);
            CameraSpeed = 0.1f;
        }

        public Vector3 CameraPosition { get; set; }

        public Vector3 CameraTarget { get; set; }

        public Vector3 CameraUpVector { get; set; }

        public Vector3 CameraForward { get; set; }

        public float CameraSpeed { get; set; }

        public void Update(KeyboardState keyboardState, TimeSpan frameTime)
        {
            var cameraMoveStep = (float) (CameraSpeed * frameTime.TotalMilliseconds);

            //Forward/Backward
            if (keyboardState.IsKeyDown(Keys.F))
            {
                CameraPosition -= CameraForward * cameraMoveStep;
            }
            if (keyboardState.IsKeyDown(Keys.B))
            {
                CameraPosition += CameraForward * cameraMoveStep;
            }

            //Left/Right
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Vector3 perpendicularToForwardAndUp = Vector3.Cross(CameraUpVector, CameraForward);
                perpendicularToForwardAndUp.Normalize();
                CameraPosition -= perpendicularToForwardAndUp * cameraMoveStep;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                Vector3 perpendicularToForwardAndUp = Vector3.Cross(CameraUpVector, CameraForward);
                perpendicularToForwardAndUp.Normalize();
                CameraPosition += perpendicularToForwardAndUp * cameraMoveStep;
            }

            //Up/Down
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                CameraPosition += CameraUpVector * cameraMoveStep;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                CameraPosition -= CameraUpVector * cameraMoveStep;
            }

            //Look Up/Look Down (spogladanie gora i dol)
            if (keyboardState.IsKeyDown(Keys.U))
            {
                var perpendicularToUpAndForward = Vector3.Cross(CameraUpVector, CameraForward);
                perpendicularToUpAndForward.Normalize();
                CameraForward = Vector3.Transform(CameraForward,
                    Matrix.CreateFromAxisAngle(perpendicularToUpAndForward, MathHelper.ToRadians(2 * cameraMoveStep)));
                CameraUpVector = Vector3.Transform(CameraUpVector,
                    Matrix.CreateFromAxisAngle(perpendicularToUpAndForward, MathHelper.ToRadians(2 * cameraMoveStep)));
                CameraForward.Normalize();
                CameraUpVector.Normalize();
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                var perpendicularToUpAndForward = Vector3.Cross(CameraUpVector, CameraForward);
                perpendicularToUpAndForward.Normalize();
                CameraForward = Vector3.Transform(CameraForward,
                    Matrix.CreateFromAxisAngle(perpendicularToUpAndForward, MathHelper.ToRadians(-2 * cameraMoveStep)));
                CameraUpVector = Vector3.Transform(CameraUpVector,
                    Matrix.CreateFromAxisAngle(perpendicularToUpAndForward, MathHelper.ToRadians(-2 * cameraMoveStep)));
                CameraForward.Normalize();
                CameraUpVector.Normalize();
            }

            //Look Left/Look Right (rozlgladanie sie na boki)
            if (keyboardState.IsKeyDown(Keys.R))
            {
                CameraForward = Vector3.Transform(CameraForward,
                    Matrix.CreateFromAxisAngle(CameraUpVector, MathHelper.ToRadians(-2 * cameraMoveStep)));
                CameraForward.Normalize();
            }
            if (keyboardState.IsKeyDown(Keys.L))
            {
                CameraForward = Vector3.Transform(CameraForward,
                    Matrix.CreateFromAxisAngle(CameraUpVector, MathHelper.ToRadians(2 * cameraMoveStep)));
                CameraForward.Normalize();
            }

            //Rotate
            if (keyboardState.IsKeyDown(Keys.S))
            {
                CameraUpVector = Vector3.Transform(CameraUpVector,
                    Matrix.CreateFromAxisAngle(CameraForward, MathHelper.ToRadians(-2 * cameraMoveStep)));
                CameraUpVector.Normalize();
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                CameraUpVector = Vector3.Transform(CameraUpVector,
                    Matrix.CreateFromAxisAngle(CameraForward, MathHelper.ToRadians(2 * cameraMoveStep)));
                CameraUpVector.Normalize();
            }
        }
    }
}