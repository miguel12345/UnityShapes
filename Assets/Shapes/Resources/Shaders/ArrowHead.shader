Shader "Hidden/Shapes/LineSegment/ArrowHead"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 100

		Pass
		{
		    ZWrite Off
		    Cull off
		    Blend SrcAlpha OneMinusSrcAlpha
		    
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 vertexDistances : TEXCOORD1;
			};

            float _AASmoothing;
			fixed4 _Color;
			float _BaseEdgeNoAAMinX;
			float _BaseEdgeNoAAMaxX;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexDistances = v.color.xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			
			    float deltas = fwidth(i.vertexDistances);
			    
			    float3 edgeAlphas = smoothstep(0,deltas*_AASmoothing,i.vertexDistances);
			    
			    float edgeAlpha = min(edgeAlphas.x,min(edgeAlphas.y,edgeAlphas.z));
			    
			    if(edgeAlpha == edgeAlphas.z && i.uv.x >= _BaseEdgeNoAAMinX && i.uv.x <= _BaseEdgeNoAAMaxX) {
			        edgeAlpha = 1.0;
			    }
			    
			    fixed4 finalColor = _Color;
			    finalColor.a *= edgeAlpha;
			    
				return finalColor;
			}
			ENDCG
		}
	}
}
