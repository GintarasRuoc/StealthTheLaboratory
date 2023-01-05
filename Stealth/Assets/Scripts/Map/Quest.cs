using UnityEngine;
[System.Serializable]
public class Quest
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private GameObject taskObject;
    [SerializeField] private Sprite taskObjectIcon;
    [SerializeField] private int reward;
    [SerializeField] private bool isFinished;

    public string getName()
    {
        return name;
    }

    public string getDescription()
    {
        return description;
    }

    public GameObject getTaskObject()
    {
        return taskObject;
    }

    public Sprite getTaskObjectIcon()
    {
        return taskObjectIcon;
    }

    public int getReward()
    {
        return reward;
    }

    public void setIsFinished()
    {
        isFinished = true;
    }

    public bool getIsFinished()
    {
        return isFinished;
    }
}
