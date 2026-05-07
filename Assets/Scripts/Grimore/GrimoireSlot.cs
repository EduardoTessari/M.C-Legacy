using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

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
    {
        // Só desliga o manager
        if (TooltipManager.instance != null)
            TooltipManager.instance.Hide();
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