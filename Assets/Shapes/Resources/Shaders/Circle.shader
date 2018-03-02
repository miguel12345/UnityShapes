Shader "Hidden/Shapes/Circle"
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
		    //Cull off
		    Blend SrcAlpha OneMinusSrcAlpha
		    
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile _ BORDER_COLOR 
			#pragma multi_compile _ SECTOR
			
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

			fixed4 _FillColor;
            float _AASmoothing;
			
			#if BORDER_COLOR
			fixed4 _BorderColor;
			float _FillWidth;
			#endif
			
		    #if SECTOR
            float4 _cutPlaneNormal1;
            float4 _cutPlaneNormal2;
            float _AngleBlend;
            #endif
            
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.vertex.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			
			    float distanceToCenter = length(i.uv);
			    
			    float distancePerPixel = fwidth(distanceToCenter);
			    float distanceAlphaFactor = 1.0 - smoothstep(1.0-distancePerPixel*_AASmoothing,1.0,distanceToCenter);
			    
			    #if BORDER_COLOR
			    float fillToBorder = smoothstep(_FillWidth,_FillWidth+distancePerPixel*_AASmoothing,distanceToCenter);
			    fixed4 circleColor = lerp(_FillColor,_BorderColor,fillToBorder);
			    #else
			    fixed4 circleColor = _FillColor;
			    #endif
			    
			    circleColor.a *= distanceAlphaFactor;
			    
			    #if SECTOR
			    
			    float2 pos = float2(i.uv.x,i.uv.y);
			    
			    float distanceToPlane1 = dot(pos,_cutPlaneNormal1);
			    float distanceToPlane1PerPixel = fwidth(distanceToPlane1);
			    float distanceToPlane1Alpha = 1.0 - smoothstep(0,0+distanceToPlane1PerPixel*_AASmoothing ,distanceToPlane1);
			    
			    float distanceToPlane2 = dot(pos,_cutPlaneNormal2);
			    float distanceToPlane2PerPixel = fwidth(distanceToPlane2);
			    float distanceToPlane2Alpha = 1.0 - smoothstep(0,0+distanceToPlane2PerPixel*_AASmoothing ,distanceToPlane2);
			    
			    if(_AngleBlend == 1){ //OR
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
