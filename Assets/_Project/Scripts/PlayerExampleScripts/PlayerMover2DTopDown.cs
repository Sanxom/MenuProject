using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover2DTopDown : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private InputRelay inputRelay;
    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputRelay.OnInteracted += HandleInteract;
        inputRelay.OnTapExampled += HandleTapExample;
        inputRelay.OnSlowTapExampled += HandleSlowTapExample;
        inputRelay.OnHoldExampled += HandleHoldExample;
        inputRelay.OnPressExampled += HandlePressAndReleaseExample;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveSpeed * inputRelay.Direction;
    }

    private void OnDestroy()
    {
        inputRelay.OnInteracted -= HandleInteract;
        inputRelay.OnTapExampled -= HandleTapExample;
        inputRelay.OnSlowTapExampled -= HandleSlowTapExample;
        inputRelay.OnHoldExampled -= HandleHoldExample;
        inputRelay.OnPressExampled -= HandlePressAndReleaseExample;
    }

    private void HandleInteract()
    {
        print("Interacted!");
    }

    private void HandleTapExample()
    {
        print("Tapped!");
    }

    private void HandleSlowTapExample()
    {
        print("Held + Released!");
    }

    private void HandleHoldExample()
    {
        print("Held!");
    }

    /// <summary>
    /// Example: A shield spell is active while the button is pressed and deactivates when released
    /// </summary>
    /// <param name="isPressed"></param>
    private void HandlePressAndReleaseExample(bool isPressed)
    {
        if (isPressed)
            print("Pressed!");
        else
            print("Released!");
    }
}