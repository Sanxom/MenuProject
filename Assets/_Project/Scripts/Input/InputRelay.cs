using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputRelay", menuName = "InputRelay")]
public class InputRelay : ScriptableObject, GameInput.IPlayerActions
{
    // --- Events
    #region Events
    public event Action<Vector2> OnMoved;
    public event Action<Vector2> OnLooked;
    public event Action<float> OnZoomed;
    public event Action<bool> OnPanned;
    public event Action OnLeftClicked;
    public event Action OnRightClicked;
    public event Action OnMiddleClicked;
    public event Action OnInteracted;
    public event Action OnPaused;
    public event Action OnTapExampled;
    public event Action OnSlowTapExampled;
    public event Action OnHoldExampled;
    public event Action<bool> OnPressExampled;
    #endregion

    // --- For Constant Checks
    #region Constant Checks
    public Vector2 Direction => gameInput.Player.Move.ReadValue<Vector2>();
    public Vector2 LookDelta => gameInput.Player.Look.ReadValue<Vector2>();
    public Vector2 ScrollDelta => gameInput.Player.Zoom.ReadValue<Vector2>();
    public Vector2 MousePosition => gameInput.UI.Point.ReadValue<Vector2>();
    public bool IsPanPressed => gameInput.Player.Pan.IsPressed();
    public bool IsMiddleButtonPressed => gameInput.Player.MiddleClick.IsPressed();
    #endregion

    private GameInput gameInput;

    public InputActionAsset Actions
    {
        get
        {
            EnsureGameInputWasCreated();
            return gameInput.asset;
        }
    }

    #region Rebinding
    private const string REBINDS_KEY = "InputRebinds";

    public void SaveRebinds()
    {
        EnsureGameInputWasCreated();
        PlayerPrefs.SetString(REBINDS_KEY, gameInput.asset.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

    private void LoadRebinds()
    {
        string json = PlayerPrefs.GetString(REBINDS_KEY, string.Empty);

        if (!string.IsNullOrEmpty(json))
            gameInput.asset.LoadBindingOverridesFromJson(json);
    }
    #endregion

    // --- Key Presses and Key Press States
    #region Key Presses and Key Press States
    public void OnMove(InputAction.CallbackContext context) => OnMoved?.Invoke(context.ReadValue<Vector2>());

    public void OnLook(InputAction.CallbackContext context) => OnLooked?.Invoke(context.ReadValue<Vector2>());

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnZoomed?.Invoke(context.ReadValue<Vector2>().y);
    }

    public void OnPan(InputAction.CallbackContext context)
    {
        if (context.started)
            OnPanned?.Invoke(true);
        if (context.canceled)
            OnPanned?.Invoke(false);
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnLeftClicked?.Invoke();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnRightClicked?.Invoke();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnMiddleClicked?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnInteracted?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnPaused?.Invoke();
    }

    public void OnTapExample(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnTapExampled?.Invoke();
    }

    public void OnSlowTapExample(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSlowTapExampled?.Invoke();
    }

    public void OnHoldExample(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnHoldExampled?.Invoke();
    }

    public void OnPressExample(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnPressExampled?.Invoke(context.ReadValueAsButton());
    }
    #endregion

    // --- Enabling and Disabling Input
    #region Enabling and Disabling Input
    public void EnableAllInput()
    {
        EnableWorldInput();
        EnableUIInput();
    }

    public void EnableWorldInput()
    {
        Debug.Log("Enable World Input");
        EnsureGameInputWasCreated();
        gameInput.Player.Enable();
    }

    public void EnableUIInput()
    {
        EnsureGameInputWasCreated();
        gameInput.UI.Enable();
    }

    public void DisableWorldInput()
    {
        Debug.Log("Disabling World Input");
        gameInput.Player.Disable();
        gameInput.Player.Pause.Enable();
    }

    public void DisableUIInput() => gameInput.UI.Disable();

    private void EnsureGameInputWasCreated()
    {
        if (gameInput != null) return;

        gameInput = new();
        gameInput.Player.SetCallbacks(this);
        LoadRebinds();
    }
    #endregion
}