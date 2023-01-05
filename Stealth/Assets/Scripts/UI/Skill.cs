using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Skill
{
    [SerializeField] private string name;
    [SerializeField] private Image imageObject;
    [SerializeField] private int cost;

    public string getName()
    {
        return name;
    }

    public Image getImageObject()
    {
        return imageObject;
    }

    public void setImageObject(Sprite icon)
    {
        imageObject.sprite = icon;
    }

    public int getCost()
    {
        return cost;
    }
}
