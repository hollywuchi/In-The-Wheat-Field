using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPattenList_SO", menuName = "Light/Light Patten")]
public class LightPattenList_SO : ScriptableObject
{
    public List<LightDetails> lightDetails = new List<LightDetails>();

    /// <summary>
    /// 根据季节返回光照信息
    /// </summary>
    /// <param name="season">季节</param>
    /// <param name="lightShift">光照时间</param>
    /// <returns></returns>
    public LightDetails GetLightDetails(Season season, LightShift lightShift)
    {
        return lightDetails.Find(l => l.season == season && l.lightShift == lightShift);
    }
}


[System.Serializable]
public class LightDetails
{
    public Season season;
    public LightShift lightShift;
    public Color lightColor;
    public float lightAmount;
}