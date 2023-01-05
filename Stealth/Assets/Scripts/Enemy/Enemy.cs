using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // Components
    private NavMeshAgent agent;
    private Animator animator;
    private FieldOfView fieldOfView;

    // Walking information
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private float minimumDistance = 0.5f;
    [SerializeField] private Transform[] path;
    private int pathIndex = 0;

    // Chase information
    private bool chasingPlayer = false;
    private bool standStill = false;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float visionTime = 2f;
    [SerializeField] private Image exlamationMark;
    private float currentVisionTime;

    // Afk information
    private bool canAfk;
    [SerializeField] private int afkChance = 5;
    [SerializeField] private GameObject phone;

    // Stun information
    [SerializeField] private bool canBeStunned;
    private bool isStunned;
    [SerializeField] private GameObject displayAsSelectedObj;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        fieldOfView = GetComponent<FieldOfView>();
        visionTime *= GameObject.Find("Player").GetComponent<PlayerMovement>().getVisibilityMultiplier();
        currentVisionTime = visionTime;
        visionTime /= 2f;
        if(path.Length > 0 && agent.isOnNavMesh)
            waypoint(1);

        StartCoroutine(checkForPlayer());
    }

    void FixedUpdate()
    {
        if (!agent.isOnNavMesh || isStunned || standStill)
            return;

        // Check if remaining distance towards the target is smaller than minimum distance
        if(agent.remainingDistance <= minimumDistance && !agent.pathPending)
        {
            // If AI is going down set path, set next waypoint
            if (!chasingPlayer)
            {
                agent.SetDestination(transform.position);
                animator.SetInteger("state", 0);
                if (Random.Range(1, 101) <= afkChance && canAfk)
                    waypointAfk();
                else waypoint(1);
            }
            else
            {
                // Player in range, catch
                if (Physics.OverlapSphere(transform.position, minimumDistance * 2, playerMask).Length != 0)
                {
                    animator.SetInteger("state", 3);
                }
                // Player escaped
                else
                {
                    currentVisionTime = visionTime;

                    chasingPlayer = false;
                    exlamationMark.gameObject.SetActive(false);

                    agent.speed = walkSpeed;
                    waypoint(0);
                }
            }
        }
    }
    
    // Go to next waypoint
    private void waypoint(int nextPath)
    {
        animator.SetInteger("state", 1);
        agent.SetDestination(path[pathIndex].position);
        pathIndex += nextPath;

        if (pathIndex >= path.Length)
            pathIndex = 0;

        canAfk = true;
    }

    // Do animation at waypoint
    private void waypointAfk()
    {
        canAfk = false;
        standStill = true;
        switch (Random.Range(1, 4))
        {
            case 1:
                animator.SetInteger("state", 4);
                fieldOfView.changeViewRadius(0);
                break;
            case 2:
                animator.SetInteger("state", 5);
                fieldOfView.changeViewRadius(1);
                break;
            default:
                animator.SetInteger("state", 6);
                break;
        }
        
    }

    // AFK animation finnished
    private void finnishAfk()
    {
        fieldOfView.changeViewRadius(2);
        standStill = false;
    }

    // Show phone in a hand
    private void showPhone(int show)
    {
        if (show == 0)
            phone.SetActive(true);
        else phone.SetActive(false);
    }

    // Check for player at interval of 0.1s
    private IEnumerator checkForPlayer()
    {
        while (true)
        {
            Vector3 playerPosition = fieldOfView.checkForPlayer();
            if (playerPosition != new Vector3())
            {
                if (!standStill && !chasingPlayer)
                {
                    // stand still and show that player is visible
                    standStill = true;

                    animator.SetInteger("state", 0);
                    agent.SetDestination(transform.position);

                    exlamationMark.gameObject.SetActive(true);
                    exlamationMark.color = Color.black;
                }
                if (currentVisionTime <= 0f)
                {
                    // chase player to the last seen location
                    standStill = false;
                    chasingPlayer = true;

                    agent.speed = runSpeed;
                    agent.SetDestination(playerPosition);
                    animator.SetInteger("state", 2);

                    exlamationMark.color = Color.white;
                }
                else
                {
                    currentVisionTime -= 0.2f;
                }
            }
            else if (standStill && exlamationMark.gameObject.activeSelf == true)
            {
                // player is no longer visible
                exlamationMark.gameObject.SetActive(false);
                standStill = false;
                currentVisionTime = visionTime;
                waypoint(0);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Heard a sound at location
    public void hearSound(Vector3 position)
    {
        fieldOfView.changeViewRadius(2);
        if (!chasingPlayer && !isStunned)
        {
            agent.SetDestination(position);
            animator.SetInteger("state", 1);

            exlamationMark.color = Color.black;
            exlamationMark.gameObject.SetActive(true);
            currentVisionTime = 0;
            chasingPlayer = true;
        }
    }

    // Give information to player. Caught the player
    private void playerCaught()
    {
        GameObject.Find("Player").GetComponent<PlayerMovement>().gotCaught(0);
    }

    // Show a circle around character ( for stun skill)
    public void displayAsSelected(bool isSelected)
    {
        displayAsSelectedObj.SetActive(isSelected);
    }

    // Gives information, if this character can be stunned
    public bool canEnemyBeStunned()
    {
        return canBeStunned;
    }
    
    // Stun character
    public void stunEnemy()
    {
        if (canBeStunned)
        {
            agent.SetDestination(transform.position);
            StartCoroutine(stunCoroutine());
        }
    }

    // Set character as stunned and after 30s wake up
    IEnumerator stunCoroutine()
    {
        animator.SetInteger("state", 10);
        isStunned = true;
        fieldOfView.changeViewRadius(0);

        yield return new WaitForSeconds(30);

        animator.SetInteger("state", 10);
        yield return new WaitForSeconds(1);
        fieldOfView.changeViewRadius(2);
        isStunned = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, transform.forward * minimumDistance));
    }
}
