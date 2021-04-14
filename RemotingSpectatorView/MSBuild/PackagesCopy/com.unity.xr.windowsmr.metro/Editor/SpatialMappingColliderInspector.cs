using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.WSA
{
    [MovedFrom("UnityEngine.VR.WSA")]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpatialMappingCollider))]
    [System.Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.google.com/document/d/1AMk4NwRVAtnG-LScXT2ne_s5mD4rmWK_C9dyn39ZDbc/edit.", false)]
    public class SpatialMappingColliderInspector : SpatialMappingBaseInspector
    {
        private static readonly GUIContent s_ColliderSettingsLabelContent = new GUIContent("Collider Settings");
        private static readonly GUIContent s_EnableCollisionsLabelContent = new GUIContent("Enable Collisions");
        private static readonly GUIContent s_MeshLayerLabelContent = new GUIContent("Mesh Layer");
        private static readonly GUIContent s_PhysicMaterialLabelContent = new GUIContent("Physic Material");

        private SerializedProperty m_EnableCollisionsProp = null;
        private SerializedProperty m_LayerProp = null;
        private SerializedProperty m_PhysicMaterialProp = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            CacheSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            ManageColliderSettings();
            EditorGUILayout.Separator();
            base.OnInspectorGUI();

            this.serializedObject.ApplyModifiedProperties();
        }

        void CacheSerializedProperties()
        {
            m_EnableCollisionsProp = this.serializedObject.FindProperty("m_EnableCollisions");
            m_LayerProp = this.serializedObject.FindProperty("m_Layer");
            m_PhysicMaterialProp = this.serializedObject.FindProperty("m_Material");
        }

        void ManageColliderSettings()
        {
            EditorGUILayout.LabelField(s_ColliderSettingsLabelContent, EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_EnableCollisionsProp, s_EnableCollisionsLabelContent);

            Rect layerRect = EditorGUILayout.GetControlRect(true);
            EditorGUI.BeginProperty(layerRect, s_MeshLayerLabelContent, m_LayerProp);
            m_LayerProp.intValue = EditorGUI.LayerField(layerRect, s_MeshLayerLabelContent, m_LayerProp.intValue);
            EditorGUI.EndProperty();

            EditorGUILayout.PropertyField(m_PhysicMaterialProp, s_PhysicMaterialLabelContent);
        }
    }
}
