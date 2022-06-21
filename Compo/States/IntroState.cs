using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace CompoEngine.States {
    internal class IntroState: IState {
        private Sprite _background;
        private Sprite _white;

        public void OnStateEntered(IState previousState) {
            _background = new Sprite(new Texture(Application.Images["polidea_splash"]));
            _white = new Sprite(new Texture(Application.Images["white"]));
        }

        public void OnStateExitted(IState nextState) {
            Application.World.Clear();
        }

        public void Update(float dt) {
        }

        public void Draw(float dt) {

            _white.Position = new Vector2f(0.0f, 0.0f);
            _white.Scale = new Vector2f(3.0f, 3.0f);
            Application.Window.Draw(_white);

            _background.Position = new Vector2f(0.0f, 0.0f);
            Application.Window.Draw(_background);
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs e) {
        }

        public void OnMouseButtonReleased(object sender, MouseButtonEventArgs e) {
        }

        public void OnMouseButtonPressed(object sender, MouseButtonEventArgs e) {
            Application.Fsm.ChangeToState(typeof(IntroState2));
        }

        public void OnKeyReleased(object sender, KeyEventArgs e) {
            Application.Fsm.ChangeToState(typeof(IntroState2));
        }

        public void OnKeyPressed(object sender, KeyEventArgs e) {
        }
    }
}