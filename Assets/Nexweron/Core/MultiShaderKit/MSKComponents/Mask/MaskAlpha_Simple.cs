namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	public class MaskAlpha_Simple : MSKComponentBase
	{
		private RenderTexture _rtM; //render texture Mask

		private List<string> _availableShaders = new List<string>() { @"MSK/Mask/BlendOff/MaskAlpha_Simple" };
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
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
