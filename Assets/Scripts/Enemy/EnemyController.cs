using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody theRB;

    private bool chasing;
    public float distanceToChase = 10f;
    public float distanceToLose = 15f;
    public float distanceToStop = 2f;

    private Vector3 targetPoint;
    private Vector3 startpoint;
    public NavMeshAgent agent;

    public float keepChasingTime = 5f;
    private float chaseCounter;


    // Start is called before the first frame update
    void Start()
    {
        startpoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;

        if(!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
                chasing = true;

            if(chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;
                if (chaseCounter < 0)
                    agent.destination = startpoint;
            }
            
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
           
        }

        
    }
}
