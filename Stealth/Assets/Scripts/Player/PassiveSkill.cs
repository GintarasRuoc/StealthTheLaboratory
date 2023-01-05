using UnityEngine;

public class PassiveSkill : MonoBehaviour
{
    private const string passiveSkillPref = "passiveSkill";

    [SerializeField] private int movementSpeedBoost = 15;
    [SerializeField] private int runSoundAmount = 75;
    [SerializeField] private float visbilityMultiplier = 1.25f;

    public void setUpPassiveSkill()
    {
        Debug.Log("Setting up passive skills");
        if(PlayerPrefs.HasKey(passiveSkillPref))
        {
            Debug.Log(PlayerPrefs.GetInt(passiveSkillPref));
            switch (PlayerPrefs.GetInt(passiveSkillPref))
            {
                case 0:
                    movementSpeed();
                    break;
                case 1:
                    quieterSteps();
                    break;
                case 2:
                    concealingClothes();
                    break;
                case 3:
                    map();
                    break;
            }
        }
    }

    private void movementSpeed()
    {
        GetComponent<PlayerMovement>().upgradeMovement(movementSpeedBoost);
    }

    private void quieterSteps()
    {
        GetComponent<PlayerMovement>().quieterRun(runSoundAmount);
    }

    private void concealingClothes()
    {
        GetComponent<PlayerMovement>().upgradeVisbility(visbilityMultiplier);
    }

    private void map()
    {
        GameObject.Find("MiniMapCamera").GetComponent<MiniMap>().showAllTiles();
    }
}
