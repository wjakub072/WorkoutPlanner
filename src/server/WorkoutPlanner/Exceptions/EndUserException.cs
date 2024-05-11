namespace WorkoutPlanner.Exceptions;

public class EndUserException : Exception
{
    public EndUserException()
    {
    }

    public EndUserException(string message)
        : base(message)
    {
    }

    public EndUserException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
