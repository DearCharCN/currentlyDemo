using UnityEngine;
using RenderHeads.Media.AVProVideo;
using VideoModule;

public class VideoTest : MonoBehaviour
{
    [SerializeField]
    DisplayUGUI displayUGUI;

    IVideoEventHandle videoEventHandle;

    void Start()
    {
        VideoManager.ViewToUGUI(displayUGUI);
        videoEventHandle = VideoManager.CurrentlyMedia.RegisterEvent(OnVideo);
        VideoManager.PlayVideo("AVProVideoSamples/BigBuckBunny-360p30-H264.mp4", VideoPathType.RelativeToStreamingAssetsFolder);
        
    }

    public void Pause()
    {
        VideoManager.PauseVideo();
    }

    void OnVideo(VideoEventData videoEventData)
    {
        Debug.Log($"[OnVideo] {videoEventData.MediaEventType} {videoEventData.ErrorCode}");
    }
}
