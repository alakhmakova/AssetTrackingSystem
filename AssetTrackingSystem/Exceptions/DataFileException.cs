using System;

namespace AssetTrackingSystem.Exceptions;

public class DataFileException : Exception
{
    public DataFileException()
        : base("Data file error.") { }

    public DataFileException(string message)
        : base(message) { }

    public DataFileException(string message, Exception innerException)
        : base(message, innerException) { }
}
