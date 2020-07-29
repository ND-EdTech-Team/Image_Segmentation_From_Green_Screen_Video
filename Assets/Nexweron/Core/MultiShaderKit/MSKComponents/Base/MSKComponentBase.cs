namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	[RequireComponent(typeof(MSKControllerBase))]
	abstract public class MSKComponentBase : MonoBehaviour
	{
		protected static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct {
			if (!currentValue.Equals(newValue)) {
				currentValue = newValue;
				return true;
			}
			return false;
		}

		protected static bool SetClass<T>(ref T currentValue, T newValue) where T : class {
			if (!currentValue.Equals(newValue)) {
				currentValue = newValue;
				return true;
			}
			return false;
		}

		[SerializeField, Tooltip("Component Shader")]
		protected Shader _shader = null;
		public Shader shader {
			get { return _shader; }
			set {
				if (SetClass(ref _shader, value)) {
					_shader = value;
					UpdateShader();
				}
			}
		}

		protected Material _shaderMaterial;
		protected MSKControllerBase _mskController;

		private bool _inited = false;

		protected virtual void Awake() {
			CheckInit();
			UpdateShader();
		}

		protected virtual void Start() { }

		public void Init(MSKControllerBase mskController) {
			if (_mskController != mskController) {
				_mskController = mskController;
				_inited = _mskController != null;
			}
		}

		protected void CheckInit() {
			if (!_inited) {
				Init(GetComponent<MSKControllerBase>());
			}
		}

		protected void UpdateShader() {
			var availableShaders = GetAvailableShaders();
			if (_shader == null || !availableShaders.Contains(shader.name)) {
				_shader = Shader.Find(availableShaders[0]);
			}
			UpdateShaderMaterial();
		}

		public virtual List<string> GetAvailableShaders() {
			return null;
		}

		public virtual void UpdateShaderProperties() {
			//
		}

		public virtual void UpdateShaderMaterial() {
			if (_shader != null) {
				if (_shaderMaterial != null && _shaderMaterial.shader != _shader) {
					_shaderMaterial.shader = _shader;
				} else {
					_shaderMaterial = new Material(_shader);
					_shaderMaterial.hideFlags = HideFlags.DontSave;
				}
				UpdateShaderProperties();
			} else {
				UpdateShader();
			}
		}

		public abstract void UpdateSourceTexture();

		public virtual RenderTexture GetRender(Texture rt_src) {
			return null;
		}

		protected virtual void OnDestroy() {
			if (_shaderMaterial != null) {
				DestroyImmediate(_shaderMaterial);
			}
			_inited = false;
		}
	}
}