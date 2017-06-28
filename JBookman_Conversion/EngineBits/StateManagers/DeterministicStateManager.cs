using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.EngineBits.Consts;
using JBookman_Conversion.GameStates;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MessageBox = System.Windows.Forms.MessageBox;

namespace JBookman_Conversion.EngineBits.StateManagers
{
    public class DeterministicStateManager
    {
        public ProcessState CurrentState { get; private set; }

        public IGameState CurrentGameState { get; private set; }

        private Dictionary<StateTransition, ProcessState> _transitionDictionary;

        private IEnumerable<IGameState> _availableStates = new List<IGameState>
        {
            new MenuState(), new WorldState()
        };

        public DeterministicStateManager()
        {
            CurrentState = ProcessState.Menu;

            _transitionDictionary = new Dictionary<StateTransition, ProcessState>
            {
                { new StateTransition(ProcessState.Menu, ProcessAction.ToWorld), ProcessState.World },
                { new StateTransition(ProcessState.Menu, ProcessAction.BattleStart), ProcessState.Battle },
                { new StateTransition(ProcessState.World, ProcessAction.BattleStart), ProcessState.Battle },
                { new StateTransition(ProcessState.World, ProcessAction.GoToMenu), ProcessState.Menu },
                { new StateTransition(ProcessState.Battle, ProcessAction.ToWorld), ProcessState.World },
                { new StateTransition(ProcessState.Battle, ProcessAction.GoToMenu), ProcessState.Menu }
            };
        }

        public IGameState GetNextState(ProcessAction actionToTake)
        {
            var newTrans = new StateTransition(CurrentState, actionToTake);

            ProcessState nextProcessState;

            if (!_transitionDictionary.TryGetValue(newTrans, out nextProcessState))
            {
                Debug.Write($"Invalid transition: {CurrentState} -> {actionToTake}");
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