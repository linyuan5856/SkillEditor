using System;
using System.Collections;
using UnityEngine;

public delegate void MonoUpdaterEvent();

public class MonoHelper : MonoSingleton<MonoHelper>
{
    //===========================================================
    private event MonoUpdaterEvent UpdateEvent;
    private event MonoUpdaterEvent FixedUpdateEvent;

    public static void AddUpdateListener(MonoUpdaterEvent listener)
    {
        Instance.UpdateEvent += listener;
    }

    public static void RemoveUpdateListener(MonoUpdaterEvent listener)
    {
        if (ms_instance != null)
        {
            Instance.UpdateEvent -= listener;
        }
    }

    public static void AddFixedUpdateListener(MonoUpdaterEvent listener)
    {
        Instance.FixedUpdateEvent += listener;
    }

    public static void RemoveFixedUpdateListener(MonoUpdaterEvent listener)
    {
        if (ms_instance != null)
        {
            Instance.FixedUpdateEvent -= listener;
        }
    }

    void Update()
    {
        if (UpdateEvent != null)
        {
            // try
            //   {
            UpdateEvent();
            //  }
//            catch (Exception e)
//            {
//                Debug.LogError(string.Format("MonoHelper Update() Error:{0}\n{1}", e.Message, e.StackTrace));
//            }
        }
    }

    void FixedUpdate()
    {
        if (FixedUpdateEvent != null)
        {
            try
            {
                FixedUpdateEvent();
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("MonoHelper FixedUpdate() Error:{0}\n{1}", e.Message,
                    e.StackTrace));
            }
        }
    }

    //===========================================================

    public static void GlobalStartCoroutine(IEnumerator routine)
    {
        MonoBehaviour mono = Instance;
        mono.StartCoroutine(routine);
    }
}