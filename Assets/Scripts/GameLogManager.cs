using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Precisamos disso para o ScrollRect
using TMPro;

public class GameLogManager : MonoBehaviour
{
    public static GameLogManager Instance { get; private set; }

    [Header("Log Settings")]
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private int maxLines = 50; // Aumentei para 50 para dar uso ao scroll!

    [Header("Scroll References")]
    [SerializeField] private ScrollRect chatScrollRect; // Arraste o seu ChatScrollView para cá

    private Queue<TextMeshProUGUI> logQueue = new Queue<TextMeshProUGUI>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddLogMessage(string message)
    {
        TextMeshProUGUI textComponent;

        if (logQueue.Count < maxLines)
        {
            GameObject newLog = Instantiate(textPrefab, contentPanel);
            textComponent = newLog.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            textComponent = logQueue.Dequeue();
            textComponent.transform.SetAsLastSibling();
        }

        textComponent.text = message;
        logQueue.Enqueue(textComponent);

        // Força a barra de rolagem a ir para o fundo após criar a mensagem
        Canvas.ForceUpdateCanvases(); // Garante que a Unity atualizou o tamanho do Content primeiro
        chatScrollRect.verticalNormalizedPosition = 0f; // 0 é o fundo, 1 é o topo
    }
}