using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GK3D
{
    public struct VertexPositionNormalColor : IVertexType
    {
        public Vector3 _position;
        public Color Color;
        public Vector3 Normal;

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
        {
            _position = position;
            Normal = normal;
            Color = color;
        }

        public static int SizeInBytes = (3 + 3 + 1) * 4;
            // 3 floats for Position + 3 floats for Normal + 4 bytes for Color

        public static VertexElement[] VertexElements = new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
        };

        public VertexDeclaration VertexDeclaration => new VertexDeclaration(VertexElements);

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
    }
}