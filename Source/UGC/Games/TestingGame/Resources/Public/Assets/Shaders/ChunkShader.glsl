#type vertex
#version 460 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in int aTextureLayer;
layout(location = 3) in int aFace;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
    vec4 position;
} camera;

layout(std140, binding = 2) uniform ModelBlock {
    mat4 model;
} modelData;

out vec2 vUV;
out vec3 vNormal;
out vec3 vFragPos;
flat out int vTextureLayer;

const vec3 FACE_NORMALS[6] = vec3[6](
    vec3(1, 0, 0),  // Face 0: +X
    vec3(-1, 0, 0), // Face 1: -X
    vec3(0, 1, 0),  // Face 2: +Y
    vec3(0, -1, 0), // Face 3: -Y
    vec3(0, 0, 1),  // Face 4: +Z
    vec3(0, 0, -1)  // Face 5: -Z
);

void main()
{
    vUV = aTexCoord;
    vTextureLayer = aTextureLayer;
    
    // Normal from face index
    vNormal = mat3(modelData.model) * FACE_NORMALS[clamp(aFace, 0, 5)];
    
    vFragPos = vec3(modelData.model * vec4(aPosition, 1.0));
    gl_Position = camera.projection * camera.view * vec4(vFragPos, 1.0);
}

#type fragment
#version 460 core

in vec2 vUV;
in vec3 vNormal;
in vec3 vFragPos;
flat in int vTextureLayer;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
    vec4 position;
} camera;

layout(std140, binding = 1) uniform ChunkMaterialProperties {
    vec4 Color;
} material;

layout(std140, binding = 3) uniform LightBlock {
    vec4 Direction; // xyz: light direction, w: padding
    vec4 Color;     // rgb: light color, w: ambient intensity
    vec4 Specular;  // x: intensity, y: shininess
} lightData;

uniform sampler2DArray uTextureArray;

out vec4 FragColor;

void main()
{
    vec4 texColor = texture(uTextureArray, vec3(vUV, float(vTextureLayer)));
    
    // Alpha testing for cutout transparency (e.g. tree leaves)
    if (texColor.a < 0.1) discard;

    vec3 albedo = pow(texColor.rgb, vec3(2.2)) * material.Color.rgb;
    
    // Basic diffuse lighting for chunks
    vec3 N = normalize(vNormal);
    vec3 L = normalize(-lightData.Direction.xyz);
    float diff = max(dot(N, L), 0.0);
    
    // Combine diffuse and ambient
    // Boosted for brightness as requested
    vec3 diffuse = diff * lightData.Color.rgb * (1.2 + lightData.Specular.x);
    vec3 ambient = vec3(0.18) * albedo * lightData.Color.w;
    
    vec3 color = ambient + diffuse * albedo;

    // Post-processing brightness boost
    float exposure = 1.25;
    color = color * exposure;
    
    // Simple Tonemapping & Gamma Correction
    color = color / (color + vec3(1.0)); 
    color = pow(color, vec3(1.0/2.2));

    FragColor = vec4(color, texColor.a * material.Color.a);
}
