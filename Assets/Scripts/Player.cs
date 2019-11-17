using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

    Rigidbody2D thisRigidbody2D;
    LineRenderer lineRenderer;

    bool aiming;
    bool falling;
    Vector2 startForce;

    public bool showTrajectory;
    [Range(2,4000)]public int trajectorySegmentCount;

    private void Start()
    {
        thisRigidbody2D = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        startForce = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (!falling)
        {
            if (Input.GetMouseButton(0))
            {
                aiming = true;
                lineRenderer.enabled = true;
            } 
            if (Input.GetMouseButtonUp(0))
            {
                falling = true;
                aiming = false;
                thisRigidbody2D.velocity = startForce;
                thisRigidbody2D.constraints = RigidbodyConstraints2D.None;
            }
        }       
    }

    private void FixedUpdate()
    {
        if (aiming)
        {
            Calculations.Trajectory(thisRigidbody2D ,transform.position, startForce, ref lineRenderer, trajectorySegmentCount);
        }
        else if (falling)
        {
            thisRigidbody2D.AddForce(Calculations.Gravity(transform.position, thisRigidbody2D.mass) * thisRigidbody2D.mass);
            Calculations.Trajectory(thisRigidbody2D ,transform.position, thisRigidbody2D.velocity, ref lineRenderer, trajectorySegmentCount);         
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        falling = false;
        lineRenderer.enabled = false;
        thisRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
