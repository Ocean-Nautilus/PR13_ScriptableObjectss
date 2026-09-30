using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Главный контроллер сцены: список предметов, выбор, создание,
/// показ и изменение конфигурации, очистка.
/// Рассчитан на Unity 6 (6000.x).
/// </summary>
public class ScriptableObjectController : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text itemInfoText;
    [SerializeField] private TMP_Text configText;
    [SerializeField] private Transform itemsContainer;

    [Header("Кнопки")]
    [SerializeField] private Button createItemButton;
    [SerializeField] private Button showConfigButton;
    [SerializeField] private Button changeDifficultyButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button itemButtonPrefab;

    [Header("ScriptableObject ссылки")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private ItemFactory itemFactory;

    [Header("Настройки")]
    [SerializeField] private float itemSpawnRadius = 3f;

    private ItemData selectedItem;
    private bool configVisible;
    private readonly List<GameObject> itemButtons = new List<GameObject>();

    private void Start()
    {
        EnsureEventSystem();
        SetupCanvasForWorldObjects();

        if (itemFactory == null) TryGetComponent(out itemFactory);

        if (createItemButton != null) createItemButton.onClick.AddListener(OnCreateItem);
        if (showConfigButton != null) showConfigButton.onClick.AddListener(OnShowConfig);
        if (changeDifficultyButton != null) changeDifficultyButton.onClick.AddListener(OnChangeDifficulty);
        if (clearButton != null) clearButton.onClick.AddListener(OnClear);

        BuildItemList();
        UpdateConfigText();
        SetConfigVisible(false);

        SetStatus("Выберите предмет для создания");
        if (itemInfoText != null) itemInfoText.text = "Информация о предмете";
    }

    private void OnDestroy()
    {
        if (createItemButton != null) createItemButton.onClick.RemoveListener(OnCreateItem);
        if (showConfigButton != null) showConfigButton.onClick.RemoveListener(OnShowConfig);
        if (changeDifficultyButton != null) changeDifficultyButton.onClick.RemoveListener(OnChangeDifficulty);
        if (clearButton != null) clearButton.onClick.RemoveListener(OnClear);
    }

    // ---------- Подготовка сцены ----------

    /// <summary>
    /// В Unity 6 при новой Input System кнопки не работают, если на EventSystem
    /// остался StandaloneInputModule. Проверяем и при необходимости исправляем.
    /// </summary>
    private void EnsureEventSystem()
    {
        EventSystem es = FindFirstObjectByType<EventSystem>();
        if (es == null)
            es = new GameObject("EventSystem", typeof(EventSystem)).GetComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (es.TryGetComponent(out StandaloneInputModule legacy))
            Destroy(legacy);

        if (!es.TryGetComponent(out InputSystemUIInputModule _))
        {
            InputSystemUIInputModule module = es.gameObject.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }
#else
        if (!es.TryGetComponent(out BaseInputModule _))
            es.gameObject.AddComponent<StandaloneInputModule>();
#endif
    }

    /// <summary>
    /// Canvas в режиме Overlay рисуется поверх всего, и фон закрывал бы 2D-объекты.
    /// Переключаем его на Screen Space - Camera.
    /// </summary>
    private void SetupCanvasForWorldObjects()
    {
        if (statusText == null) return;

        Canvas canvas = statusText.canvas;
        if (canvas == null) return;
        canvas = canvas.rootCanvas;

        Camera cam = Camera.main;
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay && cam != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 10f;
            canvas.sortingOrder = 0;
        }
    }

    // ---------- Список предметов ----------

    private void BuildItemList()
    {
        foreach (GameObject btn in itemButtons)
        {
            if (btn != null) Destroy(btn);
        }
        itemButtons.Clear();

        if (itemDatabase == null || itemButtonPrefab == null || itemsContainer == null)
        {
            Debug.LogError("ScriptableObjectController: не назначены Item Database / Item Button Prefab / Items Container.");
            return;
        }

        foreach (ItemData item in itemDatabase.allItems)
        {
            if (item == null) continue;

            ItemData current = item; // копия для лямбды

            Button btn = Instantiate(itemButtonPrefab, itemsContainer);
            btn.name = "Btn_" + current.itemName;

            // Без LayoutElement Vertical Layout Group может схлопнуть кнопку до нулевой высоты
            if (!btn.TryGetComponent(out LayoutElement le))
                le = btn.gameObject.AddComponent<LayoutElement>();
            le.minHeight = 50f;
            le.preferredHeight = 50f;

            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = $"{current.itemName} ({current.price} з.)";

            btn.onClick.AddListener(() => OnItemSelected(current));
            itemButtons.Add(btn.gameObject);
        }
    }

    // ---------- Обработчики кнопок ----------

    private void OnItemSelected(ItemData item)
    {
        selectedItem = item;
        if (itemInfoText != null) itemInfoText.text = item.GetInfo();
        SetStatus("Выбран: " + item.itemName);
    }

    private void OnCreateItem()
    {
        if (selectedItem == null)
        {
            SetStatus("Сначала выберите предмет!");
            return;
        }
        if (itemFactory == null) return;

        Vector2 offset = Random.insideUnitCircle * itemSpawnRadius;
        offset.y *= 0.7f; // чтобы предметы не заходили под UI
        itemFactory.CreateItem(selectedItem, new Vector3(offset.x, offset.y, 0f));

        SetStatus("Создан: " + selectedItem.itemName);
    }

    private void OnShowConfig()
    {
        if (gameConfig == null) return;

        // Первое нажатие — показать, второе — скрыть
        SetConfigVisible(!configVisible);

        if (configVisible)
        {
            UpdateConfigText();
            Debug.Log(gameConfig.GetFullInfo());
            SetStatus("Конфигурация показана");
        }
        else
        {
            SetStatus("Конфигурация скрыта");
        }
    }

    private void OnChangeDifficulty()
    {
        if (gameConfig == null) return;

        gameConfig.NextDifficulty();
        UpdateConfigText();
        SetStatus("Сложность: " + gameConfig.GetDifficultyName());
    }

    private void OnClear()
    {
        if (itemFactory != null) itemFactory.ClearAll();

        selectedItem = null;
        if (itemInfoText != null) itemInfoText.text = "Информация о предмете";
        SetStatus("Всё очищено");
    }

    // ---------- Вспомогательные ----------

    private void SetConfigVisible(bool visible)
    {
        configVisible = visible;
        if (configText != null) configText.gameObject.SetActive(visible);
    }

    private void UpdateConfigText()
    {
        if (configText != null && gameConfig != null)
            configText.text = gameConfig.GetShortInfo();
    }

    private void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }
}
