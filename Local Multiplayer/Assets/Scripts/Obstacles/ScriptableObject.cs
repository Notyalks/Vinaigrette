using UnityEngine;

[System.Serializable]
public class ObstacleOption
{
    public string name;
    public GameObject prefab;
    public Sprite icon;

    [Header("Configuração")]
    public bool isBomb;

    [Header("Disponibilidade por jogador")]
    public bool availableForPlayer1 = true;
    public bool availableForPlayer2 = true;
}
