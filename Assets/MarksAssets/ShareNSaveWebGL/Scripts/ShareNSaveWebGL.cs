using System;
using System.Runtime.InteropServices;
using AOT;

namespace MarksAssets.ShareNSaveWebGL {

	public class ShareNSaveWebGL {
		public enum status {Success, NotAllowedError, TypeError, AbortError, DataError, InvalidStateError, CantShareError, NotABlobError, WebShareUnsupportedError, WebShare2UnsupportedError}

		private static event Action<status> shareEvent;

		[DllImport("__Internal", EntryPoint="ShareNSaveWebGL_Share")]
		private static extern void ShareNSaveWebGL_Share(Action<status> shareCallback, byte[] file, int fileSize, string mimeType, string fileName = null, string url=null, string title=null, string text=null);
		[DllImport("__Internal", EntryPoint="ShareNSaveWebGL_ShareBlob")]
		private static extern void ShareNSaveWebGL_ShareBlob(Action<status> shareCallback, string blobPropertyPath, string fileName=null, string url=null, string title=null, string text=null);
		
		[DllImport("__Internal", EntryPoint="ShareNSaveWebGL_Save")]
		private static extern void ShareNSaveWebGL_Save(byte[] file, int fileSize, string mimeType, string fileName = null);
		[DllImport("__Internal", EntryPoint="ShareNSaveWebGL_SaveBlob")]
		private static extern void ShareNSaveWebGL_SaveBlob(string blobPropertyPath, string fileName = null);
		[DllImport("__Internal", EntryPoint="ShareNSaveWebGL_CanShare")]
		private static extern status ShareNSaveWebGL_CanShare(byte[] file, int fileSize, string mimeType, string fileName = null, string url=null, string title=null, string text=null);
		[DllImport("__Internal", EntryPoint="ShareNSaveWebGL_CanShareBlob")]
		private static extern status ShareNSaveWebGL_CanShareBlob(string blobPropertyPath, string fileName = null, string url=null, string title=null, string text=null);

		[MonoPInvokeCallback(typeof(Action<status>))]
        private static void shareCallback(status status) {
            shareEvent?.Invoke(status);
            shareEvent = null;
        }

		public static void Share(Action<status> callback, byte[] file, string mimeType, string fileName = null, string url = null, string title = null, string text = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
                shareEvent += callback;
                ShareNSaveWebGL_Share(shareCallback, file, file.Length, mimeType, fileName, url, title, text);
            #endif
		}

		public static void Share(Action<status> callback, string blobPropertyPath, string fileName = null, string url = null, string title = null, string text = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
                shareEvent += callback;
                ShareNSaveWebGL_ShareBlob(shareCallback, blobPropertyPath, fileName, url, title, text);
            #endif
		}

		public static void Save(byte[] file, string mimeType, string fileName = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				ShareNSaveWebGL_Save(file, file.Length, mimeType, fileName);
			#endif
		}

		public static void Save(string blobPropertyPath, string fileName = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				ShareNSaveWebGL_SaveBlob(blobPropertyPath, fileName);
			#endif
		}

		public static status CanShare(byte[] file, string mimeType, string fileName = null, string url = null, string title = null, string text = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				return ShareNSaveWebGL_CanShare(file, file.Length, mimeType, fileName, url, title, text);
			#else
				return status.Success;
			#endif
		}

		public static status CanShare(string blobPropertyPath, string fileName = null, string url = null, string title = null, string text = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
				return ShareNSaveWebGL_CanShareBlob(blobPropertyPath, fileName, url, title, text);
			#else
				return status.Success;
			#endif
		}
	}
}
