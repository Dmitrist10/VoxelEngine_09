using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoxelEngine.IO.Serialization;

public sealed class JsonSerializer : ISerializer
{
    private JsonSerializerOptions _options;

    public JsonSerializer()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve,
            Converters =
            {
                // new GuidConverter(),
            }
        };
    }

    public T Deserialize<T>(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Serialize<T>(Stream stream, T obj)
    {
        throw new NotImplementedException();
    }

    public object ConvertToConcrete(object rawData, Type targetType)
    {
        throw new NotImplementedException();
    }

}