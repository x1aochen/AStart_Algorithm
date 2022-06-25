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

        //�����б�
        List<Node> openSet = new List<Node>();
        //�ر��б�
        HashSet<Node> closeSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currenNode = openSet[0];
            //�ҵ�������ͽڵ�
            for (int i = 1;i < openSet.Count; i++)
            {
                //��� ������� ���յ����h ���ڵ��ڵ�ǰ�ڵ��h����ֱ�Ӳ�����
                if (openSet[i].fCost < currenNode.fCost || openSet[i].fCost == currenNode.fCost 
                    && openSet[i].hCost < currenNode.hCost)
                {
                    currenNode = openSet[i];
                }
            }

            //�ӿ����б����Ƴ�������ر��б�
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

                //�ھӾ������Ĵ���
                int moveCostToNeighbour = currenNode.gCost + GetDistance(currenNode, neighbour);
                //����������С��ԭ�ȵĴ��� ���� ��һ�� �±����ڵ�
                if (moveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //�������
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
    /// ����·��
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    private void RetracePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currenNode = endNode;
        //���ݸ��ڵ����
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
