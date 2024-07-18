using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteAlways, ImageEffectAllowedInSceneView]
public class RaytracerManager : MonoBehaviour
{
    [SerializeField] ComputeShader shader;
    [SerializeField] Vector2 ThreadCount = new Vector2(8, 8);
    [SerializeField] bool showInSceneView = true;

    Camera cam;
    RenderTexture target;

    void Awake() {
        cam = GetComponent<Camera>();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (Camera.current.name != "SceneCamera" || showInSceneView) {
            SetParamaters();
            Render(source, destination);
        }
        else {
            Graphics.Blit(source, destination);
        }
    }

    private void Render(RenderTexture source, RenderTexture destination) {
        // Make sure we have a current render target
        InitRenderTexture();

        // Set the target and dispatch the compute shader
        shader.SetTexture(0, "Result", target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / ThreadCount.x);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / ThreadCount.y);
        shader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        // RenderTexture temp = new RenderTexture(target.width, target.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        Graphics.Blit(source, target);

        // Send the information to the shader and then to the screen
        Graphics.Blit(target, destination);
        // Graphics.Blit(target, destination);
    }

    void SetParamaters() {
        shader.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        shader.SetMatrix("_CameraInverseProjection", cam.projectionMatrix.inverse);
        shader.SetVector("_BaseColor", new Vector4(0.2f, 0, 0, 0.2f));
    }

    private void InitRenderTexture() {
        if (target == null || target.width != Screen.width || target.height != Screen.height)
        {
            // Release render texture if we already have one
            if (target != null) target.Release();
            
            // Get a render target for Ray Tracing
            target = new RenderTexture(
                Screen.width, Screen.height, 0, 
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear
            );
            target.enableRandomWrite = true;
            target.Create();
        }
    }
}