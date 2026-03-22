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
    
    // Normal matrix for correct transformation with scaling
    mat3 normalMatrix = mat3(transpose(inverse(modelData.model)));
    vNormal = normalize(normalMatrix * aNormal);
    
    vFragPos = vec3(modelData.model * vec4(aPosition, 1.0));
    gl_Position = camera.projection * camera.view * vec4(vFragPos, 1.0);
}

#type fragment
#version 460 core

in vec3 vFragPos;
in vec3 vNormal;
in vec2 vTexCoord;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
    vec4 position;
} camera;

layout(std140, binding = 1) uniform PBRMaterialProperties {
    vec4 Color;
    float Metallic;
    float Roughness;
    float AO;
} material;

layout(std140, binding = 3) uniform LightBlock {
    vec4 Direction; // xyz: light direction, w: padding
    vec4 Color;     // rgb: light color, w: ambient intensity
    vec4 Specular;  // x: intensity, y: shininess
} lightData;

layout(binding = 0) uniform sampler2D uAlbedoTexture;

out vec4 FragColor;

const float PI = 3.14159265359;

// ----------------------------------------------------------------------------
// PBR Math Functions
// ----------------------------------------------------------------------------

float DistributionGGX(vec3 N, vec3 H, float roughness) {
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float nom   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / max(denom, 0.0000001); 
}

float GeometrySchlickGGX(float NdotV, float roughness) {
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness) {
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

vec3 fresnelSchlick(float cosTheta, vec3 F0) {
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

// ----------------------------------------------------------------------------
// Post-Processing Functions
// ----------------------------------------------------------------------------

vec3 ACESFilm(vec3 x) {
    float a = 2.51;
    float b = 0.03;
    float c = 2.43;
    float d = 0.59;
    float e = 0.14;
    return clamp((x*(a*x+b))/(x*(c*x+d)+e), 0.0, 1.0);
}

void main()
{
    // Albedo setup & Alpha Testing
    vec4 texColor = texture(uAlbedoTexture, vTexCoord);
    
    // Alpha testing for cutout transparency (e.g. tree leaves)
    if (texColor.a < 0.1) discard;
    
    // Basic vectors
    vec3 N = normalize(vNormal);
    vec3 V = normalize(camera.position.xyz - vFragPos);

    // Linear albedo
    vec3 albedo = pow(texColor.rgb, vec3(2.2)) * material.Color.rgb;
    
    float metallic = material.Metallic;
    float roughness = material.Roughness;
    float ao = material.AO;

    // F0 is the base reflectivity (0.04 for most dielectrics)
    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, albedo, metallic);

    // Reflectance equation
    vec3 Lo = vec3(0.0);

    // Direct lighting (One Directional Light)
    vec3 L = normalize(-lightData.Direction.xyz);
    vec3 H = normalize(V + L);
    
    // Radiance calculation - using light color directly (more brightness)
    // We combine light color withSpecular intensity for flexibility
    vec3 radiance = lightData.Color.rgb * (1.0 + lightData.Specular.x);

    // Cook-Torrance BRDF
    float NDF = DistributionGGX(N, H, roughness);   
    float G   = GeometrySmith(N, V, L, roughness);    
    vec3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);        
    
    vec3 numerator    = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.0001; 
    vec3 specular = numerator / denominator;
    
    // Diffuse vs Specular ratio
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;	  

    float NdotL = max(dot(N, L), 0.0);        
    Lo += (kD * albedo / PI + specular) * radiance * NdotL;

    // Ambient Term
    // Boosted ambient to help with the "not bright" issue
    vec3 ambient = vec3(0.12) * albedo * ao * lightData.Color.w;
    
    vec3 color = ambient + Lo;

    // Exposure and Tone Mapping (ACES)
    float exposure = 1.3;
    color = ACESFilm(color * exposure);
    
    // Gamma Correction
    color = pow(color, vec3(1.0/2.2)); 

    FragColor = vec4(color, texColor.a * material.Color.a);
}
