// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Cyhzault/UpwardApparition"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Progression("Progression", Range( 0 , 1)) = 1
		_Tint("Tint", Color) = (1,1,1,1)
		_GrayScale("GrayScale", 2D) = "white" {}
		_Edge("Edge", Color) = (0,0.8565415,0.9338235,1)
		[Toggle]_RoomSpace("RoomSpace", Float) = 0
		_Objectheight("Object height", Range( 0 , 10)) = 1
		_ObjectFloor("Object Floor", Vector) = (0,0,0,0)
		_Factor("Factor", Range( 0 , 4)) = 1
		_Divide("Divide", Range( 0.1 , 10)) = 0.25
		_Border("Border", Range( 0 , 1)) = 0.5
		_Height("Height", Range( 0 , 0.2)) = 0.1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Lambert keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			half2 uv_texcoord;
			float3 worldPos;
		};

		uniform half4 _Tint;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform half4 _Edge;
		uniform half _RoomSpace;
		uniform half3 _ObjectFloor;
		uniform half _Objectheight;
		uniform half _Progression;
		uniform half3 RoomFloor;
		uniform half RoomHeight;
		uniform half _Border;
		uniform half _Divide;
		uniform sampler2D _GrayScale;
		uniform float4 _GrayScale_ST;
		uniform half _Factor;
		uniform half _Height;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float smoothstepResult47 = smoothstep( 1 , 0 , ( distance( lerp(( ( ase_worldPos.y - _ObjectFloor.y ) - ( _Objectheight * _Progression ) ),( ( ase_worldPos.y - RoomFloor.y ) - ( RoomHeight * _Progression ) ),_RoomSpace) , _Border ) / _Divide ));
			float clampResult49 = clamp( pow( smoothstepResult47 , 4 ) , 0 , 1 );
			half3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			half3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			half3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
			half3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * tangentSign;
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float fresnelNDotV67 = dot( mul( ase_vertexNormal,ase_worldToTangent ), ase_worldViewDir );
			float fresnelNode67 = ( 1 + 5 * pow( 1.0 - fresnelNDotV67, 0 ) );
			v.vertex.xyz += ( ase_vertexNormal * clampResult49 * fresnelNode67 * _Height );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float smoothstepResult47 = smoothstep( 1 , 0 , ( distance( lerp(( ( ase_worldPos.y - _ObjectFloor.y ) - ( _Objectheight * _Progression ) ),( ( ase_worldPos.y - RoomFloor.y ) - ( RoomHeight * _Progression ) ),_RoomSpace) , _Border ) / _Divide ));
			float clampResult49 = clamp( pow( smoothstepResult47 , 4 ) , 0 , 1 );
			float4 lerpResult55 = lerp( ( _Tint * tex2D( _MainTex, uv_MainTex ) ) , _Edge , clampResult49);
			o.Albedo = lerpResult55.rgb;
			float4 lerpResult74 = lerp( float4( 0,0,0,0 ) , _Edge , clampResult49);
			o.Emission = lerpResult74.rgb;
			o.Alpha = 1;
			float2 uv_GrayScale = i.uv_texcoord * _GrayScale_ST.xy + _GrayScale_ST.zw;
			float clampResult50 = clamp( ( lerp(( ( ase_worldPos.y - _ObjectFloor.y ) - ( _Objectheight * _Progression ) ),( ( ase_worldPos.y - RoomFloor.y ) - ( RoomHeight * _Progression ) ),_RoomSpace) - ( (tex2D( _GrayScale, uv_GrayScale )).r * _Factor ) ) , 0 , 1 );
			clip( ( 1.0 - clampResult50 ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15201
461;347;1356;728;1108.71;1001.837;2.126118;True;False
Node;AmplifyShaderEditor.Vector3Node;79;-3192.45,-715.165;Half;False;Property;_ObjectFloor;Object Floor;9;0;Create;True;0;0;False;0;0,0,0;0,-1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;7;-3180.014,-373.3547;Half;False;Property;_Objectheight;Object height;8;0;Create;True;0;0;False;0;1;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-3186.412,-117.2359;Half;False;Property;_Progression;Progression;1;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;25;-3200.474,-875.8665;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;82;-3187.739,-249.6395;Half;False;Global;RoomHeight;Room Height;7;0;Create;True;0;0;False;0;1;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;70;-3189.756,-556.2763;Half;False;Global;RoomFloor;Room Floor;10;0;Create;True;0;0;False;0;0,0,0;0,-1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-2902.934,-222.4395;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-2899.406,-344.4399;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;84;-2939.937,-621.5399;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;27;-2942.75,-760.0288;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;85;-2717.534,-602.8393;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;11;-2714.046,-722.9617;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;-3199.781,646.5242;Float;False;0;34;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;80;-2467.357,-696.7109;Half;True;Property;_RoomSpace;RoomSpace;7;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1740.57,-790.3018;Half;False;Property;_Border;Border;12;0;Create;True;0;0;False;0;0.5;0.78;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1536.253,-700.5211;Half;False;Property;_Divide;Divide;11;0;Create;True;0;0;False;0;0.25;0.6;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;34;-2875.051,575.8577;Float;True;Property;_GrayScale;GrayScale;5;0;Create;True;0;0;False;0;None;9fbef4b79ca3b784ba023cb1331520d5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode;43;-1521.815,-976.0678;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;46;-1312.068,-907.0427;Float;False;2;0;FLOAT;0;False;1;FLOAT;32;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2549.799,684.4472;Half;False;Property;_Factor;Factor;10;0;Create;True;0;0;False;0;1;0.69;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;39;-2557.049,552.3021;Float;False;True;False;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-2244.741,508.4327;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;47;-1132.68,-864.2944;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;42;-1849.5,105.0366;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-512.33,-1120.156;Half;False;Property;_Tint;Tint;2;0;Create;True;0;0;False;0;1,1,1,1;0.8773585,0.8773585,0.8773585,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;48;-1149.04,-687.072;Float;False;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;75;-587.9577,-942.9614;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;8770060e23abf814a9973e7b9765d809;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;65;-814.9755,-134.3419;Float;True;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;49;-914.2057,-694.6779;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;54;-435.9795,-747.5596;Half;False;Property;_Edge;Edge;6;0;Create;True;0;0;False;0;0,0.8565415,0.9338235,1;0,0.8565415,0.9338235,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-238.5493,-955.9421;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-590.1753,202.2581;Half;False;Property;_Height;Height;13;0;Create;True;0;0;False;0;0.1;0.002;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;67;-564.1288,-34.68522;Float;False;Tangent;4;0;FLOAT3;0,0,1;False;1;FLOAT;1;False;2;FLOAT;5;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;50;-973.9149,109.3478;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-332.3752,-96.94188;Float;True;4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;74;-29.62425,-511.485;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;55;-15.09176,-770.8023;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;60;-763.4454,95.48038;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;104.1569,-273.9237;Fixed;False;Property;_Mettalic;Mettalic;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;105.6093,-182.4133;Fixed;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;598.3544,-352.0936;Half;False;True;6;Half;ASEMaterialInspector;0;0;Lambert;Cyhzault/UpwardApparition;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;True;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;14;-1;-1;-1;0;0;0;False;0;0;0;False;-1;4;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;62;-1790.57,-1026.068;Float;False;1068.936;487.3898;Comment;0;Edge;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;61;-3223.502,-957.1512;Float;False;1035.205;976.801;Comment;0;Linear mask;1,1,1,1;0;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;31;0;7;0
WireConnection;31;1;81;0
WireConnection;84;0;25;2
WireConnection;84;1;70;2
WireConnection;27;0;25;2
WireConnection;27;1;79;2
WireConnection;85;0;84;0
WireConnection;85;1;83;0
WireConnection;11;0;27;0
WireConnection;11;1;31;0
WireConnection;80;0;11;0
WireConnection;80;1;85;0
WireConnection;34;1;35;0
WireConnection;43;0;80;0
WireConnection;43;1;45;0
WireConnection;46;0;43;0
WireConnection;46;1;52;0
WireConnection;39;0;34;0
WireConnection;40;0;39;0
WireConnection;40;1;41;0
WireConnection;47;0;46;0
WireConnection;42;0;80;0
WireConnection;42;1;40;0
WireConnection;48;0;47;0
WireConnection;49;0;48;0
WireConnection;76;0;1;0
WireConnection;76;1;75;0
WireConnection;67;0;65;0
WireConnection;50;0;42;0
WireConnection;64;0;65;0
WireConnection;64;1;49;0
WireConnection;64;2;67;0
WireConnection;64;3;66;0
WireConnection;74;1;54;0
WireConnection;74;2;49;0
WireConnection;55;0;76;0
WireConnection;55;1;54;0
WireConnection;55;2;49;0
WireConnection;60;0;50;0
WireConnection;0;0;55;0
WireConnection;0;2;74;0
WireConnection;0;10;60;0
WireConnection;0;11;64;0
ASEEND*/
//CHKSM=FAD422393A32BBD8BC8A942A3070C65A84492637