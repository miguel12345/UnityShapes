using System;
using UnityEngine;

namespace Shapes
{
    [Serializable]
    public struct PolygonInfo
    {
        public int sides;
        public Vector3 center;
        public float size;

        public Color color;
        
        public bool bordered;
        public float borderWidth;
        public Color borderColor;

        public Quaternion rotation;
    }
}
