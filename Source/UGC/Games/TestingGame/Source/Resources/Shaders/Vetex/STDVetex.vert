#type vertex
#version 460 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
    vec4 position;
} camera;

layout(std140, binding = 2) uniform ModelBlock {
    mat4 model;
} modelData;

out vec3 vFragPos;
out vec3 vNormal;
out vec2 vTexCoord;

void main()
{
    vTexCoord = aTexCoord;
    vNormal = mat3(modelData.model) * aNormal;
    vFragPos = vec3(modelData.model * vec4(aPosition, 1.0));
    gl_Position = camera.projection * camera.view * vec4(vFragPos, 1.0);
}