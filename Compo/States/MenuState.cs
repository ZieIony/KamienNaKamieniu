using CompoEngine.Physics;
using SFML.Window;

namespace CompoEngine.States {
    class MenuState: IState {
        private PhysicsObject _background;

        private PhysicsObject _button1;
        private PhysicsObject _button2;

        public void OnStateEntered(IState previousState) {

        }

        public void OnStateExitted(IState nextState) {
        }

        public void Update(float dt) {
        }

        public void Draw(float dt) {
            _background.Draw(dt);
            _button1.Draw(dt);
            _button2.Draw(dt);
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs e) {
        }

        public void OnMouseButtonReleased(object sender, MouseButtonEventArgs e) {
        }

        public void OnMouseButtonPressed(object sender, MouseButtonEventArgs e) {
        }

        public void OnKeyReleased(object sender, KeyEventArgs e) {
            Application.Fsm.ChangeToState(typeof(IntroState2));
        }

        public void OnKeyPressed(object sender, KeyEventArgs e) {
        }
    }
}
