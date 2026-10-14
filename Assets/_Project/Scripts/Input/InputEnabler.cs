using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputEnabler : MonoBehaviour
{
    public event Action<bool> OnPaused;

    [SerializeField] private InputRelay inputRelay;

    private bool currentlyPaused = false;

    public bool CurrentlyPaused { get => currentlyPaused; set => currentlyPaused = value; }

    private void Awake() => inputRelay.OnPaused += ToggleGameInput;

    private void Start() => inputRelay.EnableAllInput();

    private void OnDestroy() => inputRelay.OnPaused -= ToggleGameInput;

    private void ToggleGameInput()
    {
        if (SceneManager.GetActiveScene().buildIndex < 1)
            return;
        currentlyPaused = !currentlyPaused;

        OnPaused?.Invoke(currentlyPaused);
        print($"Game Input: {currentlyPaused}");

        if (currentlyPaused)
        {
            inputRelay.DisableWorldInput();
            Time.timeScale = 0f;
        }
        else
        {
            inputRelay.EnableWorldInput();
            Time.timeScale = 1f;
        }
    }
}