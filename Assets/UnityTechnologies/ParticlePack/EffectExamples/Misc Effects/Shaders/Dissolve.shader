Shader"Custom/DissolveURP"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [HDR]_Emission ("Emission", Color) = (0,0,0,0)
        _MainTex ("Albedo", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _MetallicSmooth ("Metallic (RGB) Smooth (A)", 2D) = "white" {}
        _AO ("AO", 2D) = "white" {}
        [HDR]_EdgeColor1 ("Edge Color", Color) = (1,1,1,1)
        _Noise ("Noise", 2D) = "white" {}
        [Toggle] _Use_Gradient ("Use Gradient?", Float) = 1
        _Gradient ("Gradient", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [PerRendererData]_Cutoff ("Cutoff", Range(0,1)) = 0.0
        _EdgeSize ("EdgeSize", Range(0,1)) = 0.2
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.4
        _DisplaceAmount ("Displace Amount", Float) = 1.5
        _cutoff ("cutoff", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
Cull Off

LOD200

        Pass
        {
Name"ForwardLit"
            Tags
{"LightMode"="UniversalForward"
}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

struct Varyings
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 worldPos : TEXCOORD1;
    float3 normal : TEXCOORD2;
};

sampler2D _MainTex;
sampler2D _Normal;
sampler2D _MetallicSmooth;
sampler2D _AO;
sampler2D _Noise;
sampler2D _Gradient;
half _Glossiness, _Metallic, _Cutoff, _EdgeSize, _NoiseStrength, _DisplaceAmount;
half _cutoff;
half4 _Color, _EdgeColor1, _Emission;

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.position = UnityObjectToClipPos(IN.vertex);
    OUT.uv = IN.uv;
    OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
    OUT.normal = mul((float3x3) unity_ObjectToWorld, IN.normal);
    return OUT;
}

half4 frag(Varyings IN) : SV_Target
{
    half3 Noise = tex2D(_Noise, IN.uv).rgb;
    Noise.r = lerp(0, 1, Noise.r);
    half4 MetallicSmooth = tex2D(_MetallicSmooth, IN.uv);
    half3 Gradient = tex2D(_Gradient, IN.uv).rgb;
    half Edge = smoothstep(_Cutoff, _Cutoff - _EdgeSize, 1 - (Gradient + Noise.r * (1 - Gradient) * _NoiseStrength));

    fixed4 albedo = tex2D(_MainTex, IN.uv) * _Color;
    fixed3 emissiveCol = albedo.a * _Emission;
    half occlusion = tex2D(_AO, IN.uv).r;
    half3 normal = UnpackNormal(tex2D(_Normal, IN.uv));

    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(surfaceData);
    surfaceData.baseColor = albedo.rgb;
    surfaceData.metallic = MetallicSmooth.r * _Metallic;
    surfaceData.smoothness = MetallicSmooth.a * _Glossiness;
    surfaceData.normalWS = normal;
    surfaceData.occlusion = occlusion;
    surfaceData.emissive = emissiveCol + _EdgeColor1 * Edge;

    half4 color = UniversalFragmentPBR(surfaceData);
    clip(Noise.r - _cutoff);
    return color;
}
            ENDHLSL
        }
    }

FallBack"Diffuse"
}
