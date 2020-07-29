namespace Nexweron.Core.MSK
{
	using System.Collections.Generic;
	using UnityEngine;

	[RequireComponent(typeof(MSKControllerBase))]
	public class ChromaKey_BgColor : MSKComponentBase
	{
		[SerializeField]
		private Color _bgColor = Color.white;
		public Color bgColor {
			get { return _keyColor; }
			set { if (SetStruct(ref _bgColor, value)) _shaderMaterial.SetColor("_BgColor", value); }
		}

		[SerializeField]
		private Color _keyColor = Color.green;
		public Color keyColor {
			get { return _keyColor; }
			set { if (SetStruct(ref _keyColor, value)) _shaderMaterial.SetColor("_KeyColor", value); }
		}

		[SerializeField, Range(0.0f, 1.0f), Tooltip("Chroma max difference")]
		private float _dChroma = 0.5f;
		public float dChroma {
			get { return _dChroma; }
			set { if (SetStruct(ref _dChroma, value)) _shaderMaterial.SetFloat("_DChroma", value); }
		}

		[SerializeField, Range(0.0f, 1.0f), Tooltip("Chroma tolerance")]
		private float _dChromaT = 0.05f;
		public float dChromaT {
			get { return _dChromaT; }
			set { if (SetStruct(ref _dChromaT, value)) _shaderMaterial.SetFloat("_DChromaT", value); }
		}

		[SerializeField, Range(0.0f, 1.0f), Tooltip("Main(0) -> Bg(1)")]
		private float _chroma = 0.5f;
		public float chroma {
			get { return _chroma; }
			set { if (SetStruct(ref _chroma, value)) _shaderMaterial.SetFloat("_Chroma", value); }
		}

		[SerializeField, Range(0.0f, 1.0f), Tooltip("Main(0) -> Bg(1)")]
		private float _luma = 0.5f;
		public float luma {
			get { return _luma; }
			set { if (SetStruct(ref _luma, value)) _shaderMaterial.SetFloat("_Luma", value); }
		}

		[SerializeField, Range(0.0f, 1.0f), Tooltip("0 -> Chroma(1)")]
		private float _saturation = 1.0f;
		public float saturation {
			get { return _saturation; }
			set { if (SetStruct(ref _saturation, value)) _shaderMaterial.SetFloat("_Saturation", value); }
		}

		[SerializeField, Range(0.0f, 1.0f), Tooltip("0 -> 1")]
		private float _alpha = 1.0f;
		public float alpha {
			get { return _alpha; }
			set { if (SetStruct(ref _alpha, value)) _shaderMaterial.SetFloat("_Alpha", value); }
		}
	
		private RenderTexture _rtC; //render texture Chroma
	
		private List<string> _availableShaders = new List<string>() { @"MSK/ChromaKey/BlendOff/ChromaKey_BgColor" };
		public override List<string> GetAvailableShaders() {
			return _availableShaders;
		}
		
		public override void UpdateShaderProperties() {
			_shaderMaterial.SetColor("_BgColor", _bgColor);
			_shaderMaterial.SetColor("_KeyColor", _keyColor);
			_shaderMaterial.SetFloat("_DChroma", _dChroma);
			_shaderMaterial.SetFloat("_DChromaT", _dChromaT);
			_shaderMaterial.SetFloat("_Chroma", _chroma);
			_shaderMaterial.SetFloat("_Luma", _luma);
			_shaderMaterial.SetFloat("_Saturation", _saturation);
			_shaderMaterial.SetFloat("_Alpha", _alpha);
		}

		public override void UpdateSourceTexture() {
			RenderTextureUtils.SetRenderTextureSize(ref _rtC, _mskController.sourceTexture.width, _mskController.sourceTexture.height);
		}

		public override RenderTexture GetRender(Texture src) {
			_rtC.DiscardContents();
			Graphics.Blit(src, _rtC, _shaderMaterial);
			return _rtC;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (_rtC != null) {
				DestroyImmediate(_rtC);
			}
		}
	}
}