using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RebindOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject window;
    [SerializeField] private TMP_Text warningMessage;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        warningMessage.text = string.Empty;
        window.SetActive(true);
    }

    public void Hide() => window.SetActive(false);

    public void ShowDuplicate(string pressedKey, string usedByAction)
    {
        warningMessage.text = $"{pressedKey} is already bound to {usedByAction}!";
        window.SetActive(true);
    }
}