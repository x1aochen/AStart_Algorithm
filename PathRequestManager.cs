using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多个路径时的处理
/// </summary>
public class PathRequestManager : MonoBehaviour
{
    //所有路径请求，先进先出
    private Queue<PathRequest> pathRequests = new Queue<PathRequest>();
    //当前路径请求
    private PathRequest currentRequest;

    private static PathRequestManager instance;

    private PathFinding pathFinding;
    //是否正在处理路径
    private bool isProcessingPath;
    private void Awake()
    {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }

    public static void RequestPath(Vector3 pathStart,Vector3 pathEnd,Action<Vector3[],bool> callback)
    {
        //新请求入队
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequests.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    /// <summary>
    /// 尝试处理下一个路径
    /// </summary>
    private void TryProcessNext()
    {
        //如果当前未在工作中 并且 队列不为空
        if (!isProcessingPath && pathRequests.Count > 0)
        {
            //设置当前处理路径
            currentRequest = pathRequests.Dequeue();
            isProcessingPath = true;
            //开始进行寻路
            pathFinding.StartFindPath(currentRequest.pathStart, currentRequest.pathEnd);
        }
    }

    /// <summary>
    /// 结束后寻路后的处理
    /// </summary>
    public void FinishedProcessingPath(Vector3[] path,bool success)
    {
        //呼叫回调函数
        currentRequest.callback(path, success);
        //设置正在执行false
        isProcessingPath = false;
        //继续尝试进行下一条路径的处理
        TryProcessNext();
    }
    /// <summary>
    /// 发起路径请求所需信息
    /// </summary>
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
        {
            this.pathStart = start;
            this.pathEnd = end;
            this.callback = callback;
        }
    }
}
