using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VideoModule
{
    public class VideoManager
    {
        static public VideoMedia CurrentlyMedia
        {
            get
            {
                if (m_currentlyMedia == null)
                {
                    m_currentlyMedia = VideoDriver.Instace.CreateMedia();
                }
                return m_currentlyMedia;
            }
        }
        static VideoMedia m_currentlyMedia;

        public static void PlayVideo(string path, VideoPathType videoPathType)
        {
            CurrentlyMedia.LoadVideo(path, videoPathType, true);
        }

        public static void PauseVideo()
        {
            CurrentlyMedia.Pause();
        }

        public static void ViewToUGUI(DisplayUGUI displayUGUI)
        {
            displayUGUI.CurrentMediaPlayer = CurrentlyMedia.MediaPlayer;
        }

        public static VideoMedia CreateMedia()
        {
            return VideoDriver.Instace.CreateMedia();
        }

        public static void DisposeAll()
        {
            VideoDriver.Instace.Dispose();
            m_currentlyMedia = null;
        }
    }
}

