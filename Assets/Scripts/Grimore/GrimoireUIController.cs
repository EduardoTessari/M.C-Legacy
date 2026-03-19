using System.Collections.Generic;
using UnityEngine;

public class GrimoireUIController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject _slotPrefab;

    [Header("Grids")]
    [SerializeField] private Transform _collectionGrid; // O grid de todas as magias
    [SerializeField] private Transform _equippedGrid;  // O novo grid de 5 slots

    private void OnEnable()
    {
        PopulateGrimoire();
    }

    public void PopulateGrimoire()
    {
        // Limpa os dois grids
        foreach (Transform child in _collectionGrid) Destroy(child.gameObject);
        foreach (Transform child in _equippedGrid) Destroy(child.gameObject);

        // 1. Popula a Coleção (Tudo o que existe no Manager)
        foreach (SpellBase spell in GrimoireManager.instance.GetAllSpells())
        {
            GameObject slot = Instantiate(_slotPrefab, _collectionGrid);
            bool isUnlocked = GrimoireManager.instance.IsSpellUnlocked(spell);
            slot.GetComponent<GrimoireSlot>().SetupSlot(spell, isUnlocked, false);
        }

        // 2. Popula os 5 Slots de Equipamento
        var equipped = GrimoireManager.instance.GetEquippedSkills();
        for (int i = 0; i < 5; i++)
        {
            GameObject slot = Instantiate(_slotPrefab, _equippedGrid);
            // Se houver uma magia equipada nessa posição, passa ela. Se não, passa null.
            SpellBase spell = (i < equipped.Count) ? equipped[i] : null;
            slot.GetComponent<GrimoireSlot>().SetupSlot(spell, true, true);
        }
    }
}