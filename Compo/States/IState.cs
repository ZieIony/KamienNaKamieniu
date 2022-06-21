using SFML.Window;

namespace CompoEngine.States
{
    public interface IState
    {
        void Update(float dt);
        void Draw(float dt);

        void OnMouseMoved(object sender, MouseMoveEventArgs e);
        void OnMouseButtonReleased(object sender, MouseButtonEventArgs e);
        void OnMouseButtonPressed(object sender, MouseButtonEventArgs e);
        void OnKeyReleased(object sender, KeyEventArgs e);
        void OnKeyPressed(object sender, KeyEventArgs e);

        void OnStateEntered(IState previousState);
        void OnStateExitted(IState nextState);
    }
}
