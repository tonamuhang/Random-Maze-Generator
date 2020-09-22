using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MuhangTools{
public class Utils
{
    public static TextMesh ShowText(Transform parent, string text, Vector3 localpos, int fontsize, Color color, TextAnchor textAnchor)
    {
        GameObject gameObject = new GameObject("Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localpos;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.text = text;
        textMesh.fontSize = fontsize;
        textMesh.color = color;
        return textMesh;
    }
}
}
