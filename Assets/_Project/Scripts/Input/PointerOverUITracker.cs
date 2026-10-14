using UnityEngine;
using UnityEngine.EventSystems;

public class PointerOverUITracker : MonoBehaviour
{
    public static bool IsCurrentlyOverUI { get; private set; }

    private void Awake()
    {
        if (EventSystem.current == null)
            print("You forgot to add an EventSystem to your Scene.");
    }

    private void Update()
    {
        IsCurrentlyOverUI = EventSystem.current.IsPointerOverGameObject();
    }
}