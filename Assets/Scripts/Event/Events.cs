using UnityEngine;

public class EvtStartBossFight : IEvent
{
    public Entity boss;
    public Rect bossRoom;
}

public class EvtStopBossFight : IEvent
{
}