// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_Slime_Lit_Matcap_Triplanar_Top"
{
	Properties
	{
		[Space(33)][Header(Blend)][Space(13)][Toggle]_WorldorVertex("World or Vertex", Float) = 0
		_TopFalloffErosion("Top Falloff Erosion", Float) = 0
		_TopFalloffSmoothness("Top Falloff Smoothness", Float) = 1
		_TopOffset("Top Offset", Float) = 0
		[Space(13)][Header(General)][Space(13)]_EmissionMultiply("Emission Multiply", Float) = 0
		_Spec("Spec", Float) = 0
		_Smooth("Smooth", Float) = 0
		_OverallTilesMultiply("Overall Tiles Multiply", Float) = 1
		[Space(33)][Header(Base Texture)][Space(13)]_BaseTexture("Base Texture", 2D) = "white" {}
		[Space(33)][Header(Base Normal Texture)][Space(13)]_BaseNormalTexture("Base Normal Texture", 2D) = "bump" {}
		_BaseNormalIntensity("Base Normal Intensity", Float) = 1
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
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
			float2 uv_texcoord;
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
		uniform float _TopFalloffErosion;
		uniform float _TopFalloffSmoothness;
		uniform float _WorldorVertex;
		uniform float _TopOffset;
		uniform sampler2D _BaseNormalTexture;
		uniform float4 _BaseNormalTexture_ST;
		uniform float _BaseNormalIntensity;
		uniform sampler2D _BaseTexture;
		uniform float4 _BaseTexture_ST;
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
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 objToWorld120 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float overallTilesMult117 = _OverallTilesMultiply;
			float3 temp_output_142_0 = ( ( ( ( ase_worldPos - objToWorld120 ) * ( _WPOTile * overallTilesMult117 ) ) + _WPOTileOffsetCustom ) + ( _Time.y * _WPOTileSpeed ) );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 temp_cast_2 = (_WPOTriplanarContrast).xxx;
			float3 temp_output_131_0 = pow( abs( ase_worldNormal ) , temp_cast_2 );
			float3 break138 = temp_output_131_0;
			float3 break157 = ( temp_output_131_0 / ( break138.x + break138.y + break138.z ) );
			float3 objToWorld124 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float temp_output_147_0 = saturate( ( (( _WorldorVertex )?( ase_vertex3Pos.y ):( (( ase_worldPos - objToWorld124 )).y )) + _TopOffset ) );
			float smoothstepResult158 = smoothstep( _TopFalloffErosion , ( _TopFalloffErosion + _TopFalloffSmoothness ) , temp_output_147_0);
			float top164 = saturate( smoothstepResult158 );
			float lerpResult171 = lerp( 0.0 , _WPOIntensity , top164);
			float4 lerpResult249 = lerp( float4( float3(0,0,0) , 0.0 ) , ( float4( ase_vertex3Pos , 0.0 ) * saturate( ( ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_142_0).xy, 0, 0.0) ) * break157.z ) + ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_142_0).yz, 0, 0.0) ) * break157.x ) + ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_142_0).xz, 0, 0.0) ) * break157.y ) ) ) ) , lerpResult171);
			v.vertex.xyz += lerpResult249.rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 _Vector1 = float3(0,0,1);
			float2 uv_BaseNormalTexture = i.uv_texcoord * _BaseNormalTexture_ST.xy + _BaseNormalTexture_ST.zw;
			float3 lerpResult244 = lerp( _Vector1 , UnpackNormal( tex2D( _BaseNormalTexture, uv_BaseNormalTexture ) ) , _BaseNormalIntensity);
			float3 ase_worldPos = i.worldPos;
			float3 objToWorld124 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_147_0 = saturate( ( (( _WorldorVertex )?( ase_vertex3Pos.y ):( (( ase_worldPos - objToWorld124 )).y )) + _TopOffset ) );
			float smoothstepResult158 = smoothstep( _TopFalloffErosion , ( _TopFalloffErosion + _TopFalloffSmoothness ) , temp_output_147_0);
			float top164 = saturate( smoothstepResult158 );
			float3 lerpResult246 = lerp( _Vector1 , lerpResult244 , top164);
			o.Normal = lerpResult246;
			float2 uv_BaseTexture = i.uv_texcoord * _BaseTexture_ST.xy + _BaseTexture_ST.zw;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 objToWorld258 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float overallTilesMult117 = _OverallTilesMultiply;
			float3 temp_output_197_0 = ( ( ( ( ase_worldPos - objToWorld258 ) * ( _NormalTile * overallTilesMult117 ) ) + _NormalTileOffsetCustom ) + ( _Time.y * _NormalTileSpeed ) );
			float3 temp_cast_0 = (_NormalTriplanarContrast).xxx;
			float3 temp_output_202_0 = pow( abs( ase_worldNormal ) , temp_cast_0 );
			float3 break205 = temp_output_202_0;
			float3 break207 = ( temp_output_202_0 / ( break205.x + break205.y + break205.z ) );
			float3 normalizeResult180 = normalize( ( ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_197_0).xy ) ) * break207.z ) + ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_197_0).yz ) ) * break207.x ) + ( UnpackNormal( tex2D( _NormalTriplanarTexture, (temp_output_197_0).xz ) ) * break207.y ) ) );
			float3 break181 = normalizeResult180;
			float3 appendResult223 = (float3(( ase_worldNormal.x + ( break181.x * _NormalIntensity ) ) , ( ase_worldNormal.y + ( break181.y * _NormalIntensity ) ) , ( ase_worldNormal.z + ( break181.z * _NormalIntensity ) )));
			float3 normalizeResult224 = ASESafeNormalize( appendResult223 );
			float3 desaturateInitialColor262 = tex2D( _Matcap, ((mul( UNITY_MATRIX_V, float4( normalizeResult224 , 0.0 ) ).xyz).xy*0.5 + 0.5) ).rgb;
			float desaturateDot262 = dot( desaturateInitialColor262, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar262 = lerp( desaturateInitialColor262, desaturateDot262.xxx, _MatcapDesaturate );
			float4 temp_output_264_0 = ( float4( desaturateVar262 , 0.0 ) * _MatcapTint );
			float3 hsvTorgb233 = RGBToHSV( temp_output_264_0.rgb );
			float temp_output_216_0 = ( _HueShift + ( _Time.y * _HueShiftSpeed ) );
			float3 hsvTorgb235 = HSVToRGB( float3(( hsvTorgb233.x + temp_output_216_0 ),hsvTorgb233.y,hsvTorgb233.z) );
			float4 lerpResult248 = lerp( tex2D( _BaseTexture, uv_BaseTexture ) , float4( hsvTorgb235 , 0.0 ) , top164);
			o.Albedo = lerpResult248.rgb;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV185 = dot( normalize( normalizeResult224 ), ase_worldViewDir );
			float fresnelNode185 = ( _FrBias + _FrScale * pow( max( 1.0 - fresnelNdotV185 , 0.0001 ), _FrPower ) );
			float3 hsvTorgb250 = RGBToHSV( ( temp_output_264_0 + ( fresnelNode185 * _FrColor ) ).rgb );
			float3 hsvTorgb251 = HSVToRGB( float3(( hsvTorgb250.x + temp_output_216_0 ),hsvTorgb250.y,hsvTorgb250.z) );
			float lerpResult254 = lerp( 0.0 , _EmissionMultiply , top164);
			o.Emission = ( hsvTorgb251 * lerpResult254 );
			float3 temp_cast_10 = (_Spec).xxx;
			o.Specular = temp_cast_10;
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
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
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
Node;AmplifyShaderEditor.CommentaryNode;115;-8368,-688;Inherit;False;676;162.95;Overall;2;117;116;Overall;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-8320,-640;Inherit;False;Property;_OverallTilesMultiply;Overall Tiles Multiply;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-7936,-640;Inherit;False;overallTilesMult;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;206;-6144,0;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;208;-8320,768;Inherit;False;Property;_NormalTile;Normal Tile;25;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;227;-8064,896;Inherit;False;117;overallTilesMult;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;257;-8320,256;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;258;-8320,512;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.AbsOpNode;200;-5984,0;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-8064,768;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;259;-8064,256;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;201;-5888,128;Inherit;False;Property;_NormalTriplanarContrast;Normal Triplanar Contrast;24;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;198;-7296,768;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;202;-5888,0;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-7808,256;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;210;-7296,896;Inherit;False;Property;_NormalTileSpeed;Normal Tile Speed;26;0;Create;True;0;0;0;False;0;False;0,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;225;-7808,768;Inherit;False;Property;_NormalTileOffsetCustom;Normal Tile Offset Custom;27;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;-7040,768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;205;-5632,128;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;211;-7552,256;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;197;-7040,256;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;204;-5504,128;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;195;-6784,384;Inherit;False;False;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;196;-6784,512;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;203;-5376,0;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;231;-6784,-128;Inherit;True;Property;_NormalTriplanarTexture;Normal Triplanar Texture;22;0;Create;True;0;0;0;False;3;Space(33);Header(Normal Triplanar);Space(13);False;45f10a9d5101da849979325375a62116;45f10a9d5101da849979325375a62116;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ComponentMaskNode;232;-6784,256;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;192;-6400,256;Inherit;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;193;-6400,512;Inherit;True;Property;_TextureSample1;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;194;-6400,768;Inherit;True;Property;_TextureSample2;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;207;-4992,0;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-4992,512;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-4992,256;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;190;-4992,768;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;191;-4736,256;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;180;-4480,256;Inherit;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;181;-4096,256;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;241;-4352,-1024;Inherit;False;Property;_NormalIntensity;Normal Intensity;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;218;-3968,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;219;-3712,-1280;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-3968,-1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-3968,-896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;220;-3456,-1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;221;-3456,-896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;222;-3456,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;223;-3200,-1024;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;118;-6016,1024;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;119;-8320,1280;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;120;-8320,1536;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;121;-8320,1792;Inherit;False;Property;_WPOTile;WPO Tile;31;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;122;-8064,1920;Inherit;False;117;overallTilesMult;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;123;-3584,2048;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;124;-3584,2304;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewMatrixNode;175;-3024,-256;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.NormalizeNode;224;-2944,-1024;Inherit;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;125;-5856,1024;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-5760,1152;Inherit;False;Property;_WPOTriplanarContrast;WPO Triplanar Contrast;30;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;127;-8064,1280;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;-8064,1792;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;129;-3328,2048;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-2768,-256;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;130;-7296,1776;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;131;-5760,1024;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-7808,1280;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;133;-7296,1536;Inherit;False;Property;_WPOTileSpeed;WPO Tile Speed;32;0;Create;True;0;0;0;False;0;False;0,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;134;-7808,1792;Inherit;False;Property;_WPOTileOffsetCustom;WPO Tile Offset Custom;33;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;135;-3584,2560;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;136;-3072,2048;Inherit;False;False;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;177;-2640,-256;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-7040,1776;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;138;-5504,1152;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;139;-7552,1280;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;140;-3072,2560;Inherit;False;Property;_WorldorVertex;World or Vertex;0;0;Create;True;0;0;0;False;3;Space(33);Header(Blend);Space(13);False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-2688,2688;Inherit;False;Property;_TopOffset;Top Offset;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;178;-2432,-256;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;-7040,1280;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;143;-5376,1152;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-2688,2560;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;-2048,2304;Inherit;False;Property;_TopFalloffSmoothness;Top Falloff Smoothness;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-2048,2176;Inherit;False;Property;_TopFalloffErosion;Top Falloff Erosion;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;182;-3072,768;Inherit;False;Property;_FrPower;Fr Power;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-3072,640;Inherit;False;Property;_FrScale;Fr Scale;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;-3072,512;Inherit;False;Property;_FrBias;Fr Bias;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;236;-2432,-512;Inherit;True;Property;_Matcap;Matcap;13;0;Create;True;0;0;0;False;3;Space(33);Header(Matcap);Space(13);False;-1;ff81ca010dd2fc2438848d4292297787;ff81ca010dd2fc2438848d4292297787;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;263;-2048,-384;Inherit;False;Property;_MatcapDesaturate;Matcap Desaturate;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;147;-2304,2560;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;148;-6784,1280;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;149;-6784,1408;Inherit;False;False;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;150;-6784,1536;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;151;-5248,1024;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;152;-6784,1024;Inherit;True;Property;_WPOTriplanarTexture;WPO Triplanar Texture;28;0;Create;True;0;0;0;False;3;Space(33);Header(WPO Triplanar);Space(13);False;edba5d18294de3a4daddbf6d71ac0849;edba5d18294de3a4daddbf6d71ac0849;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;153;-1776,2192;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;185;-2688,512;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;228;-2304,640;Inherit;False;Property;_FrColor;Fr Color;18;0;Create;True;0;0;0;False;3;Space(33);Header(Fresnel);Space(13);False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;262;-2048,-512;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;265;-1792,-384;Inherit;False;Property;_MatcapTint;Matcap Tint;15;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;154;-6400,1280;Inherit;True;Property;_TextureSample3;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;155;-6400,1536;Inherit;True;Property;_TextureSample4;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;156;-6400,1792;Inherit;True;Property;_TextureSample5;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;157;-4992,1024;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SmoothstepOpNode;158;-2048,1920;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-2304,512;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;212;-1792,384;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;214;-1536,512;Inherit;False;Property;_HueShiftSpeed;Hue Shift Speed;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;264;-1792,-512;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;-4992,1536;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;-4992,1280;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;-4992,1792;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;162;-1280,2560;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;187;-2048,384;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;-1536,384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;215;-1536,256;Inherit;False;Property;_HueShift;Hue Shift;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;163;-4736,1280;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;164;-1024,2560;Inherit;False;top;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;216;-1280,384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;233;-1792,-128;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RGBToHSVNode;250;-1792,640;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;165;-4480,1280;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;166;-3072,1280;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;167;-1024,1536;Inherit;False;Property;_WPOIntensity;WPO Intensity;29;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;168;-1024,1664;Inherit;False;164;top;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;234;-1536,-128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;238;-1792,-1024;Inherit;True;Property;_BaseNormalTexture;Base Normal Texture;11;0;Create;True;0;0;0;False;3;Space(33);Header(Base Normal Texture);Space(13);False;-1;0535bac879795224ba8ccb7700208859;0535bac879795224ba8ccb7700208859;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;242;-1664,-768;Inherit;False;Constant;_Vector1;Vector 1;27;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;243;-1280,-768;Inherit;False;Property;_BaseNormalIntensity;Base Normal Intensity;12;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;253;-1280,640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;255;-1152,896;Inherit;False;Property;_EmissionMultiply;Emission Multiply;6;0;Create;True;0;0;0;False;3;Space(13);Header(General);Space(13);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;256;-896,896;Inherit;False;164;top;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-2688,1280;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;170;-1152,1280;Inherit;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;171;-640,1536;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;235;-1408,-128;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;237;-1792,-1280;Inherit;True;Property;_BaseTexture;Base Texture;10;0;Create;True;0;0;0;False;3;Space(33);Header(Base Texture);Space(13);False;-1;d3c13a5c8a52b27449c53de776bae569;d3c13a5c8a52b27449c53de776bae569;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;244;-1280,-1024;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;245;-896,-896;Inherit;False;164;top;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;247;-896,-1152;Inherit;False;164;top;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;251;-1152,640;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;254;-896,768;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;172;-2048,2560;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;173;-1536,2688;Inherit;False;Property;_TopFalloffIntensity;Top Falloff Intensity;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;-1536,2560;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;179;-3024,-128;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;217;-3072,1120;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;229;-896,256;Inherit;False;Property;_Smooth;Smooth;8;0;Create;True;0;0;0;False;0;False;0;0.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;230;-896,128;Inherit;False;Property;_Spec;Spec;7;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;246;-768,-1024;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;248;-768,-1280;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;249;-896,1280;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;-896,640;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;260;-1792,2560;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;261;-2048,2688;Inherit;False;Property;_TopFalloff;Top Falloff;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;5;ASEMaterialInspector;0;0;StandardSpecular;Vefects/SH_Vefects_VFX_Slime_Lit_Matcap_Triplanar_Top;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;117;0;116;0
WireConnection;200;0;206;0
WireConnection;226;0;208;0
WireConnection;226;1;227;0
WireConnection;259;0;257;0
WireConnection;259;1;258;0
WireConnection;202;0;200;0
WireConnection;202;1;201;0
WireConnection;209;0;259;0
WireConnection;209;1;226;0
WireConnection;199;0;198;0
WireConnection;199;1;210;0
WireConnection;205;0;202;0
WireConnection;211;0;209;0
WireConnection;211;1;225;0
WireConnection;197;0;211;0
WireConnection;197;1;199;0
WireConnection;204;0;205;0
WireConnection;204;1;205;1
WireConnection;204;2;205;2
WireConnection;195;0;197;0
WireConnection;196;0;197;0
WireConnection;203;0;202;0
WireConnection;203;1;204;0
WireConnection;232;0;197;0
WireConnection;192;0;231;0
WireConnection;192;1;232;0
WireConnection;193;0;231;0
WireConnection;193;1;195;0
WireConnection;194;0;231;0
WireConnection;194;1;196;0
WireConnection;207;0;203;0
WireConnection;188;0;193;0
WireConnection;188;1;207;0
WireConnection;189;0;192;0
WireConnection;189;1;207;2
WireConnection;190;0;194;0
WireConnection;190;1;207;1
WireConnection;191;0;189;0
WireConnection;191;1;188;0
WireConnection;191;2;190;0
WireConnection;180;0;191;0
WireConnection;181;0;180;0
WireConnection;218;0;181;2
WireConnection;218;1;241;0
WireConnection;239;0;181;0
WireConnection;239;1;241;0
WireConnection;240;0;181;1
WireConnection;240;1;241;0
WireConnection;220;0;219;1
WireConnection;220;1;239;0
WireConnection;221;0;219;2
WireConnection;221;1;240;0
WireConnection;222;0;219;3
WireConnection;222;1;218;0
WireConnection;223;0;220;0
WireConnection;223;1;221;0
WireConnection;223;2;222;0
WireConnection;224;0;223;0
WireConnection;125;0;118;0
WireConnection;127;0;119;0
WireConnection;127;1;120;0
WireConnection;128;0;121;0
WireConnection;128;1;122;0
WireConnection;129;0;123;0
WireConnection;129;1;124;0
WireConnection;176;0;175;0
WireConnection;176;1;224;0
WireConnection;131;0;125;0
WireConnection;131;1;126;0
WireConnection;132;0;127;0
WireConnection;132;1;128;0
WireConnection;136;0;129;0
WireConnection;177;0;176;0
WireConnection;137;0;130;0
WireConnection;137;1;133;0
WireConnection;138;0;131;0
WireConnection;139;0;132;0
WireConnection;139;1;134;0
WireConnection;140;0;136;0
WireConnection;140;1;135;2
WireConnection;178;0;177;0
WireConnection;142;0;139;0
WireConnection;142;1;137;0
WireConnection;143;0;138;0
WireConnection;143;1;138;1
WireConnection;143;2;138;2
WireConnection;144;0;140;0
WireConnection;144;1;141;0
WireConnection;236;1;178;0
WireConnection;147;0;144;0
WireConnection;148;0;142;0
WireConnection;149;0;142;0
WireConnection;150;0;142;0
WireConnection;151;0;131;0
WireConnection;151;1;143;0
WireConnection;153;0;146;0
WireConnection;153;1;145;0
WireConnection;185;0;224;0
WireConnection;185;1;184;0
WireConnection;185;2;183;0
WireConnection;185;3;182;0
WireConnection;262;0;236;0
WireConnection;262;1;263;0
WireConnection;154;0;152;0
WireConnection;154;1;148;0
WireConnection;155;0;152;0
WireConnection;155;1;149;0
WireConnection;156;0;152;0
WireConnection;156;1;150;0
WireConnection;157;0;151;0
WireConnection;158;0;147;0
WireConnection;158;1;146;0
WireConnection;158;2;153;0
WireConnection;186;0;185;0
WireConnection;186;1;228;0
WireConnection;264;0;262;0
WireConnection;264;1;265;0
WireConnection;159;0;155;0
WireConnection;159;1;157;0
WireConnection;160;0;154;0
WireConnection;160;1;157;2
WireConnection;161;0;156;0
WireConnection;161;1;157;1
WireConnection;162;0;158;0
WireConnection;187;0;264;0
WireConnection;187;1;186;0
WireConnection;213;0;212;0
WireConnection;213;1;214;0
WireConnection;163;0;160;0
WireConnection;163;1;159;0
WireConnection;163;2;161;0
WireConnection;164;0;162;0
WireConnection;216;0;215;0
WireConnection;216;1;213;0
WireConnection;233;0;264;0
WireConnection;250;0;187;0
WireConnection;165;0;163;0
WireConnection;234;0;233;1
WireConnection;234;1;216;0
WireConnection;253;0;250;1
WireConnection;253;1;216;0
WireConnection;169;0;166;0
WireConnection;169;1;165;0
WireConnection;171;1;167;0
WireConnection;171;2;168;0
WireConnection;235;0;234;0
WireConnection;235;1;233;2
WireConnection;235;2;233;3
WireConnection;244;0;242;0
WireConnection;244;1;238;0
WireConnection;244;2;243;0
WireConnection;251;0;253;0
WireConnection;251;1;250;2
WireConnection;251;2;250;3
WireConnection;254;1;255;0
WireConnection;254;2;256;0
WireConnection;172;0;147;0
WireConnection;172;1;261;0
WireConnection;174;0;260;0
WireConnection;174;1;173;0
WireConnection;246;0;242;0
WireConnection;246;1;244;0
WireConnection;246;2;245;0
WireConnection;248;0;237;0
WireConnection;248;1;235;0
WireConnection;248;2;247;0
WireConnection;249;0;170;0
WireConnection;249;1;169;0
WireConnection;249;2;171;0
WireConnection;252;0;251;0
WireConnection;252;1;254;0
WireConnection;260;0;172;0
WireConnection;0;0;248;0
WireConnection;0;1;246;0
WireConnection;0;2;252;0
WireConnection;0;3;230;0
WireConnection;0;4;229;0
WireConnection;0;11;249;0
ASEEND*/
//CHKSM=00DE2B547BE1B7710FFF6C59935DA02A9A68418E