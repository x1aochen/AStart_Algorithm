using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPos;

    //节点位于网格的位置
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;
    private int heapIndex;

    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPos = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;

    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get => heapIndex;
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node node)
    {
        //前者大于后者返回1，则表示前者代价更高
        int compare = fCost.CompareTo(node.fCost);
        //如果 代价相等，则根据离终点距离代价判断
        if (compare == 0)
        {
            compare = hCost.CompareTo(node.hCost);
        }
        //需要的是代价更低，即优先级更高的节点
        return -compare;
    }

}
