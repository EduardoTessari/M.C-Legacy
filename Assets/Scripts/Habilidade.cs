using UnityEngine;

[System.Serializable] // Isso faz aparecer no Inspector da Unity!
public class Habilidade
{
    public string nomeMagia;
    public float poder; // Dano ou Cura
    public bool ehCura;
    public int limiteAlvos; // 1, 2, 3, ou 10
    public string tagAlvo; // "Enemy" para ataque, "Player" para cura
}
