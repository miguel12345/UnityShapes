Shader "Hidden/Shapes/LineSegment"
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

            float _AASmoothing;
            fixed4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			    float edgeDistance = i.uv.x;
			    
			    float edgeDistanceDerivative = fwidth(edgeDistance);
			    
			    float alpha = 1.0 - smoothstep(0.5 - edgeDistanceDerivative*_AASmoothing, 0.5, edgeDistance);
			    
			    fixed4 finalColor = _Color;
			    finalColor.a *= alpha;
			    
                return finalColor;
			}
			ENDCG
		}
	}
}
