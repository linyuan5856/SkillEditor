namespace BluePro.FrameWork
{
    public interface IService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="param"></param>
        void InitService();

        void Clean();
    }
}