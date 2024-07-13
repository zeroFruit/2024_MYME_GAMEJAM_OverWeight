using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IncomeScript : MonoBehaviour
{
    private float minFontSize = 1f;
    private float fontChangeSpeed = 1.5f;

    private float moveSpeed = 0.15f;
    private float alphaSpeed = 1.5f;
    private float destroyTime = 2f;

    private float time;
    public string Message;
    public TextMeshPro Text;

    private Color Alpha;

    void Start()
    {
        time = 0;
        Text = GetComponent<TextMeshPro>();
        Text.text = Message;
        Alpha = Text.color;

        Destroy(gameObject, destroyTime);
        // time 
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        if (time < 0.2f)
        {
            Text.fontSize += Time.deltaTime * fontChangeSpeed;
        }
        else
        {
            if (!(Text.fontSize <= minFontSize))
            {
                Text.fontSize -= Time.deltaTime * fontChangeSpeed;
            }
        }

        time += Time.deltaTime * fontChangeSpeed;
        Text.color = Alpha;
    }
}