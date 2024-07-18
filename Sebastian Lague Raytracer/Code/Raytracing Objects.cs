using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEditor;
using System.Diagnostics.CodeAnalysis;
using RayTracingObjects;

namespace RayTracingObjects {    
    public struct TracerLight {
        public Vector3 position;
        public Vector3 orientation;
        public Vector3 color;
        public float intensity;

        public TracerLight(Vector3 position, Vector3 orientation, Vector3 color, float intensity) {
            this.position = position;
            this.orientation = orientation;
            this.color = color;
            this.intensity = intensity;
        }

        public static int Size() => sizeof(float) * 10;
    }
}

public class RayTracerObject : MonoBehaviour {

    public TracerObjectScriptableObject data;
}