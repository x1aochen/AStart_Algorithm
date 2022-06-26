using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行进测试单位
/// </summary>
public class Unit : MonoBehaviour
{
    public Transform target; //目标
    public float speed = 20; //行进速度
    private Vector3[] path;  //路径坐标
    private int targetIndex;
    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position,OnPathFound);
    }
    //找到路径后执行
    public void OnPathFound(Vector3[] newPath,bool pathSuccessful) 
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            //开始前进
            StartCoroutine("FollowPath");
        }
    }

    /// <summary>
    /// 跟随路径向目标行进而去
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowPath()
    {
        //从第一点开始
        Vector3 currenWayPoint = path[0];

        while (true)
        {
            //持续前进
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
