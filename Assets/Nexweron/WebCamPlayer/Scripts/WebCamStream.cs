namespace Nexweron.WebCamPlayer
{

	using System.Collections;
	using System.Linq;
	using UnityEngine;
	
	public class WebCamStream : MonoBehaviour
	{
	
		public delegate void EventHandler();
		public event EventHandler prepareCompleted = delegate { };
	
		public enum DeviceMode {
			Auto,
			DeviceIndex,
			DeviceName
		}
	
		[SerializeField]
		private DeviceMode _deviceMode;
		public DeviceMode deviceMode	{
			get { return _deviceMode; }
		}
	
		[SerializeField]
		private string _deviceName;
		public string deviceName {
			get { return _deviceName; }
		}
	
		[SerializeField]
		private int _deviceIndex;
		public int deviceIndex {
			get { return _deviceIndex; }
		}
	
		public enum ResolutionMode {
			Auto,
			Manual
		}
		
		[SerializeField]
		private ResolutionMode _resolutionMode;
		public ResolutionMode resolutionMode {
			get { return _resolutionMode; }
		}
	
		[SerializeField]
		private int _requestedWidth = 640;
		public int requestedWidth {
			get { return _requestedWidth; }
		}
	
		[SerializeField]
		private int _requestedHeight = 480;
		public int requestedHeight {
			get { return _requestedHeight; }
		}
	
		public enum FPSMode {
			Auto,
			Manual
		}
	
		[SerializeField]
		private FPSMode _fpsMode;
		public FPSMode fpsMode {
			get { return _fpsMode; }
		}
	
		[SerializeField]
		private int _requestedFPS = 30;
		public int requestedFPS	{
			get { return _requestedFPS; }
		}

		[SerializeField]
		private bool _playOnAwake = false;
		public bool playOnAwake	{
			get { return _playOnAwake; }
			set { _playOnAwake = value; }
		}

		private WebCamDevice _webCamDevice;
		public WebCamDevice webCamDevice {
			get { return _webCamDevice; }
		}

		private WebCamTexture _webCamTexture;
		public WebCamTexture webCamTexture {
			get { return _webCamTexture; }
		}

		private bool _isAuthorization = false;
		public bool isAuthorization {
			get { return _isAuthorization; }
		}

		private bool _isPrepared = false;
		public bool isPrepared {
			get { return _isPrepared; }
		}
	
		private void Start() {
			if (_playOnAwake) {
				Play(true);
			}
		}
	
		private IEnumerator AuthorizeWebCam(bool isAutoPlay) {
			if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) {
				Debug.Log("WebCamStream | webCam authorization...");
				_isAuthorization = true;
				yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
			}
			_isAuthorization = false;
			if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {
				Debug.Log("WebCamStream | webCam authorized");
				CreateWebCamTexture();
				if (isAutoPlay) {
					Play();
				}
			} else {
				Debug.LogError("WebCamStream | webCam can't authorize");
			}
		}

		private void CreateWebCamTexture() {
			if (_webCamTexture != null) {
				DestroyImmediate(_webCamTexture);
			}
			if (_deviceMode == DeviceMode.Auto) {
				if (_resolutionMode == ResolutionMode.Auto) {
					_webCamTexture = new WebCamTexture();
				} else {
					if (_fpsMode == FPSMode.Auto) {
						_webCamTexture = new WebCamTexture(_requestedWidth, _requestedHeight);
					} else {
						_webCamTexture = new WebCamTexture(_requestedWidth, _requestedHeight, _requestedFPS);
					}
				}
			} else {
				string deviceName = null;
				if (_deviceMode == DeviceMode.DeviceName) {
					deviceName = _deviceName;
				} else if (_deviceMode == DeviceMode.DeviceIndex) {
					if (_deviceIndex < WebCamTexture.devices.Length) {
						deviceName = WebCamTexture.devices[_deviceIndex].name;
					} else {
						Debug.LogError("WebCamStream | Cannot find webcam device at index " + _deviceIndex);
					}
				}
			
				if (_resolutionMode == ResolutionMode.Auto) {
					_webCamTexture = new WebCamTexture(deviceName);
				} else {
					if (_fpsMode == FPSMode.Auto) {
						_webCamTexture = new WebCamTexture(deviceName, _requestedWidth, _requestedHeight);
					} else {
						_webCamTexture = new WebCamTexture(deviceName, _requestedWidth, _requestedHeight, _requestedFPS);
					}
				}
			}
			_webCamDevice = WebCamTexture.devices.FirstOrDefault(x => x.name == _webCamTexture.deviceName);
		}

		private void StopAllWebCamTextures(bool isExceptOwn = false) {
			var wcts = FindObjectsOfType<WebCamTexture>();
			foreach (var wct in wcts) {
				if (!isExceptOwn || _webCamTexture != wct) {
					if (_webCamTexture.deviceName == wct.deviceName) {
						wct.Stop();
					}
				}
			}
		}

		private void Update() {
			if (!_isPrepared) {
				if (_webCamTexture != null) {
					if (_webCamTexture.didUpdateThisFrame) {
						_isPrepared = true;
						prepareCompleted.Invoke();
					}	
				}
			}
		}

		public void Authorize(bool isAutoPlay = false) {
			if (!_isAuthorization) {
				StartCoroutine(AuthorizeWebCam(isAutoPlay));
			}
		}

		public void Play(bool isAutoAuthorize = false) {
			if (_webCamTexture != null) {
				if (!_webCamTexture.isPlaying) {
					StopAllWebCamTextures(true);
				}
				_webCamTexture.Play();
			} else if (isAutoAuthorize) {
				Authorize(true);
			}
		}

		public void Pause() {
			if (_webCamTexture != null) {
				_webCamTexture.Pause();
			} else {
				Debug.LogWarning("WebCamStream | webCam in not authorized");
			}
		}

		public void Stop() {
			if (_webCamTexture != null) {
				_webCamTexture.Stop();
				DestroyImmediate(_webCamTexture);
			} else {
				Debug.LogWarning("WebCamStream | webCam in not authorized");
			}
			_isPrepared = false;
		}
	}
}
