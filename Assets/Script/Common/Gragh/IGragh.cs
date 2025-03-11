using System.Collections.Generic;
using UnityEngine;

public interface IGragh<T>
{
    //���һ������
    void AddVertex(T vertex);

    //���һ����Ȩ�غ������ı�
    void AddEdge(T from, T to, float weight, float capacity);

    //��ȡ���ж���
    IEnumerable<T> GetVertices();

    //��ȡ���б߼���Ȩ�غ�����
    IEnumerable<(T from, T to, float weight, float capacity)> GetEdges();

    //��ȡ��ĳ���������ڵĶ��㼰��ߵ�Ȩ�غ�����
    IEnumerable<(T neighbor, float weight, float capacity)> GetNeighbors(T vertex);

    //�ж�ͼ���Ƿ����ĳ������
    bool ContainsVertex(T vertex);

    //�ж�ͼ���Ƿ����ĳ����
    bool ContainsEdge(T from, T to);

    //��ȡĳ���ߵ�Ȩ��
    float GetEdgeWeight(T from, T to);

    //��ȡĳ���ߵ�����
    float GetEdgeCapacity(T from, T to);
}
