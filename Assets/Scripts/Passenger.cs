using System;
using HAWStudio.Common;
using TMPro;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public Animator Animator;
    public FeedbackPlayer CreateFeedback;
    public TextMeshPro TargetFloorText;
    
    public Floor StartFloor;
    public Floor TargetFloor;
    public int Weight; // 비중
    public bool isReadyToRide;

    public float maxX;
    public float movingSpeed;

    public DateTime SpawnedAt;
    public bool inElevator;
    public ElevatorController queuedElevator;

    public void Init(Floor start, Floor target)
    {
        StartFloor = start;
        TargetFloor = target ? target : FloorManager.Instance.GetRandomFloor(start);

        if (this.TargetFloorText != null)
        {
            this.TargetFloorText.text = $"{this.TargetFloor.FloorIdx + 1}F";
        }
        
        Weight = 1;
        isReadyToRide = false;

        maxX = 0.2f;
        movingSpeed = 2;

        SpawnedAt = DateTime.Now;
        inElevator = false;

        if (this.CreateFeedback != null)
        {
            this.CreateFeedback.PlayFeedbacks();
        }
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

    public void Angry()
    {
        this.Animator.SetBool("Angry", true);
    }

    public void CalmDown()
    {
        this.Animator.SetBool("Angry", false);
    }
}