Shader "Custom/FatigueVerigo"
{
    Properties
    {
        [HDR]_Color ("Base Color", Color) = (1,1,1,1)  // 默认透明黑色
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.5
        _TimeSpeed ("Time Speed", Range(0.1, 10)) = 1.0
        _MainTex ("Base (RGB)", 2D) = "black" {}       // 默认黑色纹理（透明）
    }
    
    SubShader
    {
        Tags { 
            "Queue" = "Transparent"       // 透明渲染队列
            "RenderType" = "Transparent"   // 渲染类型声明
            "IgnoreProjector" = "True"    // 禁用投影器
        }
        
        Blend SrcAlpha OneMinusSrcAlpha    // 标准透明混合
        ZWrite Off                         // 关闭深度写入
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // 新增颜色属性
            float4 _Color;
            float _DistortionStrength;
            float _TimeSpeed;
            sampler2D _MainTex;
            float4 _MainTex_ST;  // 改用_ST获取平铺偏移参数

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);  // 应用纹理平铺偏移
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float time = _Time.y * _TimeSpeed;
                
                // 添加UV流动效果
                float2 scrollUV = i.uv + float2(time * 0.1, time * 0.08);
                
                // 改进扭曲算法（多重波形叠加）
                float2 distortion = float2(
                    sin(scrollUV.y * 20 + time * 3) * 0.1 * _DistortionStrength +
                    cos(scrollUV.x * 15 - time * 2) * 0.08 * _DistortionStrength,
                    
                    sin(scrollUV.x * 18 + time * 4) * 0.12 * _DistortionStrength +
                    cos(scrollUV.y * 22 - time * 1.5) * 0.09 * _DistortionStrength
                );

                // 采样纹理时叠加基础颜色
                half4 col = tex2D(_MainTex, i.uv + distortion) * _Color;
                
                // 添加方形边缘渐隐效果
                float edgeFade = 1.0 - max(abs(i.uv.x - 0.5) * 2, abs(i.uv.y - 0.5) * 2);
                // col.a *= edgeFade;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
