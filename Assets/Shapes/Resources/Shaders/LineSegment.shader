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
            #pragma multi_compile _ DASHED
            #pragma multi_compile VERTICAL_EDGE_SMOOTH_ON VERTICAL_EDGE_SMOOTH_OFF
			
			#include "UnityCG.cginc"

            float _AASmoothing;
            fixed4 _Color;
            fixed _LineLength;
            
            #if BORDER
            fixed4 _BorderColor;
            float _FillWidth;
            #endif
            
            #if DASHED
            float _DistanceBetweenDashes;
            float _DashWidth;
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
			    
			   
			    #if DASHED
			    float distanceFromLineOrigin = i.uv.y * _LineLength;
			    float distanceFromLineOriginPerPixel = fwidth(distanceFromLineOrigin);
			    float smoothDistance = distanceFromLineOriginPerPixel*_AASmoothing;
			    float halfSmoothDistance = smoothDistance * 0.5f;
			    
			    float previousDashSegmentDistanceFromOrigin = floor(distanceFromLineOrigin / _DistanceBetweenDashes) * _DistanceBetweenDashes;
			    float nextDashSegmentDistanceFromOrigin = previousDashSegmentDistanceFromOrigin + _DistanceBetweenDashes;
			    
			    float fragmentDistanceFromPreviousDashSegment = distanceFromLineOrigin - previousDashSegmentDistanceFromOrigin;
			    float fragmentDistanceFromNextDashSegment = nextDashSegmentDistanceFromOrigin - distanceFromLineOrigin;
			    
			    float distanceFromClosestDashSegment = min(fragmentDistanceFromPreviousDashSegment,fragmentDistanceFromNextDashSegment);
			    
			    float dashAlpha = 1.0 - smoothstep(_DashWidth-halfSmoothDistance,_DashWidth+halfSmoothDistance,distanceFromClosestDashSegment);
			    
			    fillColor.a *= dashAlpha;
			    #endif
			     
			    
                float edgeAlpha = 1.0 - smoothstep(0.5 - edgeDistancePerPixel*_AASmoothing, 0.5, edgeDistance);
			    
			    #if VERTICAL_EDGE_SMOOTH_ON
			    float verticalDistance = i.uv.y;
			    float distanceToClosestVerticalEdge = min(1.0-verticalDistance,verticalDistance);
			    float distanceToClosestVerticalEdgePerPixel = fwidth(distanceToClosestVerticalEdge);
			    float verticalEdgeAlpha = smoothstep(0.0, distanceToClosestVerticalEdgePerPixel*_AASmoothing, distanceToClosestVerticalEdge);
			    fillColor.a *= verticalEdgeAlpha;
			    #endif
			     
			     fillColor.a *= edgeAlpha;
			    
                return fillColor;
			}
			ENDCG
		}
	}
}
