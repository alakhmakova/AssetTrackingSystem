namespace AssetTrackingSystem.Exceptions;

// Thrown when no asset matches a search or an id
public class AssetNotFoundException : Exception
{
    public AssetNotFoundException()
        : base("Asset not found.") { }

    public AssetNotFoundException(string message)
        : base(message) { }

    public AssetNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
