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

    [Header("UI")]
    public Image barraHPVerde; // Arraste a imagem que tem o "Filled" aqui

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        corOriginal = sr.color; // Salva a cor que você definiu na Unity

        if (barraHPVerde != null)
        {
            barraHPVerde.fillAmount = hpAtual / hpMaximo;
        }
    }

    // --- LÓGICA DE SELEÇÃO (MOUSE) ---
    private void OnMouseEnter()
    {
        // Só brilha se for a vez do player (acessando o seu BattleManager)
        // Se o seu script de batalha se chamar outro nome, ajuste o "BattleManager"
        if (BattleManager.instance != null && BattleManager.instance.minhaVez)
        {
            sr.color = Color.cyan; // Cor de "estou te escolhendo"
        }
    }

    private void OnMouseExit()
    {
        // Se não for o turno deste personagem, volta pra cor original
        // Se for o turno dele, a gente mantém a cor de destaque (ex: Amarelo)
        if (BattleManager.instance.turnoAtualPersonagem != gameObject)
        {
            sr.color = corOriginal;
        }
    }

    // --- MÉTODO PARA MUDAR A COR VIA CÓDIGO (TURNO) ---
    public void DefinirDestaque(bool status, Color corDestaque)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>(); // Garantia extra
        sr.color = status ? corDestaque : corOriginal;
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
            gameObject.SetActive(false);
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