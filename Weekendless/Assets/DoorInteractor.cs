using UnityEngine;
using TMPro; // важно: для TextMeshPro

public class DoorInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private GameObject hintText; // 👈 ссылка на надпись
    [SerializeField] private bool drawDebugRay = true;

    private FridgeDoor currentDoor = null;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (hintText != null)
            hintText.SetActive(false);
    }

    private void Update()
    {
        if (drawDebugRay)
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactDistance, Color.green);

        CheckForDoor();

        if (Input.GetKeyDown(interactKey) && currentDoor != null)
        {
            currentDoor.ToggleDoor();
        }
    }

    private void CheckForDoor()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, doorLayer))
        {
            FridgeDoor door = hit.collider.GetComponentInParent<FridgeDoor>();

            if (door != null)
            {
                currentDoor = door;

                // показываем надпись
                if (hintText != null && !hintText.activeSelf)
                    hintText.SetActive(true);

                return;
            }
        }

        // если луч никуда не попал — скрываем надпись
        currentDoor = null;
        if (hintText != null && hintText.activeSelf)
            hintText.SetActive(false);
    }
}
