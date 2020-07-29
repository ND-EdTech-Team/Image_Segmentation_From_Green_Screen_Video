namespace Nexweron.WebCamPlayer
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;

	[CustomEditor(typeof(WebCamPlayer), false)]
	public class WebCamPlayerEditor : Editor
	{
		SerializedProperty _sourceModeProperty;
		SerializedProperty _sourceWebCamStreamProperty;

		AnimBool _showSourceWebCamStreamProperty = new AnimBool();

		SerializedProperty _playOnAwakeProperty;
											
		SerializedProperty _renderModeProperty;
		SerializedProperty _targetTextureProperty;
		SerializedProperty _targetRendererProperty;

		AnimBool _showTargetTexture = new AnimBool();
		AnimBool _showRenderer = new AnimBool();
	
		private void OnEnable() {
			_sourceModeProperty = serializedObject.FindProperty("_sourceMode");
			_sourceWebCamStreamProperty = serializedObject.FindProperty("_webCamStream");

			_playOnAwakeProperty = serializedObject.FindProperty("_playOnAwake");

			_renderModeProperty = serializedObject.FindProperty("_renderMode");
			_targetTextureProperty = serializedObject.FindProperty("_targetTexture");
			_targetRendererProperty = serializedObject.FindProperty("_targetRenderer");

			var sourceMode = (WebCamPlayer.SourceMode)_sourceModeProperty.enumValueIndex;
			_showSourceWebCamStreamProperty.value = (sourceMode == WebCamPlayer.SourceMode.WebCamStream);
			_showSourceWebCamStreamProperty.valueChanged.AddListener(Repaint);

			var renderMode = (WebCamPlayer.RenderMode)_renderModeProperty.enumValueIndex;
			_showTargetTexture.value = (renderMode == WebCamPlayer.RenderMode.RenderTexture);
			_showRenderer.value = (renderMode == WebCamPlayer.RenderMode.MaterialOverride);
			_showTargetTexture.valueChanged.AddListener(Repaint);
			_showRenderer.valueChanged.AddListener(Repaint);
		}
	
		protected virtual void OnDisable() {
			_showSourceWebCamStreamProperty.valueChanged.RemoveListener(Repaint);
			_showTargetTexture.valueChanged.RemoveListener(Repaint);
			_showRenderer.valueChanged.RemoveListener(Repaint);
		}
	
		public override void OnInspectorGUI() {
			var instance = serializedObject.targetObject as WebCamPlayer;

			EditorGUILayout.Space();

			//Source
			EditorGUILayout.PropertyField(_sourceModeProperty);

			var sourceMode = (WebCamPlayer.SourceMode) _sourceModeProperty.enumValueIndex;
			_showSourceWebCamStreamProperty.target = (!_sourceModeProperty.hasMultipleDifferentValues && sourceMode == WebCamPlayer.SourceMode.WebCamStream);

			++EditorGUI.indentLevel;
			{
				if (EditorGUILayout.BeginFadeGroup(_showSourceWebCamStreamProperty.faded)) {
					EditorGUILayout.PropertyField(_sourceWebCamStreamProperty);
				}
				EditorGUILayout.EndFadeGroup();
			}
			--EditorGUI.indentLevel;

			//Render
			EditorGUILayout.PropertyField(_renderModeProperty);

			var renderMode = (WebCamPlayer.RenderMode)_renderModeProperty.enumValueIndex;
			_showTargetTexture.target = (!_renderModeProperty.hasMultipleDifferentValues && renderMode == WebCamPlayer.RenderMode.RenderTexture);
			_showRenderer.target = (!_renderModeProperty.hasMultipleDifferentValues && renderMode == WebCamPlayer.RenderMode.MaterialOverride);
		
			++EditorGUI.indentLevel;
			{
				if (EditorGUILayout.BeginFadeGroup(_showTargetTexture.faded)) {
					EditorGUILayout.PropertyField(_targetTextureProperty);
				}
				EditorGUILayout.EndFadeGroup();

				if (EditorGUILayout.BeginFadeGroup(_showRenderer.faded)) {
					var renderer = _targetRendererProperty.objectReferenceValue as Renderer;
					if (renderer == null) {
						renderer = (target as WebCamPlayer).GetComponent<Renderer>();
						_targetRendererProperty.objectReferenceValue = renderer;
					}
					EditorGUILayout.PropertyField(_targetRendererProperty);
				}
				EditorGUILayout.EndFadeGroup();
			}
			--EditorGUI.indentLevel;

			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
			{
				EditorGUILayout.PropertyField(_playOnAwakeProperty);
			}
			EditorGUI.EndDisabledGroup();

			//Buttons
			bool isEnablePlayButton = EditorApplication.isPlaying && !instance.isPlaying;
			bool isEnableStopButton = EditorApplication.isPlaying && instance.isPlaying;

			var rect = EditorGUILayout.GetControlRect();
			Rect buttonRect = rect;

			EditorGUI.BeginDisabledGroup(!isEnablePlayButton);
			{
				buttonRect.xMin = rect.xMin;
				buttonRect.xMax = rect.width * 0.5f;
				if (GUI.Button(buttonRect, "Play", EditorStyles.miniButtonLeft)) {
					instance.Play();
				}
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(!isEnableStopButton);
			{
				buttonRect.xMin = buttonRect.xMax;
				buttonRect.xMax += rect.width * 0.5f;
				if (GUI.Button(buttonRect, "Stop", EditorStyles.miniButtonRight)) {
					instance.Stop();
				}
			}
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Space();

			//Setters
			if (EditorApplication.isPlaying) {

				if (instance.sourceMode != (WebCamPlayer.SourceMode)_sourceModeProperty.enumValueIndex) {
					instance.sourceMode = (WebCamPlayer.SourceMode)_sourceModeProperty.enumValueIndex;
				}
				if (instance.sourceWebCamStream != _sourceWebCamStreamProperty.objectReferenceValue) {
					instance.sourceWebCamStream = _sourceWebCamStreamProperty.objectReferenceValue as WebCamStream;
				}
				if (instance.renderMode != (WebCamPlayer.RenderMode)_renderModeProperty.enumValueIndex) {
					instance.renderMode = (WebCamPlayer.RenderMode)_renderModeProperty.enumValueIndex;
				}
				if (instance.targetRenderer != _targetRendererProperty.objectReferenceValue) {
					instance.targetRenderer = _targetRendererProperty.objectReferenceValue as Renderer;
				}
				if (instance.targetTexture != _targetTextureProperty.objectReferenceValue) {
					instance.targetTexture = _targetTextureProperty.objectReferenceValue as RenderTexture;
				}
			}
		
			serializedObject.ApplyModifiedProperties();
		}
	}
}
