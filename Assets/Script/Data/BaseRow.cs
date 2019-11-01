public interface IBaseRow
{
    int GetId();
}


public class BaseRow : IBaseRow
{
    public virtual int GetId()
    {
        return 0;
    }
}