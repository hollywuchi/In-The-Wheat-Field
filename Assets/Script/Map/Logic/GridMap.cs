using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GirdType girdType;
    private Tilemap currentTileMap;


    void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTileMap = GetComponent<Tilemap>();

            if (mapData != null)
                mapData.tileProperties.Clear();
        }
    }

    void OnDisable()
    {

        if (!Application.IsPlaying(this))
        {
            currentTileMap = GetComponent<Tilemap>();

            UpdateTileProperties();
#if UNITY_EDITOR
            if (mapData != null)
                EditorUtility.SetDirty(mapData);
#endif
        }
    }

    private void UpdateTileProperties()
    {
        // 压缩地图？
        currentTileMap.CompressBounds();

        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                // 地图左下角坐标
                Vector3Int startPos = currentTileMap.cellBounds.min;
                // 地图右下角坐标
                Vector3Int endPos = currentTileMap.cellBounds.max;

                for (int x = startPos.x; x < endPos.x; x++)
                {
                    for (int y = startPos.y; y < endPos.y; y++)
                    {
                        TileBase tile = currentTileMap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tileCoordinate = new Vector2Int(x, y),
                                girdType = girdType,
                                boolTypeValue = true
                            };

                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
