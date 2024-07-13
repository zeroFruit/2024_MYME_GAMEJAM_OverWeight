using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public int Idx;
    public TextMeshPro Text;

    private void Awake()
    {
        Text = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    public bool IsEmpty()
    {
        return Text.text == "";
    }

    public void MakeEmpty()
    {
        Text.text = "";
    }

    public void SetText(String text)
    {
        Text.text = text;
    }

    public bool IsText(String text)
    {
        return Text.text == text;
    }
}