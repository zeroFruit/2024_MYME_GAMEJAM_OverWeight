using System;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public Floor StartFloor;
    public Floor TargetFloor;
    public int Weight; // 무게
    public bool isReadyToRide;
    
    public float maxX;
    public float movingSpeed;
    
    public void Init()
    {
        Weight = 20;
        isReadyToRide = false;
        maxX = 0.3f;
        movingSpeed = 1;
    }

    private void Start()
    {
        transform.localPosition = new Vector3(maxX * -1f, 0.45f, 0);
    }

    private void Update()
    {
        Vector3 curPos = transform.localPosition;
        float nextPosX = curPos.x + (Time.deltaTime * movingSpeed);
        if (nextPosX > maxX)
        {
            transform.localPosition = new Vector3(maxX, curPos.y, curPos.z);
            isReadyToRide = true;
            return;
        }
        transform.localPosition = new Vector3(nextPosX, curPos.y, curPos.z);
    }
}