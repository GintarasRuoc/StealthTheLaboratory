using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject skillMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject loadingPanel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            PlayerPrefs.DeleteAll();
    }

    public void startGame()
    {
        PlayerPrefs.Save();
        loadingPanel.SetActive(true);
        SceneManager.LoadScene(1);
    }

    public void showSkillMenu(bool active)
    {
        mainMenuPanel.SetActive(!active);
        skillMenuPanel.SetActive(active);
    }

    public void showOptionsMenu(bool active)
    {
        mainMenuPanel.SetActive(!active);
        optionsMenuPanel.SetActive(active);
    }

    public void quitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}
