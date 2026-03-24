using Silk.NET.OpenGL;
using VoxelEngine.Assets;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics.OpenGL;

internal unsafe class GL_GraphicsFactory : IGraphicsFactory
{
    private GL _GL;
    private GL_AssetsManager _assetsManager;

    public GL_GraphicsFactory(GL gL, GL_AssetsManager assetsManager)
    {
        _GL = gL;
        _assetsManager = assetsManager;
    }

    public IGraphicsCommandsList CreateCommandsList()
    {
        return new GL_GraphicsCommandsList(_GL, _assetsManager);
    }

    public BufferHandle CreateBuffer(BufferDescription description)
    {
        uint buffer = _GL.GenBuffer();
        BufferTargetARB target = description.Usage switch
        {
            BufferUsage.VertexBuffer => BufferTargetARB.ArrayBuffer,
            BufferUsage.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
            BufferUsage.UniformBuffer => BufferTargetARB.UniformBuffer,
            _ => BufferTargetARB.ArrayBuffer
        };

        _GL.BindBuffer(target, buffer);
        if (description.Data != 0)
        {
            _GL.BufferData(target, description.Size, (void*)description.Data, BufferUsageARB.StaticDraw);
        }
        else
        {
            _GL.BufferData(target, description.Size, null, BufferUsageARB.DynamicDraw);
        }
        _GL.BindBuffer(target, 0);

        return _assetsManager.Add(new GL_Buffer(buffer, description.Size));
    }

    public PipelineHandle CreatePipeline(PipelineDescription description)
    {
        uint vertexShader = _GL.CreateShader(ShaderType.VertexShader);
        _GL.ShaderSource(vertexShader, description.ShaderData.Vert);
        _GL.CompileShader(vertexShader);

        uint fragmentShader = _GL.CreateShader(ShaderType.FragmentShader);
        _GL.ShaderSource(fragmentShader, description.ShaderData.Frag);
        _GL.CompileShader(fragmentShader);

        uint pipeline = _GL.CreateProgram();
        _GL.AttachShader(pipeline, vertexShader);
        _GL.AttachShader(pipeline, fragmentShader);
        _GL.LinkProgram(pipeline);
        _GL.ValidateProgram(pipeline);

        _GL.DeleteShader(vertexShader);
        _GL.DeleteShader(fragmentShader);

        return _assetsManager.Add(new GL_Pipeline(pipeline));
    }

    public MeshHandle CreateMesh<T>(MeshData<T> meshData) where T : unmanaged, IVertexType
    {
        uint vao = _GL.GenVertexArray();
        _GL.BindVertexArray(vao);

        BufferHandle vboHandle;
        BufferHandle eboHandle;

        fixed (T* vData = meshData.Vertices)
        {
            vboHandle = CreateBuffer(new BufferDescription()
            {
                Size = (uint)(meshData.Vertices.Length * sizeof(T)),
                Usage = BufferUsage.VertexBuffer,
                Data = (nint)vData
            });
        }

        fixed (uint* iData = meshData.Indices)
        {
            eboHandle = CreateBuffer(new BufferDescription()
            {
                Size = (uint)(meshData.Indices.Length * sizeof(uint)),
                Usage = BufferUsage.IndexBuffer,
                Data = (nint)iData
            });
        }

        GL_Buffer vbo = _assetsManager.Get(vboHandle);
        _GL.BindBuffer(BufferTargetARB.ArrayBuffer, vbo.ID);

        GL_Buffer ebo = _assetsManager.Get(eboHandle);
        _GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo.ID);

        uint stride = T.GetStride();
        var attributes = T.GetAttributes();

        foreach (var attr in attributes)
        {
            int size = attr.Type switch
            {
                VertexAttribType.Float => 1,
                VertexAttribType.Float2 => 2,
                VertexAttribType.Float3 => 3,
                VertexAttribType.Float4 => 4,
                VertexAttribType.Int => 1,
                _ => 1
            };

            VertexAttribPointerType type = attr.Type == VertexAttribType.Int ? VertexAttribPointerType.Int : VertexAttribPointerType.Float;

            _GL.VertexAttribPointer(attr.Location, size, type, false, stride, (void*)attr.Offset);
            _GL.EnableVertexAttribArray(attr.Location);
        }

        _GL.BindVertexArray(0);

        return _assetsManager.Add(new GL_Mesh(vao, vbo.ID, ebo.ID));
    }


    public TextureHandle CreateTexture(TextureData textureData)
    {
        uint texture = _GL.GenTexture();
        _GL.BindTexture(TextureTarget.Texture2D, texture);

        TextureMinFilter minFilter = textureData.Options.FilterMode switch
        {
            TextureFilterMode.Nearest => textureData.Options.GenerateMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest,
            TextureFilterMode.Linear => textureData.Options.GenerateMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear,
            TextureFilterMode.LinearMipmap => TextureMinFilter.LinearMipmapLinear,
            _ => TextureMinFilter.Linear
        };

        TextureMagFilter magFilter = textureData.Options.FilterMode switch
        {
            TextureFilterMode.Nearest => TextureMagFilter.Nearest,
            TextureFilterMode.Linear => TextureMagFilter.Linear,
            TextureFilterMode.LinearMipmap => TextureMagFilter.Linear,
            _ => TextureMagFilter.Linear
        };

        Silk.NET.OpenGL.TextureWrapMode wrapMode = textureData.Options.WrapMode switch
        {
            TextureWrapMode.Repeat => Silk.NET.OpenGL.TextureWrapMode.Repeat,
            TextureWrapMode.ClampToEdge => Silk.NET.OpenGL.TextureWrapMode.ClampToEdge,
            TextureWrapMode.MirroredRepeat => Silk.NET.OpenGL.TextureWrapMode.MirroredRepeat,
            _ => Silk.NET.OpenGL.TextureWrapMode.Repeat
        };

        _GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
        _GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
        _GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
        _GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

        fixed (byte* ptr = textureData.pixelsData)
        {
            _GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba8, textureData.Width, textureData.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
        }

        if (textureData.Options.GenerateMipmaps)
        {
            _GL.GenerateMipmap(TextureTarget.Texture2D);
        }

        return _assetsManager.Add(new GL_TextureResource(texture, textureData.Width, textureData.Height, GL_TextureType.Texture2D));
    }

    public TextureHandle CreateTextureArray(Texture2DArrayData textureData)
    {
        uint texture = _GL.GenTexture();
        _GL.BindTexture(TextureTarget.Texture2DArray, texture);

        fixed (byte* ptr = textureData.pixelsData)
        {
            _GL.TexImage3D(TextureTarget.Texture2DArray, 0, InternalFormat.Rgba8, textureData.Width, textureData.Height, textureData.Layers, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
        }

        if (textureData.Options.GenerateMipmaps)
        {
            _GL.GenerateMipmap(TextureTarget.Texture2DArray);
        }

        TextureMinFilter minFilter = textureData.Options.FilterMode switch
        {
            TextureFilterMode.Nearest => textureData.Options.GenerateMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest,
            TextureFilterMode.Linear => textureData.Options.GenerateMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear,
            TextureFilterMode.LinearMipmap => TextureMinFilter.LinearMipmapLinear,
            _ => TextureMinFilter.Linear
        };

        TextureMagFilter magFilter = textureData.Options.FilterMode switch
        {
            TextureFilterMode.Nearest => TextureMagFilter.Nearest,
            TextureFilterMode.Linear => TextureMagFilter.Linear,
            TextureFilterMode.LinearMipmap => TextureMagFilter.Linear,
            _ => TextureMagFilter.Linear
        };

        Silk.NET.OpenGL.TextureWrapMode wrapMode = textureData.Options.WrapMode switch
        {
            TextureWrapMode.Repeat => Silk.NET.OpenGL.TextureWrapMode.Repeat,
            TextureWrapMode.ClampToEdge => Silk.NET.OpenGL.TextureWrapMode.ClampToEdge,
            TextureWrapMode.MirroredRepeat => Silk.NET.OpenGL.TextureWrapMode.MirroredRepeat,
            _ => Silk.NET.OpenGL.TextureWrapMode.Repeat
        };

        _GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)minFilter);
        _GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)magFilter);
        _GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)wrapMode);
        _GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)wrapMode);

        return _assetsManager.Add(new GL_TextureResource(texture, textureData.Width, textureData.Height, GL_TextureType.Texture2DArray, textureData.Layers));
    }


    public void DestroyMesh(MeshHandle handle)
    {
        GL_Mesh mesh = _assetsManager.Get(handle);
        _GL.DeleteVertexArray(mesh.VAO);
        _GL.DeleteBuffer(mesh.VBO);
        _GL.DeleteBuffer(mesh.EBO);
        _assetsManager.Remove(handle);  // invalidates the handle in the pool
    }

    public void DestroyTexture(TextureHandle handle)
    {
        GL_TextureResource texture = _assetsManager.Get(handle);
        _GL.DeleteTexture(texture.ID);
        _assetsManager.Remove(handle);  // invalidates the handle in the pool
    }

    public void DestroyBuffer(BufferHandle handle)
    {
        GL_Buffer buffer = _assetsManager.Get(handle);
        _GL.DeleteBuffer(buffer.ID);
        _assetsManager.Remove(handle);  // invalidates the handle in the pool
    }

    public void Dispose()
    {
    }

}
