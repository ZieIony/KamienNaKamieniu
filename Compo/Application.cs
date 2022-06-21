using System;
using CompoEngine.States;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;
using SFML.Window;
using SFML.Graphics;
using FarseerPhysics.Dynamics;
using System.IO;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using SFML.Audio;
using SFML.System;

namespace CompoEngine
{
    internal static class Application
    {
        public static RenderWindow Window;
        public static World World;
        public static Fsm Fsm { get; set; }
        public static Dictionary<string, Image> Images { get; set; }
        public static GameConfig GameConfig { get; set; }

        private static void Main()
        {
            // Create the main window
            Window = new RenderWindow(new VideoMode(1024, 768), "Kamień na kamieniu");
            World = new World(new Vector2(0f, 9.81f), new AABB(new Vector2(0.0f), new Vector2(10.0f, 7.5f)));
            GameConfig = new GameConfig();
            Fsm = new Fsm();
            
            LoadMessages();
			LoadSounds();

            //LoadMessages();

            var introState = new IntroState();
            var introState2 = new IntroState2();
            var gameplayState = new GameplayState();
            var menuState = new MenuState();

            Fsm.States.Add(introState);
            Fsm.States.Add(introState2);
            Fsm.States.Add(gameplayState);
            Fsm.States.Add(menuState);

            Fsm.CurrentState = introState;
            Fsm.ChangeToState(typeof(IntroState));

            Window.Closed += OnClosed;
            Window.Resized += OnResized;
            Window.KeyPressed += OnKeyPressed;

            Window.KeyReleased += Fsm.OnKeyReleased;
            Window.KeyPressed += Fsm.OnKeyPressed;

            Window.MouseButtonPressed += Fsm.OnMouseButtonPressed;
            Window.MouseButtonReleased += Fsm.OnMouseButtonReleased;
            Window.MouseMoved += Fsm.OnMouseMoved;

            Clock clock = new Clock();
            clock.Restart();
            var prevTime = clock.ElapsedTime;

            // Start the game loop
            while (Window.IsOpen)
            {
                var time = clock.ElapsedTime;
                var frameTime = (time - prevTime).AsMilliseconds();
                prevTime = time;

                // Process events
                Window.DispatchEvents();

                Window.Clear();

                // Apply some transformations
                World.Step(Math.Max(0.001f, Math.Min(0.016f, frameTime)));

                Fsm.CurrentState.Update(frameTime);
                Fsm.CurrentState.Draw(frameTime);

                // Finally, display the rendered frame on screen
                Window.Display();
            }
        }

        private static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            var window = (Window) sender;
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
        }

		private static void LoadMessages() {
			Images = new Dictionary<string, Image>();
			string path = String.Format("{0}\\assets", Directory.GetCurrentDirectory());
			string[] files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
			foreach (string s in files) {
				Image image = new Image(s);
				int start = s.LastIndexOfAny(new char[] { '/', '\\' }) + 1;
				Images[s.Substring(start, s.LastIndexOf('.') - start)] = image;
			}
		}

		private static void LoadSounds()
		{
			Sounds = new Dictionary<string, Sound>();
            string path = String.Format("{0}\\assets", Directory.GetCurrentDirectory());
            string[] files = Directory.GetFiles(path, "*.wav", SearchOption.AllDirectories);
            foreach (string s in files)
            {
                try {
                    SFML.Audio.SoundBuffer buffer = new SFML.Audio.SoundBuffer(s);
                    int start = s.LastIndexOfAny(new char[] { '/', '\\' }) + 1;
                    Sounds[s.Substring(start, s.LastIndexOf('.') - start)] = new Sound(buffer);
                } catch (Exception e) {
                }
            }
        }


        private static void OnClosed(object sender, EventArgs e)
        {
            Window window = (Window) sender;
            window.Close();
        }

        private static void OnResized(object sender, SizeEventArgs e)
        {
        }

		public static Dictionary<string, Sound> Sounds { get; set; }
	}
}