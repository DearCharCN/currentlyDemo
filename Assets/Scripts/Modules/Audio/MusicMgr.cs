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

    //    //EventName��ʹ��Ĭ�ϵġ�����_��Դ����,�磺Play_Close���û�����ֱ������Close���ɡ�
    //    string resourceName = handName + "_" + eventName;
    //    string bankName;

    //    //m_BankInfoDict��Event��SoundBank��ӳ���ϵ�ֵ�
    //    if (!m_BankInfoDict.TryGetValue(resourceName, out bankName))
    //    {
    //        Debug.LogError(string.Format("����event��{0}��ʧ��,û���ҵ�������SoundBank", resourceName));
    //        return;
    //    }
    //    if (!m_LoadBankList.Contains(bankName))
    //    {
    //        //����SoundBank
    //        AkBankManager.LoadBank(bankName, false, false);
    //        m_LoadBankList.Add(bankName);
    //    }
    //    if (gameObject == null)
    //    {
    //        //����һ��Ԥ���壬Ԥ�����ǿյ�
    //        gameObject = AddSoundGameObject(eventName);
    //    }
    //    //����
    //    AkSoundEngine.PostEvent(resourceName, gameObject, (uint)5, null, null);
    //    //��������
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