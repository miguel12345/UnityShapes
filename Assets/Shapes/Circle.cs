using System.Collections.Generic;
using UnityEngine;

namespace Shapes
{
	public class Circle
	{	
		private static Mesh _quadMesh;
		private static Material[] _materials = new Material[4];
		public static float antiAliasingSmoothing = 1.5f;

		private const string BorderColorKeyword = "BORDER";
		private const string SectorKeyword = "SECTOR";

		private const string FillColorParam = "_FillColor";
		private const string BorderColorParam = "_BorderColor";
		private const string FillWidthParam = "_FillWidth";
		private const string AASmoothingParam = "_AASmoothing";
		private const string SectorPlaneNormal1 = "_cutPlaneNormal1";
		private const string SectorPlaneNormal2 = "_cutPlaneNormal2";
		private const string SectorAngleBlendMode = "_AngleBlend";
		private static string[][] _materialKeywords = new string[][]
		{
			null,
			new []{"BORDER"}, 
			new []{"SECTOR"}, 
			new []{"BORDER","SECTOR"}, 
		};

		private static MaterialPropertyBlock _materialPropertyBlock;


		static Material GetMaterial(CircleInfo circleInfo)
		{
			var materialIndex = 0;

			if (circleInfo.bordered)
			{
				materialIndex = 1;
			}

			if (circleInfo.isSector)
			{
				materialIndex = 2;
			}

			if (circleInfo.bordered && circleInfo.isSector)
			{
				materialIndex = 3;
			}


			if (_materials[materialIndex] != null)
			{
				return _materials[materialIndex];
			}
			
			var mat = new Material(Shader.Find("Hidden/Shapes/Circle"));
		
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
		
		static MaterialPropertyBlock GetMaterialPropertyBlock(CircleInfo circleInfo)
		{
			if (_materialPropertyBlock == null)
			{
				_materialPropertyBlock = new MaterialPropertyBlock();
			}
		
			_materialPropertyBlock.SetColor(FillColorParam,circleInfo.fillColor);
			_materialPropertyBlock.SetFloat(AASmoothingParam,antiAliasingSmoothing);

			if (circleInfo.bordered)
			{
				_materialPropertyBlock.SetColor(BorderColorParam,circleInfo.borderColor);
				var borderWidthNormalized = circleInfo.borderWidth / circleInfo.radius;
				_materialPropertyBlock.SetFloat(FillWidthParam,1.0f-borderWidthNormalized);
			}

			if (circleInfo.isSector)
			{
				setSectorAngles(_materialPropertyBlock,circleInfo.sectorInitialAngleInDegrees, circleInfo.sectorArcLengthInDegrees);
			}


			return _materialPropertyBlock;
		}

		static Mesh GetCircleMesh()
		{
			if (_quadMesh != null)
			{
				return _quadMesh;
			}

			_quadMesh = CreateQuadMesh();

			return _quadMesh;
		}

		static Matrix4x4 GetTRSMatrix(CircleInfo circleInfo)
		{
			var rotation = Quaternion.LookRotation(circleInfo.forward);
			return Matrix4x4.TRS(circleInfo.center, rotation, new Vector3(circleInfo.radius, circleInfo.radius, 1f));
		}

		public static void Draw(CircleInfo circleInfo)
		{
			var mesh = GetCircleMesh();
			var materialPropertyBlock = GetMaterialPropertyBlock(circleInfo);
			var matrix = GetTRSMatrix(circleInfo);
			var material = GetMaterial(circleInfo);
			
			Graphics.DrawMesh(mesh,matrix,material,0,null,0,materialPropertyBlock);
		}

		static void setSectorAngles(MaterialPropertyBlock block, float initialAngleDegrees, float sectorArcLengthDegrees)
		{
		
			var initialAngleRadians = Mathf.Deg2Rad * initialAngleDegrees;
			var finalAngleRadians = initialAngleRadians+ (Mathf.Deg2Rad * sectorArcLengthDegrees);

			Vector2 cutPlaneNormal1 = new Vector2(Mathf.Sin(initialAngleRadians), -Mathf.Cos(initialAngleRadians));
			Vector2 cutPlaneNormal2 = new Vector2(-Mathf.Sin(finalAngleRadians), Mathf.Cos(finalAngleRadians));
		
			block.SetVector(SectorPlaneNormal1,cutPlaneNormal1);
			block.SetVector(SectorPlaneNormal2,cutPlaneNormal2);
			block.SetFloat(SectorAngleBlendMode,sectorArcLengthDegrees<180f?0f:1f);
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
}
