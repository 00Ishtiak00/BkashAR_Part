using UnityEngine;
using MarksAssets.RecorderWebGL;
using UnityEngine.UI;
using status = MarksAssets.RecorderWebGL.RecorderWebGL.status;
using System.Collections;
using static MarksAssets.RecorderWebGL.RecorderWebGL;
using UnityEngine.EventSystems;
//using MarksAssets.ShareNSaveWebGL;//if using my https://assetstore.unity.com/packages/tools/integration/sharensavewebgl-181122 asset as well.

public class RecorderWebGL_Example : MonoBehaviour {

    public GameObject StartBtn, PauseBtn, StopBtn, ResumeBtn, DownloadBtn, CreateRecordingMicrophoneIngameAudioBtn, CreateRecordingMicrophoneBtn, CreateRecordingIngameAudioBtn, CreateRecordingNoAudioBtn;
    public Text RecordedText, StatusText;
    public int recordForNSeconds = -1;//-1 means that it won't use timer. So it will stop recording when tapping on stop button. Any value >= 0 will use timer. Then you'd need to tap and hold the start button, and wait for the timer or release the button earlier to stop the recording before the time's up.

    //private byte[] bytes;//used if recorded as byte array

    private Coroutine timerRoutine = null;
    private MediaRecorderOptions mro = new MediaRecorderOptions("video/webm;codecs=vp8,opus");//This is to avoid creating .mkv files on browsers that can create .webm, as it seems some video players have trouble with the generated .mkv and not detect the full length of the video.


    private void Start() {

        if (recordForNSeconds >= 0) {//use timer

            StartBtn.GetComponent<EventTrigger>().triggers[0].callback.AddListener(bed => {
                StartRecording();
                DownloadBtn.SetActive(false);
            });

            StartBtn.GetComponent<EventTrigger>().triggers[1].callback.AddListener(bed => StopRecording());//add callback to stop recording on pointerup (triggers 1, the pointerup added in the inspector)
        } else {//don't use timer

            StartBtn.GetComponent<EventTrigger>().triggers[0].callback.AddListener(bed => {
                StartRecording();
                DownloadBtn.SetActive(false);
                StartBtn.SetActive(false);
            });

        }
    }

    public void StartRecording() {
        RecorderWebGL.Start(startClbk);
    }

    public void CreateRecordingMicrophoneIngameAudio() {
        RecorderWebGL.CreateMediaRecorder(createMediaRecorderCallback, mro);
    }

    public void CreateRecordingMicrophone() {
        RecorderWebGL.CreateMediaRecorder(createMediaRecorderCallback, mro, true, false);
    }

    public void CreateRecordingIngameAudio() {
        RecorderWebGL.CreateMediaRecorder(createMediaRecorderCallback, mro, false, true);
    }

    public void CreateRecordingNoAudio() {
        RecorderWebGL.CreateMediaRecorder(createMediaRecorderCallback, mro, false, false);
    }

    public void StopRecording() {
        if (RecorderWebGL.GetState() != RecordingState.stopped) {//this is important to record on a timer.
            if (timerRoutine != null) StopCoroutine(timerRoutine);//in case the user released the button before the time's up, stop timer(coroutine) prematurely.
            RecorderWebGL.Stop(stopcallback);
        }
    }

    public void PauseRecording() {
        if (RecorderWebGL.GetState() != RecordingState.paused) {
            RecorderWebGL.Pause(pauseClbk);
        }
    }

    public void ResumeRecording() {
        if (RecorderWebGL.GetState() != RecordingState.recording) {
            RecorderWebGL.Resume(resumeClbk);
        }
    }

    public void Download() {
        RecorderWebGL.Save();

        //if you have my ShareNSaveWebGL asset and recorded as a byte array: https://assetstore.unity.com/packages/tools/integration/sharensavewebgl-181122
        //ShareNSaveWebGL.Save(bytes, "video/mp4");
    }

    private void startClbk() {
        if (recordForNSeconds < 0) {//don't record with timer. Stop recording with button.
            RecordedText.text = "";
            PauseBtn.SetActive(true);
            StopBtn.SetActive(true);
            CreateRecordingMicrophoneIngameAudioBtn.SetActive(false);
            CreateRecordingMicrophoneBtn.SetActive(false);
            CreateRecordingIngameAudioBtn.SetActive(false);
            CreateRecordingNoAudioBtn.SetActive(false);
        } else {//record with timer. Tap and hold the start button
            timerRoutine = StartCoroutine(TimerRoutine());
        }
    }

    IEnumerator TimerRoutine() {
        yield return new WaitForSeconds(recordForNSeconds);
        StopRecording();
    }

    private void resumeClbk() {
        PauseBtn.SetActive(true);
    }

    private void pauseClbk() {
       ResumeBtn.SetActive(true);
    }

    private void createMediaRecorderCallback(status stat) {
        StatusText.text = stat.ToString();
        StartBtn.SetActive(true);
		
	}
    private void stopcallback() {
        StartBtn.SetActive(true);
        CreateRecordingMicrophoneIngameAudioBtn.SetActive(true);
        CreateRecordingMicrophoneBtn.SetActive(true);
        CreateRecordingIngameAudioBtn.SetActive(true);
        CreateRecordingNoAudioBtn.SetActive(true);
        if (RecorderWebGL.GetRecordingFileExtension() != null) {
            RecordedText.text = "Recorded a " + RecorderWebGL.GetRecordingFileExtension() + " file";
            DownloadBtn.SetActive(true);
        } else {
            RecordedText.text = "Recording failed";
        }
    }

    /*private void stopcallbackByteArr(byte[] bytes, int size) {
        Debug.Log("==RECORDED AS BYTE ARRAY==");
        Debug.Log("SIZE: " + size);

        this.bytes = bytes;

        StartBtn.SetActive(true);
        CreateRecordingMicrophoneIngameAudioBtn.SetActive(true);
        CreateRecordingMicrophoneBtn.SetActive(true);
        CreateRecordingIngameAudioBtn.SetActive(true);
        CreateRecordingNoAudioBtn.SetActive(true);
        if (RecorderWebGL.GetRecordingFileExtension() != null) {
            RecordedText.text = "Recorded a " + RecorderWebGL.GetRecordingFileExtension() + " file";
            DownloadBtn.SetActive(true);
        } else {
            RecordedText.text = "Recording failed";
        }
    }*/

   
}
