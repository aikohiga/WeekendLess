using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestControllerFiveItems : MonoBehaviour
{
    [Header("Настройки предметов")]
    [SerializeField] private List<string> requiredItems = new List<string>();
    [SerializeField] private int maxItems = 5; // Изменено на 5

    [Header("Целевой объект")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float interactionDistance = 3f;

    [Header("UI элементы")]
    [Tooltip("Текст для отображения подсказок квеста")]
    [SerializeField] private Component questTextComponent;

    [Tooltip("Текст для отображения счетчика предметов")]
    [SerializeField] private Component itemCountTextComponent;

    [Tooltip("Панель завершения квеста")]
    [SerializeField] private GameObject completionPanel;

    [Tooltip("Текст завершения квеста")]
    [SerializeField] private Component completionTextComponent;

    [Tooltip("Панель для затемнения экрана")]
    [SerializeField] private UnityEngine.UI.Image fadePanel;

    [Header("Настройки завершения")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private string completionMessage = "Квест завершен!\nВсе 5 предметов успешно установлены!";

    [Header("Настройки подсказок")]
    [SerializeField] private string startMessage = "Найдите и соберите 5 предметов";
    [SerializeField] private string collectMessage = "Собрано предметов: {0}/5";
    [SerializeField] private string readyMessage = "Все предметы собраны! Подойдите к объекту и нажмите E";
    [SerializeField] private string nearObjectMessage = "Нажмите E чтобы установить предметы";

    private HashSet<string> collectedItems = new HashSet<string>();
    private bool questCompleted = false;
    private Transform playerTransform;
    private bool isPlayerNearTarget = false;

    void Start()
    {
        // Инициализация списка предметов, если он пустой
        if (requiredItems.Count == 0)
        {
            for (int i = 1; i <= maxItems; i++)
            {
                requiredItems.Add($"Предмет {i}");
            }
        }

        // Находим игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Игрок не найден! Убедитесь, что у игрока есть тег 'Player'");
        }

        // Инициализируем UI
        UpdateUI();

        // Скрываем панель завершения
        if (completionPanel != null)
            completionPanel.SetActive(false);

        // Настраиваем панель затемнения
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (questCompleted) return;

        // Проверяем расстояние до целевого объекта
        bool wasNearTarget = isPlayerNearTarget;
        isPlayerNearTarget = IsPlayerNearTarget();

        // Обновляем UI если состояние изменилось
        if (wasNearTarget != isPlayerNearTarget)
        {
            UpdateUI();
        }

        // Проверяем возможность взаимодействия
        if (IsAllItemsCollected() && isPlayerNearTarget && Input.GetKeyDown(KeyCode.E))
        {
            CompleteQuest();
        }
    }

    // Метод для подбора предмета
    public bool CollectItem(string itemName)
    {
        if (questCompleted) return false;

        if (collectedItems.Count < maxItems && !collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            UpdateUI();

            Debug.Log($"Предмет '{itemName}' подобран. Собрано: {collectedItems.Count}/{maxItems}");

            // Проверяем, известен ли предмет
            if (requiredItems.Contains(itemName))
            {
                Debug.Log($"Предмет '{itemName}' является одним из необходимых!");
            }

            return true;
        }

        return false;
    }

    // Проверка, все ли предметы собраны
    private bool IsAllItemsCollected()
    {
        return collectedItems.Count >= maxItems;
    }

    // Проверка, находится ли игрок рядом с целевым объектом
    private bool IsPlayerNearTarget()
    {
        if (playerTransform == null || targetObject == null) return false;

        float distance = Vector3.Distance(playerTransform.position, targetObject.transform.position);
        return distance <= interactionDistance;
    }

    // Завершение квеста
    private void CompleteQuest()
    {
        if (questCompleted) return;

        questCompleted = true;
        Debug.Log("Квест завершен! Все 5 предметов установлены.");

        // Запускаем корутину для эффекта завершения
        StartCoroutine(QuestCompletionRoutine());
    }

    // Корутина для эффекта завершения квеста
    private IEnumerator QuestCompletionRoutine()
    {
        // Активируем панель затемнения
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);

            // Плавное затемнение
            float elapsedTime = 0f;
            Color panelColor = fadePanel.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                panelColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                fadePanel.color = panelColor;
                yield return null;
            }
        }

        // Показываем панель с текстом завершения
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
            SetText(completionTextComponent, completionMessage);
        }

        // Дополнительная задержка перед следующими действиями
        yield return new WaitForSeconds(3f);

        // Здесь можно добавить дополнительные действия:
        // - Загрузку следующей сцены
        // - Активацию новых объектов
        // - Воспроизведение звука
    }

    // Обновление UI
    private void UpdateUI()
    {
        // Обновляем счетчик предметов
        SetText(itemCountTextComponent, string.Format(collectMessage, collectedItems.Count));

        // Обновляем подсказку квеста
        if (questTextComponent != null)
        {
            string hintText;

            if (IsAllItemsCollected())
            {
                hintText = isPlayerNearTarget ? nearObjectMessage : readyMessage;
            }
            else
            {
                hintText = startMessage;

                // Можно добавить конкретные подсказки о недостающих предметах
                if (collectedItems.Count > 0)
                {
                    hintText += $"\nОсталось собрать: {maxItems - collectedItems.Count} предметов";
                }
            }

            SetText(questTextComponent, hintText);
        }
    }

    // Универсальный метод для установки текста
    private void SetText(Component textComponent, string message)
    {
        if (textComponent == null) return;

        // Проверяем тип текстового компонента
        System.Type componentType = textComponent.GetType();

        // Стандартный UI Text
        if (componentType == typeof(UnityEngine.UI.Text))
        {
            ((UnityEngine.UI.Text)textComponent).text = message;
        }
        // TextMeshPro
        else if (componentType.ToString().Contains("TMP_Text") ||
                 componentType.ToString().Contains("TextMeshProUGUI"))
        {
            // Используем рефлексию для совместимости
            var prop = componentType.GetProperty("text");
            if (prop != null)
            {
                prop.SetValue(textComponent, message);
            }
        }
        else
        {
            Debug.LogWarning($"Неподдерживаемый тип текстового компонента: {componentType}");
        }
    }

    // Метод для проверки прогресса (можно вызвать из других скриптов)
    public int GetCollectedCount()
    {
        return collectedItems.Count;
    }

    public int GetRequiredCount()
    {
        return maxItems;
    }

    public bool IsQuestComplete()
    {
        return questCompleted;
    }

    // Метод для сброса квеста (для отладки)
    public void ResetQuest()
    {
        collectedItems.Clear();
        questCompleted = false;
        UpdateUI();

        if (completionPanel != null)
            completionPanel.SetActive(false);

        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(false);
        }
    }

    // Визуализация радиуса взаимодействия в редакторе
    void OnDrawGizmosSelected()
    {
        if (targetObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetObject.transform.position, interactionDistance);
        }
    }
}