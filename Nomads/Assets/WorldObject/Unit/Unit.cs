using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using RTS;

public class Unit : WorldObject
{
    protected bool moving, rotating, teleporting;

    
    private Vector3 destination;
    private Quaternion targetRotation;
    public float moveSpeed;// { get; set; }
    public float rotateSpeed; // { get; set; }
   // public CharacterController ch;

    /*** Game Engine methods, all can be overridden by subclass ***/

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        if (rotating) TurnToTarget();
        else if (moving) MakeMove();
        else if (teleporting) MakeTeleport();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }

    public override void SetHoverState(GameObject hoverObject)
    {
        base.SetHoverState(hoverObject);
        //only handle input if owned by a human player and currently selected
        if (player && player.human && currentlySelected)
        {
            if ((hoverObject.name == "Ground") || (hoverObject.name == "TeleportationGround"))  player.hud.SetCursorState(CursorState.Move);
        }
    }

    public override void MouseClickMove(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClickMove(hitObject, hitPoint, controller);
        //only handle input if owned by a human player and currently selected
        if (player && player.human && currentlySelected)
        {
            if (((hitObject.name == "Ground") || (hitObject.name == "TeleportationGround")) && (hitPoint != ResourceManager.InvalidPosition))
            {
                float x = hitPoint.x;
                //makes sure that the unit stays on top of the surface it is on
                float y = hitPoint.y + player.SelectedObject.transform.position.y;
                float z = hitPoint.z;
                Vector3 destination = new Vector3(x, y, z);
                StartMove(destination);
                attacking = false;
                aiming = false;
              //  rotating = false;
            }
        }
    }

    public override void MouseClickTeleport(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClickTeleport(hitObject, hitPoint, controller);
        //only handle input if owned by a human player and currently selected
        if (player && player.human && currentlySelected)
        {
            if (((hitObject.name== "TeleportationGround")) && (hitPoint != ResourceManager.InvalidPosition) && (TeleportPossible()==true))
            {
                float x = hitPoint.x;
                //makes sure that the unit stays on top of the surface it is on
                float y = hitPoint.y + player.SelectedObject.transform.position.y;
                float z = hitPoint.z;
                Vector3 destination = new Vector3(x, y, z);
                Teleport(destination);
              //  attacking = false;
                aiming = false;
                moving = false;
              //  rotating = false;
            }
        }
    }

    public void Teleport(Vector3 destination)
    {
        this.destination = destination;
        targetRotation = Quaternion.LookRotation(destination - transform.position);
        teleporting = true;
    }
    public void StartMove(Vector3 destination)
    {
        this.destination = destination;
        targetRotation = Quaternion.LookRotation(destination - transform.position);
        rotating = true;
        teleporting = false;
    }

    public override bool TeleportPossible()
    {
        if (Time.time - teleportCooldown > 1.0f)
        {
            teleportCooldown = Time.time;
            return true;
        }
        else return false;
    }

    private void TurnToTarget()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
        //sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
        Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
        if (transform.rotation == targetRotation || transform.rotation == inverseTargetRotation)
        {
            rotating = false;
            moving = true;
        }
        CalculateBounds();
    }

    public void MakeMove()
    {
        if (player.human)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
            if (transform.position == destination)
            {
                moving = false;
                movingIntoPosition = false;
            }
            CalculateBounds();
        } else
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);

           /* Vector3 refVel = Vector3.zero;
            rigid = GetComponent<Rigidbody>();
            rigid.MovePosition(Vector3.SmoothDamp(rigid.transform.position, destination, ref refVel, 0.1f));
            rigid.MovePosition(destination); */

            /*     Vector3 dir = destination - transform.position;
                 Vector3 movement = dir.normalized * moveSpeed * Time.deltaTime;
                 if (movement.magnitude > dir.magnitude) movement = dir;
                 ch = GetComponent<CharacterController>();
                 ch.Move(movement); */



            if (transform.position == destination)
            {
                moving = false;
                movingIntoPosition = false;
            }
            CalculateBounds();
        }
    }

    private void MakeTeleport()
    {
        Vector3 relative = transform.position - Camera.main.transform.position;
        transform.position = new Vector3(destination.x, destination.y, destination.z);
        Camera.main.transform.position = transform.position - relative;
        if (transform.position == destination)
        {
            teleporting = false;
           // movingIntoPosition = false;
        }
        CalculateBounds();
    }

    protected override bool ShouldMakeDecision()
    {
        if (moving || rotating) return false;
        return base.ShouldMakeDecision();
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("laso drq");
    }
}