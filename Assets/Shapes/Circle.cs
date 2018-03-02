using System.Collections.Generic;
using UnityEngine;

public class Circle
{	
	private static Mesh _quadMesh;
	private static Material[] _materials = new Material[4];
	public static float antiAliasingSmoothing = 1.5f;

	private const string BorderColorKeyword = "BORDER_COLOR";
	private const string SectorKeyword = "SECTOR";

	private const string FillColorParam = "_FillColor";
	private const string BorderColorParam = "_BorderColor";
	private const string FillWidthParam = "_FillWidth";
	private const string AASmoothingParam = "_AASmoothing";
	private const string SectorPlaneNormal1 = "_cutPlaneNormal1";
	private const string SectorPlaneNormal2 = "_cutPlaneNormal2";
	private const string SectorAngleBlendMode = "_AngleBlend";

	private static int _borderlessCircleMaterialIndex = 0;
	private static int _borderCircleMaterialIndex = 1;
	private static int _borderlessSectorMaterialIndex = 2;
	private static int _borderSectorMaterialIndex = 3;

	public static void DrawSector(Vector3 center, float radius,Color fillColor, Color borderColor, float borderWidth,
		float initialAngleDegrees, float sectorArcLength)
	{
		if (_quadMesh == null)
		{
			_quadMesh = CreateQuadMesh();
		}

		var material = GetOrCreateCircleMaterial(_borderSectorMaterialIndex, BorderColorKeyword,SectorKeyword);
		
		var materialPropertyBlock = new MaterialPropertyBlock();
		
		materialPropertyBlock.SetColor(FillColorParam,fillColor);
		materialPropertyBlock.SetColor(BorderColorParam,borderColor);
		materialPropertyBlock.SetFloat(FillWidthParam,1.0f-borderWidth);
		materialPropertyBlock.SetFloat(AASmoothingParam,antiAliasingSmoothing);

		setSectorAngles(materialPropertyBlock,initialAngleDegrees, sectorArcLength);

		Graphics.DrawMesh(_quadMesh,Matrix4x4.TRS(center,Quaternion.identity, new Vector3(radius,radius,1f)),material,0,null,0,materialPropertyBlock);
	}
	
	public static void DrawSector(Vector3 center, float radius,Color fillColor,
		float initialAngleDegrees, float sectorArcLength)
	{
		if (_quadMesh == null)
		{
			_quadMesh = CreateQuadMesh();
		}

		var material = GetOrCreateCircleMaterial(_borderlessSectorMaterialIndex,SectorKeyword);
		
		var materialPropertyBlock = new MaterialPropertyBlock();
		
		materialPropertyBlock.SetColor(FillColorParam,fillColor);
		materialPropertyBlock.SetFloat(AASmoothingParam,antiAliasingSmoothing);

		setSectorAngles(materialPropertyBlock,initialAngleDegrees, sectorArcLength);

		Graphics.DrawMesh(_quadMesh,Matrix4x4.TRS(center,Quaternion.identity, new Vector3(radius,radius,1f)),material,0,null,0,materialPropertyBlock);
	}

	static void setSectorAngles(MaterialPropertyBlock block, float initialAngleDegrees, float sectorArcLengthDegrees)
	{
		
		var initialAngleRadians = Mathf.Deg2Rad * initialAngleDegrees;
		var finalAngleRadians = initialAngleRadians+ (Mathf.Deg2Rad * sectorArcLengthDegrees);

		Vector2 cutPlaneNormal1 = new Vector2(Mathf.Sin(initialAngleRadians), -Mathf.Cos(initialAngleRadians));
		Vector2 cutPlaneNormal2 = new Vector2(-Mathf.Sin(finalAngleRadians), Mathf.Cos(finalAngleRadians));
		
		//Convert from 0 -> 360° to -PI -> PI
		
		block.SetVector(SectorPlaneNormal1,cutPlaneNormal1);
		block.SetVector(SectorPlaneNormal2,cutPlaneNormal2);
		block.SetFloat(SectorAngleBlendMode,sectorArcLengthDegrees<180f?0f:1f);
	}
	
	public static void Draw(Vector3 center, float radius,Color fillColor, Color borderColor,float borderWidth)
	{
		if (_quadMesh == null)
		{
			_quadMesh = CreateQuadMesh();
		}

		var material = GetOrCreateCircleMaterial(_borderCircleMaterialIndex, BorderColorKeyword);
		
		var materialPropertyBlock = new MaterialPropertyBlock();
		
		materialPropertyBlock.SetColor(FillColorParam,fillColor);
		materialPropertyBlock.SetColor(BorderColorParam,borderColor);
		materialPropertyBlock.SetFloat(FillWidthParam,1.0f-borderWidth);
		materialPropertyBlock.SetFloat(AASmoothingParam,antiAliasingSmoothing);

		Graphics.DrawMesh(_quadMesh,Matrix4x4.TRS(center,Quaternion.identity, new Vector3(radius,radius,1f)),material,0,null,0,materialPropertyBlock);
	}
	
	public static void Draw(Vector3 center, float radius,Color fillColor)
	{
		if (_quadMesh == null)
		{
			_quadMesh = CreateQuadMesh();
		}

		var material = GetOrCreateCircleMaterial(_borderlessCircleMaterialIndex);
		
		var materialPropertyBlock = new MaterialPropertyBlock();
		
		materialPropertyBlock.SetColor(FillColorParam,fillColor);
		materialPropertyBlock.SetFloat(AASmoothingParam,antiAliasingSmoothing);

		Graphics.DrawMesh(_quadMesh,Matrix4x4.TRS(center,Quaternion.identity, new Vector3(radius,radius,1f)),material,0,null,0,materialPropertyBlock);
	}

	private static Material GetOrCreateCircleMaterial(int materialIndex,params string[] keywords)
	{
		if (_materials[materialIndex] != null)
		{
			return _materials[materialIndex];
		}
		
		var mat = new Material(Shader.Find("Hidden/Shapes/Circle"));
		foreach (var keyword in keywords)
		{
			mat.EnableKeyword(keyword);
		}

		_materials[materialIndex] = mat;
		
		return mat;
	}
	
	private static Mesh CreateQuadMesh()
	{
		var quadMesh = new Mesh();
		quadMesh.SetVertices(new List<Vector3>
		{
			new Vector3(-1f, -1f, 0f),
			new Vector3(1f, -1f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(-1f, 1f, 0f)
		});

		quadMesh.triangles = new[]
		{
			0, 2, 1,
			0, 3, 2
		};

		var uvMin = -1f;
		var uvMax = 1f;
		
		quadMesh.uv = new[]
		{
			new Vector2(uvMin, uvMin),
			new Vector2(uvMax, uvMin),
			new Vector2(uvMax, uvMax),
			new Vector2(uvMin, uvMax)
		};

		return quadMesh;
	}
}
