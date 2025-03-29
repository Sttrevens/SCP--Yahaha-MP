Shader "Custom/FatigueVerigo"
{
    Properties
    {
        _MainTex ("Base Map" , 2D) = "white" {}
    }

    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            half4 ApplyGaussianBlur(float2 uv)
            {
                float2 offsets[5];
                offsets[0] = float2(0.0 , 0.0);
                offsets[1] = float2(1.0 / _ScreenParams.x , 0.0);
                offsets[2] = float2(-1.0 / _ScreenParams.x , 0.0);
                offsets[3] = float2(0.0 , 1.0 / _ScreenParams.y);
                offsets[4] = float2(0.0 , -1.0 / _ScreenParams.y);

                half4 color = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,uv);
                for (int i = 0 ; i < 5 ; i++)
                {
                    color += SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,uv+offsets[i]);
                }
                return color / 6.0;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv,_MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Apply Gaussian Blur
                return ApplyGaussianBlur(IN.uv);
            }
            ENDHLSL
        }
    }
    Fallback "Hidden/InternalErrorShader"
}
