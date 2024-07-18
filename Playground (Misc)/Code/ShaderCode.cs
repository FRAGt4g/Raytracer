using System;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderCode : MonoBehaviour
{
    public Shader shader;
    private Material material;
    
    [SerializeField] public Color screen_color;

    [SerializeField] public Vector3 viewParams = new Vector3(1.0f, 0.5f, 0.25f);
    [SerializeField] bool showX;
    [SerializeField] bool showY;
    [SerializeField] bool showZ;


    void Start()
    {
        if (shader == null)
        {
            Debug.LogError("Shader is not assigned!");
            return;
        }
        material = new Material(shader);
        Shader.SetGlobalVector("ViewParams", viewParams);
        Camera.main.RenderWithShader(shader, "");
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Transform cam = Camera.main.transform;
        Vector3 anglePercentages = cam.rotation.eulerAngles;
        anglePercentages.x = CalcAngleColor(anglePercentages.x);
        anglePercentages.y = CalcAngleColor(anglePercentages.y);
        anglePercentages.z = CalcAngleColor(anglePercentages.z);
        Shader.SetGlobalVector("ViewParams", anglePercentages);
        Shader.SetGlobalVector("CameraPosition", cam.position);
        Shader.SetGlobalMatrix("CamLocalToWorldMatrix", cam.localToWorldMatrix);
        if (material != null)
        {
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    float CalcAngleColor(float angle)
    {
        angle -= 180; //set angle domain to -180 to 180
        
        var final = Mathf.Max(-2*Mathf.Abs(angle/180) + 1, 0);
        return final;
    }

    private void OnDrawGizmos()
    {
        int rayCount = 2;

        if (showX)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < 360; i += rayCount)
            {
                Vector3 rotatedVector = RotateVector(transform.forward, Vector3.right, i);
                Gizmos.color = new Color(CalcAngleColor(i), 0, 0);
                Gizmos.DrawRay(transform.position, rotatedVector);
            }
        }
        if (showY)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < 360; i += rayCount)
            {
                Vector3 rotatedVector = RotateVector(transform.right, Vector3.up, i);
                Gizmos.color = new Color(0, CalcAngleColor(i), 0);
                Gizmos.DrawRay(transform.position, rotatedVector);
            }
        }
        if (showZ)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < 360; i += rayCount)
            {
                Vector3 rotatedVector = RotateVector(transform.up, Vector3.forward, i);
                Gizmos.color = new Color(0, 0, CalcAngleColor(i));
                Gizmos.DrawRay(transform.position, rotatedVector);
            }
        }
    }

    Vector3 RotateVector(Vector3 vector, Vector3 axis, float angleInDegrees)
    {
        // Convert angle to radians (Unity's Quaternion uses radians)
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Create a quaternion for the rotation
        Quaternion rotation = Quaternion.AngleAxis(angleInDegrees, axis);

        // Rotate the vector using the quaternion
        return rotation * vector;
    }
}

// print(transform.rotation.eulerAngles);
// screen_color = new Color(CalcAngleColor(transform.rotation.eulerAngles.x), CalcAngleColor(transform.rotation.eulerAngles.y), CalcAngleColor(transform.rotation.eulerAngles.z));
// Gizmos.color = screen_color;
// Gizmos.DrawRay(transform.position, transform.forward);