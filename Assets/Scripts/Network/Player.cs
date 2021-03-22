using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : MonoBehaviour
{
    public Account accountData;
    public int characterID;
    public string displayName;
    public int bodyType;
    public string genativ;
    public string referal;
    public float rot;
    public Vector3 currentPosition;
    public Vector3 requestedPosition;

    public float height;
    public float weight;

    public float walkingSpeed = 2;

    public CharacterController controller;

    public Vector3 UpdatePos(float x, float z)
    {
        Vector3 vx = transform.right * x;
        Vector3 vz = transform.forward * z;

        Vector3 move = vx + vz;
        move *= PlayerManager.instance.baseSpeed * Time.deltaTime;

        controller.Move(move);
        return transform.position;
    }
}