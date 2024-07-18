using System.Collections;
using System.Collections.Generic;
using RayTracingObjects;
using UnityEngine;

public class IncludeInRaytracer : MonoBehaviour
{
    [SerializeField] TracerObjectScriptableObject scriptableObject;

    public static implicit operator TracerObject(IncludeInRaytracer t) => t.scriptableObject.tracerObject;

    public TracerObject ToTracerObjectFromGameObject() {
        Renderer renderer = GetComponent<Renderer>();
        Vector3 albedo = new Vector3(renderer.sharedMaterial.color.b, renderer.sharedMaterial.color.g, renderer.sharedMaterial.color.b);
        return new TracerObject(
            transform.position,
            transform.lossyScale.x * 0.5f,
            new TracerMaterial(
                albedo,
                albedo * renderer.sharedMaterial.GetFloat("_Glossiness")
            )
        );
    }
}
