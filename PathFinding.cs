using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathFinding : MonoBehaviour
{
    private PathRequestManager requestManager;

    private Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 pathStart, Vector3 pathEnd)
    {
        StartCoroutine(FindPath(pathStart, pathEnd));
    }

    private IEnumerator FindPath(Vector3 startPos,Vector3 targetpos)
    {
        //计时器
        Stopwatch sw = new Stopwatch();
        sw.Start();
        //最终回溯的路径
        Vector3[] wayPoints = new Vector3[0];
        //是否寻找成功
        bool pathSuccess = false;
 
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetpos);
        //只有起点和终点都不是阻挡的时候才执行
        if (startNode.walkable && targetNode.walkable)
        {
            //开启列表
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            //关闭列表
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);

            //死路：开启列表为空
            while (openSet.Count > 0)
            {
                //找到代价最低节点
                Node currenNode = openSet.RemoveFirst();

                //加入关闭列表
                closeSet.Add(currenNode);

                if (currenNode == targetNode)
                {
                    sw.Stop();
                    //打印寻找时间
                    print("path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }
                foreach (var neighbour in grid.GetNeighbours(currenNode))
                {
                    if (!neighbour.walkable || closeSet.Contains(neighbour))
                        continue;

                    //邻居距离起点的新代价
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
                        else //如果已经在开启列表中，则更新优先级
                        { 
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(wayPoints, pathSuccess);

    }

    /// <summary>
    /// 回溯路径
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns>返回路径点数组</returns>
    private Vector3[] RetracePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currenNode = endNode;
        //根据父节点回溯
        while (currenNode != startNode)
        {
            path.Add(currenNode);
            currenNode = currenNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints; 
    }
    /// <summary>
    /// 简化最终路径，原先的路径在Grid中使用List在堆上存储节点Node，并且Grid与路径是相关联
    /// 现在直接用一个数组存储在方向上发生了改变的那些点
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1;i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPos);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
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
