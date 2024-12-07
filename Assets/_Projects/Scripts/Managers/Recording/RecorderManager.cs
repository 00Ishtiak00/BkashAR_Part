using System;
using UnityEngine;
using MarksAssets.RecorderWebGL;
using UnityEngine.UI;
using status = MarksAssets.RecorderWebGL.RecorderWebGL.status;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MarksAssets.ShareNSaveWebGL;
using Sirenix.OdinInspector;
using static MarksAssets.RecorderWebGL.RecorderWebGL;
using UnityEngine.EventSystems;

public class RecorderManager : MonoBehaviour
{
    public GameObject StartBtn, StopBtn, DownloadBtn, ShareBtn, CancelBtn;
    
    public Text RecordedText, StatusText;
    
    public int recordForNSeconds = -1;//-1 means that it won't use timer. So it will stop recording when tapping on stop button. Any value >= 0 will use timer. Then you'd need to tap and hold the start button, and wait for the timer or release the button earlier to stop the recording before the time's up.

    private Coroutine timerRoutine = null;
    private readonly RecorderWebGL.MediaRecorderOptions mro = new("video/webm;codecs=vp8,opus");//This is to avoid creating .mkv files on browsers that can create .webm, as it seems some video players have trouble with the generated .mkv and not detect the full length of the video.

    private const string BlobPropertyPath = "Module.RecorderWebGL.mediaRecorderBlob";

    private string fileName;

    [SerializeField] private GameObject _btnAudioSourcesParentGO; // Parent GameObject containing all AudioSources
    
    public bool isRecording = false;
    
    private void Start()
    {
        StartBtn.SetActive(true);
        StopBtn.SetActive(false);
        CancelBtn.SetActive(false);
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
                CancelBtn.SetActive(false);
                DownloadBtn.SetActive(false);
                ShareBtn.SetActive(false);
                StartBtn.SetActive(false);
                StartRecording();
            });
        }
    }
    
    private async UniTaskVoid ActivateAllChildsOfBtnAudioSourcesParentGo()
    {
        foreach (Transform child in _btnAudioSourcesParentGO.transform)
        {
            child.gameObject.SetActive(true);
            await UniTask.Delay(10);
        }
        
        DOVirtual.DelayedCall(1f, CreateRecordingIngameAudio).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                RecorderWebGL.Start(StartRecordCallBack);
                isRecording = true;
            });
        });
    }
    
    private async UniTaskVoid DeActivateAllChildsOfBtnAudioSourcesParentGo()
    {
        foreach (Transform child in _btnAudioSourcesParentGO.transform)
        {
            child.gameObject.SetActive(false);
            await UniTask.Delay(10);
        }
        
        isRecording = false;
        CancelBtn.SetActive(true);
        DownloadBtn.SetActive(true);
        ShareBtn.SetActive(true);
    }

    [Button]
    private void StartRecording() 
    {
        // Active all childs of btnAudioSourcesParentGO
        /*foreach (Transform child in _btnAudioSourcesParentGO.transform)
        {
            child.gameObject.SetActive(true);
        }
        
        DOVirtual.DelayedCall(1f, CreateRecordingIngameAudio).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                RecorderWebGL.Start(StartRecordCallBack);
                isRecording = true;
            });
        });*/
        ActivateAllChildsOfBtnAudioSourcesParentGo().Forget();
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
    
    [Button]
    public void StopRecording() 
    {
        if (RecorderWebGL.GetState() != RecordingState.stopped) 
        {//this is important to record on a timer.
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

    [Button]
    public void DownloadRecord() 
    {
        //RecorderWebGL.Save();
        
        DateTime currentTime = DateTime.Now; 
        
        string formattedTime = currentTime.ToString("yyyyMMddHHmmss");

        fileName = "recording_" + formattedTime;
        
        //StartBtn.SetActive(true);
        
        if(fileName != string.Empty) 
        {
            ShareNSaveWebGL.Save(BlobPropertyPath, fileName);
        }
    }

    private void StartRecordCallBack() 
    {
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
        //StartBtn.SetActive(true);
	}
    private void StopCallBack() {
        
        if (RecorderWebGL.GetRecordingFileExtension() != null) 
        {
            RecordedText.text = "Recorded a " + RecorderWebGL.GetRecordingFileExtension() + " file";
            
            StopBtn.SetActive(false);

            DOVirtual.DelayedCall(1f, () =>
            {
                DeActivateAllChildsOfBtnAudioSourcesParentGo().Forget();
            });
            
            /*foreach (Transform child in _btnAudioSourcesParentGO.transform)
            {
                child.gameObject.SetActive(false);
            }
            
            DOVirtual.DelayedCall(0.5f, () =>
            {
                CancelBtn.SetActive(true);
                DownloadBtn.SetActive(true);
                ShareBtn.SetActive(true);
                isRecording = false;
            });*/
        } 
        else 
        {
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
    
    [Button]
    public void ShareRecord() 
    {
        StatusText.text = "status: " + ShareNSaveWebGL.CanShare(BlobPropertyPath, fileName);
        
        ShareNSaveWebGL.Share(ShareCallBack, BlobPropertyPath, fileName);
    }
    
    public void CancelRecord() 
    {
        CancelBtn.SetActive(false);
        DownloadBtn.SetActive(false);
        ShareBtn.SetActive(false);
        
        DOVirtual.DelayedCall(1f, () =>
        {
           RecorderWebGL.Destroy();
        }).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                StartBtn.SetActive(true);
            });
        });
    }

    private void ShareCallBack(ShareNSaveWebGL.status obj)
    {
        StatusText.text = "status: " + obj;
    }
}
