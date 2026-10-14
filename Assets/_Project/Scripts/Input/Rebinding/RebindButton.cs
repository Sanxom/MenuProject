using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindButton : MonoBehaviour
{
    private const string GAMEPAD_CONTROL_SCHEME_NAME = "Gamepad";

    [Header("References")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button rebindButton;

    private InputRelay relay;
    private InputAction action;
    private RebindOverlay overlay;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private string previousOverride;
    private int bindingIndex;
    private bool actionWasEnabled;

    private void OnEnable() => rebindButton.onClick.AddListener(StartRebind);

    private void Reset()
    {
        label = GetComponentInChildren<TMP_Text>();
        rebindButton = GetComponentInChildren<Button>();
    }

    private void OnDisable()
    {
        rebindButton.onClick.RemoveListener(StartRebind);

        if (rebindingOperation == null) return;

        rebindingOperation.Dispose();
        rebindingOperation = null;

        if (overlay != null)
            overlay.Hide();

        if (actionWasEnabled)
            action.Enable();
    }

    public void Init(InputRelay relay, InputAction action, int bindingIndex, RebindOverlay overlay)
    {
        this.relay = relay;
        this.action = action;
        this.bindingIndex = bindingIndex;
        this.overlay = overlay;

        RefreshLabel();
    }
    
    public void RefreshLabel()
    {
        label.text = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);

    }

    public void ResetToDefault()
    {
        action.RemoveBindingOverride(bindingIndex);
        RefreshLabel();
        relay.SaveRebinds();
    }

    private void StartRebind()
    {
        previousOverride = action.bindings[bindingIndex].overridePath;
        bool wasEnabled = action.enabled;
        actionWasEnabled = wasEnabled;
        action.Disable();

        overlay.Show();
        PerformRebind(wasEnabled);
    }

    private void PerformRebind(bool wasEnabled)
    {
        rebindingOperation?.Dispose();

        bool isGamepad = action.bindings[bindingIndex].groups.Contains(GAMEPAD_CONTROL_SCHEME_NAME);

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithControlsExcluding("<Mouse>/leftButton")
            .WithCancelingThrough(isGamepad ? "<Gamepad>/start" : "<Keyboard>/escape")
            .OnComplete(_ => OnRebindComplete(wasEnabled))
            .OnCancel(_ => Finish(wasEnabled));

        RestrictToBindingDevices();

        rebindingOperation.Start();
    }

    private void RestrictToBindingDevices()
    {
        string groups = action.bindings[bindingIndex].groups;
        if (string.IsNullOrEmpty(groups)) return;

        InputActionAsset asset = action.actionMap.asset;

        foreach (string groupName in groups.Split(InputBinding.Separator))
        {
            if (string.IsNullOrEmpty(groupName)) continue;

            foreach (InputControlScheme scheme in asset.controlSchemes)
            {
                if (scheme.bindingGroup != groupName) continue;

                foreach (var requirement in scheme.deviceRequirements)
                {
                    if (!string.IsNullOrEmpty(requirement.controlPath))
                        rebindingOperation.WithControlsHavingToMatchPath(requirement.controlPath);
                }
            }
        }
    }

    private void OnRebindComplete(bool wasEnabled)
    {
        if (TryFindDuplicate(out string usedBy))
        {
            string pressedKey = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);

            if (string.IsNullOrEmpty(previousOverride))
                action.RemoveBindingOverride(bindingIndex);
            else
                action.ApplyBindingOverride(bindingIndex, previousOverride);

            overlay.ShowDuplicate(pressedKey, usedBy);
            PerformRebind(wasEnabled);
            return;
        }

        Finish(wasEnabled);
    }

    private bool TryFindDuplicate(out string usedBy)
    {
        InputBinding newBinding = action.bindings[bindingIndex];

        foreach (InputAction action in action.actionMap.actions)
        {
            foreach (InputBinding binding in action.bindings)
            {
                if (binding.isComposite) continue;
                if (binding.id == newBinding.id) continue;
                if (binding.effectivePath != newBinding.effectivePath) continue;
                if (!SharesGroup(binding, newBinding)) continue;
                if (binding.effectiveInteractions != newBinding.effectiveInteractions) continue;

                usedBy = binding.isPartOfComposite ? $"{action.name} {binding.name.Capitalize()}" : action.name;
                return true;
            }
        }

        usedBy = null;
        return false;
    }

    private bool SharesGroup(InputBinding binding, InputBinding newBinding)
    {
        if (string.IsNullOrEmpty(binding.groups) || string.IsNullOrEmpty(newBinding.groups))
            return true;

        foreach (string group in binding.groups.Split(InputBinding.Separator))
        {
            if (string.IsNullOrEmpty(group)) continue;
            if (newBinding.groups.Split(InputBinding.Separator).Contains(group))
                return true;
        }

        return false;
    }

    private void Finish(bool reEnableAction)
    {
        rebindingOperation?.Dispose();
        rebindingOperation = null;

        overlay.Hide();

        if (reEnableAction)
            action.Enable();

        RefreshLabel();
        relay.SaveRebinds();
    }
}