using System.Collections.Generic; //Necessário para usar List<T>.
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<SpellBase> _spells;
    List<GameObject> players = new List<GameObject>();

    public SpellBase ChooseAction()
    {
        if (_spells == null || _spells.Count == 0)
        {
            players.Clear();
            foreach (GameObject c in BattleManager.instance.combatentes)
            {
                if (c != null && c.CompareTag("Player"))
                    players.Add(c);
            }

            if (players.Count > 0)
            {
                int rand = Random.Range(0, players.Count);
                players[rand].GetComponent<StatusBatalha>().ReceberDano(35);

                // AQUI ESTÁ O TRUQUE:
                return null; // "Manager, eu já fiz o ataque básico, não escolhi magia nenhuma!"
            }
        }

        // Se ele chegar aqui, é porque tem magias na lista
        int randomIndex = Random.Range(0, _spells.Count);
        return _spells[randomIndex];
    }
}
