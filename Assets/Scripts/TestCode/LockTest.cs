using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Threading;

public class LockTest : MonoBehaviour
{
    ThreadTesterer threadTesterer1 = new ThreadTesterer();
    ThreadTesterer threadTesterer2 = new ThreadTesterer();

    public void OnDo1()
    {
        threadTesterer1.Doit(objLock,"A");
    }
    public void OnCancel1()
    {
        threadTesterer1.StopDo();
    }

    public void OnDo2()
    {
        threadTesterer2.Doit(objLock,"B");
    }
    public void OnCancel2()
    {
        threadTesterer2.StopDo();
    }

    object objLock = new object();
}

public class ThreadTesterer : ThreadContainer
{
    object objLock;
    string msg;
    bool isDo = false;
    public void Doit(object objLock,string msg)
    {
        this.objLock = objLock;
        this.msg = msg;
        isDo = true;
    }

    public void StopDo()
    {
        isDo = false;
    }
    protected override void Update()
    {
        if (isDo)
        {
            lock (objLock)
            {
                while (isDo)
                {
                    DODODO();
                }
            }
        }
    }

    protected void DODODO()
    {
        lock (objLock)
        {
            Debug.Log(msg);
        }
    }
}
