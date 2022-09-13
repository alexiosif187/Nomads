using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Tank2 : Unit
{

    private Quaternion aimRotation;
    public Tank2Trigger Tank2Trigger;
    protected override void Start()
    {
        rigid = GetComponent<Rigidbody>();
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (aiming)
        {
            GetComponent<Rigidbody>().transform.rotation = Quaternion.RotateTowards(GetComponent<Rigidbody>().transform.rotation, aimRotation, weaponAimSpeed);
            CalculateBounds();
            //sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
            Quaternion inverseAimRotation = new Quaternion(-aimRotation.x, -aimRotation.y, -aimRotation.z, -aimRotation.w);
            if (transform.rotation == aimRotation || transform.rotation == inverseAimRotation)
            {
                aiming = false;
            }
        }
        stop(Tank2Trigger);
    }

    public override bool CanAttack()
    {
        return true;
    }

    protected override void AimAtTarget()
    {
        base.AimAtTarget();
        aimRotation = Quaternion.LookRotation(target.transform.position - transform.position);
    }

    public override void UseWeapon()
    {
        base.UseWeapon();
        this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip);
        Vector3 spawnPoint = transform.position;
        spawnPoint.x += (2.1f * transform.forward.x);
        spawnPoint.y += 1.4f;
        spawnPoint.z += (2.1f * transform.forward.z);
        GameObject gameObject = (GameObject)Instantiate(ResourceManager.GetWorldObject("TankProjectile"), spawnPoint, transform.rotation);
        TankProjectile projectile = gameObject.GetComponentInChildren<TankProjectile>();
        projectile.SetRange(0.9f * weaponRange);
        projectile.SetTarget(target);
    }

    protected void stop(Tank2Trigger Tank2Trigger)
    {
        if (Tank2Trigger.hitPoints == 0)
            this.moveSpeed=0;
            
    }


}
