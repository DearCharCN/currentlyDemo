using System.Collections.Generic;

public static class ListUtils
{
    public static void InsertRange<T>(this List<T> list, int index, IEnumerable<T> items)
    {
        if (list == null)
            throw new System.Exception("this is null");

        if (index > list.Count)
            throw new System.Exception("Index greater or equal than Count");

        if (index == list.Count)
        {
            list.AddRange(items);
            return;
        }
        foreach (var d in items)
        {
            list.Insert(index, d);
            ++index;
        }
    }

    public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> items)
    {
        if (list == null)
            throw new System.Exception("this is null");

        if (index == list.Count)
        {
            list.AddRange(items);
            return;
        }
        foreach (var d in items)
        {
            list.Insert(index, d);
            ++index;
        }
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list == null)
            throw new System.Exception("this is null");

        foreach (var item in items)
        {
            list.Add(item);
        }
    }
}
