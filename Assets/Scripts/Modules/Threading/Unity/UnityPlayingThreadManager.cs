using System.Collections.Generic;
using UnityEngine;

namespace DearChar.Threading.Unity
{
    public class UnityPlayingThreadManager
    {
        static ThreadManagerForUnity m_threadManagerForUnity;

        static ThreadManagerForUnity threadManagerForUnity
        {
            get
            {
                if (m_threadManagerForUnity == null)
                {
                    m_threadManagerForUnity = UnityDriver.Instance.AddComponent<ThreadManagerForUnity>();
                }
                return m_threadManagerForUnity;
            }
        }

        public static void OnContainerCreate(ThreadContainer threadContainer)
        {
            threadManagerForUnity.OnContainerCreate(threadContainer);
        }

        public static void OnContainerDestroy(ThreadContainer threadContainer)
        {
            threadManagerForUnity.OnContainerDestroy(threadContainer);
        }

        protected class ThreadManagerForUnity : MonoBehaviour
        {
            Dictionary<ThreadContainer, int> PlayingActiveContainer = new Dictionary<ThreadContainer, int>();

            internal void OnContainerCreate(ThreadContainer threadContainer)
            {
                if (!UnityEngine.Application.isPlaying)
                    return;

                lock (PlayingActiveContainer)
                {
                    PlayingActiveContainer[threadContainer] = 1;
                }
            }

            internal void OnContainerDestroy(ThreadContainer threadContainer)
            {
                if(_ApplicationQuiting)
                {
                    return;
                }

                lock (PlayingActiveContainer)
                {
                    PlayingActiveContainer.Remove(threadContainer);
                }
            }

            bool _ApplicationQuiting = false;

            private void OnDestroy()
            {
                _ApplicationQuiting = true;
                lock (PlayingActiveContainer)
                {
                    foreach (var kv in PlayingActiveContainer)
                    {
                        kv.Key.Destroy();
                    }
                    PlayingActiveContainer.Clear();
                }
            }
        }
    }

    
}

