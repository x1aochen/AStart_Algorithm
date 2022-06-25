using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;

    //�ڵ�λ�������λ��
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;


    public Node(bool walkable, Vector3 worldPos,int gridX,int gridY)
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
}
