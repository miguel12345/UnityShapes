Shader "Hidden/Shapes/Circle"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" "DisableBatching" ="true" }
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
			#pragma multi_compile _ SECTOR
            #pragma multi_compile_instancing
			
			#include "UnityCG.cginc"

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

            UNITY_INSTANCING_BUFFER_START(CommonProps)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _FillColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _AASmoothing)
            UNITY_INSTANCING_BUFFER_END(CommonProps)

			#if BORDER
			UNITY_INSTANCING_BUFFER_START(BorderProps)
			     UNITY_DEFINE_INSTANCED_PROP(fixed4, _BorderColor)
			     UNITY_DEFINE_INSTANCED_PROP(float, _FillWidth)
			UNITY_INSTANCING_BUFFER_END(BorderProps)
			#endif
			
		    #if SECTOR
			UNITY_INSTANCING_BUFFER_START(SectorProps)
			     UNITY_DEFINE_INSTANCED_PROP(float4, _cutPlaneNormal1)
			     UNITY_DEFINE_INSTANCED_PROP(float4, _cutPlaneNormal2)
			     UNITY_DEFINE_INSTANCED_PROP(float, _AngleBlend)
			UNITY_INSTANCING_BUFFER_END(SectorProps)
            #endif
            
			v2f vert (appdata v)
			{
				v2f o;
				
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.vertex.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			
			    UNITY_SETUP_INSTANCE_ID(i);
			    
			    float aaSmoothing = UNITY_ACCESS_INSTANCED_PROP(CommonProps, _AASmoothing);
			    fixed4 fillColor = UNITY_ACCESS_INSTANCED_PROP(CommonProps, _FillColor);
			    
			    float distanceToCenter = length(i.uv);
			    
			    float distancePerPixel = fwidth(distanceToCenter);
			    float distanceAlphaFactor = 1.0 - smoothstep(1.0-distancePerPixel*aaSmoothing,1.0,distanceToCenter);
			    float halfSmoothFactor = 0.5f * distancePerPixel * aaSmoothing;
			    
			    #if BORDER
			    float fillWidth = UNITY_ACCESS_INSTANCED_PROP(BorderProps, _FillWidth);
			    fixed4 borderColor = UNITY_ACCESS_INSTANCED_PROP(BorderProps, _BorderColor);
			    
			    float fillToBorder = smoothstep(fillWidth-halfSmoothFactor,fillWidth+halfSmoothFactor,distanceToCenter);
			    fixed4 circleColor = lerp(fillColor,borderColor,fillToBorder);
			    #else
			    fixed4 circleColor = fillColor;
			    #endif
			    
			    circleColor.a *= distanceAlphaFactor;
			    
			    #if SECTOR
			    
			    float4 cutPlaneNormal1 = UNITY_ACCESS_INSTANCED_PROP(SectorProps, _cutPlaneNormal1);
			    float4 cutPlaneNormal2 = UNITY_ACCESS_INSTANCED_PROP(SectorProps, _cutPlaneNormal2);
			    float angleBlend = UNITY_ACCESS_INSTANCED_PROP(SectorProps, _AngleBlend);
			    
			    float2 pos = float2(i.uv.x,i.uv.y);
			    
			    float distanceToPlane1 = dot(pos,cutPlaneNormal1);
			    float distanceToPlane1PerPixel = fwidth(distanceToPlane1);
			    float distanceToPlane1Alpha = 1.0 - smoothstep(0,0+distanceToPlane1PerPixel*aaSmoothing ,distanceToPlane1);
			    
			    float distanceToPlane2 = dot(pos,cutPlaneNormal2);
			    float distanceToPlane2PerPixel = fwidth(distanceToPlane2);
			    float distanceToPlane2Alpha = 1.0 - smoothstep(0,0+distanceToPlane2PerPixel*aaSmoothing ,distanceToPlane2);
			    
			    if(angleBlend == 1){ //OR
			        circleColor.a *= max(distanceToPlane1Alpha, distanceToPlane2Alpha);
			    }
			    else { //AND
			        circleColor.a *= distanceToPlane1Alpha * distanceToPlane2Alpha;
			    }
			   
			    #endif
			    
                return circleColor;
			}
			ENDCG
		}
	}
}
