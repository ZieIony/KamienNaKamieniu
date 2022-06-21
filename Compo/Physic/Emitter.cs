using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using FarseerPhysics.Dynamics;
using SFML.System;

namespace CompoEngine.Physic {
	class Particle {
		public Vector2f pos;
		public float time;
		public float _frame;
		public Vector2f vel;
		public Particle(Vector2f pos,Vector2f vel) {
			this.pos = pos;
			this.vel = vel;
			time = 0;
			_frame = 0;
		}
	}
	class Emitter {
		public float spawnRate = 0.002f;
		public float lifeSpan = 0.3f;
		public Sprite Sprite;
		public List<Texture> Images = new List<Texture>();
		public List<Particle> particles = new List<Particle>();
		public float Speed = 1;
		public float time = 0;
		Vector2f pos;
		public float velScale = 0.2f;
		public float emitterLifeSpan = 0.3f;
		public float totalTime = 0;
		public Vector2f force;
		public PhysicObject block;
		public float scale =1.0f;

		public bool HasFinished {
			get {
				return emitterLifeSpan < totalTime&&particles.Count==0;
			}
		}

		public Emitter(Texture image,Vector2f pos,Vector2f force) {
			var images = new Texture[1];
			images[0] = image;

			Init(images, pos, force);
		}

		public Emitter(Texture[] image, Vector2f pos, Vector2f force) {
			Init(image, pos, force);
		}

		private void Init(Texture[] image, Vector2f pos, Vector2f force) {
			Images.AddRange(image);
			Sprite = new Sprite(image[0]);
			this.force = force;
			this.pos = pos;
		}

		public void Draw(float dt) {
			if (block!=null) {
				pos.X = block.Body.Position.X*PhysicObject.PhysicScale;
				pos.Y = block.Body.Position.Y * PhysicObject.PhysicScale;
			}
			time += dt;
			totalTime += dt;
			Random random = new Random();
			while (time > spawnRate&&totalTime<emitterLifeSpan) {
				time -= spawnRate;
				particles.Add(new Particle(new Vector2f(pos.X, pos.Y), new Vector2f(velScale*(float)(random.NextDouble() * 2 - 1+force.X),velScale*(float)( random.NextDouble() * 2 - 1+force.Y))));
			}
			for(int i=0;i<particles.Count;) {
				Particle p = particles[i];
				p.time += dt;
				if (p.time > lifeSpan) {
					particles.RemoveAt(i);
					continue;
				}
				p.pos.X += p.vel.X;
				p.pos.Y += p.vel.Y;
				Sprite.Color = new Color(255,255,255,(byte)((p.time<lifeSpan/2?p.time/2 / lifeSpan:0.5-p.time/2/lifeSpan)*2*255));
				p._frame += Speed * dt;
				p._frame %= Images.Count;
				Sprite.Texture = Images[(int)Math.Floor(p._frame)];
				Sprite.Position = new Vector2f(p.pos.X,p.pos.Y);
				Sprite.Scale = new Vector2f(scale, scale);
				Application.Window.Draw(Sprite);
				i++;
			}
		}
	}
}
