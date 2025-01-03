Shader "ShaderYahaha/Effect/VFXSimpleWithDepth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color ("_Color", Color) = (1,1,1,1)

        [Space][Header(__________ Fade __________)][Space]
        [Toggle(_EnableDepthTexture)]_EnableDepthTexture("EnableDepthTexture", float) = 0
        _DepthFade ("_DepthFade", Range(0,3)) = 1

        [Space][Header(__________ Other __________)][Space]
        [Toggle]_BlackToAlpha ("_BlackToAlpha", float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendSrc("_BlendSrc",float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendDst("_BlendDst",float) = 10
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode ("CullMode", float) = 0
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType"="Plane" }
        LOD 100
        ZWrite On
        // Cull Off
        Cull [_CullMode]
        Blend [_BlendSrc] [_BlendDst]

        Pass
        {
            ZWrite On
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local_fragment _EnableDepthTexture


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.yahaha.props.multiplayertemplate/Assets/Original Assets/VFX/Shader/gfunc.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD1;
                float4 projPos : TEXCOORD4;

            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float _DepthFade;
                float _BlackToAlpha;
            CBUFFER_END
            sampler2D _MainTex;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.projPos = ComputeScreenPos (o.vertex);
                o.projPos.z = -TransformWorldToView( mul(unity_ObjectToWorld,v.vertex) ).z;
                return o;
            }



            half4 frag (v2f i) : SV_Target{

                
                //depth fade
                float depth = DepthFadeS(i.projPos,_DepthFade);
                depth = saturate(depth);
                i.color.a *= depth;

                half4 col = tex2D(_MainTex, i.uv);
                col = lerp(col,half4(1,1,1,col.r),_BlackToAlpha);
                col *= _Color * i.color;



                return col;
            }
            ENDHLSL
        }
    }
}
