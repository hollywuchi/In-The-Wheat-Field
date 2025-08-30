using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // 让脚本持续运行，以保证guid的唯一性
public class DataGUID : MonoBehaviour
{
    public string guid;

    void Awake()
    {
        if (guid == string.Empty)
            guid = System.Guid.NewGuid().ToString();
    }
}
