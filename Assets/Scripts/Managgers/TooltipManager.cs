using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _textElement;
    [SerializeField] private CanvasGroup _canvasGroup;

    void Awake()
    {
        // Singleton para ser acessado de qualquer lugar
        if (instance == null) instance = this;
        else Destroy(gameObject);

        Hide();
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        // 1. Descobrimos em que "porcentagem" da tela o mouse está
        // 0 = esquerda/baixo | 1 = direita/cima
        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;

        // 2. Aplicamos o pivot inversamente
        // Se o mouse está na direita (0.9), o pivot fica em 1 (desenha pra esquerda)
        // Se o mouse está no topo (0.9), o pivot fica em 1 (desenha pra baixo)
        _rectTransform.pivot = new Vector2(pivotX, pivotY);

        // 3. Cola o Tooltip no mouse (o Pivot cuida do resto!)
        transform.position = mousePos;
    }

    public void Show(string content)
    {
        _textElement.text = content;

        // ATENÇÃO AQUI:
        // Se o texto for maior que o Preferred Width do Layout Element, 
        // ele vai ativar o wrapping automaticamente.
        _canvasGroup.alpha = 1;

        // Forçamos o rebuild duas vezes para garantir que o fundo entenda a nova altura
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
    }
}