using UnityEngine;
namespace Farm.AStar
{
    public class GridNodes
    {
        public int witch;
        public int height;
        private Node[,] gridNodes;

        /// <summary>
        /// 构造函数初始化节点范围数组
        /// </summary>
        /// <param name="witch">地图宽度</param>
        /// <param name="height">地图高度</param>
        public GridNodes(int witch, int height)
        {
            this.witch = witch;
            this.height = height;

            gridNodes = new Node[witch, height];
            for (int x = 0; x < witch; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridNodes[x, y] = new Node(new Vector2Int(x, y));
                }
            }
        }

        public Node GetGridNode(int xPos, int yPos)
        {
            if (xPos < witch && yPos < height)
            {
                return new Node(new Vector2Int(xPos, yPos));
            }
            Debug.Log("超出地图范围");
            return null;
        }

    }
}