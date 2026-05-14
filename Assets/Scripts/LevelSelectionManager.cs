using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image[] dropSlots;
    [SerializeField] private Button startBattleButton;

    // AQUI ESTÁ O SEGREDO: A variável que vai guardar o seu script da cena!
    [SerializeField] private BattleStarter battleStarter;

    private LevelData currentSelectedLevel;

    void Start()
    {
        startBattleButton.interactable = false;
        ClearDropSlots();
    }

    public void SelectLevel(LevelData levelClicked)
    {
        currentSelectedLevel = levelClicked;

        UpdateDropSlots(levelClicked);
        startBattleButton.interactable = true;
    }

    private void UpdateDropSlots(LevelData level)
    {
        ClearDropSlots();

        for (int i = 0; i < level.possibleDrops.Count && i < dropSlots.Length; i++)
        {
            dropSlots[i].gameObject.SetActive(true);
            dropSlots[i].sprite = level.possibleDrops[i].Icon;
        }
    }

    private void ClearDropSlots()
    {
        foreach (Image slot in dropSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void OnStartBattleClicked()
    {
        // Adicionei uma trava de segurança pra te avisar caso esqueça de arrastar no Inspector!
        if (currentSelectedLevel != null && battleStarter != null)
        {
            Debug.Log($"Iniciando batalha do {currentSelectedLevel.levelName}! Multiplicador: {currentSelectedLevel.difficultyMultiplier}x");

            // Agora sim, chamamos pela VARIÁVEL e não pela classe genérica
            battleStarter.StartLevelBattle(currentSelectedLevel);
        }
        else if (battleStarter == null)
        {
            Debug.LogError("<color=red>Faltou arrastar o objeto BattleStarter lá no Inspector do LevelSelectionManager!</color>");
        }
    }
}