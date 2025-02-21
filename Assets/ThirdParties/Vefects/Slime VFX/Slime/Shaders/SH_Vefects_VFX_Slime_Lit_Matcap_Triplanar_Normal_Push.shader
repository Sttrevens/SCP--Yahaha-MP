// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_Slime_Lit_Matcap_Triplanar_Normal_Push"
{
	Properties
	{
		[Space(13)][Header(General)][Space(13)]_EmissionMultiply("Emission Multiply", Float) = 0
		_Spec("Spec", Float) = 0
		_Smooth("Smooth", Float) = 0
		_OverallTilesMultiply("Overall Tiles Multiply", Float) = 1
		[Space(33)][Header(Matcap)][Space(13)]_Matcap("Matcap", 2D) = "white" {}
		_MatcapDesaturate("Matcap Desaturate", Float) = 0
		_MatcapTint("Matcap Tint", Color) = (1,1,1,0)
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
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
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
			float4 lerpResult199 = lerp( float4( float3(0,0,0) , 0.0 ) , ( float4( ase_vertexNormal , 0.0 ) * saturate( ( ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_135_0).xy, 0, 0.0) ) * break145.z ) + ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_135_0).yz, 0, 0.0) ) * break145.x ) + ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_135_0).xz, 0, 0.0) ) * break145.y ) ) ) ) , _WPOIntensity);
			v.vertex.xyz += lerpResult199.rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 ase_worldPos = i.worldPos;
			float3 objToWorld191 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float overallTilesMult117 = _OverallTilesMultiply;
			float3 temp_output_179_0 = ( ( ( ( ase_worldPos - objToWorld191 ) * ( _NormalTile * overallTilesMult117 ) ) + _NormalTileOffsetCustom ) + ( _Time.y * _NormalTileSpeed ) );
			float3 temp_cast_0 = (_NormalTriplanarContrast).xxx;
			float3 temp_output_184_0 = pow( abs( ase_worldNormal ) , temp_cast_0 );
			float3 break187 = temp_output_184_0;
			float3 break189 = ( temp_output_184_0 / ( break187.x + break187.y + break187.z ) );
			float3 normalizeResult161 = normalize( ( ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_179_0).xy ) ) * break189.z ) + ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_179_0).yz ) ) * break189.x ) + ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_179_0).xz ) ) * break189.y ) ) );
			float3 break162 = normalizeResult161;
			float3 appendResult228 = (float3(( ase_vertexNormal.x + ( break162.x * _NormalIntensity ) ) , ( ase_vertexNormal.y + ( break162.y * _NormalIntensity ) ) , ( ase_vertexNormal.z + ( break162.z * _NormalIntensity ) )));
			float3 normalizeResult213 = ASESafeNormalize( appendResult228 );
			float3 desaturateInitialColor229 = tex2D( _Matcap, ((mul( UNITY_MATRIX_V, float4( normalizeResult213 , 0.0 ) ).xyz).xy*0.5 + 0.5) ).rgb;
			float desaturateDot229 = dot( desaturateInitialColor229, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar229 = lerp( desaturateInitialColor229, desaturateDot229.xxx, _MatcapDesaturate );
			float4 temp_output_231_0 = ( float4( desaturateVar229 , 0.0 ) * _MatcapTint );
			float3 hsvTorgb197 = RGBToHSV( temp_output_231_0.rgb );
			float temp_output_209_0 = ( _HueShift + ( _Time.y * _HueShiftSpeed ) );
			float3 hsvTorgb198 = HSVToRGB( float3(( hsvTorgb197.x + temp_output_209_0 ),hsvTorgb197.y,hsvTorgb197.z) );
			o.Albedo = hsvTorgb198;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV166 = dot( normalize( normalizeResult213 ), ase_worldViewDir );
			float fresnelNode166 = ( _FrBias + _FrScale * pow( max( 1.0 - fresnelNdotV166 , 0.0001 ), _FrPower ) );
			float3 hsvTorgb200 = RGBToHSV( ( temp_output_231_0 + ( fresnelNode166 * _FrColor ) ).rgb );
			float3 hsvTorgb201 = HSVToRGB( float3(( hsvTorgb200.x + temp_output_209_0 ),hsvTorgb200.y,hsvTorgb200.z) );
			o.Emission = ( hsvTorgb201 * _EmissionMultiply );
			float3 temp_cast_8 = (_Spec).xxx;
			o.Specular = temp_cast_8;
			o.Smoothness = _Smooth;
			o.Alpha = 1;
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
Node;AmplifyShaderEditor.CommentaryNode;115;-8112,-560;Inherit;False;676;162.95;Overall;2;117;116;Overall;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-8064,-512;Inherit;False;Property;_OverallTilesMultiply;Overall Tiles Multiply;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-7680,-512;Inherit;False;overallTilesMult;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;188;-5888,128;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;190;-8064,384;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;191;-8064,640;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;193;-8064,896;Inherit;False;Property;_NormalTile;Normal Tile;16;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;217;-7808,1024;Inherit;False;117;overallTilesMult;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;182;-5728,128;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;192;-7808,384;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;216;-7808,896;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-5632,256;Inherit;False;Property;_NormalTriplanarContrast;Normal Triplanar Contrast;15;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;180;-7040,896;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;184;-5632,128;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;-7552,384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;195;-7040,1024;Inherit;False;Property;_NormalTileSpeed;Normal Tile Speed;17;0;Create;True;0;0;0;False;0;False;0,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;215;-7552,896;Inherit;False;Property;_NormalTileOffsetCustom;Normal Tile Offset Custom;18;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;181;-6784,896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;187;-5376,256;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;196;-7296,384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;179;-6784,384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;186;-5248,256;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;176;-6528,384;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;177;-6528,512;Inherit;False;False;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;178;-6528,640;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;185;-5120,128;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;222;-6528,0;Inherit;True;Property;_NormalTriplanarTexture;Normal Triplanar Texture;13;0;Create;True;0;0;0;False;3;Space(33);Header(Normal Triplanar);Space(13);False;45f10a9d5101da849979325375a62116;45f10a9d5101da849979325375a62116;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;173;-6144,384;Inherit;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;174;-6144,640;Inherit;True;Property;_TextureSample1;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;175;-6144,896;Inherit;True;Property;_TextureSample2;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;189;-4736,128;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-4736,640;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-4736,384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-4736,896;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;-4480,384;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;161;-4224,384;Inherit;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;162;-3840,384;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;214;-4096,-896;Inherit;False;Property;_NormalIntensity;Normal Intensity;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;151;-3456,-384;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-3712,-896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;211;-3712,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;-3712,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;225;-3072,-896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;226;-3072,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;227;-3072,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;228;-2816,-896;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;118;-5760,1152;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;119;-8064,1408;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;120;-8064,1664;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;121;-8064,1920;Inherit;False;Property;_WPOTile;WPO Tile;22;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;122;-7808,2048;Inherit;False;117;overallTilesMult;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewMatrixNode;156;-2768,-128;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.NormalizeNode;213;-2688,-896;Inherit;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;123;-5600,1152;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-5504,1280;Inherit;False;Property;_WPOTriplanarContrast;WPO Triplanar Contrast;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;125;-7808,1408;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-7808,1920;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-2512,-128;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;127;-7040,1904;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;128;-5504,1152;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-7552,1408;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;130;-7040,1664;Inherit;False;Property;_WPOTileSpeed;WPO Tile Speed;23;0;Create;True;0;0;0;False;0;False;0,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;131;-7552,1920;Inherit;False;Property;_WPOTileOffsetCustom;WPO Tile Offset Custom;24;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;158;-2384,-128;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-6784,1904;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;133;-5248,1280;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-7296,1408;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;159;-2176,-128;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;135;-6784,1408;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;136;-5120,1280;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-2816,896;Inherit;False;Property;_FrPower;Fr Power;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-2816,768;Inherit;False;Property;_FrScale;Fr Scale;10;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-2816,640;Inherit;False;Property;_FrBias;Fr Bias;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;223;-2176,-384;Inherit;True;Property;_Matcap;Matcap;4;0;Create;True;0;0;0;False;3;Space(33);Header(Matcap);Space(13);False;-1;ff81ca010dd2fc2438848d4292297787;ff81ca010dd2fc2438848d4292297787;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;230;-1792,-256;Inherit;False;Property;_MatcapDesaturate;Matcap Desaturate;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;137;-6528,1408;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;138;-6528,1536;Inherit;False;False;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;139;-6528,1664;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;140;-4992,1152;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;141;-6528,1152;Inherit;True;Property;_WPOTriplanarTexture;WPO Triplanar Texture;19;0;Create;True;0;0;0;False;3;Space(33);Header(WPO Triplanar);Space(13);False;edba5d18294de3a4daddbf6d71ac0849;edba5d18294de3a4daddbf6d71ac0849;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FresnelNode;166;-2432,640;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;218;-2048,768;Inherit;False;Property;_FrColor;Fr Color;9;0;Create;True;0;0;0;False;3;Space(33);Header(Fresnel);Space(13);False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;229;-1792,-384;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;232;-1536,-256;Inherit;False;Property;_MatcapTint;Matcap Tint;6;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;142;-6144,1408;Inherit;True;Property;_TextureSample3;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;143;-6144,1664;Inherit;True;Property;_TextureSample4;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;144;-6144,1920;Inherit;True;Property;_TextureSample5;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;145;-4736,1152;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;167;-2048,640;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;205;-1536,512;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;207;-1280,640;Inherit;False;Property;_HueShiftSpeed;Hue Shift Speed;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;231;-1536,-384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-4736,1664;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-4736,1408;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-4736,1920;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;168;-1792,512;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;-1280,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;208;-1280,384;Inherit;False;Property;_HueShift;Hue Shift;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;149;-4480,1408;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RGBToHSVNode;200;-1536,1024;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;209;-1024,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;150;-4224,1408;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RGBToHSVNode;197;-1536,0;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;203;-1024,1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-640,1408;Inherit;False;Property;_WPOIntensity;WPO Intensity;20;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;153;-896,1280;Inherit;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;-2432,1408;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.HSVToRGBNode;201;-896,1024;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;204;-1024,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-640,1152;Inherit;False;Property;_EmissionMultiply;Emission Multiply;0;0;Create;True;0;0;0;False;3;Space(13);Header(General);Space(13);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;155;-2816,1408;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;160;-2768,0;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;198;-640,0;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;199;-640,1280;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;202;-640,1024;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;219;-640,384;Inherit;False;Property;_Smooth;Smooth;2;0;Create;True;0;0;0;False;0;False;0;0.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-640,256;Inherit;False;Property;_Spec;Spec;1;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;224;-3584,-1152;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;5;ASEMaterialInspector;0;0;StandardSpecular;Vefects/SH_Vefects_VFX_Slime_Lit_Matcap_Triplanar_Normal_Push;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;117;0;116;0
WireConnection;182;0;188;0
WireConnection;192;0;190;0
WireConnection;192;1;191;0
WireConnection;216;0;193;0
WireConnection;216;1;217;0
WireConnection;184;0;182;0
WireConnection;184;1;183;0
WireConnection;194;0;192;0
WireConnection;194;1;216;0
WireConnection;181;0;180;0
WireConnection;181;1;195;0
WireConnection;187;0;184;0
WireConnection;196;0;194;0
WireConnection;196;1;215;0
WireConnection;179;0;196;0
WireConnection;179;1;181;0
WireConnection;186;0;187;0
WireConnection;186;1;187;1
WireConnection;186;2;187;2
WireConnection;176;0;179;0
WireConnection;177;0;179;0
WireConnection;178;0;179;0
WireConnection;185;0;184;0
WireConnection;185;1;186;0
WireConnection;173;0;222;0
WireConnection;173;1;176;0
WireConnection;174;0;222;0
WireConnection;174;1;177;0
WireConnection;175;0;222;0
WireConnection;175;1;178;0
WireConnection;189;0;185;0
WireConnection;169;0;174;0
WireConnection;169;1;189;0
WireConnection;170;0;173;0
WireConnection;170;1;189;2
WireConnection;171;0;175;0
WireConnection;171;1;189;1
WireConnection;172;0;170;0
WireConnection;172;1;169;0
WireConnection;172;2;171;0
WireConnection;161;0;172;0
WireConnection;162;0;161;0
WireConnection;210;0;162;0
WireConnection;210;1;214;0
WireConnection;211;0;162;1
WireConnection;211;1;214;0
WireConnection;212;0;162;2
WireConnection;212;1;214;0
WireConnection;225;0;151;1
WireConnection;225;1;210;0
WireConnection;226;0;151;2
WireConnection;226;1;211;0
WireConnection;227;0;151;3
WireConnection;227;1;212;0
WireConnection;228;0;225;0
WireConnection;228;1;226;0
WireConnection;228;2;227;0
WireConnection;213;0;228;0
WireConnection;123;0;118;0
WireConnection;125;0;119;0
WireConnection;125;1;120;0
WireConnection;126;0;121;0
WireConnection;126;1;122;0
WireConnection;157;0;156;0
WireConnection;157;1;213;0
WireConnection;128;0;123;0
WireConnection;128;1;124;0
WireConnection;129;0;125;0
WireConnection;129;1;126;0
WireConnection;158;0;157;0
WireConnection;132;0;127;0
WireConnection;132;1;130;0
WireConnection;133;0;128;0
WireConnection;134;0;129;0
WireConnection;134;1;131;0
WireConnection;159;0;158;0
WireConnection;135;0;134;0
WireConnection;135;1;132;0
WireConnection;136;0;133;0
WireConnection;136;1;133;1
WireConnection;136;2;133;2
WireConnection;223;1;159;0
WireConnection;137;0;135;0
WireConnection;138;0;135;0
WireConnection;139;0;135;0
WireConnection;140;0;128;0
WireConnection;140;1;136;0
WireConnection;166;0;213;0
WireConnection;166;1;165;0
WireConnection;166;2;164;0
WireConnection;166;3;163;0
WireConnection;229;0;223;0
WireConnection;229;1;230;0
WireConnection;142;0;141;0
WireConnection;142;1;137;0
WireConnection;143;0;141;0
WireConnection;143;1;138;0
WireConnection;144;0;141;0
WireConnection;144;1;139;0
WireConnection;145;0;140;0
WireConnection;167;0;166;0
WireConnection;167;1;218;0
WireConnection;231;0;229;0
WireConnection;231;1;232;0
WireConnection;146;0;143;0
WireConnection;146;1;145;0
WireConnection;147;0;142;0
WireConnection;147;1;145;2
WireConnection;148;0;144;0
WireConnection;148;1;145;1
WireConnection;168;0;231;0
WireConnection;168;1;167;0
WireConnection;206;0;205;0
WireConnection;206;1;207;0
WireConnection;149;0;147;0
WireConnection;149;1;146;0
WireConnection;149;2;148;0
WireConnection;200;0;168;0
WireConnection;209;0;208;0
WireConnection;209;1;206;0
WireConnection;150;0;149;0
WireConnection;197;0;231;0
WireConnection;203;0;200;1
WireConnection;203;1;209;0
WireConnection;154;0;151;0
WireConnection;154;1;150;0
WireConnection;201;0;203;0
WireConnection;201;1;200;2
WireConnection;201;2;200;3
WireConnection;204;0;197;1
WireConnection;204;1;209;0
WireConnection;198;0;204;0
WireConnection;198;1;197;2
WireConnection;198;2;197;3
WireConnection;199;0;153;0
WireConnection;199;1;154;0
WireConnection;199;2;152;0
WireConnection;202;0;201;0
WireConnection;202;1;221;0
WireConnection;0;0;198;0
WireConnection;0;2;202;0
WireConnection;0;3;220;0
WireConnection;0;4;219;0
WireConnection;0;11;199;0
ASEEND*/
//CHKSM=6807DD0A59E23FE50D78679588DB140D538F802B