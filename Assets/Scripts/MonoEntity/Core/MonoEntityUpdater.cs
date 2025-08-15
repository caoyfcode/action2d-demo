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
        EntityManager.Instance.Init();
    }

    void Update()
    {
        EntityManager.Instance.UpdateAll();
    }

    void FixedUpdate()
    {
        EntityManager.Instance.FixedUpdateAll();
    }

    void LateUpdate()
    {
        EntityManager.Instance.LateUpdateAll();
    }
}