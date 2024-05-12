using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [HideInInspector] public Vector3 targetPosition;
    private RoomManager level;
    void Start()
    {
        level = FindObjectOfType<RoomManager>();
    }

    private void Update()
    {
        if(level != null) {
            targetPosition = level.GetRoomPosition();
            targetPosition.z = -10;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}
