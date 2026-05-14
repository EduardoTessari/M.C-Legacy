using System.Collections.Generic;
using UnityEngine;

// Esse "pacotinho" diz qual é o bicho e a quantidade dele
[System.Serializable]
public class EnemySpawnInfo
{
    public string groupName; // Só pra você organizar no Inspector (ex: "Minions Base", "Elite")
    public GameObject enemyPrefab;
    public int count; // Quantos desse prefab vão nascer
}

[CreateAssetMenu(fileName = "NewLevel", menuName = "Battle/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName = "Andar 1";
    public int levelIndex; // 1 a 5
    public bool isUnlockedByDefault;

    [Header("Combat Settings")] //Faz com que eu possa configurar o que vai nascer e a quantidade que nascerá em cada andar direto pelo Inspector, sem precisar criar um prefab só pra isso
    // AQUI É A MÁGICA: Uma lista de pacotinhos em vez de um prefab só
    public List<EnemySpawnInfo> enemiesToSpawn;

    public float difficultyMultiplier = 1.0f;

    [Header("Rewards (Max 3)")] //Permite configurar as recompensas de cada andar direto pelo Inspector, sem precisar criar um prefab só pra isso
    public List<ItemData> possibleDrops;
}