namespace BluePro.FrameWork
{
    public abstract class BaseService : IService, IBehaviour
    {
        public virtual void InitService()
        {
        }

        public virtual void Clean()
        {
        }

        public virtual void OnDrawGizmos()
        {
        }

        public virtual void DoAwake()
        {
        }

        public virtual void DoDestroy()
        {
        }

        public virtual void DoUpdate(float deltaTime)
        {
        }

        public virtual void OnApplicationQuit()
        {
        }
    }
}