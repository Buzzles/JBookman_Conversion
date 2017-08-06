using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.EngineBits.Consts;
using JBookman_Conversion.GameStates;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MessageBox = System.Windows.Forms.MessageBox;
using System;
using OpenTK.Input;

namespace JBookman_Conversion.EngineBits.StateManagers
{
    public class DeterministicStateManager
    {
        public ProcessState CurrentState { get; private set; }

        public IGameState CurrentGameState { get; private set; }

        private Dictionary<StateTransition, ProcessState> _transitionDictionary;

        private ICollection<IGameState> _availableStates = new List<IGameState>();

        public DeterministicStateManager()
        {
            _transitionDictionary = new Dictionary<StateTransition, ProcessState>
            {
                { new StateTransition(ProcessState.Initial, ProcessAction.GoToMenu), ProcessState.Menu },
                { new StateTransition(ProcessState.Menu, ProcessAction.ToWorld), ProcessState.World },
                { new StateTransition(ProcessState.Menu, ProcessAction.BattleStart), ProcessState.Battle },
                { new StateTransition(ProcessState.World, ProcessAction.BattleStart), ProcessState.Battle },
                { new StateTransition(ProcessState.World, ProcessAction.GoToMenu), ProcessState.Menu },
                { new StateTransition(ProcessState.Battle, ProcessAction.ToWorld), ProcessState.World },
                { new StateTransition(ProcessState.Battle, ProcessAction.GoToMenu), ProcessState.Menu }
            };
        }

        public void AddNewState(IGameState newState)
        {
            _availableStates.Add(newState);
        }

        public IGameState GetNextState(ProcessAction actionToTake)
        {
            var newTrans = new StateTransition(CurrentState, actionToTake);

            ProcessState nextProcessState;

            if (!_transitionDictionary.TryGetValue(newTrans, out nextProcessState))
            {
                Debug.Write($"Invalid transition: {CurrentState} -> {actionToTake}");
                return CurrentGameState;
            };

            var nextState = _availableStates.FirstOrDefault(s => s.ProcessState == nextProcessState);

            return nextState;
        }

        public ProcessState MoveNext(ProcessAction command)
        {
            var existingState = CurrentState;

            CurrentGameState = GetNextState(command);
            CurrentState = CurrentGameState.ProcessState;

            // TODO: Debug remove
            MessageBox.Show($"Changing existing state {existingState} with Command:{command} to: {CurrentState} ");

            return CurrentState;
        }

        internal void DrawCurrentState(Renderer renderer)
        {
            var drawableState = CurrentGameState as IDrawable;

            if (drawableState != null) // 2nd half is temp hack
            {
                drawableState.Draw(renderer);

                //var getDrawables = drawableState.Draw(renderer);

                //if (CurrentState == ProcessState.World)
                //{ }
                //else
                //{
                //    renderer.RenderPrimitives();
                //}

            }
        }

        public void UpdateCurrentState(KeyboardState keyboardState)
        {
            var updatableState = CurrentGameState as IUpdatable;

            if (updatableState != null)
            {
                var result = updatableState.Update(keyboardState);
                ////var stateChange = updatableState.Update(keyboardState); // ? bad?
                // tie an event/delegate into this?

                if (result.ChangeState && result.ActionToDo.HasValue)
                {
                    MoveNext(result.ActionToDo.Value);
                }
            }
        }

        class StateTransition
        {
            public ProcessState State { get; private set; }
            public ProcessAction Action { get; private set; }

            public StateTransition(ProcessState startState, ProcessAction action)
            {
                State = startState;
                Action = action;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * State.GetHashCode() + 31 * Action.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                return other != null && this.State == other.State && this.Action == other.Action;
            }
        }
    }
}