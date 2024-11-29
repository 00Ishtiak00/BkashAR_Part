// Starts the default camera and assigns the texture to the current renderer

using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace MarksAssets.DeviceCameraWebGL {
	public class DeviceCameraWebGL {
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_start")]
        private static extern void DeviceCameraWebGL_start(Action<status> callback, string constraints, string selector, in byte canUpdateTexture);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_stop")]
        private static extern void DeviceCameraWebGL_stop(Action callback, string selector);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_getDevices")]
        private static extern void DeviceCameraWebGL_getDevices(Action<string> callback, string selector);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_isPlaying")]
        private static extern bool DeviceCameraWebGL_isPlaying(string selector);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_isDesktop")]
        private static extern bool DeviceCameraWebGL_isDesktop(string selector);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_createVideoElement")]
        private static extern void DeviceCameraWebGL_createVideoElement(Action<uint, uint> resizeCallback, string id, string css);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_assignCSSToVideo")]
        private static extern void DeviceCameraWebGL_assignCSSToVideo(string selector, string css);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_assignResizeEventToVideo")]
        private static extern void DeviceCameraWebGL_assignResizeEventToVideo(Action<uint, uint> callback, string selector);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_assignPlayingEventToVideo")]
        private static extern void DeviceCameraWebGL_assignPlayingEventToVideo(Action<status> callback, string selector, in byte canUpdateTexture);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_getCurrentFacingMode")]
        private static extern string DeviceCameraWebGL_getCurrentFacingMode(string selector);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_DestroyVideo")]
        private static extern void DeviceCameraWebGL_DestroyVideo(string selector);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_updateTexture")]
        private static extern void DeviceCameraWebGL_updateTexture(string selector, IntPtr texturePtr);
		[DllImport("__Internal", EntryPoint="DeviceCameraWebGL_waitForElement")]
        private static extern void DeviceCameraWebGL_waitForElement(Action callback, string selector);

		public enum status {
            Success = 0,
            AbortError = 1,
            NotAllowedError  = 2,
			NotFoundError = 3,
			NotReadableError = 4,
			OverconstrainedError = 5,
			SecurityError = 6,
			TypeError = 7
        };

		[Serializable]
		public class MediaDeviceInfo {
			public string deviceId;
			public string groupId;
			public string kind;
			public string label;
		}

		[Serializable]
		private class Wrapper<T> {
			public T[] devices;
		}

		public static readonly byte canUpdateTexture = 0;

		public static string id = "DeviceCameraWebGL";
		public static string selector = $"#{id}";

		private static event Action<status> startCallbackEvent;
		private static event Action<MediaDeviceInfo[]> getDevicesCallbackEvent;
		private static event Action stopCallbackEvent;
		private static Action<uint, uint> resizeCallbackEvent;


        [MonoPInvokeCallback(typeof(Action<status>))]
        private static void startCallback(status status) {
            startCallbackEvent?.Invoke(status);
            startCallbackEvent = null;
        }

		[MonoPInvokeCallback(typeof(Action<string>))]
        private static void getDevicesCallback(string mediaDevices) {
			MediaDeviceInfo[] devices = JsonUtility.FromJson<Wrapper<MediaDeviceInfo>>(mediaDevices).devices;
            getDevicesCallbackEvent?.Invoke(devices);
            getDevicesCallbackEvent = null;
        }

		[MonoPInvokeCallback(typeof(Action))]
        private static void stopCallback() {
            stopCallbackEvent?.Invoke();
            stopCallbackEvent = null;
        }

		[MonoPInvokeCallback(typeof(Action<uint, uint>))]
        private static void resizeCallback(uint videoWidth, uint videoHeight) {
            resizeCallbackEvent?.Invoke(videoWidth, videoHeight);
        }

		public static void start(string constraints, Action<status> callback = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				if (callback != null) {
					startCallbackEvent += callback;
					DeviceCameraWebGL_start(startCallback, constraints, selector, in canUpdateTexture);
				} else {
					DeviceCameraWebGL_start(null, constraints, selector, in canUpdateTexture);
				}
			#endif
		}

		public static void startFrontCamera(Action<status> callback = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				string constraints = $"{{\"video\": {{\"width\": {{\"min\": 1280, \"ideal\": 1920, \"max\": 2560}},\"height\": {{\"min\": 720, \"ideal\": 1080, \"max\": 1440}}, \"facingMode\": \"user\"}}}}";
				//string constraints = $"{{\"video\": {{ \"width\": {{ \"ideal\": 4096 }}, \"height\": {{ \"ideal\": 2160 }} , \"facingMode\": \"user\" }}}}";
				if (callback != null) {
					startCallbackEvent += callback;
					DeviceCameraWebGL_start(startCallback, constraints, selector, in canUpdateTexture);
				} else {
					DeviceCameraWebGL_start(null, constraints, selector, in canUpdateTexture);
				}
			#endif
		}

		public static void startRearCamera(Action<status> callback = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				string constraints = $"{{\"video\": {{\"width\": {{\"min\": 1280, \"ideal\": 1920, \"max\": 2560}},\"height\": {{\"min\": 720, \"ideal\": 1080, \"max\": 1440}}, \"facingMode\": \"environment\"}}}}";
				//string constraints = $"{{\"video\": {{ \"width\": {{ \"ideal\": 4096 }}, \"height\": {{ \"ideal\": 2160 }} , \"facingMode\": \"environment\" }}}}";
				if (callback != null) {
					startCallbackEvent += callback;
					DeviceCameraWebGL_start(startCallback, constraints, selector, in canUpdateTexture);
				} else {
					DeviceCameraWebGL_start(null, constraints, selector, in canUpdateTexture);
				}
			#endif
		}

		public static void stop(Action callback = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				if (callback != null) {
					stopCallbackEvent += callback;
					DeviceCameraWebGL_stop(stopCallback, selector);
				} else {
					DeviceCameraWebGL_stop(null, selector);
				}
			#endif
		}

		public static void waitForElement(Action callback) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				stopCallbackEvent += callback;
				DeviceCameraWebGL_waitForElement(stopCallback, selector);
			#endif
		}

		public static void getDevices(Action<MediaDeviceInfo[]> devices) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				getDevicesCallbackEvent += devices;
				DeviceCameraWebGL_getDevices(getDevicesCallback, selector);
			#endif
		}

		public static bool isPlaying() {
			#if UNITY_WEBGL && !UNITY_EDITOR
				return DeviceCameraWebGL_isPlaying(selector);
			#else
				return false;
			#endif
		}

		public static bool isDesktop() {
			#if UNITY_WEBGL && !UNITY_EDITOR
				return DeviceCameraWebGL_isDesktop(selector);
			#else
				return false;
			#endif
		}

		public static void switchCamera(Action<status> callback = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				string facingMode = DeviceCameraWebGL.getCurrentFacingMode();
				if (facingMode == "user") {
					DeviceCameraWebGL.startRearCamera(callback);
				} else {
					DeviceCameraWebGL.startFrontCamera(callback);
				}
			#endif
		}

		public static void createVideoElement(Action<uint, uint> callback = null, string css = "pointer-events: none; width: 1px; height: 1px; z-index: -1000; position: absolute; left: 0; top: 0;") {
			#if UNITY_WEBGL && !UNITY_EDITOR
				resizeCallbackEvent = null;
				if (callback != null) {	
					resizeCallbackEvent += callback;
					DeviceCameraWebGL_createVideoElement(resizeCallback, id, css);
				} else {
					DeviceCameraWebGL_createVideoElement(null, id, css);
				}
			#endif
		}

		public static void assignResizeEventToVideo(Action<uint, uint> callback) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				resizeCallbackEvent = null;
				resizeCallbackEvent += callback;
				DeviceCameraWebGL_assignResizeEventToVideo(resizeCallback, selector);
			#endif
		}

		public static void assignCSSToVideo(string css = "pointer-events: none; width: 1px; height: 1px; z-index: -1000; position: absolute; left: 0; top: 0;") {
			#if UNITY_WEBGL && !UNITY_EDITOR
				DeviceCameraWebGL_assignCSSToVideo(selector, css);
			#endif
		}

		public static void assignPlayingEventToVideo(Action<status> callback = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				if (callback != null) {
					startCallbackEvent += callback;
					DeviceCameraWebGL_assignPlayingEventToVideo(startCallback, selector, in canUpdateTexture);
				} else {
					DeviceCameraWebGL_assignPlayingEventToVideo(null, selector, in canUpdateTexture);
				}
			#endif
		}

		public static string getCurrentFacingMode() {
			#if UNITY_WEBGL && !UNITY_EDITOR
				return DeviceCameraWebGL_getCurrentFacingMode(selector);
			#else
				return "none";
			#endif
		}

		public static void destroyVideo() {
			#if UNITY_WEBGL && !UNITY_EDITOR
				DeviceCameraWebGL_DestroyVideo(selector);
			#endif
		}

		public static void updateTexture(IntPtr texturePtr) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				DeviceCameraWebGL_updateTexture(selector, texturePtr);
			#endif
		}

	}
}