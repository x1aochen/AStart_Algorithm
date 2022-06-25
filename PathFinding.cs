using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
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
        if (Input.GetButtonDown("Jump"))
            FindPath(seeker.position, target.position);
    }

    private void FindPath(Vector3 startPos,Vector3 targetpos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetpos);

        //开启列表
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        //关闭列表
        HashSet<Node> closeSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            //找到代价最低节点

            Node currenNode = openSet.RemoveFirst();

            //从开启列表中移除，加入关闭列表
            closeSet.Add(currenNode);

            if (currenNode == targetNode)
            {
                sw.Stop();
                //打印寻找时间
                print("path found: " + sw.ElapsedMilliseconds + " ms" );
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
                    {
                        openSet.Add(neighbour);
                    }
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

    /// <summary>
    /// 两点间距离
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private int GetDistance(Node nodeA,Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }
}
