using FarseerPhysics.Dynamics;

namespace CompoEngine.Physics {
    internal class BodyDesc {
        public BodyType BodyType;
        public Microsoft.Xna.Framework.Vector2 StartPos;
        public FarseerPhysics.Collision.Shapes.Shape Shape;
        public Microsoft.Xna.Framework.Vector2 Size;
        public float Resitution;
        public float Rotation;
    }
}