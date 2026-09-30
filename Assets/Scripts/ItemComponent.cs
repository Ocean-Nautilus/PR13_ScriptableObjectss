using UnityEngine;

/// <summary>
/// Вешается на созданный в сцене предмет и хранит его ItemData.
/// </summary>
public class ItemComponent : MonoBehaviour
{
    public ItemData data;
    public GameConfig config;

    [Header("Анимация")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;
    private float timeOffset;

    public void Initialize(ItemData itemData, GameConfig gameConfig)
    {
        data = itemData;
        config = gameConfig;
        startPosition = transform.position;
        timeOffset = UnityEngine.Random.value * Mathf.PI * 2f;
    }

    /// <summary>Цена продажи с учётом множителя из конфигурации.</summary>
    public int GetSellPrice()
    {
        if (data == null) return 0;
        float multiplier = config != null ? config.sellPriceMultiplier : 1f;
        return Mathf.RoundToInt(data.price * multiplier);
    }

    private void Update()
    {
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(Time.time * floatSpeed + timeOffset) * floatAmplitude;
        transform.position = pos;
    }
}
