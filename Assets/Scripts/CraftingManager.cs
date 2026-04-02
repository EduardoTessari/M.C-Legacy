using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] CraftSlotUI[] _recipeSlot, _inventorySlot;
    [SerializeField] CraftSlotUI _itemToCraft;


    private RecipeData _currentRecipe;

    public void SelectRecipe(RecipeData recipe)
    {
        _currentRecipe = recipe;

        // Loop que passa por todos os slots visuais da receita que você criou na tela (provavelmente 3)
        for (int i = 0; i < _recipeSlot.Length; i++)
        {

            // Se o índice atual for menor que a quantidade de ingredientes da receita, tem item pra mostrar!
            if (i < recipe.Ingredients.Length)
            {
                _recipeSlot[i].gameObject.SetActive(true); // Garante que o slot vai aparecer!
                _recipeSlot[i].SetupSlot(_currentRecipe.Ingredients[i].item, _currentRecipe.Ingredients[i].amount);

            }
            else
            {
                // Se a receita tem menos ingredientes que o total de slots, a gente desliga o que sobrar!
                _recipeSlot[i].gameObject.SetActive(false);
            }
        }

        _itemToCraft.SetupSlot(_currentRecipe.ResultItem, 1);
    }

    public void UpdateInventorySlots()
    {
        // 1. Passa por todos os slots de entrada do painel (os 3 de baixo)
        for (int i = 0; i < _inventorySlot.Length; i++)
        {
            // 2. Verifica se a receita atual precisa desse slot
            if (i < _currentRecipe.Ingredients.Length)
            {
                _inventorySlot[i].gameObject.SetActive(true); // Liga o slot para garantir que ele apareça

                // Descobre qual item a receita está pedindo nessa posição
                ItemData requiredItem = _currentRecipe.Ingredients[i].item;

                int playerAmount = 0;// Variável para contar quantos desse item o jogador tem na mochila

                // 3. Vasculha a mochila do jogador!
                foreach (var slot in InventoryManager.instance.inventory)
                {
                    // Se o item da mochila for o mesmo que a receita pede...
                    if (slot.item == requiredItem)
                    {
                        playerAmount += slot.quantity; // Soma a quantidade que achou
                    }
                }

                // 4. Manda o slot visual se desenhar com o ícone do item e a quantidade que o jogador TEM
                _inventorySlot[i].SetupSlot(requiredItem, playerAmount);
            }
            else
            {
                // Se a receita tem menos ingredientes (ex: só 2), desliga o 3º slot
                _inventorySlot[i].gameObject.SetActive(false);
            }
        }
    }

    public bool CanCraft()
    {
        // Se não tem receita selecionada, obviamente não pode craftar!
        if (_currentRecipe == null) return false;

        // 1. O CanCraft faz um loop por TODOS os ingredientes da receita
        for (int i = 0; i < _currentRecipe.Ingredients.Length; i++)
        {
            RecipeIngredient requiredIngredient = _currentRecipe.Ingredients[i];
            int amountInInventory = 0;

            // 2. Vai lá na mochila e conta quanto o jogador tem DESSE ingrediente específico
            foreach (var slot in InventoryManager.instance.inventory)
            {
                if (slot.item == requiredIngredient.item)
                {
                    amountInInventory += slot.quantity;
                }
            }

            // 3. O Pulo do Gato: A hora do teste!
            // Se a quantidade na mochila for menor do que a exigida na receita, FALHOU!
            if (amountInInventory < requiredIngredient.amount)
            {
                Debug.Log($"Faltou item! Precisa de {requiredIngredient.amount} {requiredIngredient.item.ItemName}, mas só tem {amountInInventory}.");
                return false; // Corta o mal pela raiz e sai da função imediatamente
            }
        }

        // 4. Se ele passou por TODO o loop acima sem cair no "return false", 
        // significa que o jogador tem tudo em quantidade suficiente!
        return true;
    }

       public void CraftItem()
    {
        if (!CanCraft()) return;

        // 2. Consome os ingredientes da mochila usando a sua função nova
        for (int i = 0; i < _currentRecipe.Ingredients.Length; i++)
        {
            RecipeIngredient requiredIngredient = _currentRecipe.Ingredients[i];

            // Chama o RemoveItem do seu Manager para ele fazer o trabalho sujo e apagar os itens do jeito certo!
            InventoryManager.instance.RemoveItem(requiredIngredient.item, requiredIngredient.amount);
        }

        // 3. A Recompensa! Adiciona 1 unidade do item final na mochila do jogador
        InventoryManager.instance.AddItem(_currentRecipe.ResultItem, 1);

        // 4. Manda a tela do Craft atualizar pra mostrar que os materiais sumiram
        UpdateInventorySlots();
    }
}