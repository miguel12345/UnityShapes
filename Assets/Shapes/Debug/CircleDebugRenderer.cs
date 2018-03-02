using UnityEngine;

namespace Shapes.Debug
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
			if (Border)
			{
				if (Sector)
				{
					Circle.DrawSector(transform.position,Radius,FillColor,BorderColor,BorderWidth,InitialAngle,ArcLength);
				}
				else
				{
					Circle.Draw(transform.position,Radius,FillColor,BorderColor,BorderWidth);
				}
			}
			else
			{
				if (Sector)
				{
					Circle.DrawSector(transform.position,Radius,FillColor,InitialAngle,ArcLength);
				}
				else
				{
					Circle.Draw(transform.position,Radius,FillColor);
				}
			}
		}
	}
}
