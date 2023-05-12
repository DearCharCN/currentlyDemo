using UnityEngine;

namespace DearChar
{
    public class UnityDriver
    {
        static GameObject m_gameObject;
        public static GameObject GameObject
        {
            get
            {
                if (m_gameObject == null)
                {
                    if (!UnityEngine.Application.isPlaying)
                        throw new System.Exception("��Ϸ���������������µ�Driver");
                    GameObject gameObject = new GameObject();
                    gameObject.name = "UnityDriver";
                    GameObject.DontDestroyOnLoad(gameObject);
                    gameObject.AddComponent<UnityDriverComponent>();
                    m_gameObject = gameObject;
                }
                return m_gameObject;
            }
        }

        public static bool ApplicationIsPlayIng { get => m_applicationIsPlaying; }

        static bool m_applicationIsPlaying;

        private class UnityDriverComponent : MonoBehaviour
        {
            private void Update()
            {
                m_applicationIsPlaying = UnityEngine.Application.isPlaying;
            }

            private void OnDestroy()
            {
                m_applicationIsPlaying = false;
            }
        }
    }


}

