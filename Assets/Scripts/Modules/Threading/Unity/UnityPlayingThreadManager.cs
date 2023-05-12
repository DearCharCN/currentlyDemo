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
                    m_threadManagerForUnity = UnityDriver.GameObject.AddComponent<ThreadManagerForUnity>();
                }
                return m_threadManagerForUnity;
            }
        }

        public static void OnContainerCreate(ThreadContainer threadContainer)
        {
            threadManagerForUnity.OnContainerCreate(threadContainer);
        }
        protected class ThreadManagerForUnity : MonoBehaviour
        {
            List<ThreadContainer> PlayingActiveContainer = new List<ThreadContainer>();

            internal void OnContainerCreate(ThreadContainer threadContainer)
            {
                if (!UnityDriver.ApplicationIsPlayIng)
                    return;

                lock (PlayingActiveContainer)
                {
                    PlayingActiveContainer.Add(threadContainer);
                }
            }

            float updateCount = 0;

            private void Update()
            {
                updateCount += Time.unscaledDeltaTime;
                if (updateCount < 60)
                    return;
                updateCount = 0;

                for (int i = 0; i < PlayingActiveContainer.Count; ++i)
                {
                    if (PlayingActiveContainer[i].IsDestroy)
                    {
                        PlayingActiveContainer.RemoveAt(i);
                        --i;
                    }
                }
            }

            private void OnDestroy()
            {
                lock (PlayingActiveContainer)
                {
                    PlayingActiveContainer.For((item) =>
                    {
                        if(!item.IsDestroy)
                        {
                            item.Destroy();
                        }
                    });
                    PlayingActiveContainer.Clear();
                }
            }
        }
    }

    
}

