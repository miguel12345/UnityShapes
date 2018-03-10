using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shapes
{
	public static class LineSegment
	{
		private static Mesh _lineSegmentMesh;
		private static Mesh _arrowHeadMesh;
		private static Material _lineSegmentMaterial;
		private static Matrix4x4 _cacheMatrix = Matrix4x4.identity;
		private static MaterialPropertyBlock _materialPropertyBlock;
		public static float antiAliasingSmoothing = 1.5f;
		private static Material[] _lineMaterials = new Material[8];
		private static string[][] _materialKeywords = new string[][]
		{
			null,
			new []{"BORDER"}, 
			new []{"DASHED"}, 
			new []{"BORDER","DASHED"}, 
			new []{"VERTICAL_EDGE_SMOOTH_OFF"},
			new []{"BORDER","VERTICAL_EDGE_SMOOTH_OFF"},
			new []{"DASHED","VERTICAL_EDGE_SMOOTH_OFF"},
			new []{"BORDER","DASHED","VERTICAL_EDGE_SMOOTH_OFF"}, 
		};
		
		private static Material _arrowHeadMaterial = null;

		private static string _colorParam = "_Color";
		private static string _antiAliasingSmoothingParam = "_AASmoothing";
		private static string _fillWidthParam = "_FillWidth";
		private static string _borderColorParam = "_BorderColor";
		private static string _lineLengthParam = "_LineLength";
		private static string _distanceBetweenDashesParam = "_DistanceBetweenDashes";
		private static string _dashWidthParam = "_DashWidth";
		private static string _arrowHeadBaseEdgeNoAAMinX = "_BaseEdgeNoAAMinX";
		private static string _arrowHeadBaseEdgeNoAAMaxX = "_BaseEdgeNoAAMaxX";

		public static Vector3 FaceCameraForward
		{
			get { return -Camera.main.transform.forward; }
		}

		static Material GetLineMaterial(LineInfo lineInfo)
		{
			int materialIndex = 0;
			bool hasArrows = lineInfo.startArrow || lineInfo.endArrow;

			if (lineInfo.bordered)
			{
				materialIndex = 1;
			}
			
			if (lineInfo.dashed)
			{
				materialIndex = 2;
			}
			
			if (lineInfo.bordered && lineInfo.dashed)
			{
				materialIndex = 3;
			}

			if (hasArrows)
			{
				materialIndex = 4;
			}
			
			if (hasArrows && lineInfo.bordered)
			{
				materialIndex = 5;
			}
			
			if (hasArrows && lineInfo.dashed)
			{
				materialIndex = 6;
			}
			
			if (hasArrows && lineInfo.bordered && lineInfo.dashed)
			{
				materialIndex = 7;
			}

			if (_lineMaterials[materialIndex] == null)
			{
				var material = new Material(Shader.Find("Hidden/Shapes/LineSegment"));
				
				if (SystemInfo.supportsInstancing)
				{
					material.enableInstancing = true;
				}
				
				if (_materialKeywords[materialIndex] != null)
				{
					material.shaderKeywords = _materialKeywords[materialIndex];
				}
				_lineMaterials[materialIndex] = material;
				return material;
			}

			return _lineMaterials[materialIndex];
		}

		static Mesh GetLineMesh(LineInfo lineInfo)
		{
			if (_lineSegmentMesh == null)
			{
				_lineSegmentMesh = CreateLineSegmentMesh();
			}

			return _lineSegmentMesh;
		}

		static MaterialPropertyBlock GetMaterialPropertyBlock(LineInfo lineInfo)
		{
			if (_materialPropertyBlock == null)
			{
				_materialPropertyBlock = new MaterialPropertyBlock();
			}
			else
			{
				_materialPropertyBlock.Clear();
			}

			return _materialPropertyBlock;
		}

		static Material GetArrowHeadMaterial()
		{
			if (_arrowHeadMaterial == null)
			{
				_arrowHeadMaterial = new Material(Shader.Find("Hidden/Shapes/LineSegment/ArrowHead"));
			}

			return _arrowHeadMaterial;
		}

		static Mesh GetArrowHeadMesh()
		{
			if (_arrowHeadMesh == null)
			{
				_arrowHeadMesh = createArrowHeadMesh();
			}

			return _arrowHeadMesh;
		}

		
		
		private static Mesh createArrowHeadMesh()
		{
			var quadMesh = new Mesh();
		
			quadMesh.SetVertices(new List<Vector3>
			{
				new Vector3(-0.5f, 0.0f, 0f),
				new Vector3(0.5f, 0.0f, 0f),
				new Vector3(0.0f, 1.0f, 0f),
			});

			quadMesh.triangles = new[]
			{
				0, 1, 2,
			};
		
			quadMesh.uv = new[]
			{
				new Vector2(-0.5f, 0.0f),
				new Vector2(0.5f, 0.0f),
				new Vector2(0.0f, 1.0f),
			};

			quadMesh.colors32 = new[]
			{
				new Color32(Byte.MaxValue, 0, 0, 0),
				new Color32(0, Byte.MaxValue, 0, 0),
				new Color32(0, 0, Byte.MaxValue, 0),
			};

			return quadMesh;
		}

		static void FillPropertyBlock(MaterialPropertyBlock block, LineInfo lineInfo,float lineSegmentLength)
		{
			block.SetColor(_colorParam,lineInfo.fillColor);
			block.SetFloat(_antiAliasingSmoothingParam,antiAliasingSmoothing);

			if (lineInfo.bordered)
			{
				block.SetColor(_borderColorParam,lineInfo.borderColor);
				var borderWidthNormalized = lineInfo.borderWidth / lineInfo.width;
				block.SetFloat(_fillWidthParam,0.5f-borderWidthNormalized);
			}

			if (lineInfo.dashed)
			{
				block.SetFloat(_lineLengthParam,lineSegmentLength);
				block.SetFloat(_distanceBetweenDashesParam,lineInfo.distanceBetweenDashes);
				block.SetFloat(_dashWidthParam,lineInfo.dashLength);
			}

			if (lineInfo.startArrow || lineInfo.endArrow)
			{
				float baseEdgeNoAAMaxX = (lineInfo.width / lineInfo.arrowWidth) * 0.5f;
				float baseEdgeNoAAMinX = -baseEdgeNoAAMaxX;
				block.SetFloat(_arrowHeadBaseEdgeNoAAMinX,baseEdgeNoAAMinX);
				block.SetFloat(_arrowHeadBaseEdgeNoAAMaxX,baseEdgeNoAAMaxX);
			}
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

			var uvXLeft = 0.5f;
			var uvXCenter = 0f;
			var uvXRight = 0.5f;

			var uvYBottom = 0f;
			var uvYTop = 1f;
		
			quadMesh.uv = new[]
			{
				new Vector2(uvXLeft, 	uvYBottom),
				new Vector2(uvXCenter, 	uvYBottom),
				new Vector2(uvXRight, 	uvYBottom),
				new Vector2(uvXLeft, 	uvYTop),
				new Vector2(uvXCenter, 	uvYTop),
				new Vector2(uvXRight, 	uvYTop)
			};

			return quadMesh;
		}
		
		static Matrix4x4 GetLineTRSMatrix(Vector3 startPos, Vector3 endPos,Vector3 forward, float width,out float lineLength)
		{
			lineLength = Vector3.Distance(endPos,startPos);

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

		static void DrawArrowHead(Vector3 startPos, Vector3 endPos, Vector3 forward, float width, MaterialPropertyBlock materialPropertyBlock)
		{
			float arrowHeadHeight;
			//TODO reuse matrix from line
			var matrix = GetLineTRSMatrix(startPos, endPos, forward, width,out arrowHeadHeight);

			var arrowHeadMesh = GetArrowHeadMesh();
			var arrowHeadMaterial = GetArrowHeadMaterial();
			
			Graphics.DrawMesh(arrowHeadMesh,matrix,arrowHeadMaterial,0,null,0,materialPropertyBlock);
		}


		public static void Draw(LineInfo lineInfo)
		{
			var lineMaterial = GetLineMaterial(lineInfo);
			var lineSegmentMesh = GetLineMesh(lineInfo); 
			var materialPropertyBlock = GetMaterialPropertyBlock(lineInfo);

			var lineSegmentStartPos = lineInfo.startPos;
			var lineSegmentEndPos = lineInfo.endPos;

			var lineDirection = (lineSegmentEndPos - lineSegmentStartPos).normalized;
			
			if (lineInfo.startArrow)
			{
				lineSegmentStartPos = lineSegmentStartPos + lineDirection * lineInfo.arrowLength;
			}
			
			if (lineInfo.endArrow)
			{
				lineSegmentEndPos = lineSegmentEndPos - lineDirection * lineInfo.arrowLength;
			}
			
			var forward = lineInfo.forward;
			var width = lineInfo.width;
			float lineLength;

			var matrix = GetLineTRSMatrix(lineSegmentStartPos, lineSegmentEndPos, forward, width,out lineLength);

			FillPropertyBlock(materialPropertyBlock, lineInfo, lineLength);
			
			Graphics.DrawMesh(lineSegmentMesh,matrix,lineMaterial,0,null,0,materialPropertyBlock);


			if (lineInfo.endArrow)
			{
				DrawArrowHead(lineSegmentEndPos, lineInfo.endPos, forward, lineInfo.arrowWidth,materialPropertyBlock);
			}
			
			if (lineInfo.startArrow)
			{
				DrawArrowHead(lineSegmentStartPos,lineInfo.startPos, forward, lineInfo.arrowWidth,materialPropertyBlock);
			}
		}
	}
}
