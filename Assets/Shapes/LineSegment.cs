using System.Collections.Generic;
using UnityEngine;

public class LineSegment
{
	private static Mesh _lineSegmentMesh;
	private static Material _lineSegmentMaterial;
	private static Matrix4x4 _cacheMatrix = Matrix4x4.identity;
	private static MaterialPropertyBlock _materialPropertyBlock;
	public static float antiAliasingSmoothing = 1.5f;

	public static Vector3 FaceCameraForward
	{
		get { return -Camera.main.transform.forward; }
	}
	
	public static void Draw(Vector3 startPos, Vector3 endPos,Vector3 forward, float width, Color color)
	{
		if (_lineSegmentMesh == null)
		{
			_lineSegmentMesh = CreateLineSegmentMesh();
		}

		if (_lineSegmentMaterial == null)
		{
			_lineSegmentMaterial = CreateLineSegmentMaterial();
		}

		if (_materialPropertyBlock == null)
		{
			_materialPropertyBlock = new MaterialPropertyBlock();
		}

		var lineLength = Vector3.Distance(endPos,startPos);

		var up = (endPos - startPos).normalized;

		forward = forward - Vector3.Dot(forward, up) * up;
		forward.Normalize();

		var right = Vector3.Cross(up, forward);

		right.Normalize();

		var mat = _cacheMatrix;
		
		//Orthonormal basis
		mat.SetColumn(0,right * width);//equivalent to mat.SetColumn(0,right) followed by a mat *= Matrix4x4.Scale(new Vector3(width, 1f, 1f));
		mat.SetColumn(1,up * lineLength); //equivalent to mat.SetColumn(1,up) followed by a mat *= Matrix4x4.Scale(new Vector3(1f, lineLength, 1f));
		mat.SetColumn(2,forward);
		
		//origin translation
		Vector4 translation = startPos;
		translation.w = 1f;
		mat.SetColumn(3,translation);
		
		_materialPropertyBlock.SetColor("_Color",color);
		_materialPropertyBlock.SetFloat("_AASmoothing",antiAliasingSmoothing);
		
		Graphics.DrawMesh(_lineSegmentMesh,mat,_lineSegmentMaterial,0,null,0,_materialPropertyBlock);
	}
	
	private static Material CreateLineSegmentMaterial()
	{
		return new Material(Shader.Find("Hidden/Shapes/LineSegment"));
	}
	
	private static Mesh CreateLineSegmentMesh()
	{
		var quadMesh = new Mesh();
		
		var xLeft = -0.5f;
		var xCenter = 0f;
		var xRight = 0.5f;

		var yBottom = 0f;
		var yTop = 1f;
		
		quadMesh.SetVertices(new List<Vector3>
		{
			new Vector3(xLeft, yBottom, 0f),
			new Vector3(xCenter, yBottom, 0f),
			new Vector3(xRight, yBottom, 0f),
			
			new Vector3(xLeft, yTop, 0f),
			new Vector3(xCenter, yTop, 0f),
			new Vector3(xRight, yTop, 0f),
		});

		quadMesh.triangles = new[]
		{
			0, 1, 4,
			4, 3, 0,
			
			1, 2, 5,
			5, 4, 1,
		};

		var uvLeft = 0.5f;
		var uvCenter = 0f;
		var uvRight = 0.5f;
		
		quadMesh.uv = new[]
		{
			new Vector2(uvLeft, 	0f),
			new Vector2(uvCenter, 	0f),
			new Vector2(uvRight, 	0f),
			new Vector2(uvLeft, 	0f),
			new Vector2(uvCenter, 	0f),
			new Vector2(uvRight, 	0f)
		};

		return quadMesh;
	}
}
