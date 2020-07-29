namespace Nexweron.Core.MSK
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(MSKComponentBase), true)]
	public class MSKComponentBaseEditor : Editor
	{
		SerializedProperty _shaderProperty;

		private void OnEnable() {
			_shaderProperty = serializedObject.FindProperty("_shader");
		}

		public override void OnInspectorGUI() {
			var prevShader = _shaderProperty.objectReferenceValue;

			DrawDefaultInspector();

			var component = target as MSKComponentBase;

			var shader = _shaderProperty.objectReferenceValue;
			var availableShaders = component.GetAvailableShaders();
		
			if (shader == null || !availableShaders.Contains(shader.name)) {
				shader = Shader.Find(availableShaders[0]);
				_shaderProperty.objectReferenceValue = shader;
			}

			serializedObject.ApplyModifiedProperties();

			if (EditorApplication.isPlaying && component.isActiveAndEnabled) {
				if (prevShader != shader) {
					component.UpdateShaderMaterial();
				} else {
					component.UpdateShaderProperties();
				}
			}		
		}
	}
}