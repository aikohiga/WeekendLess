using UnityEngine;
using TMPro;

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

    private bool isNearObject = false;

    void Start()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckObject();

        if (isNearObject && Input.GetKeyDown(interactKey))
        {
            TeleportPlayer();
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

    void TeleportPlayer()
    {
        if (playerTransform == null || teleportTarget == null)
            return;

        CharacterController controller = playerTransform.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        playerTransform.position = teleportTarget.position;
        playerTransform.rotation = teleportTarget.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }

        if (hintText != null)
            hintText.gameObject.SetActive(false);

        isNearObject = false;
    }
}