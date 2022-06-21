using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace CompoEngine.States {
    class CreditsState : IState {
        private Sprite _background;
		private Text currentPlayerText;
		private Font font;
		private float pos=800;
        public void OnStateEntered(IState previousState)
        {
			font = new Font("assets/BRLNSDB.TTF");
			currentPlayerText = new Text("Kamien na Kamieniu\nCompo IGK 2013\n\nGrafika:\nMalgorzata Jablonska\n\nProgramowanie:\nMarcin Korniluk\nMichal Mizera\nKarol Swiniarski", font, 80);

            _background = new Sprite(new Texture(Application.Images["credits"]));
            _background.Position = new Vector2f(0.0f, 0.0f);
            _background.Scale = new Vector2f(1024.0f/_background.Texture.Size.X, 768.0f/_background.Texture.Size.Y);
        }

        public void OnStateExitted(IState nextState)
        {      
        }

        public void Update(float dt)
        {
        }

        public void Draw(float dt)
        {
			currentPlayerText.Position = new Vector2f(100, pos-=0.1f);
			pos = Math.Max(100, pos);
			currentPlayerText.Scale = new Vector2f(1, 1);
			currentPlayerText.FillColor = new Color(0,91,149);

            Application.Window.Draw(_background);
			Application.Window.Draw(currentPlayerText);
		}

        public void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
        }

        public void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
        }

        public void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            Window window = (Window) sender;
            window.Close();
        }

        public void OnKeyReleased(object sender, KeyEventArgs e)
        {
            Window window = (Window) sender;
            window.Close();
        }

        public void OnKeyPressed(object sender, KeyEventArgs e)
        {
        }
    }
}


