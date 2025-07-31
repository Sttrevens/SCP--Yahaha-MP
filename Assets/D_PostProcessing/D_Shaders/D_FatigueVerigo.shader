Shader "Custom/FatigueVerigo"
{
    Properties
    {
        _BlueNoiseTex("Blue Noise Texture" , 2D) = "white"{}
        _Intensity("Blur Intensity" , Range(0,1)) = 0.5
        _QuantizationNum("The Number Of Quantization",Int) = 6
        _NoiseIntensity("Blue Noise Intensity" , Range(0,1)) = 0.2
        
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness (px)", Range(1,5)) = 5
        _EdgeThreshold("Edge Sensitivity", Range(0,0.5)) = 0.02
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
            sampler2D _CameraDepthTexture;
            CBUFFER_START(UnityPerMaterial)
                float _Intensity;
                int _QuantizationNum;
                float _NoiseIntensity;
                float4 _OutlineColor;
                float  _OutlineThickness;
                float  _EdgeThreshold;
            CBUFFER_END

            half4 frag(Varyings IN) : SV_Target
            {
                half3 color = GetSource(IN).rgb;
                half3 gammaCorrected = pow(abs(color), 2.2);
                half  luminance = dot(gammaCorrected, half3(0.299,0.587,0.114));

                // 细粒度蓝噪声抖动
                half  blueNoise = tex2D(_BlueNoiseTex, frac(IN.uv * 0.5 * 1 + 0.5)).r;
                half  dither    = (blueNoise - 0.5) * (1.0/_QuantizationNum) * _NoiseIntensity;
                half  steps     = _QuantizationNum;

                // —— 1. clamp 保证不会负数 —— 
                half  lpd = clamp(luminance + dither, 0, 1);
                half  lf  = lpd * steps;

                // —— 2. 限制 ratio，不放大阴影 —— 
                half3 ratio = gammaCorrected / max(luminance, 1e-5);
                ratio = saturate(ratio);

                // —— 3. 对超暗区域强制黑 —— 
                half quantizedLuminance;
                half  edge = 0.5 / steps;
                half baseLevel = floor(lf);
                half fracPart  = lf - baseLevel;
                half smooth    = smoothstep(0.0, edge, fracPart)
                                    * (1.0 - smoothstep(1.0-edge, 1.0, fracPart));
                quantizedLuminance = (baseLevel + smooth) / steps;

                half3 quantColor = pow(abs(saturate(ratio * quantizedLuminance)), 0.4545);

                // 2. 描边检测（深度差分法）
                float2 texel = _OutlineThickness / float2(_ScreenParams.x, _ScreenParams.y);
                float centerDepth = Linear01Depth(tex2D(_CameraDepthTexture, IN.uv).r, _ZBufferParams);

                float d1 = Linear01Depth(tex2D(_CameraDepthTexture, IN.uv + float2( texel.x, 0)).r,_ZBufferParams);
                float d2 = Linear01Depth(tex2D(_CameraDepthTexture, IN.uv + float2(-texel.x, 0)).r,_ZBufferParams);
                float d3 = Linear01Depth(tex2D(_CameraDepthTexture, IN.uv + float2(0, texel.y)).r,_ZBufferParams);
                float d4 = Linear01Depth(tex2D(_CameraDepthTexture, IN.uv + float2(0,-texel.y)).r,_ZBufferParams);

                float maxDiff = max(max(abs(centerDepth-d1), abs(centerDepth-d2)),
                                    max(abs(centerDepth-d3), abs(centerDepth-d4)));

                // 如果深度差超过阈值，则认为是轮廓边缘
                float edgeMask = step(_EdgeThreshold, maxDiff);

                // 3. 最终输出：边缘处覆盖描边色
                half3 finalColor = lerp(quantColor, _OutlineColor.rgb, edgeMask);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
