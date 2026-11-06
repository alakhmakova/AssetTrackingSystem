using AssetTrackingSystem.Exceptions;
using System.Text.Json;
using System.Text.Json.Serialization;
using AssetTrackingSystem.Models;

namespace AssetTrackingSystem.Services;

public class AssetFileStorage
{
    private readonly string _filePath;
    // Readable JSON: line breaks and indentation
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };

    public AssetFileStorage(string filePath)
    {
        _filePath = filePath;
    }

    // Write assets and next id to the file (serialization)
    public void Save(AssetData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (IOException e) { throw new DataFileException("Could not save assets.", e); }
        catch (UnauthorizedAccessException e) { throw new DataFileException("Could not save assets. Access denied.", e); }
    }
    // Read products and next id from the file (deserialization)
    public AssetData Load()
    {
        try
        {
            if (!File.Exists(_filePath)) // first run: no file yet
            {
                return new();
            }

            string fromFile = File.ReadAllText(_filePath);
            // The Asset constructor runs here, so invalid data is rejected
            AssetData? loaded = JsonSerializer.Deserialize<AssetData>(fromFile, _jsonOptions);
            return loaded ?? new(); // "null" in the file -> empty data
        }
        catch (JsonException e) { throw new DataFileException("Could not load assets. The data file is corrupted.", e); }
        catch (IOException e) { throw new DataFileException("Could not load assets.", e); }
        catch (UnauthorizedAccessException e) { throw new DataFileException("Could not load assets. Access denied.", e); }
        catch (ValidationException e) { throw new DataFileException("Could not load assets. The file contains an invalid asset.", e); }

    }
}
