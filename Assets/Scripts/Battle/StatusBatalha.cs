using UnityEngine;
using UnityEngine.UI;

public class StatusBatalha : MonoBehaviour
{
    public string nomeEntidade;
    public float hpAtual = 100;
    public float hpMaximo = 100;

    public GameObject prefabTextoDano;
    public Transform pontoDeSpawn;

    // --- NOVAS VARIÁVEIS PARA A COR ---
    private SpriteRenderer sr;
    private Color corOriginal;
    private bool _corFoiSalva = false; // Nossa trava de segurança

    [Header("UI")]
    public Image barraHPVerde; // Arraste a imagem que tem o "Filled" aqui

    void Awake()
    {
        // Tentamos salvar a cor logo no nascimento (Awake é o primeiro de tudo)
        SalvarCorOriginal();
    }

    void Start()
    {
        // Garantimos que a barra de HP comece certa
        if (barraHPVerde != null)
        {
            barraHPVerde.fillAmount = hpAtual / hpMaximo;
        }
    }

    // --- LÓGICA DE SELEÇÃO (MOUSE) ---

    private void SalvarCorOriginal()
    {
        if (_corFoiSalva) return; // Se já salvamos, não faz nada

        if (sr == null) sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f; // Garante que não é transparente
            sr.color = c;
            corOriginal = c; // Salva a cor REAL do prefab
            _corFoiSalva = true;
        }
    }

    public void DefinirDestaque(bool status, Color corDestaque)
    {
        // Caso alguém chame esse método antes do Awake/Start rodar:
        SalvarCorOriginal();

        sr.color = status ? corDestaque : corOriginal;
    }
    private void OnMouseEnter()
    {
        // Só brilha em Ciano se for a vez do player escolher alvo
        if (BattleManager.instance.minhaVez && BattleManager.instance.selecionandoAlvo)
        {
            sr.color = Color.cyan;
        }
    }

    private void OnMouseExit()
    {
        // SÓ volta ao normal se eu não for o cara do turno!
        if (BattleManager.instance.turnoAtualPersonagem != gameObject)
        {
            sr.color = corOriginal;
        }
        else
        {
            // Se eu ainda sou o dono do turno, fico amarelo!
            sr.color = Color.yellow;
        }
    }


    public void ReceberDano(float dano)
    {
        hpAtual -= dano;

        // Configura a barra no início
        if (barraHPVerde != null)
        {
            barraHPVerde.fillAmount = hpAtual / hpMaximo;
        }

        GameObject textoGO = Instantiate(prefabTextoDano, pontoDeSpawn.position, Quaternion.identity, pontoDeSpawn);
        textoGO.GetComponent<TextoDano>().Configurar(dano.ToString());

        if (hpAtual <= 0)
        {
            Debug.Log(nomeEntidade + " morreu!");
            Destroy(gameObject, 0.5f);
        }

        BattleManager.instance.VerificarFimDeBatalha(); // Verifica se a batalha acabou após o dano
    }

    public void ReceberCura(float quantidade)
    {
        hpAtual += quantidade;

        GameObject textoGO = Instantiate(prefabTextoDano, pontoDeSpawn.position, Quaternion.identity, pontoDeSpawn);
        // Pega o script do texto
        TextoDano scriptTexto = textoGO.GetComponent<TextoDano>();

        // Configura o texto com um "+" na frente e pinta de verde!
        scriptTexto.Configurar("+" + quantidade.ToString());
        scriptTexto.PintarTexto(Color.green);

        // Garante que a vida não ultrapassa o limite máximo
        if (hpAtual > hpMaximo)
        {
            hpAtual = hpMaximo;
        }

        // Atualiza a barra visual verde
        if (barraHPVerde != null)
        {
            barraHPVerde.fillAmount = hpAtual / hpMaximo;
        }

        // Opcional: Futuramente podemos colocar um texto verde a subir aqui!
        Debug.Log(nomeEntidade + " recuperou " + quantidade + " de HP!");
    }
}