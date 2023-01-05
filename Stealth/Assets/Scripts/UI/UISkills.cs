using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UISkills : MonoBehaviour
{
    [SerializeField] private Sprite baseSkillIcon;
    [SerializeField] private Sprite selectedSkillIcon;
    [SerializeField] private Sprite preSelectedSkillIcon;
    [SerializeField] private Sprite lockedSkillIcon;

    [SerializeField] private Skill[] passiveSkills;
    [SerializeField] private Skill[] activeSkills;

    [SerializeField] private Text knowledgePointsText;

    private string unlockingSkill;
    [SerializeField] private GameObject unlockPopUp;
    [SerializeField] private Text skillNameHolder;
    [SerializeField] private Text skillCostHolder;
    [SerializeField] private GameObject errorPopUp;

    private int oldPassiveSkill = -1;
    private int oldActiveSkill = -1;
    private int selectedPassiveSkill = -1;
    private int selectedActiveSkill = -1;

    [SerializeField] private string activeSkillPref = "activeSkill";
    [SerializeField] private string passiveSkillPref = "passiveSkill";
    [SerializeField] private string knowledgePointsPref = "knowledgePoints";

    private void Awake()
    {
        // Testing code
        if (!PlayerPrefs.HasKey("test"))
        {
            PlayerPrefs.SetInt("test", 1);
            PlayerPrefs.SetInt(activeSkillPref, 2);

            PlayerPrefs.SetInt(activeSkills[2].getName(), 1);
            PlayerPrefs.SetInt(activeSkills[3].getName(), 1);
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        // Check, if skills are unlocked
        setUpPassiveSkills(passiveSkills[0].getName(), 0);
        setUpPassiveSkills(passiveSkills[1].getName(), 1);
        setUpPassiveSkills(passiveSkills[2].getName(), 2);
        setUpPassiveSkills(passiveSkills[3].getName(), 3);
        setUpActiveSkills(activeSkills[0].getName(), 0);
        setUpActiveSkills(activeSkills[1].getName(), 1);
        setUpActiveSkills(activeSkills[2].getName(), 2);
        setUpActiveSkills(activeSkills[3].getName(), 3);

        // If selected passive skill exists, display it
        if (PlayerPrefs.HasKey(passiveSkillPref) && PlayerPrefs.GetInt(passiveSkillPref) >= 0)
        {
            Debug.Log(PlayerPrefs.GetInt(passiveSkillPref));
            oldPassiveSkill = PlayerPrefs.GetInt(passiveSkillPref);
            selectedPassiveSkill = PlayerPrefs.GetInt(passiveSkillPref);
            passiveSkills[oldPassiveSkill].setImageObject(selectedSkillIcon);
        }
        // If selected active skill exists, display it
        if (PlayerPrefs.HasKey(activeSkillPref) && PlayerPrefs.GetInt(activeSkillPref) >= 0)
        {
            Debug.Log(PlayerPrefs.GetInt(activeSkillPref));
            oldActiveSkill = PlayerPrefs.GetInt(activeSkillPref);
            selectedActiveSkill = PlayerPrefs.GetInt(activeSkillPref);
            activeSkills[oldActiveSkill].setImageObject(selectedSkillIcon);
        }
        // Display currenct knowledge point amount
        if (!PlayerPrefs.HasKey(knowledgePointsPref))
            PlayerPrefs.SetInt(knowledgePointsPref, 0);

        knowledgePointsText.text = PlayerPrefs.GetInt(knowledgePointsPref).ToString();
    }

    // Check, if passive skill is unlocked
    private void setUpPassiveSkills(string name, int skill)
    {
        if (PlayerPrefs.HasKey(name))
            passiveSkills[skill].setImageObject(baseSkillIcon);
    }

    // Check, if active skill is unlocked
    private void setUpActiveSkills(string name, int skill)
    {
        if (PlayerPrefs.HasKey(name))
            activeSkills[skill].setImageObject(baseSkillIcon);
    }

    // React to press on skill. Diffiriantite skill type ( passive, active) and state ( locked, unlocked, preselected, selected)
    public void skillPress(string skill)
    {
        int btnNumber = int.Parse(skill[1].ToString());
        switch (skill[0])
        {
            case 'p':
                if (passiveSkills[btnNumber].getImageObject().sprite == baseSkillIcon || passiveSkills[btnNumber].getImageObject().sprite == selectedSkillIcon)
                    selectPassiveSkill(btnNumber);
                else if (passiveSkills[btnNumber].getImageObject().sprite == lockedSkillIcon)
                    startUnlockingSkill(skill);
                break;
            case 'a':
                if (activeSkills[btnNumber].getImageObject().sprite == baseSkillIcon || activeSkills[btnNumber].getImageObject().sprite == selectedSkillIcon)
                    selectActiveSkill(btnNumber);
                else if (activeSkills[btnNumber].getImageObject().sprite == lockedSkillIcon)
                    startUnlockingSkill(skill);
                break;
            default:
                Debug.Log("UISkills > Skill press not setup correctly!");
                break;
        }
    }

    // Preselect passive skill
    private void selectPassiveSkill(int number)
    {
        if(passiveSkills[selectedPassiveSkill].getImageObject().sprite == preSelectedSkillIcon)
            passiveSkills[selectedPassiveSkill].setImageObject(baseSkillIcon);
        if (passiveSkills[number].getImageObject().sprite == baseSkillIcon)
            passiveSkills[number].setImageObject(preSelectedSkillIcon);
        selectedPassiveSkill = number;
    }

    // Preselect active skill
    private void selectActiveSkill(int number)
    {
        if (activeSkills[selectedActiveSkill].getImageObject().sprite == preSelectedSkillIcon)
            activeSkills[selectedActiveSkill].setImageObject(baseSkillIcon);
        if (activeSkills[number].getImageObject().sprite != selectedSkillIcon)
            activeSkills[number].setImageObject(preSelectedSkillIcon);
        selectedActiveSkill = number;
    }

    // Initiate skill unlock
    private void startUnlockingSkill(string skill)
    {
        int skillNumber = int.Parse(skill[1].ToString());

        switch (skill[0])
        {
            case 'p':
                skillNameHolder.text = passiveSkills[skillNumber].getName();
                skillCostHolder.text = passiveSkills[skillNumber].getCost().ToString();
                break;
            case 'a':
                skillNameHolder.text = activeSkills[skillNumber].getName();
                skillCostHolder.text = activeSkills[skillNumber].getCost().ToString();
                break;
            default:
                Debug.Log("UISkills >> startUnlockingSkill >> Given skill was wrong: " + skill);
                break;
        }
        unlockingSkill = skill;
        unlockPopUp.SetActive(true);
    }

    // Fully unlock skill
    public void unlockSkill()
    {
        int skillNumber = int.Parse(unlockingSkill[1].ToString());
        int knowledgePoints;
        if (!PlayerPrefs.HasKey(knowledgePointsPref))
            knowledgePoints = 0;
        else knowledgePoints = PlayerPrefs.GetInt(knowledgePointsPref);
        if (int.Parse(skillCostHolder.text) <= knowledgePoints)
        {
            PlayerPrefs.SetInt(knowledgePointsPref, knowledgePoints - int.Parse(skillCostHolder.text));
            switch (unlockingSkill[0])
            {
                case 'p':
                    if (oldPassiveSkill < 0)
                    {
                        passiveSkills[skillNumber].setImageObject(selectedSkillIcon);
                        oldPassiveSkill = skillNumber;
                        selectedPassiveSkill = skillNumber;
                        PlayerPrefs.SetInt(passiveSkillPref, selectedPassiveSkill);
                    }
                    else passiveSkills[skillNumber].setImageObject(baseSkillIcon);
                    PlayerPrefs.SetInt(passiveSkills[skillNumber].getName(), 1);
                    PlayerPrefs.Save();
                    break;
                case 'a':
                    if (oldActiveSkill < 0)
                    {
                        activeSkills[skillNumber].setImageObject(selectedSkillIcon);
                        oldActiveSkill = skillNumber;
                        selectedActiveSkill = skillNumber;
                        PlayerPrefs.SetInt(activeSkillPref, selectedActiveSkill);
                    }
                    else activeSkills[skillNumber].setImageObject(baseSkillIcon);
                    PlayerPrefs.SetInt(activeSkills[skillNumber].getName(), 1);
                    PlayerPrefs.Save();
                    break;
            }
        }
        else errorPopUp.SetActive(true);
        unlockPopUp.SetActive(false);
        PlayerPrefs.Save();
    }

    // Save skill selection change
    public void saveSelection()
    {
        
        if (oldPassiveSkill != selectedPassiveSkill)
        {
            passiveSkills[oldPassiveSkill].setImageObject(baseSkillIcon);
            passiveSkills[selectedPassiveSkill].setImageObject(selectedSkillIcon);
            oldPassiveSkill = selectedPassiveSkill;
        }
        if (oldActiveSkill != selectedActiveSkill)
        {
            activeSkills[oldActiveSkill].setImageObject(baseSkillIcon);
            activeSkills[selectedActiveSkill].setImageObject(selectedSkillIcon);
            oldActiveSkill = selectedActiveSkill;
        }
        PlayerPrefs.SetInt(passiveSkillPref, oldPassiveSkill);
        PlayerPrefs.SetInt(activeSkillPref, oldActiveSkill);
        PlayerPrefs.Save();
    }

    // Cancel skill selection change
    public void cancelSelection()
    {
        if (oldPassiveSkill != selectedPassiveSkill)
            passiveSkills[selectedPassiveSkill].setImageObject(baseSkillIcon);
        if (oldActiveSkill != selectedActiveSkill)
            activeSkills[selectedActiveSkill].setImageObject(baseSkillIcon);
    }

    // Remove unlocked skill and knowledge points
    public void resetSkills()
    {
        PlayerPrefs.DeleteKey(knowledgePointsPref);
        if (PlayerPrefs.HasKey(passiveSkillPref))
            PlayerPrefs.DeleteKey(passiveSkillPref);
        if (PlayerPrefs.HasKey(activeSkillPref))
            PlayerPrefs.DeleteKey(activeSkillPref);
        foreach (Skill s in passiveSkills)
            if (PlayerPrefs.HasKey(s.getName()))
                PlayerPrefs.DeleteKey(s.getName());
        foreach (Skill s in activeSkills)
            if (PlayerPrefs.HasKey(s.getName()))
                PlayerPrefs.DeleteKey(s.getName());
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }
}
