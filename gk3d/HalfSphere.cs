using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D
{
    public class HalfSphere
    {
        public VertexPositionNormalColor[] vertices;
            //later, I will provide another example with VertexPositionNormalTexture

        public short[] indices; //my laptop can only afford Reach, no HiDef :(
        float radius;
        int nvertices, nindices;
        GraphicsDevice graphics;
        private Color color;

        private int m = 64;

        public HalfSphere(float Radius, Color color)
        {
            this.color = color;
            radius = Radius;
            nvertices = m * m; // 90 vertices in a circle, 90 circles in a sphere
            nindices = m * m * 6;
            CreateSphereVertices();
            CreateIndices();

            //RotateZ(180);
            //Translate(new Vector3(0,-30,0));
            //Scale(0.4f);
        }

        private void CreateSphereVertices()
        {
            vertices = new VertexPositionNormalColor[nvertices];
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 rad = new Vector3((float) Math.Abs(radius), 0, 0);
            for (int x = 0; x < m; x++) //90 circles, difference between each is 4 degrees
            {
                float difx = 360.0f / m;
                for (int y = 0; y <= m / 2; y++) //90 veritces, difference between each is 4 degrees 
                {
                    float dify = 360.0f / m;
                    Matrix zrot = Matrix.CreateRotationZ(MathHelper.ToRadians(y * dify)); //rotate vertex around z
                    Matrix yrot = Matrix.CreateRotationY(MathHelper.ToRadians(x * difx)); // rotate circle around y
                    Vector3 point = Vector3.Transform(Vector3.Transform(rad, zrot), yrot); //transformation

                    vertices[x + y * m] = new VertexPositionNormalColor(point, GetNormal(point), color);
                }
            }
        }

        private Vector3 GetNormal(Vector3 vertex)
        {
            Vector3 normal = new Vector3(vertex.X, vertex.Y, vertex.Z);
            normal.Normalize();
            return normal;
        }

        private void CreateIndices()
        {
            indices = new short[nindices];
            int i = 0;
            for (int x = 0; x < m; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    int s1 = x == (m - 1) ? 0 : x + 1;
                    int s2 = y == (m - 1) ? 0 : y + 1;
                    short upperLeft = (short) (x * m + y);
                    short upperRight = (short) (s1 * m + y);
                    short lowerLeft = (short) (x * m + s2);
                    short lowerRight = (short) (s1 * m + s2);
                    indices[i++] = upperLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerLeft;
                    indices[i++] = lowerLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerRight;
                }
            }
        }

        public void RotateX(int degrees)
        {
            List<Vector3> posPom = new List<Vector3>();
            foreach (var point in vertices)
            {
                posPom.Add(Vector3.Transform(point.Position, Matrix.CreateRotationX(MathHelper.ToRadians(degrees))));
            }
            int i = 0;
            foreach (var point in posPom)
            {
                vertices[i] = new VertexPositionNormalColor(point, vertices[i].Normal, vertices[i].Color);
                i++;
            }
        }

        public void RotateY(int degrees)
        {
            List<Vector3> posPom = new List<Vector3>();
            foreach (var point in vertices)
            {
                posPom.Add(Vector3.Transform(point.Position, Matrix.CreateRotationY(MathHelper.ToRadians(degrees))));
            }
            int i = 0;
            foreach (var point in posPom)
            {
                vertices[i] = new VertexPositionNormalColor(point, vertices[i].Normal, vertices[i].Color);
                i++;
            }
        }

        public void RotateZ(int degrees)
        {
            List<Vector3> posPom = new List<Vector3>();
            foreach (var point in vertices)
            {
                posPom.Add(Vector3.Transform(point.Position, Matrix.CreateRotationZ(MathHelper.ToRadians(degrees))));
            }
            int i = 0;
            foreach (var point in posPom)
            {
                vertices[i] = new VertexPositionNormalColor(point, vertices[i].Normal, vertices[i].Color);
                i++;
            }
        }

        public void Translate(Vector3 translation)
        {
            List<Vector3> posPom = new List<Vector3>();
            foreach (var point in vertices)
            {
                posPom.Add(Vector3.Transform(point.Position, Matrix.CreateTranslation(translation)));
            }
            int i = 0;
            foreach (var point in posPom)
            {
                vertices[i] = new VertexPositionNormalColor(point, vertices[i].Normal, vertices[i].Color);
                i++;
            }
        }

        public void Scale(float scaleFactor)
        {
            List<Vector3> posPom = new List<Vector3>();
            foreach (var point in vertices)
            {
                posPom.Add(Vector3.Transform(point.Position, Matrix.CreateScale(scaleFactor)));
            }
            int i = 0;
            foreach (var point in posPom)
            {
                vertices[i] = new VertexPositionNormalColor(point, vertices[i].Normal, vertices[i].Color);
                i++;
            }
        }
    }
}