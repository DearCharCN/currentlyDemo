using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

namespace VideoModule
{
    public class VideoDriver : MonoBehaviour, IDisposable
    {
        public static VideoDriver Instace
        {
            get
            {
                if (m_instace == null)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = "Video Driver";
                    m_instace = gameObject.AddComponent<VideoDriver>();
                    DontDestroyOnLoad(gameObject);
                }
                return m_instace;
            }
        }
        static VideoDriver m_instace;

        List<VideoMedia> mediaPlayers = new List<VideoMedia>();

        public VideoMedia CreateMedia()
        {
            GameObject mediaPlayerObj = new GameObject();
            mediaPlayerObj.transform.SetParent(transform);
            mediaPlayerObj.transform.localPosition = Vector3.zero;
            mediaPlayerObj.transform.rotation = Quaternion.identity;
            mediaPlayerObj.name = "Video Media";
            var mediaPlayer = mediaPlayerObj.AddComponent<MediaPlayer>();
            VideoMedia videoMedia = new VideoMedia(mediaPlayer);

            mediaPlayers.Add(videoMedia);
            return videoMedia;
        }

        public void DestroyMedia(VideoMedia videoMedia)
        {
            videoMedia.Dispose();
            mediaPlayers.Remove(videoMedia);
        }

        public void Dispose()
        {
            while (mediaPlayers.Count > 0)
            {
                DestroyMedia(mediaPlayers[mediaPlayers.Count - 1]);
            }
            Destroy(this.gameObject);
        }
    }
}

