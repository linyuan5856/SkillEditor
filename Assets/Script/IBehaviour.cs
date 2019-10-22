namespace BluePro.FrameWork
{
    public interface IBehaviour
    {
        void DoAwake();
        void DoUpdate(float deltaTime);
        void DoDestroy();
        void OnApplicationQuit();
    }
}