namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	public class FilterHSBC : MSKComponentBase
	{
		[SerializeField]
		private Color _baseColor = Color.white;
		public Color baseColor {
			get { return _baseColor; }
			set { if (SetStruct(ref _baseColor, value)) _shaderMaterial.SetColor("_BaseColor", value); }
		}

		[SerializeField]
		private Color _tintColor = Color.clear;
		public Color tintColor {
			get { return _tintColor; }
			set { if (SetStruct(ref _tintColor, value)) _shaderMaterial.SetColor("_TintColor", value); }
		}

		[SerializeField, Range(0, 360)]
		private int _hue = 0;
		public int hue {
			get { return _hue; }
			set { if (SetStruct(ref _hue, value)) _shaderMaterial.SetFloat("_Hue", value); }
		}

		[SerializeField, Range(-1.0f, 2.0f)]
		private float _saturation = 0.0f;
		public float saturation {
			get { return _saturation; }
			set { if (SetStruct(ref _saturation, value)) _shaderMaterial.SetFloat("_Saturation", value); }
		}

		[SerializeField, Range(-1.0f, 10.0f)]
		private float _brightness = 0.0f;
		public float brightness {
			get { return _brightness; }
			set { if (SetStruct(ref _brightness, value)) _shaderMaterial.SetFloat("_Brightness", value); }
		}

		[SerializeField, Range(0.0f, 10.0f)]
		private float _contrast = 1.0f;
		public float contrast {
			get { return _contrast; }
			set { if (SetStruct(ref _contrast, value)) _shaderMaterial.SetFloat("_Contrast", value); }
		}

		private RenderTexture _rtF; //render texture Filter

		private List<string> _availableShaders = new List<string>() { @"MSK/Filter/BlendOff/FilterHSBC" };
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
		}

		public override void UpdateShaderProperties() {
			_shaderMaterial.SetColor("_BaseColor", _baseColor);
			_shaderMaterial.SetColor("_TintColor", _tintColor);
			_shaderMaterial.SetFloat("_Hue", _hue);
			_shaderMaterial.SetFloat("_Saturation", _saturation);
			_shaderMaterial.SetFloat("_Brightness", _brightness);
			_shaderMaterial.SetFloat("_Contrast", _contrast);
		}
	
		public override void UpdateSourceTexture() {
			RenderTextureUtils.SetRenderTextureSize(ref _rtF, _mskController.sourceTexture.width, _mskController.sourceTexture.height);
		}

		public override RenderTexture GetRender(Texture src) {
			_rtF.DiscardContents();
			Graphics.Blit(src, _rtF, _shaderMaterial);
			return _rtF;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (_rtF != null) {
				DestroyImmediate(_rtF);
			}
		}
	}
}