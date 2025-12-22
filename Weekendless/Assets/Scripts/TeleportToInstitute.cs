using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InstituteEntranceDoor : MonoBehaviour
{
    [Header("=== НАСТРОЙКИ ДВЕРИ В ИНСТИТУТ ===")]
    [Tooltip("Эта дверь ТОЛЬКО для входа в институт с улицы")]
    public float interactionDistance = 3f;
    public Camera playerCamera;
    public TMP_Text hintText;
    public KeyCode interactKey = KeyCode.E;

    [Header("Точка телепортации")]
    [Tooltip("Перетащите сюда точку ВНУТРИ ИНСТИТУТА (не на улице!)")]
    public Transform enterToInstitutePoint;

    [Header("Игрок")]
    public Transform playerTransform;

    [Header("Эффекты телепортации")]
    public Image fadeImage;
    public AudioClip teleportSound;
    public float fadeDuration = 1f;
    public float teleportDelay = 0.5f;

    private bool isNearThisDoor = false;
    private bool isTeleportingFromThisDoor = false;
    private AudioSource audioSource;

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
    }
    void Update()
    {
        if (!isTeleportingFromThisDoor && !isOnCooldown)
        {
            CheckThisDoorOnly();

            if (isNearThisDoor && Input.GetKeyDown(interactKey))
            {
                StartCoroutine(InstituteEntranceTeleportSequence());
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
            if (hit.collider.gameObject == this.gameObject)
            {
                if (!isNearThisDoor)
                {
                    isNearThisDoor = true;
                    if (hintText != null)
                    {
                        hintText.gameObject.SetActive(true);
                        hintText.text = $"Нажмите {interactKey} чтобы войти в институт";
                    }
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

    IEnumerator InstituteEntranceTeleportSequence()
    {
        isTeleportingFromThisDoor = true;
        isOnCooldown = true;

        if (hintText != null)
            hintText.gameObject.SetActive(false);

        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }
        if (fadeImage != null)
        {
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
        if (playerTransform != null && enterToInstitutePoint != null)
        {
            CharacterController controller = playerTransform.GetComponent<CharacterController>();
            bool hadController = false;

            if (controller != null)
            {
                hadController = true;
                controller.enabled = false;
            }

            Vector3 oldPosition = playerTransform.position;
            playerTransform.position = enterToInstitutePoint.position;
            playerTransform.rotation = Quaternion.Euler(0, Random.Range(120, 240), 0);

            if (hadController)
            {
                controller.enabled = true;
            }
        }

        if (fadeImage != null)
        {
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
        isTeleportingFromThisDoor = false;
        isNearThisDoor = false;
        yield return new WaitForSeconds(doorCooldown);
        isOnCooldown = false;
    }

    void OnDrawGizmosSelected()
    {
        if (enterToInstitutePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, enterToInstitutePoint.position);
            Gizmos.DrawWireSphere(enterToInstitutePoint.position, 0.5f);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, "ДВЕРЬ В ИНСТИТУТ");
            UnityEditor.Handles.Label(enterToInstitutePoint.position + Vector3.up * 0.5f, "ТОЧКА В ИНСТИТУТЕ");
        }
    }
}
