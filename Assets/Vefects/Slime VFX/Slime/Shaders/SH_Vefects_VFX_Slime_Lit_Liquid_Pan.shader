// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_Slime_Lit_Liquid_Pan"
{
	Properties
	{
		_EmissiveMult("Emissive Mult", Float) = 0
		[Space(13)][Header(LUT Texture)][Space(33)]_LUT("LUT", 2D) = "white" {}
		_LUTRange("LUT Range", Float) = 1
		_LUTOffset("LUT Offset", Float) = 0
		_LUTErosion("LUT Erosion", Float) = 1
		_EroSSSmoothness("Ero SS Smoothness", Float) = 1
		[Space(13)][Header(Base Texture)][Space(33)]_BaseTexture("Base Texture", 2D) = "white" {}
		_UVScale("UV Scale", Vector) = (1,1,0,0)
		[Space(13)][Header(Secondary Texture)][Space(33)]_SecondaryTexture("Secondary Texture", 2D) = "white" {}
		_UVSecScale("UV Sec Scale", Vector) = (1,1,0,0)
		[Space(33)][Header(Distortion Texture)][Space(33)]_MainTexture1("UVD Tex", 2D) = "white" {}
		_UVDScale("UVD Scale", Vector) = (1,1,0,0)
		_UVDLerp("UVD Lerp", Float) = 0
		[Space(33)][Header(Cutout)][Space(33)]_CutoutTexture("Cutout Texture", 2D) = "white" {}
		_CutoutPower("Cutout Power", Float) = 1
		_CutoutMult("Cutout Mult", Float) = 1
		[Space(33)][Header(Pan Cutout)][Space(33)]_PanCutoutTexture("Pan Cutout Texture", 2D) = "white" {}
		_PanCutoutPower("Pan Cutout Power", Float) = 1
		_PanCutoutMult("Pan Cutout Mult", Float) = 1
		_PanCutoutUVScale("Pan Cutout UV Scale", Vector) = (1,1,0,0)
		_HorizontalCutOverallMult("Horizontal Cut Overall Mult", Float) = 1
		_HorizontalCutOverallPower("Horizontal Cut Overall Power", Float) = 1
		_HorizontalCut01Mult("Horizontal Cut 01 Mult", Float) = 1
		_HorizontalCut01Power("Horizontal Cut 01 Power", Float) = 1
		_HorizontalCut02Mult("Horizontal Cut 02 Mult", Float) = 1
		_HorizontalCut02Power("Horizontal Cut 02 Power", Float) = 1
		_ToSecondCutMultiply("To Second Cut Multiply", Float) = 1
		_ToSecondCutPower("To Second Cut Power", Float) = 1
		[Space(33)][Header(Oil Color)][Space(13)]_SlimeColorBase01("Slime Color Base 01", Color) = (0.1197541,1,0,0)
		_SlimeColorBase02("Slime Color Base 02", Color) = (0.4968553,0.3934868,0.01718676,0)
		_SlimeColorSpecular("Slime Color Specular", Color) = (1,1,1,0)
		_SlimeColorSpecularSS("Slime Color Specular SS", Float) = 0.9
		_SlimeColorSpecularSSSmooth("Slime Color Specular SS Smooth", Float) = 0
		_Specular("Specular", Float) = 0.01
		_Smoothness("Smoothness", Float) = 0.99
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 2
		_Src("Src", Float) = 5
		_Dst("Dst", Float) = 10
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_Cull]
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		Blend [_Src] [_Dst]
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.5
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float4 uv_texcoord;
			float4 uv2_texcoord2;
			float4 uv3_texcoord3;
		};

		uniform float _Src;
		uniform float _Dst;
		uniform float _ZWrite;
		uniform float _ZTest;
		uniform float _Cull;
		uniform float4 _SlimeColorBase01;
		uniform float4 _SlimeColorBase02;
		uniform sampler2D _LUT;
		uniform sampler2D _BaseTexture;
		uniform float2 _UVScale;
		uniform sampler2D _MainTexture1;
		uniform float2 _UVDScale;
		uniform float _UVDLerp;
		uniform sampler2D _SecondaryTexture;
		uniform float2 _UVSecScale;
		uniform float _ToSecondCutPower;
		uniform float _ToSecondCutMultiply;
		uniform sampler2D _CutoutTexture;
		uniform float4 _CutoutTexture_ST;
		uniform float _CutoutPower;
		uniform float _CutoutMult;
		uniform sampler2D _PanCutoutTexture;
		uniform float2 _PanCutoutUVScale;
		uniform float _PanCutoutPower;
		uniform float _PanCutoutMult;
		uniform float _HorizontalCut01Power;
		uniform float _HorizontalCut01Mult;
		uniform float _HorizontalCut02Power;
		uniform float _HorizontalCut02Mult;
		uniform float _HorizontalCutOverallPower;
		uniform float _HorizontalCutOverallMult;
		uniform float _LUTErosion;
		uniform float _LUTRange;
		uniform float _LUTOffset;
		uniform float4 _SlimeColorSpecular;
		uniform float _SlimeColorSpecularSS;
		uniform float _SlimeColorSpecularSSSmooth;
		uniform float _EmissiveMult;
		uniform float _Specular;
		uniform float _Smoothness;
		uniform float _EroSSSmoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 temp_output_412_0 = ( i.uv_texcoord.xy * _UVScale );
			float2 appendResult391 = (float2(i.uv_texcoord.z , i.uv_texcoord.w));
			float randomUVOffset393 = i.uv2_texcoord2.w;
			float2 temp_output_392_0 = ( i.uv_texcoord.xy * _UVDScale );
			float2 lerpResult429 = lerp( float2( 0,0 ) , ( ( (tex2D( _MainTexture1, ( ( temp_output_392_0 + appendResult391 ) + randomUVOffset393 ) )).rg + -0.5 ) * 2.0 ) , _UVDLerp);
			float2 temp_output_414_0 = ( i.uv_texcoord.xy * _UVSecScale );
			float4 lerpResult441 = lerp( tex2D( _BaseTexture, ( ( ( temp_output_412_0 + appendResult391 ) + randomUVOffset393 ) + lerpResult429 ) ) , tex2D( _SecondaryTexture, ( ( ( temp_output_414_0 + appendResult391 ) + randomUVOffset393 ) + lerpResult429 ) ) , saturate( ( pow( ( 1.0 - i.uv_texcoord.xy.y ) , _ToSecondCutPower ) * _ToSecondCutMultiply ) ));
			float2 uv_CutoutTexture = i.uv_texcoord * _CutoutTexture_ST.xy + _CutoutTexture_ST.zw;
			float4 temp_cast_0 = (_CutoutPower).xxxx;
			float2 appendResult362 = (float2(i.uv3_texcoord3.x , i.uv3_texcoord3.y));
			float4 temp_cast_1 = (_PanCutoutPower).xxxx;
			float4 temp_output_437_0 = ( i.uv2_texcoord2.z + saturate( ( 1.0 - ( saturate( ( saturate( ( saturate( ( pow( tex2D( _CutoutTexture, uv_CutoutTexture ) , temp_cast_0 ) * _CutoutMult ) ) * saturate( ( pow( tex2D( _PanCutoutTexture, ( ( i.uv_texcoord.xy * _PanCutoutUVScale ) + appendResult362 ) ) , temp_cast_1 ) * _PanCutoutMult ) ) ) ) * saturate( ( pow( ( saturate( ( pow( i.uv_texcoord.xy.x , _HorizontalCut01Power ) * _HorizontalCut01Mult ) ) * saturate( ( pow( ( 1.0 - i.uv_texcoord.xy.x ) , _HorizontalCut02Power ) * _HorizontalCut02Mult ) ) ) , _HorizontalCutOverallPower ) * _HorizontalCutOverallMult ) ) ) ) * 30.0 ) ) ) );
			float4 smoothstepResult454 = smoothstep( temp_output_437_0 , ( temp_output_437_0 + float4( 1,0,0,0 ) ) , lerpResult441);
			float4 lerpResult451 = lerp( lerpResult441 , saturate( smoothstepResult454 ) , _LUTErosion);
			float4 temp_output_447_0 = ( ( lerpResult451 * _LUTRange ) + _LUTOffset );
			float4 tex2DNode479 = tex2D( _LUT, temp_output_447_0.rg );
			float4 lerpResult465 = lerp( _SlimeColorBase01 , _SlimeColorBase02 , tex2DNode479);
			float4 temp_cast_3 = (_SlimeColorSpecularSS).xxxx;
			float4 temp_cast_4 = (( _SlimeColorSpecularSS + _SlimeColorSpecularSSSmooth )).xxxx;
			float4 smoothstepResult466 = smoothstep( temp_cast_3 , temp_cast_4 , tex2DNode479);
			float4 lerpResult468 = lerp( ( i.vertexColor * lerpResult465 ) , _SlimeColorSpecular , smoothstepResult466);
			o.Albedo = lerpResult468.rgb;
			o.Emission = ( lerpResult468 * _EmissiveMult ).rgb;
			float3 temp_cast_7 = (_Specular).xxx;
			o.Specular = temp_cast_7;
			o.Smoothness = _Smoothness;
			float4 smoothstepResult443 = smoothstep( temp_output_437_0 , ( temp_output_437_0 + _EroSSSmoothness ) , lerpResult441);
			o.Alpha = saturate( ( i.vertexColor.a * saturate( smoothstepResult443 ) ) ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows noforwardadd 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float4 customPack3 : TEXCOORD3;
				float3 worldPos : TEXCOORD4;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xyzw = customInputData.uv_texcoord;
				o.customPack1.xyzw = v.texcoord;
				o.customPack2.xyzw = customInputData.uv2_texcoord2;
				o.customPack2.xyzw = v.texcoord1;
				o.customPack3.xyzw = customInputData.uv3_texcoord3;
				o.customPack3.xyzw = v.texcoord2;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xyzw;
				surfIN.uv2_texcoord2 = IN.customPack2.xyzw;
				surfIN.uv3_texcoord3 = IN.customPack3.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;-8320,3200;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;358;-8064,3328;Inherit;False;Property;_PanCutoutUVScale;Pan Cutout UV Scale;24;0;Create;True;0;0;0;False;0;False;1,1;1,0.96;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexCoordVertexDataNode;359;-8320,2816;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;360;-7552,3584;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;361;-8064,3200;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;362;-8064,2816;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;363;-7168,3712;Inherit;False;Property;_HorizontalCut01Power;Horizontal Cut 01 Power;28;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;364;-7440,3968;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;365;-7184,4096;Inherit;False;Property;_HorizontalCut02Power;Horizontal Cut 02 Power;30;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;366;-7808,3200;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;367;-7168,3584;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;368;-6912,3712;Inherit;False;Property;_HorizontalCut01Mult;Horizontal Cut 01 Mult;27;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;369;-7184,3968;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;370;-6928,4096;Inherit;False;Property;_HorizontalCut02Mult;Horizontal Cut 02 Mult;29;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;371;-7168,3328;Inherit;False;Property;_PanCutoutPower;Pan Cutout Power;22;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;372;-6912,3584;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;373;-6928,3968;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;374;-7168,2944;Inherit;False;Property;_CutoutPower;Cutout Power;19;0;Create;True;0;0;0;False;0;False;1;1.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;376;-7552,3200;Inherit;True;Property;_PanCutoutTexture;Pan Cutout Texture;21;0;Create;True;0;0;0;False;3;Space(33);Header(Pan Cutout);Space(33);False;-1;a7b46127e3d00254e8dfcf84bef27394;a7b46127e3d00254e8dfcf84bef27394;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;375;-7552,2816;Inherit;True;Property;_CutoutTexture;Cutout Texture;18;0;Create;True;0;0;0;False;3;Space(33);Header(Cutout);Space(33);False;-1;b1f212ebadb408243930c2abe294793c;b1f212ebadb408243930c2abe294793c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;377;-7168,3200;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;378;-6912,3328;Inherit;False;Property;_PanCutoutMult;Pan Cutout Mult;23;0;Create;True;0;0;0;False;0;False;1;13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;379;-6656,3584;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;380;-6672,3968;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;381;-7168,2816;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;382;-6912,2944;Inherit;False;Property;_CutoutMult;Cutout Mult;20;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;383;-4992,2304;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;384;-9344,128;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;385;-9472,1152;Inherit;False;Property;_UVDScale;UVD Scale;15;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;386;-9728,1024;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;387;-6912,3200;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;388;-6400,3584;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;389;-6144,3712;Inherit;False;Property;_HorizontalCutOverallPower;Horizontal Cut Overall Power;26;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;390;-6912,2816;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;391;-9216,640;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;392;-9472,1024;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;393;-4736,2560;Inherit;False;randomUVOffset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;394;-6656,3200;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;395;-6144,3584;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;396;-5888,3712;Inherit;False;Property;_HorizontalCutOverallMult;Horizontal Cut Overall Mult;25;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;397;-6656,2816;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;398;-8448,384;Inherit;False;393;randomUVOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;399;-8960,1024;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;400;-6400,2816;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;401;-5888,3584;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;402;-8448,1024;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;403;-6272,2816;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;404;-5760,3584;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;405;-9472,640;Inherit;False;Property;_UVScale;UV Scale;9;0;Create;True;0;0;0;False;0;False;1,1;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;406;-9728,512;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;407;-9728,1536;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;408;-9472,1664;Inherit;False;Property;_UVSecScale;UV Sec Scale;12;0;Create;True;0;0;0;False;0;False;1,1;2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;409;-5632,2816;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;410;-8192,1024;Inherit;True;Property;_MainTexture1;UVD Tex;14;0;Create;True;0;0;0;False;3;Space(33);Header(Distortion Texture);Space(33);False;-1;64f823e64f4977243bb7c015fa29d472;96737679f794cdf4ab003933273d69da;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;411;-5760,1536;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;412;-9472,512;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;413;-7808,1024;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;414;-9472,1536;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;415;-5504,2816;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;416;-5248,2944;Inherit;False;Constant;_Float0;Float 0;44;0;Create;True;0;0;0;False;0;False;30;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;417;-5520,1536;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;418;-8960,512;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;419;-7552,1024;Inherit;False;ConstantBiasScale;-1;;12;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT2;0,0;False;1;FLOAT;-0.5;False;2;FLOAT;2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;420;-8448,1408;Inherit;False;393;randomUVOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;421;-8960,1536;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;422;-7296,896;Inherit;False;Constant;_Vector0;Vector 0;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;423;-7296,1152;Inherit;False;Property;_UVDLerp;UVD Lerp;17;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;424;-5248,2816;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;425;-5376,1664;Inherit;False;Property;_ToSecondCutPower;To Second Cut Power;32;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;426;-5376,1536;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;427;-8448,512;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;428;-8448,1536;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;429;-7040,1024;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;430;-4992,2816;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;431;-5120,1664;Inherit;False;Property;_ToSecondCutMultiply;To Second Cut Multiply;31;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;432;-5120,1536;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;433;-7040,512;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;434;-7040,1536;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;435;-4736,2816;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;436;-4864,1536;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;437;-4096,2432;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;439;-6528,512;Inherit;True;Property;_BaseTexture;Base Texture;8;0;Create;True;0;0;0;False;3;Space(13);Header(Base Texture);Space(33);False;-1;64f823e64f4977243bb7c015fa29d472;64f823e64f4977243bb7c015fa29d472;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;440;-6528,1536;Inherit;True;Property;_SecondaryTexture;Secondary Texture;11;0;Create;True;0;0;0;False;3;Space(13);Header(Secondary Texture);Space(33);False;-1;64f823e64f4977243bb7c015fa29d472;64f823e64f4977243bb7c015fa29d472;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;441;-4224,512;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;455;-3584,1024;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;454;-3584,896;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;453;-3712,384;Inherit;False;Property;_LUTErosion;LUT Erosion;6;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;456;-3328,896;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;451;-3712,512;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;452;-3328,384;Inherit;False;Property;_LUTRange;LUT Range;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;449;-3072,384;Inherit;False;Property;_LUTOffset;LUT Offset;4;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;469;-3328,512;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;438;-3584,1792;Inherit;False;Property;_EroSSSmoothness;Ero SS Smoothness;7;0;Create;True;0;0;0;False;0;False;1;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;447;-3072,512;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;442;-3584,1664;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;473;-1664,512;Inherit;False;Property;_SlimeColorSpecularSS;Slime Color Specular SS;36;0;Create;True;0;0;0;False;0;False;0.9;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;474;-1664,640;Inherit;False;Property;_SlimeColorSpecularSSSmooth;Slime Color Specular SS Smooth;37;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;475;-2432,256;Inherit;False;Property;_SlimeColorBase01;Slime Color Base 01;33;0;Create;True;0;0;0;False;3;Space(33);Header(Oil Color);Space(13);False;0.1197541,1,0,0;0.119754,1,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;476;-2432,432;Inherit;False;Property;_SlimeColorBase02;Slime Color Base 02;34;0;Create;True;0;0;0;False;0;False;0.4968553,0.3934868,0.01718676,0;0.4968553,0.3934867,0.01718676,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;479;-2560,640;Inherit;True;Property;_LUT;LUT;1;0;Create;True;0;0;0;False;3;Space(13);Header(LUT Texture);Space(33);False;-1;fdfd935d00cf0964897b8e875b510f96;fdfd935d00cf0964897b8e875b510f96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;443;-3584,1536;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;444;-2304,0;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;465;-1664,128;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;467;-1392,560;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;445;-3328,1536;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;457;-1664,0;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;466;-1424,336;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;472;-1408,-256;Inherit;False;Property;_SlimeColorSpecular;Slime Color Specular;35;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;446;-1664,768;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;231;464,-48;Inherit;False;1252;162.95;Ge Lush was here! <3;5;236;235;234;233;232;Ge Lush was here! <3;0.3782746,0.2798741,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;468;-1248,0;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;471;-640,0;Inherit;False;Property;_EmissiveMult;Emissive Mult;0;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;460;-1280,768;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;477;-640,256;Inherit;False;Property;_Specular;Specular;38;0;Create;True;0;0;0;False;0;False;0.01;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;478;-640,384;Inherit;False;Property;_Smoothness;Smoothness;39;0;Create;True;0;0;0;False;0;False;0.99;0.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;232;768,0;Inherit;False;Property;_Src;Src;41;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;233;1024,0;Inherit;False;Property;_Dst;Dst;42;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;234;1280,0;Inherit;False;Property;_ZWrite;ZWrite;43;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;1536,0;Inherit;False;Property;_ZTest;ZTest;44;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;512,0;Inherit;False;Property;_Cull;Cull;40;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;448;-2816,512;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;450;-2816,384;Inherit;False;Property;_LUTPan;LUT Pan;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;458;-8704,768;Inherit;False;Property;_UVPan;UV Pan;10;0;Create;True;0;0;0;False;0;False;0,0;-0.1,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;459;-8704,640;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;461;-8704,1280;Inherit;False;Property;_UVDPan;UVD Pan;16;0;Create;True;0;0;0;False;0;False;0,0;0.1,0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;462;-8704,1152;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;463;-8704,1664;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;464;-8704,1792;Inherit;False;Property;_UVSecPan;UV Sec Pan;13;0;Create;True;0;0;0;False;0;False;0,0;0.1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;470;-640,128;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;5;ASEMaterialInspector;0;0;StandardSpecular;Vefects/SH_Vefects_VFX_Slime_Lit_Liquid_Pan;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;Back;0;True;_ZWrite;0;True;_ZTest;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;True;_Src;10;True;_Dst;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;2;-1;-1;-1;0;False;0;0;True;_Cull;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;361;0;357;0
WireConnection;361;1;358;0
WireConnection;362;0;359;1
WireConnection;362;1;359;2
WireConnection;364;0;360;1
WireConnection;366;0;361;0
WireConnection;366;1;362;0
WireConnection;367;0;360;1
WireConnection;367;1;363;0
WireConnection;369;0;364;0
WireConnection;369;1;365;0
WireConnection;372;0;367;0
WireConnection;372;1;368;0
WireConnection;373;0;369;0
WireConnection;373;1;370;0
WireConnection;376;1;366;0
WireConnection;377;0;376;0
WireConnection;377;1;371;0
WireConnection;379;0;372;0
WireConnection;380;0;373;0
WireConnection;381;0;375;0
WireConnection;381;1;374;0
WireConnection;387;0;377;0
WireConnection;387;1;378;0
WireConnection;388;0;379;0
WireConnection;388;1;380;0
WireConnection;390;0;381;0
WireConnection;390;1;382;0
WireConnection;391;0;384;3
WireConnection;391;1;384;4
WireConnection;392;0;386;0
WireConnection;392;1;385;0
WireConnection;393;0;383;4
WireConnection;394;0;387;0
WireConnection;395;0;388;0
WireConnection;395;1;389;0
WireConnection;397;0;390;0
WireConnection;399;0;392;0
WireConnection;399;1;391;0
WireConnection;400;0;397;0
WireConnection;400;1;394;0
WireConnection;401;0;395;0
WireConnection;401;1;396;0
WireConnection;402;0;399;0
WireConnection;402;1;398;0
WireConnection;403;0;400;0
WireConnection;404;0;401;0
WireConnection;409;0;403;0
WireConnection;409;1;404;0
WireConnection;410;1;402;0
WireConnection;412;0;406;0
WireConnection;412;1;405;0
WireConnection;413;0;410;0
WireConnection;414;0;407;0
WireConnection;414;1;408;0
WireConnection;415;0;409;0
WireConnection;417;0;411;2
WireConnection;418;0;412;0
WireConnection;418;1;391;0
WireConnection;419;3;413;0
WireConnection;421;0;414;0
WireConnection;421;1;391;0
WireConnection;424;0;415;0
WireConnection;424;1;416;0
WireConnection;426;0;417;0
WireConnection;426;1;425;0
WireConnection;427;0;418;0
WireConnection;427;1;398;0
WireConnection;428;0;421;0
WireConnection;428;1;420;0
WireConnection;429;0;422;0
WireConnection;429;1;419;0
WireConnection;429;2;423;0
WireConnection;430;0;424;0
WireConnection;432;0;426;0
WireConnection;432;1;431;0
WireConnection;433;0;427;0
WireConnection;433;1;429;0
WireConnection;434;0;428;0
WireConnection;434;1;429;0
WireConnection;435;0;430;0
WireConnection;436;0;432;0
WireConnection;437;0;383;3
WireConnection;437;1;435;0
WireConnection;439;1;433;0
WireConnection;440;1;434;0
WireConnection;441;0;439;0
WireConnection;441;1;440;0
WireConnection;441;2;436;0
WireConnection;455;0;437;0
WireConnection;454;0;441;0
WireConnection;454;1;437;0
WireConnection;454;2;455;0
WireConnection;456;0;454;0
WireConnection;451;0;441;0
WireConnection;451;1;456;0
WireConnection;451;2;453;0
WireConnection;469;0;451;0
WireConnection;469;1;452;0
WireConnection;447;0;469;0
WireConnection;447;1;449;0
WireConnection;442;0;437;0
WireConnection;442;1;438;0
WireConnection;479;1;447;0
WireConnection;443;0;441;0
WireConnection;443;1;437;0
WireConnection;443;2;442;0
WireConnection;465;0;475;0
WireConnection;465;1;476;0
WireConnection;465;2;479;0
WireConnection;467;0;473;0
WireConnection;467;1;474;0
WireConnection;445;0;443;0
WireConnection;457;0;444;0
WireConnection;457;1;465;0
WireConnection;466;0;479;0
WireConnection;466;1;473;0
WireConnection;466;2;467;0
WireConnection;446;0;444;4
WireConnection;446;1;445;0
WireConnection;468;0;457;0
WireConnection;468;1;472;0
WireConnection;468;2;466;0
WireConnection;460;0;446;0
WireConnection;448;0;447;0
WireConnection;448;2;450;0
WireConnection;459;0;412;0
WireConnection;459;2;458;0
WireConnection;462;0;392;0
WireConnection;462;2;461;0
WireConnection;463;0;414;0
WireConnection;463;2;464;0
WireConnection;470;0;468;0
WireConnection;470;1;471;0
WireConnection;0;0;468;0
WireConnection;0;2;470;0
WireConnection;0;3;477;0
WireConnection;0;4;478;0
WireConnection;0;9;460;0
ASEEND*/
//CHKSM=D98802BF8DAA7ABEFDA6BCB5D1E97636C83175F8