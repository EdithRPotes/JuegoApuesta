using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private BombProjectile bombProjectilePrefab;
    [SerializeField] private Transform shootPosition;
    [SerializeField, Range(1f, 20f)] private float rotationSpeed;

    private Camera cam;
    private bool isAiming = false;
    private float launchPower = 0f;
    private bool hasFired = false;
  


    private GameManager gameManager;

    private void Start()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        enabled = false;
        
    }

    void Update()
    {
        Vector2 mouseWorldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPoint - (Vector2)transform.position;
        transform.up = Vector2.MoveTowards(transform.up, direction, rotationSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(0) && !hasFired )
        {
            isAiming = true;
        }

        if (Input.GetMouseButton(0) && isAiming)
        {
            launchPower = Mathf.Clamp(launchPower + Time.deltaTime * 10f, 0f, 20f);
            //Debug.Log("Launch Power: " + launchPower);
        }

        if (Input.GetMouseButtonUp(0) && isAiming && enabled )
        {
            isAiming = false;
            hasFired = true;
            BombProjectile projectile = Instantiate(bombProjectilePrefab, shootPosition.position, transform.rotation);
            projectile.LaunchBombProjectile(transform.up);
            

        }
    }

    
}


