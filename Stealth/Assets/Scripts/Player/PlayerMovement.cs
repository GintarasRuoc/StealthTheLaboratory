using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private GameMasterController gameMaster;
    private NavMeshAgent agent;
    private Animator animator;

    private bool moved = false;
    private bool isWalking = true;

    [SerializeField] private float distanceBeforeTryingToHide = 0.1f;
    [SerializeField] private float maxDistanceBetweenPlayerAndHidableObject = 1f;
    // Layer mask of objects, that can player hide behind
    [SerializeField] private LayerMask hidableObjectsLayerMask;
    
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float currentWalkSpeed;
    [SerializeField] private float currentRunSpeed;

    private const string standHideTag = "StandingHide";
    [SerializeField] private BoxCollider topBoxCollider;

    // Interact values
    private const string questTag = "Quest";
    private const string exitTag = "Exit";
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private LayerMask interactLayer;

    [SerializeField] private float runSoundRange = 5f;
    [SerializeField] private LayerMask enemyLayerMask;

    [SerializeField] private float visibiltyMultiplier = 1f;

    void Awake()
    {
        Camera.main.gameObject.GetComponent<CameraMovement>().setPlayer(transform);
        gameMaster = GameObject.Find("GameManager").GetComponent<GameMasterController>();

        currentWalkSpeed = walkSpeed;
        currentRunSpeed = runSpeed;
        interactLayer = ~interactLayer;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            gotCaught(0);
        if (!moved)
            return;

        float destinationDistanace = (transform.position - agent.destination).magnitude;
        if (destinationDistanace <= distanceBeforeTryingToHide)
        {
            agent.SetDestination(transform.position);
            animator.SetInteger("state", 0);
            moved = false;
            tryHiding();
        }
        if(!isWalking)
            emitSound();
    }

    // Player controlled character tries to hide behind the object. Firstly, script gets all objects that are close to a player. 
    // Second, program finds closest object. Then checks objects tag and plays appropiate animation
    private void tryHiding()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistanceBetweenPlayerAndHidableObject, hidableObjectsLayerMask);

        if (colliders.Length > 0)
        {
            Collider closestObject = null;
            float lowestDistance = Mathf.Infinity;
            foreach (Collider temp in colliders)
            {
                float tempDistance = (temp.transform.position - transform.position).magnitude;
                if (tempDistance < lowestDistance)
                {
                    lowestDistance = tempDistance;
                    closestObject = temp;
                }
            }
            Debug.Log(closestObject);
            if (closestObject != null)
            {
                if (closestObject.tag.Equals(standHideTag))
                    animator.SetInteger("state", 2);
                else
                {
                    animator.SetInteger("state", 1);
                    topBoxCollider.enabled = false;
                }
            }
            else animator.SetInteger("state", 0);
        }
        else animator.SetInteger("state", 0);
    }

    // If player is running, notify enemies in range
    private void emitSound()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, runSoundRange, enemyLayerMask);
        if(colliders.Length > 0)
            foreach (Collider col in colliders)
                col.GetComponent<Enemy>().hearSound(transform.position);
    }

    // Player got caught by enemy
    public void gotCaught(int playedAnimation)
    {
        if(animator != null)
            if (playedAnimation == 0)
            {
                if (animator.GetInteger("state") != 10)
                    animator.SetInteger("state", 10);
            }
            else gameMaster.endGame(false);
    }

    // Upgrade movement speed
    public void upgradeMovement(int buff)
    {
        if (buff > 0)
        {
            currentRunSpeed += runSpeed * buff / 100;
            currentWalkSpeed += walkSpeed * buff / 100;
        }
        else
        {
            currentRunSpeed = runSpeed;
            currentWalkSpeed = walkSpeed;
        }
        if (agent != null)
            if (!isWalking)
                agent.speed = currentRunSpeed;
            else
                agent.speed = currentWalkSpeed;
    }

    // Quieter running ( passive skill)
    public void quieterRun(int buff)
    {
        runSoundRange *= buff / 100;
    }

    // Give player visibility multiplier
    public float getVisibilityMultiplier()
    {
        return visibiltyMultiplier;
    }

    // Upgrade player visibility (pasive skill)
    public void upgradeVisbility(float buff)
    {
        visibiltyMultiplier = buff;
    }

    // Play throw animation
    public bool throwAnimation()
    {
        if (animator != null)
        {
            agent.SetDestination(transform.position);
            animator.SetInteger("state", 4);
            return true;
        }
        return false;
    }

    // Player input calls
    // Player called for character to move to a position
    public void goTo(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                agent.speed = currentWalkSpeed;
                agent.angularSpeed = 360f;
            }
            if (animator == null)
                animator = GetComponent<Animator>();
            if (topBoxCollider.enabled == false)
                topBoxCollider.enabled = true;

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, interactLayer))
            {
                if (isWalking)
                    animator.SetInteger("state", -1);
                else animator.SetInteger("state", -2);
                Vector3 destination = hit.point;
                destination.y = 0f;
                agent.SetDestination(destination);
                moved = true;
            }
        }
    }

    // Player changed movement type ( walk, run)
    public void changeMovementSpeed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isWalking)
            {
                isWalking = !isWalking;
                agent.speed = currentRunSpeed;
                animator.SetInteger("state", -2);
            }
            else
            {
                isWalking = !isWalking;
                agent.speed = currentWalkSpeed;
                animator.SetInteger("state", -1);
            }
        }
    }

    // Player pressed interact button
    public void interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, ~interactLayer);
            Debug.Log("Collders length " + colliders.Length);
            foreach (Collider col in colliders)
            {
                Debug.Log("col - " + col.tag);
                if (col.tag.Equals(questTag))
                {
                    col.transform.parent.GetComponent<TaskPrefab>().completeQuest();
                    break;
                }
                else if (col.tag.Equals(exitTag))
                {
                    gameMaster.endGame(true);
                    break;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Display the hide radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, runSoundRange);
    }
}
