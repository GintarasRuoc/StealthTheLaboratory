using UnityEngine;
using UnityEngine.InputSystem;

public class GameMasterController : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private QuestManager questManager;
    private PlayerInput inputs;

    [SerializeField] private GameObject player;
    [SerializeField] private UIManager uIManager;

    int playerLocation;
    void Awake()
    {
        inputs = GetComponent<PlayerInput>();
        if (inputs != null)
        {
            string rebinds = PlayerPrefs.GetString("rebinds", string.Empty);
            if (!string.IsNullOrEmpty(rebinds))
                inputs.actions.LoadBindingOverridesFromJson(rebinds);
        }
        else Debug.Log("Player input not set for GameMasterController");
        inputs.actions.FindActionMap("Player").Disable();
        inputs.actions.FindActionMap("SkillUse").Disable();

        Time.timeScale = 0f;

        prepareMap();
        preparePlayer();
        prepareQuests();
        uIManager.setUpInteractInfo(inputs);
    }

    private void prepareMap()
    {
        mapGenerator = GetComponent<MapGenerator>();
        if (mapGenerator != null)
            playerLocation = mapGenerator.startGeneratingMap();
        else Debug.Log("GameManager // Map generator not found!");
    }

    private void preparePlayer()
    {
        player.transform.position = new Vector3(playerLocation, 0f, playerLocation);
        player.transform.rotation = new Quaternion();
        player.GetComponent<PassiveSkill>().setUpPassiveSkill();
        player.GetComponent<ActiveSkill>().setUpActiveSkill();
    }

    private void prepareQuests()
    {
        questManager = GetComponent<QuestManager>();
        questManager.spawnQuests(mapGenerator.getSpawnedRooms(), playerLocation);
    }

    public void endGame(bool isWin)
    {
        if (isWin)
        {
            if (questManager.isQuestDone(true)){
                Time.timeScale = 0f;
                inputs.actions.FindActionMap("Player").Disable();
                inputs.actions.FindActionMap("SkillUse").Disable();
                if (questManager.isQuestDone(false))
                {
                    uIManager.finnishGame(true, questManager.getSideQuestReward());
                    PlayerPrefs.SetInt("knowledgePoints", PlayerPrefs.GetInt("knowledgePoints") + questManager.getSideQuestReward());
                }
                else uIManager.finnishGame(true, 0);
            }
            Debug.Log("You won the game");
        }
        else
        {
            Time.timeScale = 0f;
            inputs.actions.FindActionMap("Player").Disable();
            inputs.actions.FindActionMap("SkillUse").Disable();
            uIManager.finnishGame(false, 0);
            Debug.Log("You lost the game");
        }
    }
}
