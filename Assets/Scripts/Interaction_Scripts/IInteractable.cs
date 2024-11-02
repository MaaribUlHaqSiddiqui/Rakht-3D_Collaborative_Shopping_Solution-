using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string Interaction_prompt { get; }
    public bool Interact(Interaction interactor);
}
