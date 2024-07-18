using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RenderDepth : MonoBehaviour
{
    public Shader depthShader;
    private Camera cam;
    private Material depthMaterial;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (depthShader != null)
        {
            depthMaterial = new Material(depthShader);
            cam.depthTextureMode = DepthTextureMode.Depth;
        }
        else
        {
            Debug.LogWarning("Depth shader not assigned.");
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (depthMaterial != null)
        {
            Graphics.Blit(src, dest, depthMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
