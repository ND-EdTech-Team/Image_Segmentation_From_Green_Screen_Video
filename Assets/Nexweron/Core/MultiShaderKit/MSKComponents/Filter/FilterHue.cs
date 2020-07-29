namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	public class FilterHue : MSKComponentBase
	{
		[SerializeField, Range(0, 360)]
		private int _hue = 0;
		public int hue {
			get { return _hue; }
			set { if (SetStruct(ref _hue, value)) _shaderMaterial.SetFloat("_Hue", value); }
		}
	
		private RenderTexture _rtF; //render texture Filter

		private List<string> _availableShaders = new List<string>() { @"MSK/Filter/BlendOff/FilterHue" };
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
		}

		public override void UpdateShaderProperties() {
			_shaderMaterial.SetFloat("_Hue", _hue);
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