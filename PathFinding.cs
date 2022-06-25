using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private Grid grid;

    public Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    private void FindPath(Vector3 startPos,Vector3 targetpos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetpos);

        //开启列表
        List<Node> openSet = new List<Node>();
        //关闭列表
        HashSet<Node> closeSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currenNode = openSet[0];
            //找到代价最低节点
            for (int i = 1;i < openSet.Count; i++)
            {
                //如果 遍历点的 离终点代价h 大于等于当前节点的h，则直接不考虑
                if (openSet[i].fCost < currenNode.fCost || openSet[i].fCost == currenNode.fCost 
                    && openSet[i].hCost < currenNode.hCost)
                {
                    currenNode = openSet[i];
                }
            }

            //从开启列表中移除，加入关闭列表
            openSet.Remove(currenNode);
            closeSet.Add(currenNode);

            if (currenNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            foreach (var neighbour in grid.GetNeighbours(currenNode))
            {
                if (!neighbour.walkable || closeSet.Contains(neighbour))
                    continue;

                //邻居距离起点的代价
                int moveCostToNeighbour = currenNode.gCost + GetDistance(currenNode, neighbour);
                //如果这个代价小于原先的代价 或者 是一个 新遍历节点
                if (moveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //计算代价
                    neighbour.gCost = moveCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currenNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    /// <summary>
    /// 回溯路径
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    private void RetracePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currenNode = endNode;
        //根据父节点回溯
        while (currenNode != startNode)
        {
            path.Add(currenNode);
            currenNode = currenNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }

    private int GetDistance(Node nodeA,Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }
}
