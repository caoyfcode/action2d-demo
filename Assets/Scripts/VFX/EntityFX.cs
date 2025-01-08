using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    public SpriteRenderer flashedSprite;
    private Material originalMaterial;
    public float flashDuration = 0.2f;
    private bool flashStarted = false;
    public float bloodFXOffet = 0.1f;

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
        flashedSprite.material = PreloadResource.instance.whiteMaterial;
        yield return new WaitForSeconds(flashDuration);
        flashedSprite.material = originalMaterial;
        flashStarted = false;
    }

    public void StartBloodFX(Vector2 position, Vector2 hitDirection)
    {
        var obj = Instantiate(PreloadResource.instance.bloodEffectPrefab);
        if (hitDirection.x < 0)
        {
            obj.transform.Rotate(0f, 180f, 0f);
            position.x -= bloodFXOffet;
        }
        else
        {
            position.x += bloodFXOffet;
        }
        obj.transform.position = position;
    }

    public void StartDeathFX(Vector2 position)
    {
        var obj = Instantiate(PreloadResource.instance.explosionPrefab);
        obj.transform.position = position;
    }

    public void GenDarkSprite()
    {
        var obj = new GameObject();
        obj.transform.parent = transform.parent;
        obj.transform.SetPositionAndRotation(transform.position, transform.rotation);
        obj.transform.localScale = transform.localScale;
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = flashedSprite.sprite;
        sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        sr.sortingLayerID = flashedSprite.sortingLayerID;
        sr.sortingOrder = flashedSprite.sortingOrder;
    }
}