namespace Nexweron.Core.MSK
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using UnityEngine.UI;

	[CustomEditor(typeof(MSKBridgeBase), true)]
	public class MSKBridgeBaseEditor : Editor
	{
		SerializedProperty _scriptProperty;
		SerializedProperty _renderModeProperty;
		SerializedProperty _targetTextureProperty;
		SerializedProperty _targetRendererProperty;
		SerializedProperty _targetRawImageProperty;

		AnimBool _showTargetTexture = new AnimBool();
		AnimBool _showRenderer = new AnimBool();
		AnimBool _showRawImage = new AnimBool();

		private void OnEnable() {
			_scriptProperty = serializedObject.FindProperty("m_Script");
			_renderModeProperty = serializedObject.FindProperty("_renderMode");
			_targetTextureProperty = serializedObject.FindProperty("_targetTexture");
			_targetRendererProperty = serializedObject.FindProperty("_targetRenderer");
			_targetRawImageProperty = serializedObject.FindProperty("_targetRawImage");

			var renderMode = (MSKBridgeBase.RenderMode)_renderModeProperty.enumValueIndex;
			_showTargetTexture.value = (renderMode == MSKBridgeBase.RenderMode.RenderTexture);
			_showRenderer.value = (renderMode == MSKBridgeBase.RenderMode.MaterialOverride);
			_showRawImage.value = (renderMode == MSKBridgeBase.RenderMode.RawImage);
			_showTargetTexture.valueChanged.AddListener(Repaint);
			_showRenderer.valueChanged.AddListener(Repaint);
			_showRawImage.valueChanged.AddListener(Repaint);
		}
	
		protected virtual void OnDisable() {
			_showTargetTexture.valueChanged.RemoveListener(Repaint);
			_showRenderer.valueChanged.RemoveListener(Repaint);
			_showRawImage.valueChanged.RemoveListener(Repaint);
		}
	
		public override void OnInspectorGUI() {
			serializedObject.Update();
		
			//Script
			EditorGUI.BeginDisabledGroup(true);
			{ EditorGUILayout.PropertyField(_scriptProperty, true, new GUILayoutOption[0]);	}
			EditorGUI.EndDisabledGroup();

			//Default
			DrawPropertiesExcluding(serializedObject, new string[] { _scriptProperty.name, _renderModeProperty.name, _targetTextureProperty.name, _targetRendererProperty.name, _targetRawImageProperty.name });
		
			var instance = serializedObject.targetObject as MSKBridgeBase;

			EditorGUILayout.Space();
		
			//Render
			EditorGUILayout.PropertyField(_renderModeProperty);

			var renderMode = (MSKBridgeBase.RenderMode)_renderModeProperty.enumValueIndex;
			_showTargetTexture.target = (!_renderModeProperty.hasMultipleDifferentValues && renderMode == MSKBridgeBase.RenderMode.RenderTexture);
			_showRenderer.target = (!_renderModeProperty.hasMultipleDifferentValues && renderMode == MSKBridgeBase.RenderMode.MaterialOverride);
			_showRawImage.target = (!_renderModeProperty.hasMultipleDifferentValues && renderMode == MSKBridgeBase.RenderMode.RawImage);
			++EditorGUI.indentLevel;
			{
				if (EditorGUILayout.BeginFadeGroup(_showTargetTexture.faded)) {
					EditorGUILayout.PropertyField(_targetTextureProperty);
				}
				EditorGUILayout.EndFadeGroup();

				if (EditorGUILayout.BeginFadeGroup(_showRenderer.faded)) {
					var renderer = _targetRendererProperty.objectReferenceValue as Renderer;
					if (renderer == null) {
						renderer = (target as MSKBridgeBase).GetComponent<Renderer>();
						_targetRendererProperty.objectReferenceValue = renderer;
					}
					EditorGUILayout.PropertyField(_targetRendererProperty);
				}
				EditorGUILayout.EndFadeGroup();

				if (EditorGUILayout.BeginFadeGroup(_showRawImage.faded)) {
					var rawImage = _targetRawImageProperty.objectReferenceValue as RawImage;
					if (rawImage == null) {
						rawImage = (target as MSKBridgeBase).GetComponent<RawImage>();
						_targetRawImageProperty.objectReferenceValue = rawImage;
					}
					EditorGUILayout.PropertyField(_targetRawImageProperty);
				}
				EditorGUILayout.EndFadeGroup();
			}
			--EditorGUI.indentLevel;
		
			EditorGUILayout.Space();

			//Setters
			if (EditorApplication.isPlaying) {
				if (instance.renderMode != (MSKBridgeBase.RenderMode)_renderModeProperty.enumValueIndex) {
					instance.renderMode = (MSKBridgeBase.RenderMode)_renderModeProperty.enumValueIndex;
				}
				if (instance.targetTexture != _targetTextureProperty.objectReferenceValue) {
					instance.targetTexture = _targetTextureProperty.objectReferenceValue as RenderTexture;
				}
				if (instance.targetRenderer != _targetRendererProperty.objectReferenceValue) {
					instance.targetRenderer = _targetRendererProperty.objectReferenceValue as Renderer;
				}
				if (instance.targetRawImage != _targetRawImageProperty.objectReferenceValue) {
					instance.targetRawImage = _targetRawImageProperty.objectReferenceValue as RawImage;
				}
			}
		
			serializedObject.ApplyModifiedProperties();
		}
	}
}
