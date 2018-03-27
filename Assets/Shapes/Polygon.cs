using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shapes
{
	public class Polygon
	{
		public static float antiAliasingSmoothing = 1.5f;
		
		private static MaterialPropertyBlock _materialPropertyBlock;
		private static Material _material;
		
		private const string FillColorParam = "_FillColor";
		private const string AASmoothingParam = "_AASmoothing";
		private const string BorderColorParam = "_BorderColor";
		private const string FillWidthParam = "_FillWidth";
		
		private static string[][] _materialKeywords = new string[][]
		{
			null,
			new []{"BORDER"},
		};
		private static Material[] _materials = new Material[4];

		static Mesh GenerateMeshPolygon(PolygonInfo polygonInfo)
		{
			var polygonMesh = new Mesh();

			var vertices = new List<Vector3> {Vector3.zero};


			var angleIncrement = (Mathf.PI * 2) / polygonInfo.sides;
			
			for (var currentAngle = 0f; currentAngle < (Mathf.PI * 2); currentAngle+=angleIncrement)
			{
				var x = Mathf.Cos(currentAngle);
				var y = Mathf.Sin(currentAngle);
				
				vertices.Add(new Vector3(x,y,0f));
			}
			
			var triangles = new int[(vertices.Count-1) * 3];

			var triangleIndex = 0;
			for (var vertexIndex = 1; vertexIndex < vertices.Count; vertexIndex++)
			{
				triangles[triangleIndex] = 0;
				triangles[triangleIndex+1] = vertexIndex;
				triangles[triangleIndex+2] = vertexIndex+1;

				if ((vertexIndex + 1) >= vertices.Count)
				{
					triangles[triangleIndex+2] = 1;
				}

				triangleIndex += 3;
			}
		
			polygonMesh.SetVertices(vertices);

			polygonMesh.triangles = triangles;

			var uv = new Vector2[vertices.Count];

			for (var uvIndex = 1; uvIndex < uv.Length; uvIndex++)
			{
				uv[uvIndex] = Vector2.one;
			}

			polygonMesh.uv = uv;
			
			return polygonMesh;
		}

		static MaterialPropertyBlock GetMaterialPropertyBlock(PolygonInfo polygonInfo)
		{
			if (_materialPropertyBlock == null)
			{
				_materialPropertyBlock = new MaterialPropertyBlock();
			}
			
			_materialPropertyBlock.SetColor(FillColorParam,polygonInfo.color);
			_materialPropertyBlock.SetFloat(AASmoothingParam,antiAliasingSmoothing);

			if (polygonInfo.bordered)
			{
				_materialPropertyBlock.SetColor(BorderColorParam,polygonInfo.borderColor);
				var borderWidthNormalized = polygonInfo.borderWidth / polygonInfo.size;
				_materialPropertyBlock.SetFloat(FillWidthParam,1.0f-borderWidthNormalized);
			}
			
			return _materialPropertyBlock;
		}
		
		static Material GetMaterial(PolygonInfo polygonInfo)
		{
			
			var materialIndex = 0;

			if (polygonInfo.bordered)
			{
				materialIndex = 1;
			}

			if (_materials[materialIndex] != null)
			{
				return _materials[materialIndex];
			}
			
			var mat = new Material(Shader.Find("Hidden/Shapes/Polygon"));
		
			if (SystemInfo.supportsInstancing)
			{
				mat.enableInstancing = true;
			}

			var keywords = _materialKeywords[materialIndex];

			if (keywords != null)
			{
				mat.shaderKeywords = keywords;
			}
			

			_materials[materialIndex] = mat;

			return mat;
		}
		
		public static void Draw(PolygonInfo polygonInfo)
		{
			if (polygonInfo.sides < 2)
			{
				throw new ArgumentException("Polygon must have at least 3 sides");
			}

			var polygonMesh = GenerateMeshPolygon(polygonInfo);

			var rotation = polygonInfo.rotation;
			var polygonMatrix = Matrix4x4.TRS(polygonInfo.center, rotation, new Vector3(polygonInfo.size, polygonInfo.size, 1f));

			var materialPropertyBlock = GetMaterialPropertyBlock(polygonInfo);
			var material = GetMaterial(polygonInfo);
			
			Graphics.DrawMesh(polygonMesh,polygonMatrix,material,0,null,0,materialPropertyBlock);
		}
	}
}
