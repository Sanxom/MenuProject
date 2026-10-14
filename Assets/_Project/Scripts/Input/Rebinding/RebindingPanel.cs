using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindingPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputRelay inputRelay;
    [SerializeField] private RebindOverlay overlay;
    [SerializeField] private Transform rowContainer;
    [SerializeField] private Button resetToDefaultButton;
    [SerializeField] private Button closeButton;

    [Header("Prefabs")]
    [SerializeField] private RebindRow rebindRowPrefab;
    [SerializeField] private RebindHeader rebindHeaderPrefab;

    [Header("What To Show")]
    [SerializeField] private string headerForSimpleInput = "Actions";
    [SerializeField] private string[] excludedActionArray = { "Move", "Look", "Zoom" };
    [SerializeField] private string[] includedMapArray = { "Player" };
    [SerializeField] private string[] includedSchemeArray = { "Keyboard&Mouse", "Gamepad" };

    private List<RebindButton> rebindButtonList = new();
    private RebindHeader nonCompositeHeader;

    private void OnEnable()
    {
        Build();
        resetToDefaultButton.onClick.AddListener(ResetToDefault);
        closeButton.onClick.AddListener(Close);
        inputRelay.OnPaused += Close;
    }

    private void OnDisable()
    {
        resetToDefaultButton.onClick.RemoveListener(ResetToDefault);
        closeButton.onClick.RemoveListener(Close);
        inputRelay.OnPaused -= Close;
    }

    public void Open() => gameObject.SetActive(true);

    public void Close() => gameObject.SetActive(false);

    private bool IsExcluded(InputAction action)
    {
        foreach (string excluded in excludedActionArray)
            if (action.name == excluded) 
                return true;

        return false;
    }

    private bool SchemeIncluded(InputBinding binding)
    {
        if (string.IsNullOrEmpty(binding.groups))
            return true;

        foreach (string group in binding.groups.Split(InputBinding.Separator))
        {
            foreach (string scheme in includedSchemeArray)
            {
                if (group == scheme)
                    return true;
            }
        }

        return false;
    }

    private void Build()
    {
        Clear();

        InputActionAsset asset = inputRelay.Actions;

        foreach (string mapName in includedMapArray)
        {
            InputActionMap map = asset.FindActionMap(mapName, throwIfNotFound: false);
            if (map == null)
            {
                Debug.LogWarning($"There is no Action Map named '{mapName}'. Check your spelling.", this);
                continue;
            }

            nonCompositeHeader = null;

            foreach (InputAction action in map.actions)
            {
                BuildRowsForAction(action, true);
            }

            foreach (InputAction action in map.actions)
            {
                BuildRowsForAction(action, false);
            }
        }
    }

    private void BuildRowsForAction(InputAction action, bool isComposite)
    {
        Dictionary<string, RebindRow> rowsByKey = new();
        RebindHeader header = null;
        bool loggedSkip = false;

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];

            if (binding.isComposite) continue;
            if (binding.isPartOfComposite != isComposite) continue;
            if (!SchemeIncluded(binding)) continue;

            string key;
            string displayName;

            if (binding.isPartOfComposite)
            {
                if (header == null)
                {
                    header = Instantiate(rebindHeaderPrefab, rowContainer);
                    header.Init(action.name);
                }

                key = $"{action.name}/{binding.name}";
                displayName = binding.name.Capitalize();
            }
            else
            {
                if (IsExcluded(action))
                {
                    if (!loggedSkip)
                    {
                        Debug.Log($"Skipping '{action.name}': listed under Excluded Actions.", this);
                        loggedSkip = true;
                    }
                    continue;
                }

                if (nonCompositeHeader == null && !string.IsNullOrEmpty(headerForSimpleInput))
                {
                    nonCompositeHeader = Instantiate(rebindHeaderPrefab, rowContainer);
                    nonCompositeHeader.Init(headerForSimpleInput);
                }

                key = action.name;
                displayName = action.name;
            }

            if (!rowsByKey.TryGetValue(key, out RebindRow row))
            {
                row = Instantiate(rebindRowPrefab, rowContainer);
                row.Init(displayName);
                rowsByKey.Add(key, row);
            }

            rebindButtonList.Add(row.AddBinding(inputRelay, action, i, overlay));
        }
    }

    private void Clear()
    {
        foreach (Transform child in rowContainer)
            Destroy(child.gameObject);

        rebindButtonList.Clear();
        nonCompositeHeader = null;
    }

    private void ResetToDefault()
    {
        foreach (InputActionMap map in inputRelay.Actions.actionMaps)
            map.RemoveAllBindingOverrides();

        inputRelay.SaveRebinds();

        foreach (RebindButton button in rebindButtonList)
            button.RefreshLabel();
    }
}