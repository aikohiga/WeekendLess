using UnityEngine;
using System.Collections;

public class SimpleItemQuest : MonoBehaviour
{
    [Header("Настройки квеста")]
    [SerializeField] private int totalItemsRequired = 5;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask finalObjectLayer;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("Настройки затемнения")]
    [SerializeField] private float fadeDuration = 2f;

    [Header("Текст завершения")]
    [SerializeField] private string successMessage = "Вы успешно починили лифт!";

    [Header("Ссылки")]
    [SerializeField] private Camera playerCamera;

    private int collectedItems = 0;
    private bool questCompleted = false;
    private bool isFading = false;
    private bool showMessage = false;
    private float fadeAlpha = 0f;
    private Texture2D blackTexture;
    private GUIStyle messageStyle;
    private RaycastHit hit;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Создаём чёрную текстуру
        blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Apply();

        // Настраиваем стиль для текста
        messageStyle = new GUIStyle();
        messageStyle.alignment = TextAnchor.MiddleCenter;
        messageStyle.fontSize = 40;
        messageStyle.fontStyle = FontStyle.Bold;
        messageStyle.normal.textColor = Color.white;
    }

    void Update()
    {
        if (questCompleted) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // После сбора всех предметов ищем финальный объект
        if (collectedItems >= totalItemsRequired)
        {
            if (Physics.Raycast(ray, out hit, interactionRange, finalObjectLayer))
            {
                if (Input.GetKeyDown(interactionKey))
                {
                    StartCoroutine(CompleteQuest());
                }
            }
        }
        // Сбор предметов
        else if (Physics.Raycast(ray, out hit, interactionRange, itemLayer))
        {
            if (Input.GetKeyDown(interactionKey))
            {
                collectedItems++;
                Destroy(hit.collider.gameObject);

                if (collectedItems >= totalItemsRequired)
                {
                    Debug.Log("Все детали собраны. Найдите и почините лифт");
                }
            }
        }
    }

    private IEnumerator CompleteQuest()
    {
        questCompleted = true;

        // Затемнение экрана
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeAlpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        fadeAlpha = 1f;

        // Показываем сообщение
        showMessage = true;

        // Ждём 3 секунды
        yield return new WaitForSeconds(3f);

        // Дальше можно добавить загрузку следующей сцены или другие действия
        Debug.Log("Квест завершён!");
    }

    void OnGUI()
    {
        // Затемнение экрана
        if (fadeAlpha > 0)
        {
            GUI.color = new Color(0, 0, 0, fadeAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
            GUI.color = Color.white;
        }

        // Сообщение о завершении
        if (showMessage)
        {
            GUI.Label(new Rect(0, Screen.height / 2 - 50, Screen.width, 100),
                     successMessage, messageStyle);
        }

        // Прогресс квеста (пока не завершён)
        if (!questCompleted)
        {
            GUI.Label(new Rect(10, 10, 300, 30),
                     $"Собрано деталей: {collectedItems}/{totalItemsRequired}");

            if (collectedItems >= totalItemsRequired)
            {
                GUI.Label(new Rect(10, 40, 400, 30),
                         $"Найдите и почините лифт. Нажмите {interactionKey} для взаимодействия");
            }
        }
    }
}