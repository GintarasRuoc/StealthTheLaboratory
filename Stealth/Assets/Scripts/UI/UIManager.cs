using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private UISkill skillUI;

    [SerializeField] private UITask mainQuestUI;
    [SerializeField] private UITask sideQuestUI;

    [SerializeField] private Text interactButtonText;

    [SerializeField] private GameObject skillInfoObject;
    [SerializeField] private Text skillConfirmText;
    [SerializeField] private Text skillCancelText;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text rewardText;
    [SerializeField] private GameObject losePanel;

    private PlayerInput input;

    public void setUpInteractInfo(PlayerInput _input)
    {
        input = _input;

        interactButtonText.text = "Press " + input.actions.FindAction("interact").bindings[0].ToDisplayString() + " to interact";
        skillConfirmText.text = "Press " + input.actions.FindAction("activate").bindings[0].ToDisplayString() + " to activate skill";
        skillCancelText.text = "Press " + input.actions.FindAction("cancel").bindings[0].ToDisplayString() + " to cancel skill";

        loadingScreen.SetActive(false);
    }

    // Show selected active skill in UI
    public void setUpActiveSkill(int skill, int skillAmount, string action)
    {
        skillUI.setUpActiveSkill(skill, skillAmount, action);
    }

    // Show available quest in UI
    public void setUpQuests(Quest mainQuest, Quest sideQuest)
    {
        mainQuestUI.setUpQuest(mainQuest);
        sideQuestUI.setUpQuest(sideQuest);
    }

    // Show quest as completed
    public void completeQuest(bool isMain)
    {
        if (isMain)
            mainQuestUI.questDone();
        else sideQuestUI.questDone();
    }

    // Show interact button
    public void showInteractButton(bool show)
    {
        if(interactButtonText != null)
        {
            interactButtonText.gameObject.SetActive(show);
        }
    }

    // Show information about skill use buttons
    public void showSkillUseText(bool show)
    {
        if (skillInfoObject != null)
        {
            skillInfoObject.SetActive(show);
        }
    }

    // Update active skill amount of uses left
    public void usedSkill()
    {
        skillUI.usedSkill();
    }

    // Display finnished game tab
    public void finnishGame(bool isWin, int reward)
    {
        if (isWin)
        {
            if (reward != 0)
            {
                rewardText.text = "You gained " + reward + " knowledge points!";
                rewardText.gameObject.SetActive(true);
            }
            winPanel.SetActive(true);
        }
        else losePanel.SetActive(true);
    }

    // Buttons

    public void startGame()
    {
        input.actions.FindActionMap("Player").Enable();
        Time.timeScale = 1f;
    }

    public void pauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            input.actions.FindActionMap("Player").Disable();
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
    }

    public void backToGame()
    {
        input.actions.FindActionMap("Player").Enable();
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void mainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
