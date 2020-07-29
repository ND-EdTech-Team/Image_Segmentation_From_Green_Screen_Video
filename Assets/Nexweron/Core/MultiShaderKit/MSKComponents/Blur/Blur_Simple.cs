namespace Nexweron.Core.MSK
{
	using UnityEngine;
	using System.Collections.Generic;

	public class Blur_Simple : MSKComponentBase
	{
		[SerializeField, Range(0.0f, 100.0f)]
		private float _blurXY = 3.0f;
		public float blurXY	{
			get { return _blurXY; }
			set { if (SetStruct(ref _blurXY, value)) UpdateShaderProperties(); }
		}

		private int _blurIterations = -1;

		private List<RenderTexture> _rtBs = new List<RenderTexture>(); //render textures Blur
		private List<string> _availableShaders = new List<string>() { @"MSK/Blur/BlendOff/Blur_010-101-010",
																	  @"MSK/Blur/BlendOff/Blur_010-111-010",
																	  @"MSK/Blur/BlendOff/Blur_101-000-101",
																	  @"MSK/Blur/BlendOff/Blur_101-010-101",
																	  @"MSK/Blur/BlendOff/Blur_111-101-111",
																	  @"MSK/Blur/BlendOff/Blur_111-111-111"};
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
		}

		public override void UpdateShaderProperties() {
			_shaderMaterial.SetFloat("_BlurOffsetX", _blurXY);
			_shaderMaterial.SetFloat("_BlurOffsetY", _blurXY);

			int blurIterations = Mathf.Min(4, (int)Mathf.Max(1, Mathf.Ceil(Mathf.Log(_blurXY, 2) - 2)));
			if (_blurIterations != blurIterations) {
				_blurIterations = blurIterations;
				UpdatePropertyBlurIterations();
			}
		}

		private void UpdatePropertyBlurIterations() {
			int w = 4;
			int h = 4;
			if (_mskController.sourceTexture != null) {
				w = _mskController.sourceTexture.width;
				h = _mskController.sourceTexture.height;
			}
			var count = _rtBs.Count;
			if (count < _blurIterations) {
				for (int i = count; i < _blurIterations; i++) {
					var rtB = RenderTextureUtils.CreateRenderTexture(w, h);
					_rtBs.Add(rtB);
				}
			} else {
				for (int i = count; i > _blurIterations; i--) {
					var index = i - 1;
					var rtB = _rtBs[index];
					DestroyImmediate(rtB);
					_rtBs.RemoveAt(index);
				}
			}
		}

		public override void UpdateSourceTexture() {
			int w = _mskController.sourceTexture.width;
			int h = _mskController.sourceTexture.height;
			for (int i = 0; i < _rtBs.Count; i++) {
				_rtBs[i] = RenderTextureUtils.ResizeRenderTexture(_rtBs[i], w, h);
			}
		}

		public override RenderTexture GetRender(Texture src) {
			RenderTexture rt = null;
			Texture rtS = src;
			for (var i = 1; i <= _blurIterations; i++) {
				_shaderMaterial.SetFloat("_BlurOffsetX", _blurXY / Mathf.Pow(2, i));
				_shaderMaterial.SetFloat("_BlurOffsetY", _blurXY / Mathf.Pow(2, i));
				rt = _rtBs[i - 1];
				rt.DiscardContents();
				Graphics.Blit(rtS, rt, _shaderMaterial);
				rtS = rt;
			}
			return rt;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			foreach (var rtB in _rtBs) {
				rtB.DiscardContents();
				DestroyImmediate(rtB);
			}
			_rtBs.Clear();
		}
	}
}
