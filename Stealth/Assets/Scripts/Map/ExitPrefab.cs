using UnityEngine;
using UnityEngine.UI;

public class ExitPrefab : MonoBehaviour
{
    private const string playerTag = "Player";
    private UIManager uiManager;

    [SerializeField] private Image exitAreaImage;
    [SerializeField] private Sprite canExitSprite;

    // Change gray circle to green
    public void changeSprite()
    {
        exitAreaImage.sprite = canExitSprite;
    }

    // If player is in range, show interact button
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals(playerTag))
        {
            if (uiManager == null)
                uiManager = GameObject.Find("InterfaceManager").GetComponent<UIManager>();
            if(exitAreaImage.sprite == canExitSprite)
                uiManager.showInteractButton(true);
        }
    }
    
    // If player left exit area, don't show interact button
    private void OnTriggerExit(Collider col)
    {
        if (col.tag.Equals(playerTag))
        {
            uiManager.showInteractButton(false);
        }
    }
}
