using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    private bool chasing;
    public float distanceToChase = 10f;
    public float distanceToLose = 15f;
    public float distanceToStop = 2f;

    private Vector3 targetPoint;
    private Vector3 startpoint;
    public NavMeshAgent agent;

    public float keepChasingTime = 5f;
    private float chaseCounter;

    public GameObject bullet;
    public Transform firePoint;

    public float fireRate;
    public float waitBetweenShots;
    public float timeToShoot = 1f;
    private float fireCount;
    private float shotWaitCounter;
    private float shootTimeCounter;
    // Start is called before the first frame update
    void Start()
    {
        startpoint = transform.position;
        shootTimeCounter = timeToShoot;
        shotWaitCounter = waitBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;

        if(!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                chasing = true;
                shootTimeCounter = timeToShoot;
                shotWaitCounter = waitBetweenShots;
            }

            if(chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;
                if (chaseCounter < 0)
                    agent.destination = startpoint;
            }

            if(agent.remainingDistance < 0.25f)
                animator.SetBool("isMoving", false);
            else
                animator.SetBool("isMoving", true);
        }
        else
        {
            //transform.LookAt(targetPoint);
            //theRB.velocity = transform.forward * moveSpeed;
            if (Vector3.Distance(transform.position, targetPoint) > distanceToStop)
                agent.destination = targetPoint;
            else
                agent.destination = transform.position;

            if (Vector3.Distance(transform.position, targetPoint) > distanceToLose)
            {
                chasing = false;
                chaseCounter = keepChasingTime;
            }
            if(shotWaitCounter > 0)
            {
                shotWaitCounter -= Time.deltaTime;
                if(shotWaitCounter <= 0)
                {
                    shootTimeCounter = timeToShoot;
                }
                animator.SetBool("isMoving", true);
            }
            else
            {
                shootTimeCounter -= Time.deltaTime;

                if (shootTimeCounter > 0)
                {
                    fireCount -= Time.deltaTime;
                    if (fireCount <= 0)
                    {
                        fireCount = fireRate;
                        firePoint.LookAt(PlayerController.instance.transform.position);

                        float angle = CheckAngleToPlayer();
                        if (Mathf.Abs(angle) < 30)
                        {
                            Instantiate(bullet, firePoint.position, firePoint.rotation);
                            animator.SetTrigger("fireShot");
                        }
                           
                        else
                            shotWaitCounter = waitBetweenShots;
                    }
                    agent.destination = transform.position;
                }

                else
                {
                    shotWaitCounter = waitBetweenShots;
                }
                animator.SetBool("isMoving", false);
            }

            
        }

        
    }

    private float CheckAngleToPlayer()
    {
        Vector3 targetDir= PlayerController.instance.transform.position - transform.position;
        return Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
        

    }
}
