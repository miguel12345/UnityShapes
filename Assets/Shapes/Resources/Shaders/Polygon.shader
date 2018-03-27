Shader "Hidden/Shapes/Polygon"
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
            #pragma multi_compile_instancing
            #pragma multi_compile _ BORDER

			#include "UnityCG.cginc"

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
                
                float aaSmoothing = UNITY_ACCESS_INSTANCED_PROP(CommonProps, _AASmoothing);
			    fixed4 fillColor = UNITY_ACCESS_INSTANCED_PROP(CommonProps, _FillColor);
			    
			    float distanceToCenter = i.uv.x;
			    
			    float distancePerPixel = fwidth(distanceToCenter);
			    float distanceAlphaFactor = 1.0 - smoothstep(1.0-distancePerPixel*aaSmoothing,1.0,distanceToCenter);
			                    
                #if BORDER
			    float fillWidth = UNITY_ACCESS_INSTANCED_PROP(BorderProps, _FillWidth);
			    fixed4 borderColor = UNITY_ACCESS_INSTANCED_PROP(BorderProps, _BorderColor);
			    
			    float fillToBlendColorLerpFactor = smoothstep(fillWidth,fillWidth + distancePerPixel*aaSmoothing,distanceToCenter);
			    fillColor = lerp(fillColor,borderColor,fillToBlendColorLerpFactor);
			    #endif
			    
			    fillColor.a *= distanceAlphaFactor;
			    
			    return fillColor;
			}
			ENDCG
		}
	}
}
