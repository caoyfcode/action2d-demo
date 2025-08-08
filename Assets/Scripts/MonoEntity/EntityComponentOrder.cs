using System;
using System.Collections.Generic;

public partial class EntityComponentOrder
{
    private static List<Type> componentTypes = new()
    {
        typeof(AnimatorComponent),
        typeof(MovementComponent),
    };
}