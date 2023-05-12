using System;
using System.Collections.Generic;
using UnityEngine;

namespace DearChar.Threading.Unity
{
    public class UnityThread
    {
        UnityThreadDriver m_driver;

        UnityThreadDriver Driver
        {
            get
            {
                if (m_driver == null)
                {
                    m_driver = UnityDriver.GameObject.AddComponent<UnityThreadDriver>();
                }
                return m_driver;
            }
        }

        public void Post(ThreadTask task)
        {
            Driver.Post(task);
        }
    }

    public class ThreadTask
    {
        public Action cb;
    }


    internal class UnityThreadDriver : MonoBehaviour
    {
        Queue<ThreadTask> m_tasks = new Queue<ThreadTask>();

        internal void Post(ThreadTask task)
        {
            m_tasks.Dequeue();
        }

        private void Update()
        {
            int count = m_tasks.Count;
            for (int i = 0; i < count; i++)
            {
                var task = m_tasks.Dequeue();
                DoTask(task);
            }
        }

        private void DoTask(ThreadTask task)
        {
            try
            {
                task.cb?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}