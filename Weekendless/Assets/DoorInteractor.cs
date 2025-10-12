using UnityEngine;
using TMPro; 

public class DoorInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 1;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private GameObject hintText; 
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

        
        if (Physics.Raycast(ray, out hit, interactDistance, ~0))
        {
            
            FridgeDoor door = hit.collider.GetComponentInParent<FridgeDoor>();
            
            if (door != null)
            {
                
                if (((1 << hit.collider.gameObject.layer) & doorLayer) != 0)
                {
                    currentDoor = door;

                    
                    if (hintText != null && !hintText.activeSelf)
                        hintText.SetActive(true);

                    return;
                }
            }
        }

        currentDoor = null;
        if (hintText != null && hintText.activeSelf)
            hintText.SetActive(false);
    }
}