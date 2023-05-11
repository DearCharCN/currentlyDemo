using UnityEngine;

namespace DearChar
{
    public class UnityDriver
    {
        static GameObject m_instance;
        public static GameObject Instance
        {
            get
            {
                if (m_instance == null)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = "UnityDriver";
                    GameObject.DontDestroyOnLoad(gameObject);
                    m_instance = gameObject;
                }
                return m_instance;
            }
        }
    }
}
    
