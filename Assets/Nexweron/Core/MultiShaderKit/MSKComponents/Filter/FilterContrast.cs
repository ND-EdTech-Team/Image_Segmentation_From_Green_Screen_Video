namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	public class FilterContrast : MSKComponentBase
	{
		[SerializeField, Range(0.0f, 10.0f)]
		private float _contrast = 1.0f;
		public float contrast {
			get { return _contrast; }
			set { if (SetStruct(ref _contrast, value)) _shaderMaterial.SetFloat("_Contrast", value); }
		}

		private RenderTexture _rtF; //render texture Filter

		private List<string> _availableShaders = new List<string>() { @"MSK/Filter/BlendOff/FilterContrast" };
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
		}

		public override void UpdateShaderProperties() {
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