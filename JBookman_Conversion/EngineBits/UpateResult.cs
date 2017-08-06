using JBookman_Conversion.EngineBits.Consts;

namespace JBookman_Conversion.EngineBits
{
    public class UpdateResult
    {
        public bool ChangeState { get; set; }

        public ProcessAction? ActionToDo { get; set; }
    }
}