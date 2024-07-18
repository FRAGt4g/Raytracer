using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace RayTracingObjects
{
    [System.Serializable]
    public struct TracerObject {
        public Vector3 position;
        public float radius;
        public TracerMaterial material;

        public TracerObject(Vector3 position, float radius, Vector3 albedo, Vector3 specular) {
            this.position = position;
            this.radius = radius;
            this.material = new TracerMaterial(albedo, specular);
        }

        public TracerObject(Vector3 position, float radius, TracerMaterial material) {
            this.position = position;
            this.radius = radius;
            this.material = material;
        }

        public static int Size() => TracerMaterial.Size() + sizeof(float) * 4;

        public static TracerObject Convert(IncludeInRaytracer t) {
            return t.ToTracerObjectFromGameObject();
        }
        public static TracerObject Convert(Renderer renderer) {
            Vector3 color = new Vector3(renderer.sharedMaterial.color.r, renderer.sharedMaterial.color.g, renderer.sharedMaterial.color.b); 
            return new TracerObject() {
                radius = renderer.transform.lossyScale.x * 0.5f,
                position = renderer.transform.position,
                material = new TracerMaterial() {
                    albedo = color, 
                    specular = color * renderer.sharedMaterial.GetFloat("_Glossiness")
                }
            };
        }
    }
    
    [CustomPropertyDrawer(typeof(TracerObject))]
    public class TracerObjectDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty pos = property.FindPropertyRelative("position");
            SerializedProperty radius = property.FindPropertyRelative("radius");
            SerializedProperty material = property.FindPropertyRelative("material");

            Rect labelPosition = new Rect(position.x, position.y, position.width, position.height);

            AddLine(ref labelPosition, pos, "Position", "Position of the object");
            AddLine(ref labelPosition, radius, "Radius");
            AddLine(ref labelPosition, material, "Material", "Material of the object");

            EditorGUI.EndProperty();
        }

        public static void AddLine(ref Rect labelPosition, SerializedProperty value, string label, string tooltip = "") {
            EditorGUI.PropertyField(
                EditorGUI.PrefixLabel(
                    labelPosition, 
                    EditorGUIUtility.GetControlID(FocusType.Passive), 
                    new GUIContent(
                        label, 
                        tooltip
                    )
                ), 
                value, 
                new GUIContent()
            );
            labelPosition.y += EditorGUIUtility.singleLineHeight + 2;
        }
    }

    [CreateAssetMenu(menuName = "Ray Tracer/Basic Object", fileName = "Basic Ray Tracer Object")]
    public class TracerObjectScriptableObject : ScriptableObject
    {
        public TracerObject tracerObject;
    }
}