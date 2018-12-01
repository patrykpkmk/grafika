using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D
{
    public class Sphere
    {
        public VertexPositionNormalColor[] vertices;
        public short[] indices;
        float radius;
        private int nvertices, nindices;

        public Sphere(float radius)
        {
            this.radius = radius;
            nvertices = 90 * 90; // 90 vertices in a circle, 90 circles in a sphere
            nindices = 90 * 90 * 6;
            CreateSphereVertices();
            CreateIndices();
        }

        private void CreateSphereVertices()
        {
            vertices = new VertexPositionNormalColor[nvertices];
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 rad = new Vector3((float) Math.Abs(radius), 0, 0);
            for (int x = 0; x < 90; x++) //90 circles, difference between each is 4 degrees
            {
                float difx = 360.0f / 90.0f;
                for (int y = 0; y < 90; y++) //90 veritces, difference between each is 4 degrees 
                {
                    float dify = 360.0f / 90.0f;
                    Matrix zrot = Matrix.CreateRotationZ(MathHelper.ToRadians(y * dify)); //rotate vertex around z
                    Matrix yrot = Matrix.CreateRotationY(MathHelper.ToRadians(x * difx)); // rotate circle around y
                    Vector3 point = Vector3.Transform(Vector3.Transform(rad, zrot), yrot); //transformation

                    vertices[x + y * 90] = new VertexPositionNormalColor(point, GetNormal(point), Color.Green);
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
            for (int x = 0; x < 90; x++)
            {
                for (int y = 0; y < 90; y++)
                {
                    int s1 = x == 89 ? 0 : x + 1;
                    int s2 = y == 89 ? 0 : y + 1;
                    short upperLeft = (short) (x * 90 + y);
                    short upperRight = (short) (s1 * 90 + y);
                    short lowerLeft = (short) (x * 90 + s2);
                    short lowerRight = (short) (s1 * 90 + s2);
                    indices[i++] = upperLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerLeft;
                    indices[i++] = lowerLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerRight;
                }
            }
        }
    }
}