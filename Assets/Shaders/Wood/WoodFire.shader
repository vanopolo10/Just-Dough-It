Shader "Custom/WoodFire_Fixed"
{
    Properties
    {
        _BaseMap ("Base", 2D) = "white" {}
        _CharredMap ("Charred", 2D) = "white" {}
        _EmissionMask ("Mask", 2D) = "white" {}

        _BaseColor ("Color", Color) = (1,1,1,1)
        _EmissiveColor ("Fire Color", Color) = (1,0.5,0.1,1)

        _BurnProgress ("Burn", Range(0,1)) = 0
        _EmissionProgress ("Emission", Range(0,1)) = 0
        _EmissionIntensity ("Intensity", Range(0,10)) = 0 // ? FIX

        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_CharredMap); SAMPLER(sampler_CharredMap);
            TEXTURE2D(_EmissionMask); SAMPLER(sampler_EmissionMask);

            float4 _BaseColor;
            float4 _EmissiveColor;

            float _BurnProgress;
            float _EmissionProgress;
            float _EmissionIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.pos.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 baseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).rgb;
                float3 charCol = SAMPLE_TEXTURE2D(_CharredMap, sampler_CharredMap, i.uv).rgb;
                float mask = SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, i.uv).r;

                float burn = smoothstep(0.3, 0.8, _BurnProgress);

                float3 albedo = lerp(baseCol, charCol, burn) * _BaseColor.rgb;

                float emission = _EmissionProgress * _EmissionIntensity * mask;

                float3 finalColor = albedo + _EmissiveColor.rgb * emission;

                return float4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}