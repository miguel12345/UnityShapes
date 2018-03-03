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
            #pragma multi_compile _ BORDER
			
			#include "UnityCG.cginc"

            float _AASmoothing;
            fixed4 _Color;
            
            #if BORDER
            fixed4 _BorderColor;
            float _FillWidth;
            #endif

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
			    
			    float edgeDistancePerPixel = fwidth(edgeDistance);
			    			    
			    fixed4 fillColor = _Color;
			    
			    #if BORDER
			    float fillToBlendColorLerpFactor = smoothstep(_FillWidth,_FillWidth + edgeDistancePerPixel*_AASmoothing,edgeDistance);
			    fillColor = lerp(fillColor,_BorderColor,fillToBlendColorLerpFactor);
			    #endif
			    
                float edgeAlpha = 1.0 - smoothstep(0.5 - edgeDistancePerPixel*_AASmoothing, 0.5, edgeDistance);

			    fillColor.a *= edgeAlpha;
			    
                return fillColor;
			}
			ENDCG
		}
	}
}
