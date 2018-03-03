using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDebugRenderer : MonoBehaviour
{
	public Transform startPointTransform;
	public Transform endPointTransform;
	[Range(0f,2f)]
	public float lineWidth = 0.5f;
	[Range(0f,6f)]
	public float aaSmoothing = 0.5f;
	public Color color = Color.red;

	public bool Border;
	public Color BorderColor;
	[Range(0.1f,1f)]
	public float BorderWidth = 0.2f;
	
	private void Update()
	{
		if (startPointTransform == null || endPointTransform == null) return;

		if (Border)
		{
			LineSegment.Draw(startPointTransform.position,endPointTransform.position,LineSegment.FaceCameraForward,lineWidth,color,BorderColor,BorderWidth);
		}
		else
		{
			LineSegment.Draw(startPointTransform.position,endPointTransform.position,LineSegment.FaceCameraForward,lineWidth,color);
		}
		
	}
}
