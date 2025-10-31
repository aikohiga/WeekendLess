public interface IInteractableDoor
{
    void ToggleDoor();
    void OpenDoor();
    void CloseDoor();
    bool IsOpen { get; }
    string DoorName { get; }
}