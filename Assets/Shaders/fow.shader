Shader "Unlit/Fog of War"
{
	Properties
	{
		_ScrollSpeed ("Fog Scroll Speed", Float) = 0.11
		_Color1 ("Fog FG Color", Color) = (0.169,0.167,0.377,1)
		_Color2 ("Fog BG Color", Color) = (0.08,0.08,0.113,1)
		_Octaves ("Fog Complexity", Float) = 2
		_FogSize ("Fog Size", Float) = 1
		_FogEdge ("Fog Edge Roughness", Float) = 5
		_FogEdgeSoftness ("Fog Edge Softness", Range (0, 2)) = 0.7
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "Fbm.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			float _ScrollSpeed;
			float _Octaves;
			float4 _Color1;
			float4 _Color2;
			float _FogSize;
			float _FogEdge;
			float _FogEdgeSoftness;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = o.vertex * _FogSize;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float aspect = _ScreenParams.x / _ScreenParams.y;
				float2 uv = i.uv;
				uv.x = uv.x * aspect;
				float2 scrolluv = uv + (_ScrollSpeed * _Time.y);
				float f = fbm(uv + fbm(_ScrollSpeed + scrolluv, _Octaves), _Octaves);
				float3 fbmColor = lerp(_Color1, _Color2, 2*f);
				fixed4 col = fixed4(fbmColor, 1);

				float raggedEdges = fbm(scrolluv, _FogEdge);
				fixed4 mask = i.color;
				fixed4 maskEdgeOne = lerp( ceil(mask - raggedEdges), ceil(mask - raggedEdges * 0.75), raggedEdges);
				fixed4 maskEdge = lerp( maskEdgeOne, ceil(maskEdgeOne - raggedEdges * .4), raggedEdges);
				mask = lerp(mask, maskEdge, raggedEdges * _FogEdgeSoftness);
				col -= mask;
				return col;
			}
			ENDCG
		}
	}
}
