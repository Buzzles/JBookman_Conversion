using JBookman_Conversion.EngineBits.Consts;

namespace JBookman_Conversion.EngineBits.StateManagers
{
    public class StateQueueItem
    {
        public bool ChangeState { get; set; }

        public ProcessAction? ActionToDo { get; set; }
    }
}