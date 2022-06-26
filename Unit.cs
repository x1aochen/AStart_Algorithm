using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �н����Ե�λ
/// </summary>
public class Unit : MonoBehaviour
{
    public Transform target; //Ŀ��
    public float speed = 20; //�н��ٶ�
    private Vector3[] path;  //·������
    private int targetIndex;
    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position,OnPathFound);
    }
    //�ҵ�·����ִ��
    public void OnPathFound(Vector3[] newPath,bool pathSuccessful) 
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            //��ʼǰ��
            StartCoroutine("FollowPath");
        }
    }

    /// <summary>
    /// ����·����Ŀ���н���ȥ
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowPath()
    {
        //�ӵ�һ�㿪ʼ
        Vector3 currenWayPoint = path[0];

        while (true)
        {
            //����ǰ��
            if (transform.position == currenWayPoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currenWayPoint = path[targetIndex]; 
            }
            transform.position = Vector3.MoveTowards(transform.position, currenWayPoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex;i < path.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                } 
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        } 
    }
}
