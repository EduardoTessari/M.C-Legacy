using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 10)]
    public string content; // O que vai aparecer no balão

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.instance.Show(content);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.Hide();
    }
}