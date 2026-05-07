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
    public BattleStarter npcThatCalled; // Guarda a referência do NPC que iniciou a batalha para poder resetá-lo depois

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

    public void IniciarBatalha()
    {
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

    // AGORA É PUBLIC E COM O NOME QUE O STATUS_BATALHA QUER!
    public bool VerificarFimDeBatalha()
    {
        int p = 0; int e = 0;
        foreach (GameObject go in combatentes)
        {
            if (go == null) continue;
            if (go.CompareTag("Player")) p++;
            if (go.CompareTag("Enemy")) e++;
        }

        if (e <= 0 || p <= 0)
        {
            EndBattle();
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

        // 1. Verificamos se clicamos em alguém válido
        if (hit.collider != null && hit.collider.CompareTag(selectedSpell.TargetTag))
        {
            List<GameObject> alvosFinais = new List<GameObject>();

            // 2. O alvo clicado SEMPRE entra primeiro
            GameObject alvoPrincipal = hit.collider.gameObject;
            alvosFinais.Add(alvoPrincipal);

            // 3. Se a magia for de área (mais de 1 alvo), buscamos os extras
            if (selectedSpell.MaxTargets > 1)
            {
                // Em vez de buscar na cena toda, olhamos só para quem está na batalha!
                foreach (GameObject combatente in BattleManager.instance.combatentes)
                {
                    // Para o loop se a lista encher
                    if (alvosFinais.Count >= selectedSpell.MaxTargets) break;

                    // Só adiciona se: não for nulo, tiver a tag certa E não for o alvo principal
                    if (combatente != null &&
                        combatente.CompareTag(selectedSpell.TargetTag) &&
                        combatente != alvoPrincipal)
                    {
                        alvosFinais.Add(combatente);
                    }
                }
            }

            // 4. Única chamada de execução para todos os casos!
            StartCoroutine(ExecutarAçãoPlayer(alvosFinais));
        }
    }

    // Agora a corrotina aceita uma LISTA de GameObjects
    IEnumerator ExecutarAçãoPlayer(List<GameObject> targets)
    {
        isBusy = true;

        foreach (GameObject t in targets)
        {
            ApplyEffect(t); // Aplica dano/VFX em cada um da lista
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
                // 1. COMEÇA COM A TAG QUE ESTÁ NO ASSET
                string tagParaBuscar = selectedSpell.TargetTag;

                // 2. LÓGICA DE ESPELHAMENTO: "O inimigo do meu inimigo é meu alvo"
                // Se o conjurador é um Inimigo, a gente inverte as intenções:
                if (turnoAtualPersonagem.CompareTag("Enemy"))
                {
                    if (selectedSpell.TargetTag == "Enemy")
                        tagParaBuscar = "Player"; // Ataques agora focam em você
                    else if (selectedSpell.TargetTag == "Player")
                        tagParaBuscar = "Enemy"; // Curas agora focam nele mesmo
                }

                List<GameObject> alvosFinais = new List<GameObject>();

                // 3. BUSCA COM A TAG "TRADUZIDA"
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
                // Opcional: Aqui é onde o Inimigo já fez o "Ataque Básico" lá na EnemyAI
                // Você pode colocar um pequeno delay aqui para o jogador ver o dano subir
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

            // CASO A: A magia é de alteração de Atributo (Pique Pokémon)
            if (selectedSpell.IsStatModifier)
            {
                if (selectedSpell.DefenseChange != 0)
                {
                    statsAlvo.AlterarDefesaTemporariamente(selectedSpell.DefenseChange);
                    // Você pode até colocar um Log específico:
                    string acao = selectedSpell.DefenseChange > 0 ? "subiu" : "caiu";
                    Debug.Log($"{target.name} teve sua defesa reduzida/aumentada!");
                }
                // Aqui você pode adicionar outros modificadores no futuro (Atk, Speed...)
                if (selectedSpell.MultiplicadorDano <= 0) return;
            }

            // CASO B: A magia é de Cura ou Dano

            if (selectedSpell.IsHealing)
            {
                // CURA: Ignora defesa. É apenas seu Ataque (ou Int) * Multiplicador
                resultadoFinal = statsConjurador.CurrentAttack * selectedSpell.MultiplicadorDano;
            }
            else
            {
                // DANO: (Ataque - Defesa) * Multiplicador
                int defesaAlvo = (statsAlvo != null) ? statsAlvo.CurrentDefense : 0;
                float poderPenetracao = statsConjurador.CurrentAttack - defesaAlvo;

                // Trava de dano mínimo antes do multiplicador (Pelo menos 1 de "base")
                if (poderPenetracao < 1) poderPenetracao = 1;

                resultadoFinal = poderPenetracao * selectedSpell.MultiplicadorDano;
            }

            // Arredonda e garante que o dano final nunca seja menor que 1 (Trava Global)
            int valorFinal = Mathf.Max(1, Mathf.RoundToInt(resultadoFinal));

            // 3. Aplica o resultado
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
        // 1. Limpa a UI lateral (Timeline)
        foreach (Transform child in painelTimeline) Destroy(child.gameObject);

        // 2. Pente fino nos combatentes no cenário
        foreach (GameObject go in combatentes)
        {
            if (go == null) continue;

            StatusBatalha status = go.GetComponent<StatusBatalha>();
            if (status != null)
            {
                // Se o objeto da lista é o personagem que vai agir AGORA -> Amarelo
                // Se NÃO é ele -> Volta para a cor original (Branco/Normal)
                bool ehOTurnoDele = (go == turnoAtualPersonagem);
                status.DefinirDestaque(ehOTurnoDele, Color.yellow);
            }
        }

        // 3. Reconstrói a Timeline Visual (os nomes na direita)
        for (int i = 0; i < combatentes.Count; i++)
        {
            int index = (turnoAtual + i) % combatentes.Count;
            GameObject c = combatentes[index];

            GameObject item = Instantiate(templateTurnoPrefab, painelTimeline);
            var text = item.GetComponent<TextMeshProUGUI>();
            text.text = c.name;

            // Na UI, quem está no topo (i=0) fica amarelo
            if (i == 0) text.color = Color.yellow;
            else text.color = c.CompareTag("Player") ? Color.cyan : Color.red;
        }
    }

    public void EndBattle()
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

                // 1. Pegamos os dois componentes do Player
                CharacterStats stats = playerNoMundo.GetComponent<CharacterStats>();
                StatusBatalha status = playerNoMundo.GetComponent<StatusBatalha>();

                if (stats != null && status != null)
                {
                    // 2. Atualiza os limites (HP Máximo baseado nos itens)
                    stats.UpdateStats();

                    // 3. CURA REAL: Resetamos o HP atual para o máximo
                    status.hpMaximo = stats.CurrentHealth;
                    status.hpAtual = status.hpMaximo;

                    // 4. Agora sim, chamamos a atualização da barra do OBJETO 'status'
                    status.DefinirDestaque(false, Color.white); // Limpa brilho de turno se houver

                    // 5. FORÇA A BARRA DE VIDA A SE RECALCULAR
                    status.AtualizarBarraUI();
                }
            }

            if (npcThatCalled != null) npcThatCalled.ResetNPC();
        }
    }

    #endregion
}