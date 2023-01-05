using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UISkill
{
    [SerializeField] private Sprite[] passiveSkillIcon;
    [SerializeField] private Image passiveSkillPlaceholder;

    [SerializeField] private Sprite[] activeSkillIcon;
    [SerializeField] private Image activeSkillPlaceholder;
    [SerializeField] private Text activeSkillButtonText;
    [SerializeField] private Text activeSkillAmountText;

    // Display active skill
    public void setUpActiveSkill(int skill, int skillAmount, string action)
    {
        if (skill >= 0)
        {
            activeSkillPlaceholder.sprite = activeSkillIcon[skill];
            activeSkillAmountText.text = skillAmount.ToString();
            activeSkillButtonText.text = action;
        }
        else activeSkillPlaceholder.color = new Color(0, 0, 0, 0);

        setUpPassiveSkill();
    }

    // Display passive skill
    private void setUpPassiveSkill()
    {
        if (PlayerPrefs.HasKey("passiveSkill") && PlayerPrefs.GetInt("passiveSkill") >= 0)
            passiveSkillPlaceholder.sprite = passiveSkillIcon[PlayerPrefs.GetInt("passiveSkill")];
        else passiveSkillPlaceholder.color = new Color(0, 0, 0, 0);
    }

    // Display active skill amount of uses
    public void usedSkill()
    {
        activeSkillAmountText.text = (int.Parse(activeSkillAmountText.text) - 1).ToString();
    }
}
