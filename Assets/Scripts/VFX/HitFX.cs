using System.Collections;
using UnityEngine;

public class HitFX : MonoBehaviour
{
    public SpriteRenderer flashedSprite;
    public Material flashMaterial;
    private Material originalMaterial;
    public float flashDuration = 0.2f;
    private bool flashStarted = false;
    public bool flashFXOnHit = true;
    public bool boodFXOnHit = true;

    public void StartFlash()
    {
        if (flashStarted)
            return;
        StartCoroutine(FlashCourotine());
    }


    IEnumerator FlashCourotine()
    {
        flashStarted = true;
        originalMaterial = flashedSprite.material;
        flashedSprite.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        flashedSprite.material = originalMaterial;
        flashStarted = false;
    }

    public void InstantiateBloodFX(Vector2 position, Vector2 hitDirection)
    {
        var obj = Instantiate(Game.VFXLoader.bloodEffectPrefab);
        obj.transform.position = position;
        if (hitDirection.x < 0)
        {
            obj.transform.Rotate(180f, 0, 0f);
        }
    }

    public void OnHit(HitInfo hitInfo)
    {
        if (flashFXOnHit)
            StartFlash();
        if (boodFXOnHit)
            InstantiateBloodFX(hitInfo.hitPoint, hitInfo.force);
    }
}