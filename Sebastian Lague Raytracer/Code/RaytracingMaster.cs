using UnityEngine;
using System.Collections.Generic;
using RayTracingObjects;
using System;
using UnityEditor;

[ExecuteAlways, ImageEffectAllowedInSceneView]
public class RayTracingMaster : MonoBehaviour
{
    public enum RenderMode {
        OnlyTracerObjects,
        All
    };

    #region Variables 
    [SerializeField] bool showInScene = true;
    [SerializeField] RenderMode renderMode = RenderMode.OnlyTracerObjects;

    [Header("Floor variables"), Space]
    [SerializeField] bool showFloor = false;
    [SerializeField] Vector3 floorSpecular = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] Vector3 floorAlbedo = new Vector3(0.04f, 0.04f, 0.04f);

    [Header("Objects"), Space]
    [SerializeField] ComputeShader RayTracingShader;
    [SerializeField] Texture SkyboxTexture;
    [SerializeField] Light DirectionalLight;
    [SerializeField] List<Light> Lights = new List<Light>();
    
    ComputeBuffer _objectBuffer;
    ComputeBuffer _lightBuffer;
    RenderTexture _target;
    Material _addMaterial;
    Camera _camera;
    int _currentSample = 0;
    bool useAntiAliasing = false;
    #endregion

    #region Life-Cycle Functions 
    private void Awake() {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable() {
        _currentSample = 0;
        UpdateGameObjectBuffer();
    }

    private void OnDisable() { if (_objectBuffer != null) _objectBuffer.Release(); }

    private void Update() 
    {
        _currentSample++;
        if (transform.hasChanged || DirectionalLight.transform.hasChanged)
        {
            _currentSample = 0;
            transform.hasChanged = false;
            DirectionalLight.transform.hasChanged = false;
        }
    }
    #endregion

    #region Setting Compute Shader Variables
    void UpdateGameObjectBuffer() {
        List<TracerObject> spheres = GetObjectList(renderMode);
        _objectBuffer = new ComputeBuffer(spheres.Count, TracerObject.Size());
        _objectBuffer.SetData(spheres);
    }
    void UpdateLightBuffer() {
        List<TracerLight> lights = GetLightList();
        _lightBuffer = new ComputeBuffer(lights.Count, TracerLight.Size());
        _lightBuffer.SetData(lights);
    }

    List<TracerLight> GetLightList() {
        List<TracerLight> tracerLights = new List<TracerLight>();
        foreach (RayTracerLight light in FindObjectsOfType<RayTracerLight>()) {
            tracerLights.Add(light.tracerLight);
        }
        return tracerLights;
    }
    
    List<TracerObject> GetObjectList(RenderMode mode = RenderMode.All) {
        List<TracerObject> spheres = new List<TracerObject>();

        if (mode == RenderMode.All) {
            foreach (var obj in FindObjectsOfType<Renderer>()) {
                spheres.Add(TracerObject.Convert(obj));
            }
        }
        else if (mode == RenderMode.OnlyTracerObjects) {
            foreach (var obj in FindObjectsOfType<IncludeInRaytracer>()) {
                spheres.Add(TracerObject.Convert(obj));
            }
        }

        return spheres;
    }

    private void SetShaderParameters()
    {
        UpdateGameObjectBuffer();
        UpdateLightBuffer();
        RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        RayTracingShader.SetBuffer(0, "_Spheres", _objectBuffer);
        RayTracingShader.SetBuffer(0, "_Lights", _lightBuffer);
        RayTracingShader.SetBuffer(0, "_FloorMaterial", new TracerMaterial() { 
            albedo = floorAlbedo, 
            specular = floorSpecular
        }.ToBuffer());
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        RayTracingShader.SetVector("_PixelOffset", useAntiAliasing 
            ? new Vector2(UnityEngine.Random.value, UnityEngine.Random.value) 
            : new Vector2(0.5f, 0.5f)
        );
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(
            DirectionalLight.transform.forward.x, 
            DirectionalLight.transform.forward.y, 
            DirectionalLight.transform.forward.z, 
            DirectionalLight.intensity
        ));
        RayTracingShader.SetBool("_ShowFloor", showFloor);
    }
    #endregion

    #region Redering 
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Camera.current.name != "SceneCamera" || showInScene) {
            SetShaderParameters();
            Render(destination);
        }
        else {
            Graphics.Blit(source, destination);
        }
    }

    private void Render(RenderTexture destination)
    {
        if (useAntiAliasing) {
            // Blit the result texture to the screen
            if (_addMaterial == null) _addMaterial = new Material(Shader.Find("Hidden/AddShader"));
            _addMaterial.SetFloat("_Sample", _currentSample);
            Graphics.Blit(_target, destination, _addMaterial);
        }

        // Make sure we have a current render target
        InitRenderTexture();

        // Set the target and dispatch the compute shader
        RayTracingShader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Blit the result texture to the screen
        Graphics.Blit(_target, destination);
    }

    private void InitRenderTexture() {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            _currentSample = 0;
            
            // Release render texture if we already have one
            if (_target != null) _target.Release();
            
            // Get a render target for Ray Tracing
            _target = new RenderTexture(
                Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear
            );
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
    #endregion
}