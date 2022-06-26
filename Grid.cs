using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    //��ͼ��С
    public Vector2 gridWorldSize;
    //�ڵ�뾶
    public float nodeRadius;
    private Node[,] grid;

    //�ڵ�ֱ��
    private float nodeDiameter;
    //��ͼ����
    private int gridSizeX, gridSizeY;
    /// <summary>
    /// �����С
    /// </summary>
    public int MaxSize => gridSizeX * gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreadGrid();
    }
    //��ʼ����ͼ
    private void CreadGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        //�������½ǵ�
        Vector3 worldBottmLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //�ڵ����꣬���������½ǵ㿪ʼ�������нڵ㣬��Ϊ���Խڵ�����λ�ã�����Ӧ�ü��ϰ뾶��Ϊ�˲��ýڵ�Խ��
                Vector3 worldPoint = worldBottmLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //������ײ����жϵ�ǰλ���Ƿ������
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    /// <summary>
    /// ������Χ�ڵ�
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                //�ж��Ƿ�Խ��
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //��������ת�ڵ�����
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //�õ����������ڵ�ͼ�ϵİٷֱ�
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        //���ٷֱȿ�����0��1֮��
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //�����������ֵ gridSize - 1
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.black;
                Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}