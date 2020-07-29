namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	public class FilterBrightness : MSKComponentBase
	{
		[SerializeField, Range(-1.0f, 10.0f)]
		private float _brightness = 0.0f;
		public float brightness {
			get { return _brightness; }
			set { if (SetStruct(ref _brightness, value)) _shaderMaterial.SetFloat("_Brightness", value); }
		}
	
		private RenderTexture _rtF; //render texture Filter

		private List<string> _availableShaders = new List<string>() { @"MSK/Filter/BlendOff/FilterBrightness" };
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
		}

		public override void UpdateShaderProperties() {
			_shaderMaterial.SetFloat("_Brightness", _brightness);
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