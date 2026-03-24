namespace VoxelEngine.IO;

public interface ISerializer
{
    // void Write<T>(T data) where T : unmanaged;
    // void Write(string data);
    // // void Read();

    // Save/Load to streams
    void Serialize<T>(Stream stream, T obj);
    T Deserialize<T>(Stream stream);

    // Core conversion: Takes a generic parsed object (like a JsonElement or 
    // MessagePack dynamic object) and turns it into a concrete C# Component type.
    object ConvertToConcrete(object rawData, Type targetType);
}
