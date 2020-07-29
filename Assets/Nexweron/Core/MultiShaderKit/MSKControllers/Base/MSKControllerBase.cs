namespace Nexweron.Core.MSK
{
	using UnityEngine;
	using System.Collections.Generic;

	abstract public class MSKControllerBase : MonoBehaviour
	{
		[SerializeField]
		protected List<MSKComponentBase> _components = new List<MSKComponentBase>();

		private Texture _sourceTexture;
		public Texture sourceTexture {
			get { return _sourceTexture; }
		}

		protected RenderTexture _texture;
		public RenderTexture texture {
			get { return _texture; }
		}

		private Texture _textureNull; // Texture null
		private bool _inited = false;

		protected virtual void Awake() {
			CheckInit();
		}

		protected void Init() {
			Texture2D tex2D = new Texture2D(1, 1);
			tex2D.SetPixels(new Color[1] { Color.red });
			tex2D.Apply();
			_textureNull = tex2D;

			_inited = true;

			foreach (var component in _components) {
				if (component != null) {
					component.Init(this);
				}
			}
		}

		protected void CheckInit() {
			if (!_inited) {
				Init();
			}
		}

		public void SetSourceTexture(Texture texture) {
			CheckInit();

			if (texture != null) {
				_sourceTexture = texture;
			} else {
				_sourceTexture = _textureNull;
			}

			RenderTextureUtils.SetRenderTextureSize(ref _texture, _sourceTexture.width, _sourceTexture.height);

			foreach (var component in _components) {
				if (component != null) {
					component.UpdateSourceTexture();
				}
			}
		}

		public RenderTexture RenderIn() {
			CheckInit();

			var t = _sourceTexture;
			foreach (var component in _components) {
				if (component != null && component.isActiveAndEnabled) {
					t = component.GetRender(t);
				}
			}
			_texture.DiscardContents();
			Graphics.Blit(t, _texture);

			return _texture;
		}

		public void RenderOut(RenderTexture rt) {
			CheckInit();

			Texture t = _sourceTexture;
			foreach (var component in _components) {
				if (component != null && component.isActiveAndEnabled) {
					t = component.GetRender(t);
				}
			}
			if (rt != t) {
				rt.DiscardContents();
				Graphics.Blit(t, rt);
			}
		}

		protected void OnDestroy() {
			if (_textureNull != null) { DestroyImmediate(_textureNull); }
			if (_texture != null) { DestroyImmediate(_texture); }
			_inited = false;
		}
	}
}
