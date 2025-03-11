using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph : IGragh<int>
{
    private readonly Dictionary<int, Dictionary<int, (float weight, float capacity)>> adjacencyList;

    public Graph()
    {
        adjacencyList = new Dictionary<int, Dictionary<int, (float, float)>>();
    }

    public void AddVertex(int vertex)
    {
        if (!adjacencyList.ContainsKey(vertex))
        {
            adjacencyList[vertex] = new Dictionary<int, (float, float)>();
        }
    }

    public void AddEdge(int from, int to, float weight, float capacity)
    {
        if (!adjacencyList.ContainsKey(from))
        {
            AddVertex(from);
        }
        if (!adjacencyList.ContainsKey(to))
        {
            AddVertex(to);
        }
        adjacencyList[from][to] = (weight, capacity);
        adjacencyList[to][from] = (weight, 0); // 反向边，初始容量为0
    }

    public IEnumerable<int> GetVertices()
    {
        return adjacencyList.Keys;
    }

    public IEnumerable<(int from, int to, float weight, float capacity)> GetEdges()
    {
        var edges = new HashSet<(int, int, float, float)>();
        foreach (var from in adjacencyList)
        {
            foreach (var to in from.Value)
            {
                edges.Add((from.Key, to.Key, to.Value.weight, to.Value.capacity));
            }
        }
        return edges;
    }

    public IEnumerable<(int neighbor, float weight, float capacity)> GetNeighbors(int vertex)
    {
        if (adjacencyList.ContainsKey(vertex))
        {
            foreach (var neighbor in adjacencyList[vertex])
            {
                yield return (neighbor.Key, neighbor.Value.weight, neighbor.Value.capacity);
            }
        }
    }

    public bool ContainsVertex(int vertex)
    {
        return adjacencyList.ContainsKey(vertex);
    }

    public bool ContainsEdge(int from, int to)
    {
        return adjacencyList.ContainsKey(from) && adjacencyList[from].ContainsKey(to);
    }

    public float GetEdgeWeight(int from, int to)
    {
        if (ContainsEdge(from, to))
        {
            return adjacencyList[from][to].weight;
        }
        throw new KeyNotFoundException("Edge does not exist.");
    }

    public float GetEdgeCapacity(int from, int to)
    {
        if (ContainsEdge(from, to))
        {
            return adjacencyList[from][to].capacity;
        }
        throw new KeyNotFoundException("Edge does not exist.");
    }

    // SPFA算法
    private bool SPFA(int s, int t, int[] last, float[] flow, float[] dist)
    {
        var inq = new bool[adjacencyList.Count];
        var que = new Queue<int>();
        Array.Fill(last, -1);
        Array.Fill(dist, float.MaxValue);
        flow[s] = float.MaxValue;
        dist[s] = 0;
        que.Enqueue(s);

        while (que.Count > 0)
        {
            int q = que.Dequeue();
            inq[q] = false;
            foreach (var neighbor in adjacencyList[q])
            {
                int to = neighbor.Key;
                float capacity = neighbor.Value.capacity;
                float cost = neighbor.Value.weight;
                if (capacity > 0 && dist[to] > dist[q] + cost)
                {
                    last[to] = q;
                    flow[to] = Math.Min(flow[q], capacity);
                    dist[to] = dist[q] + cost;
                    if (!inq[to])
                    {
                        que.Enqueue(to);
                        inq[to] = true;
                    }
                }
            }
        }
        return last[t] != -1;
    }

    //最小费用最大流算法
    public (float maxflow, float mincost) MCMF(int s, int t)
    {
        float maxflow = 0, mincost = 0;
        var last = new int[adjacencyList.Count];
        var flow = new float[adjacencyList.Count];
        var dist = new float[adjacencyList.Count];

        while (SPFA(s, t, last, flow, dist))
        {
            maxflow += flow[t];
            mincost += dist[t] * flow[t];
            for (int i = t; i != s; i = last[i])
            {
                int from = last[i];
                adjacencyList[from][i] = (adjacencyList[from][i].weight, adjacencyList[from][i].capacity - flow[t]);
                adjacencyList[i][from] = (adjacencyList[i][from].weight, adjacencyList[i][from].capacity + flow[t]);
            }
        }
        return (maxflow, mincost);
    }

    public IEnumerable<int> DFS(int startVertex)
    {
        var visited = new HashSet<int>();
        var stack = new Stack<int>();
        stack.Push(startVertex);

        while (stack.Count > 0)
        {
            var vertex = stack.Pop();
            if (!visited.Contains(vertex))
            {
                visited.Add(vertex);
                yield return vertex;

                foreach (var neighbor in adjacencyList[vertex].Keys)
                {
                    if (!visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }
    }

    public IEnumerable<int> BFS(int startVertex)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(startVertex);

        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            if (!visited.Contains(vertex))
            {
                visited.Add(vertex);
                yield return vertex;

                foreach (var neighbor in adjacencyList[vertex].Keys)
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }
}

public class TestGraph
{
    public static void Main()
    {
        var graph = new Graph();
        int n = 4; //顶点数
        int s = 0, t = 3; //源点和汇点

        //添加顶点
        for (int i = 0; i < n; i++)
        {
            graph.AddVertex(i);
        }

        //添加边 (from, to, weight, capacity)
        graph.AddEdge(0, 1, 1, 3);
        graph.AddEdge(0, 2, 1, 2);
        graph.AddEdge(1, 2, 1, 1);
        graph.AddEdge(1, 3, 1, 2);
        graph.AddEdge(2, 3, 1, 4);

        var result = graph.MCMF(s, t);
        Debug.Log($"Maxflow: {result.maxflow}, Mincost: {result.mincost}");
    }
}

