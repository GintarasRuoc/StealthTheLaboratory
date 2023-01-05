using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private bool isTaskFinished = false;
    private bool isSideTaskFinished = false;

    [SerializeField] private float minDistanceToTaskRoom = 75f;
    [SerializeField] private Quest[] mainTasks;
    [SerializeField] private Quest[] sideTasks;
    [SerializeField] private GameObject questPrefab;

    [Tooltip("Name of empty object, that holds available locations of tasks in room")]
    [SerializeField] private const string taskLocationListName = "TaskLocations";

    [SerializeField] private UIManager uIManager;
    private int mainQuest;
    private int sideQuest;

    // Spawn side and main task
    public void spawnQuests(List<GameObject> spawnedRooms, float startPosition)
    {
        if (spawnedRooms.Count > 0)
        {
            // Spawn side task
            if(Random.Range(0,2) < 1)
            sideQuest = spawnQuest(spawnedRooms, startPosition, 0f, sideTasks, false);
            // Spawn main task
            mainQuest = spawnQuest(spawnedRooms, startPosition, minDistanceToTaskRoom, mainTasks, true);

            uIManager.setUpQuests(mainTasks[mainQuest], sideTasks[sideQuest]);
        }
        else Debug.Log("GameManger // GameMasterController >> TaskGenerator // Spawned rooms are empty.");
    }

    // Choose random task from list and random available room
    private int spawnQuest(List<GameObject> spawnedRooms, float startPosition, float minDistance, Quest[] tasks, bool isMain)
    {
        if(tasks.Length > 0)
        {
            Quest quest = null;
            int task = Random.Range(0, tasks.Length);
            quest = tasks[task];
            bool spawnedTask = false;
            while(spawnedTask == false)
            {
                GameObject selectedRoom = spawnedRooms[Random.Range(1, spawnedRooms.Count)];

                spawnedTask = instantiateTask(selectedRoom, startPosition, minDistance, quest, isMain);
                spawnedRooms.Remove(selectedRoom);
            }
            
            return task;
        }
        else Debug.Log("GameManger // GameMasterController >> TaskGenerator // Couldn't spawn task. Empty task list.");
        return 0;
    }
    
    // Choose random location in give room and spawn task
    private bool instantiateTask(GameObject selectedRoom, float startPosition, float minDistance, Quest task, bool isMain)
    {
        List<Transform> taskLocations = new List<Transform>();
        foreach (Transform child in selectedRoom.transform.Find(taskLocationListName))
        {
            if (minDistanceToTaskRoom < Vector3.Distance(new Vector3(startPosition, 0f, startPosition), child.position))
                taskLocations.Add(child);
        }
        if (taskLocations.Count > 0)
        {
            int taskLocNum = Random.Range(0, taskLocations.Count);
            GameObject tempQuest = Instantiate(questPrefab, taskLocations[taskLocNum].position, new Quaternion(), transform);
            tempQuest.GetComponent<TaskPrefab>().setUpQuest(GetComponent<QuestManager>(), uIManager, isMain);
            Instantiate(task.getTaskObject(), tempQuest.transform.position, task.getTaskObject().transform.rotation, tempQuest.transform);
        }
        else return false;
        return true;
    }

    // Complete quest
    public void completeQuest(bool isMain)
    {
        if (isMain)
        {
            mainTasks[mainQuest].setIsFinished();
            GameObject.FindGameObjectWithTag("Exit").GetComponent<ExitPrefab>().changeSprite();
        }
        else sideTasks[sideQuest].setIsFinished();

        uIManager.completeQuest(isMain);
    }

    // Check, if quest is completed
    public bool isQuestDone(bool isMain)
    {
        if(isMain)
            return mainTasks[mainQuest].getIsFinished();
        else return sideTasks[sideQuest].getIsFinished();
    }

    // Get side quest reward
    public int getSideQuestReward()
    {
        return sideTasks[sideQuest].getReward();
    }
}
