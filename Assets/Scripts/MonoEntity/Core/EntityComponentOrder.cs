using System.Collections.Generic;
using System;

public static partial class EntityComponentOrder
{
    public static int TypeCount => componentTypes.Count;
    private static Dictionary<Type, int> componentTypeIndexDic = null;

    private static void CalculateOrders()
    {
        componentTypeIndexDic = new();
        for (int i = 0; i < componentTypes.Count; i++)
        {
            componentTypeIndexDic.Add(componentTypes[i], i);
        }
    }

    public static int GetOrder(Type type)
    {
        if (componentTypeIndexDic == null)
            CalculateOrders();
        if (componentTypeIndexDic!.ContainsKey(type))
            return componentTypeIndexDic[type];
        return -1; // -1 在排序时仍然有意义, 即排在最前面, 不过还是希望所有的都加到 componentTypes 中
    }
}