Shader "Custom/FatigueVerigo"
{
    Properties
    {
        _BlueNoiseTex("Blue Noise Texture" , 2D) = "white"{}
        _Intensity("Blur Intensity" , Range(0,1)) = 0.5
        _QuantizationNum("The Number Of Quantization",Int) = 6
        _NoiseIntensity("Blue Noise Intensity" , Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 200
        ZWrite Off
        Cull Off
        
        HLSLINCLUDE
        #include "Assets/D_PostProcessing/PostProcessing.hlsl"
        ENDHLSL
        Pass
        {
            Name "Quantization Pass"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            sampler2D _BlueNoiseTex;
            CBUFFER_START(UnityPerMaterial)
                float _Intensity;
                int _QuantizationNum;
                float _NoiseIntensity;
            CBUFFER_END

            half4 frag(Varyings IN) : SV_Target
            {
                half3 originalColor = GetSource(IN).rgb;
                half3 gammaCorrected = pow(abs(originalColor), 2.2);

                half luminance = dot(gammaCorrected, half3(0.299,0.587,0.114));

                // —— 更细的蓝噪声抖动 —— 
                // 假设噪声贴图 256×256
                float2 noiseScale = float2(_ScreenParams.x, _ScreenParams.y) / 256.0;
                float2 noiseOffset = sin(_Time.y * 0.01 + IN.uv.xy * 0.5 * 0.01) * 0.1;
                half blueNoise = tex2D(_BlueNoiseTex, frac(IN.uv.xy * 0.5 * noiseScale + noiseOffset)).r;

                // 量化 + 抖动
                half dither = (blueNoise - 0.5) * (1.0/_QuantizationNum) * _NoiseIntensity;
                half steps = _QuantizationNum;
                half lf = (luminance + dither) * steps;
                half baseLevel = floor(lf);
                half fracPart = lf - baseLevel;

                // 平滑过渡
                float edge = 0.5 / steps;
                half smooth = smoothstep(0.0, edge, fracPart) * (1.0 - smoothstep(1.0 - edge, 1.0, fracPart));
                half quantizedLuminance = (baseLevel + smooth) / steps;

                half3 ratio = gammaCorrected / max(luminance, 1e-5);
                half3 quantColor = pow(abs(saturate(ratio * quantizedLuminance)), 0.4545);

                return half4(quantColor, 1.0);
            }
            ENDHLSL
        }
    }
}
