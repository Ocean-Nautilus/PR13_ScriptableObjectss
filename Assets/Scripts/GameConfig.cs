using System;
using UnityEngine;

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard,
    Nightmare
}

/// <summary>
/// ScriptableObject с глобальными настройками игры.
/// Создание ассета: Create → PR13 → Game Config
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "PR13/Game Config", order = 2)]
public class GameConfig : ScriptableObject
{
    [Header("Игрок")]
    [Min(1)] public int playerHealth = 100;
    [Min(0)] public int playerMana = 50;
    [Min(0f)] public float playerSpeed = 5f;

    [Header("Физика")]
    public float gravity = 9.81f;

    [Header("Сложность")]
    public GameDifficulty difficulty = GameDifficulty.Normal;
    [Min(0f)] public float enemyDamageMultiplier = 1f;
    [Range(0f, 1f)] public float lootDropChance = 0.5f;

    [Header("Экономика")]
    [Min(0)] public int startingGold = 500;
    [Range(0f, 1f)] public float sellPriceMultiplier = 0.5f;

    /// <summary>Следующая сложность по кругу: Easy → Normal → Hard → Nightmare → Easy.</summary>
    public void NextDifficulty()
    {
        int count = Enum.GetNames(typeof(GameDifficulty)).Length;
        SetDifficulty((GameDifficulty)(((int)difficulty + 1) % count));
    }

    /// <summary>Устанавливает сложность и связанные с ней множители.</summary>
    public void SetDifficulty(GameDifficulty newDifficulty)
    {
        difficulty = newDifficulty;

        switch (difficulty)
        {
            case GameDifficulty.Easy:
                enemyDamageMultiplier = 0.5f;
                lootDropChance = 0.75f;
                break;
            case GameDifficulty.Normal:
                enemyDamageMultiplier = 1f;
                lootDropChance = 0.5f;
                break;
            case GameDifficulty.Hard:
                enemyDamageMultiplier = 1.5f;
                lootDropChance = 0.3f;
                break;
            case GameDifficulty.Nightmare:
                enemyDamageMultiplier = 2.5f;
                lootDropChance = 0.15f;
                break;
        }
    }

    public string GetDifficultyName() => difficulty switch
    {
        GameDifficulty.Easy      => "Лёгкая",
        GameDifficulty.Normal    => "Нормальная",
        GameDifficulty.Hard      => "Сложная",
        GameDifficulty.Nightmare => "Кошмар",
        _                        => "—"
    };

    /// <summary>Короткая строка для экрана.</summary>
    public string GetShortInfo()
    {
        return $"Конфигурация: сложность — {GetDifficultyName()} | " +
               $"урон врагов x{enemyDamageMultiplier:0.##} | " +
               $"шанс лута {lootDropChance * 100f:0}%";
    }

    /// <summary>Полный текст конфигурации (для консоли).</summary>
    public string GetFullInfo()
    {
        return "=== Конфигурация игры ===\n" +
               $"Здоровье игрока: {playerHealth}\n" +
               $"Мана игрока: {playerMana}\n" +
               $"Скорость игрока: {playerSpeed}\n" +
               $"Гравитация: {gravity}\n" +
               $"Сложность: {GetDifficultyName()} ({difficulty})\n" +
               $"Множитель урона врагов: x{enemyDamageMultiplier:0.##}\n" +
               $"Шанс выпадения лута: {lootDropChance * 100f:0}%\n" +
               $"Стартовое золото: {startingGold}\n" +
               $"Множитель цены продажи: x{sellPriceMultiplier:0.##}";
    }
}
