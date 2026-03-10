using UnityEngine;
using UnityEngine.UI;

public class GrimoireSlot : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    private SpellBase _mySpell;

    public void SetupSlot(SpellBase spell, bool isUnlocked)
    {
        _mySpell = spell;
        _iconImage.sprite = spell.Icon;

        if (isUnlocked)
        {
            _iconImage.color = Color.white; // Colorido
            // GetComponent<Button>().interactable = true; // Se tiver botão, ativa
        }
        else
        {
            // Fica cinza e meio transparente (Estilo "Cadeado")
            _iconImage.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
            // GetComponent<Button>().interactable = false; // Se tiver botão, desativa
        }
    }
}