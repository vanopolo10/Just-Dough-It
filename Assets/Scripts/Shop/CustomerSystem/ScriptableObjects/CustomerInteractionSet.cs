using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




[Serializable]
public class CustomerInteraction {
    public string dialogueKey, animationKey;
    public void PlayOut(Customer target)
    {
        Animator animator = target.Animator;
        DialogueManager dialogueManager = target.DialogueManager;

        animator.Play(animationKey);
        dialogueManager.DisplayText(dialogueKey); // replace with localization key
    }
}

[Serializable]
public struct DialogueOption
{
    public string textKey;
    public CustomerInteraction interaction;
}


[CreateAssetMenu(fileName = "CustomerInteraction", menuName = "ScriptableObjects/CustomerSystem/CustomerInteractionSet")]
public class CustomerInteractionSet : ScriptableObject
{
    
    public CustomerInteraction OnGreeting, OnItemAccepted, OnItemRejected, OnQuestCompleted;

    public List<DialogueOption> DialogueOptions = new List<DialogueOption>();

}
