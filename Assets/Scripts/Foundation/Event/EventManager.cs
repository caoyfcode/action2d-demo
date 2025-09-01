
using System;
using System.Collections.Generic;

public interface IEvent {}

public class EventManager : SingletonBase<EventManager>
{
    private Dictionary<Type, List<Listener>> eventListeners = new();
    private bool isHandlingEvent = false;
    private List<(Type, Delegate)> deferRemoveListeners = new();

    // TODO filter (譬如事件目标 filter, 即已经有的 target, 也可以有 source, 甚至进一步有 group)
    private struct Listener
    {
        public Delegate handler;
        public object target; // listener 的 target 为 null 表示接受任何目标的事件, 在 TriggerEvent 时如果目标为 null 则只有为 null 的 listener 可以接收到
    }
    
    public void Register<T>(Action<T> handler, object target = null) where T : IEvent
    {
        // 添加是不用管 isHandlingEvent, 继续添加到后面
        var type = typeof(T);
        if (!eventListeners.ContainsKey(type))
        {
            eventListeners.Add(type, new());
        }
        eventListeners[type].Add(new Listener()
        {
            handler = handler,
            target = target,
        });

    }

    public void UnRegister<T>(Action<T> handler) where T : IEvent
    {
        var type = typeof(T);
        if (!eventListeners.ContainsKey(type))
            return;
        var result = eventListeners[type].FindAll(l => l.handler == handler as Delegate);
        if (result.Count == 0)
            return;
        if (isHandlingEvent)
        {
            deferRemoveListeners.Add((type, handler));
        }
        else
        {
            eventListeners[type].RemoveAll(l => l.handler == handler as Delegate);
        }
    }

    private void RemoveAllDeferredListeners()
    {
        foreach (var (type, handler) in deferRemoveListeners)
        {
            if (eventListeners.ContainsKey(type))
            {
                eventListeners[type].RemoveAll(l => l.handler == handler);
            }
        }
    }

    public void TriggerEvent<T>(T evt, object target = null) where T : IEvent
    {
        var type = typeof(T);
        isHandlingEvent = true;
        if (eventListeners.TryGetValue(type, out var listeners))
        {
            // 这里必须用普通for循环，因为触发过程中可能 listeners 增多
            for (int i = 0; i < listeners.Count; i++)
            {
                var listener = listeners[i];
                if (listener.target == null || listener.target == target)
                {
                    listener.handler?.DynamicInvoke(evt);
                }
                isHandlingEvent = true; // 这里是考虑触发过程中又触发的行为
            }
        }
        RemoveAllDeferredListeners();
        isHandlingEvent = false;
    }
}






// 下面是老的试图具有队列的代码，不采用
// public class EventManager : SingletonBase<EventManager>
// {
//     private List<EventData> eventQueue = new();
//     private Dictionary<Type, List<ListenerData>> eventListeners = new();
//     private bool isHandlingEvent = false;
//     private List<(Type, Delegate)> deferRemoveListeners = new();
//     
//     // TODO filter (譬如事件目标 filter)
//
//     private enum EventAliveType
//     {
//         Instant, // 立即触发，无论处理与否都删除
//         UntilProcessed, // 队列触发，直到被处理
//         UntilProcessedOrTimeout, // 队列触发，直到超出时间或被处理
//         FixedDuration, // 队列触发，保持固定时间
//     }
//     
//     private struct EventData
//     {
//         public Type type;
//         public IEvent evt;
//         public object target;
//         public EventAliveType aliveType;
//         public float destroyTimer;
//
//         public void TimeDec(float delta)
//         {
//             destroyTimer -= delta;
//         }
//     }
//     
//     private struct ListenerData
//     {
//         public Delegate handler;
//         public object target;
//     }
//
//     public void Register<T>(Action<T> handler, object target=null) where T : IEvent
//     {
//         var type = typeof(T);
//         if (!eventListeners.ContainsKey(type))
//         {
//             eventListeners.Add(type, new ());
//         }
//         else
//         {
//             eventListeners[type].Add(new ListenerData()
//             {
//                 handler = handler,
//                 target = target,
//             });
//         }
//         
//     }
//
//     public void UnRegister<T>(Action<T> handler) where T : IEvent
//     {
//         var type = typeof(T);
//         if (!eventListeners.ContainsKey(type))
//             return;
//         var result = eventListeners[type].FindAll(l => l.handler == handler as Delegate);
//         if (result.Count == 0)
//             return;
//         if (isHandlingEvent)
//         {
//             deferRemoveListeners.Add((type, handler));
//         }
//         else
//         {
//             eventListeners[type].RemoveAll(l => l.handler == handler as Delegate);
//         }
//     }
//
//     private void RemoveAllDeferredListeners()
//     {
//         foreach (var (type, handler) in deferRemoveListeners)
//         {
//             if (eventListeners.ContainsKey(type))
//             {
//                 eventListeners[type].RemoveAll(l => l.handler == handler as Delegate);
//             }
//         }
//     }
//     
//     public void TriggerEvent<T>(T evt, object target=null) where T : IEvent
//     {
//         AddEventInner(new EventData()
//         {
//             type = typeof(T),
//             evt = evt,
//             target = target,
//             aliveType = EventAliveType.Instant
//         });
//     }
//
//     public void QueueEvent<T>(T evt, object target=null) where T : IEvent
//     {
//         AddEventInner(new EventData()
//         {
//             type = typeof(T),
//             evt = evt,
//             target = target,
//             aliveType = EventAliveType.UntilProcessed
//         });
//     }
//     
//     public void QueueEvent<T>(T evt, float maxDuration, object target=null) where T : IEvent
//     {
//         AddEventInner(new EventData()
//         {
//             type = typeof(T),
//             evt = evt,
//             target = target,
//             aliveType = EventAliveType.UntilProcessedOrTimeout,
//             destroyTimer = maxDuration
//         });
//     }
//     
//     public void QueueEventFixedDuration<T>(T evt, float duration, object target=null) where T : IEvent
//     {
//         AddEventInner(new EventData()
//         {
//             type = typeof(T),
//             evt = evt,
//             target = target,
//             aliveType = EventAliveType.FixedDuration,
//             destroyTimer = duration
//         });
//     }
//     // TODO 处理这种情况：在处理事件(Trigger或Process)时又添加了新的事件
//     private void AddEventInner(EventData eventData)
//     {
//         if (eventData.aliveType == EventAliveType.Instant)
//         {
//             isHandlingEvent = true;
//             if (eventListeners.TryGetValue(eventData.type, out var listeners))
//             {
//                 for (int i = 0; i < listeners.Count; i++)
//                 {
//                     var listener = listeners[i];
//                     if (listener.target == eventData.target)
//                     {
//                         listener.handler?.DynamicInvoke(eventData.evt);
//                     }
//                 }
//             }
//             RemoveAllDeferredListeners();
//             isHandlingEvent = false;
//         }
//         else
//         {
//             eventQueue.Add(eventData);
//         }
//     }
//
//     public void ProcessQueueEvents(float deltaTime)
//     {
//         List<int> destroyIndices = new();
//         for (var i = 0; i < eventQueue.Count; i++)
//         {
//             bool processed = false;
//             var type = eventQueue[i].type;
//             if (eventListeners.TryGetValue(type, out var listeners))
//             {
//                 foreach (var listener in listeners)
//                 {
//                     if (listener.target == eventQueue[i].target)
//                     {
//                         processed = true;
//                         listener.handler?.DynamicInvoke(eventQueue[i].evt);
//                     }
//                 }
//             }
//             bool timeout = false;
//             switch (eventQueue[i].aliveType)
//             {
//                 case EventAliveType.UntilProcessed:
//                     if (processed)
//                     {
//                         destroyIndices.Add(i);
//                     }
//                     break;
//                 case EventAliveType.UntilProcessedOrTimeout:
//                     eventQueue[i].TimeDec(deltaTime);
//                     timeout = eventQueue[i].destroyTimer <= 0;
//                     if (processed || timeout)
//                     {
//                         destroyIndices.Add(i);
//                     }
//                     break;
//                 case EventAliveType.FixedDuration:
//                     eventQueue[i].TimeDec(deltaTime);
//                     timeout = eventQueue[i].destroyTimer <= 0;
//                     if (timeout)
//                     {
//                         destroyIndices.Add(i);
//                     }
//                     break;
//                 default:
//                     break;
//             }
//         }
//         for (int i = destroyIndices.Count - 1; i >= 0; i--)
//         {
//             eventQueue.RemoveAt(i);
//         }
//     }
//     
// }