using System.Collections;
using System.Collections.Generic;
using RayTracingObjects;
using UnityEngine;

public class RayTracerLight : MonoBehaviour
{
    [SerializeField] float intensity;
    [SerializeField] Color color;
    public TracerLight tracerLight {
        get {
            return new TracerLight() {
                position = transform.position,
                orientation = transform.forward,
                intensity = intensity,
                color = new Vector3(color.r, color.g, color.b)
            };
        }
    }

    public Vector4 toVector4() {
        return new Vector4(
            transform.forward.x, 
            transform.forward.y,
            transform.forward.z, 
            intensity
        );
    }
}
