using UnityEngine;
using UnityEngine.UI;
using status = MarksAssets.DeviceCameraWebGL.DeviceCameraWebGL.status;
using MediaDeviceInfo = MarksAssets.DeviceCameraWebGL.DeviceCameraWebGL.MediaDeviceInfo;

namespace MarksAssets.DeviceCameraWebGL {
	public class Callbacks : MonoBehaviour {
		[SerializeField] private Text textStatus;
		[SerializeField] private GameObject buttons;
		
		public void setText(status stat) {
			textStatus.text = stat.ToString();
			if (stat == status.Success)
				buttons.SetActive(true);
			else
				buttons.SetActive(false);
		}

		public void setDevicesText(MediaDeviceInfo[] devices) {
			string deviceInfo = "";
			for (var i = 0; i < devices.Length; ++i) {
				deviceInfo += JsonUtility.ToJson(devices[i]);
				deviceInfo += ",";
			}

			textStatus.text = deviceInfo;
		}

		public void getDevices() {
			DeviceCameraWebGL.getDevices(setDevicesText);
		}
	}
}
