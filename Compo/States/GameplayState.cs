using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CompoEngine.Physic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using SFML.System;

namespace CompoEngine.States
{
    internal class GameplayState : IState
    {
        private Font font;
        private Text timerText;
        private Emitter gwiazdkaEmitter;
        private Fixture _currentFixture;
        private FixedMouseJoint currentMouseJoint;
        private PhysicObject _ground1, _ground2, _ground3;

        private Collection<Brick> bricks { get; set; }
        private Stopwatch _shootingStopwatch;
        private Stopwatch _roundStopwatch;
        private float roundLength = 12000.0f;
        private Text currentPlayerText;
        private Stopwatch _dropPackagesStopwatch;
		private Collection<Ball> balls { get; set; }
		private Collection<Emitter> emitters { get; set; }
		SFML.Audio.Music music;
        private Side currentPlayer { get; set; }
        private float gameOverLineOffset = 4.75f;        
        private float gameOverSpriteScale = 0.0f;
        private Sprite gameOverLeftSprite, gameOverRightSprite;
        private Sprite leftPlayerSprite;
        private Sprite rightPlayerSprite;
        private Sprite backgroundSprite;

        private Sprite blackSprite;
        private float blackoutAmount;

		Random rand = new Random();
        private Side wygrywaSide;
		private int goalpha = 0;


        public void OnStateEntered(IState previousState)
        {
            bricks = new Collection<Brick>();
            balls = new Collection<Ball>();
            emitters = new Collection<Emitter>();
            gameOverLeftSprite = new Sprite(new Texture(Application.Images["gameoverScreenLeft"]));
            gameOverLeftSprite.Scale = new Vector2f(1024.0f / gameOverLeftSprite.Texture.Size.X, 768.0f / gameOverLeftSprite.Texture.Size.Y);
            gameOverRightSprite = new Sprite(new Texture(Application.Images["gameoverScreenRight"]));
            gameOverRightSprite.Scale = new Vector2f(1024.0f / gameOverRightSprite.Texture.Size.X, 768.0f / gameOverRightSprite.Texture.Size.Y);
            backgroundSprite = new Sprite(new Texture(Application.Images["bg"]));
            backgroundSprite.Scale = new Vector2f(1024.0f/backgroundSprite.Texture.Size.X, 768.0f/backgroundSprite.Texture.Size.Y);
            blackSprite = new Sprite(new Texture(Application.Images["splash"]));
            blackoutAmount = 255;

            leftPlayerSprite = new Sprite(new Texture(Application.Images["head_blue"]));
            leftPlayerSprite.Position = new Vector2f(0.0f, 0.0f);
            leftPlayerSprite.Scale = new Vector2f(1.3f, 1.3f);

            rightPlayerSprite = new Sprite(new Texture(Application.Images["head_red"]));
            rightPlayerSprite.Position = new Vector2f(1024, 0.0f);
			rightPlayerSprite.Origin = new Vector2f(100, 0);
			rightPlayerSprite.Scale = new Vector2f(0.5f, 0.5f);

            AddBrick(new Vector2f(1.25f, 5.25f), 0.0f,  Side.Left, BrickType.Wall1);
            AddBrick(new Vector2f(1.75f, 5.25f), 0.0f, Side.Left, BrickType.Wall1);
            AddBrick(new Vector2f(2.25f, 5.25f), 0.0f, Side.Left, BrickType.Wall1);
            AddBrick(new Vector2f(2.75f, 5.25f), 0.0f, Side.Left, BrickType.Wall1);
            AddBrick(new Vector2f(1.25f, 4.75f), 0.0f, Side.Left, BrickType.Wall2);
            AddBrick(new Vector2f(1.75f, 4.75f), 0.0f, Side.Left, BrickType.Wall2);
            AddBrick(new Vector2f(2.25f, 4.75f), 0.0f, Side.Left, BrickType.Wall2);
            AddBrick(new Vector2f(2.75f, 4.75f), 0.0f, Side.Left, BrickType.Shooting);
            AddBrick(new Vector2f(1.25f, 4.25f), 0.0f, Side.Left, BrickType.Wall3);
            AddBrick(new Vector2f(2.0f, 4.25f), 0.0f, Side.Left, BrickType.Wall3);
            AddBrick(new Vector2f(2.75f, 4.25f), 0.0f, Side.Left, BrickType.Wall3);
            AddBrick(new Vector2f(2.0f, 3.75f), 0.0f, Side.Left, BrickType.Wall4);


            AddBrick(new Vector2f(7.25f, 5.25f), 0.0f, Side.Right, BrickType.Wall1);
            AddBrick(new Vector2f(7.75f, 5.25f), 0.0f, Side.Right, BrickType.Wall1);
            AddBrick(new Vector2f(8.25f, 5.25f), 0.0f, Side.Right, BrickType.Wall1);
            AddBrick(new Vector2f(8.75f, 5.25f), 0.0f, Side.Right, BrickType.Wall1);
            AddBrick(new Vector2f(7.25f, 4.75f), (float) Math.PI, Side.Right, BrickType.Shooting);
            AddBrick(new Vector2f(7.75f, 4.75f), 0.0f, Side.Right, BrickType.Wall2);
            AddBrick(new Vector2f(8.25f, 4.75f), 0.0f, Side.Right, BrickType.Wall2);
            AddBrick(new Vector2f(8.75f, 4.75f), 0.0f, Side.Right, BrickType.Wall2);
            AddBrick(new Vector2f(7.25f, 4.25f), 0.0f, Side.Right, BrickType.Wall3);
            AddBrick(new Vector2f(8.0f, 4.25f), 0.0f, Side.Right, BrickType.Wall3);
            AddBrick(new Vector2f(8.75f, 4.25f), 0.0f, Side.Right, BrickType.Wall3);
            AddBrick(new Vector2f(8.0f, 3.75f), 0.0f, Side.Right, BrickType.Wall4);

            currentPlayer = Side.Left;
            _shootingStopwatch = new Stopwatch();
            _roundStopwatch = new Stopwatch();
            _dropPackagesStopwatch = new Stopwatch();
            _dropPackagesStopwatch.Restart();
            _shootingStopwatch.Restart();
            _roundStopwatch.Restart();

            SetupGround();

            gwiazdkaEmitter = new Emitter(new Texture[] {
                new Texture(Application.Images["fire_00000"]),
                new Texture(Application.Images["fire_00005"]),
                new Texture(Application.Images["fire_00010"]),
                new Texture(Application.Images["fire_00015"]),
                new Texture(Application.Images["fire_00020"])
            }, new Vector2f(100, 100), new Vector2f(0, -5));
            gwiazdkaEmitter.velScale = 0.02f;
			gwiazdkaEmitter.lifeSpan = 0.5f;
			gwiazdkaEmitter.Speed = 20;
			gwiazdkaEmitter.emitterLifeSpan = 100;
			gwiazdkaEmitter.spawnRate = 0.05f;
			gwiazdkaEmitter.scale = 0.3f;

            // Create a graphical string to display
			font = new Font("assets/BRLNSDB.TTF");
            timerText = new Text("0", font);
            currentPlayerText = new Text("Left", font, 50);

			music = new SFML.Audio.Music("assets/music1.ogg");
			music.Play();

            Application.World.ContactManager.OnBroadphaseCollision += OnBroadphaseCollision;
        }

        private void SetupGround()
        {
            var bodyDesc = new BodyDesc
                {
                    StartPos = new Microsoft.Xna.Framework.Vector2(1.729f, 6.5f),
                    BodyType = BodyType.Static,
                    Shape = new PolygonShape(0),
                    Size =
                        new Microsoft.Xna.Framework.Vector2(3.457f, 2.4f)
                };
            _ground1 = new PhysicObject(new Texture(Application.Images["ground_left"]), bodyDesc, new Vector2f(0f, 0f), new Vector2f(0f,-20.0f));

            var ground2desc = new BodyDesc {
                StartPos = new Microsoft.Xna.Framework.Vector2(10.0f - 1.729f, 6.5f),
                BodyType = BodyType.Static,
                Shape = new PolygonShape(0),
                Size =
                    new Microsoft.Xna.Framework.Vector2(3.457f, 2.4f)
            };
            _ground2 = new PhysicObject(new Texture(Application.Images["ground_right"]), ground2desc, new Vector2f(0f, 0f), new Vector2f(0f, -20.0f));

            var wall1desc = new BodyDesc {
                StartPos = new Microsoft.Xna.Framework.Vector2(-5.0f, 10.0f),
                BodyType = BodyType.Static,
                Shape = new PolygonShape(0),
                Size = new Microsoft.Xna.Framework.Vector2(10.0f, 30.0f)
            };
            new PhysicObject(new Texture(Application.Images["brick1"]), wall1desc);

            var wall2desc = new BodyDesc {
                StartPos = new Microsoft.Xna.Framework.Vector2(15.0f, 10.0f),
                BodyType = BodyType.Static,
                Shape = new PolygonShape(0),
                Size = new Microsoft.Xna.Framework.Vector2(10.0f, 30.0f)
            };
            new PhysicObject(new Texture(Application.Images["brick1"]), wall2desc);


            var sandacz = new BodyDesc {
                StartPos = new Microsoft.Xna.Framework.Vector2(5.0f, 7.45f),
                BodyType = BodyType.Static,
                Shape = new PolygonShape(0),
                Size = new Microsoft.Xna.Framework.Vector2(4.0f, 0.5f)
            };
            _ground3 = new PhysicObject(new Texture(Application.Images["sand"]), sandacz, new Vector2f(0f, 0f), new Vector2f(0f, -20.0f));
        }

        private Ball AddBall(Vector2f pos)
        {
            var ballDesc = new BodyDesc
            {
                StartPos = new Microsoft.Xna.Framework.Vector2(pos.X, pos.Y),
                BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic,
                Shape = new FarseerPhysics.Collision.Shapes.CircleShape(0.1f, 250),
                Size =
                    new Microsoft.Xna.Framework.Vector2(0.1f, 0.1f),
            };



            var physicBrick = new PhysicObject(new Texture(Application.Images["cannonball"]), ballDesc);


            var ball = new Ball() {
                PhysicObject = physicBrick
            };

            ball.PhysicObject.Body.OnCollision += Body_OnCollision;
            ball.PhysicObject.Body.UserData= ball ;

            return ball;
        }

        bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
            var ball = (fixtureA.Body.UserData is Ball ? fixtureA.Body.UserData : fixtureB.Body.UserData) as Ball;
            var brick = (fixtureA.Body.UserData is Ball ? fixtureB.Body.UserData : fixtureA.Body.UserData) as Brick;

            if(!fixtureA.Body.IsStatic)
            Application.World.RemoveBody(fixtureA.Body);
            
            if(!fixtureB.Body.IsStatic) Application.World.RemoveBody(fixtureB.Body);
            
            balls.Remove(ball);
            bricks.Remove(brick);
            
            explosion(ball.PhysicObject.Body.Position);
			//System.Console.WriteLine("explosion");

            return true;
        }

        private void explosion(Microsoft.Xna.Framework.Vector2 position) {

            foreach (var brick in bricks) {
                var impulse = (brick.PhysicObject.Body.Position - position);
                var d = impulse.Length();
                if(d <= 0.0f) continue;
                impulse.Normalize();
                impulse *= 25.0f / (d * d);
                brick.PhysicObject.Body.ApplyLinearImpulse(impulse);

				Emitter emitter = new Emitter(new Texture[]{
                    new Texture(Application.Images["explosion"]),
                    new Texture(Application.Images["explosion1"]),
                    new Texture(Application.Images["explosion2"]),
                    new Texture(Application.Images["explosion3"])
                }, new Vector2f(position.X * PhysicObject.PhysicScale, position.Y * PhysicObject.PhysicScale), new Vector2f(0, 0));
				emitters.Add(emitter);
				emitter.velScale = 1.0f;
				emitter.spawnRate = 0.001f;
				emitter.emitterLifeSpan = 0.01f;
				emitter.Speed = 6;

				/*Sound sound = Application.Sounds["explosion"];
				if (sound.Status != SoundStatus.Playing) {
					sound.PlayingOffset = Time.FromMilliseconds(0);
					sound.Play();
				}*/

			}


        }

        private void AddBrick(Vector2f pos, float rotation, Side side, BrickType type)
        {
			string sufix;
			if (side == Side.Left) {
				sufix = "";
			} else if (side == Side.Right) {
				sufix = "2";
			} else {
				sufix = "_uns";
			}

            var brickDesc = new BodyDesc
                {
                    StartPos = new Microsoft.Xna.Framework.Vector2(pos.X, pos.Y),
                    BodyType = BodyType.Dynamic,
                    Shape = new PolygonShape(100.0f),
                    Size =
                        new Microsoft.Xna.Framework.Vector2(0.5f, 0.5f),
                    Resitution = 0.3f
                };
			string[] imagePath;
			PhysicObject physicBrick;

			switch (type) {
				case BrickType.Wall1:
					physicBrick = new PhysicObject(new Texture(Application.Images["brick" + sufix + "_door_00000"]), brickDesc);
					imagePath = new string[50];
					for (int i = 0; i < 50; i++) {
						imagePath[i] = string.Format("brick"+sufix+"_door_{0:00000}", i);
					}
					physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Wall2:
					physicBrick = new PhysicObject(new Texture(Application.Images["window" + sufix + "_open_00000"]), brickDesc);
					imagePath = new string[50];
					for (int i = 0; i < 50; i++) {
						imagePath[i] = string.Format("window" + sufix + "_open_{0:00000}", i);
					}
						physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Wall3:
					imagePath = new string[] { "brick" + sufix + "_blanks" };
					physicBrick = new PhysicObject(imagePath, brickDesc);
					break;
				case BrickType.Wall4:
					physicBrick = new PhysicObject(new Texture(Application.Images["brick" + sufix + "_roof_00000"]), brickDesc);
					imagePath = new string[24];
					for (int i = 0; i < 24; i++) {
						imagePath[i] = string.Format("brick" + sufix + "_roof_{0:00000}", i);
					}
					physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Shooting:
					physicBrick = new PhysicObject(new Texture(Application.Images["brick" + sufix + "_cannon_00000"]), brickDesc);
					imagePath = new string[17];
					for (int i = 0; i < 17; i++) {
						imagePath[i] = string.Format("brick" + sufix + "_cannon_{0:00000}", i);
					}
					physicBrick.AddAnimation(imagePath);
                    physicBrick.Speed = 48;
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
			}


            Brick brick = new Brick()
                {
                    PhysicObject = physicBrick,
                    Side = side,
                    Type = type
                };

			physicBrick.Body.UserData = brick;
            physicBrick.Body.Rotation = rotation;
            bricks.Add(brick);
        }

		private void DropBlock()
		{

		    bool cannonFound = false;
		    foreach (var brick1 in bricks)
		    {
		        if (brick1.Type == BrickType.Shooting && brick1.Side != currentPlayer)
		        {
		            cannonFound = true;
		        }
		    }

            var type = BrickType.Shooting;
		    
            if (cannonFound)
            {
                var value = rand.Next(8);

                if (value > 4)
                {
                    type = BrickType.Shooting;
                }
                else
                {
                    type = (BrickType)value;
                }

                
            }
           
			var brickDesc = new BodyDesc {
                StartPos = new Microsoft.Xna.Framework.Vector2((float)(rand.NextDouble() * 10), -0.25f),
                BodyType = BodyType.Dynamic,
				Shape = new PolygonShape(100),
				Size =
					new Microsoft.Xna.Framework.Vector2(0.5f, 0.5f),
				Resitution = 0.3f
			};

			string[] imagePath;
			PhysicObject physicBrick;

			switch (type) {
				case BrickType.Wall1:
					physicBrick = new PhysicObject(new Texture(Application.Images["brick_uns_door_00000"]), brickDesc);
					imagePath = new string[50];
					for (int i = 0; i < 50; i++) {
						imagePath[i] = string.Format("brick_uns_door_{0:00000}", i);
					}
					physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Wall2:
					physicBrick = new PhysicObject(new Texture(Application.Images["window_uns_open_00000"]), brickDesc);
					imagePath = new string[50];
					for (int i = 0; i < 50; i++) {
						imagePath[i] = string.Format("window_uns_open_{0:00000}", i);
					}
					physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Wall3:
					imagePath = new string[] { "brick_uns_blanks" };
					physicBrick = new PhysicObject(imagePath, brickDesc);
					break;
				case BrickType.Wall4:
					physicBrick = new PhysicObject(new Texture(Application.Images["brick_uns_roof_00000"]), brickDesc);
					imagePath = new string[24];
					for (int i = 0; i < 24; i++) {
						imagePath[i] = string.Format("brick_uns_roof_{0:00000}", i);
					}
					physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Shooting:
					physicBrick = new PhysicObject(new Texture(Application.Images["brick_uns_cannon_00000"]), brickDesc);
					imagePath = new string[17];
					for (int i = 0; i < 17; i++) {
						imagePath[i] = string.Format("brick_uns_cannon_{0:00000}", i);
					}
					physicBrick.AddAnimation(imagePath);
                    physicBrick.Speed = 48;
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
			}

			Brick brick = new Brick() {
				PhysicObject = physicBrick,
				Side = Side.Unspecified,
				Type = type
			};

			brick.PhysicObject.Body.LinearDamping = 10;
			brick.PhysicObject.Body.UserData = brick;
			bricks.Add(brick);
		}
		
		private void OnBroadphaseCollision(ref FixtureProxy proxyA, ref FixtureProxy proxyB)
        {
            if (proxyA.Fixture.Body.UserData != null && proxyB.Fixture.Body.UserData != null && proxyA.Fixture.Body.UserData is Brick && proxyB.Fixture.Body.UserData is Brick) {
                Brick brick1 = (Brick)proxyA.Fixture.Body.UserData;
                Brick brick2 = (Brick)proxyB.Fixture.Body.UserData;
                if (brick1.Side == Side.Unspecified && brick2.Side != Side.Unspecified) {
                    proxyA.Fixture.Body.LinearDamping = 0.3f;
                    TakeBrick(brick1,brick2.Side);
                }
                if (brick2.Side == Side.Unspecified && brick1.Side != Side.Unspecified) {
                    proxyB.Fixture.Body.LinearDamping = 0.3f;
					TakeBrick(brick2, brick1.Side);

                }
            } else {
                
            }
		}

        public void OnStateExitted(IState nextState)
        {
			music.Stop();
        }

        public void Update(float dt)
        {
            blackoutAmount -= dt*100.0f;

            if (blackoutAmount < 0) blackoutAmount = 0.0f;

            if (blackoutAmount > 0)
            {
                _roundStopwatch.Restart();
                _dropPackagesStopwatch.Restart();
                _shootingStopwatch.Restart();

                return;
            }

			foreach (var brick in bricks) {
				if (rand.NextDouble() > 0.9999) {
					if (brick.Type!=BrickType.Shooting&&brick.PhysicObject.Images.Count > 1 && brick.PhysicObject.Animation != 1) {
						brick.PhysicObject.Animation = 1;
					}
				}
			}
            if (_shootingStopwatch.ElapsedMilliseconds > 30000)
            {
                // shoot
				foreach (var brick in bricks) {
					if (new Random().NextDouble() > 0.9) {
						if (brick.PhysicObject.Images.Count > 1 && brick.PhysicObject.Animation!=1) {
							brick.PhysicObject.Animation = 1;
						}
					}
					if (brick.Type == BrickType.Shooting) {

						var alpha = brick.PhysicObject.Body.Rotation;
						var dir = new Microsoft.Xna.Framework.Vector2((float)Math.Cos(alpha), (float)Math.Sin(alpha));

                

                        Microsoft.Xna.Framework.Vector2 position = brick.PhysicObject.Body.Position + 0.5f * dir;
						brick.PhysicObject.Animation = 1;
                        var ball = AddBall(new Vector2f(position.X, position.Y));

                        var impulse = 70.0f * dir;
                        ball.PhysicObject.Body.ApplyLinearImpulse(impulse);

						Sound sound = Application.Sounds["canon shot"];
						if(sound.Status!=SoundStatus.Playing)
							sound.Play();

                        balls.Add(ball);
                    }
                }

				

                _shootingStopwatch.Restart();                
            }

            if (_roundStopwatch.ElapsedMilliseconds > roundLength)
            {
                // round ended
                currentPlayer = currentPlayer == Side.Left ? Side.Right : Side.Left;
                
                Shoot();
                
                if (currentMouseJoint != null && _currentFixture != null) {
                    Application.World.RemoveJoint(currentMouseJoint);
                    _currentFixture = null;
                    currentMouseJoint = null;
                }

                _roundStopwatch.Restart();
            }

            if (_dropPackagesStopwatch.ElapsedMilliseconds > 6000)
            {
                DropBlock();
                _dropPackagesStopwatch.Restart();
            }

            var numberOfActiveBrickPlayerLeft = 0;
            var numberOfActiveBrickPlayerRight = 0;

            if (!Application.GameConfig.IsGameOver)
            {
                foreach (var brick in bricks)
                {
                    if (brick.PhysicObject.Body.Position.Y < gameOverLineOffset)
                    {
                        if (brick.Side == Side.Left)
                        {
                            numberOfActiveBrickPlayerLeft++;
                        }
                        else if(brick.Side == Side.Right)
                        {
                            numberOfActiveBrickPlayerRight++;
                        }
                    }
                }
            }


            if (!Application.GameConfig.IsGameOver && (numberOfActiveBrickPlayerLeft == 0 || numberOfActiveBrickPlayerRight == 0))
            {
                Application.GameConfig.IsGameOver = true;
                wygrywaSide = numberOfActiveBrickPlayerLeft > 0 ? Side.Left : Side.Right;
            }

            if (Application.GameConfig.IsGameOver) {
                gameOverSpriteScale = Math.Min(gameOverSpriteScale + dt * 3.0f, 1.0f);
                gameOverLeftSprite.Color = wygrywaSide == Side.Left ? new Color(255, 255, 255, (byte)(255.0f * (gameOverSpriteScale))) : new Color(0, 0, 0, 0);
                gameOverRightSprite.Color = wygrywaSide == Side.Right ? new Color(255, 255, 255, (byte)(255.0f * ( gameOverSpriteScale))) : new Color(0, 0, 0, 0);
            }
            
            leftPlayerSprite.Color = new Color(255,255,255  ,(byte) (currentPlayer == Side.Left ? 255 : 100));
			leftPlayerSprite.Scale = currentPlayer == Side.Left ? new Vector2f(1.3f,1.3f) : new Vector2f(0.5f,0.5f);
            rightPlayerSprite.Color = new Color(255,255,255  ,(byte) (currentPlayer == Side.Right ? 255 : 100));
			rightPlayerSprite.Scale = currentPlayer == Side.Right ? new Vector2f(1.3f, 1.3f) : new Vector2f(0.5f, 0.5f);
         
		}

        private void Shoot()
        {
            foreach (var brick in bricks)
            {
                if (brick.Type == BrickType.Shooting)
                {
                    var alpha = brick.PhysicObject.Body.Rotation;
                    var dir = new Microsoft.Xna.Framework.Vector2((float) Math.Cos(alpha), (float) Math.Sin(alpha));

                    Microsoft.Xna.Framework.Vector2 position = brick.PhysicObject.Body.Position + 0.5f*dir;
                    brick.PhysicObject.Animation = 1;
                    var ball = AddBall(new Vector2f(position.X, position.Y));

                    var impulse = 70.0f*dir;
                    ball.PhysicObject.Body.ApplyLinearImpulse(impulse);

					Emitter emitter = new Emitter(new Texture(Application.Images["trail"]), new Vector2f(position.X, position.Y), new Vector2f(0, 0));
					emitter.emitterLifeSpan = 0.3f;
					emitter.spawnRate = 0.01f;
					emitter.velScale = 0;
					emitter.block = ball.PhysicObject;
					emitters.Add(emitter);

                    balls.Add(ball);

					Sound sound = Application.Sounds["canon shot"];
					if (sound.Status != SoundStatus.Playing) {
						sound.PlayingOffset = Time.FromMilliseconds(0);
						sound.Play();
					}
                }
            }
        }

        public void Draw(float dt)
        {

            backgroundSprite.Position = new Vector2f(0.0f,0.0f);

            Application.Window.Draw(backgroundSprite);

            foreach (var brick in bricks) {
                brick.PhysicObject.Draw(dt);
            }

            foreach (var ball in balls) {
                ball.PhysicObject.Draw(dt);
            }

            _ground3.Draw(dt);
            _ground1.Draw(dt);
            _ground2.Draw(dt);
            
            
			foreach (var emitter in emitters) {
				emitter.Draw(dt);
			}

            //gwiazdkaEmitter.Draw();


            if (!Application.GameConfig.IsGameOver)
            {
                float seconds = (roundLength - _roundStopwatch.ElapsedMilliseconds) / 1000.0f;
                if (seconds < 0) seconds = 0;
                timerText.DisplayedString = String.Format("{0:0.0}s", seconds);
                var frame = timerText.GetGlobalBounds();
                var percent = ((_roundStopwatch.ElapsedMilliseconds) / roundLength);
                timerText.Scale = new Vector2f(1.0f, 1.0f) + 1.0f *  new Vector2f(percent, percent);
                timerText.FillColor = currentPlayer == Side.Left ? new Color(0, 0, 255) : new Color(255, 0, 0);
                timerText.Position = new Vector2f(currentPlayer == Side.Left ? (512.0f - 400.0f) : (512.0f - frame.Width + 400.0f), 200.0f);

                currentPlayerText.DisplayedString = currentPlayer == Side.Left ? "Teraz gra niebieski" : "Teraz gra czerwony";
                var currentPlayerFrame = currentPlayerText.GetGlobalBounds();
                currentPlayerText.Position = new Vector2f((1024.0f - currentPlayerFrame.Width) / 2.0f, 50);
                currentPlayerText.Scale = new Vector2f(0.7f, 0.7f);
                currentPlayerText.FillColor = currentPlayer == Side.Left ? new Color(0, 0, 255) : new Color(255, 0, 0);

                Application.Window.Draw(timerText);
                Application.Window.Draw(currentPlayerText);       
            }

            Application.Window.Draw(leftPlayerSprite);
            Application.Window.Draw(rightPlayerSprite);

			if (Application.GameConfig.IsGameOver) {
				if (wygrywaSide == Side.Left) {
					gameOverLeftSprite.Color = new Color(255, 255, 255, (byte)goalpha);
					Application.Window.Draw(gameOverLeftSprite);
				} else {
					gameOverRightSprite.Color = new Color(255, 255, 255,(byte) goalpha);
					Application.Window.Draw(gameOverRightSprite);
				}
				goalpha+=2;
				goalpha = Math.Min(255,Math.Max((byte)0, goalpha));
			}

            blackSprite.Color = new Color(255, 255, 255, (byte)blackoutAmount);
            blackSprite.Position = new Vector2f(0.0f, 0.0f);
            blackSprite.Scale = new Vector2f(1.0f, 1.0f);

            Application.Window.Draw(blackSprite);            
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (currentMouseJoint != null && _currentFixture != null)
            {
                var mousePos = new Microsoft.Xna.Framework.Vector2(e.X, e.Y);
                mousePos /= PhysicObject.PhysicScale;

                if (currentPlayer == Side.Left) {
                    mousePos.X = Math.Min(mousePos.X, 5.0f);
                } else if (currentPlayer == Side.Right) {
                    mousePos.X = Math.Max(mousePos.X, 5.0f);
                }
                currentMouseJoint.WorldAnchorB = mousePos;
            }
        }

        public void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (currentMouseJoint != null && _currentFixture != null)
            {
                Application.World.RemoveJoint(currentMouseJoint);

                _currentFixture = null;
                currentMouseJoint = null;
            }
        }

		private void TakeBrick(Brick brick,Side side) {
			brick.Side = side;
			string sufix="";
			if (side == Side.Left) {
				sufix = "";
			} else if (side == Side.Right) {
				sufix = "2";
			}

			string[] imagePath;
			PhysicObject physicBrick = brick.PhysicObject;

			switch (brick.Type) {
				case BrickType.Wall1:
					imagePath = new string[50];
					for (int i = 0; i < 50; i++) {
						imagePath[i] = string.Format("brick" + sufix + "_door_{0:00000}", i);
					}
					physicBrick.Images.Clear();
					physicBrick.AddAnimation(new string[]{"brick" + sufix + "_door_00000"});
					physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Wall2:
					imagePath = new string[50];
					for (int i = 0; i < 50; i++) {
						imagePath[i] = string.Format("window" + sufix + "_open_{0:00000}", i);
					}
					physicBrick.Images.Clear();
					physicBrick.AddAnimation(new string[]{"window" + sufix + "_open_00000"});
					physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Wall3:
					physicBrick.Images.Clear();
					physicBrick.AddAnimation(new string[] { "brick" + sufix + "_blanks" });
					break;
				case BrickType.Wall4:
					imagePath = new string[24];
					for (int i = 0; i < 24; i++) {
						imagePath[i] = string.Format("brick" + sufix + "_roof_{0:00000}", i);
					}
					physicBrick.Images.Clear();
					physicBrick.AddAnimation(new string[] { "brick" + sufix + "_roof_00000" });
					physicBrick.AddAnimation(imagePath);
					break;
				case BrickType.Shooting:
					imagePath = new string[17];
					for (int i = 0; i < 17; i++) {
						imagePath[i] = string.Format("brick" + sufix + "_cannon_{0:00000}", i);
					}
					physicBrick.Images.Clear();
					physicBrick.AddAnimation(new string[] { "brick" + sufix + "_cannon_00000" });
					physicBrick.AddAnimation(imagePath);
					physicBrick.Speed = 48;
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

        public void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (currentMouseJoint == null && _currentFixture == null)
            {
                var mousePos = new Microsoft.Xna.Framework.Vector2(e.X, e.Y);
                mousePos /= PhysicObject.PhysicScale;
              
                _currentFixture = Application.World.TestPoint(mousePos);

                if (_currentFixture != null)
                {
					if (_currentFixture.Body.UserData != null) {
						Brick brick = _currentFixture.Body.UserData as Brick;
						if (brick.Side == Side.Unspecified) {
							TakeBrick(brick, currentPlayer);//.Side = currentPlayer;
							_currentFixture.Body.LinearDamping = 0.3f;
						} else if (brick.Side != currentPlayer) {
							_currentFixture = null;
							return;
						}
					}
                    currentMouseJoint = new FixedMouseJoint(_currentFixture.Body, mousePos);
                    currentMouseJoint.MaxForce = 50.0f *_currentFixture.Body.Mass;

                    Application.World.AddJoint(currentMouseJoint);
                }
            }

            if (Application.GameConfig.IsGameOver) {
                Application.Fsm.ChangeToState(typeof(CreditsState));
            }
        }

        public void OnKeyReleased(object sender, KeyEventArgs e)
        {
        }

        public void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.O)
            {
                Application.GameConfig.IsGameOver = true;
            }
            else if(e.Code == Keyboard.Key.R)
            {
                Application.GameConfig.IsGameOver = false;
                _roundStopwatch.Restart();
                _shootingStopwatch.Restart();

                bricks.Clear();
                balls.Clear();
                Application.World.Clear();
                OnStateEntered(null);
            }


            if (gameOverSpriteScale >= 0.9999f) {
                
                
                
                Application.Fsm.ChangeToState(typeof(CreditsState));
            }
        }

        public Microsoft.Xna.Framework.Vector2 impulse { get; set; }
    }
}