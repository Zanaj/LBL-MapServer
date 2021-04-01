using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : Entity
{
    public static float TARGET_VIEW_DISTANCE = 50;
    public static float AttackSpeed = 750;

    public static DateTime lastAttack;

    public Account accountData;
    public PlayerDesign design;
    public EntityStats stats;

    public int characterID;
    public DateTime lastRebirth;

    public int guildId;
    public int ap;
    public float exp;
    public int totalLevel;

    public float rotation = 0;
    public float walkingSpeed = 5;
    public float jumpHeight = 5;
    public float gravity = -9.81f;
    private float yVelocity;
    public CharacterController controller;

    public bool[] inputs;

    public int[] skillBar;

    public override bool isInteractable => true;
    public override EntityType type => EntityType.Player; 

    private void Start()
    {
        lastAttack = DateTime.Now;
    }

    private void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, rotation, 0);

        if (inputs.Length > 0)
        {

            Vector2 _inputDirection = Vector2.zero;
            _inputDirection.y += inputs[0] ? 1 : 0;
            _inputDirection.y -= inputs[1] ? 1 : 0;

            _inputDirection.x -= inputs[2] ? 1 : 0;
            _inputDirection.x += inputs[3] ? 1 : 0;

            Move(_inputDirection);
        }
    }

    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= walkingSpeed * Time.deltaTime;

        if (controller.isGrounded)
        {
            yVelocity = 0;
            yVelocity = inputs[4] ? jumpHeight : 0;
        }

        //yVelocity += gravity;
        _moveDirection.y = 0;
        controller.Move(_moveDirection);

        EntitySync sync = new EntitySync();
        sync.entity = this;
        
        sync.entityType = EntityType.Player;
        sync.Serialize();

        NetworkManager.instance.SendAll(sync);
    }

    public override void MakeEntityPacket(BinaryWriter writer)
    {
        writer.Write((int)EntityType.Player);
        writer.Write(displayName);
        writer.Write(entityGUID);
        writer.Write(isInteractable);

        writer.Write(transform.position.x);
        writer.Write(transform.position.y);
        writer.Write(transform.position.z);

        writer.Write(0);

        writer.Write(characterID);
        writer.Write(design.bodyType);
        writer.Write(design.genativ);
        writer.Write(design.referal);
        writer.Write(rotation);
    }

    public bool CanAttack()
    {
        TimeSpan span = DateTime.Now - lastAttack;
        return span.TotalMilliseconds >= AttackSpeed;
    }
}