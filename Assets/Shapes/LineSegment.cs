using System.Collections.Generic;
using UnityEngine;

public class LineSegment
{
	private static Mesh _lineSegmentMesh;
	private static Material _lineSegmentMaterial;
	private static Matrix4x4 _cacheMatrix = Matrix4x4.identity;
	private static MaterialPropertyBlock _materialPropertyBlock;
	public static float antiAliasingSmoothing = 1.5f;
	private static Material[] _materials = new Material[4];

	private static string _colorParam = "_Color";
	private static string _antiAliasingSmoothingParam = "_AASmoothing";
	private static string _fillWidthParam = "_FillWidth";
	private static string _borderColorParam = "_BorderColor";


	public static Vector3 FaceCameraForward
	{
		get { return -Camera.main.transform.forward; }
	}

	static void Setup(int materialIndex, params string[] materialKeywords)
	{
		if (_lineSegmentMesh == null)
		{
			_lineSegmentMesh = CreateLineSegmentMesh();
		}

		_lineSegmentMaterial = GetOrCreateMaterial(materialIndex,materialKeywords);

		if (_materialPropertyBlock == null)
		{
			_materialPropertyBlock = new MaterialPropertyBlock();
		}
	}

	static Matrix4x4 GetLineTRSMatrix(Vector3 startPos, Vector3 endPos,Vector3 forward, float width)
	{
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

		return mat;
	}
	
	public static void Draw(Vector3 startPos, Vector3 endPos,Vector3 forward, float width, Color color)
	{
		Setup(0);

		var mat = GetLineTRSMatrix(startPos, endPos, forward, width);
		
		_materialPropertyBlock.SetColor(_colorParam,color);
		_materialPropertyBlock.SetFloat(_antiAliasingSmoothingParam,antiAliasingSmoothing);
		
		Graphics.DrawMesh(_lineSegmentMesh,mat,_lineSegmentMaterial,0,null,0,_materialPropertyBlock);
	}
	
	
	public static void Draw(Vector3 startPos, Vector3 endPos,Vector3 forward, float width, 
		Color color, Color borderColor, float borderWidth)
	{
		Setup(1,"BORDER");

		var mat = GetLineTRSMatrix(startPos, endPos, forward, width);
		
		_materialPropertyBlock.SetColor(_colorParam,color);
		_materialPropertyBlock.SetFloat(_antiAliasingSmoothingParam,antiAliasingSmoothing);
		_materialPropertyBlock.SetColor(_borderColorParam,borderColor);
		float borderWidthNormalized = borderWidth / width;
		_materialPropertyBlock.SetFloat(_fillWidthParam,0.5f-borderWidthNormalized);
		
		Graphics.DrawMesh(_lineSegmentMesh,mat,_lineSegmentMaterial,0,null,0,_materialPropertyBlock);
	}
	
	private static Material GetOrCreateMaterial(int materialIndex,params string[] keywords)
	{
		if (_materials[materialIndex] != null)
		{
			return _materials[materialIndex];
		}
		
		var mat = new Material(Shader.Find("Hidden/Shapes/LineSegment"));
		if (SystemInfo.supportsInstancing)
		{
			mat.enableInstancing = true;
		}
		foreach (var keyword in keywords)
		{
			mat.EnableKeyword(keyword);
		}

		_materials[materialIndex] = mat;
		
		return mat;
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
