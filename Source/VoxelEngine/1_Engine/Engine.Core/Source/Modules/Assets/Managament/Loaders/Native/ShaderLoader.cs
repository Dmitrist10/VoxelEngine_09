using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Diagnostics;
using VoxelEngine.IO.FilesManagement;

namespace VoxelEngine.Assets;

public static class ShaderLoader
{

    public static ShaderData Load(string vPath, ShaderOptions options, IFileManager fileManager)
    {
        string source = fileManager.ReadAllText(vPath);

        if (vPath.EndsWith(".glsl", System.StringComparison.OrdinalIgnoreCase))
        {
            string[] splitSource = source.Split("#type ", System.StringSplitOptions.RemoveEmptyEntries);

            string vertexSource = "";
            string fragmentSource = "";

            foreach (var part in splitSource)
            {
                var eol = part.IndexOfAny(['\r', '\n']);
                if (eol == -1) continue;

                string type = part.Substring(0, eol).Trim();
                string content = part.Substring(eol).Trim();

                if (type == "vertex") vertexSource = content;
                else if (type == "fragment") fragmentSource = content;
            }

            return new ShaderData(vertexSource, fragmentSource);
        }
        else if (vPath.EndsWith(".vert", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShaderData(source, "");
        }
        else if (vPath.EndsWith(".frag", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShaderData("", source);
        }

        return new ShaderData(source, "");
    }



}
