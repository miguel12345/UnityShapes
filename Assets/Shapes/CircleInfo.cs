using UnityEngine;

namespace Shapes
{
	public struct CircleInfo
	{
		public float radius;
		public Vector3 center;
		public Vector3 forward;

		public Color fillColor;
		
		public bool bordered;
		public Color borderColor;
		public float borderWidth;

		public bool isSector;
		public float sectorInitialAngleInDegrees;
		public float sectorArcLengthInDegrees;
	}
}
