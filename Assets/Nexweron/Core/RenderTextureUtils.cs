namespace Nexweron.Core
{
	using UnityEngine;

	public class RenderTextureUtils : ScriptableObject
	{
		public static RenderTexture CreateRenderTexture(int width, int height) {
			return new RenderTexture(width, height, 0);
		}

		public static RenderTexture ResizeRenderTexture(RenderTexture rt, int width, int height) {
			SetRenderTextureSize(ref rt, width, height);
			return rt;
		}

		public static void SetRenderTextureSize(ref RenderTexture rt, int width, int height) {
			if (rt != null) {
				if (rt.width != width || rt.height != height) {
					if (rt.IsCreated()) {
						DestroyImmediate(rt);
						rt = CreateRenderTexture(width, height);
					} else {
						rt.width = width;
						rt.height = height;
					}
				}
			} else {
				rt = CreateRenderTexture(width, height);
			}
		}
	}
}
