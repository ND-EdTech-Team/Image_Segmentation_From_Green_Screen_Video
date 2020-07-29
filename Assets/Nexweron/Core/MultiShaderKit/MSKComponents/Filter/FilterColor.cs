namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	public class FilterColor : MSKComponentBase
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
	
		private RenderTexture _rtF; //render texture Filter

		private List<string> _availableShaders = new List<string>() { @"MSK/Filter/BlendOff/FilterColor" };
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
		}

		public override void UpdateShaderProperties() {
			_shaderMaterial.SetColor("_BaseColor", _baseColor);
			_shaderMaterial.SetColor("_TintColor", _tintColor);
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