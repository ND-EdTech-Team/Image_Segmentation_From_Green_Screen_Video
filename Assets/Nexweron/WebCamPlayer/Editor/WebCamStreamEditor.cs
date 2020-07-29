namespace Nexweron.WebCamPlayer
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;

	[CustomEditor(typeof(WebCamStream), true)]
	public class WebCamStreamEditor : Editor
	{
		SerializedProperty _deviceModeProperty;
		SerializedProperty _deviceNameProperty;
		SerializedProperty _deviceIndexProperty;

		SerializedProperty _resolutionModeProperty;
		SerializedProperty _requestedWidthProperty;
		SerializedProperty _requestedHeightProperty;

		SerializedProperty _fpsModeProperty;
		SerializedProperty _requestedFPSProperty;

		SerializedProperty _playOnAwakeProperty;

		SerializedProperty _infoWidthProperty;

		AnimBool _showDeviceNameProperty = new AnimBool();
		AnimBool _showDeviceIndexProperty = new AnimBool();

		AnimBool _showResolutionProperties = new AnimBool();
		AnimBool _showFPSProperty = new AnimBool();
	
		private void OnEnable() {
			_deviceModeProperty = serializedObject.FindProperty("_deviceMode");
			_deviceNameProperty = serializedObject.FindProperty("_deviceName");
			_deviceIndexProperty = serializedObject.FindProperty("_deviceIndex");

			_resolutionModeProperty = serializedObject.FindProperty("_resolutionMode");
			_requestedWidthProperty = serializedObject.FindProperty("_requestedWidth");
			_requestedHeightProperty = serializedObject.FindProperty("_requestedHeight");

			_fpsModeProperty = serializedObject.FindProperty("_fpsMode");
			_requestedFPSProperty = serializedObject.FindProperty("_requestedFPS");

			_playOnAwakeProperty = serializedObject.FindProperty("_playOnAwake");
		
			var deviceMode = (WebCamStream.DeviceMode)_deviceModeProperty.enumValueIndex;
			_showDeviceNameProperty.value = (deviceMode == WebCamStream.DeviceMode.DeviceName);
			_showDeviceIndexProperty.value = (deviceMode == WebCamStream.DeviceMode.DeviceIndex);
			_showDeviceNameProperty.valueChanged.AddListener(Repaint);
			_showDeviceIndexProperty.valueChanged.AddListener(Repaint);

			var resolutionMode = (WebCamStream.ResolutionMode)_resolutionModeProperty.enumValueIndex;
			_showResolutionProperties.value = (resolutionMode == WebCamStream.ResolutionMode.Manual);
			_showResolutionProperties.valueChanged.AddListener(Repaint);

			var fpsMode = (WebCamStream.FPSMode)_fpsModeProperty.enumValueIndex;
			_showFPSProperty.value = (fpsMode == WebCamStream.FPSMode.Manual && resolutionMode == WebCamStream.ResolutionMode.Manual);
			_showFPSProperty.valueChanged.AddListener(Repaint);
		}
	
		protected virtual void OnDisable() {
			_showDeviceNameProperty.valueChanged.RemoveListener(Repaint);
			_showDeviceIndexProperty.valueChanged.RemoveListener(Repaint);
			_showResolutionProperties.valueChanged.RemoveListener(Repaint);
			_showFPSProperty.valueChanged.RemoveListener(Repaint);
		}
	
		public override void OnInspectorGUI() {
			var instance = serializedObject.targetObject as WebCamStream;

			EditorGUILayout.Space();

			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying && (instance.webCamTexture != null || instance.isAuthorization));
			{
				//Device
				EditorGUILayout.PropertyField(_deviceModeProperty);

				var deviceMode = (WebCamStream.DeviceMode)_deviceModeProperty.enumValueIndex;
				_showDeviceNameProperty.target = (!_deviceModeProperty.hasMultipleDifferentValues && deviceMode == WebCamStream.DeviceMode.DeviceName);
				_showDeviceIndexProperty.target = (!_deviceModeProperty.hasMultipleDifferentValues && deviceMode == WebCamStream.DeviceMode.DeviceIndex);

				++EditorGUI.indentLevel;
				{
					if (EditorGUILayout.BeginFadeGroup(_showDeviceNameProperty.faded)) {
						EditorGUILayout.PropertyField(_deviceNameProperty);
					}
					EditorGUILayout.EndFadeGroup();

					if (EditorGUILayout.BeginFadeGroup(_showDeviceIndexProperty.faded)) {
						EditorGUILayout.PropertyField(_deviceIndexProperty);
					}
					EditorGUILayout.EndFadeGroup();
				}
				--EditorGUI.indentLevel;

				//Resolution
				EditorGUILayout.PropertyField(_resolutionModeProperty);

				var resolutionMode = (WebCamStream.ResolutionMode)_resolutionModeProperty.enumValueIndex;
				_showResolutionProperties.target = (!_resolutionModeProperty.hasMultipleDifferentValues && resolutionMode == WebCamStream.ResolutionMode.Manual);

				++EditorGUI.indentLevel;
				{
					if (EditorGUILayout.BeginFadeGroup(_showResolutionProperties.faded)) {
						EditorGUILayout.PropertyField(_requestedWidthProperty);
						EditorGUILayout.PropertyField(_requestedHeightProperty);
					}
					EditorGUILayout.EndFadeGroup();
				}
				--EditorGUI.indentLevel;

				//Fps
				EditorGUI.BeginDisabledGroup(resolutionMode != WebCamStream.ResolutionMode.Manual);
				{
					EditorGUILayout.PropertyField(_fpsModeProperty);
					if (resolutionMode != WebCamStream.ResolutionMode.Manual) {
						_fpsModeProperty.enumValueIndex = (int)WebCamStream.FPSMode.Auto;
					}
					var fpsMode = (WebCamStream.FPSMode)_fpsModeProperty.enumValueIndex;
					_showFPSProperty.target = (!_fpsModeProperty.hasMultipleDifferentValues && fpsMode == WebCamStream.FPSMode.Manual);

					++EditorGUI.indentLevel;
					{
						if (EditorGUILayout.BeginFadeGroup(_showFPSProperty.faded)) {
							EditorGUILayout.PropertyField(_requestedFPSProperty);
						}
						EditorGUILayout.EndFadeGroup();
					}
					--EditorGUI.indentLevel;
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUI.EndDisabledGroup();

			//Awake
			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
			{
				EditorGUILayout.PropertyField(_playOnAwakeProperty);
			}
			EditorGUI.EndDisabledGroup();

			//Buttons
			bool isEnablePlayButton = EditorApplication.isPlaying && (instance.webCamTexture == null || (instance.webCamTexture != null && !instance.webCamTexture.isPlaying));
			bool isEnablePauseButton = EditorApplication.isPlaying && (instance.webCamTexture != null && instance.webCamTexture.isPlaying);
			bool isEnableStopButton = EditorApplication.isPlaying && instance.webCamTexture != null;

			var rect = EditorGUILayout.GetControlRect();
			Rect buttonRect = rect;

			EditorGUI.BeginDisabledGroup(!isEnablePlayButton);
			{
				buttonRect.xMin = rect.xMin;
				buttonRect.xMax = rect.width*0.34f;
				if (GUI.Button(buttonRect, "Play", EditorStyles.miniButtonLeft)) {
					instance.Play(true);
				}
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(!isEnablePauseButton);
			{
				buttonRect.xMin = buttonRect.xMax;
				buttonRect.xMax += rect.width*0.33f;
				if (GUI.Button(buttonRect, "Pause", EditorStyles.miniButtonMid)) {
					instance.Pause();

				}
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(!isEnableStopButton);
			{
				buttonRect.xMin = buttonRect.xMax;
				buttonRect.xMax += rect.width*0.33f;
				if (GUI.Button(buttonRect, "Stop", EditorStyles.miniButtonRight)) {
					instance.Stop();
				}
			}
			EditorGUI.EndDisabledGroup();

			//Info
			string deviceNameValue = "-";
			string deviceIndexValue = "";
			string deviceResolutionValue = "";

			if (instance.isPrepared) {
				var webCamTexture = instance.webCamTexture;
				deviceNameValue = webCamTexture.deviceName;
				deviceIndexValue = "["+ArrayUtility.IndexOf(WebCamTexture.devices, instance.webCamDevice)+"]";
				deviceResolutionValue = ", "+webCamTexture.width + "x" + webCamTexture.height;
			}
			EditorGUILayout.LabelField("Device"+ deviceIndexValue +": " + deviceNameValue + deviceResolutionValue, EditorStyles.miniLabel);
		
			serializedObject.ApplyModifiedProperties();
		}
	}
}
