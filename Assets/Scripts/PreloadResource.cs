using System;
using UnityEngine;

[Serializable]
public class PreloadResource : SingletonBase<PreloadResource>
{

    public GameObject smokeRingPrefab;
    public GameObject afterImagePrefab;
    public Material whiteMaterial;
    public GameObject bloodEffectPrefab;
    public GameObject explosionPrefab;
}