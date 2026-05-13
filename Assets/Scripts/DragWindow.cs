using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform _panelToMove;
    private Canvas _canvas;

    void Awake()
    {
        if (_panelToMove == null) _panelToMove = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _panelToMove.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _panelToMove.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
}