using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleButton : MonoBehaviour, Interactable
{
    [SerializeField]
    MeshRenderer ButtonRenderer;

    [HideInInspector]
    public ObjectiveButton Objective;

    public bool Interacted;
    public bool InteractedOnce;
    public bool PermanentlyOn;
    public bool ChangesColor;

    public Material InteractedMaterial;
    public Material OriginalMaterial;

    private void Awake()
    {
        if(ChangesColor)
            OriginalMaterial = ButtonRenderer.sharedMaterial;
    }
    public virtual void Interact()
    {
        InteractedOnce = true;

        if (PermanentlyOn && Interacted == true)
            return;
        else
        {
            Interacted = !Interacted;
        }
    }

    public void ChangeColor()
    {
        if (!ChangesColor)
            return;

        if (Interacted)
        {
            ButtonRenderer.sharedMaterial = InteractedMaterial;
        } else
        {
            ButtonRenderer.sharedMaterial = OriginalMaterial;
        }
    }
}
