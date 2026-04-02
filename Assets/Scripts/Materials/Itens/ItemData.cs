using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Collectable/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private string itemID;
    [SerializeField] private string itemName;
    [TextArea][SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isStackable;

    [SerializeField] private int maxStack = 999;

    // --- Getters ---
    public string ItemName => itemName;
    public string Description => description;
    public Sprite Icon => icon;

    public bool IsStackable => isStackable;
}
