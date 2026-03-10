using System.Collections.Generic;
using UnityEngine;

public class GrimoireUIController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _gridParent;

    private void OnEnable()
    {
        PopulateGrimoire();
    }

    public void PopulateGrimoire()
    {
        // 1. Limpa o grid
        foreach (Transform child in _gridParent) Destroy(child.gameObject);

        // 2. Pega TODAS as magias existentes (via método público agora!)
        List<SpellBase> allSpells = GrimoireManager.instance.GetAllSpells();

        // 3. Cria os slots
        foreach (SpellBase spell in allSpells)
        {
            GameObject newSlot = Instantiate(_slotPrefab, _gridParent);
            GrimoireSlot slotScript = newSlot.GetComponent<GrimoireSlot>();

            if (slotScript != null)
            {
                // Pergunta ao manager se esta magia está liberada para o Edu
                bool isUnlocked = GrimoireManager.instance.IsSpellUnlocked(spell);

                // Passa a magia e o estado (liberada ou não) para o slot se virar
                slotScript.SetupSlot(spell, isUnlocked);
            }
        }
    }
}