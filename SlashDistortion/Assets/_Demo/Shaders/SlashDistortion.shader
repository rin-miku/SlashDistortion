Shader "Custom/SlashDistortion"
{
    Properties
    {
        _DistortionTex ("Distortion Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Float) = 0.05
        _AberrationStrength ("Aberration Strength", Float) = 0.1
        _AberrationThreshold ("Aberration Threshold", Float) = 0.1
        _ScrollSpeed ("Scroll Speed", Vector) = (0.1, 0, 0, 0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "SlashDistortion"
            Tags { "LightMode" = "SlashDistortion" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_DistortionTex);
            SAMPLER(sampler_DistortionTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);

            float4 _DistortionTex_ST;
            float4 _MaskTex_ST;
            float  _DistortionStrength;
            float _AberrationStrength;
            float _AberrationThreshold;
            float4 _ScrollSpeed;

            Varyings Vert (Attributes input)
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _DistortionTex);

                return output;
            }

            half4 Frag (Varyings input) : SV_Target
            {
                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv).a;
                mask = pow(mask, 2.0);

                float2 scrollUV = input.uv + _ScrollSpeed.xy * _Time.y;
                float2 distortion = SAMPLE_TEXTURE2D(_DistortionTex, sampler_DistortionTex, scrollUV);
                distortion = (distortion * 2 - 1) * _DistortionStrength * mask;

                float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);
                float2 distortedUV = screenUV + distortion;

                float aberration = smoothstep(_AberrationThreshold, 1.0, length(distortion)) * _AberrationStrength * mask;
                float2 aberrationOffset = distortion * aberration;

                float r = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, distortedUV + aberrationOffset).r;
                float g = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, distortedUV).g;
                float b = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, distortedUV - aberrationOffset).b;

                return half4(r, g, b, mask);
            }
            ENDHLSL
        }
    }
}
