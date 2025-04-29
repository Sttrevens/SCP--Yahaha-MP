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
                half3 gammaCorrected = pow(abs(originalColor),2.2);

                // 1. 计算亮度
                half luminance = dot(gammaCorrected, half3(0.299, 0.587, 0.114));

                // 2. 动态偏移采样蓝噪声
                half2 blueNoiseUV = IN.uv * 0.5;

                // 动态偏移，可以用时间作简单平移
                float2 noiseOffset = float2(0.05, 0.05) * frac(_Time.y * 0.1); 
                // 小幅度平移，0.1是位移尺度，0.5是速度
                blueNoiseUV = frac(blueNoiseUV + noiseOffset); // 保持UV在[0,1]

                half blueNoiseValue = tex2D(_BlueNoiseTex, blueNoiseUV).r;
                blueNoiseValue *= _NoiseIntensity;
                // 3. 加入抖动
                half ditherStrength = 1.0 / _QuantizationNum;
                half dither = (blueNoiseValue - 0.5) * ditherStrength;

                // 4. 量化亮度
                half quantizedLuminance = floor((luminance + dither) * _QuantizationNum) / _QuantizationNum;

                // 5. 保持色相饱和度
                half3 colorRatio = gammaCorrected / max(luminance, 1e-5);
                half3 quantizedColor = saturate(colorRatio * quantizedLuminance);
                quantizedColor = pow(abs(quantizedColor),0.4545);
                
                return half4(quantizedColor, 1.0);
            }
            ENDHLSL
        }
    }
}
