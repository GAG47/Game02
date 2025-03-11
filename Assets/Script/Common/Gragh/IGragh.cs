using System.Collections.Generic;
using UnityEngine;

public interface IGragh<T>
{
    //添加一个顶点
    void AddVertex(T vertex);

    //添加一条带权重和容量的边
    void AddEdge(T from, T to, float weight, float capacity);

    //获取所有顶点
    IEnumerable<T> GetVertices();

    //获取所有边及其权重和容量
    IEnumerable<(T from, T to, float weight, float capacity)> GetEdges();

    //获取与某个顶点相邻的顶点及其边的权重和容量
    IEnumerable<(T neighbor, float weight, float capacity)> GetNeighbors(T vertex);

    //判断图中是否包含某个顶点
    bool ContainsVertex(T vertex);

    //判断图中是否包含某条边
    bool ContainsEdge(T from, T to);

    //获取某条边的权重
    float GetEdgeWeight(T from, T to);

    //获取某条边的容量
    float GetEdgeCapacity(T from, T to);
}
