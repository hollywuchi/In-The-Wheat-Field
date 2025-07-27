using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    [SceneName] public string sceneName;
    [Header("整个地图的大小")]
    public int gridWitch;
    public int gridHeight;
    [Header("地图左下角点的位置")]
    public int originX;
    public int originY;
    public List<TileProperty> tileProperties;
}
