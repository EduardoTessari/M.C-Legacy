using UnityEngine;
using System.Collections.Generic;

public class PlayersSlotManager : MonoBehaviour
{
    [SerializeField] private Transform[] _playerSlots, _enemySlots;

    // Removi a variável do _playerPrefab, não precisamos mais clonar ele!

    [SerializeField] private BattleManager _battleManager;

    public void SpawnCombatentes(int qtdPlayers, LevelData levelInfo)
    {
        _battleManager.combatentes.Clear();

        // 1. Move o Player REAL (que já tem seus status reais) para o slot de batalha
        // Pega todos os players da cena (geralmente só você) e bota na posição correta
        GameObject[] playersNaCena = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playersNaCena.Length; i++)
        {
            if (i < _playerSlots.Length)
            {
                playersNaCena[i].transform.position = _playerSlots[i].position;
            }
        }

        // 2. Spawn dos Inimigos baseados na Lista do LevelData!
        int slotIndex = 0;

        foreach (EnemySpawnInfo enemyGroup in levelInfo.enemiesToSpawn)
        {
            for (int i = 0; i < enemyGroup.count; i++)
            {
                if (slotIndex < _enemySlots.Length)
                {
                    GameObject e = Instantiate(enemyGroup.enemyPrefab, _enemySlots[slotIndex].position, Quaternion.identity);

                    CharacterStats enemyStats = e.GetComponent<CharacterStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.ApplyDifficultyMultiplier(levelInfo.difficultyMultiplier);
                    }

                    slotIndex++;
                }
                else
                {
                    Debug.LogWarning("Tem mais inimigos do que slots no mapa!");
                }
            }
        }
    }
}