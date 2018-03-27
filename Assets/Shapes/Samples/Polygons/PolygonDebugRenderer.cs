using UnityEngine;

namespace Shapes.Samples.Polygons
{
	public class PolygonDebugRenderer : MonoBehaviour
	{
		public PolygonInfo polygonInfo;


		private void Update()
		{
			polygonInfo.center = transform.position;
			polygonInfo.rotation = transform.rotation;

			if (polygonInfo.sides < 3)
			{
				polygonInfo.sides = 3;
			}
			
			Polygon.Draw(polygonInfo);
		}
	}
}
