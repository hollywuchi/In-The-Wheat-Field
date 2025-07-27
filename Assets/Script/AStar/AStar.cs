using System.Collections.Generic;
using Farm.Map;
using UnityEngine;

namespace Farm.AStar
{
    public class AStar : MonoBehaviour
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;
        private int gridWitch;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodeList;        // 当前选中Node周围的八个点
        private HashSet<Node> closedNodeList;   // 所有被选中的点
        private bool pathFound;


        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int targetPos)
        {
            pathFound = false;

            // 查找最短路径
            if (FindShortestPath())
            {
                // 构建NPC移动路径
            }

        }

        /// <summary>
        /// 构建网格节点信息，初始化两个列表
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="startPos">起点</param>
        /// <param name="targetPos">终点</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int targetPos)
        {
            if (GridMapManager.Instance.GetGridDemensions(sceneName, out Vector2Int gridDimensions, out Vector2Int girdOrigin))
            {
                // 根据瓦片地图范围构建网格移动节点范围数组
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWitch = gridDimensions.x;
                gridHeight = gridDimensions.y;

                originX = girdOrigin.x;
                originY = girdOrigin.y;

                openNodeList = new List<Node>();

                closedNodeList = new HashSet<Node>();

                // GridNodes的范围是从0开始，所以要减去原点坐标得到实际位置
                startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
                targetNode = gridNodes.GetGridNode(targetPos.x - originX, targetPos.y - originY);

            }
            else
                return false;

            for (int x = 0; x < gridWitch; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    // FIXME:样例有误
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);

                    TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);

                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCObstacle)
                            node.isObstable = true;
                    }
                }
            }
            return true;
        }


        private bool FindShortestPath()
        {
            // 添加起点
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                // Node排序，内部比较函数
                openNodeList.Sort();

                Node closeNode = openNodeList[0];

                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;

                }

                // 计算周围8个节点补充到OpenList
            }
            return pathFound;
        }
    }
}