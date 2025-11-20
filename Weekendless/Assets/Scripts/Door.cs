using UnityEngine;

public class Door : MonoBehaviour, IInteractableDoor
{
    [Header("Door Settings")]
    public Transform doorPivot;     
    public float openAngle = 90f; 
    public float openSpeed = 2f;  

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    public bool IsOpen => isOpen;
    public string DoorName => "Wooden Door";

    void Start()
    {
        if (doorPivot == null)
            doorPivot = transform; 

        closedRotation = doorPivot.rotation;
        openRotation = Quaternion.Euler(doorPivot.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        doorPivot.rotation = Quaternion.Slerp(
            doorPivot.rotation, 
            targetRotation, 
            Time.deltaTime * openSpeed
        );
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }
}
