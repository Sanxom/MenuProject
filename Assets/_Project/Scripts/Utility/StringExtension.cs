using System.Collections.Generic;
using UnityEngine;

public static class StringExtension
{
    public static string Capitalize(this string value) => string.IsNullOrEmpty(value) ? value : $"{char.ToUpper(value[0])}{value.Substring(1)}";
}