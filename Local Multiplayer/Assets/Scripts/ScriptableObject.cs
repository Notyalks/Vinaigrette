using UnityEngine;

[System.Serializable]
public struct ObstacleOption
{
    public GameObject prefab;
    public Sprite icon;
    public bool isBomb; // marque true no Inspector para as bombas
}
