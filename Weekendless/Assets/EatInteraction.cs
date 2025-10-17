using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WashInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public float interactionDistance = 3f;
    public Camera playerCamera;
    public TMP_Text hintText;
    public AudioClip eatSound;
    public float destroyDelay = 0.2f;

    private AudioSource audioSource;
    private Renderer objectRenderer;
    private Collider objectCollider;

    private bool isNearFood = false;
    private bool isEaten = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }

        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isEaten)
        {
            CheckFood();
            HandleInput();
        }
    }

    void CheckFood()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Food"))
            {
                if (!isNearFood)
                {
                    isNearFood = true;

                    if (hintText != null)
                    {
                        hintText.gameObject.SetActive(true);
                        hintText.text = "Нажмите E";
                    }
                }
                return;
            }
        }

        if (isNearFood)
        {
            isNearFood = false;

            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }

    void HandleInput()
    {
        if (isNearFood && Input.GetKeyDown(KeyCode.E) && !isEaten)
        {
            EatObject();
        }
    }

    void EatObject()
    {
        isEaten = true;

        if (hintText != null)
            hintText.gameObject.SetActive(false);

        if (objectRenderer != null)
            objectRenderer.enabled = false;

        if (objectCollider != null)
            objectCollider.enabled = false;

        if (eatSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(eatSound);

            Destroy(gameObject, eatSound.length);
        }
        else
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}