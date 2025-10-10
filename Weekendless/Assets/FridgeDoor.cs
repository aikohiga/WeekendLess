using UnityEngine;

public class FridgeDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f; // на сколько градусов открывается дверь
    public float openSpeed = 2f;  // скорость открытия/закрытия

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Запоминаем исходное положение двери
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        // Плавное вращение двери
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    // 👇 Этот метод будет вызываться извне (лучом)
    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
