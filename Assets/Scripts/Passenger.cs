using System;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public Floor StartFloor;
    public Floor TargetFloor;
    public int Weight; // 비중
    public bool isReadyToRide;

    public float maxX;
    public float movingSpeed;

    public DateTime SpawnedAt;
    public bool inElevator;
    public ElevatorController queuedElevator;

    public void Init(Floor floor)
    {
        StartFloor = floor;
        TargetFloor = FloorManager.Instance.GetRandomFloor(floor);
        Weight = 20;
        isReadyToRide = false;

        maxX = 0.2f;
        movingSpeed = 2;

        SpawnedAt = DateTime.Now;
        inElevator = false;
    }

    private void Start()
    {
        transform.localPosition = new Vector3(maxX * -1f, 0.6f, 0);
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