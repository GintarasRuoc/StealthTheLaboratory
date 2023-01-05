using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class ActiveSkill : MonoBehaviour
{
    private const string activeSkillPref = "activeSkill";

    private UIManager uIManager;
    private int activeSkillId = -1;
    private int amount = 0;

    private PlayerInput inputs;
    private PlayerMovement player;
    private SkinnedMeshRenderer[] renderer;
    private BoxCollider[] playerColliders;

    [SerializeField] private Image activeSkillIcon;
    [SerializeField] private Text countdownText;

    [SerializeField] private int MovementSpeedMultiplier = 25;

    private bool isCoinActive = false;
    [SerializeField] private Coin coinPrefab;
    [SerializeField] private Transform coinTarget;

    private bool isStunActive = false;
    private Enemy selectedEnemy;
    [SerializeField] private float stunRange = 20f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Text explanationText;
    [SerializeField] private Text cantStunText;

    private bool needToDisableEffect = false;

    private void Update()
    {
        // If coin skill active, show coin position in scene
        if (isCoinActive)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50))
            {
                Vector3 pos = hit.point;
                pos.y = 0.1f;
                coinTarget.position = pos;
            }
        }
        // If stun skill active, show moused over enemy
        if (isStunActive)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, stunRange, enemyLayer))
            {
                if (selectedEnemy != hit.collider.GetComponent<Enemy>())
                {
                    if (selectedEnemy != null)
                        selectedEnemy.displayAsSelected(false);

                    selectedEnemy = hit.collider.GetComponent<Enemy>();
                    selectedEnemy.displayAsSelected(true);
                }
            }
            else if (selectedEnemy != null)
            {
                selectedEnemy.displayAsSelected(false);
                selectedEnemy = null;
            }
        }
    }

    // Show, which active skill is selected in main menu
    public void setUpActiveSkill()
    {
        inputs = GameObject.Find("GameManager").GetComponent<PlayerInput>();
        uIManager = GameObject.Find("InterfaceManager").GetComponent<UIManager>();
        if(PlayerPrefs.HasKey(activeSkillPref) && PlayerPrefs.GetInt(activeSkillPref) >= 0)
            activeSkillId = PlayerPrefs.GetInt(activeSkillPref);
        setActiveSkill();

        player = GetComponent<PlayerMovement>();
    }

    // Set skill amount of uses
    private void setActiveSkill()
    {
        switch (activeSkillId)
        {
            case 0:
                amount = 3;
                break;
            case 1:
                amount = 2;
                break;
            case 2:
                amount = 5;
                break;
            case 3:
                amount = 3;
                break;
            default:
                break;
        }

        uIManager.setUpActiveSkill(activeSkillId, amount, inputs.actions.FindAction("activeSkill").bindings[0].ToDisplayString());
    }

    // inputs
    // Player pressed skill use button
    public void activateSkll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (amount > 0 && !needToDisableEffect)
            {
                switchInputMap(true);
                switch (activeSkillId)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        isCoinActive = true;
                        coinTarget.gameObject.SetActive(true);
                        break;
                    case 3:
                        isStunActive = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Player confirmed skill use
    public void useSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (activeSkillId)
            {
                case 0:
                    player.upgradeMovement(MovementSpeedMultiplier);
                    needToDisableEffect = true;
                    amount--;
                    uIManager.usedSkill();
                    StartCoroutine(disableSkillEffect(5));
                    break;
                case 1:
                    if (renderer == null)
                        renderer = GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer mesh in renderer)
                        mesh.enabled = false;
                    if (playerColliders == null)
                        playerColliders = GetComponents<BoxCollider>();
                    foreach (BoxCollider col in playerColliders)
                        col.enabled = false;
                    needToDisableEffect = true;
                    amount--;
                    uIManager.usedSkill();
                    StartCoroutine(disableSkillEffect(5));
                    break;
                case 2:
                    player.throwAnimation();
                    isCoinActive = false;
                    coinTarget.gameObject.SetActive(false);
                    break;
                case 3:
                    if (selectedEnemy != null)
                    {
                        if (selectedEnemy.canEnemyBeStunned())
                        {
                            player.throwAnimation();
                            
                            isStunActive = false;
                        }
                        else StartCoroutine(showError(false));
                    }
                    else StartCoroutine(showError(true));
                    break;
                default:
                    break;
            }
            if(!isStunActive)
                switchInputMap(false);
        }
    }

    // If skill needs to be disabled, disable after timer run out ( movement, invisibilty skills)
    IEnumerator disableSkillEffect(float waitTime)
    {
        activeSkillIcon.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);
        while (waitTime != 0)
        {
            countdownText.text = waitTime.ToString();
            waitTime--;
            yield return new WaitForSeconds(1);
        }

        if (activeSkillId == 0)
            player.upgradeMovement(0);
        else if (activeSkillId == 1)
        {
            foreach (SkinnedMeshRenderer mesh in renderer)
                mesh.enabled = true;
            foreach (BoxCollider col in playerColliders)
                col.enabled = true;
        }

        activeSkillIcon.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(false);
        needToDisableEffect = false;
    }

    // If skill was used incorrectly, show error ( coins, rocks skills)
    IEnumerator showError(bool notSelected)
    {
        if (notSelected)
        {
            explanationText.gameObject.SetActive(true);
            cantStunText.gameObject.SetActive(false);
        }
        else
        {
            explanationText.gameObject.SetActive(false);
            cantStunText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(5);

        explanationText.gameObject.SetActive(false);
        cantStunText.gameObject.SetActive(false);
    }
     
    // Player canceled skill use
    public void cancelSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (activeSkillId)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    isCoinActive = false;
                    coinTarget.gameObject.SetActive(false);
                    break;
                case 3:
                    isStunActive = false;
                    if (selectedEnemy != null)
                        selectedEnemy.displayAsSelected(false);
                    selectedEnemy = null;
                    break;
                default:
                    break;
            }
            switchInputMap(false);

        }
    }

    // Change input map
    private void switchInputMap(bool changeToSkillMap)
    {
        if (changeToSkillMap)
        {
            inputs.actions.FindActionMap("Player").Disable();
            inputs.actions.FindActionMap("SkillUse").Enable();
            uIManager.showSkillUseText(true);
            
        }
        else
        {
            inputs.actions.FindActionMap("SkillUse").Disable();
            inputs.actions.FindActionMap("Player").Enable();
            uIManager.showSkillUseText(false);
        }
    }

    // Activate skill effect after animation
    private void useSkillAfterAnimation()
    {
        if (activeSkillId == 2)
            throwCoin();
        else stunEnemy();

        amount--;
        uIManager.usedSkill();
    }

    // Emit sound at selected point after animation
    private void throwCoin()
    {
        Coin spawned = Instantiate(coinPrefab, coinTarget.position, new Quaternion());
    }

    // Stun selected enemy after animation
    private void stunEnemy()
    {
        selectedEnemy.stunEnemy();
        selectedEnemy.displayAsSelected(false);
        selectedEnemy = null;
    }
}
