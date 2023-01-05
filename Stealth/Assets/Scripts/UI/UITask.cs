using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class UITask
{
    // Start panel quest info
    [SerializeField] private Image startQuestIcon;
    [SerializeField] private Text startQuestName;
    [SerializeField] private Text startQuestDescription;
    [SerializeField] private Text startQuestReward;
    // Pause panel quest info
    [SerializeField] private Image questIcon;
    [SerializeField] private Text questName;
    [SerializeField] private Text questDescription;
    [SerializeField] private Text questReward;
    // Game panels
    [SerializeField] private GameObject questCompleted;
    [SerializeField] private GameObject startNoQuest;
    [SerializeField] private GameObject noQuest;

    // Show quest in starting tab and pause menu
    public void setUpQuest(Quest quest)
    {
        if (quest != null)
        {
            startQuestIcon.sprite = questIcon.sprite = quest.getTaskObjectIcon();
            startQuestName.text = questName.text = quest.getName();
            startQuestDescription.text = questDescription.text = quest.getDescription();
            if (startQuestReward != null && questReward != null)
                startQuestReward.text = questReward.text = "Quest reward: " +  quest.getReward().ToString() + " knowledge points";
        }
        else
        {
            startNoQuest.SetActive(true);
            noQuest.SetActive(true);
        }
    }

    // Show that quest is completed
    public void questDone()
    {
        questCompleted.SetActive(true);
    }
}
