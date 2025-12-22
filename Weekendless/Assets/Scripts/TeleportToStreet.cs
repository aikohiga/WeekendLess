using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ApartmentExitDoor : MonoBehaviour
{
    [Header("=== НАСТРОЙКИ ДВЕРИ ИЗ КВАРТИРЫ ===")]
    [Tooltip("Эта дверь ТОЛЬКО для выхода из квартиры на улицу")]
    public float interactionDistance = 3f;
    public Camera playerCamera;
    public TMP_Text hintText;
    public KeyCode interactKey = KeyCode.E;

    [Header("Точка телепортации")]
    [Tooltip("Перетащите сюда точку НА УЛИЦЕ (не в институте!)")]
    public Transform exitToStreetPoint;

    [Header("Игрок")]
    public Transform playerTransform;

    [Header("Эффекты телепортации")]
    public Image fadeImage;
    public AudioClip teleportSound;
    public float fadeDuration = 1f;
    public float teleportDelay = 0.5f;

    // Приватные переменные - УНИКАЛЬНЫЕ для этой двери
    private bool isNearThisDoor = false;
    private bool isTeleportingFromThisDoor = false;
    private AudioSource audioSource;

    // Кулдаун для ЭТОЙ конкретной двери
    private float doorCooldown = 2f;
    private bool isOnCooldown = false;

    void Start()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        Debug.Log("Скрипт 'ApartmentExitDoor' инициализирован");
        Debug.Log($"Эта дверь телепортирует на улицу в точку: {exitToStreetPoint?.name}");
    }

    void Update()
    {
        if (!isTeleportingFromThisDoor && !isOnCooldown)
        {
            CheckThisDoorOnly();

            if (isNearThisDoor && Input.GetKeyDown(interactKey))
            {
                StartCoroutine(ApartmentExitTeleportSequence());
            }
        }
    }

    void CheckThisDoorOnly()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // КРИТИЧЕСКИ ВАЖНО: проверяем только ЭТУ конкретную дверь
            if (hit.collider.gameObject == this.gameObject)
            {
                if (!isNearThisDoor)
                {
                    isNearThisDoor = true;
                    if (hintText != null)
                    {
                        hintText.gameObject.SetActive(true);
                        hintText.text = $"Нажмите {interactKey} чтобы выйти на улицу";
                    }
                    Debug.Log("Игрок смотрит на дверь из квартиры");
                }
                return;
            }
        }

        if (isNearThisDoor)
        {
            isNearThisDoor = false;
            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }

    IEnumerator ApartmentExitTeleportSequence()
    {
        Debug.Log("=== НАЧАЛО ТЕЛЕПОРТАЦИИ ИЗ КВАРТИРЫ ===");
        isTeleportingFromThisDoor = true;
        isOnCooldown = true;

        if (hintText != null)
            hintText.gameObject.SetActive(false);

        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
            Debug.Log("Проигрывается звук телепортации из квартиры");
        }

        // Фаза 1: Затемнение
        if (fadeImage != null)
        {
            Debug.Log("Начинаем затемнение...");
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0, 1, t / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }
        }

        yield return new WaitForSeconds(teleportDelay);

        // ТЕЛЕПОРТАЦИЯ ИЗ КВАРТИРЫ НА УЛИЦУ
        if (playerTransform != null && exitToStreetPoint != null)
        {
            CharacterController controller = playerTransform.GetComponent<CharacterController>();
            bool hadController = false;

            if (controller != null)
            {
                hadController = true;
                controller.enabled = false;
                Debug.Log("CharacterController отключен");
            }

            Vector3 oldPosition = playerTransform.position;
            playerTransform.position = exitToStreetPoint.position;

            // Поворачиваем игрока ОТ точки телепорта (чтобы не смотрел на другие двери)
            playerTransform.rotation = Quaternion.Euler(0, Random.Range(120, 240), 0);

            if (hadController)
            {
                controller.enabled = true;
                Debug.Log("CharacterController включен");
            }

            Debug.Log($"Телепортирован ИЗ КВАРТИРЫ: {oldPosition} → {exitToStreetPoint.position}");
        }
        else
        {
            Debug.LogError("Ошибка: игрок или точка телепорта не назначены!");
        }

        // Фаза 2: Возвращение
        if (fadeImage != null)
        {
            Debug.Log("Возвращаем экран...");
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(1, 0, t / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }
        }

        // Завершение
        isTeleportingFromThisDoor = false;
        isNearThisDoor = false;

        // Включаем кулдаун на 2 секунды для ЭТОЙ двери
        yield return new WaitForSeconds(doorCooldown);
        isOnCooldown = false;

        Debug.Log("=== ТЕЛЕПОРТАЦИЯ ИЗ КВАРТИРЫ ЗАВЕРШЕНА ===");
    }

    void OnDrawGizmosSelected()
    {
        if (exitToStreetPoint != null)
        {
            // ЗЕЛЕНЫЙ цвет для двери из квартиры
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, exitToStreetPoint.position);
            Gizmos.DrawWireSphere(exitToStreetPoint.position, 0.5f);

            // Подписи
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, "ДВЕРЬ ИЗ КВАРТИРЫ");
            UnityEditor.Handles.Label(exitToStreetPoint.position + Vector3.up * 0.5f, "ТОЧКА НА УЛИЦЕ");
        }
    }
}