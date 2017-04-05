using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBookman_Conversion.Engine
{
    class StateHandler
    {
        public enum ProcessState
        {   
            Menu,
            World,
            Battle
        };

        public enum ProcessAction
        {
            GoToMenu,
            ToWorld,
            BattleStart
        }

        public ProcessState CurrentState { get; private set; }

        private Dictionary<StateTransition, ProcessState> _transitionDictionary;

        public StateHandler()
        {
            CurrentState = ProcessState.Menu;

            _transitionDictionary = new Dictionary<StateTransition, ProcessState>
            {
                { new StateTransition(ProcessState.Menu, ProcessAction.ToWorld), ProcessState.World },
                { new StateTransition(ProcessState.World, ProcessAction.BattleStart), ProcessState.Battle },
                { new StateTransition(ProcessState.World, ProcessAction.GoToMenu), ProcessState.Menu },
                { new StateTransition(ProcessState.Battle, ProcessAction.ToWorld), ProcessState.World },
                { new StateTransition(ProcessState.Battle, ProcessAction.GoToMenu), ProcessState.Menu }
            };
        }

        public ProcessState GetNextState(ProcessAction actionToTake)
        {
            var newTrans = new StateTransition(CurrentState, actionToTake);

            ProcessState nextState;

            if (!_transitionDictionary.TryGetValue(newTrans, out nextState))
            {
                Debug.Write($"Invalid transition: {CurrentState} -> {actionToTake}");
            };
            
            return nextState;
        }

        public ProcessState MoveNext(ProcessAction command)
        {
            CurrentState = GetNextState(command);
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
