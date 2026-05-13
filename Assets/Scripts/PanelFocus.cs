using UnityEngine;
using UnityEngine.EventSystems;

public class PanelFocus : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // SetAsLastSibling move o objeto para o fim da hierarquia do pai,
        // o que na UI da Unity significa ser desenhado por último (na frente).
        transform.SetAsLastSibling();
    }
}