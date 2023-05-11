using UnityEngine;
using DearChar.Threading;

public class Threadtest : MonoBehaviour
{
    ThreadContainer threadContainer;
    // Start is called before the first frame update
    void Start()
    {
        threadContainer = new Threader(false);
    }

    // Update is called once per frame
    void Update()
    {

    }


    bool s = true;
    public void OnClickSetActive()
    {
        threadContainer.SetActive(s = !s);
    }

    public void OnClickDestroy()
    {
        threadContainer.Destroy();
    }
}


public class Threader : ThreadContainer
{
    public Threader(bool Active) : base(Active)
    {
    }

    protected override void Awake()
    {
        Debug.Log("Awake");
    }

    protected override void Start()
    {
        Debug.Log("Start");
    }

    protected override void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    protected override void Update()
    {
        Debug.Log("Update");
    }

    protected override void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    protected override void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }
}

