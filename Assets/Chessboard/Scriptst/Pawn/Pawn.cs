using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [HideInInspector]
    public Material BaseMaterial;
    [HideInInspector]
    public Material DeleteMaterial;
    [HideInInspector]
    public PawnController Parent;
    [HideInInspector]
    public GPULineRenderer LineRenderer;

    protected IState currentState;

    public event Action onStateChange;
    public abstract void ChangeMaterialByConnectingState(Material ChangedMsterial);
    public virtual void SetState(IState NewState)
    {
        currentState?.Exit();
        currentState = NewState;
        currentState?.Enter();

        onStateChange?.Invoke();
    }
}
