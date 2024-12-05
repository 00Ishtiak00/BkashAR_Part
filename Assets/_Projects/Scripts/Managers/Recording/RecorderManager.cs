using System;
using UnityEngine;
using MarksAssets.RecorderWebGL;
using UnityEngine.UI;
using status = MarksAssets.RecorderWebGL.RecorderWebGL.status;
using System.Collections;
using MarksAssets.ShareNSaveWebGL;
using static MarksAssets.RecorderWebGL.RecorderWebGL;
using UnityEngine.EventSystems;

public class RecorderManager : MonoBehaviour
{
    public GameObject StartBtn, StopBtn, DownloadBtn, ShareBtn;
    
    public Text RecordedText, StatusText;
    
    public int recordForNSeconds = -1;//-1 means that it won't use timer. So it will stop recording when tapping on stop button. Any value >= 0 will use timer. Then you'd need to tap and hold the start button, and wait for the timer or release the button earlier to stop the recording before the time's up.

    private Coroutine timerRoutine = null;
    private readonly RecorderWebGL.MediaRecorderOptions mro = new("video/webm;codecs=vp8,opus");//This is to avoid creating .mkv files on browsers that can create .webm, as it seems some video players have trouble with the generated .mkv and not detect the full length of the video.

    private const string BlobPropertyPath = "Module.RecorderWebGL.mediaRecorderBlob";

    private string fileName;

    private void Start()
    {
        StartBtn.SetActive(false);
        StopBtn.SetActive(false);
        DownloadBtn.SetActive(false);
        ShareBtn.SetActive(false);
        
        if (recordForNSeconds >= 0) {//use timer

            StartBtn.GetComponent<EventTrigger>().triggers[0].callback.AddListener(bed => {
                StartRecording();
                DownloadBtn.SetActive(false);
            });

            StartBtn.GetComponent<EventTrigger>().triggers[1].callback.AddListener(bed => StopRecording());//add callback to stop recording on pointerup (triggers 1, the pointerup added in the inspector)
        } 
        else {//don't use timer

            StartBtn.GetComponent<EventTrigger>().triggers[0].callback.AddListener(bed => {
                StartRecording();
                DownloadBtn.SetActive(false);
                ShareBtn.SetActive(false);
                StartBtn.SetActive(false);
            });

        }

        Invoke(nameof(EnableRecording), 1f);
    }

    private void EnableRecording()
    {
        CreateRecordingIngameAudio();
    }
    
    private void StartRecording() {
        Debug.Log("StartRecordCallBack");
        RecorderWebGL.Start(StartRecordCallBack);
    }

    private void CreateRecordingMicrophoneIngameAudio() {
        RecorderWebGL.CreateMediaRecorder(CreateMediaRecorderCallback, mro);
    }

    private void CreateRecordingMicrophone() {
        RecorderWebGL.CreateMediaRecorder(CreateMediaRecorderCallback, mro, true, false);
    }

    private void CreateRecordingIngameAudio() {
        RecorderWebGL.CreateMediaRecorder(CreateMediaRecorderCallback, mro, false, true);
    }

    private void CreateRecordingNoAudio() {
        RecorderWebGL.CreateMediaRecorder(CreateMediaRecorderCallback, mro, false, false);
    }

    public void StopRecording() {
        
        if (RecorderWebGL.GetState() != RecordingState.stopped) {//this is important to record on a timer.
            if (timerRoutine != null) StopCoroutine(timerRoutine);//in case the user released the button before the time's up, stop timer(coroutine) prematurely.
            RecorderWebGL.Stop(StopCallBack);
        }
    }

    public void PauseRecording() {
        
        if (RecorderWebGL.GetState() != RecordingState.paused) {
            RecorderWebGL.Pause(PauseCallBack);
        }
    }

    public void ResumeRecording() {
        
        if (RecorderWebGL.GetState() != RecordingState.recording) {
            RecorderWebGL.Resume(ResumeCallBack);
        }
    }

    public void Download() {
        
        //RecorderWebGL.Save();

        SaveRecord();
    }

    private void StartRecordCallBack() {
        if (recordForNSeconds < 0) {//don't record with timer. Stop recording with button.
            RecordedText.text = "";
            StopBtn.SetActive(true);
            Debug.Log("Start recording");
        } 
        else {//record with timer. Tap and hold the start button
            timerRoutine = StartCoroutine(TimerRoutine());
        }
    }

    IEnumerator TimerRoutine() {
        yield return new WaitForSeconds(recordForNSeconds);
        StopRecording();
    }

    private void ResumeCallBack() {
        
        //PauseBtn.SetActive(true);
    }

    private void PauseCallBack() {
       
        //ResumeBtn.SetActive(true);
    }

    private void CreateMediaRecorderCallback(status stat) {
        StatusText.text = stat.ToString();
        StartBtn.SetActive(true);
	}
    private void StopCallBack() {
        
        if (RecorderWebGL.GetRecordingFileExtension() != null) {
            RecordedText.text = "Recorded a " + RecorderWebGL.GetRecordingFileExtension() + " file";
            DownloadBtn.SetActive(true);
            ShareBtn.SetActive(true);
        } 
        else {
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
    
    public void ShareRecord() {
        
        StatusText.text = "status: " + ShareNSaveWebGL.CanShare(BlobPropertyPath, fileName);
        
        ShareBtn.SetActive(false);
        DownloadBtn.SetActive(false);
        StartBtn.SetActive(true);
        
        ShareNSaveWebGL.Share(ShareCallBack, BlobPropertyPath, fileName);
    }
    
    private void SaveRecord() {
        
        DateTime currentTime = DateTime.Now; 
        
        string formattedTime = currentTime.ToString("yyyyMMddHHmmss");

        fileName = "recording_" + formattedTime;
        
        if(fileName != string.Empty) 
        {
            ShareNSaveWebGL.Save(BlobPropertyPath, fileName);
        }
    }

    private void ShareCallBack(ShareNSaveWebGL.status obj)
    {
        StatusText.text = "status: " + obj;
    }
    
}
