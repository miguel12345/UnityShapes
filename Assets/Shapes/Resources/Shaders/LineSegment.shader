Shader "Hidden/Shapes/LineSegment"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" "DisableBatching" ="true"}
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
            #pragma multi_compile_instancing
			
			#include "UnityCG.cginc"

            UNITY_INSTANCING_BUFFER_START(CommonProps)
                UNITY_DEFINE_INSTANCED_PROP(float, _AASmoothing)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_INSTANCING_BUFFER_END(CommonProps)
            
            #if BORDER
            UNITY_INSTANCING_BUFFER_START(BorderProps)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _BorderColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _FillWidth)
            UNITY_INSTANCING_BUFFER_END(BorderProps)
            #endif
            
            #if DASHED
            UNITY_INSTANCING_BUFFER_START(DashProps)
                UNITY_DEFINE_INSTANCED_PROP(float, _DistanceBetweenDashes)
                UNITY_DEFINE_INSTANCED_PROP(float, _DashWidth)
                UNITY_DEFINE_INSTANCED_PROP(fixed, _LineLength)
            UNITY_INSTANCING_BUFFER_END(DashProps)
            #endif

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			v2f vert (appdata v)
			{
				v2f o;
				
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			    UNITY_SETUP_INSTANCE_ID(i);
			    
			    float edgeDistance = i.uv.x;
			    float edgeDistancePerPixel = fwidth(edgeDistance);
               
			    float aaSmoothing = UNITY_ACCESS_INSTANCED_PROP(CommonProps, _AASmoothing);			    
			    fixed4 fillColor = UNITY_ACCESS_INSTANCED_PROP(CommonProps, _Color);
			    
			    #if BORDER
			    float fillWidth = UNITY_ACCESS_INSTANCED_PROP(BorderProps, _FillWidth);
			    fixed4 borderColor = UNITY_ACCESS_INSTANCED_PROP(BorderProps, _BorderColor);
			    
			    float fillToBlendColorLerpFactor = smoothstep(fillWidth,fillWidth + edgeDistancePerPixel*aaSmoothing,edgeDistance);
			    fillColor = lerp(fillColor,borderColor,fillToBlendColorLerpFactor);
			    #endif
			    
			   
			    #if DASHED
			    float lineLength = UNITY_ACCESS_INSTANCED_PROP(DashProps, _LineLength);
			    float distanceBetweenDashes = UNITY_ACCESS_INSTANCED_PROP(DashProps, _DistanceBetweenDashes);
			    float dashWidth = UNITY_ACCESS_INSTANCED_PROP(DashProps, _DashWidth);
			    
			    float distanceFromLineOrigin = i.uv.y * lineLength;
			    float distanceFromLineOriginPerPixel = fwidth(distanceFromLineOrigin);
			    float smoothDistance = distanceFromLineOriginPerPixel*aaSmoothing;
			    float halfSmoothDistance = smoothDistance * 0.5f;
			    
			    float previousDashSegmentDistanceFromOrigin = floor(distanceFromLineOrigin / distanceBetweenDashes) * distanceBetweenDashes;
			    float nextDashSegmentDistanceFromOrigin = previousDashSegmentDistanceFromOrigin + distanceBetweenDashes;
			    
			    float fragmentDistanceFromPreviousDashSegment = distanceFromLineOrigin - previousDashSegmentDistanceFromOrigin;
			    float fragmentDistanceFromNextDashSegment = nextDashSegmentDistanceFromOrigin - distanceFromLineOrigin;
			    
			    float distanceFromClosestDashSegment = min(fragmentDistanceFromPreviousDashSegment,fragmentDistanceFromNextDashSegment);
			    
			    float dashAlpha = 1.0 - smoothstep(dashWidth-halfSmoothDistance,dashWidth+halfSmoothDistance,distanceFromClosestDashSegment);
			    
			    fillColor.a *= dashAlpha;
			    #endif
			     
			    
                float edgeAlpha = 1.0 - smoothstep(0.5 - edgeDistancePerPixel*aaSmoothing, 0.5, edgeDistance);
			    
			    #if VERTICAL_EDGE_SMOOTH_ON
			    float verticalDistance = i.uv.y;
			    float distanceToClosestVerticalEdge = min(1.0-verticalDistance,verticalDistance);
			    float distanceToClosestVerticalEdgePerPixel = fwidth(distanceToClosestVerticalEdge);
			    float verticalEdgeAlpha = smoothstep(0.0, distanceToClosestVerticalEdgePerPixel*aaSmoothing, distanceToClosestVerticalEdge);
			    fillColor.a *= verticalEdgeAlpha;
			    #endif
			     
			     fillColor.a *= edgeAlpha;
			    
                return fillColor;
			}
			ENDCG
		}
	}
}
