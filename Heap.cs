using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 堆结构，堆排序
/// 维持一个小根堆，顶部节点为代价最低点
/// </summary>
/// <typeparam name="T"></typeparam>
public class Heap<T> where T : IHeapItem<T>
{
    //堆底层采用数组
    private T[] items;
    //堆大小 heapSize
    private int heapSize;
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = heapSize;
        items[heapSize] = item;
        //每加入一个节点就进行优先级排序
        SortUp(item);
        heapSize++;
    }
    /// <summary>
    /// 移除第一项
    /// </summary>
    public T RemoveFirst()
    {
        T firstItem = items[0];
        heapSize--;
        items[0] = items[heapSize];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    /// <summary>
    /// 判断当前节点是否存在
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        //当前节点所存储的堆下标，在数组中对应的值是否是自己
        return Equals(items[item.HeapIndex], item);
    }
    
    /// <summary>
    /// A星：找到更低代价的节点后进行更新优先级
    /// </summary>
    /// <param name="item"></param>
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    /// <summary>
    /// 堆大小
    /// </summary>
    public int Count
    {
        get => heapSize;
    }

    //删除顶部节点时的向下排序，从上往下寻找儿子与自身作比较
    private void SortDown(T item)
    {
        while (true)
        {
            int leftIndex = item.HeapIndex * 2 + 1;
            int rightIndex = item.HeapIndex * 2 + 2;
            int tmpIndex = 0;

            //左孩子存在则进行
            if (leftIndex < heapSize)
            {
                tmpIndex = leftIndex;
                //右孩子也存在
                if (rightIndex < heapSize)
                {
                    //比较左右两孩子,如果左孩子优先级更低
                    if (items[leftIndex].CompareTo(items[rightIndex]) < 0)
                    {
                        tmpIndex = rightIndex;
                    }
                }
                //自身与 优先级更高的孩子做比较，谁高谁做父亲
                if (item.CompareTo(items[tmpIndex]) < 0)
                {
                    Swap(item, items[tmpIndex]);
                }
                else
                {
                    return;
                }
            }
            //如果没有任何子节点
            else
                return;
        }
    }


    //增加节点时的向上排序，从下往上找 父亲与自身作比较
    private void SortUp(T item)
    {
        //父节点索引 = 自己的索引 - 1 除 2
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            //item类实现比较方法
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            } else
            {
                break;  
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T itemA,T itemB)
    {
        //自身数组内位置交换
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        //T对象中存储的其自身在堆中索引的交换
        int tmp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tmp;
    }

}
//要求必须存储T对象在堆中索引并且提供比较方法
public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex
    {
        get;
        set;
    }
}


