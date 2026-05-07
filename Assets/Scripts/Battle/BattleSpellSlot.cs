using UnityEngine;
using UnityEngine.EventSystems; // FUNDAMENTAL para detectar o mouse
using UnityEngine.UI;

public class BattleSpellSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    private SpellBase _mySpell;

    public void Setup(SpellBase spell)
    {
        _mySpell = spell;
        _iconImage.sprite = spell.Icon;
    }

    public void OnClick()
    {
        BattleManager.instance.SelectSpell(_mySpell);
    }

    // --- TOOLTIP AQUI ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_mySpell != null)
        {
            // Criamos a string de forma dinâmica aqui
            string header = $"<b>{_mySpell.SpellName}</b>";
            string level = _mySpell.Level.ToString();
            string content = _mySpell.Description; // Ou qualquer info extra

            // Se for magia de status, adicionamos a info extra
            if (_mySpell.IsStatModifier)
            {
                string cor = _mySpell.DefenseChange > 0 ? "green" : "red";
                content += $"\n<color={cor}>Defesa: {_mySpell.DefenseChange}</color>";
            }

            // Chamamos o Manager único do jogo
            TooltipManager.instance.Show($"{header}\n<size=80%>{content}</size>");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {// Só desliga o manager
        if (TooltipManager.instance != null)
            TooltipManager.instance.Hide();
    }
}