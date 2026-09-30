using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Создаёт в сцене игровые объекты по данным ItemData.
/// </summary>
public class ItemFactory : MonoBehaviour
{
    [Header("ScriptableObject ссылки")]
    public ItemDatabase itemDatabase;
    public GameConfig gameConfig;

    private readonly List<GameObject> createdItems = new List<GameObject>();
    private static Sprite squareSprite;

    public int CreatedCount => createdItems.Count;

    // Сброс статики при отключённой перезагрузке домена (Enter Play Mode Options)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => squareSprite = null;

    public GameObject CreateItem(ItemData data, Vector3 position)
    {
        if (data == null)
        {
            Debug.LogWarning("ItemFactory: data == null, предмет не создан.");
            return null;
        }

        GameObject obj = new GameObject("Item_" + data.itemName);
        obj.transform.position = position;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = GetSquareSprite();
        sr.color = data.itemColor;
        sr.sortingOrder = 100; // выше Canvas (Screen Space - Camera)

        float scale = 0.8f + (int)data.rarity * 0.2f;
        obj.transform.localScale = new Vector3(scale, scale, 1f);

        obj.AddComponent<BoxCollider2D>();

        ItemComponent comp = obj.AddComponent<ItemComponent>();
        comp.Initialize(data, gameConfig);

        createdItems.Add(obj);

        Debug.Log($"Создан предмет: {data.itemName} ({data.GetRarityName()}), " +
                  $"цена {data.price}, цена продажи {comp.GetSellPrice()}");
        return obj;
    }

    public GameObject CreateRandomItem(Vector3 position)
    {
        return itemDatabase != null ? CreateItem(itemDatabase.GetRandom(), position) : null;
    }

    public void ClearAll()
    {
        foreach (GameObject obj in createdItems)
        {
            if (obj != null) Destroy(obj);
        }
        createdItems.Clear();
        Debug.Log("Все предметы удалены.");
    }

    // Белый квадрат, генерируется в коде: 4x4 px, 4 pixels per unit = 1x1 юнит
    private static Sprite GetSquareSprite()
    {
        if (squareSprite != null) return squareSprite;

        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        Color32[] pixels = new Color32[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(255, 255, 255, 255);
        tex.SetPixels32(pixels);
        tex.Apply();

        squareSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        return squareSprite;
    }
}
