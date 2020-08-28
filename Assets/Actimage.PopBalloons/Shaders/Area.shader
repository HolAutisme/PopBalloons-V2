// Upgrade NOTE: upgraded instancing buffer 'CyhzaultArea' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Cyhzault/Area"
{
	Properties
	{
		_GuardianTexture("GuardianTexture", 2D) = "white" {}
		_RimColor("RimColor", Color) = (0,1,0.9172416,1)
		_AreaColor("AreaColor", Color) = (0.4485294,0.8859025,1,1)
		_FloorHeight("FloorHeight", Float) = 0
		_Threshold("Threshold", Range(0.01 , 1)) = 0.1799104
		_ObjectHeight("ObjectHeight", Range(0.01 , 4)) = 2
		_RimStrength("RimStrength", Range(0 , 20)) = 10
		_Speed("Speed", Range(0 , 2)) = 0.5
		[HideInInspector] _texcoord("", 2D) = "white" {}
		[HideInInspector] __dirty("", Int) = 1
	}

		SubShader
		{
			Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
			Cull Off
			CGPROGRAM
			#include "UnityShaderVariables.cginc"
			#pragma target 3.0
			#pragma multi_compile_instancing
			#pragma surface surf Standard alpha:fade keepalpha noshadow 
			struct Input
			{
				float3 worldPos;
				half2 uv_texcoord;
			};

			uniform half4 _AreaColor;
			uniform half4 _RimColor;
			uniform half _ObjectHeight;
			uniform half _RimStrength;
			uniform half _Threshold;
			uniform sampler2D _GuardianTexture;
			uniform fixed _Speed;
			uniform float4 _GuardianTexture_ST;

			UNITY_INSTANCING_BUFFER_START(CyhzaultArea)
				UNITY_DEFINE_INSTANCED_PROP(half, _FloorHeight)
#define _FloorHeight_arr CyhzaultArea
			UNITY_INSTANCING_BUFFER_END(CyhzaultArea)

			void surf(Input i , inout SurfaceOutputStandard o)
			{
				float3 ase_worldPos = i.worldPos;
				float _FloorHeight_Instance = UNITY_ACCESS_INSTANCED_PROP(_FloorHeight_arr, _FloorHeight);
				float clampResult29 = clamp((((ase_worldPos.y - _FloorHeight_Instance) / _ObjectHeight) * _RimStrength) , 0 , 1);
				float temp_output_11_0 = ((1.0 - clampResult29) * _Threshold);
				float4 lerpResult19 = lerp(_AreaColor , _RimColor , temp_output_11_0);
				o.Albedo = lerpResult19.rgb;
				o.Emission = lerpResult19.rgb;
				o.Metallic = 0.0;
				o.Smoothness = 0.0;
				float mulTime47 = _Time.y * _Speed;
				float2 appendResult46 = (half2(0.5 , -1));
				float2 uv_GuardianTexture = i.uv_texcoord * _GuardianTexture_ST.xy + _GuardianTexture_ST.zw;
				float2 panner36 = (uv_GuardianTexture + mulTime47 * appendResult46);
				half4 tex2DNode1 = tex2D(_GuardianTexture, panner36);
				half4 blendOpSrc55 = tex2DNode1;
				half4 blendOpDest55 = tex2DNode1;
				float clampResult34 = clamp((pow(temp_output_11_0 , 4) + (exp(temp_output_11_0) * (saturate((blendOpSrc55 + blendOpDest55 - 1.0))).r * temp_output_11_0)) , 0 , 1);
				o.Alpha = clampResult34;
			}

			ENDCG
		}
			CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15201
7;29;1906;1004;1869.985;676.8878;1.6;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;7;-1034.134,-199.4862;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;4;-1013.714,-47.59121;Float;False;InstancedProperty;_FloorHeight;FloorHeight;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;8;-823.0898,-123.5367;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1011.836,49.52828;Half;False;Property;_ObjectHeight;ObjectHeight;5;0;Create;True;0;0;False;0;2;0.01;0.01;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;15;-676.2021,-130.0866;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-952.288,151.3559;Half;False;Property;_RimStrength;RimStrength;6;0;Create;True;0;0;False;0;0.5;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1653.211,-270.9993;Fixed;False;Property;_Speed;Speed;7;0;Create;True;0;0;False;0;0.5;0.5;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-554.6588,-128.6202;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;47;-1332.041,-334.8716;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;-1260.041,-456.8716;Float;False;FLOAT2;4;0;FLOAT;0.5;False;1;FLOAT;-1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-1391.393,-593.6124;Float;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;29;-427.5396,-135.7733;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;36;-1059.247,-514.7469;Float;True;3;0;FLOAT2;0,1;False;2;FLOAT2;0,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;30;-285.7009,-127.8725;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-787.1295,-439.0817;Float;True;Property;_GuardianTexture;GuardianTexture;0;0;Create;True;0;0;False;0;8994953da3b95f14cb001c0c45e4d604;8994953da3b95f14cb001c0c45e4d604;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-871.0657,268.0829;Half;False;Property;_Threshold;Threshold;4;0;Create;True;0;0;False;0;0.1799104;0;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-25.69452,71.10342;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;55;-498.2224,-294.777;Float;False;LinearBurn;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ExpOpNode;60;157.5568,116.0076;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;56;-286.4036,-278.8232;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;323.5369,-107.6722;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;58;193.281,331.8444;Float;False;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-240.5187,-519.886;Float;False;Property;_RimColor;RimColor;1;0;Create;True;0;0;False;0;0,1,0.9172416,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-253.845,-723.7695;Float;False;Property;_AreaColor;AreaColor;2;0;Create;True;0;0;False;0;0.4485294,0.8859025,1,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;50;579.4252,150.2024;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;19;119.4873,-361.7856;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;34;736.8823,92.79006;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;596.3621,-148.4564;Float;False;Constant;_Smoothness;Smoothness;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;602.8134,-74.96372;Float;False;Constant;_Mettallic;Mettallic;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;976.6829,-195.6732;Half;False;True;2;Half;ASEMaterialInspector;0;0;Standard;Cyhzault/Area;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;7;2
WireConnection;8;1;4;0
WireConnection;15;0;8;0
WireConnection;15;1;27;0
WireConnection;32;0;15;0
WireConnection;32;1;31;0
WireConnection;47;0;41;0
WireConnection;29;0;32;0
WireConnection;36;0;39;0
WireConnection;36;2;46;0
WireConnection;36;1;47;0
WireConnection;30;0;29;0
WireConnection;1;1;36;0
WireConnection;11;0;30;0
WireConnection;11;1;10;0
WireConnection;55;0;1;0
WireConnection;55;1;1;0
WireConnection;60;0;11;0
WireConnection;56;0;55;0
WireConnection;35;0;60;0
WireConnection;35;1;56;0
WireConnection;35;2;11;0
WireConnection;58;0;11;0
WireConnection;50;0;58;0
WireConnection;50;1;35;0
WireConnection;19;0;2;0
WireConnection;19;1;3;0
WireConnection;19;2;11;0
WireConnection;34;0;50;0
WireConnection;0;0;19;0
WireConnection;0;2;19;0
WireConnection;0;3;51;0
WireConnection;0;4;48;0
WireConnection;0;9;34;0
ASEEND*/
//CHKSM=D8CA877FD1B698A7986182A39154E95DA6F4E4B4