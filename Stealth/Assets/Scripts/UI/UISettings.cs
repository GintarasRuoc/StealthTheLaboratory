using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UISettings : MonoBehaviour
{
    private const string rebindsPref = "rebinds";

    [SerializeField] private InputActions inputActions;
    [SerializeField] private Text[] keybindInputSlot;

    [SerializeField] private GameObject popUp;
    [SerializeField] private Text popUpText;
    [SerializeField] private GameObject confirmationPopUp;

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;
    private InputAction action;
    private int keybindInputIndex;

    private void Start()
    {
        // If buttons has been changed, read overrides
        inputActions = new InputActions();
        string rebinds = PlayerPrefs.GetString(rebindsPref, string.Empty);
        if (!string.IsNullOrEmpty(rebinds))
            inputActions.LoadBindingOverridesFromJson(rebinds);

        updateKeybindUI();
    }

    // Show current control buttons
    private void updateKeybindUI()
    {
        keybindInputSlot[0].text = inputActions.Player.goTo.bindings[0].ToDisplayString();
        keybindInputSlot[1].text = inputActions.Player.toggleWalk.bindings[0].ToDisplayString();
        keybindInputSlot[2].text = inputActions.Player.activeSkill.bindings[0].ToDisplayString();
        keybindInputSlot[3].text = inputActions.SkillUse.activate.bindings[0].ToDisplayString();
        keybindInputSlot[4].text = inputActions.SkillUse.cancel.bindings[0].ToDisplayString();
        keybindInputSlot[5].text = inputActions.Player.interact.bindings[0].ToDisplayString();
        keybindInputSlot[6].text = inputActions.Player.pauseGame.bindings[0].ToDisplayString();
        keybindInputSlot[7].text = inputActions.Player.zoom.bindings[0].ToDisplayString();
        keybindInputSlot[8].text = inputActions.Player.lookAround.bindings[0].ToDisplayString();
    }

    // Prepare for button change by creating correct button rebind function
    public void prepChangeButton(string button)
    {
        inputActions.Disable();
        switch (button)
        {
            case "moveChar":
                action = inputActions.Player.goTo;
                keybindInputIndex = 0;
                
                rebindOperation = action.PerformInteractiveRebinding(0)
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(callback =>
                    {
                        popUpText.text = action.bindings[0].ToDisplayString();
                        inputActions.Enable();
                        callback.Dispose();
                    });
                break;
            case "walkToggle":
                action = inputActions.Player.toggleWalk;
                keybindInputIndex = 1;
                rebindOperation = action.PerformInteractiveRebinding(0)
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(callback =>
                    {
                        popUpText.text = action.bindings[0].ToDisplayString();
                        inputActions.Enable();
                        callback.Dispose();
                    });
                break;
            case "activateSkill":
                action = inputActions.Player.activeSkill;
                keybindInputIndex = 2;
                rebindOperation = action.PerformInteractiveRebinding(0)
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(callback =>
                    {
                        popUpText.text = action.bindings[0].ToDisplayString();
                        inputActions.Enable();
                        callback.Dispose();
                    });
                break;
            case "confirmSkillUse":
                action = inputActions.SkillUse.activate;
                keybindInputIndex = 3;
                rebindOperation = action.PerformInteractiveRebinding(0)
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(callback =>
                    {
                        popUpText.text = action.bindings[0].ToDisplayString();
                        inputActions.Enable();
                        callback.Dispose();
                    });
                break;
            case "cancelSkillUse":
                action = inputActions.SkillUse.cancel;
                keybindInputIndex = 4;
                rebindOperation = action.PerformInteractiveRebinding(0)
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(callback =>
                    {
                        popUpText.text = action.bindings[0].ToDisplayString();
                        inputActions.Enable();
                        callback.Dispose();
                    });
                break;
            case "interact":
                action = inputActions.Player.interact;
                keybindInputIndex = 5;
                rebindOperation = action.PerformInteractiveRebinding(0)
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(callback =>
                    {
                        popUpText.text = action.bindings[0].ToDisplayString();
                        inputActions.Enable();
                        callback.Dispose();
                    });
                break;
            case "pauseGame":
                action = inputActions.Player.pauseGame;
                keybindInputIndex = 6;
                rebindOperation = action.PerformInteractiveRebinding(0)
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(callback =>
                    {
                        popUpText.text = action.bindings[0].ToDisplayString();
                        inputActions.Enable();
                        callback.Dispose();
                    });
                break;
            default:
                Debug.Log("Change button error, couldn't find switch case: " + button);
                break;
        }
        popUpText.text = action.bindings[0].ToDisplayString();
        popUp.SetActive(true);
    }

    // Activate prepared button change function
    public void listenChangeButton()
    {
        popUpText.text = "Listening...";
        rebindOperation.Start();
    }

    // Confirm control button change
    public void confirmChangeButton(bool checkDuplicate)
    {
        if (checkDuplicateBindings() && checkDuplicate)
            confirmationPopUp.SetActive(true);
        else
        {
            keybindInputSlot[keybindInputIndex].text = action.bindings[0].ToDisplayString();
            string rebinds = inputActions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(rebindsPref, rebinds);
            rebindOperation.Dispose();
            popUp.SetActive(false);
        } 
    }

    // if there is duplicate binding, notify user
    private bool checkDuplicateBindings()
    {

        InputBinding newBinding = action.bindings[0];
        foreach (InputBinding binding in action.actionMap.bindings)
        {
            if (binding.action == newBinding.action)
                continue;
            if (binding.effectivePath == newBinding.effectivePath)
            {
                confirmationPopUp.SetActive(true);
                return true;
            }
        }
        return false;
    }

    // canceled control button change
    public void cancelChangeButton()
    {
        action.RemoveBindingOverride(0);
        rebindOperation.Dispose();
    }

    // Change control buttons to default
    public void resetAllButtons()
    {
        inputActions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(rebindsPref);
        updateKeybindUI();
    }
}
