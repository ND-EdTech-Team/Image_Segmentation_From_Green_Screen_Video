using UnityEngine;
using Nexweron.Core.MSK;
using Nexweron.WebCamPlayer;

public class MSKBridgeWebCamPlayer : MonoBehaviour {

	public WebCamPlayer webCamPlayer;
	public MSKController mskController;

	private Texture _texture;
	
	private void UpdateProperties() {
		if (_texture != webCamPlayer.texture || _texture.width != webCamPlayer.texture.width || _texture.height != webCamPlayer.texture.height) {
			_texture = webCamPlayer.texture;
			mskController.SetSourceTexture(_texture);
		}
	}

	private void OnEnable() {
		if (webCamPlayer != null) {
			if (mskController != null) {
				webCamPlayer.frameReady += WebCamPlayerFrameReady;
			} else {
				Debug.LogError("MSKBridgeWebCamPlayer | mskController = null");
			}
		} else {
			Debug.LogError("MSKBridgeWebCamPlayer | webCamPlayer = null");
		}
	}

	private void OnDisable() {
		if (webCamPlayer != null) {
			webCamPlayer.frameReady -= WebCamPlayerFrameReady;
		}
	}
	
	private void WebCamPlayerFrameReady() {
		UpdateProperties();
		mskController.RenderOut(webCamPlayer.texture);
	}
}
