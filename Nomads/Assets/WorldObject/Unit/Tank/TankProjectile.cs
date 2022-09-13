using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class TankProjectile : MonoBehaviour
{

    public float velocity = 10;
    public int damage = 100;

    private float range = 70;
    private WorldObject target;

    public void Update()
    {
        if (HitSomething())
        {
            InflictDamage();
            Destroy(gameObject);
        }
        if (range > 0)
        {
            float positionChange = Time.deltaTime * velocity * 10;
            range -= positionChange;
            transform.position += (positionChange * transform.forward);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetRange(float range)
    {
        this.range = range;
    }

    public void SetTarget(WorldObject target)
    {
        this.target = target;
    }

    private bool HitSomething()
    {
        if (target && target.GetSelectionBounds().Contains(transform.position)) return true;
        return false;
    }

    private void InflictDamage()
    {
        if (target) target.TakeDamage(damage);
    }
}
