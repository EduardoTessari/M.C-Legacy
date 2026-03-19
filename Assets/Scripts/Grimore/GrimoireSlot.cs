using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GrimoireSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    private SpellBase _mySpell;
    private bool _isEquippedSlot; // Agora ele sabe onde ele mora!

    // Atualizamos para 3 argumentos (o último tem um valor padrão = false)
    public void SetupSlot(SpellBase spell, bool isUnlocked, bool isEquippedSlot = false)
    {
        _mySpell = spell;
        _isEquippedSlot = isEquippedSlot;

        // Se o slot for um dos 5 de cima e estiver vazio
        if (spell == null)
        {
            _iconImage.gameObject.SetActive(false); // Esconde o ícone
            return;
        }

        // Se tiver magia, mostra o ícone e configura a cor
        _iconImage.gameObject.SetActive(true);
        _iconImage.sprite = spell.Icon;

        if (isUnlocked)
        {
            _iconImage.color = Color.white;
        }
        else
        {
            // Visual de bloqueado (cinza)
            _iconImage.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        }
    }

    // --- Lógica do Tooltip ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_mySpell != null) SpellTooltip.instance.ShowTooltip(_mySpell);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SpellTooltip.instance != null) SpellTooltip.instance.HideTooltip();
    }

    // --- Lógica de Clique (Para o Passo 2 e 5) ---
    public void OnClickSlot()
    {
        if (_mySpell == null) return;

        if (_isEquippedSlot)
        {
            // Se eu sou um slot de cima, ao clicar eu me removo
            GrimoireManager.instance.UnequipSpell(_mySpell);
        }
        else
        {
            // Se eu sou da coleção, ao clicar eu tento equipar
            if (GrimoireManager.instance.IsSpellUnlocked(_mySpell))
            {
                GrimoireManager.instance.EquipSpell(_mySpell);
            }
        }

        // Depois de mudar os dados, manda a UI se redesenhar
        FindObjectOfType<GrimoireUIController>().PopulateGrimoire();
    }
}