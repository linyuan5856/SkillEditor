using System;
using System.Collections.Generic;
using BluePro.FrameWork;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class EffectUtil
{
    #region VFX

#if UNITY_EDITOR
    private static Dictionary<string, string> editorPathDic = new Dictionary<string, string>
    {
        {VFX_DEATH, "Prefabs/Combat/Death/"},
        {VFX_ATTACK, "Prefabs/Combat/Sword/SwordHitCritical/"}
    };

    private static string GetEditorAssetPath(string obj)
    {
        editorPathDic.TryGetValue(obj, out var path);
        return path;
    }
#endif

    public const string VFX_DEATH = "ElectricDeathBlue";
    public const string VFX_ATTACK = "SwordHitBlueCritical";

    private static string GetEffectPath(string effectName)
    {
        string path = string.Empty;
#if UNITY_EDITOR
        path = GetEditorAssetPath(effectName) + effectName;
#else
        path = VFX_DEATH;
#endif
        return path;
    }

    public static GameObject CreateEffect(string effect)
    {
        return CreateEffect(effect, Vector3.zero, Quaternion.identity);
    }

    public static GameObject CreateEffect(string effect, Vector3 pos, Quaternion rotation)
    {
        var effectPath = GetEffectPath(effect);
        LoaderService loader = ServiceLocate.Instance.GetService<LoaderService>();
        return InstantiateEffect(effectPath, pos, rotation);
    }

    public static GameObject InstantiateEffect(string effectPath, Vector3 pos, Quaternion rotation)
    {
        var asset = LoadEffect(effectPath);
        if (asset == null)
            return null;
        return UnityEngine.Object.Instantiate(asset, pos, rotation) as GameObject;
    }


    private static UnityEngine.Object LoadEffect(string name)
    {
        string finalPath = String.Empty;
        UnityEngine.Object asset = null;
#if UNITY_EDITOR
        const string path = "Assets/Art/Epic Toon FX/";
        finalPath = path + name;
        asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(finalPath + ".prefab");
#else
        finalPath = name;
        asset = Resources.Load<GameObject>(finalPath);
#endif
        return asset;
    }

    #endregion
    
}