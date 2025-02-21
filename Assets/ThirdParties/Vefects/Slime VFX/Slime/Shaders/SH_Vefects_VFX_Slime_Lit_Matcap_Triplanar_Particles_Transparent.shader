// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_Slime_Lit_Matcap_Triplanar_Particles_Transparent"
{
	Properties
	{
		[Space(13)][Header(General)][Space(13)]_EmissionMultiply("Emission Multiply", Float) = 0
		_Spec("Spec", Float) = 0
		_Smooth("Smooth", Float) = 0
		_OverallTilesMultiply("Overall Tiles Multiply", Float) = 1
		[Space(33)][Header(Matcap)][Space(13)]_Matcap("Matcap", 2D) = "white" {}
		_MatcapTint("Matcap Tint", Color) = (1,1,1,0)
		_MatcapDesaturate("Matcap Desaturate", Float) = 0
		_HueShift("Hue Shift", Float) = 0
		_HueShiftSpeed("Hue Shift Speed", Float) = 0
		[Space(33)][Header(Fresnel)][Space(13)]_FrColor("Fr Color", Color) = (1,1,1,0)
		_FrScale("Fr Scale", Float) = 1
		_FrPower("Fr Power", Float) = 1
		_FrBias("Fr Bias", Float) = 0
		[Space(33)][Header(Normal Triplanar)][Space(13)]_NormalTriplanarTexture("Normal Triplanar Texture", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Float) = 0
		_NormalTriplanarContrast("Normal Triplanar Contrast", Float) = 1
		_NormalTile("Normal Tile", Vector) = (1,1,1,0)
		_NormalTileSpeed("Normal Tile Speed", Vector) = (0,1,1,0)
		_NormalTileOffsetCustom("Normal Tile Offset Custom", Vector) = (0,0,0,0)
		[Space(33)][Header(WPO Triplanar)][Space(13)]_WPOTriplanarTexture("WPO Triplanar Texture", 2D) = "white" {}
		_WPOIntensity("WPO Intensity", Float) = 0.1
		_WPOTriplanarContrast("WPO Triplanar Contrast", Float) = 1
		_WPOTile("WPO Tile", Vector) = (1,1,1,0)
		_WPOTileSpeed("WPO Tile Speed", Vector) = (0,1,1,0)
		_WPOTileOffsetCustom("WPO Tile Offset Custom", Vector) = (0,0,0,0)
		[Space(33)][Header(Opacity)][Space(13)]_OpacityOverall("Opacity Overall", Float) = 1
		_OpacityMin("Opacity Min", Float) = 0
		_OpacityMax("Opacity Max", Float) = 1
		_OpacityFrScale("Opacity Fr Scale", Float) = 1
		_OpacityFrPower("Opacity Fr Power", Float) = 1
		_OpacityFrBias("Opacity Fr Bias", Float) = 0
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 2
		_Src("Src", Float) = 5
		_Dst("Dst", Float) = 10
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
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
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _Src;
		uniform float _Dst;
		uniform float _ZWrite;
		uniform float _ZTest;
		uniform float _Cull;
		uniform sampler2D _WPOTriplanarTexture;
		uniform float3 _WPOTile;
		uniform float _OverallTilesMultiply;
		uniform float3 _WPOTileOffsetCustom;
		uniform float3 _WPOTileSpeed;
		uniform float _WPOTriplanarContrast;
		uniform float _WPOIntensity;
		uniform sampler2D _Matcap;
		uniform sampler2D _NormalTriplanarTexture;
		uniform float3 _NormalTile;
		uniform float3 _NormalTileOffsetCustom;
		uniform float3 _NormalTileSpeed;
		uniform float _NormalTriplanarContrast;
		uniform float _NormalIntensity;
		uniform float _MatcapDesaturate;
		uniform float4 _MatcapTint;
		uniform float _HueShift;
		uniform float _HueShiftSpeed;
		uniform float _FrBias;
		uniform float _FrScale;
		uniform float _FrPower;
		uniform float4 _FrColor;
		uniform float _EmissionMultiply;
		uniform float _Spec;
		uniform float _Smooth;
		uniform float _OpacityMin;
		uniform float _OpacityMax;
		uniform float _OpacityFrBias;
		uniform float _OpacityFrScale;
		uniform float _OpacityFrPower;
		uniform float _OpacityOverall;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		float3 ASESafeNormalize(float3 inVec)
		{
			float dp3 = max(1.175494351e-38, dot(inVec, inVec));
			return inVec* rsqrt(dp3);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 objToWorld120 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float overallTilesMult117 = _OverallTilesMultiply;
			float3 temp_output_135_0 = ( ( ( ( ase_worldPos - objToWorld120 ) * ( _WPOTile * overallTilesMult117 ) ) + _WPOTileOffsetCustom ) + ( _Time.y * _WPOTileSpeed ) );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 temp_cast_2 = (_WPOTriplanarContrast).xxx;
			float3 temp_output_128_0 = pow( abs( ase_worldNormal ) , temp_cast_2 );
			float3 break133 = temp_output_128_0;
			float3 break145 = ( temp_output_128_0 / ( break133.x + break133.y + break133.z ) );
			float4 lerpResult201 = lerp( float4( float3(0,0,0) , 0.0 ) , ( float4( ase_vertexNormal , 0.0 ) * saturate( ( ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_135_0).xy, 0, 0.0) ) * break145.z ) + ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_135_0).yz, 0, 0.0) ) * break145.x ) + ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_135_0).xz, 0, 0.0) ) * break145.y ) ) ) ) , ( _WPOIntensity * v.texcoord.z ));
			v.vertex.xyz += lerpResult201.rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			float3 objToWorld193 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float overallTilesMult117 = _OverallTilesMultiply;
			float3 temp_output_181_0 = ( ( ( ( ase_worldPos - objToWorld193 ) * ( _NormalTile * overallTilesMult117 ) ) + _NormalTileOffsetCustom ) + ( _Time.y * _NormalTileSpeed ) );
			float3 temp_cast_0 = (_NormalTriplanarContrast).xxx;
			float3 temp_output_186_0 = pow( abs( ase_worldNormal ) , temp_cast_0 );
			float3 break189 = temp_output_186_0;
			float3 break191 = ( temp_output_186_0 / ( break189.x + break189.y + break189.z ) );
			float3 normalizeResult163 = normalize( ( ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_181_0).xy ) ) * break191.z ) + ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_181_0).yz ) ) * break191.x ) + ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_181_0).xz ) ) * break191.y ) ) );
			float3 break164 = normalizeResult163;
			float3 appendResult219 = (float3(( ase_worldNormal.x + ( break164.x * _NormalIntensity ) ) , ( ase_worldNormal.y + ( break164.y * _NormalIntensity ) ) , ( ase_worldNormal.z + ( break164.z * _NormalIntensity ) )));
			float3 normalizeResult220 = ASESafeNormalize( appendResult219 );
			float3 desaturateInitialColor250 = tex2D( _Matcap, ((mul( UNITY_MATRIX_V, float4( normalizeResult220 , 0.0 ) ).xyz).xy*0.5 + 0.5) ).rgb;
			float desaturateDot250 = dot( desaturateInitialColor250, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar250 = lerp( desaturateInitialColor250, desaturateDot250.xxx, _MatcapDesaturate );
			float4 temp_output_252_0 = ( float4( desaturateVar250 , 0.0 ) * _MatcapTint );
			float3 hsvTorgb199 = RGBToHSV( temp_output_252_0.rgb );
			float temp_output_211_0 = ( _HueShift + ( _Time.y * _HueShiftSpeed ) );
			float3 hsvTorgb200 = HSVToRGB( float3(( hsvTorgb199.x + temp_output_211_0 ),hsvTorgb199.y,hsvTorgb199.z) );
			o.Albedo = hsvTorgb200;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV168 = dot( normalize( normalizeResult220 ), ase_worldViewDir );
			float fresnelNode168 = ( _FrBias + _FrScale * pow( max( 1.0 - fresnelNdotV168 , 0.0001 ), _FrPower ) );
			float3 hsvTorgb202 = RGBToHSV( ( temp_output_252_0 + ( fresnelNode168 * _FrColor ) ).rgb );
			float3 hsvTorgb203 = HSVToRGB( float3(( hsvTorgb202.x + temp_output_211_0 ),hsvTorgb202.y,hsvTorgb202.z) );
			o.Emission = ( hsvTorgb203 * _EmissionMultiply );
			float3 temp_cast_8 = (_Spec).xxx;
			o.Specular = temp_cast_8;
			o.Smoothness = _Smooth;
			float fresnelNdotV240 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode240 = ( _OpacityFrBias + _OpacityFrScale * pow( max( 1.0 - fresnelNdotV240 , 0.0001 ), _OpacityFrPower ) );
			float lerpResult243 = lerp( _OpacityMin , _OpacityMax , fresnelNode240);
			float op248 = saturate( ( saturate( lerpResult243 ) * _OpacityOverall ) );
			o.Alpha = op248;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
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
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
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
Node;AmplifyShaderEditor.CommentaryNode;115;-7984,-560;Inherit;False;676;162.95;Overall;2;117;116;Overall;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-7936,-512;Inherit;False;Property;_OverallTilesMultiply;Overall Tiles Multiply;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-7552,-512;Inherit;False;overallTilesMult;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;190;-5760,128;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;192;-7936,384;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;193;-7936,640;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;195;-7936,896;Inherit;False;Property;_NormalTile;Normal Tile;17;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;224;-7680,1024;Inherit;False;117;overallTilesMult;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;184;-5600,128;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;194;-7680,384;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;-7680,896;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;185;-5504,256;Inherit;False;Property;_NormalTriplanarContrast;Normal Triplanar Contrast;16;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;182;-6912,896;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;186;-5504,128;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;196;-7424,384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;197;-6912,1024;Inherit;False;Property;_NormalTileSpeed;Normal Tile Speed;18;0;Create;True;0;0;0;False;0;False;0,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;222;-7424,896;Inherit;False;Property;_NormalTileOffsetCustom;Normal Tile Offset Custom;19;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;-6656,896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;189;-5248,256;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;198;-7168,384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;181;-6656,384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;188;-5120,256;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;178;-6400,384;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;179;-6400,512;Inherit;False;False;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;180;-6400,640;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;187;-4992,128;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;229;-6400,0;Inherit;True;Property;_NormalTriplanarTexture;Normal Triplanar Texture;14;0;Create;True;0;0;0;False;3;Space(33);Header(Normal Triplanar);Space(13);False;45f10a9d5101da849979325375a62116;45f10a9d5101da849979325375a62116;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;175;-6016,384;Inherit;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;176;-6016,640;Inherit;True;Property;_TextureSample1;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;177;-6016,896;Inherit;True;Property;_TextureSample2;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;191;-4608,128;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-4608,640;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;-4608,384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-4608,896;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;174;-4352,384;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;163;-4096,384;Inherit;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;164;-3712,384;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;221;-3968,-896;Inherit;False;Property;_NormalIntensity;Normal Intensity;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;-3584,-896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;-3584,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;214;-3584,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;215;-3328,-1152;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;216;-3072,-896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;217;-3072,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;218;-3072,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;219;-2816,-896;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;118;-5632,1152;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;119;-7936,1408;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;120;-7936,1664;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;121;-7936,1920;Inherit;False;Property;_WPOTile;WPO Tile;23;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;122;-7680,2048;Inherit;False;117;overallTilesMult;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewMatrixNode;158;-2640,-128;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.NormalizeNode;220;-2560,-896;Inherit;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;123;-5472,1152;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-5376,1280;Inherit;False;Property;_WPOTriplanarContrast;WPO Triplanar Contrast;22;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;125;-7680,1408;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-7680,1920;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;-2384,-128;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;127;-6912,1904;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;128;-5376,1152;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-7424,1408;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;130;-6912,1664;Inherit;False;Property;_WPOTileSpeed;WPO Tile Speed;24;0;Create;True;0;0;0;False;0;False;0,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;131;-7424,1920;Inherit;False;Property;_WPOTileOffsetCustom;WPO Tile Offset Custom;25;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;160;-2256,-128;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-6656,1904;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;133;-5120,1280;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-7168,1408;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;161;-2048,-128;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;135;-6656,1408;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;136;-4992,1280;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-2688,896;Inherit;False;Property;_FrPower;Fr Power;12;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;166;-2688,768;Inherit;False;Property;_FrScale;Fr Scale;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-2688,640;Inherit;False;Property;_FrBias;Fr Bias;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;237;-2304,2688;Inherit;False;Property;_OpacityFrBias;Opacity Fr Bias;31;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;238;-2304,2944;Inherit;False;Property;_OpacityFrPower;Opacity Fr Power;30;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;239;-2304,2816;Inherit;False;Property;_OpacityFrScale;Opacity Fr Scale;29;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;230;-2048,-512;Inherit;True;Property;_Matcap;Matcap;5;0;Create;True;0;0;0;False;3;Space(33);Header(Matcap);Space(13);False;-1;ff81ca010dd2fc2438848d4292297787;ff81ca010dd2fc2438848d4292297787;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;251;-1664,-384;Inherit;False;Property;_MatcapDesaturate;Matcap Desaturate;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;137;-6400,1408;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;138;-6400,1536;Inherit;False;False;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;139;-6400,1664;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;140;-4864,1152;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;141;-6400,1152;Inherit;True;Property;_WPOTriplanarTexture;WPO Triplanar Texture;20;0;Create;True;0;0;0;False;3;Space(33);Header(WPO Triplanar);Space(13);False;edba5d18294de3a4daddbf6d71ac0849;edba5d18294de3a4daddbf6d71ac0849;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FresnelNode;168;-2304,640;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;225;-1920,768;Inherit;False;Property;_FrColor;Fr Color;10;0;Create;True;0;0;0;False;3;Space(33);Header(Fresnel);Space(13);False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;240;-1920,2688;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;241;-1920,2432;Inherit;False;Property;_OpacityMin;Opacity Min;27;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-1920,2560;Inherit;False;Property;_OpacityMax;Opacity Max;28;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;250;-1664,-512;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;253;-1408,-384;Inherit;False;Property;_MatcapTint;Matcap Tint;6;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;142;-6016,1408;Inherit;True;Property;_TextureSample3;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;143;-6016,1664;Inherit;True;Property;_TextureSample4;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;144;-6016,1920;Inherit;True;Property;_TextureSample5;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;145;-4608,1152;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-1920,640;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;207;-1408,512;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;209;-1152,640;Inherit;False;Property;_HueShiftSpeed;Hue Shift Speed;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;243;-1536,2432;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;-1408,-512;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-4608,1664;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-4608,1408;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-4608,1920;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;170;-1664,512;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-1152,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;210;-1152,384;Inherit;False;Property;_HueShift;Hue Shift;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;244;-1280,2432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;245;-1024,2560;Inherit;False;Property;_OpacityOverall;Opacity Overall;26;0;Create;True;0;0;0;False;3;Space(33);Header(Opacity);Space(13);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;149;-4352,1408;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RGBToHSVNode;202;-1408,1024;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;211;-896,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;-896,2432;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;150;-4096,1408;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;151;-2528,1744;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;152;-768,1536;Inherit;False;Property;_WPOIntensity;WPO Intensity;21;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;153;-768,1664;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RGBToHSVNode;199;-1408,0;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;205;-896,1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;247;-640,2432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;154;-768,1280;Inherit;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-2128,1696;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;156;-512,1536;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;203;-768,1024;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;206;-896,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;228;-512,1152;Inherit;False;Property;_EmissionMultiply;Emission Multiply;1;0;Create;True;0;0;0;False;3;Space(13);Header(General);Space(13);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;231;464,-48;Inherit;False;1252;162.95;Ge Lush was here! <3;5;236;235;234;233;232;Ge Lush was here! <3;0.3782746,0.2798741,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;248;-384,2432;Inherit;False;op;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;157;-2528,1904;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;162;-2640,0;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;200;-512,0;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;201;-512,1280;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;204;-512,1024;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;226;-512,384;Inherit;False;Property;_Smooth;Smooth;3;0;Create;True;0;0;0;False;0;False;0;0.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;227;-512,256;Inherit;False;Property;_Spec;Spec;2;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;232;768,0;Inherit;False;Property;_Src;Src;33;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;233;1024,0;Inherit;False;Property;_Dst;Dst;34;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;234;1280,0;Inherit;False;Property;_ZWrite;ZWrite;35;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;1536,0;Inherit;False;Property;_ZTest;ZTest;36;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;512,0;Inherit;False;Property;_Cull;Cull;32;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;249;-512,512;Inherit;False;248;op;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;5;ASEMaterialInspector;0;0;StandardSpecular;Vefects/SH_Vefects_VFX_Slime_Lit_Matcap_Triplanar_Particles_Transparent;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;True;_ZWrite;0;True;_ZTest;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;True;_Src;10;True;_Dst;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;True;_Cull;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;117;0;116;0
WireConnection;184;0;190;0
WireConnection;194;0;192;0
WireConnection;194;1;193;0
WireConnection;223;0;195;0
WireConnection;223;1;224;0
WireConnection;186;0;184;0
WireConnection;186;1;185;0
WireConnection;196;0;194;0
WireConnection;196;1;223;0
WireConnection;183;0;182;0
WireConnection;183;1;197;0
WireConnection;189;0;186;0
WireConnection;198;0;196;0
WireConnection;198;1;222;0
WireConnection;181;0;198;0
WireConnection;181;1;183;0
WireConnection;188;0;189;0
WireConnection;188;1;189;1
WireConnection;188;2;189;2
WireConnection;178;0;181;0
WireConnection;179;0;181;0
WireConnection;180;0;181;0
WireConnection;187;0;186;0
WireConnection;187;1;188;0
WireConnection;175;0;229;0
WireConnection;175;1;178;0
WireConnection;176;0;229;0
WireConnection;176;1;179;0
WireConnection;177;0;229;0
WireConnection;177;1;180;0
WireConnection;191;0;187;0
WireConnection;171;0;176;0
WireConnection;171;1;191;0
WireConnection;172;0;175;0
WireConnection;172;1;191;2
WireConnection;173;0;177;0
WireConnection;173;1;191;1
WireConnection;174;0;172;0
WireConnection;174;1;171;0
WireConnection;174;2;173;0
WireConnection;163;0;174;0
WireConnection;164;0;163;0
WireConnection;212;0;164;0
WireConnection;212;1;221;0
WireConnection;213;0;164;1
WireConnection;213;1;221;0
WireConnection;214;0;164;2
WireConnection;214;1;221;0
WireConnection;216;0;215;1
WireConnection;216;1;212;0
WireConnection;217;0;215;2
WireConnection;217;1;213;0
WireConnection;218;0;215;3
WireConnection;218;1;214;0
WireConnection;219;0;216;0
WireConnection;219;1;217;0
WireConnection;219;2;218;0
WireConnection;220;0;219;0
WireConnection;123;0;118;0
WireConnection;125;0;119;0
WireConnection;125;1;120;0
WireConnection;126;0;121;0
WireConnection;126;1;122;0
WireConnection;159;0;158;0
WireConnection;159;1;220;0
WireConnection;128;0;123;0
WireConnection;128;1;124;0
WireConnection;129;0;125;0
WireConnection;129;1;126;0
WireConnection;160;0;159;0
WireConnection;132;0;127;0
WireConnection;132;1;130;0
WireConnection;133;0;128;0
WireConnection;134;0;129;0
WireConnection;134;1;131;0
WireConnection;161;0;160;0
WireConnection;135;0;134;0
WireConnection;135;1;132;0
WireConnection;136;0;133;0
WireConnection;136;1;133;1
WireConnection;136;2;133;2
WireConnection;230;1;161;0
WireConnection;137;0;135;0
WireConnection;138;0;135;0
WireConnection;139;0;135;0
WireConnection;140;0;128;0
WireConnection;140;1;136;0
WireConnection;168;0;220;0
WireConnection;168;1;167;0
WireConnection;168;2;166;0
WireConnection;168;3;165;0
WireConnection;240;1;237;0
WireConnection;240;2;239;0
WireConnection;240;3;238;0
WireConnection;250;0;230;0
WireConnection;250;1;251;0
WireConnection;142;0;141;0
WireConnection;142;1;137;0
WireConnection;143;0;141;0
WireConnection;143;1;138;0
WireConnection;144;0;141;0
WireConnection;144;1;139;0
WireConnection;145;0;140;0
WireConnection;169;0;168;0
WireConnection;169;1;225;0
WireConnection;243;0;241;0
WireConnection;243;1;242;0
WireConnection;243;2;240;0
WireConnection;252;0;250;0
WireConnection;252;1;253;0
WireConnection;146;0;143;0
WireConnection;146;1;145;0
WireConnection;147;0;142;0
WireConnection;147;1;145;2
WireConnection;148;0;144;0
WireConnection;148;1;145;1
WireConnection;170;0;252;0
WireConnection;170;1;169;0
WireConnection;208;0;207;0
WireConnection;208;1;209;0
WireConnection;244;0;243;0
WireConnection;149;0;147;0
WireConnection;149;1;146;0
WireConnection;149;2;148;0
WireConnection;202;0;170;0
WireConnection;211;0;210;0
WireConnection;211;1;208;0
WireConnection;246;0;244;0
WireConnection;246;1;245;0
WireConnection;150;0;149;0
WireConnection;199;0;252;0
WireConnection;205;0;202;1
WireConnection;205;1;211;0
WireConnection;247;0;246;0
WireConnection;155;0;151;0
WireConnection;155;1;150;0
WireConnection;156;0;152;0
WireConnection;156;1;153;3
WireConnection;203;0;205;0
WireConnection;203;1;202;2
WireConnection;203;2;202;3
WireConnection;206;0;199;1
WireConnection;206;1;211;0
WireConnection;248;0;247;0
WireConnection;200;0;206;0
WireConnection;200;1;199;2
WireConnection;200;2;199;3
WireConnection;201;0;154;0
WireConnection;201;1;155;0
WireConnection;201;2;156;0
WireConnection;204;0;203;0
WireConnection;204;1;228;0
WireConnection;0;0;200;0
WireConnection;0;2;204;0
WireConnection;0;3;227;0
WireConnection;0;4;226;0
WireConnection;0;9;249;0
WireConnection;0;11;201;0
ASEEND*/
//CHKSM=374EA3997179ED6C333A28F0864A67D174E5CF84