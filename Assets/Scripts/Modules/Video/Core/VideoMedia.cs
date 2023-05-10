using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.IO;

namespace VideoModule
{
    public class VideoMedia : IDisposable
    {
        #region Life Cycle
        public MediaPlayer MediaPlayer
        {
            get
            {
                return m_mediaPlayer;
            }
        }

        MediaPlayer m_mediaPlayer;
        public VideoMedia(MediaPlayer mediaPlayer)
        {
            m_mediaPlayer = mediaPlayer;
            m_mediaPlayer.Events.AddListener(OnMediaEvent);
        }

        public void Dispose()
        {
            m_mediaPlayer.Events.RemoveListener(OnMediaEvent);
            m_mediaPlayer = null;

            //Dispose Event
            isDistributing = false;
            while (eventList.Count > 0)
            {
                UnRegisterEvent(eventList[eventList.Count - 1]);
            }
            eventList.Clear();
        }

        #endregion

        #region Video API

        public void LoadVideo(string path, VideoPathType videoPathType, bool automaticPlay = true)
        {
            MediaPathType mediaPathType = MediaPathType.AbsolutePathOrURL;
            switch (videoPathType)
            {
                case VideoPathType.AbsolutePathOrURL:
                    mediaPathType = MediaPathType.AbsolutePathOrURL;
                    break;
                case VideoPathType.RelativeToProjectFolder:
                    mediaPathType = MediaPathType.RelativeToProjectFolder;
                    break;
                case VideoPathType.RelativeToStreamingAssetsFolder:
                    mediaPathType = MediaPathType.RelativeToStreamingAssetsFolder;
                    break;
                case VideoPathType.RelativeToDataFolder:
                    mediaPathType = MediaPathType.RelativeToDataFolder;
                    break;
                case VideoPathType.RelativeToPersistentDataFolder:
                    mediaPathType = MediaPathType.RelativeToPersistentDataFolder;
                    break;
            }
            m_mediaPlayer.OpenMedia(mediaPathType, path, automaticPlay);
        }
        public void Play()
        {
            m_mediaPlayer.Play();
        }

        public void Pause()
        {
            m_mediaPlayer.Pause();
        }

        public void Seek(double targetTime)
        {
            m_mediaPlayer.SeekToLiveTime(targetTime);
        }
        #endregion

        #region Event

        public IVideoEventHandle RegisterEvent(Action<VideoEventData> callback)
        {
            var handle = new VideoEventHandle();
            handle.callBack = callback;
            eventList.Add(handle);
            return handle;
        }

        public void UnRegisterEvent(IVideoEventHandle videoEventHandle)
        {
            if (isDistributing)
            {
                throw new InvalidOperationException("正在分发事件中，不可注销事件");
            }
            VideoEventHandle _videoEventHandle = (VideoEventHandle)videoEventHandle;
            eventList.Remove(_videoEventHandle);
            _videoEventHandle.Dispose();
        }
        #endregion

        #region Event Internal

        List<VideoEventHandle> eventList = new List<VideoEventHandle>();

        class VideoEventHandle : IVideoEventHandle, IDisposable
        {
            public Action<VideoEventData> callBack;

            public void Dispose()
            {
                callBack = null;
            }
        }

        void OnMediaEvent(MediaPlayer mediaPlayer, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
        {
            MediaEventType mediaEventType = MediaEventType.Error;
            string errorCodeStr = string.Empty;
            switch (eventType)
            {
                case MediaPlayerEvent.EventType.ReadyToPlay:
                    mediaEventType = MediaEventType.ReadyToPlay;
                    break;
                case MediaPlayerEvent.EventType.Started:
                    mediaEventType = MediaEventType.Started;
                    break;
                case MediaPlayerEvent.EventType.Closing:
                    mediaEventType = MediaEventType.Closing;
                    break;
                case MediaPlayerEvent.EventType.Error:
                    mediaEventType = MediaEventType.Error;
                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:
                    mediaEventType = MediaEventType.FinishedPlaying;
                    break;
                default:
                    return;
            }

            if (errorCode != ErrorCode.None)
            {
                mediaEventType = MediaEventType.Error;
                errorCodeStr = errorCode.ToString();
            }


            VideoEventData eventData = new VideoEventData(mediaEventType, errorCodeStr);
            DistributeEvent(eventData);
        }

        bool isDistributing = false;
        void DistributeEvent(VideoEventData eventData)
        {
            isDistributing = true;
            for (int i = 0; i < eventList.Count; i++)
            {
                try
                {
                    eventList[i].callBack?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            isDistributing = false;
        }
        #endregion
    }

    public enum MediaEventType
    {
        ReadyToPlay,
        Started,
        FinishedPlaying,
        Closing,
        Error,
    }

    public struct VideoEventData
    {
        public MediaEventType MediaEventType { get; private set; }
        public string ErrorCode { get; private set; }

        public VideoEventData(MediaEventType mediaEventType, string errorCode)
        {
            MediaEventType = mediaEventType;
            ErrorCode = errorCode;
        }
    }

    public interface IVideoEventHandle
    {

    }

    public enum VideoPathType
    {
        AbsolutePathOrURL,
        RelativeToProjectFolder,
        RelativeToStreamingAssetsFolder,
        RelativeToDataFolder,
        RelativeToPersistentDataFolder,
    }
}


