using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace CompoEngine.States {
    class IntroState2: IState {
        private Sprite _background;
        private Sprite _white;
        private Sprite _background_polidea;
        private float alpha;

        public void OnStateEntered(IState previousState) {
            _background = new Sprite(new Texture(Application.Images["splash"]));
            _background_polidea = new Sprite(new Texture(Application.Images["polidea_splash"]));
            _white = new Sprite(new Texture(Application.Images["white"]));

            alpha = 255;
        }

        public void OnStateExitted(IState nextState) {
            Application.World.Clear();
        }

        public void Update(float dt) {
            alpha = Math.Max(0.0f, alpha - (dt * 100.0f));
        }

        public void Draw(float dt) {
            _background.Position = new Vector2f(0.0f, 0.0f);
            Application.Window.Draw(_background);

            _white.Color = new Color(255, 255, 255, (byte)alpha);
            _white.Position = new Vector2f(0.0f, 0.0f);
            _white.Scale = new Vector2f(3.0f, 3.0f);
            Application.Window.Draw(_white);

            _background_polidea.Color = new Color(255, 255, 255, (byte)alpha);
            _background_polidea.Position = new Vector2f(0.0f, 0.0f);
            Application.Window.Draw(_background_polidea);
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs e) {
        }

        public void OnMouseButtonReleased(object sender, MouseButtonEventArgs e) {
        }

        public void OnMouseButtonPressed(object sender, MouseButtonEventArgs e) {
            Application.Fsm.ChangeToState(typeof(GameplayState));
        }

        public void OnKeyReleased(object sender, KeyEventArgs e) {
            Application.Fsm.ChangeToState(typeof(GameplayState));
        }

        public void OnKeyPressed(object sender, KeyEventArgs e) {

        }
    }
}
