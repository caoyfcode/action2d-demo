using UnityEngine;

// TODO 在 Unity Script Execution Order 中将这个放在 Mono Entity 之前
public class MonoEntityUpdater : MonoBehaviour
{
    private static MonoEntityUpdater Ins;

    void Awake()
    {
        if (Ins != null)
        {
            Destroy(gameObject);
            return;
        }
        EntityManager.Ins.Init();
    }

    void Update()
    {
        EntityManager.Ins.UpdateAll();
    }

    void FixedUpdate()
    {
        EntityManager.Ins.FixedUpdateAll();
    }

    void LateUpdate()
    {
        EntityManager.Ins.LateUpdateAll();
    }
}