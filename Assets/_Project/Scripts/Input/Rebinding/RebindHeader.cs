using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RebindHeader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text label;

    private void Reset() => label = GetComponentInChildren<TMP_Text>();

    public void Init(string title) => label.text = title;
}