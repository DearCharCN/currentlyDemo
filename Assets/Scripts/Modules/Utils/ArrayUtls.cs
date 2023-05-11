using System;
using System.Collections.Generic;

public static class ArrayUtls
{
    /// <summary>
    /// �����󳤶ȵ�����
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static int GetLongestIndex<T>(IEnumerable<T[]> list)
    {
        int maxLen = int.MinValue;
        int maxIdx = -1;

        int idx = 0;
        foreach (var b in list)
        {
            if (b != null && b.Length > maxLen)
            {
                maxLen = b.Length;
                maxIdx = idx;
            }
            ++idx;
        }
        return maxIdx;
    }

    /// <summary>
    /// ����ϲ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arrays"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static T[] Merage<T>(params T[][] arrays)
    {
        int totalLen = 0;
        for (int i = 0; i < arrays.Length; i++)
        {
            if (arrays == null)
                throw new Exception("array is null");

            totalLen += arrays[i].Length;
        }
        T[] result = new T[totalLen];

        int sumLen = 0;
        for (int i = 0; i < arrays.Length; i++)
        {
            T[] data = arrays[i];
            Array.Copy(data, 0, result, sumLen, data.Length);
            sumLen += data.Length;
        }
        return result;
    }

    /// <summary>
    /// ������ض�
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cutIndex"></param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public static T[][] CutOff<T>(T[] data, int cutIndex)
    {
        if (cutIndex <= data.Length)
            throw new System.Exception("cutIndex greater or equal than length");

        T[] r1 = new T[cutIndex];
        T[] r2 = new T[data.Length - cutIndex];

        Array.Copy(data, 0, r1, 0, cutIndex);
        Array.Copy(data, cutIndex, r2, 0, data.Length - cutIndex);
        T[][] result = new T[][] { r1, r2 };
        return result;
    }

    /// <summary>
    /// ������ضϣ��ضϵ����
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static T[][] CutOffWithRandomlyCutPos<T>(T[] data)
    {
        int s = UnityEngine.Random.Range(1, data.Length);
        return ArrayUtls.CutOff(data, s);
    }

    /// <summary>
    /// ����������ض�Ϊ�̶�����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T[][] CutOffByCount<T>(T[] data, int count)
    {
        if (data.Length - 1 <= count)
            return null;
        List<T[]> r = new List<T[]>();
        r.AddRange(ArrayUtls.CutOffWithRandomlyCutPos(data));
        for (int i = 1; i < count - 1; i++)
        {
            int splitIdx = ArrayUtls.GetLongestIndex(r);
            T[][] sqRes = ArrayUtls.CutOffWithRandomlyCutPos(r[splitIdx]);
            r.RemoveAt(splitIdx);
            r.InsertRange(splitIdx, sqRes);
        }
        return r.ToArray();
    }
}