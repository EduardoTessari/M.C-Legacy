using UnityEngine;

public class RecipeButtonUI : MonoBehaviour
{
    [SerializeField] private RecipeData _recipeToCraft;
    [SerializeField] private CraftingManager _craftingManager; // O botão precisa saber quem é o manager!

    // Você vai atrelar essa função no OnClick() do seu botão lá no Inspector
    public void OnClickRecipeButton()
    {
        _craftingManager.SelectRecipe(_recipeToCraft);
        _craftingManager.UpdateInventorySlots(); // Atualiza os slots de baixo para mostrar se o jogador tem os ingredientes ou não
    }
}