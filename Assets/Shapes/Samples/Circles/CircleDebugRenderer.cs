using UnityEngine;

namespace Shapes.Samples.Circles
{
	
	public class CircleDebugRenderer : MonoBehaviour {
		
		public Color FillColor;

		public bool Border;
		
		public Color BorderColor;
		[Range(0f,0.8f)]
		public float BorderWidth = 0.2f;

		[Range(1f, 10f)] public float Radius = 2f;


		public bool Sector;

		[Range(0f,360f)]
		public float InitialAngle;
		[Range(0f,360f)]
		public float ArcLength=360f;
		
		private void Update()
		{

			var circleInfo = new CircleInfo
			{
				center = transform.position,
				forward = transform.forward,
				radius = Radius,
				fillColor = FillColor
			};


			if (Border)
			{
				circleInfo.bordered = true;
				circleInfo.borderColor = BorderColor;
				circleInfo.borderWidth = BorderWidth;
			}

			if (Sector)
			{
				circleInfo.isSector = true;
				circleInfo.sectorInitialAngleInDegrees = InitialAngle;
				circleInfo.sectorArcLengthInDegrees = ArcLength;
			}
			
			
			Circle.Draw(circleInfo);
		}
	}
}
