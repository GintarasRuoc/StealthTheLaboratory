using UnityEngine;

public class TaskPrefab : MonoBehaviour
{
    private const string playerTag = "Player";
    private QuestManager questManager;
    private UIManager uiManager;
    private bool isMain;

    public void setUpQuest(QuestManager _questManager, UIManager manager, bool _isMain)
    {
        questManager = _questManager;
        uiManager = manager;
        isMain = _isMain;
    }

    // If player is in range, show interact button
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals(playerTag))
        {
            uiManager.showInteractButton(true);
        }
    }

    // If player left the area, don't show interact button
    private void OnTriggerExit(Collider col)
    {
        if (col.tag.Equals(playerTag))
        {
            uiManager.showInteractButton(false);
        }
    }

    // Player completed quest
    public void completeQuest()
    {
        questManager.completeQuest(isMain);
        uiManager.showInteractButton(false);
        Destroy(gameObject);
    }
}
