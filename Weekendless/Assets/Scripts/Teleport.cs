using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TeleportInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public float interactionDistance = 3f;
    public Camera playerCamera;
    public TMP_Text hintText;
    public KeyCode interactKey = KeyCode.E;

    [Header("Настройки телепортации")]
    public Transform teleportTarget;
    public Transform playerTransform;

    [Header("Эффекты телепортации")]
    public Image fadeImage;
    public AudioClip teleportSound;
    public float fadeDuration = 1f;
    public float teleportDelay = 0.5f;

    private bool isNearObject = false;
    private bool isTeleporting = false;
    private AudioSource audioSource;

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
        if (!isTeleporting)
        {
            CheckObject();

            if (isNearObject && Input.GetKeyDown(interactKey))
            {
                StartCoroutine(TeleportSequence());
            }
        }
    }

    void CheckObject()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Teleport"))
            {
                if (!isNearObject)
                {
                    isNearObject = true;
                    if (hintText != null)
                        hintText.gameObject.SetActive(true);
                }
                return;
            }
        }

        if (isNearObject)
        {
            isNearObject = false;
            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }

    IEnumerator TeleportSequence()
    {
        isTeleporting = true;

        if (hintText != null)
            hintText.gameObject.SetActive(false);

        if (teleportSound != null && audioSource != null)
            audioSource.PlayOneShot(teleportSound);

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

        if (playerTransform != null && teleportTarget != null)
        {
            CharacterController controller = playerTransform.GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;

            playerTransform.position = teleportTarget.position;
            playerTransform.rotation = teleportTarget.rotation;

            if (controller != null)
                controller.enabled = true;
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

        isTeleporting = false;
        isNearObject = false;
    }
}