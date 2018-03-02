using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSegmentDrawTest : MonoBehaviour
{
	public Transform startPointTransform;
	public Transform endPointTransform;
	[Range(0f,2f)]
	public float lineWidth = 0.5f;
	[Range(0f,6f)]
	public float aaSmoothing = 0.5f;
	public Color color = Color.red;
	
	private void Update()
	{
		if (startPointTransform == null || endPointTransform == null) return;
		
		LineSegment.Draw(startPointTransform.position,endPointTransform.position,LineSegment.FaceCameraForward,lineWidth,color);
	}
}
