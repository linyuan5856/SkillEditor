using System;

namespace BluePro.FrameWork
{
    public class ServiceLocate : Singleton<ServiceLocate>, IService, IBehaviour
    {
        private MapList<Type, IService> map;

        public void InitService()
        {
            map = new MapList<Type, IService>();
        }

        public void InitAllServices()
        {
            ForEachService(service => service.InitService());
        }

        public void Clean()
        {
            ForEachService(service => service.Clean());
        }

        public void RegisterService<T>() where T : class, IService
        {
            Type type = typeof(T);
            if (map.ContainsKey(type))
                return;

            var service = Activator.CreateInstance<T>();
            map.Add(type, service);
        }


        public T GetService<T>() where T : class, IService
        {
            Type type = typeof(T);
            if (map.ContainsKey(type))
                return map[type] as T;
            Debuger.LogError($"{type} Service 尚未注册");
            return null;
        }


        public void DoDestroy()
        {
            ForEachBehaviour(Destroy);
        }

        public void DoAwake()
        {
        }

        public void DoUpdate(float deltaTime)
        {
            ForEachBehaviour(b => b.DoUpdate(deltaTime));
        }

        public void OnApplicationQuit()
        {
            ForEachBehaviour(ApplicationQuit);
        }

        private void Destroy(IBehaviour behaviour)
        {
            behaviour.DoDestroy();
        }

        private void ApplicationQuit(IBehaviour behaviour)
        {
            behaviour?.OnApplicationQuit();
        }

        void ForEachBehaviour(Action<IBehaviour> ac)
        {
            var list = map.AsList();
            for (int i = 0; i < list.Count; i++)
            {
                var service = list[i];
                if (service is IBehaviour)
                {
                    var behaviour = (IBehaviour) service;
                    ac(behaviour);
                }
            }
        }

        void ForEachService(Action<IService> ac)
        {
            var list = map.AsList();
            for (int i = 0; i < list.Count; i++)
            {
                var service = list[i];
                ac(service);
            }
        }
    }
}