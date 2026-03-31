Shader "Custom/WoodFire"
{
    Properties
    {
        [MainTexture] _BaseMap ("Base Map (Normal Wood)", 2D) = "white" {}
        [MainColor] _BaseColor ("Base Color", Color) = (1,1,1,1)

        _CharredMap ("Charred Map (Burnt Wood)", 2D) = "white" {}
        _EmissionMask ("Emission Mask", 2D) = "white" {}

        _EmissiveColor ("Emissive Color", Color) = (1, 0.5, 0.1, 1)

        _BurnProgress ("Burn Progress", Range(0,1)) = 0
        _EmissionProgress ("Emission Progress", Range(0,1)) = 0
        _EmissionIntensity ("Emission Intensity", Range(0, 20)) = 5

        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_CharredMap); SAMPLER(sampler_CharredMap);
            TEXTURE2D(_EmissionMask); SAMPLER(sampler_EmissionMask);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _EmissiveColor;

                float _BurnProgress;
                float _EmissionProgress;
                float _EmissionIntensity;
                float _Metallic;
                float _Smoothness;
            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            float GetEmissionCurve(float t)
            {
                return saturate((1.0 - t) * t * 4.0);
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                half4 charTex = SAMPLE_TEXTURE2D(_CharredMap, sampler_CharredMap, i.uv);
                half mask = SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, i.uv).r;

                float burn = smoothstep(0.3, 0.8, _BurnProgress);
                half3 albedo = lerp(baseTex.rgb, charTex.rgb, burn) * _BaseColor.rgb;

                float glow = GetEmissionCurve(_EmissionProgress);
                float emissionStrength = glow * _EmissionIntensity * mask;
                half3 emission = _EmissiveColor.rgb * emissionStrength;

                InputData inputData = (InputData)0;
                inputData.positionWS = i.positionWS;
                inputData.normalWS = normalize(i.normalWS);
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(i.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(i.positionWS);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = albedo;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.emission = emission;
                surfaceData.occlusion = 1;
                surfaceData.alpha = 1;

                return UniversalFragmentPBR(inputData, surfaceData);
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}