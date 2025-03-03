﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float vel = 10f;
    private Rigidbody2D rb;
    private float angle_shot;
    private float angle;
    private int bullet_damage = 20;

    public float activation_period = 0.03f;
    private float activation_moment;
    public int tank_number;

    private int maxBounces = 3;
    private int timesBounced;

    LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        angle = angle_shot * Mathf.Deg2Rad;
        activation_moment = Time.time + activation_period;
        mask = LayerMask.GetMask("Wall");
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            rb.velocity = new Vector2(Mathf.Cos(angle) * vel, Mathf.Sin(angle) * vel);

            // Ricochet effect
            //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y) + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.15f, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), Color.red);

            // Create RAY
            Ray ray = new Ray(new Vector2(transform.position.x, transform.position.y) + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.25f, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));

            // Create RAY HIT
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.25f, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));

            // Check if the ray hits something
            if (hit)
            {
                // Check if the ray hits a wall within a distance of 0.15f
                if (hit.distance < 0.15f && hit.transform.tag == "Wall")
                {
                    if (timesBounced >= maxBounces) Destroy(gameObject);
                    else timesBounced++;

                    // Reflect bullet
                    Vector2 reflectDir = Vector2.Reflect(ray.direction, hit.normal);
                    angle = Mathf.Atan2(reflectDir.y, reflectDir.x);
                    transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
                    
                    if (hit.transform.name == "WoodCrate(Clone)")
                    {
                        Wall wallscript = hit.transform.gameObject.GetComponent<Wall>();

                        wallscript.Hit(bullet_damage); // Hit wall with 10 damage
                    }
                }
                else if (hit.transform.tag == "Player" && hit.distance < 0.05f)
                {
                    PlayerController playerscript = hit.transform.gameObject.GetComponent<PlayerController>();
                    if ((Time.time > activation_moment) || playerscript.tank_number != tank_number)
                    {
                        playerscript.Hit(bullet_damage, tank_number); // Hit player with 10 damage
                        Destroy(gameObject); // Destroy Bullet
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    public void setTankNumber(int tanknr)
    {
        tank_number = tanknr;
        string player_b = "Barrel " + tanknr;
        PlayerGun playerGun = GameObject.Find(player_b).GetComponent<PlayerGun>();
        angle_shot = playerGun.angle;
    }
}
