using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using System;

public class WorldObject : MonoBehaviour
{

    public int ID;

    public string objectName;
    public Texture2D buildImage;
    public int cost, sellValue, maxHitPoints;
    public int hitPoints; //{ get; set; }
    protected Bounds selectionBounds;

    protected Player player;
    protected string[] actions = { };
    protected bool currentlySelected = false;

    protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

    protected GUIStyle healthStyle = new GUIStyle();
    protected float healthPercentage = 1.0f;

    protected WorldObject target = null;
    protected bool attacking = false;

    public float weaponRange = 10.0f;
    protected bool movingIntoPosition = false;
    protected bool aiming = false;

    public float weaponRechargeTime = 1.0f;
    public float currentWeaponChargeTime;
    public float weaponAimSpeed = 1.0f;

    public float teleportCooldown = 5.0f;
    protected GUIStyle teleportStyle = new GUIStyle();
    protected float teleportPercentage = 1.0f;
    public int teleportCharging, teleportReady;

    public float detectionRange = 20.0f;
    protected List<WorldObject> nearbyObjects;

    //we want to restrict how many decisions are made to help with game performance
    //the default time at the moment is a tenth of a second
    private float timeSinceLastDecision = 0.0f, timeBetweenDecisions = 0.1f;
    
    
    public WorldObject tankPlayer;
    public List<WorldObject> tankEnemy;

    public int ObjectId { get; set; }
    public Rigidbody rigid;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        selectionBounds = ResourceManager.InvalidBounds;
        CalculateBounds();
    }

    protected virtual void Start()
    {
        player = transform.root.GetComponentInChildren<Player>();
        if (player) SetTeamColor();
    }

    protected virtual void Update()
    {
        if (ShouldMakeDecision()) DecideWhatToDo();
        currentWeaponChargeTime += Time.deltaTime;
        if (attacking && !movingIntoPosition && !aiming) PerformAttack();
    }

    protected virtual void OnGUI()
    {
        if (currentlySelected) DrawSelection();
    }

    private void DrawSelection()
    {
        GUI.skin = ResourceManager.SelectBoxSkin;
        Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
        //Draw the selection box around the currently selected object, within the bounds of the playing area
        GUI.BeginGroup(playingArea);
        DrawSelectionBox(selectBox);
        GUI.EndGroup();
    }
    public virtual void SetSelection(bool selected, Rect playingArea)
    {
        currentlySelected = selected;
        if (selected) this.playingArea = playingArea;
    }

    public string[] GetActions()
    {
        return actions;
    }

    public virtual void PerformAction(string actionToPerform)
    {
        //it is up to children with specific actions to determine what to do with each of those actions
    }

    public virtual void MouseClickMove(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        //only handle input if currently selected
        if (currentlySelected && hitObject && ((hitObject.name != "Ground") && (hitObject.name != "TeleportationGround")))
        {
            WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
            //clicked on another selectable object
            if (worldObject)
            {
                Player owner = hitObject.transform.root.GetComponent<Player>();
                if (owner)
                { //the object is controlled by a player
                    if (player && player.human)
                    { //this object is controlled by a human player
                      //start attack if object is not owned by the same player and this object can attack, else select
                        if (player.username != owner.username && CanAttack()) BeginAttack(worldObject);
                        else ChangeSelection(worldObject, controller);
                    }
                    else ChangeSelection(worldObject, controller);
                }
                else ChangeSelection(worldObject, controller);
            }
        }
    }

    public virtual void MouseClickTeleport(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        //only handle input if currently selected
        if (currentlySelected && hitObject && (hitObject.name != "Ground") && (hitObject.name != "TeleportationGround"))
        {
            WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
        }
    }

    private void ChangeSelection(WorldObject worldObject, Player controller)
    {
        //this should be called by the following line, but there is an outside chance it will not
        SetSelection(false, playingArea);
        if (controller.SelectedObject) controller.SelectedObject.SetSelection(false, playingArea);
        controller.SelectedObject = worldObject;
        worldObject.SetSelection(true, controller.hud.GetPlayingArea());
    }

    public virtual bool TeleportPossible() { return false; }


    public void CalculateBounds()
    {
        selectionBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            selectionBounds.Encapsulate(r.bounds);
        }
    }

    protected virtual void DrawSelectionBox(Rect selectBox)
    {
        GUI.Box(selectBox, "");
        CalculateCurrentHealth();
        GUI.Label(new Rect(selectBox.x, selectBox.y - 7, selectBox.width * healthPercentage, 5), "", healthStyle);
        CalculateTeleportCooldown();
    //    GUI.Label(new Rect(selectBox.x, selectBox.y    , selectBox.width * 100,              5), "", teleportStyle);
    }

    public virtual void SetHoverState(GameObject hoverObject)
    {
        //only handle input if owned by a human player and currently selected
        if (player && player.human && currentlySelected)
        {
            //something other than the ground is being hovered over
            if ((hoverObject.name != "Ground") && (hoverObject.name != "TeleportationGround"))
            {
                Player owner = hoverObject.transform.root.GetComponent<Player>();
                Unit unit = hoverObject.transform.parent.GetComponent<Unit>();
                Building building = hoverObject.transform.parent.GetComponent<Building>();
                if (owner)
                { //the object is owned by a player
                    if (owner.username == player.username) player.hud.SetCursorState(CursorState.Select);
                    else if (CanAttack()) player.hud.SetCursorState(CursorState.Attack);
                    else player.hud.SetCursorState(CursorState.Select);
                }
                else if (unit || building && CanAttack()) player.hud.SetCursorState(CursorState.Attack);
                else player.hud.SetCursorState(CursorState.Select);
            }
        }
    }

    public bool IsOwnedBy(Player owner)
    {
        if (player && player.Equals(owner))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual void CalculateCurrentHealth()
    {
        healthPercentage = (float)hitPoints / (float)maxHitPoints;
        if (healthPercentage > 0.65f) healthStyle.normal.background = ResourceManager.HealthyTexture;
        else if (healthPercentage > 0.35f) healthStyle.normal.background = ResourceManager.DamagedTexture;
        else healthStyle.normal.background = ResourceManager.CriticalTexture;
    }

    public virtual bool CanAttack()
    {
        //default behaviour needs to be overidden by children
        return false;
    }

    protected void SetTeamColor()
    {
        TeamColor[] teamColors = GetComponentsInChildren<TeamColor>();
        foreach (TeamColor teamColor in teamColors) teamColor.GetComponent<Renderer>().material.color = player.teamColor;
    }

    protected virtual void BeginAttack(WorldObject target)
    {
        this.target = target;
        if (TargetInRange())
        {
            attacking = true;
            PerformAttack();
        }
        else AdjustPosition();
    }

    private bool TargetInRange()
    {
        Vector3 targetLocation = target.GetComponent<Rigidbody>().transform.position;
        Vector3 direction = targetLocation - GetComponent<Rigidbody>().transform.position;
        if (direction.sqrMagnitude < weaponRange * weaponRange)
        {
            return true;
        }
        return false;
    }

    private void AdjustPosition()
    {
        Unit self = this as Unit;
        if (self)
        {
            movingIntoPosition = true;
            Vector3 attackPosition = FindNearestAttackPosition();
            self.StartMove(attackPosition);
            attacking = true;
        }
        else attacking = false;
    }

    private Vector3 FindNearestAttackPosition()
    {
        Vector3 targetLocation = target.transform.position;
        Vector3 direction = targetLocation - transform.position;
        float targetDistance = direction.magnitude;
        float distanceToTravel = targetDistance - (0.9f * weaponRange);
        return Vector3.Lerp(transform.position, targetLocation, distanceToTravel / targetDistance);
    }

    public void PerformAttack()
    {
        if (!target)
        {
            attacking = false;
            return;
        }
        if (!TargetInRange()) AdjustPosition();
        else if (!TargetInFrontOfWeapon()) AimAtTarget();
        else if (ReadyToFire()) UseWeapon();
    }

    private bool TargetInFrontOfWeapon()
    {
        Vector3 targetLocation = target.transform.position;
        Vector3 direction = targetLocation - transform.position;
        if (direction.normalized == transform.forward.normalized) return true;
        else return false;
    }

    protected virtual void AimAtTarget()
    {
        aiming = true;
        //this behaviour needs to be specified by a specific object
    }

    private bool ReadyToFire()
    {
        if (currentWeaponChargeTime >= weaponRechargeTime) return true;
        return false;
    }

    public virtual void UseWeapon()
    {
        currentWeaponChargeTime = 0.0f;
        //this behaviour needs to be specified by a specific object
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0) Destroy(gameObject);
    }

    public Bounds GetSelectionBounds()
    {
        return selectionBounds;
    }

    /**
 * A child class should only determine other conditions under which a decision should
 * not be made. This could be 'harvesting' for a harvester, for example. Alternatively,
 * an object that never has to make decisions could just return false.
 */
    protected virtual bool ShouldMakeDecision()
    {
        if (!attacking && !movingIntoPosition && !aiming)
        {
            //we are not doing anything at the moment
            if (timeSinceLastDecision > timeBetweenDecisions)
            {
                timeSinceLastDecision = 0.0f;
                return true;
            }
            timeSinceLastDecision += Time.deltaTime;
        }
        return false;
    }

    protected virtual void DecideWhatToDo()
    {
        //determine what should be done by the world object at the current point in time
        Vector3 currentPosition = transform.position;
            if (CanAttack())// && player.human == false)
            {
                if (Vector3.Distance(transform.position, tankPlayer.transform.position) < detectionRange)
                    BeginAttack(tankPlayer);
            }

            if (CanAttack() && player.human)
            {
                for (int i = 0; i <= tankEnemy.Count; i++)
                    if (Vector3.Distance(transform.position, tankEnemy[i].transform.position) < detectionRange)
                        BeginAttack(tankEnemy[i]);
            }
    }


    protected virtual void CalculateTeleportCooldown()
    {
      //  teleportPercentage = 1f;
      //  teleportPercentage = (float)teleportCharging / (float)teleportReady;
       // if (teleportPercentage == 1f) 
        //    teleportStyle.normal.background = ResourceManager.ReadyTexture;
      //  else  teleportStyle.normal.background = ResourceManager.ChargingTexture;
    }

    public Player GetPlayer()
    {
        return player;
    }
}
