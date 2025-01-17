// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_Slime_Lit_Liquid_Droplet"
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
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 2
		[Space(33)][Header(Slime Color)][Space(13)]_SlimeColorBase01("Slime Color Base 01", Color) = (0,0,0,0)
		_SlimeColorBase02("Slime Color Base 02", Color) = (0.1294118,0.1490196,0.2,0)
		_Src("Src", Float) = 5
		_SlimeColorSpecular("Slime Color Specular", Color) = (1,1,1,0)
		_Dst("Dst", Float) = 10
		_SlimeColorSpecularSS("Slime Color Specular SS", Float) = 0.9
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
		_SlimeColorSpecularSSSmooth("Slime Color Specular SS Smooth", Float) = 0
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
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
			float2 uv_texcoord;
			float4 uv2_texcoord2;
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
		uniform float4 _SlimeColorBase01;
		uniform float4 _SlimeColorBase02;
		uniform float4 _SlimeColorSpecular;
		uniform float _SlimeColorSpecularSS;
		uniform float _SlimeColorSpecularSSSmooth;
		uniform sampler2D _LUT;
		uniform float _LUTPan;
		uniform sampler2D _LiquidTexture;
		uniform sampler2D _LiquidSecondaryTexture;
		uniform float _Ero;
		uniform sampler2D _CutoutTexture;
		uniform float4 _CutoutTexture_ST;
		uniform float _CutoutPower;
		uniform float _CutoutMult;
		uniform float _LUTErosion;
		uniform float _LUTRange;
		uniform float _LUTOffset;
		uniform float _EmissiveMult;
		uniform float _Specular;
		uniform float _EroSSSmoothness;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 panner288 = ( 1.0 * _Time.y * _UVPan + ( i.uv_texcoord * _UVScale ));
			float randomOffset255 = i.uv2_texcoord2.z;
			float2 panner258 = ( 1.0 * _Time.y * _UVDPan + ( i.uv_texcoord * _UVDScale ));
			float2 lerpResult291 = lerp( float2( 0,0 ) , ( ( (tex2D( _MainTexture1, ( panner258 + randomOffset255 ) )).rg + -0.5 ) * 2.0 ) , _UVDLerp);
			float2 temp_output_299_0 = ( ( panner288 + randomOffset255 ) + lerpResult291 );
			float2 panner283 = ( 1.0 * _Time.y * _UVSecPan + ( i.uv_texcoord * _UVSecScale ));
			float2 temp_output_300_0 = ( ( panner283 + randomOffset255 ) + lerpResult291 );
			float clampResult9_g10 = clamp( ( ( length( (float2( -1,-1 ) + (i.uv_texcoord - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) + -_ToDropletsGradientSize ) * _ToDropletsGradientSharpness ) , 0.0 , 1.0 );
			float temp_output_306_0 = saturate( ( pow( clampResult9_g10 , _ToDropletsCutPower ) * _ToDropletsCutMultiply ) );
			float3 lerpResult329 = lerp( UnpackNormal( tex2D( _LiquidNormal, temp_output_299_0 ) ) , UnpackNormal( tex2D( _LiquidSecondaryNormal, temp_output_300_0 ) ) , temp_output_306_0);
			float3 lerpResult327 = lerp( float3(0,0,1) , lerpResult329 , _NormalIntensity);
			float3 norm337 = lerpResult327;
			o.Normal = norm337;
			float4 lerpResult344 = lerp( _SlimeColorBase01 , _SlimeColorBase02 , float4( 0,0,0,0 ));
			float4 temp_cast_0 = (_SlimeColorSpecularSS).xxxx;
			float4 temp_cast_1 = (( _SlimeColorSpecularSS + _SlimeColorSpecularSSSmooth )).xxxx;
			float2 temp_cast_2 = (_LUTPan).xx;
			float4 lerpResult309 = lerp( tex2D( _LiquidTexture, temp_output_299_0 ) , tex2D( _LiquidSecondaryTexture, temp_output_300_0 ) , temp_output_306_0);
			float clampResult9_g11 = clamp( ( ( length( (float2( -1,-1 ) + (i.uv_texcoord - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) + -0.0 ) * 1.0 ) , 0.0 , 1.0 );
			float2 uv_CutoutTexture = i.uv_texcoord * _CutoutTexture_ST.xy + _CutoutTexture_ST.zw;
			float4 temp_cast_3 = (_CutoutPower).xxxx;
			float4 temp_output_304_0 = ( ( _Ero * saturate( clampResult9_g11 ) ) + saturate( ( 1.0 - saturate( ( pow( tex2D( _CutoutTexture, uv_CutoutTexture ) , temp_cast_3 ) * _CutoutMult ) ) ) ) );
			float4 smoothstepResult325 = smoothstep( temp_output_304_0 , ( temp_output_304_0 + float4( 1,0,0,0 ) ) , lerpResult309);
			float4 lerpResult322 = lerp( lerpResult309 , saturate( smoothstepResult325 ) , _LUTErosion);
			float2 panner319 = ( 1.0 * _Time.y * temp_cast_2 + ( ( lerpResult322 * _LUTRange ) + _LUTOffset ).rg);
			float4 smoothstepResult345 = smoothstep( temp_cast_0 , temp_cast_1 , tex2D( _LUT, panner319 ));
			float4 lerpResult347 = lerp( ( i.vertexColor * lerpResult344 ) , _SlimeColorSpecular , smoothstepResult345);
			float4 alb334 = lerpResult347;
			o.Albedo = alb334.rgb;
			float4 em333 = ( lerpResult347 * _EmissiveMult );
			o.Emission = em333.rgb;
			float4 smoothstepResult311 = smoothstep( temp_output_304_0 , ( temp_output_304_0 + _EroSSSmoothness ) , lerpResult309);
			float4 op316 = saturate( ( i.vertexColor.a * saturate( smoothstepResult311 ) ) );
			o.Specular = ( _Specular * op316 ).rgb;
			o.Smoothness = _Smoothness;
			float4 temp_output_336_0 = op316;
			o.Alpha = temp_output_336_0.r;
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
				float2 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				float4 tSpace0 : TEXCOORD4;
				float4 tSpace1 : TEXCOORD5;
				float4 tSpace2 : TEXCOORD6;
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
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyzw = customInputData.uv2_texcoord2;
				o.customPack2.xyzw = v.texcoord1;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv2_texcoord2 = IN.customPack2.xyzw;
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
Node;AmplifyShaderEditor.CommentaryNode;250;-9136,-432;Inherit;False;662;252;Random Offset;2;255;253;Random Offset;0,0,0,1;0;0
Node;AmplifyShaderEditor.Vector2Node;251;-9856,768;Inherit;False;Property;_UVDScale;UVD Scale;21;0;Create;True;0;0;0;False;0;False;1,1;1.777,2.222;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;252;-10112,640;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;253;-9088,-384;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;254;-9856,640;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;255;-8704,-384;Inherit;False;randomOffset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;256;-9472,768;Inherit;False;Property;_UVDPan;UVD Pan;22;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;257;-9088,768;Inherit;False;255;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;258;-9472,640;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;259;-6528,2560;Inherit;False;Property;_CutoutPower;Cutout Power;25;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;260;-9088,640;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;261;-6912,2432;Inherit;True;Property;_CutoutTexture;Cutout Texture;24;0;Create;True;0;0;0;False;3;Space(33);Header(Cutout);Space(33);False;-1;bfcee05dba39b9c448796bf658e1d66a;bfcee05dba39b9c448796bf658e1d66a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;262;-6528,2432;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;263;-6272,2560;Inherit;False;Property;_CutoutMult;Cutout Mult;26;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;264;-9856,384;Inherit;False;Property;_UVScale;UV Scale;12;0;Create;True;0;0;0;False;0;False;1,1;0.3,0.3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;265;-10112,256;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;266;-10112,1152;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;267;-9856,1280;Inherit;False;Property;_UVSecScale;UV Sec Scale;15;0;Create;True;0;0;0;False;0;False;1,1;0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;268;-8576,640;Inherit;True;Property;_MainTexture1;UVD Tex;20;0;Create;True;0;0;0;False;3;Space(33);Header(Distortion Texture);Space(33);False;-1;None;b2c7718f1f3d50f47ba55e3ee15c5e1e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;269;-6272,2432;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;270;-8192,640;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;271;-9856,256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;272;-9856,1152;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;273;-9472,1280;Inherit;False;Property;_UVSecPan;UV Sec Pan;16;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;274;-9472,384;Inherit;False;Property;_UVPan;UV Pan;13;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;275;-6912,1536;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;276;-6272,2048;Inherit;False;Constant;_Float0;Float 0;41;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;277;-6272,2176;Inherit;False;Constant;_Float1;Float 1;41;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;278;-6272,1664;Inherit;False;Property;_ToDropletsGradientSize;To Droplets Gradient Size;29;0;Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;279;-6272,1792;Inherit;False;Property;_ToDropletsGradientSharpness;To Droplets Gradient Sharpness;30;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;280;-7680,512;Inherit;False;Constant;_Vector0;Vector 0;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;281;-7424,640;Inherit;False;Property;_UVDLerp;UVD Lerp;23;0;Create;True;0;0;0;False;0;False;0;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;282;-7936,640;Inherit;False;ConstantBiasScale;-1;;1;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT2;0,0;False;1;FLOAT;-0.5;False;2;FLOAT;2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;283;-9472,1152;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;284;-5632,1664;Inherit;False;Property;_ToDropletsCutPower;To Droplets Cut Power;27;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;285;-6016,2432;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;286;-9088,384;Inherit;False;255;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;287;-9088,1280;Inherit;False;255;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;288;-9472,256;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;289;-6016,1536;Inherit;False;RadialGradient;-1;;10;ec972f7745a8353409da2eb8d000a2e3;0;3;1;FLOAT2;0,0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;290;-6016,2048;Inherit;False;RadialGradient;-1;;11;ec972f7745a8353409da2eb8d000a2e3;0;3;1;FLOAT2;0,0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;291;-7424,512;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;292;-4864,2688;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;293;-5632,1536;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;-5376,1664;Inherit;False;Property;_ToDropletsCutMultiply;To Droplets Cut Multiply;28;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;295;-5120,2304;Inherit;False;Property;_Ero;Ero;4;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;296;-5504,2048;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;297;-9088,256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;298;-9088,1152;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;299;-7424,256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;300;-7424,1152;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;301;-4608,2688;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-5376,1536;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;303;-4816,2320;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;304;-4352,2304;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;306;-5120,1536;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;307;-7040,256;Inherit;True;Property;_LiquidTexture;Liquid Texture;11;0;Create;True;0;0;0;False;3;Space(13);Header(Liquid Texture);Space(33);False;-1;64f823e64f4977243bb7c015fa29d472;64f823e64f4977243bb7c015fa29d472;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;308;-7040,1152;Inherit;True;Property;_LiquidSecondaryTexture;Liquid Secondary Texture;14;0;Create;True;0;0;0;False;3;Space(13);Header(Liquid Secondary Texture);Space(33);False;-1;64f823e64f4977243bb7c015fa29d472;64f823e64f4977243bb7c015fa29d472;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;309;-4480,768;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;330;-3968,768;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;325;-3968,640;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;324;-4096,128;Inherit;False;Property;_LUTErosion;LUT Erosion;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;326;-3712,640;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;322;-4096,256;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;323;-3712,128;Inherit;False;Property;_LUTRange;LUT Range;6;0;Create;True;0;0;0;False;0;False;1;1.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;305;-3712,2560;Inherit;False;Property;_EroSSSmoothness;Ero SS Smoothness;10;0;Create;True;0;0;0;False;0;False;1;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;318;-3712,256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;320;-3456,128;Inherit;False;Property;_LUTOffset;LUT Offset;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;310;-3712,2432;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;317;-3456,256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;321;-3200,128;Inherit;False;Property;_LUTPan;LUT Pan;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;311;-3456,2304;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;319;-3200,256;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;349;-2304,-1040;Inherit;False;Property;_SlimeColorSpecularSS;Slime Color Specular SS;37;0;Create;True;0;0;0;False;0;False;0.9;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;350;-2304,-912;Inherit;False;Property;_SlimeColorSpecularSSSmooth;Slime Color Specular SS Smooth;40;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;352;-3072,-1104;Inherit;False;Property;_SlimeColorBase02;Slime Color Base 02;33;0;Create;True;0;0;0;False;0;False;0.1294118,0.1490196,0.2,0;0.4339623,0.3975906,0.1105375,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;353;-3072,-1280;Inherit;False;Property;_SlimeColorBase01;Slime Color Base 01;32;0;Create;True;0;0;0;False;3;Space(33);Header(Slime Color);Space(13);False;0,0,0,0;0.4330971,1,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;312;-2304,640;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;313;-2944,-1536;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;344;-2304,-1408;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;346;-2032,-992;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;356;-2944,256;Inherit;True;Property;_LUT;LUT;5;0;Create;True;0;0;0;False;3;Space(13);Header(LUT Texture);Space(33);False;-1;fdfd935d00cf0964897b8e875b510f96;fdfd935d00cf0964897b8e875b510f96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;314;-2048,512;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;343;-2304,-1536;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;345;-2064,-1200;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;348;-2048,-1792;Inherit;False;Property;_SlimeColorSpecular;Slime Color Specular;35;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;354;-4608,-256;Inherit;True;Property;_LiquidSecondaryNormal;Liquid Secondary Normal;18;0;Create;True;0;0;0;False;0;False;-1;aaf6d9d855e7074468950ee57023a62b;aaf6d9d855e7074468950ee57023a62b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;355;-4608,-512;Inherit;True;Property;_LiquidNormal;Liquid Normal;17;0;Create;True;0;0;0;False;3;Space(13);Header(Liquid Normal);Space(33);False;-1;aaf6d9d855e7074468950ee57023a62b;aaf6d9d855e7074468950ee57023a62b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;315;-1664,512;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;328;-4608,-896;Inherit;True;Constant;_Vector1;Vector 1;32;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;329;-4096,-512;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;332;-1664,0;Inherit;False;Property;_EmissiveMult;Emissive Mult;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;347;-1872,-1552;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;351;-3712,-256;Inherit;False;Property;_NormalIntensity;Normal Intensity;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;316;-1408,512;Inherit;False;op;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;327;-3712,-512;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;331;-1664,-128;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;231;464,-48;Inherit;False;1252;162.95;Ge Lush was here! <3;5;236;235;234;233;232;Ge Lush was here! <3;0.3782746,0.2798741,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;333;-1408,-128;Inherit;False;em;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;334;-1408,-256;Inherit;False;alb;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;336;-640,640;Inherit;False;316;op;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;337;-1408,-512;Inherit;False;norm;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;338;-896,256;Inherit;False;Property;_Specular;Specular;2;0;Create;True;0;0;0;False;0;False;0;0.003;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;232;768,0;Inherit;False;Property;_Src;Src;34;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;233;1024,0;Inherit;False;Property;_Dst;Dst;36;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;234;1280,0;Inherit;False;Property;_ZWrite;ZWrite;38;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;1536,0;Inherit;False;Property;_ZTest;ZTest;39;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;512,0;Inherit;False;Property;_Cull;Cull;31;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;335;-640,0;Inherit;False;334;alb;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;339;-896,384;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;340;-640,512;Inherit;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;341;-640,128;Inherit;False;337;norm;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;342;-640,256;Inherit;False;333;em;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;5;ASEMaterialInspector;0;0;StandardSpecular;Vefects/SH_Vefects_VFX_Slime_Lit_Liquid_Droplet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;True;_ZWrite;0;True;_ZTest;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;True;_Src;10;True;_Dst;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;True;_Cull;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;254;0;252;0
WireConnection;254;1;251;0
WireConnection;255;0;253;3
WireConnection;258;0;254;0
WireConnection;258;2;256;0
WireConnection;260;0;258;0
WireConnection;260;1;257;0
WireConnection;262;0;261;0
WireConnection;262;1;259;0
WireConnection;268;1;260;0
WireConnection;269;0;262;0
WireConnection;269;1;263;0
WireConnection;270;0;268;0
WireConnection;271;0;265;0
WireConnection;271;1;264;0
WireConnection;272;0;266;0
WireConnection;272;1;267;0
WireConnection;282;3;270;0
WireConnection;283;0;272;0
WireConnection;283;2;273;0
WireConnection;285;0;269;0
WireConnection;288;0;271;0
WireConnection;288;2;274;0
WireConnection;289;1;275;0
WireConnection;289;6;278;0
WireConnection;289;7;279;0
WireConnection;290;1;275;0
WireConnection;290;6;276;0
WireConnection;290;7;277;0
WireConnection;291;0;280;0
WireConnection;291;1;282;0
WireConnection;291;2;281;0
WireConnection;292;0;285;0
WireConnection;293;0;289;0
WireConnection;293;1;284;0
WireConnection;296;0;290;0
WireConnection;297;0;288;0
WireConnection;297;1;286;0
WireConnection;298;0;283;0
WireConnection;298;1;287;0
WireConnection;299;0;297;0
WireConnection;299;1;291;0
WireConnection;300;0;298;0
WireConnection;300;1;291;0
WireConnection;301;0;292;0
WireConnection;302;0;293;0
WireConnection;302;1;294;0
WireConnection;303;0;295;0
WireConnection;303;1;296;0
WireConnection;304;0;303;0
WireConnection;304;1;301;0
WireConnection;306;0;302;0
WireConnection;307;1;299;0
WireConnection;308;1;300;0
WireConnection;309;0;307;0
WireConnection;309;1;308;0
WireConnection;309;2;306;0
WireConnection;330;0;304;0
WireConnection;325;0;309;0
WireConnection;325;1;304;0
WireConnection;325;2;330;0
WireConnection;326;0;325;0
WireConnection;322;0;309;0
WireConnection;322;1;326;0
WireConnection;322;2;324;0
WireConnection;318;0;322;0
WireConnection;318;1;323;0
WireConnection;310;0;304;0
WireConnection;310;1;305;0
WireConnection;317;0;318;0
WireConnection;317;1;320;0
WireConnection;311;0;309;0
WireConnection;311;1;304;0
WireConnection;311;2;310;0
WireConnection;319;0;317;0
WireConnection;319;2;321;0
WireConnection;312;0;311;0
WireConnection;344;0;353;0
WireConnection;344;1;352;0
WireConnection;346;0;349;0
WireConnection;346;1;350;0
WireConnection;356;1;319;0
WireConnection;314;0;313;4
WireConnection;314;1;312;0
WireConnection;343;0;313;0
WireConnection;343;1;344;0
WireConnection;345;0;356;0
WireConnection;345;1;349;0
WireConnection;345;2;346;0
WireConnection;354;1;300;0
WireConnection;355;1;299;0
WireConnection;315;0;314;0
WireConnection;329;0;355;0
WireConnection;329;1;354;0
WireConnection;329;2;306;0
WireConnection;347;0;343;0
WireConnection;347;1;348;0
WireConnection;347;2;345;0
WireConnection;316;0;315;0
WireConnection;327;0;328;0
WireConnection;327;1;329;0
WireConnection;327;2;351;0
WireConnection;331;0;347;0
WireConnection;331;1;332;0
WireConnection;333;0;331;0
WireConnection;334;0;347;0
WireConnection;337;0;327;0
WireConnection;339;0;338;0
WireConnection;339;1;336;0
WireConnection;0;0;335;0
WireConnection;0;1;341;0
WireConnection;0;2;342;0
WireConnection;0;3;339;0
WireConnection;0;4;340;0
WireConnection;0;9;336;0
ASEEND*/
//CHKSM=86B6C69661303855C3C4E0A83E2379BB5E0A4F17