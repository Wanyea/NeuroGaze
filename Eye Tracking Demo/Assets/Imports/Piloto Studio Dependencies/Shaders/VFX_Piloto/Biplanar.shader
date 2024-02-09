// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Biplanar"
{
	Properties
	{
		[NoScaleOffset]_tex("tex", 2D) = "white" {}
		_scale("scale", Float) = 2

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Vertex"
			Tags { "LightMode"="Vertex" }

			CGPROGRAM
			

			#define ASE_USING_SAMPLING_MACROS 1

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
			#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
			#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex.SampleLevel(samplerTex,coord, lod)
			#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex.SampleBias(samplerTex,coord,bias)
			#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex.SampleGrad(samplerTex,coord,ddx,ddy)
			#else//ASE Sampling Macros
			#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
			#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex2Dlod(tex,float4(coord,0,lod))
			#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex2Dbias(tex,float4(coord,0,bias))
			#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex2Dgrad(tex,coord,ddx,ddy)
			#endif//ASE Sampling Macros
			


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			UNITY_DECLARE_TEX2D_NOSAMPLER(_tex);
			SamplerState sampler_tex;
			uniform float _scale;
			float4 blending7( UNITY_DECLARE_TEX2D_NOSAMPLER(tex), SamplerState samp, float3 dpdx, float3 dpdy, float4 samplingCoords, float2 blendFactors )
			{
				    // Unpack the float4 into two float2s for sampling
				    float2 coords_x = samplingCoords.xy;
				    float2 coords_y = samplingCoords.zw;
				    // Sample textures
				    float4 x = SAMPLE_TEXTURE2D_GRAD(tex, samp, coords_x, float2(dpdx.y, dpdx.z), float2(dpdy.y, dpdy.z));
				    float4 y = SAMPLE_TEXTURE2D_GRAD(tex, samp, coords_y, float2(dpdx.y, dpdx.z), float2(dpdy.y, dpdy.z));
				    // Use the blend factors for blending
				    float2 w = blendFactors;
				    // Perform blending
				    return (x * w.x + y * w.y) / (w.x + w.y);
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				UNITY_DECLARE_TEX2D_NOSAMPLER(tex7) = _tex;
				SamplerState samp7 = sampler_tex;
				float3 temp_output_28_0 = ( WorldPosition * _scale );
				float3 dpdx24 = ddx( temp_output_28_0 );
				float3 dpdx7 = dpdx24;
				float3 dpdy25 = ddy( temp_output_28_0 );
				float3 dpdy7 = dpdy25;
				float3 localprecalcreusable12 = ( float3( 0,0,0 ) );
				float3 p12 = temp_output_28_0;
				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float3 normalizeResult16 = normalize( abs( ase_worldNormal ) );
				float3 n12 = normalizeResult16;
				float4 samplingCoords12 = float4( 0,0,0,0 );
				float2 blendFactors12 = float2( 0,0 );
				{
				    // Major axis
				    uint ma_x, ma_y, ma_z;
				    if (n12.x > n12.y && n12.x > n12.z) {
				        ma_x = 0; ma_y = 1; ma_z = 2;
				    } else if (n12.y > n12.z) {
				        ma_x = 1; ma_y = 2; ma_z = 0;
				    } else {
				        ma_x = 2; ma_y = 0; ma_z = 1;
				    }
				    // Minor axis
				    uint mi_x, mi_y, mi_z;
				    if (n12.x < n12.y && n12.x < n12.z) {
				        mi_x = 0; mi_y = 1; mi_z = 2;
				    } else if (n12.y < n12.z) {
				        mi_x = 1; mi_y = 2; mi_z = 0;
				    } else {
				        mi_x = 2; mi_y = 0; mi_z = 1;
				    }
				    // Median axis
				    uint me_x = 3 - mi_x - ma_x;
				    uint me_y = 3 - mi_y - ma_y;
				    uint me_z = 3 - mi_z - ma_z;
				    // Return float4 containing two float2s for sampling
				    samplingCoords12 = float4(p12[ma_y], p12[ma_z], p12[me_y], p12[me_z]);
				    // Blend factors
				    blendFactors12 = float2(n12[ma_x], n12[me_x]);
				    // Make local support
				    blendFactors12 = saturate((blendFactors12 - 0.5773) / (1 - 0.5773));
				}
				float4 uvs17 = samplingCoords12;
				float4 samplingCoords7 = uvs17;
				float2 blendfactors18 = blendFactors12;
				float2 blendFactors7 = blendfactors18;
				float4 localblending7 = blending7( tex7 , samp7 , dpdx7 , dpdy7 , samplingCoords7 , blendFactors7 );
				
				
				finalColor = localblending7;
				return finalColor;
			}
			ENDCG
		}
		
	}
	
	
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;21;-2024.113,-710.7651;Inherit;False;1161.251;593.2015;setup things that can be reused during sampling;13;23;25;24;22;18;17;14;15;16;13;12;28;29;;1,0.7877358,0.9849128,1;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;162.287,-543.1647;Float;False;True;-1;2;ASEMaterialInspector;100;5;Biplanar;0770190933193b94aaa3065e307002fa;True;Vertex;0;0;Vertex;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Vertex;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;True;0
Node;AmplifyShaderEditor.NormalizeNode;16;-1594.964,-300.5605;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;15;-1731.577,-300.175;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;14;-1934.845,-300.175;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1134.199,-506.963;Inherit;False;uvs;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-1112.945,-348.1719;Inherit;False;blendfactors;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DdxOpNode;22;-1528.632,-649.2772;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-1333.475,-655.7867;Inherit;False;dpdx;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-1353.114,-565.661;Inherit;False;dpdy;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DdyOpNode;23;-1524.478,-559.9753;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1705.472,-549.7386;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;13;-1961.484,-622.6968;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;29;-1929.547,-412.3471;Inherit;False;Property;_scale;scale;1;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;12;-1424.09,-451.3441;Inherit;False;    // Major axis$    uint ma_x, ma_y, ma_z@$    if (n.x > n.y && n.x > n.z) {$        ma_x = 0@ ma_y = 1@ ma_z = 2@$    } else if (n.y > n.z) {$        ma_x = 1@ ma_y = 2@ ma_z = 0@$    } else {$        ma_x = 2@ ma_y = 0@ ma_z = 1@$    }$$    // Minor axis$    uint mi_x, mi_y, mi_z@$    if (n.x < n.y && n.x < n.z) {$        mi_x = 0@ mi_y = 1@ mi_z = 2@$    } else if (n.y < n.z) {$        mi_x = 1@ mi_y = 2@ mi_z = 0@$    } else {$        mi_x = 2@ mi_y = 0@ mi_z = 1@$    }$$    // Median axis$    uint me_x = 3 - mi_x - ma_x@$    uint me_y = 3 - mi_y - ma_y@$    uint me_z = 3 - mi_z - ma_z@$$    // Return float4 containing two float2s for sampling$    samplingCoords = float4(p[ma_y], p[ma_z], p[me_y], p[me_z])@$$    // Blend factors$    blendFactors = float2(n[ma_x], n[me_x])@$$    // Make local support$    blendFactors = saturate((blendFactors - 0.5773) / (1 - 0.5773))@;7;Create;4;True;p;FLOAT3;0,0,0;In;;Inherit;False;True;n;FLOAT3;0,0,0;In;;Inherit;False;True;samplingCoords;FLOAT4;0,0,0,0;Out;;Inherit;False;True;blendFactors;FLOAT2;0,0;Out;;Inherit;False;pre calc reusable;True;False;0;;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT2;0,0;False;3;FLOAT3;0;FLOAT4;4;FLOAT2;5
Node;AmplifyShaderEditor.StickyNoteNode;31;416.0305,-542.0581;Inherit;False;150;100;note;;0.9811321,0.6895692,0.6895692,1;needs sampling macros;0;0
Node;AmplifyShaderEditor.CustomExpressionNode;7;-152.1352,-538.5912;Inherit;False;    // Unpack the float4 into two float2s for sampling$    float2 coords_x = samplingCoords.xy@$    float2 coords_y = samplingCoords.zw@$$    // Sample textures$    float4 x = SAMPLE_TEXTURE2D_GRAD(tex, samp, coords_x, float2(dpdx.y, dpdx.z), float2(dpdy.y, dpdy.z))@$    float4 y = SAMPLE_TEXTURE2D_GRAD(tex, samp, coords_y, float2(dpdx.y, dpdx.z), float2(dpdy.y, dpdy.z))@$$    // Use the blend factors for blending$    float2 w = blendFactors@$$    // Perform blending$    return (x * w.x + y * w.y) / (w.x + w.y)@;4;Create;6;True;tex;SAMPLER2D;;In;;Inherit;False;True;samp;SAMPLERSTATE;;In;;Inherit;False;True;dpdx;FLOAT3;0,0,0;In;;Inherit;False;True;dpdy;FLOAT3;0,0,0;In;;Inherit;False;True;samplingCoords;FLOAT4;0,0,0,0;In;;Inherit;False;True;blendFactors;FLOAT2;0,0;In;;Inherit;False;blending;True;False;0;;False;6;0;SAMPLER2D;;False;1;SAMPLERSTATE;;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-825.868,-686.9304;Inherit;True;Property;_tex;tex;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;27;-389.9785,-395.4478;Inherit;False;25;dpdy;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-352.0931,-292.5118;Inherit;False;17;uvs;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-296.6826,-175.7959;Inherit;False;18;blendfactors;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;-434.8983,-481.0363;Inherit;False;24;dpdx;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;3;-1064.068,128.146;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;4;-1254.448,339.194;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.AbsOpNode;5;-968.7009,339.194;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;6;-782.6009,368.5012;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;2;-588.6965,97.87355;Inherit;False;    float3 dpdx = ddx(p)@$    float3 dpdy = ddy(p)@$// Major axis$uint ma_x, ma_y, ma_z@$if (n.x > n.y && n.x > n.z) {$    ma_x = 0@ ma_y = 1@ ma_z = 2@$} else if (n.y > n.z) {$    ma_x = 1@ ma_y = 2@ ma_z = 0@$} else {$    ma_x = 2@ ma_y = 0@ ma_z = 1@$}$$// Minor axis$uint mi_x, mi_y, mi_z@$if (n.x < n.y && n.x < n.z) {$    mi_x = 0@ mi_y = 1@ mi_z = 2@$} else if (n.y < n.z) {$    mi_x = 1@ mi_y = 2@ mi_z = 0@$} else {$    mi_x = 2@ mi_y = 0@ mi_z = 1@$}$$// Median axis$uint me_x = 3 - mi_x - ma_x@$uint me_y = 3 - mi_y - ma_y@$uint me_z = 3 - mi_z - ma_z@$$// Project + fetch$float4 x = SAMPLE_TEXTURE2D_GRAD(tex, samp, float2(p[ma_y], p[ma_z]), float2(dpdx[ma_y], dpdx[ma_z]), float2(dpdy[ma_y], dpdy[ma_z]))@$float4 y = SAMPLE_TEXTURE2D_GRAD(tex, samp, float2(p[me_y], p[me_z]), float2(dpdx[me_y], dpdx[me_z]), float2(dpdy[me_y], dpdy[me_z]))@$$// Blend factors$float2 w = float2(n[ma_x], n[me_x])@$$// Make local support$w = saturate((w - 0.5773) / (1 - 0.5773))@$$$// Blending$return (x * w.x + y * w.y) / (w.x + w.y)@;4;Create;4;True;tex;SAMPLER2D;;In;;Inherit;False;True;samp;SAMPLERSTATE;;In;;Inherit;False;True;p;FLOAT3;0,0,0;In;;Inherit;False;True;n;FLOAT3;0,0,0;In;;Inherit;False;all in one;True;False;0;;False;4;0;SAMPLER2D;;False;1;SAMPLERSTATE;;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT4;0
WireConnection;0;0;7;0
WireConnection;16;0;15;0
WireConnection;15;0;14;0
WireConnection;17;0;12;4
WireConnection;18;0;12;5
WireConnection;22;0;28;0
WireConnection;24;0;22;0
WireConnection;25;0;23;0
WireConnection;23;0;28;0
WireConnection;28;0;13;0
WireConnection;28;1;29;0
WireConnection;12;1;28;0
WireConnection;12;2;16;0
WireConnection;7;0;1;0
WireConnection;7;1;1;1
WireConnection;7;2;26;0
WireConnection;7;3;27;0
WireConnection;7;4;20;0
WireConnection;7;5;19;0
WireConnection;5;0;4;0
WireConnection;6;0;5;0
WireConnection;2;0;1;0
WireConnection;2;1;1;1
WireConnection;2;2;3;0
WireConnection;2;3;6;0
ASEEND*/
//CHKSM=C87F58DDBEAB81E70A0DDB64482531E29943FFAF