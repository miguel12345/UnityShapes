using System;
using UnityEngine;

namespace Shapes
{
	[Serializable]
	public struct LineInfo
	{
		public Vector3 startPos;
		public Vector3 endPos;
		public Color color;
		public Vector3 forward;
		public float width;

		public bool bordered;
		public Color borderColor;
		public float borderWidth;

		public bool dashed;
		public float distanceBetweenDashes;
		public float dashLength;

		public bool startArrow;
		public float startArrowWidth;
		public float startArrowLength;
		
		public bool endArrow;
		public float endArrowWidth;
		public float endArrowLength;
	}
}
