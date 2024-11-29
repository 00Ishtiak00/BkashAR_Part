using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using System.Collections;

namespace MarksAssets.RecorderWebGL {
    public class RecorderWebGL {
        [DllImport("__Internal", EntryPoint="CreateMediaRecorderNoFrameRate_RecorderWebGL")]
        private static extern void CreateMediaRecorderNoFrameRate_RecorderWebGL(Action<status> callback = null, string options = null, bool microphone = true, bool ingameAudio = true, string target = null);
        [DllImport("__Internal", EntryPoint="CreateMediaRecorder_RecorderWebGL")]
        private static extern void CreateMediaRecorder_RecorderWebGL(Action<status> callback = null, string options = null, bool microphone = true, bool ingameAudio = true, double frameRequestRate = 0, string target = null);
        [DllImport("__Internal", EntryPoint="Start_RecorderWebGL")]
        private static extern void Start_RecorderWebGL(uint timeslice, Action callback = null);
        [DllImport("__Internal", EntryPoint="StartNoTimeslice_RecorderWebGL")]
        private static extern void StartNoTimeslice_RecorderWebGL(Action callback = null);
        [DllImport("__Internal", EntryPoint="Stop_RecorderWebGL")]
        private static extern void Stop_RecorderWebGL(Action callback = null, bool save = false, string fileName = null);
        [DllImport("__Internal", EntryPoint="StopDataURL_RecorderWebGL")]
        private static extern void StopDataURL_RecorderWebGL(Action<string> callback);
        [DllImport("__Internal", EntryPoint="StopObjectURL_RecorderWebGL")]
        private static extern void StopObjectURL_RecorderWebGL(Action<string> callback);
        [DllImport("__Internal", EntryPoint="StopByteArray_RecorderWebGL")]
        private static extern void StopByteArray_RecorderWebGL(Action<byte[], int> callback);
        [DllImport("__Internal", EntryPoint="Resume_RecorderWebGL")]
        private static extern void Resume_RecorderWebGL(Action callback = null);
        [DllImport("__Internal", EntryPoint="Pause_RecorderWebGL")]
        private static extern void Pause_RecorderWebGL(Action callback = null);
        [DllImport("__Internal", EntryPoint="ToDataURL_RecorderWebGL")]
        private static extern void ToDataURL_RecorderWebGL(Action<string> callback);
        [DllImport("__Internal", EntryPoint="ToObjectURL_RecorderWebGL")]
        private static extern void ToObjectURL_RecorderWebGL(Action<string> callback);
        [DllImport("__Internal", EntryPoint="RevokeObjectURL_RecorderWebGL")]
        private static extern void RevokeObjectURL_RecorderWebGL(string url);
        [DllImport("__Internal", EntryPoint="ToByteArray_RecorderWebGL")]
        private static extern void ToByteArray_RecorderWebGL(Action<byte[], int> callback);
        [DllImport("__Internal", EntryPoint="GetRecordingFileExtension_RecorderWebGL")]
        private static extern string GetRecordingFileExtension_RecorderWebGL();
        [DllImport("__Internal", EntryPoint="Save_RecorderWebGL")]
        private static extern void Save_RecorderWebGL(string fileName);
        [DllImport("__Internal", EntryPoint="GetCanvasWidth_RecorderWebGL")]
        private static extern uint GetCanvasWidth_RecorderWebGL();
        [DllImport("__Internal", EntryPoint="GetCanvasHeight_RecorderWebGL")]
        private static extern uint GetCanvasHeight_RecorderWebGL();
        [DllImport("__Internal", EntryPoint="IsTypeSupported_RecorderWebGL")]
        private static extern bool IsTypeSupported_RecorderWebGL(string type);
        [DllImport("__Internal", EntryPoint="GetState_RecorderWebGL")]
        private static extern RecordingState GetState_RecorderWebGL();
        [DllImport("__Internal", EntryPoint="GetVideoBitsPerSecond_RecorderWebGL")]
        private static extern uint GetVideoBitsPerSecond_RecorderWebGL();
        [DllImport("__Internal", EntryPoint="GetAudioBitsPerSecond_RecorderWebGL")]
        private static extern uint GetAudioBitsPerSecond_RecorderWebGL();
        [DllImport("__Internal", EntryPoint="CanRecord_RecorderWebGL")]
        private static extern bool CanRecord_RecorderWebGL();
        [DllImport("__Internal", EntryPoint="RequestFrame_RecorderWebGL")]
        private static extern void RequestFrame_RecorderWebGL();
        [DllImport("__Internal", EntryPoint="Destroy_RecorderWebGL")]
        private static extern void Destroy_RecorderWebGL(Action callback = null);
        [DllImport("__Internal", EntryPoint="Discard_RecorderWebGL")]
        private static extern void Discard_RecorderWebGL();
        
        public class MediaRecorderOptions {//https://www.w3.org/TR/mediastream-recording/#mediarecorderoptions-section
            public readonly string mimeType;
            public readonly uint? audioBitsPerSecond;
            public readonly uint? videoBitsPerSecond;
            public readonly uint? bitsPerSecond;

            public MediaRecorderOptions(string mimeType = null, uint? audioBitsPerSecond = null, uint? videoBitsPerSecond = null, uint? bitsPerSecond = null) {
                this.mimeType = mimeType;
                this.audioBitsPerSecond = audioBitsPerSecond;
                this.videoBitsPerSecond = videoBitsPerSecond;
                this.bitsPerSecond = bitsPerSecond;
            }

        }

        public enum status {
            Success = 0,
            NotAllowedError = 1,
            MediaRecorderAPIUnsupportedError = 2
        };

        public enum RecordingState {
            mediaRecorderDoesntExist = -1,
            inactive = 0,
            recording = 1,
            paused = 2,
            stopped = 3,
        };

        private static event Action<status> CreateMediaRecorderCallbackEvent;
        private static event Action<string> StopDataURLorObjectURLCallbackEvent;
        private static event Action<byte[], int> StopByteArrayCallbackEvent;
        private static event Action parameterlessEvent;

        [MonoPInvokeCallback(typeof(Action<status>))]
        private static void CreateMediaRecorderCallback(status status) {
            CreateMediaRecorderCallbackEvent?.Invoke(status);
            CreateMediaRecorderCallbackEvent = null;
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void StopDataURLorObjectURLCallback(string dataURL) {
            StopDataURLorObjectURLCallbackEvent?.Invoke(dataURL);
            StopDataURLorObjectURLCallbackEvent = null;
        }

        [MonoPInvokeCallback(typeof(Action<byte[], int>))]
        private static void StopByteArrayCallback([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)] byte[] bytes, int size) {
            StopByteArrayCallbackEvent?.Invoke(bytes, size);
            StopByteArrayCallbackEvent = null;
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void parameterlessCallback() {
            parameterlessEvent?.Invoke();
            parameterlessEvent = null;
        }

        public static void CreateMediaRecorder(Action<status> callback = null, MediaRecorderOptions recorderOptions = null, bool microphone = true, bool ingameAudio = true, double? frameRequestRate = null, string target = null) {
#if UNITY_WEBGL && !UNITY_EDITOR
                if (callback != null) CreateMediaRecorderCallbackEvent += callback;

                string recorderOptionsJSONString =  null;
                
                if (recorderOptions != null) {
                    if (recorderOptions.mimeType != null && recorderOptions.audioBitsPerSecond != null && recorderOptions.videoBitsPerSecond != null && recorderOptions.bitsPerSecond != null)
                        recorderOptionsJSONString =  $"{{\"mimeType\": \"{recorderOptions.mimeType}\", \"audioBitsPerSecond\": {recorderOptions.audioBitsPerSecond}, \"videoBitsPerSecond\": {recorderOptions.videoBitsPerSecond}, \"bitsPerSecond\": {recorderOptions.bitsPerSecond}}}";
                    else if (recorderOptions.mimeType != null && recorderOptions.audioBitsPerSecond != null && recorderOptions.videoBitsPerSecond != null && recorderOptions.bitsPerSecond == null)
                        recorderOptionsJSONString =  $"{{\"mimeType\": \"{recorderOptions.mimeType}\", \"audioBitsPerSecond\": {recorderOptions.audioBitsPerSecond}, \"videoBitsPerSecond\": {recorderOptions.videoBitsPerSecond}}}";
                    else if (recorderOptions.mimeType != null && recorderOptions.audioBitsPerSecond != null && recorderOptions.videoBitsPerSecond == null && recorderOptions.bitsPerSecond != null)
                        recorderOptionsJSONString =  $"{{\"mimeType\": \"{recorderOptions.mimeType}\", \"audioBitsPerSecond\": {recorderOptions.audioBitsPerSecond}, \"bitsPerSecond\": {recorderOptions.bitsPerSecond}}}";
                    else if (recorderOptions.mimeType != null && recorderOptions.audioBitsPerSecond != null && recorderOptions.videoBitsPerSecond == null && recorderOptions.bitsPerSecond == null)
                        recorderOptionsJSONString =  $"{{\"mimeType\": \"{recorderOptions.mimeType}\", \"audioBitsPerSecond\": {recorderOptions.audioBitsPerSecond}}}";
                    else if (recorderOptions.mimeType != null && recorderOptions.audioBitsPerSecond == null && recorderOptions.videoBitsPerSecond != null && recorderOptions.bitsPerSecond != null)
                        recorderOptionsJSONString =  $"{{\"mimeType\": \"{recorderOptions.mimeType}\", \"videoBitsPerSecond\": {recorderOptions.videoBitsPerSecond}, \"bitsPerSecond\": {recorderOptions.bitsPerSecond}}}";
                    else if (recorderOptions.mimeType != null && recorderOptions.audioBitsPerSecond == null && recorderOptions.videoBitsPerSecond != null && recorderOptions.bitsPerSecond == null)
                        recorderOptionsJSONString =  $"{{\"mimeType\": \"{recorderOptions.mimeType}\", \"videoBitsPerSecond\": {recorderOptions.videoBitsPerSecond} }}";
                    else if (recorderOptions.mimeType != null && recorderOptions.audioBitsPerSecond == null && recorderOptions.videoBitsPerSecond == null && recorderOptions.bitsPerSecond != null)
                        recorderOptionsJSONString =  $"{{\"mimeType\": \"{recorderOptions.mimeType}\", \"bitsPerSecond\": {recorderOptions.bitsPerSecond}}}";
                    else if (recorderOptions.mimeType != null && recorderOptions.audioBitsPerSecond == null && recorderOptions.videoBitsPerSecond == null && recorderOptions.bitsPerSecond == null)
                        recorderOptionsJSONString =  $"{{\"mimeType\": \"{recorderOptions.mimeType}\" }}";
                    else if (recorderOptions.mimeType == null && recorderOptions.audioBitsPerSecond != null && recorderOptions.videoBitsPerSecond != null && recorderOptions.bitsPerSecond != null)
                        recorderOptionsJSONString =  $"{{ \"audioBitsPerSecond\": {recorderOptions.audioBitsPerSecond}, \"videoBitsPerSecond\": {recorderOptions.videoBitsPerSecond}, \"bitsPerSecond\": {recorderOptions.bitsPerSecond}}}";
                    else if (recorderOptions.mimeType == null && recorderOptions.audioBitsPerSecond != null && recorderOptions.videoBitsPerSecond != null && recorderOptions.bitsPerSecond == null)
                        recorderOptionsJSONString =  $"{{ \"audioBitsPerSecond\": {recorderOptions.audioBitsPerSecond}, \"videoBitsPerSecond\": {recorderOptions.videoBitsPerSecond} }}";
                    else if (recorderOptions.mimeType == null && recorderOptions.audioBitsPerSecond != null && recorderOptions.videoBitsPerSecond == null && recorderOptions.bitsPerSecond != null)
                        recorderOptionsJSONString =  $"{{\"audioBitsPerSecond\": {recorderOptions.audioBitsPerSecond}, \"bitsPerSecond\": {recorderOptions.bitsPerSecond}}}";
                    else if (recorderOptions.mimeType == null && recorderOptions.audioBitsPerSecond == null && recorderOptions.videoBitsPerSecond != null && recorderOptions.bitsPerSecond != null)
                        recorderOptionsJSONString =  $"{{\"videoBitsPerSecond\": {recorderOptions.videoBitsPerSecond}, \"bitsPerSecond\": {recorderOptions.bitsPerSecond}}}";
                    else if (recorderOptions.mimeType == null && recorderOptions.audioBitsPerSecond != null && recorderOptions.videoBitsPerSecond == null && recorderOptions.bitsPerSecond == null)
                        recorderOptionsJSONString =  $"{{ \"audioBitsPerSecond\": {recorderOptions.audioBitsPerSecond} }}";
                    else if (recorderOptions.mimeType == null && recorderOptions.audioBitsPerSecond == null && recorderOptions.videoBitsPerSecond == null && recorderOptions.bitsPerSecond != null)
                        recorderOptionsJSONString =  $"{{\"bitsPerSecond\": {recorderOptions.bitsPerSecond}}}";
                    else if (recorderOptions.mimeType == null && recorderOptions.audioBitsPerSecond == null && recorderOptions.videoBitsPerSecond != null && recorderOptions.bitsPerSecond == null)
                        recorderOptionsJSONString =  $"{{\"videoBitsPerSecond\": {recorderOptions.videoBitsPerSecond}}}";
                }


                if (frameRequestRate == null) {
                    if (callback != null)
                        CreateMediaRecorderNoFrameRate_RecorderWebGL(CreateMediaRecorderCallback, recorderOptionsJSONString, microphone, ingameAudio, target);
                    else
                        CreateMediaRecorderNoFrameRate_RecorderWebGL(null, recorderOptionsJSONString, microphone, ingameAudio, target);
                }   else {
                    if (callback != null)
                        CreateMediaRecorder_RecorderWebGL(CreateMediaRecorderCallback, recorderOptionsJSONString, microphone, ingameAudio, (double)frameRequestRate < 0 ? -(double)frameRequestRate : (double)frameRequestRate, target);
                    else
                        CreateMediaRecorder_RecorderWebGL(null, recorderOptionsJSONString, microphone, ingameAudio, (double)frameRequestRate < 0 ? -(double)frameRequestRate : (double)frameRequestRate, target);
                }
                    
#endif
        }

        public static void Start(Action callback = null, uint? timeslice = null) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                if (timeslice == null) {
                    if (callback == null)
                        StartNoTimeslice_RecorderWebGL();
                    else {
                        parameterlessEvent += callback;
                        StartNoTimeslice_RecorderWebGL(parameterlessCallback);
                    }   
                }
                else {
                    if (callback == null)
                        Start_RecorderWebGL((uint)timeslice);
                    else {
                        parameterlessEvent += callback;
                        Start_RecorderWebGL((uint)timeslice, parameterlessCallback);
                    }
                }
            #endif
        }

        public static void Stop(Action callback = null, bool save = false, string fileName = null) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                if (callback != null) {
                    parameterlessEvent += callback;
                    Stop_RecorderWebGL(parameterlessCallback, save, fileName);
                }
                else
                    Stop_RecorderWebGL(null, save, fileName);
            #endif
        }

        public static void Stop(Action<string> dataURLorObjectURLCallback, bool isDataURL = true) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                StopDataURLorObjectURLCallbackEvent += dataURLorObjectURLCallback;
                if (isDataURL)
                    StopDataURL_RecorderWebGL(StopDataURLorObjectURLCallback);
                else
                    StopObjectURL_RecorderWebGL(StopDataURLorObjectURLCallback);
            #endif
        }

        public static void Stop(Action<byte[], int> byteArrayCallback) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                StopByteArrayCallbackEvent += byteArrayCallback;   
                StopByteArray_RecorderWebGL(StopByteArrayCallback);
            #endif
        }

        public static void ToDataURL(Action<string> dataURLCallback) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                StopDataURLorObjectURLCallbackEvent += dataURLCallback;
                ToDataURL_RecorderWebGL(StopDataURLorObjectURLCallback);
            #endif
        }

        public static void ToObjectURL(Action<string> objectURLCallback) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                StopDataURLorObjectURLCallbackEvent += objectURLCallback;
                ToObjectURL_RecorderWebGL(StopDataURLorObjectURLCallback);
            #endif
        }

        public static void ToByteArray(Action<byte[], int> byteArrayCallback) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                StopByteArrayCallbackEvent += byteArrayCallback;   
                ToByteArray_RecorderWebGL(StopByteArrayCallback);
            #endif
        }

        public static void RevokeObjectURL(string url) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                RevokeObjectURL_RecorderWebGL(url);
            #endif
        }

        public static void Resume(Action callback = null) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                if (callback != null) {
                    parameterlessEvent += callback;
                    Resume_RecorderWebGL(parameterlessCallback);
                }
                else
                    Resume_RecorderWebGL();
            #endif
        }

        public static void Pause(Action callback = null) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                if (callback != null) {
                    parameterlessEvent += callback;
                    Pause_RecorderWebGL(parameterlessCallback);
                }
                else
                    Pause_RecorderWebGL();
            #endif
        }

        public static string GetRecordingFileExtension() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                string extension = GetRecordingFileExtension_RecorderWebGL();
                return extension == "failed" ? null : extension;
            #else
                return null;
            #endif
        }

        public static void Save(string fileName = null) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                Save_RecorderWebGL(fileName);
            #endif
        }

        public static uint GetCanvasWidth() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                return GetCanvasWidth_RecorderWebGL();
            #else
                return 0;
            #endif
        }

        public static uint GetCanvasHeight() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                return GetCanvasHeight_RecorderWebGL();
            #else
                return 0;
            #endif
        }

        public static bool IsTypeSupported(string type) {
            #if UNITY_WEBGL && !UNITY_EDITOR
                return IsTypeSupported_RecorderWebGL(type);
            #else
                return false;
            #endif
        }

        public static RecordingState GetState() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                return GetState_RecorderWebGL();
             #else
                return RecordingState.mediaRecorderDoesntExist;
            #endif
        }

        public static uint GetVideoBitsPerSecond() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                return GetVideoBitsPerSecond_RecorderWebGL();
            #else
                return 0;
            #endif
        }

        public static uint GetAudioBitsPerSecond() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                return GetAudioBitsPerSecond_RecorderWebGL();
            #else
                return 0;
            #endif
        }

        public static bool CanRecord() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                return CanRecord_RecorderWebGL();
            #else
                return false;
            #endif
        }

        public static void Discard() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                Discard_RecorderWebGL();
            #endif
        }

        public static void RequestFrame() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestFrame_RecorderWebGL();
            #endif
        }

        public static void Destroy(Action callback = null) {
             #if UNITY_WEBGL && !UNITY_EDITOR
                if (callback != null) {
                    parameterlessEvent += callback;
                    Destroy_RecorderWebGL(parameterlessCallback);
                }
                else
                    Destroy_RecorderWebGL();
            #endif
        }

    }
}
