using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using SFML.Window;

namespace CompoEngine.States
{
    public class Fsm
    {
        public static Collection<IState> States { get; set; }
        public static IState CurrentState { get; set; }

        public Fsm()
        {
            States = new Collection<IState>();
        }

        public void ChangeToState(Type stateType)
        {
            foreach (var state in States)
            {
                if (state.GetType() == stateType)
                {
                    CurrentState.OnStateExitted(state);
                    state.OnStateEntered(CurrentState);
                    CurrentState = state;
                }
            }
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            CurrentState.OnMouseMoved(sender, e);
        }

        public void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            CurrentState.OnMouseButtonReleased(sender, e);
        }

        public void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            CurrentState.OnMouseButtonPressed(sender, e);
        }

        public void OnKeyReleased(object sender, KeyEventArgs e)
        {
            CurrentState.OnKeyReleased(sender, e);
        }

        public void OnKeyPressed(object sender, KeyEventArgs e)
        {
            CurrentState.OnKeyPressed(sender, e);
        }
    }
}