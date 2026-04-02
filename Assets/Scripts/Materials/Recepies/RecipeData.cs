using UnityEngine;

// 1. A Struct que une o item e a quantidade necessária
[System.Serializable]
public struct RecipeIngredient
{
    public ItemData item;
    public int amount;
}

// 2. O Scriptable Object da Receita
[CreateAssetMenu(fileName = "NewRecipe", menuName = "Collectable/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Recipe Configuration")]

    [Tooltip("List of items and amounts needed to craft this recipe.")]
    [SerializeField] private RecipeIngredient[] ingredients;

    [Tooltip("The final item/equipment given to the player.")]
    [SerializeField] private ItemData resultItem;

    // --- Getters ---
    public RecipeIngredient[] Ingredients => ingredients;
    public ItemData ResultItem => resultItem;
}