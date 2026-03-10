using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    // --- SINGLETON ---
    public static BattleManager instance;

    [Header("New Spell System")]
    [SerializeField] private SpellBase selectedSpell; // Magia "armada" pelo botão

    [Header("Combat State")]
    public List<GameObject> combatentes = new List<GameObject>();
    public int turnoAtual = 0;
    public GameObject turnoAtualPersonagem;
    public bool minhaVez = false;
    public bool selecionandoAlvo = false;

    [Header("UI & Environment")]
    public Transform painelTimeline;
    public GameObject templateTurnoPrefab;
    public GameObject mainCanvas;
    public GameObject battleCanvas;
    public GameObject worldEnvironment;
    public GameObject combatEnvironment;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Só tenta ler o clique se for sua vez E se você já escolheu uma magia
        if (minhaVez && selecionandoAlvo && Input.GetMouseButtonDown(0))
        {
            VerificarCliqueNoAlvo();
        }
    }

    #region Turn Management

    public void IniciarBatalha()
    {
        combatentes.Clear();

        // Procura quem está no cenário de combate (Ativado pelo NPC)
        combatentes.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        combatentes.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        turnoAtual = 0;
        ProximoTurno();
    }

    public void ProximoTurno()
    {
        // Limpa personagens mortos/desativados
        combatentes.RemoveAll(item => item == null || !item.activeInHierarchy);

        if (combatentes.Count == 0) return;

        if (turnoAtual >= combatentes.Count) turnoAtual = 0;

        turnoAtualPersonagem = combatentes[turnoAtual];
        AtualizarTimelineVisual();

        if (turnoAtualPersonagem.CompareTag("Player"))
        {
            minhaVez = true;
            Debug.Log($"Your turn! Spell loaded: {(selectedSpell != null ? selectedSpell.SpellName : "None")}");
        }
        else
        {
            minhaVez = false;
            ExecutarTurnoInimigo(turnoAtualPersonagem);
        }
    }

    public void FinalizarTurno()
    {
        if (turnoAtualPersonagem != null)
        {
            StatusBatalha status = turnoAtualPersonagem.GetComponent<StatusBatalha>();
            if (status != null) status.DefinirDestaque(false, Color.white);
        }

        VerificarFimDeBatalha();
    }

    public void VerificarFimDeBatalha()
    {
        int players = 0;
        int enemies = 0;

        foreach (GameObject go in combatentes)
        {
            if (go.CompareTag("Player")) players++;
            if (go.CompareTag("Enemy")) enemies++;
        }

        if (enemies <= 0 || players <= 0)
        {
            EndBattle();
        }
        else
        {
            turnoAtual++;
            ProximoTurno();
        }
    }

    #endregion

    #region Player Interaction

    public void SelectSpellFromButton(int index)
    {
        if (!minhaVez) return;

        // Pega a lista do Grimório
        var currentSkills = GrimoireManager.instance.GetEquippedSkills();

        if (index < currentSkills.Count)
        {
            selectedSpell = currentSkills[index];

            if (selectedSpell.IsPassive)
            {
                Debug.Log($"{selectedSpell.SpellName} is Passive and cannot be cast manually.");
                selectedSpell = null;
                return;
            }

            selecionandoAlvo = true;
            Debug.Log($"Spell armed: {selectedSpell.SpellName}. Select a target!");
        }
    }

    void VerificarCliqueNoAlvo()
    {
        if (selectedSpell == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag(selectedSpell.TargetTag))
        {
            GameObject target = hit.collider.gameObject;

            // Lógica de Área vs Único
            if (selectedSpell.MaxTargets <= 1)
            {
                ApplyEffect(target);
            }
            else
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag(selectedSpell.TargetTag);
                int count = 0;
                foreach (GameObject t in targets)
                {
                    if (count >= selectedSpell.MaxTargets) break;
                    ApplyEffect(t);
                    count++;
                }
            }

            FinalizarAtaque();
        }
        else if (hit.collider != null)
        {
            Debug.Log($"Invalid Target! {selectedSpell.SpellName} needs a: {selectedSpell.TargetTag}");
        }
    }

    void ApplyEffect(GameObject target)
    {
        // 1. Visual (VFX)
        if (selectedSpell.VfxPrefab != null)
            Instantiate(selectedSpell.VfxPrefab, target.transform.position, Quaternion.identity);

        // 2. Lógica de Atributos
        StatusBatalha status = target.GetComponent<StatusBatalha>();
        if (status != null)
        {
            if (selectedSpell.IsHealing) status.ReceberCura(selectedSpell.GetCurrentPower());
            else status.ReceberDano(selectedSpell.GetCurrentPower());
        }
    }

    void FinalizarAtaque()
    {
        selecionandoAlvo = false;
        minhaVez = false;
        selectedSpell = null;
        Invoke("FinalizarTurno", 1.2f); // Delay para o VFX aparecer
    }

    #endregion

    #region Enemy AI (Simple)

    void ExecutarTurnoInimigo(GameObject inimigo)
    {
        Invoke("AtaqueInimigoSimples", 1.5f);
    }

    void AtaqueInimigoSimples()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            int rand = Random.Range(0, players.Length);
            players[rand].GetComponent<StatusBatalha>().ReceberDano(10);
            Debug.Log($"{turnoAtualPersonagem.name} attacked {players[rand].name}");
        }

        FinalizarTurno();
    }

    #endregion

    #region UI & Cleanup

    public void AtualizarTimelineVisual()
    {
        foreach (Transform child in painelTimeline) Destroy(child.gameObject);

        for (int i = 0; i < combatentes.Count; i++)
        {
            int index = (turnoAtual + i) % combatentes.Count;
            GameObject c = combatentes[index];

            GameObject item = Instantiate(templateTurnoPrefab, painelTimeline);
            var text = item.GetComponent<TextMeshProUGUI>();
            text.text = c.name;

            StatusBatalha status = c.GetComponent<StatusBatalha>();

            if (i == 0) // Atual
            {
                text.color = Color.yellow;
                if (status != null) status.DefinirDestaque(true, Color.yellow);
            }
            else
            {
                text.color = c.CompareTag("Player") ? Color.cyan : Color.red;
                if (status != null) status.DefinirDestaque(false, Color.white);
            }
        }
    }

    public void EndBattle()
    {
        Debug.Log("Battle Ended.");
        if (battleCanvas != null) battleCanvas.SetActive(false);
        if (mainCanvas != null) mainCanvas.SetActive(true);
        if (combatEnvironment != null) combatEnvironment.SetActive(false);
        if (worldEnvironment != null) worldEnvironment.SetActive(true);
    }

    #endregion
}