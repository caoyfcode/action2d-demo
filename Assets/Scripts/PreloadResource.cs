using System;
using UnityEngine;

[Serializable]
public class PreloadResource : MonoSingletonBase<PreloadResource>
{

    public GameObject smokeRingPrefab;
    public GameObject afterImagePrefab;
    public Material whiteMaterial;
    public GameObject bloodEffectPrefab;
    public GameObject explosionPrefab;
    public GameObject pickupItemPrefab;
}