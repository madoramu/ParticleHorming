using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
    private static bool CheckList<T>(List<T> list)
    {
        return list != null && list.Count > 0;
    }

    public static T GetRandom<T>(this List<T> list)
    {
        if(!CheckList(list)) return default(T);
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}