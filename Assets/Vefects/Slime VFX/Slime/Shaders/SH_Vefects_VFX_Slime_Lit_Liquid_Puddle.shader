// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_Slime_Lit_Liquid_Puddle"
{
	Properties
	{
		_EmissiveMult("Emissive Mult", Float) = 0
		_Specular("Specular", Float) = 0
		_Smoothness("Smoothness", Float) = 1
		_Ero("Ero", Float) = 0
		[Space(13)][Header(LUT Texture)][Space(33)]_LUT("LUT", 2D) = "white" {}
		_LUTRange("LUT Range", Float) = 1
		_LUTOffset("LUT Offset", Float) = 0
		_LUTPan("LUT Pan", Float) = 0
		_LUTErosion("LUT Erosion", Float) = 1
		_EroSSSmoothness("Ero SS Smoothness", Float) = 1
		[Space(13)][Header(Liquid Texture)][Space(33)]_LiquidTexture("Liquid Texture", 2D) = "white" {}
		_UVScale("UV Scale", Vector) = (1,1,0,0)
		_UVPan("UV Pan", Vector) = (0,0,0,0)
		[Space(13)][Header(Liquid Secondary Texture)][Space(33)]_LiquidSecondaryTexture("Liquid Secondary Texture", 2D) = "white" {}
		_UVSecScale("UV Sec Scale", Vector) = (1,1,0,0)
		_UVSecPan("UV Sec Pan", Vector) = (0,0,0,0)
		[Space(13)][Header(Liquid Normal)][Space(33)]_LiquidNormal("Liquid Normal", 2D) = "bump" {}
		_LiquidSecondaryNormal("Liquid Secondary Normal", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Float) = 1
		[Space(33)][Header(Distortion Texture)][Space(33)]_MainTexture1("UVD Tex", 2D) = "white" {}
		_UVDScale("UVD Scale", Vector) = (1,1,0,0)
		_UVDPan("UVD Pan", Vector) = (0,0,0,0)
		_UVDLerp("UVD Lerp", Float) = 0
		[Space(33)][Header(Cutout)][Space(33)]_CutoutTexture("Cutout Texture", 2D) = "white" {}
		_CutoutPower("Cutout Power", Float) = 1
		_CutoutMult("Cutout Mult", Float) = 1
		_ToDropletsCutPower("To Droplets Cut Power", Float) = 1
		_ToDropletsCutMultiply("To Droplets Cut Multiply", Float) = 1
		_ToDropletsGradientSize("To Droplets Gradient Size", Float) = 0
		_ToDropletsGradientSharpness("To Droplets Gradient Sharpness", Float) = 0
		[Space(33)][Header(Oil Color)][Space(13)]_SlimeColorBase01("Slime Color Base 01", Color) = (0.4171708,1,0,0)
		_SlimeColorBase02("Slime Color Base 02", Color) = (0.4402515,0.3744411,0,0)
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 2
		_SlimeColorSpecular("Slime Color Specular", Color) = (1,1,1,0)
		_SlimeColorSpecularSS("Slime Color Specular SS", Float) = 0.9
		_SlimeColorSpecularSSSmooth("Slime Color Specular SS Smooth", Float) = 0
		_Src("Src", Float) = 5
		_Dst("Dst", Float) = 10
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.5
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float4 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _Src;
		uniform float _Dst;
		uniform float _ZWrite;
		uniform float _ZTest;
		uniform float _Cull;
		uniform sampler2D _LiquidNormal;
		uniform float2 _UVPan;
		uniform float2 _UVScale;
		uniform sampler2D _MainTexture1;
		uniform float2 _UVDPan;
		uniform float2 _UVDScale;
		uniform float _UVDLerp;
		uniform sampler2D _LiquidSecondaryNormal;
		uniform float2 _UVSecPan;
		uniform float2 _UVSecScale;
		uniform float _ToDropletsGradientSize;
		uniform float _ToDropletsGradientSharpness;
		uniform float _ToDropletsCutPower;
		uniform float _ToDropletsCutMultiply;
		uniform float _NormalIntensity;
		uniform float _Ero;
		uniform sampler2D _CutoutTexture;
		uniform float4 _CutoutTexture_ST;
		uniform float _CutoutPower;
		uniform float _CutoutMult;
		uniform float _EroSSSmoothness;
		uniform sampler2D _LiquidTexture;
		uniform sampler2D _LiquidSecondaryTexture;
		uniform float4 _SlimeColorBase01;
		uniform float4 _SlimeColorBase02;
		uniform sampler2D _LUT;
		uniform float _LUTPan;
		uniform float _LUTErosion;
		uniform float _LUTRange;
		uniform float _LUTOffset;
		uniform float4 _SlimeColorSpecular;
		uniform float _SlimeColorSpecularSS;
		uniform float _SlimeColorSpecularSSSmooth;
		uniform float _EmissiveMult;
		uniform float _Specular;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 ase_worldPos = i.worldPos;
			float2 appendResult359 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner392 = ( 1.0 * _Time.y * _UVPan + ( appendResult359 * _UVScale ));
			float2 panner363 = ( 1.0 * _Time.y * _UVDPan + ( appendResult359 * _UVDScale ));
			float2 lerpResult390 = lerp( float2( 0,0 ) , ( ( (tex2D( _MainTexture1, panner363 )).rg + -0.5 ) * 2.0 ) , _UVDLerp);
			float2 temp_output_398_0 = ( panner392 + lerpResult390 );
			float2 panner393 = ( 1.0 * _Time.y * _UVSecPan + ( appendResult359 * _UVSecScale ));
			float2 temp_output_399_0 = ( panner393 + lerpResult390 );
			float clampResult9_g13 = clamp( ( ( length( (float2( -1,-1 ) + (i.uv_texcoord.xy - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) + -_ToDropletsGradientSize ) * _ToDropletsGradientSharpness ) , 0.0 , 1.0 );
			float temp_output_386_0 = clampResult9_g13;
			float temp_output_405_0 = saturate( ( pow( temp_output_386_0 , _ToDropletsCutPower ) * _ToDropletsCutMultiply ) );
			float3 lerpResult439 = lerp( UnpackNormal( tex2D( _LiquidNormal, temp_output_398_0 ) ) , UnpackNormal( tex2D( _LiquidSecondaryNormal, temp_output_399_0 ) ) , temp_output_405_0);
			float3 lerpResult433 = lerp( float3(0,0,1) , lerpResult439 , _NormalIntensity);
			float3 norm417 = lerpResult433;
			o.Normal = norm417;
			float clampResult9_g14 = clamp( ( ( length( (float2( -1,-1 ) + (i.uv_texcoord.xy - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) + -0.0 ) * 1.0 ) , 0.0 , 1.0 );
			float2 uv_CutoutTexture = i.uv_texcoord * _CutoutTexture_ST.xy + _CutoutTexture_ST.zw;
			float4 temp_cast_0 = (_CutoutPower).xxxx;
			float4 temp_output_403_0 = ( ( ( _Ero + i.uv_texcoord.z ) * saturate( clampResult9_g14 ) ) + saturate( ( 1.0 - saturate( ( pow( tex2D( _CutoutTexture, uv_CutoutTexture ) , temp_cast_0 ) * _CutoutMult ) ) ) ) );
			float4 lerpResult408 = lerp( tex2D( _LiquidTexture, temp_output_398_0 ) , tex2D( _LiquidSecondaryTexture, temp_output_399_0 ) , temp_output_405_0);
			float4 smoothstepResult410 = smoothstep( temp_output_403_0 , ( temp_output_403_0 + _EroSSSmoothness ) , lerpResult408);
			float4 op415 = saturate( ( i.vertexColor.a * saturate( smoothstepResult410 ) ) );
			float2 temp_cast_1 = (_LUTPan).xx;
			float4 smoothstepResult431 = smoothstep( temp_output_403_0 , ( temp_output_403_0 + float4( 1,0,0,0 ) ) , lerpResult408);
			float4 lerpResult428 = lerp( lerpResult408 , saturate( smoothstepResult431 ) , _LUTErosion);
			float2 panner425 = ( 1.0 * _Time.y * temp_cast_1 + ( ( lerpResult428 * _LUTRange ) + _LUTOffset ).rg);
			float4 tex2DNode461 = tex2D( _LUT, panner425 );
			float4 lerpResult453 = lerp( _SlimeColorBase01 , _SlimeColorBase02 , saturate( tex2DNode461 ));
			float4 temp_cast_3 = (_SlimeColorSpecularSS).xxxx;
			float4 temp_cast_4 = (( _SlimeColorSpecularSS + _SlimeColorSpecularSSSmooth )).xxxx;
			float4 smoothstepResult443 = smoothstep( temp_cast_3 , temp_cast_4 , tex2DNode461);
			float4 lerpResult445 = lerp( ( i.vertexColor * lerpResult453 ) , _SlimeColorSpecular , saturate( smoothstepResult443 ));
			float4 alb418 = lerpResult445;
			o.Albedo = ( op415 * alb418 ).rgb;
			float4 em422 = ( lerpResult445 * _EmissiveMult );
			o.Emission = em422.rgb;
			o.Specular = saturate( ( _Specular * op415 ) ).rgb;
			o.Smoothness = _Smoothness;
			float4 temp_output_456_0 = op415;
			o.Alpha = temp_output_456_0.r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows 

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
				float3 worldPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xyzw = customInputData.uv_texcoord;
				o.customPack1.xyzw = v.texcoord;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
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
Node;AmplifyShaderEditor.WorldPosInputsNode;357;-11520,-128;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;358;-11264,768;Inherit;False;Property;_UVDScale;UVD Scale;21;0;Create;True;0;0;0;False;0;False;1,1;1.777,2.222;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;359;-11136,-128;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;360;-11264,640;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;361;-10880,768;Inherit;False;Property;_UVDPan;UVD Pan;22;0;Create;True;0;0;0;False;0;False;0,0;-0.05,0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;362;-7936,2560;Inherit;False;Property;_CutoutPower;Cutout Power;25;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;363;-10880,640;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;364;-8320,2432;Inherit;True;Property;_CutoutTexture;Cutout Texture;24;0;Create;True;0;0;0;False;3;Space(33);Header(Cutout);Space(33);False;-1;bfcee05dba39b9c448796bf658e1d66a;bfcee05dba39b9c448796bf658e1d66a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;365;-7936,2432;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;366;-7680,2560;Inherit;False;Property;_CutoutMult;Cutout Mult;26;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;367;-9984,640;Inherit;True;Property;_MainTexture1;UVD Tex;20;0;Create;True;0;0;0;False;3;Space(33);Header(Distortion Texture);Space(33);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;368;-7680,2432;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;369;-9600,640;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;370;-11264,384;Inherit;False;Property;_UVScale;UV Scale;12;0;Create;True;0;0;0;False;0;False;1,1;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;371;-11264,1280;Inherit;False;Property;_UVSecScale;UV Sec Scale;15;0;Create;True;0;0;0;False;0;False;1,1;0.3,0.3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;372;-8320,1536;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;373;-8320,1792;Inherit;False;Property;_ToDropletsGradientSize;To Droplets Gradient Size;29;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;374;-8320,1920;Inherit;False;Property;_ToDropletsGradientSharpness;To Droplets Gradient Sharpness;30;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;375;-7504,2096;Inherit;False;Constant;_Float0;Float 0;41;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;376;-7472,2176;Inherit;False;Constant;_Float1;Float 1;41;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;377;-9088,512;Inherit;False;Constant;_Vector0;Vector 0;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;378;-8832,640;Inherit;False;Property;_UVDLerp;UVD Lerp;23;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;379;-9344,640;Inherit;False;ConstantBiasScale;-1;;12;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT2;0,0;False;1;FLOAT;-0.5;False;2;FLOAT;2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;380;-11264,256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;381;-11264,1152;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;382;-10880,1280;Inherit;False;Property;_UVSecPan;UV Sec Pan;16;0;Create;True;0;0;0;False;0;False;0,0;-0.001,-0.002;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;383;-10880,384;Inherit;False;Property;_UVPan;UV Pan;13;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;384;-7040,1664;Inherit;False;Property;_ToDropletsCutPower;To Droplets Cut Power;27;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;385;-7424,2432;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;386;-8064,1536;Inherit;False;RadialGradient;-1;;13;ec972f7745a8353409da2eb8d000a2e3;0;3;1;FLOAT2;0,0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;387;-7296,2064;Inherit;False;RadialGradient;-1;;14;ec972f7745a8353409da2eb8d000a2e3;0;3;1;FLOAT2;0,0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;388;-6272,2048;Inherit;False;Property;_Ero;Ero;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;389;-6272,1792;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;390;-8832,512;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;391;-6272,2688;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;392;-10880,256;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;393;-10880,1152;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;394;-7040,1536;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;395;-6784,1664;Inherit;False;Property;_ToDropletsCutMultiply;To Droplets Cut Multiply;28;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;396;-6912,2096;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;397;-6064,2048;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;398;-8832,256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;399;-8832,1152;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;400;-6016,2688;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;401;-6784,1536;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;402;-6224,2320;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;403;-5760,2304;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;405;-6528,1536;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;406;-8448,256;Inherit;True;Property;_LiquidTexture;Liquid Texture;11;0;Create;True;0;0;0;False;3;Space(13);Header(Liquid Texture);Space(33);False;-1;64f823e64f4977243bb7c015fa29d472;64f823e64f4977243bb7c015fa29d472;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;407;-8448,1152;Inherit;True;Property;_LiquidSecondaryTexture;Liquid Secondary Texture;14;0;Create;True;0;0;0;False;3;Space(13);Header(Liquid Secondary Texture);Space(33);False;-1;64f823e64f4977243bb7c015fa29d472;64f823e64f4977243bb7c015fa29d472;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;408;-5888,768;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;440;-5376,768;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;431;-5376,640;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;430;-5504,128;Inherit;False;Property;_LUTErosion;LUT Erosion;9;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;432;-5120,640;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;428;-5504,256;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;429;-5120,128;Inherit;False;Property;_LUTRange;LUT Range;6;0;Create;True;0;0;0;False;0;False;1;1.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;424;-5120,256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;426;-4864,128;Inherit;False;Property;_LUTOffset;LUT Offset;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;404;-5120,2560;Inherit;False;Property;_EroSSSmoothness;Ero SS Smoothness;10;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;423;-4864,256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;427;-4608,128;Inherit;False;Property;_LUTPan;LUT Pan;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;409;-5120,2432;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;425;-4608,256;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;410;-4864,2304;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;448;-3456,-1040;Inherit;False;Property;_SlimeColorSpecularSS;Slime Color Specular SS;35;0;Create;True;0;0;0;False;0;False;0.9;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;449;-3456,-912;Inherit;False;Property;_SlimeColorSpecularSSSmooth;Slime Color Specular SS Smooth;36;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;461;-4352,256;Inherit;True;Property;_LUT;LUT;5;0;Create;True;0;0;0;False;3;Space(13);Header(LUT Texture);Space(33);False;-1;fdfd935d00cf0964897b8e875b510f96;fdfd935d00cf0964897b8e875b510f96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;411;-4096,-1536;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;412;-2432,768;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;444;-3184,-992;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;450;-4224,-1280;Inherit;False;Property;_SlimeColorBase01;Slime Color Base 01;31;0;Create;True;0;0;0;False;3;Space(33);Header(Oil Color);Space(13);False;0.4171708,1,0,0;0.4156863,1,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;451;-4224,-1104;Inherit;False;Property;_SlimeColorBase02;Slime Color Base 02;32;0;Create;True;0;0;0;False;0;False;0.4402515,0.3744411,0,0;0.4392157,0.372549,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;455;-3968,256;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;413;-2176,640;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;443;-3216,-1200;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;453;-3712,-1280;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;414;-1792,640;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;442;-3456,-1536;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;447;-3200,-1792;Inherit;False;Property;_SlimeColorSpecular;Slime Color Specular;34;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;454;-2944,-1200;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;462;-6016,-512;Inherit;True;Property;_LiquidNormal;Liquid Normal;17;0;Create;True;0;0;0;False;3;Space(13);Header(Liquid Normal);Space(33);False;-1;aaf6d9d855e7074468950ee57023a62b;aaf6d9d855e7074468950ee57023a62b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;463;-6016,-256;Inherit;True;Property;_LiquidSecondaryNormal;Liquid Secondary Normal;18;0;Create;True;0;0;0;False;0;False;-1;aaf6d9d855e7074468950ee57023a62b;aaf6d9d855e7074468950ee57023a62b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;415;-1536,640;Inherit;False;op;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;421;-1792,128;Inherit;False;Property;_EmissiveMult;Emissive Mult;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;434;-6016,-896;Inherit;True;Constant;_Vector1;Vector 1;32;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;435;-5120,-256;Inherit;False;Property;_NormalIntensity;Normal Intensity;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;439;-5504,-512;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;445;-3024,-1552;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;418;-1536,-256;Inherit;False;alb;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;420;-1792,0;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;433;-5120,-512;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;456;-768,896;Inherit;False;415;op;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;457;-768,384;Inherit;False;Property;_Specular;Specular;1;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;231;464,-48;Inherit;False;1252;162.95;Ge Lush was here! <3;5;236;235;234;233;232;Ge Lush was here! <3;0.3782746,0.2798741,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;416;-768,0;Inherit;False;418;alb;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;417;-1536,-512;Inherit;False;norm;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;422;-1536,0;Inherit;False;em;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;458;-768,512;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;232;768,0;Inherit;False;Property;_Src;Src;37;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;233;1024,0;Inherit;False;Property;_Dst;Dst;38;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;234;1280,0;Inherit;False;Property;_ZWrite;ZWrite;39;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;1536,0;Inherit;False;Property;_ZTest;ZTest;40;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;512,0;Inherit;False;Property;_Cull;Cull;33;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;419;-560,-112;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;436;-11520,640;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;437;-11520,256;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;438;-11520,1152;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;441;-7424,1632;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;446;-768,128;Inherit;False;417;norm;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;452;-768,256;Inherit;False;422;em;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;459;-640,512;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;460;-512,640;Inherit;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;5;ASEMaterialInspector;0;0;StandardSpecular;Vefects/SH_Vefects_VFX_Slime_Lit_Liquid_Puddle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;True;_ZWrite;0;True;_ZTest;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;True;_Src;10;True;_Dst;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;2;-1;-1;-1;0;False;0;0;True;_Cull;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;359;0;357;1
WireConnection;359;1;357;3
WireConnection;360;0;359;0
WireConnection;360;1;358;0
WireConnection;363;0;360;0
WireConnection;363;2;361;0
WireConnection;365;0;364;0
WireConnection;365;1;362;0
WireConnection;367;1;363;0
WireConnection;368;0;365;0
WireConnection;368;1;366;0
WireConnection;369;0;367;0
WireConnection;379;3;369;0
WireConnection;380;0;359;0
WireConnection;380;1;370;0
WireConnection;381;0;359;0
WireConnection;381;1;371;0
WireConnection;385;0;368;0
WireConnection;386;1;372;0
WireConnection;386;6;373;0
WireConnection;386;7;374;0
WireConnection;387;1;372;0
WireConnection;387;6;375;0
WireConnection;387;7;376;0
WireConnection;390;0;377;0
WireConnection;390;1;379;0
WireConnection;390;2;378;0
WireConnection;391;0;385;0
WireConnection;392;0;380;0
WireConnection;392;2;383;0
WireConnection;393;0;381;0
WireConnection;393;2;382;0
WireConnection;394;0;386;0
WireConnection;394;1;384;0
WireConnection;396;0;387;0
WireConnection;397;0;388;0
WireConnection;397;1;389;3
WireConnection;398;0;392;0
WireConnection;398;1;390;0
WireConnection;399;0;393;0
WireConnection;399;1;390;0
WireConnection;400;0;391;0
WireConnection;401;0;394;0
WireConnection;401;1;395;0
WireConnection;402;0;397;0
WireConnection;402;1;396;0
WireConnection;403;0;402;0
WireConnection;403;1;400;0
WireConnection;405;0;401;0
WireConnection;406;1;398;0
WireConnection;407;1;399;0
WireConnection;408;0;406;0
WireConnection;408;1;407;0
WireConnection;408;2;405;0
WireConnection;440;0;403;0
WireConnection;431;0;408;0
WireConnection;431;1;403;0
WireConnection;431;2;440;0
WireConnection;432;0;431;0
WireConnection;428;0;408;0
WireConnection;428;1;432;0
WireConnection;428;2;430;0
WireConnection;424;0;428;0
WireConnection;424;1;429;0
WireConnection;423;0;424;0
WireConnection;423;1;426;0
WireConnection;409;0;403;0
WireConnection;409;1;404;0
WireConnection;425;0;423;0
WireConnection;425;2;427;0
WireConnection;410;0;408;0
WireConnection;410;1;403;0
WireConnection;410;2;409;0
WireConnection;461;1;425;0
WireConnection;412;0;410;0
WireConnection;444;0;448;0
WireConnection;444;1;449;0
WireConnection;455;0;461;0
WireConnection;413;0;411;4
WireConnection;413;1;412;0
WireConnection;443;0;461;0
WireConnection;443;1;448;0
WireConnection;443;2;444;0
WireConnection;453;0;450;0
WireConnection;453;1;451;0
WireConnection;453;2;455;0
WireConnection;414;0;413;0
WireConnection;442;0;411;0
WireConnection;442;1;453;0
WireConnection;454;0;443;0
WireConnection;462;1;398;0
WireConnection;463;1;399;0
WireConnection;415;0;414;0
WireConnection;439;0;462;0
WireConnection;439;1;463;0
WireConnection;439;2;405;0
WireConnection;445;0;442;0
WireConnection;445;1;447;0
WireConnection;445;2;454;0
WireConnection;418;0;445;0
WireConnection;420;0;445;0
WireConnection;420;1;421;0
WireConnection;433;0;434;0
WireConnection;433;1;439;0
WireConnection;433;2;435;0
WireConnection;417;0;433;0
WireConnection;422;0;420;0
WireConnection;458;0;457;0
WireConnection;458;1;456;0
WireConnection;419;0;456;0
WireConnection;419;1;416;0
WireConnection;441;0;386;0
WireConnection;459;0;458;0
WireConnection;0;0;419;0
WireConnection;0;1;446;0
WireConnection;0;2;452;0
WireConnection;0;3;459;0
WireConnection;0;4;460;0
WireConnection;0;9;456;0
ASEEND*/
//CHKSM=985B39B71A062FE9D50206681AF334E70DA6F1F0