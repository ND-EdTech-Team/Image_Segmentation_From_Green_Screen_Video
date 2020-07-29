namespace Nexweron.WebCamPlayer
{
	using UnityEngine;
	using Nexweron.Core;

	public class WebCamPlayer : MonoBehaviour
	{
		public delegate void EventHandler();
		public event EventHandler frameReady = delegate { };

		public enum SourceMode
		{
			WebCamStream,
			SetByAPI
		}

		[SerializeField]
		private SourceMode _sourceMode = SourceMode.WebCamStream;
		public SourceMode sourceMode
		{
			get { return _sourceMode; }
			set { SeSourceMode(value); }
		}
		private void SeSourceMode(SourceMode value, bool isForce = false) {
			if (_sourceMode != value || isForce) {
				_sourceMode = value;

				if (_sourceMode == SourceMode.WebCamStream) {
					SetSourceWebCamStream(_webCamStream, true);
				} else {
					SetWebCamTexture(_webCamTexture, true);
				}
			}
		}

		[SerializeField]
		private WebCamStream _webCamStream;
		public WebCamStream sourceWebCamStream
		{
			get { return _webCamStream; }
			set {
				if (_sourceMode == SourceMode.WebCamStream) {
					SetSourceWebCamStream(value);
				} else {
					Debug.LogError("WebCamPlayer | sourceWebCamStream can be set only in SourceMode.WebCamStream mode");
				}
			}
		}
		private void SetSourceWebCamStream(WebCamStream value, bool isForce = false) {
			if (_webCamStream != value || isForce) {
				if (_webCamStream != null) {
					_webCamStream.prepareCompleted -= SourceWebCamStreamPrepareCompleted;
				}
				_webCamStream = value;
				if (_webCamStream != null) {
					_webCamStream.prepareCompleted += SourceWebCamStreamPrepareCompleted;
					if (_webCamStream.isPrepared) {
						SourceWebCamStreamPrepareCompleted();
					}
				}
			}
		}
		private void SourceWebCamStreamPrepareCompleted() {
			SetWebCamTexture(_webCamStream.webCamTexture, true);
		}

		public enum RenderMode
		{
			None,
			APIOnly,
			RenderTexture,
			MaterialOverride
		}

		[SerializeField]
		private RenderMode _renderMode = RenderMode.APIOnly;
		public RenderMode renderMode
		{
			get { return _renderMode; }
			set { SetRenderMode(value); }
		}
		private void SetRenderMode(RenderMode mode, bool isForce = false) {
			if (_renderMode != mode || isForce) {
				_renderMode = mode;
				UpdateTexture();
			}
		}

		private WebCamTexture _webCamTexture;
		public WebCamTexture webCamTexture
		{
			get { return _webCamTexture; }
			set {
				if (_sourceMode == SourceMode.SetByAPI) {
					SetWebCamTexture(value);
				} else {
					Debug.LogError("WebCamPlayer | webCamTexture can be set only in SourceMode.SetByAPI mode");
				}
			}
		}
		private void SetWebCamTexture(WebCamTexture value, bool isForce = false) {
			if (_webCamTexture != value || isForce) {
				_webCamTexture = value;
				UpdateRenderTexture();
				UpdateTexture();
			}
		}

		[SerializeField]
		private RenderTexture _targetTexture;
		public RenderTexture targetTexture
		{
			get { return _targetTexture; }
			set {
				if (_renderMode == RenderMode.RenderTexture) {
					SetTargetTexture(value);
				} else {
					Debug.LogError("WebCamPlayer | targetTexture can be set only in RenderMode.RenderTexture mode");
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
		private Renderer _targetRenderer;
		public Renderer targetRenderer
		{
			get { return _targetRenderer; }
			set {
				if (_renderMode == RenderMode.MaterialOverride) {
					SetTargetRenderer(value);
				} else {
					Debug.LogError("WebCamPlayer | targetRenderer can be set only in RenderMode.MaterialOverride mode");
				}
			}
		}
		private void SetTargetRenderer(Renderer value) {
			_targetRenderer = value;
			if (_targetRenderer != null) {
				SetTargetMaterial(_targetRenderer.material, true);
			}
		}

		private Material _targetMaterial;
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

		private RenderTexture _texture;
		public RenderTexture texture
		{
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
				}
			}
		}

		[SerializeField]
		private bool _playOnAwake = false;
		public bool playOnAwake
		{
			get { return _playOnAwake; }
			set { _playOnAwake = value; }
		}

		private bool _didUpdateTexture = false;
		public bool didUpdateTexture
		{
			get { return _didUpdateTexture; }
		}

		private bool _isPlaying = false;
		public bool isPlaying
		{
			get { return _isPlaying; }
		}

		private RenderTexture _renderTexture;
		private void UpdateRenderTexture() {
			if (_webCamTexture != null) {
				RenderTextureUtils.SetRenderTextureSize(ref _renderTexture, _webCamTexture.width, _webCamTexture.height);
			} else if (_renderTexture != null) {
				DestroyImmediate(_renderTexture);
			}
		}

		private void Start() {
			SeSourceMode(_sourceMode, true);

			if (_sourceMode == SourceMode.WebCamStream) {
				SetSourceWebCamStream(_webCamStream, true);
			} else if (_sourceMode == SourceMode.SetByAPI) {
				SetWebCamTexture(_webCamTexture, true);
			}

			SetRenderMode(_renderMode, true);
			SetTargetRenderer(_targetRenderer);

			if (_playOnAwake) {
				Play();
			}
		}

		private void OnEnable() {
			if (_playOnAwake) {
				Play();
			}
		}

		private void OnDisable() {
			Stop();
		}

		private void Update() {
			_didUpdateTexture = false;
			if (_isPlaying) {
				if (_renderMode != RenderMode.None) {
					if (_webCamTexture != null) {
						if (_webCamTexture.didUpdateThisFrame) {
							_didUpdateTexture = _webCamTexture.didUpdateThisFrame;

							RenderTextureUtils.SetRenderTextureSize(ref _renderTexture, _webCamTexture.width, _webCamTexture.height);
							_texture.DiscardContents();
							Graphics.Blit(_webCamTexture, _texture);
						}
					}
				}
			}
			if (_didUpdateTexture) {
				frameReady.Invoke();
			}
		}

		public void Play() {
			_isPlaying = true;
		}

		public void Stop() {
			_isPlaying = false;
		}

		private void OnDestroy() {
			if (_renderTexture != null) {
				DestroyImmediate(_renderTexture);
			}
			if (_webCamStream != null) {
				_webCamStream.prepareCompleted -= SourceWebCamStreamPrepareCompleted;
			}
		}
	}
}