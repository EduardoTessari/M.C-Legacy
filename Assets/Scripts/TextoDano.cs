using UnityEngine;
using TMPro; // Importante para o TextMeshPro

public class TextoDano : MonoBehaviour
{
    public float velocidadeSubida = 2f;
    public float tempoVida = 1f;

    void Start()
    {
        // Destrói o número depois de 1 segundo para não encher a memória
        Destroy(gameObject, tempoVida);
    }

    void Update()
    {
        // Faz o número subir
        transform.position += Vector3.up * velocidadeSubida * Time.deltaTime;

    }

    public void Configurar(string texto)
    {
        GetComponent<TextMeshProUGUI>().text = texto;
    }

    public void PintarTexto(Color novaCor)
    {
        // Se você usa TextMeshPro (recomendado):
        GetComponent<TextMeshProUGUI>().color = novaCor;

        // OBS: Se você usar UI Text normal, seria GetComponent<Text>().color = novaCor;
    }
}