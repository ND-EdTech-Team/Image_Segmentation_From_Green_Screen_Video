Shader "MSK/ChromaKey/BlendOff/ChromaKey_BgColor" {
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
		_BgColor("BgColor", Color) = (1,1,1,1)
		_KeyColor("KeyColor", Color) = (1,1,1,1)
		_DChroma("D Chroma", range(0.0, 1.0)) = 0.5
		_DChromaT("D Chroma Tolerance", range(0.0, 1.0)) = 0.05

		_Chroma("Chroma (Main -> Bg)", range(0.0, 1.0)) = 0.5
		_Luma("Luma (Main -> Bg)", range(0.0, 1.0)) = 0.5
		_Saturation("Saturation (0 -> Chroma)", range(0.0, 1.0)) = 1.0
		_Alpha("Alpha (Chroma -> Bg)", range(0.0, 1.0)) = 1.0
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	struct VS_OUT {
		fixed4 position:POSITION;
		fixed2 texcoord0:TEXCOORD0;
	};

	sampler2D _MainTex;
	fixed4 _MainTex_ST;
	
	fixed4 _BgColor;

	fixed4 _KeyColor;
	fixed _DChroma;
	fixed _DChromaT;

	fixed _Chroma;
	fixed _Luma;
	fixed _Saturation;
	fixed _Alpha;

	VS_OUT vert(appdata_base input) {
		VS_OUT o;
		o.position = UnityObjectToClipPos(input.vertex);
		o.texcoord0 = TRANSFORM_TEX(input.texcoord, _MainTex);
		return o;
	}

	fixed3 RGB_To_YCbCr(fixed3 RGB) {
		fixed Y = 0.299 * RGB.r + 0.587 * RGB.g + 0.114 * RGB.b;
		fixed Cb = 0.564 * (RGB.b - Y);
		fixed Cr = 0.713 * (RGB.r - Y);
		return fixed3(Cb, Cr, Y);
	}

	fixed3 YCbCr_To_RGB(fixed3 YCbCr) {
		fixed R = YCbCr.z + 1.402 * YCbCr.y;
		fixed G = YCbCr.z - 0.334 * YCbCr.x - 0.714 * YCbCr.y;
		fixed B = YCbCr.z + 1.772 * YCbCr.x;
		return fixed3(R, G, B);
	}
	fixed2 lerp3_2(fixed2 A, fixed2 B, fixed2 C, fixed v) {
		if (v < 0.5) {
			return lerp(A, B, 2*v);
		}
		else {
			return lerp(B, C, 2*(v-0.5));
		}
	}
	fixed lerp3_1(fixed A, fixed B, fixed C, fixed v) {
		if (v < 0.5) {
			return lerp(A, B, 2 * v);
		}
		else {
			return lerp(B, C, 2 * (v - 0.5));
		}
	}
	fixed4 frag(VS_OUT input) : SV_Target {
		fixed4 c = tex2D(_MainTex, input.texcoord0);
		if(c.a > 0) {
			fixed4 c_bg = _BgColor;

			fixed3 src_YCbCr = RGB_To_YCbCr(c.rgb);
			fixed3 target_YCbCr = RGB_To_YCbCr(c_bg.rgb);
			fixed3 key_YCbCr = RGB_To_YCbCr(_KeyColor);

			fixed dChroma = distance(src_YCbCr.xy, key_YCbCr.xy);
			if (dChroma < _DChroma) {
				fixed ta = 0;
				c.rgb = c_bg.rgb;
				if (dChroma > _DChroma - _DChromaT) {
					ta = (dChroma - _DChroma + _DChromaT) / _DChromaT;
					fixed2 cta = lerp(src_YCbCr.xy, target_YCbCr.xy, 1 - ta);
					fixed2 ct = lerp3_2(src_YCbCr.xy, cta, target_YCbCr.xy, _Chroma);

					fixed sa = length(cta);
					fixed s = lerp(0, sa, _Saturation);
					ct *= s / sa;

					fixed la = lerp(src_YCbCr.z, target_YCbCr.z, 1 - ta);
					fixed l = lerp3_1(src_YCbCr.z, la, target_YCbCr.z, _Luma);

					c.rgb = YCbCr_To_RGB(float3(ct.x, ct.y, l));
				}
				ta = lerp(ta, c.a, _Alpha);
				if(c.a > ta){
					c.a = ta;
				}
			}
		}
		return c;
	}
	ENDCG
	
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		Lighting Off
		ZWrite Off
		AlphaTest Off
		Blend Off

		Pass {
			CGPROGRAM
			  #pragma vertex vert
			  #pragma fragment frag
			ENDCG
		}
	}
	Fallback Off
}