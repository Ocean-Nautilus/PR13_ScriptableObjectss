using System.Text;
using UnityEngine;

public enum ItemCategory
{
    Weapon,
    Armor,
    Potion,
    Misc
}

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

/// <summary>
/// ScriptableObject с данными одного предмета.
/// Создание ассета: Create → PR13 → Item Data
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "PR13/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [Header("Основное")]
    public string itemName = "Новый предмет";
    [TextArea(2, 4)]
    public string description = "Описание предмета";

    [Header("Характеристики")]
    [Min(0)] public int damage;
    [Min(0)] public int defense;
    [Min(0f)] public float weight = 1f;
    [Min(0)] public int price = 10;

    [Header("Классификация")]
    public ItemCategory category = ItemCategory.Misc;
    public ItemRarity rarity = ItemRarity.Common;

    [Header("Визуал")]
    public Color itemColor = Color.white;

    public string GetCategoryName() => category switch
    {
        ItemCategory.Weapon => "Оружие",
        ItemCategory.Armor  => "Броня",
        ItemCategory.Potion => "Зелье",
        _                   => "Прочее"
    };

    public string GetRarityName() => rarity switch
    {
        ItemRarity.Common    => "Обычный",
        ItemRarity.Rare      => "Редкий",
        ItemRarity.Epic      => "Эпический",
        ItemRarity.Legendary => "Легендарный",
        _                    => "—"
    };

    public Color GetRarityColor() => rarity switch
    {
        ItemRarity.Common    => new Color(0.8f, 0.8f, 0.8f),
        ItemRarity.Rare      => new Color(0.3f, 0.6f, 1f),
        ItemRarity.Epic      => new Color(0.7f, 0.3f, 0.9f),
        ItemRarity.Legendary => new Color(1f, 0.75f, 0.1f),
        _                    => Color.white
    };

    /// <summary>Описание предмета для TextMeshPro (rich text).</summary>
    public string GetInfo()
    {
        string rarityHex = ColorUtility.ToHtmlStringRGB(GetRarityColor());

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<b>Название:</b> {itemName}");
        sb.AppendLine($"<b>Описание:</b> {description}");
        sb.AppendLine($"<b>Категория:</b> {GetCategoryName()}");
        sb.AppendLine($"<b>Редкость:</b> <color=#{rarityHex}>{GetRarityName()}</color>");
        if (damage > 0)  sb.AppendLine($"<b>Урон:</b> {damage}");
        if (defense > 0) sb.AppendLine($"<b>Защита:</b> {defense}");
        sb.AppendLine($"<b>Вес:</b> {weight:F1} кг");
        sb.Append($"<b>Цена:</b> {price}");
        return sb.ToString();
    }
}
