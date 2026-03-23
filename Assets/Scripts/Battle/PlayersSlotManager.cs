using UnityEngine;
using System.Collections.Generic;

public class PlayersSlotManager : MonoBehaviour
{
    [SerializeField] private Transform[] _playerSlots, _enemySlots;
    [SerializeField] private GameObject _playerPrefab, _enemyPrefab;

    // Referência para o BattleManager para podermos preencher a lista de combatentes
    [SerializeField] private BattleManager _battleManager;

    public void SpawnCombatentes(int qtdPlayers, int qtdInimigos)
    {
        // Limpa a lista antes de começar, para garantir que não tem lixo de batalhas passadas
        _battleManager.combatentes.Clear();

        // Spawn dos Players
        for (int i = 0; i < qtdPlayers; i++)
        {
            if (i < _playerSlots.Length)
            {
                GameObject p = Instantiate(_playerPrefab, _playerSlots[i].position, Quaternion.identity);
                _battleManager.combatentes.Add(p);
            }
        }

        // Spawn dos Inimigos
        for (int i = 0; i < qtdInimigos; i++)
        {
            if (i < _enemySlots.Length)
            {
                GameObject e = Instantiate(_enemyPrefab, _enemySlots[i].position, Quaternion.identity);
                _battleManager.combatentes.Add(e);
            }
        }
    }
}