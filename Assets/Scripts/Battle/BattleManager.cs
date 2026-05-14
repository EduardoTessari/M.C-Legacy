using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private GameObject playerNoMundo;

    [Header("New Spell System")]
    [SerializeField] private SpellBase selectedSpell;

    [Header("Combat State")]
    public List<GameObject> combatentes = new List<GameObject>();
    public int turnoAtual = 0;
    public GameObject turnoAtualPersonagem;
    public bool minhaVez = false;
    public bool selecionandoAlvo = false;
    public bool isBusy = false;

    [Header("UI & Environment")]
    public Transform painelTimeline;
    public GameObject templateTurnoPrefab;
    public GameObject mainCanvas, battleCanvas, worldEnvironment, combatEnvironment;

    // NOVO: Guarda as informações do nível atual para saber o que dropar no final!
    private LevelData currentLevelData;

    [Header("Dynamic UI")]
    [SerializeField] private GameObject _battleSlotPrefab;
    [SerializeField] private Transform _skillContainer;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (minhaVez && selecionandoAlvo && !isBusy && Input.GetMouseButtonDown(0))
        {
            VerificarCliqueNoAlvo();
        }
    }

    #region Turn Management (O Fluxo que não trava)

    // AGORA RECEBE O NÍVEL COMO PARÂMETRO
    public void IniciarBatalha(LevelData levelInfo)
    {
        currentLevelData = levelInfo; // Salva o nível na memória do Manager

        combatentes.Clear();
        combatentes.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        combatentes.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        playerNoMundo = combatentes.Find(go => go.CompareTag("Player"));

        SetupBattleUI();
        turnoAtual = 0;

        StopAllCoroutines();
        StartCoroutine(LoopDaBatalha());
    }

    IEnumerator LoopDaBatalha()
    {
        while (true)
        {
            // 1. Limpeza de quem morreu
            combatentes.RemoveAll(item => item == null || !item.activeInHierarchy);

            // 2. Verifica se a batalha acabou ANTES de começar o turno
            if (VerificarFimDeBatalha()) yield break;

            if (turnoAtual >= combatentes.Count) turnoAtual = 0;
            turnoAtualPersonagem = combatentes[turnoAtual];

            // --- LOG DE INVESTIGAÇÃO ---
            Debug.Log($"<color=cyan>--- TURNO ATUAL: {turnoAtual} | PERSONAGEM: {turnoAtualPersonagem.name} | TAG: {turnoAtualPersonagem.tag} ---</color>");

            AtualizarTimelineVisual();
            yield return new WaitForSeconds(0.5f);

            // 3. Execução: Checa a TAG para decidir quem age
            if (turnoAtualPersonagem.CompareTag("Player"))
            {
                minhaVez = true;
                selecionandoAlvo = false;
                // O código TRAVA aqui até você fazer a sua jogada
                yield return new WaitUntil(() => minhaVez == false);
            }
            else
            {
                minhaVez = false;
                yield return StartCoroutine(IA_AtaqueInimigo());
            }

            // 4. Incremento
            turnoAtual++;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public bool VerificarFimDeBatalha()
    {
        int p = 0; int e = 0;
        foreach (GameObject go in combatentes)
        {
            if (go == null) continue;
            if (go.CompareTag("Player")) p++;
            if (go.CompareTag("Enemy")) e++;
        }

        // Se acabaram os inimigos, Vitória! Se acabou o player, Derrota.
        if (e <= 0)
        {
            EndBattle(true); // Venceu!
            return true;
        }
        else if (p <= 0)
        {
            EndBattle(false); // Perdeu!
            return true;
        }

        return false;
    }

    #endregion

    #region Ações

    public void SelectSpell(SpellBase spell)
    {
        if (!minhaVez || isBusy) return;
        selectedSpell = spell;
        selecionandoAlvo = true;
    }

    public void VerificarCliqueNoAlvo()
    {
        if (selectedSpell == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag(selectedSpell.TargetTag))
        {
            List<GameObject> alvosFinais = new List<GameObject>();
            GameObject alvoPrincipal = hit.collider.gameObject;
            alvosFinais.Add(alvoPrincipal);

            if (selectedSpell.MaxTargets > 1)
            {
                foreach (GameObject combatente in BattleManager.instance.combatentes)
                {
                    if (alvosFinais.Count >= selectedSpell.MaxTargets) break;

                    if (combatente != null &&
                        combatente.CompareTag(selectedSpell.TargetTag) &&
                        combatente != alvoPrincipal)
                    {
                        alvosFinais.Add(combatente);
                    }
                }
            }

            StartCoroutine(ExecutarAçãoPlayer(alvosFinais));
        }
    }

    IEnumerator ExecutarAçãoPlayer(List<GameObject> targets)
    {
        isBusy = true;

        foreach (GameObject t in targets)
        {
            ApplyEffect(t);
        }

        yield return new WaitForSeconds(1.2f);

        isBusy = false;
        minhaVez = false;
    }

    IEnumerator IA_AtaqueInimigo()
    {
        isBusy = true;
        yield return new WaitForSeconds(1.0f);

        EnemyAI ai = turnoAtualPersonagem.GetComponent<EnemyAI>();

        if (ai != null)
        {
            selectedSpell = ai.ChooseAction();

            if (selectedSpell != null)
            {
                string tagParaBuscar = selectedSpell.TargetTag;

                if (turnoAtualPersonagem.CompareTag("Enemy"))
                {
                    if (selectedSpell.TargetTag == "Enemy")
                        tagParaBuscar = "Player";
                    else if (selectedSpell.TargetTag == "Player")
                        tagParaBuscar = "Enemy";
                }

                List<GameObject> alvosFinais = new List<GameObject>();

                foreach (GameObject combatente in combatentes)
                {
                    if (alvosFinais.Count >= selectedSpell.MaxTargets) break;

                    if (combatente != null && combatente.CompareTag(tagParaBuscar))
                    {
                        alvosFinais.Add(combatente);
                    }
                }

                yield return StartCoroutine(ExecutarAçãoPlayer(alvosFinais));
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return new WaitForSeconds(1.0f);
        isBusy = false;
    }

    void ApplyEffect(GameObject target)
    {
        if (selectedSpell.VfxPrefab != null)
            Instantiate(selectedSpell.VfxPrefab, target.transform.position, Quaternion.identity);

        CharacterStats statsConjurador = turnoAtualPersonagem.GetComponent<CharacterStats>();
        CharacterStats statsAlvo = target.GetComponent<CharacterStats>();
        StatusBatalha statusBatalhaAlvo = target.GetComponent<StatusBatalha>();

        if (statsConjurador != null && statusBatalhaAlvo != null)
        {
            float resultadoFinal = 0;

            if (selectedSpell.IsStatModifier)
            {
                if (selectedSpell.DefenseChange != 0)
                {
                    statsAlvo.AlterarDefesaTemporariamente(selectedSpell.DefenseChange);
                    string acao = selectedSpell.DefenseChange > 0 ? "subiu" : "caiu";
                    Debug.Log($"{target.name} teve sua defesa reduzida/aumentada!");
                }
                if (selectedSpell.MultiplicadorDano <= 0) return;
            }

            if (selectedSpell.IsHealing)
            {
                resultadoFinal = statsConjurador.CurrentAttack * selectedSpell.MultiplicadorDano;
            }
            else
            {
                int defesaAlvo = (statsAlvo != null) ? statsAlvo.CurrentDefense : 0;
                float poderPenetracao = statsConjurador.CurrentAttack - defesaAlvo;

                if (poderPenetracao < 1) poderPenetracao = 1;

                resultadoFinal = poderPenetracao * selectedSpell.MultiplicadorDano;
            }

            int valorFinal = Mathf.Max(1, Mathf.RoundToInt(resultadoFinal));

            if (selectedSpell.IsHealing)
                statusBatalhaAlvo.ReceberCura(valorFinal);
            else
                statusBatalhaAlvo.ReceberDano(valorFinal);

            Debug.Log($"{turnoAtualPersonagem.name} usou {selectedSpell.SpellName}. Valor Final: {valorFinal} (Defesa do Alvo: {(statsAlvo != null ? statsAlvo.CurrentDefense : 0)})");
        }
    }

    #endregion

    #region Helpers (UI)

    private void SetupBattleUI()
    {
        foreach (Transform child in _skillContainer) Destroy(child.gameObject);
        foreach (SpellBase spell in GrimoireManager.instance.GetEquippedSkills())
        {
            GameObject newSlot = Instantiate(_battleSlotPrefab, _skillContainer);
            newSlot.GetComponent<BattleSpellSlot>().Setup(spell);
        }
    }

    public void AtualizarTimelineVisual()
    {
        foreach (Transform child in painelTimeline) Destroy(child.gameObject);

        foreach (GameObject go in combatentes)
        {
            if (go == null) continue;

            StatusBatalha status = go.GetComponent<StatusBatalha>();
            if (status != null)
            {
                bool ehOTurnoDele = (go == turnoAtualPersonagem);
                status.DefinirDestaque(ehOTurnoDele, Color.yellow);
            }
        }

        for (int i = 0; i < combatentes.Count; i++)
        {
            int index = (turnoAtual + i) % combatentes.Count;
            GameObject c = combatentes[index];

            GameObject item = Instantiate(templateTurnoPrefab, painelTimeline);
            var text = item.GetComponent<TextMeshProUGUI>();
            text.text = c.name;

            if (i == 0) text.color = Color.yellow;
            else text.color = c.CompareTag("Player") ? Color.cyan : Color.red;
        }
    }

    public void EndBattle(bool vitoria)
    {
        StopAllCoroutines();

        foreach (GameObject go in combatentes)
        {
            if (go != null)
            {
                if (!go.CompareTag("Player"))
                {
                    Destroy(go);
                }
            }
        }
        combatentes.Clear();

        if (battleCanvas != null) battleCanvas.SetActive(false);
        if (mainCanvas != null) mainCanvas.SetActive(true);
        if (combatEnvironment != null) combatEnvironment.SetActive(false);

        if (worldEnvironment != null)
        {
            worldEnvironment.SetActive(true);

            if (playerNoMundo != null)
            {
                playerNoMundo.SetActive(true);

                CharacterStats stats = playerNoMundo.GetComponent<CharacterStats>();
                StatusBatalha status = playerNoMundo.GetComponent<StatusBatalha>();

                if (stats != null && status != null)
                {
                    stats.UpdateStats();

                    status.hpMaximo = stats.CurrentHealth;
                    status.hpAtual = status.hpMaximo;

                    status.DefinirDestaque(false, Color.white);

                    status.AtualizarBarraUI();
                }
            }

            // --- SISTEMA DE DROPS AQUI ---
            if (vitoria && currentLevelData != null)
            {
                Debug.Log("<color=green>VITÓRIA! Calculando Drops...</color>");

                foreach (ItemData drop in currentLevelData.possibleDrops)
                {
                    // 50% de chance de dropar cada item da lista do nível. Pode ajustar esse valor!
                    if (Random.Range(0, 100) < 50)
                    {
                        // Avisa o nosso chat visual
                        if (GameLogManager.Instance != null)
                        {
                            string timeString = System.DateTime.Now.ToString("HH:mm:ss");
                            GameLogManager.Instance.AddLogMessage($"[{timeString}]: <color=orange>Drop +1 {drop.ItemName}</color>");
                        }

                        // Adiciona no inventário de verdade! 
                        if (InventoryManager.instance != null)
                        {
                            InventoryManager.instance.AddItem(drop,1);
                        }
                    }
                }
            }
            else if (!vitoria)
            {
                Debug.Log("<color=red>DERROTA! Você foi amassado e voltou pra cidade sem nada.</color>");
            }
        }
    }

    #endregion
}