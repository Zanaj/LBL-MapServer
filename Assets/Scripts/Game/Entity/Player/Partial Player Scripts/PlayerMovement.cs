using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    [Header("Movement Settings")]
    public float rotation = 0;
    public float walkingSpeed = 5;
    public float jumpHeight = 5;
    public float gravity = -9.81f;
    private float yVelocity;
    public CharacterController controller;
    public bool[] inputs;

    // Start is called before the first frame update
    private void Movement_Start()
    {
        
    }

    // Update is called once per frame
    private void Movement_Update()
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

        _moveDirection.y = 0;
        controller.Move(_moveDirection);

        EntitySync sync = new EntitySync();
        sync.entity = this;

        sync.entityType = EntityType.Player;
        sync.Serialize();

        NetworkManager.instance.SendAll(sync);
    }

}
