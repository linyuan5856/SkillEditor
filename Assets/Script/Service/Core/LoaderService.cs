using System.Collections.Generic;
using BluePro.FrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class LoaderService : BaseService
{
    Dictionary<string, IBaseTable> tableDic = new Dictionary<string, IBaseTable>();

    public IBaseTable GetTable(string name)
    {
        if (tableDic.ContainsKey(name))
            return tableDic[name];

        IBaseTable table = null;
#if UNITY_EDITOR
        const string path = "Assets/Resources/Config/";
        var asset = AssetDatabase.LoadAssetAtPath(path + name + ".asset", typeof(IBaseTable));
        //使用实例化的Copy  防止Editor模式下 更改配置
        table = Object.Instantiate(asset) as IBaseTable;
#else
        table = Resources.Load("Config/"+name)as BaseTable;
#endif
        if (table == null)
            Debug.LogError(string.Format("Table-> {0} is null", name));
        else
            tableDic.Add(name, table);
        return table;
    }

    public IBaseRow GetRowFromTable(string tableName, int id)
    {
        var table = GetTable(tableName);
        if (table != null)
            return table.GetRow(id);
        return null;
    }

    public T GetRowDataFromTable<T>(string tableName, int id) where T : class
    {
        var row = GetRowFromTable(tableName, id);
        return row as T;
    }

    public UnityEngine.Object LoadAsset(string path)
    {
        return Resources.Load(path);
    }

    public GameObject Instantiate(string path)
    {
        var obj = LoadAsset(path);
        return UnityEngine.Object.Instantiate(obj) as GameObject;
    }

    public GameObject InstantiateInEditor(string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path + ".prefab");
        return UnityEngine.Object.Instantiate(asset);
    }

    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneName, mode);
    }

    public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        return SceneManager.LoadSceneAsync(sceneName, mode);
    }
}