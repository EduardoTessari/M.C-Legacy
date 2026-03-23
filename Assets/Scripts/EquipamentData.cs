using UnityEngine;

// 1. Definimos os "Slots" do corpo (Você pode adicionar mais depois)
public enum EquipmentType
{
    Weapon,
    Armor,
    Helmet,
    Boots
}

// 2. A Herança: Como ele herda de ItemData, ele já tem id, nome, descrição e ícone!
[CreateAssetMenu(fileName = "NewEquipment", menuName = "Collectable/Equipment")]
public class EquipmentData : ItemData
{
    [Header("Configuração de Equipamento")]
    public EquipmentType equipType;

    [Header("Atributos Bônus")]
    public int bonusAttack;
    public int bonusHealth;
    public int bonusSpeed;
}