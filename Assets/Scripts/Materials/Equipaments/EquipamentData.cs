using UnityEngine;

// 1. Definimos os "Slots" do corpo (Você pode adicionar mais depois)
public enum EquipmentType
{
    Weapon,
    Armor,
    Helmet,
    Boots
}

public enum EquipamentSet
{
    None,
    FuriaSanguinea, //Bonus de Ataque
    DefesaEterna, // Bonus de Defesa
    BençãoDivina, // Bonus de Vida
    PassosVelozes // Bonus de Velocidade
}

// 2. A Herança: Como ele herda de ItemData, ele já tem id, nome, descrição e ícone!
[CreateAssetMenu(fileName = "NewEquipment", menuName = "Collectable/Equipment")]
public class EquipmentData : ItemData
{
    [Header("Configuração de Equipamento")]
    public EquipmentType equipType;
    public EquipamentSet equipSet;

    [Header("Atributos Bônus")]
    public int bonusAttack;
    public int bonusHealth;
    public int bonusSpeed;
    public int bonusDefense;
}