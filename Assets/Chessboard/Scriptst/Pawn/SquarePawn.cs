using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquarePawn : Pawn
{
    private Renderer[] childRenderers;

    private void Start()
    {
        onStateChange += ChangeState;
        childRenderers = GetComponentsInChildren<Renderer>(true);

        SetState(new IdleState(this));
    }

    private void Update()
    {
        currentState?.Update();
    }

    private void ChangeState()
    {
        switch (currentState)
        {
            case IdleState:
                ChangeMaterial(BaseMaterial);
                break;
            case DraggedState:
                ChangeMaterial(BaseMaterial);
                break;
            case OutOfBoundsState:
                ChangeMaterial(DeleteMaterial);
                break;
            case ConnectingState:
                break;
        }
    }

    private void ChangeMaterial(Material NewMaterial)
    {
        for (int i = 0;i < childRenderers.Length;i++)
            childRenderers[i].material = NewMaterial;
    }

    public override void ChangeMaterialByConnectingState(Material ChangedMsterial)
    {
        for (int i = 0; i < childRenderers.Length; i++)
            if (childRenderers[i].gameObject.layer == LayerMask.NameToLayer("Connector"))
                childRenderers[i].material = ChangedMsterial;
    }

    private void OnDisable()
    {
        onStateChange -= ChangeState;
    }
}
