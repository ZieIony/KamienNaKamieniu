using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace CompoEngine.Physics {
    internal class PhysicsObject {
        public const float PhysicScale = 102.4f;
        private Fixture _fixture;
        public Sprite Sprite;
        public List<List<Texture>> Images = new List<List<Texture>>();
        public float Speed = 25;
        private int _animation;
        private float _frame;
        public Body Body;
        public BodyDesc BodyDesc;
        private Vector2f Offset;

        public PhysicsObject(Texture image, BodyDesc body) {
            var images = new Texture[1];
            images[0] = image;

            Init(images, body, new Vector2f(0.0f, 0.0f), new Vector2f(0.0f, 0.0f));
        }

        public PhysicsObject(Texture image, BodyDesc body, Vector2f size, Vector2f offset) {
            var images = new Texture[1];
            images[0] = image;

            Init(images, body, size, offset);
        }

        public PhysicsObject(Texture[] image, BodyDesc body) {
            Init(image, body, new Vector2f(0.0f, 0.0f), new Vector2f(0.0f, 0.0f));
        }

        public PhysicsObject(string[] image, BodyDesc body) {
            Texture[] images = new Texture[image.Length];
            for (int i = 0; i < image.Length; i++) {
                images[i] = new Texture(Application.Images[image[i]]);
            }
            Init(images, body, new Vector2f(0.0f, 0.0f), new Vector2f(0.0f, 0.0f));
        }

        public void AddAnimation(string[] image) {
            Texture[] images = new Texture[image.Length];
            for (int i = 0; i < image.Length; i++) {
                images[i] = new Texture(Application.Images[image[i]]);
            }
            List<Texture> anim = new List<Texture>();
            Images.Add(anim);
            anim.AddRange(images);
        }

        private void Init(Texture[] image, BodyDesc body, Vector2f size, Vector2f _offset) {
            BodyDesc = body;
            Images.Add(new List<Texture>());
            Images[_animation].AddRange(image);
            Sprite = new Sprite(image[0]);

            if (body.Shape is FarseerPhysics.Collision.Shapes.CircleShape) {
                body.Shape.Radius = body.Size.X;
            } else if (body.Shape is PolygonShape) {
                ((PolygonShape)body.Shape).SetAsBox(body.Size.X / 2.0f, body.Size.Y / 2.0f);
            }

            Offset = _offset;

            if (size.X != 0.0f && size.Y != 0.0f) {
                Sprite.Scale = new Vector2f(size.X * PhysicScale / Sprite.Texture.Size.X, size.Y * PhysicScale / Sprite.Texture.Size.Y);
            } else {
                Sprite.Scale = new Vector2f(body.Size.X * PhysicScale / Sprite.Texture.Size.X, body.Size.Y * PhysicScale / Sprite.Texture.Size.Y);
            }


            Body = new Body(Application.World) {
                Position = body.StartPos,
                BodyType = body.BodyType,
                Restitution = body.Resitution,
                Friction = 100.0f,
                AngularDamping = 0.7f,
                LinearDamping = 0.3f
            };

            _fixture = Body.CreateFixture(body.Shape);
        }

        public int Animation {
            set {
                _animation = value;
                _frame = 0;
            }
            get { return _animation; }
        }

        public void Draw(float frameTime) {
            _frame += Speed * frameTime;
            if (_frame >= Images[_animation].Count) {
                if (_animation != 0) {
                    _animation = 0;
                    _frame = 0;
                } else {
                    _frame %= Images[_animation].Count;
                }
            }
            Sprite.Texture = Images[_animation][(int)Math.Floor(_frame)];
            Sprite.Position = new Vector2f((Body.Position.X) * PhysicScale, (Body.Position.Y) * PhysicScale) + Offset;
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2.0f, Sprite.Texture.Size.Y / 2.0f);

            Sprite.Rotation = (float)(Body.Rotation * (-180.0f) / Math.PI);
            Application.Window.Draw(Sprite);
        }
    }
}