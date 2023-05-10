using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_SERVER
public class MusicMgr : MonoBehaviour
{
    //AkAudioListener listener;
    Dictionary<string, string> m_BankInfoDict;
    List<string> m_LoadBankList;

    // Start is called before the first frame update
    void Start()
    {
        //listener = GetComponent<AkAudioListener>();

        Debug.Log("Load bank");
        AkBankManager.LoadBank("New_SoundBank", false, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void Init()
    //{
    //    GameObject global = new GameObject("m_WwiseGlobalPrefabName");
    //    global.SetActive(false);
    //    GameObject.DontDestroyOnLoad(global);
    //    AkInitializer akInitializer = global.AddComponent<AkInitializer>();
    //    object settingObj = Resources.Load("AkWwiseInitializationSettings");
    //    if (settingObj != null)
    //    {
    //        AkWwiseInitializationSettings settings = settingObj as AkWwiseInitializationSettings;
    //        akInitializer.InitializationSettings = settings;
    //        global.SetActive(true);
    //    }
    //}

    //bool m_PlaySound;
    //float m_SoundVolume;

    //public void PlaySound(string soundName)
    //{
    //    if (string.IsNullOrEmpty(soundName))
    //    {
    //        return;
    //    }
    //    if (!m_PlaySound)
    //    {
    //        return;
    //    }
    //    PlayEvent("Play", soundName, null, m_SoundVolume, true);
    //}

    //private void PlayEvent(string handName, string eventName, GameObject gameObject, float volume, bool finishCallback = false)
    //{
    //    if (listener == null)
    //    {
    //        return;
    //    }

    //    //EventName我使用默认的“操作_资源名”,如：Play_Close。用户输入直接输入Close即可。
    //    string resourceName = handName + "_" + eventName;
    //    string bankName;

    //    //m_BankInfoDict是Event和SoundBank的映射关系字典
    //    if (!m_BankInfoDict.TryGetValue(resourceName, out bankName))
    //    {
    //        Debug.LogError(string.Format("加载event（{0}）失败,没有找到所属的SoundBank", resourceName));
    //        return;
    //    }
    //    if (!m_LoadBankList.Contains(bankName))
    //    {
    //        //加载SoundBank
    //        AkBankManager.LoadBank(bankName, false, false);
    //        m_LoadBankList.Add(bankName);
    //    }
    //    if (gameObject == null)
    //    {
    //        //加载一个预制体，预制体是空的
    //        gameObject = AddSoundGameObject(eventName);
    //    }
    //    //播放
    //    AkSoundEngine.PostEvent(resourceName, gameObject, (uint)5, null, null);
    //    //设置音量
    //    AkSoundEngine.SetGameObjectOutputBusVolume(gameObject, listener.gameObject, volume);
    //}

    private GameObject AddSoundGameObject(string eventName) { return null; }

    public void OnClick()
    {
        Debug.Log("Post Event to Wwise");
        AkSoundEngine.PostEvent("Play_Test", gameObject, (uint)5, null, null);
    }
}
#endif