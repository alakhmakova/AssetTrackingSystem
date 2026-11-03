namespace AssetTrackingSystem.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("Invalid asset.") { }

    public ValidationException(string message)
        : base(message) { }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException) { }
}
