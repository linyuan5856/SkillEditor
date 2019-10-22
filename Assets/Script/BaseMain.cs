using BluePro.FrameWork;
using UnityEngine;

public class BaseMain : MonoBehaviour
{
    void Awake()
    {
        ServiceLocate.Instance.InitService();
        RegisterCoreServices();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        Init();
    }

    void OnDestroy()
    {
        Destroy();
    }


    void OnApplicationQuit()
    {
        ApplicationQuit();
    }

    void Update()
    {
        OnUpdate();
    }

    protected virtual void RegisterCoreServices()
    {
        var ins = ServiceLocate.Instance;
        ins.RegisterService<LoaderService>();
        ins.RegisterService<TimerService>();
        this.RegisterGameServices();
        ins.InitAllServices();
    }

    protected virtual void RegisterGameServices()
    {
    }

    protected virtual void Init()
    {
    }

    protected virtual void Destroy()
    {
        ServiceLocate.Instance.DoDestroy();
    }

    protected virtual void ApplicationQuit()
    {
        ServiceLocate.Instance.OnApplicationQuit();
    }

    protected virtual void OnUpdate()
    {
        ServiceLocate.Instance.DoUpdate(Time.deltaTime);
    }
}