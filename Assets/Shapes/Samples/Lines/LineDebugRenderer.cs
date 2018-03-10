using UnityEngine;

namespace Shapes.Samples.Lines
{
	public class LineDebugRenderer : MonoBehaviour
	{
		public Transform startPointTransform;
		public Transform endPointTransform;

		public LineInfo lineInfo;
	
		private void Update()
		{
			if (startPointTransform == null || endPointTransform == null) return;

			lineInfo.startPos = startPointTransform.position;
			lineInfo.endPos = endPointTransform.position;

			lineInfo.forward = -Camera.main.transform.forward;
		
			LineSegment.Draw(lineInfo);
		}
	}
}
