using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dot_Cover : MonoBehaviour
{
    Camera cam;

    [SerializeField] GameObject dot;
    [SerializeField] GameObject arrow;
    [SerializeField] Vector2 dotCount = Vector2.zero;

    private delegate void DrawCommand();
    private Queue<DrawCommand> drawCommands = new Queue<DrawCommand>();

    private void Start()
    {
        cam = GetComponent<Camera>();
        /*PlaceCamPoints(cam, dot, dotCount);*/
    }

    private void Update()
    {
        DrawRaysFromCam(cam, dotCount);
    }

    void DrawRaysFromCam(Camera cam, Vector2 dotCount)
    {
        float frontHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float frontWidth = frontHeight * cam.aspect;

        Vector3 topRightLocal = new Vector3(frontWidth * 0.5f, frontHeight * 0.5f, cam.nearClipPlane);

        for (int i = 0; i < dotCount.x; i++)
        {
            for (int j = 0; j < dotCount.y; j++)
            {
                float percent_X = i / (dotCount.x - 1);
                float percent_Y = j / (dotCount.y - 1);

                Vector3 local_position = topRightLocal - new Vector3(frontWidth * percent_X, frontHeight * percent_Y);
                Vector3 world_location = cam.transform.position + cam.transform.right * local_position.x + cam.transform.up * local_position.y + cam.transform.forward * local_position.z;

                DrawRay(cam.transform.position, world_location);
            }
        }
    }

    void PlaceCamPoints(Camera cam, GameObject point, Vector2 dotCount)
    {
        float frontHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float frontWidth = frontHeight * cam.aspect;

        Vector3 topRightLocal = new Vector3(frontWidth * 0.5f, frontHeight * 0.5f, cam.nearClipPlane);

        for (int i = 0; i < dotCount.x; i++)
        {
            for (int j = 0; j < dotCount.y; j++)
            {
                float percent_X = i / (dotCount.x - 1);
                float percent_Y = j / (dotCount.y - 1);

                Vector3 local_position = topRightLocal - new Vector3(frontWidth * percent_X, frontHeight * percent_Y);

                Vector3 world_location = cam.transform.position + cam.transform.right * local_position.x + cam.transform.up * local_position.y + cam.transform.forward * local_position.z;

                PlacePoint(world_location).SetParent(cam.transform);
                DrawRay(cam.transform.position, world_location);
            }
        }
    }

    Transform PlacePoint(Vector3 location)
    {
        var p = Instantiate(dot, location, Quaternion.identity);
        p.tag = "Camera Dot";
        return p.transform;
    }

    void DrawRay(Vector3 start, Vector3 end)
    {
        drawCommands.Enqueue(() => { 
            Gizmos.DrawRay(start, end - start); 
        });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        while (drawCommands.Count > 0)
        {
            drawCommands.Dequeue()();
        }
    }
}
