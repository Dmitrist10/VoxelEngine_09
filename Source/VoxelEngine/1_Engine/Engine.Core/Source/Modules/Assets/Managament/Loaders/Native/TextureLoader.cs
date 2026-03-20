using StbImageSharp;

using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Diagnostics;
using VoxelEngine.IO.FilesManagement;

namespace VoxelEngine.Assets;

public static class TextureLoader
{

    public static TextureData Load(string path, TextureOptions options, IFileManager fileManager)
    {
        byte[] fileData = fileManager.ReadAllBytes(path);
        ImageResult image = ImageResult.FromMemory(fileData, ColorComponents.RedGreenBlueAlpha);

        if (image.Data == null || image.Data.Length == 0)
        {
            Logger.Error($"[TextureLoader] Failed to load {path} or empty.");
            return new TextureData([255, 0, 255, 255], 1, 1, options); // Magenta error texture
        }

        byte[] pixels = image.Data; // Copy pointer

        if (options.FlipVertically)
        {
            pixels = FlipVertically(image.Data, image.Width, image.Height);
        }

        var data = new TextureData(pixels, (uint)image.Width, (uint)image.Height, (uint)image.Comp, options);
        return data;
    }

    private static byte[] FlipVertically(byte[] data, int width, int height)
    {
        int bytesPerRow = width * 4;
        byte[] flipped = new byte[data.Length];

        for (int y = 0; y < height; y++)
        {
            int srcRow = y * bytesPerRow;
            int dstRow = (height - 1 - y) * bytesPerRow;
            Array.Copy(data, srcRow, flipped, dstRow, bytesPerRow);
        }

        return flipped;
    }


}
