using System;
using UnityEngine;

[Serializable]
public struct KVIntMap
{
    [SerializeField]
    private int mKey;
    [SerializeField]
    private int mValue;

    public int Key => mKey;
    public int Value => mValue;

    public KVIntMap(int mKey, int mValue)
    {
        this.mKey = mKey;
        this.mValue = mValue;
    }
}