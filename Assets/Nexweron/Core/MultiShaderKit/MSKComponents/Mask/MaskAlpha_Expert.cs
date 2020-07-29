namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	public class MaskAlpha_Expert : MSKComponentBase
	{
		[SerializeField, Range(0, 1)]
		private float _alphaEdge = 0.0f;
		public float alphaEdge {
			get { return _alphaEdge; }
			set { if (SetStruct(ref _alphaEdge, value)) _shaderMaterial.SetFloat("_AlphaEdge", value); }
		}

		[SerializeField, Range(0, 1)]
		private float _alphaPow = 1.0f;
		public float alphaPow {
			get { return _alphaPow; }
			set { if (SetStruct(ref _alphaPow, value)) _shaderMaterial.SetFloat("_AlphaPow", value); }
		}

		private RenderTexture _rtM; //render texture Mask

		private List<string> _availableShaders = new List<string>() { @"MSK/Mask/BlendOff/MaskAlpha_Expert" };
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
		}

		public override void UpdateShaderProperties() {
			_shaderMaterial.SetFloat("_AlphaEdge", _alphaEdge);
			_shaderMaterial.SetFloat("_AlphaPow", _alphaPow);
		}

		public override void UpdateSourceTexture() {
			RenderTextureUtils.SetRenderTextureSize(ref _rtM, _mskController.sourceTexture.width, _mskController.sourceTexture.height);
		}

		public override RenderTexture GetRender(Texture src) {
			_shaderMaterial.SetTexture("_MaskTex", src);
			_rtM.DiscardContents();
			Graphics.Blit(_mskController.sourceTexture, _rtM, _shaderMaterial);
			return _rtM;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (_rtM != null) {
				DestroyImmediate(_rtM);
			}
		}
	}
}