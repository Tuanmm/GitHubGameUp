// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sand shader"
{
	Properties
	{
		_Blend("Blend color 1-2", Range( 0 , 1)) = 0.7777807
		_Blend1("Blend color 3", Range( 0 , 1)) = 0.6061082
		_Soft("Softness color 1-2", Range( 0 , 1)) = 1
		_Soft1("Softness color 3", Range( 0 , 1)) = 1
		_Color1("Color 1", Color) = (1,0.8705691,0,0)
		_Color2("Color 2", Color) = (1,0.07438041,0,0)
		_Color3("Color 3", Color) = (0,0.1847806,1,0)
		[Toggle(_FLIPCOLOR3_ON)] _Flipcolor3("Flip color 3", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _FLIPCOLOR3_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float _Soft;
		uniform float _Blend;
		uniform float4 _Color3;
		uniform float _Soft1;
		uniform float _Blend1;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 ase_vertex4Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float smoothstepResult20 = smoothstep( 0.0 , _Soft , ( (-1.0 + (_Blend - 0.0) * (2.0 - -1.0) / (1.0 - 0.0)) - ase_vertex4Pos.y ));
			float4 lerpResult2 = lerp( _Color1 , _Color2 , smoothstepResult20);
			float smoothstepResult30 = smoothstep( 0.0 , _Soft1 , ( (-1.0 + (_Blend1 - 0.0) * (2.0 - -1.0) / (1.0 - 0.0)) - ase_vertex4Pos.y ));
			#ifdef _FLIPCOLOR3_ON
				float staticSwitch37 = smoothstepResult30;
			#else
				float staticSwitch37 = ( 1.0 - smoothstepResult30 );
			#endif
			float4 lerpResult23 = lerp( lerpResult2 , _Color3 , staticSwitch37);
			o.Albedo = ( tex2D( _TextureSample0, uv_TextureSample0 ) * lerpResult23 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
249;186;1646;833;983.5742;427.1237;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;25;-1621.71,-788.7877;Inherit;False;Property;_Blend1;Blend color 3;1;0;Create;False;0;0;False;0;False;0.6061082;0.37;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1975.276,-284.8405;Inherit;False;Property;_Blend;Blend color 1-2;0;0;Create;False;0;0;False;0;False;0.7777807;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;26;-1581.662,-499.5682;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;27;-1302.783,-784.4044;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;3;-1935.228,4.378872;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;18;-1656.349,-280.4572;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;28;-953.6621,-677.5683;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1165.562,-444.1784;Inherit;False;Property;_Soft1;Softness color 3;3;0;Create;False;0;0;False;0;False;1;0.59;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1519.128,59.76867;Inherit;False;Property;_Soft;Softness color 1-2;2;0;Create;False;0;0;False;0;False;1;0.59;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;30;-690.0306,-499.9065;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;10;-1307.228,-173.6211;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-1050.539,233.1026;Inherit;False;Property;_Color1;Color 1;4;0;Create;True;0;0;False;0;False;1,0.8705691,0,0;1,0.8237302,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;-1066.112,410.3761;Inherit;False;Property;_Color2;Color 2;5;0;Create;True;0;0;False;0;False;1,0.07438041,0,0;1,0.03724591,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;36;-582.327,-700.7557;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;20;-943.2039,-70.71712;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;31;-752.5684,430.078;Inherit;False;Property;_Color3;Color 3;6;0;Create;True;0;0;False;0;False;0,0.1847806,1,0;0,0.1847806,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;2;-461.5529,2.061343;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;37;-489.521,-465.4034;Inherit;False;Property;_Flipcolor3;Flip color 3;7;0;Create;True;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;23;-130.0067,-39.32317;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;46;-116.5742,-305.1237;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;243.4258,2.876282;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;472,12;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Sand shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;0;25;0
WireConnection;18;0;9;0
WireConnection;28;0;27;0
WireConnection;28;1;26;2
WireConnection;30;0;28;0
WireConnection;30;2;29;0
WireConnection;10;0;18;0
WireConnection;10;1;3;2
WireConnection;36;0;30;0
WireConnection;20;0;10;0
WireConnection;20;2;21;0
WireConnection;2;0;1;0
WireConnection;2;1;4;0
WireConnection;2;2;20;0
WireConnection;37;1;36;0
WireConnection;37;0;30;0
WireConnection;23;0;2;0
WireConnection;23;1;31;0
WireConnection;23;2;37;0
WireConnection;47;0;46;0
WireConnection;47;1;23;0
WireConnection;0;0;47;0
ASEEND*/
//CHKSM=E1AF13B593A9616CA75F3165DD51F3043B0F921E