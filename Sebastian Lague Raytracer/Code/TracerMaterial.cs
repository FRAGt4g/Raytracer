using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace RayTracingObjects
{
    [System.Serializable]
    public struct TracerMaterial {
        public Vector3 albedo;
        public Vector3 specular;

        public TracerMaterial(Vector3 albedo, Vector3 specular) {
            this.albedo = albedo;
            this.specular = specular;
        }

        public ComputeBuffer ToBuffer() {
            ComputeBuffer buffer = new ComputeBuffer(1, Size());
            buffer.SetData(new TracerMaterial[] { this });
            return buffer;
        }

        public static int Size() => sizeof(float) * 6;
    }
    
    [CustomPropertyDrawer(typeof(TracerMaterial))]
    public class TracerMaterialDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty albedo = property.FindPropertyRelative("albedo");
            SerializedProperty specular = property.FindPropertyRelative("specular");

            Rect labelPosition = new Rect(position.x, position.y, position.width, position.height);

            AddLine(ref labelPosition, albedo, "Albedo", "Albedo of the object");
            AddLine(ref labelPosition, specular, "Specular", "Specular of the object");

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

    [CreateAssetMenu(menuName = "Ray Tracer/Basic Material", fileName = "Basic Ray Tracer Material")]
    public class TracerMaterialScriptable : ScriptableObject
    {
        public TracerMaterial material;
    }
}