using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindRow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button resetButton;

    [Header("Prefab")]
    [SerializeField] private RebindButton rebindButtonPrefab;

    private readonly List<RebindButton> rebindButtonList = new();

    private void OnEnable() => resetButton.onClick.AddListener(ResetToDefault);

    private void OnDisable() => resetButton.onClick.RemoveListener(ResetToDefault);

    public void Init(string displayName) => nameLabel.text = displayName;

    public RebindButton AddBinding(InputRelay relay, InputAction action, int bindingIndex, RebindOverlay overlay)
    {
        RebindButton button = Instantiate(rebindButtonPrefab, buttonContainer);
        button.Init(relay, action, bindingIndex, overlay);
        rebindButtonList.Add(button);
        return button;
    }

    private void ResetToDefault()
    {
        foreach (RebindButton button in rebindButtonList)
            button.ResetToDefault();
    }
}