using System.Collections;
using UnityEngine;

public class HitFlashFX : MonoBehaviour
{
    public SpriteRenderer flashedSprite;
    public Material flashMaterial;
    private Material originalMaterial;
    public float flashDuration = 0.2f;
    private bool flashStarted = false;

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
}