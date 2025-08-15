using System;
using System.Collections.Generic;

public static partial class EntityComponentOrder
{
    private static List<Type> componentTypes = new()
    {
        typeof(AnimatorComponent),
        typeof(SurroundDetectComponent),
        typeof(MoveComponent),
    };
}