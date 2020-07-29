namespace Nexweron.Core.MSK
{
	using UnityEngine;
	using UnityEngine.UI;

	public class MSKBridgeBase : MonoBehaviour
	{
		public enum RenderMode {
			APIOnly,
			RenderTexture,
			MaterialOverride,
			RawImage
		}

		[SerializeField]
		protected RenderMode _renderMode = RenderMode.APIOnly;
		public RenderMode renderMode {
			get { return _renderMode; }
			set { SetRenderMode(value); }
		}
		private void SetRenderMode(RenderMode mode, bool isForce = false) {
			if (_renderMode != mode || isForce) {
				_renderMode = mode;
				UpdateTexture();
			}
		}

		protected Texture _sourceTexture;
		public Texture sourceTexture {
			get { return _sourceTexture; }
			set {
				//if (_sourceMode == SourceMode.SetByAPI) {
					SetSourceTexture(value);
				//} else {
				//	Debug.LogError("MSKBridge | sourceTexture can be set only in SourceMode.SetByAPI mode");
				//}
			}
		}
		private void SetSourceTexture(Texture value, bool isForce = false) {
			if (_sourceTexture != value || isForce) {
				_sourceTexture = value;
				UpdateRenderTexture();
				UpdateTexture();
			}
		}

		[SerializeField]
		protected RenderTexture _targetTexture;
		public RenderTexture targetTexture {
			get { return _targetTexture; }
			set {
				if (_renderMode == RenderMode.RenderTexture) {
					SetTargetTexture(value);
				} else {
					Debug.LogError("MSKBridge | targetTexture can be set only in RenderMode.RenderTexture mode");
				}
			}
		}
		private void SetTargetTexture(RenderTexture value, bool isForce = false) {
			if (_targetTexture != value || isForce) {
				_targetTexture = value;
				UpdateTexture();
			}
		}

		[SerializeField]
		protected Renderer _targetRenderer;
		public Renderer targetRenderer {
			get { return _targetRenderer; }
			set {
				if (_renderMode == RenderMode.MaterialOverride) {
					SetTargetRenderer(value);
				} else {
					Debug.LogError("MSKBridge | targetRenderer can be set only in RenderMode.MaterialOverride mode");
				}
			}
		}
		private void SetTargetRenderer(Renderer value) {
			_targetRenderer = value;
			if (_targetRenderer != null) {
				SetTargetMaterial(_targetRenderer.material, true);
			}
		}

		protected Material _targetMaterial;
		private void SetTargetMaterial(Material value, bool isForce = false) {
			if (_targetMaterial != value || isForce) {
				_targetMaterial = value;
				SetTargetMaterialMainTexture(_texture, isForce);
			}
		}
		private void SetTargetMaterialMainTexture(Texture value, bool isForce = false) {
			if (_targetMaterial != null) {
				if (_targetMaterial.mainTexture != value || isForce) {
					_targetMaterial.mainTexture = value;
				}
			}
		}

		[SerializeField]
		protected RawImage _targetRawImage;
		public RawImage targetRawImage {
			get { return _targetRawImage; }
			set {
				if (_renderMode == RenderMode.RawImage) {
					SetTargetRawImage(value);
				} else {
					Debug.LogError("MSKBridge | targetRawImage can be set only in RenderMode.RawImage mode");
				}
			}
		}
		private void SetTargetRawImage(RawImage value) {
			_targetRawImage = value;
			if (_targetRawImage != null) {
				SetTargetRawImageTexture(_texture, true);
			}
		}
		private void SetTargetRawImageTexture(Texture value, bool isForce = false) {
			if (_targetRawImage != null) {
				if (_targetRawImage.texture != value || isForce) {
					_targetRawImage.texture = value;
				}
			}
		}

		protected RenderTexture _texture;
		public RenderTexture texture {
			get { return _texture; }
		}
		private void UpdateTexture() {
			if (_renderMode == RenderMode.RenderTexture) {
				if (_texture != _targetTexture) {
					if (_targetTexture != null) {
						_texture = _targetTexture;
					} else {
						_texture = _renderTexture;
					}
				}
			} else {
				_texture = _renderTexture;
				if (_renderMode == RenderMode.MaterialOverride) {
					SetTargetMaterialMainTexture(_texture);
				} else if(_renderMode == RenderMode.RawImage) {
					SetTargetRawImageTexture(_texture);
				}
			}
		}
		protected RenderTexture _renderTexture;
		private void UpdateRenderTexture() {
			if (_sourceTexture != null) {
				RenderTextureUtils.SetRenderTextureSize(ref _renderTexture, _sourceTexture.width, _sourceTexture.height);
			} else if (_renderTexture != null) {
				DestroyImmediate(_renderTexture);
			}
		}
		protected void Start() {
			SetSourceTexture(_sourceTexture, true);
			SetRenderMode(_renderMode, true);
			SetTargetRenderer(_targetRenderer);
		}
	
		protected void OnDestroy() {
			if (_renderTexture != null) {
				DestroyImmediate(_renderTexture);
			}
		}
	}
}
