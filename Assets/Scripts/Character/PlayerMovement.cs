using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlled by attached Joystick
/// <para>'IsTriggered' is always set to True so attached object can move whenever there's a joystick on script</para>
/// </summary>
public class PlayerMovement : ObjectMovement
{
    [SerializeField] private Joystick moveJoy;

    protected override void Start()
    {
        base.Start();
    }
    
    protected override void Update()
    {
        base.Update();
        
        Vector2 moveInput = new Vector2(moveJoy.Horizontal, moveJoy.Vertical);
        moveVelocity = moveInput.normalized * moveSpeed;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Move()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    protected override bool CheckTrigger()
    {
        return true;
    }
}
