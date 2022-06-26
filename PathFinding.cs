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
        //��ʱ��
        Stopwatch sw = new Stopwatch();
        sw.Start();
        //���ջ��ݵ�·��
        Vector3[] wayPoints = new Vector3[0];
        //�Ƿ�Ѱ�ҳɹ�
        bool pathSuccess = false;
 
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetpos);
        //ֻ�������յ㶼�����赲��ʱ���ִ��
        if (startNode.walkable && targetNode.walkable)
        {
            //�����б�
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            //�ر��б�
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);

            //��·�������б�Ϊ��
            while (openSet.Count > 0)
            {
                //�ҵ�������ͽڵ�
                Node currenNode = openSet.RemoveFirst();

                //����ر��б�
                closeSet.Add(currenNode);

                if (currenNode == targetNode)
                {
                    sw.Stop();
                    //��ӡѰ��ʱ��
                    print("path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }
                foreach (var neighbour in grid.GetNeighbours(currenNode))
                {
                    if (!neighbour.walkable || closeSet.Contains(neighbour))
                        continue;

                    //�ھӾ��������´���
                    int moveCostToNeighbour = currenNode.gCost + GetDistance(currenNode, neighbour);
                    //����������С��ԭ�ȵĴ��� ���� ��һ�� �±����ڵ�
                    if (moveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        //�������
                        neighbour.gCost = moveCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currenNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else //����Ѿ��ڿ����б��У���������ȼ�
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
    /// ����·��
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns>����·��������</returns>
    private Vector3[] RetracePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currenNode = endNode;
        //���ݸ��ڵ����
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
    /// ������·����ԭ�ȵ�·����Grid��ʹ��List�ڶ��ϴ洢�ڵ�Node������Grid��·���������
    /// ����ֱ����һ������洢�ڷ����Ϸ����˸ı����Щ��
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
    /// ��������
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
