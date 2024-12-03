using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MarksAssets.ShareNSaveWebGL;
using status = MarksAssets.ShareNSaveWebGL.ShareNSaveWebGL.status;

public class ScreenShotter : MonoBehaviour {
	private Texture2D texture;
	private byte[] file;
	
	public GameObject shareGO, saveGO, ssGO;
	public Text text;
	
	public void callCaptureToTexture() {
		StartCoroutine(captureToTexture());
	}
	
    private IEnumerator captureToTexture()
    {
        yield return new WaitForEndOfFrame();
        texture = ScreenCapture.CaptureScreenshotAsTexture();
		file = texture.EncodeToPNG();
		ssGO.SetActive(true);
		shareGO.SetActive(true);
		saveGO.SetActive(true);
    }
	
	public void share() {
		Debug.Log(ShareNSaveWebGL.CanShare(file, "image/png"));
		ShareNSaveWebGL.Share(shareCallback, file, "image/png");

		//ShareNSaveWebGL.Share(shareCallback, "Module.ScreenshotWebGL.screenShotBlob");//share using my ScreenshotWebGL asset
		//ShareNSaveWebGL.Share(shareCallback, null, null, "MyURL", "MyTitle", "MyText");//share text only
	}
	
	public void save() {
		ShareNSaveWebGL.Save(file, "image/png");
	}

	public void shareCallback(status stat) {
		text.text = "status: " + stat.ToString();
		ssGO.SetActive(true);
		shareGO.SetActive(true);
		saveGO.SetActive(true);
	}

}