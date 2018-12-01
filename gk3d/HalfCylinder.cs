using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D
{
    public class HalfCylinder
    {
        public List<VertexPositionNormalColor> vertices;
        //later, I will provide another example with VertexPositionNormalTexture

        public List<short> indices; //my laptop can only afford Reach, no HiDef :(
        float radius;
        private float height;
        private Color color;
        int m = 24;
        private double angle;


        public HalfCylinder(float rad, float h, Color color)
        {
            height = h;
            radius = rad;
            angle = Math.PI / (m);
            this.color = color;
            CreateVertices();
            CreateIndices();
          
        }

        private void CreateVertices()
        {
            vertices = new List<VertexPositionNormalColor>();
            Vector3 center = new Vector3(0, 0, 0);
            int i = 0;
            var heightsCollection = new List<float>() {0, height};
            foreach (var y in heightsCollection)
            {
                for (double alfa = 0; alfa <= Math.PI; alfa += angle)
                {
                    float x = (float) (center.X + radius * Math.Cos(alfa));
                    float z = (float) (center.Z - radius * Math.Sin(alfa));
                    vertices.Add(new VertexPositionNormalColor(new Vector3(x, y, z),GetNormal(new Vector3(x,y,z)),color));
                }
            }
            vertices.Add(new VertexPositionNormalColor(new Vector3(0, 0, 0), GetNormal(new Vector3(0, 0, 1)),color));
            //vertices.Add(new VertexPositionNormalColor(new Vector3(0, (float) height / 2, 0), new Vector3(1, 0, 0),
            //    Color.Green));
            vertices.Add(new VertexPositionNormalColor(new Vector3(0, height, 0), GetNormal(new Vector3(0, 0, 1)), color));
        }
        private Vector3 GetNormal(Vector3 vertex)
        {
            Vector3 normal = new Vector3(vertex.X, vertex.Y, vertex.Z);
            normal.Normalize();
            return normal;
        }

        private void CreateIndices()
        {
            indices = new List<short>();

            //wall
            for (int i = 0; i <= m - 1; i++)
            {
                short lowerLeft = (short) i;
                short lowerRight = (short) (i + 1);
                short upperLeft = (short) (i + m + 1);
                short upperRight = (short) (i + m + 2);

                indices.Add(lowerLeft);
                indices.Add(upperLeft);
                indices.Add(lowerRight);

                indices.Add(upperLeft);
                indices.Add(upperRight);
                indices.Add(lowerRight);
            }


            //bottom flor
            for (int i = 0; i < m; i++)
            {
                indices.Add((short) (i));
                indices.Add((short) (i + 1));
                indices.Add((short) (vertices.Count - 2));
            }

            //top flor
            for (int i = m + 1; i < 2 * m + 1; i++)
            {
                indices.Add((short) (i));
                indices.Add((short) (vertices.Count - 1));
                indices.Add((short) (i + 1));
            }

            //rear wall
            //indices.Add((short)(0));
            //indices.Add((short)(vertices.Count - 1));
            //indices.Add((short)(vertices.Count - 2));

            //indices.Add((short)(0));
            //indices.Add((short)(m + 1));
            //indices.Add((short)(vertices.Count - 1));

            //indices.Add((short)(vertices.Count - 2));
            //indices.Add((short)(2 * m + 1));
            //indices.Add((short)(m));

            //indices.Add((short)(vertices.Count - 2));
            //indices.Add((short)(vertices.Count - 1));
            //indices.Add((short)(2 * m + 1));



            short lowerLeftA = (short)0;
            short upperLeftA = (short)(m + 1);
            short lowerRightA = (short)(m);
            short upperRightA = (short)(2 * m + 1);

            indices.Add(upperRightA);
            indices.Add(upperLeftA);
            indices.Add(lowerLeftA);


            indices.Add(upperRightA);
            indices.Add(lowerLeftA);

            indices.Add(lowerRightA);


        }

        public void RotateX(int degrees)
        {
            List<Vector3> posPom= new List<Vector3>();
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