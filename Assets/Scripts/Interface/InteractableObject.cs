using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject :MonoBehaviour
{
    public virtual bool Interact(Character character)
    {
        return true;
    }

    public virtual bool ExitInteract(Character character)
    {
        return true;
    }
}
