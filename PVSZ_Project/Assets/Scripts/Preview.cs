using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PreviewType
{
    Card,
    Remove
}

public class Preview : MonoBehaviour
{
    public PreviewType Type;
    public CardType CardType;
    
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // NOTE(sftl): in front of camera
        transform.position = mousePosition;
    }
}