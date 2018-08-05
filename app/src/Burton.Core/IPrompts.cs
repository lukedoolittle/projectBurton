namespace Burton.Core
{
    public interface IPrompts
    {
        string AskForWord { get; }
        string Continuation { get; }
        string Correct { get; }
        string Correction { get; }
        string Fail { get; }
        string PhonemicCheck { get; }
        string PhonicCheck { get; }
        string Start { get; }
        string Success { get; }
        string Try { get; }
        string Wait { get; }
        string QuestionCorrection { get; }
    }
}