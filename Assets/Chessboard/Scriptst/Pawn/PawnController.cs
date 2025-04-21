using CrazyPawn;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController
{
    private List<Pawn> pawns;
    private GPULineRenderer lineRenderer;

    private Material baseMaterial;
    private Material deleteMaterial;
    private Material activeConnectorMaterial;

    public List<Pawn> Pawns => pawns;

    public PawnController(CrazyPawnSettings Settings, GPULineRenderer LineRenderer)
    {
        pawns = new();
        lineRenderer = LineRenderer;

        baseMaterial = Settings.BaseMaterial;
        deleteMaterial = Settings.DeleteMaterial;
        activeConnectorMaterial = Settings.ActiveConnectorMaterial;
    }

    public void AddPawn(Pawn Pawn)
    {
        Pawn.LineRenderer = lineRenderer;
        Pawn.Parent = this;

        Pawn.BaseMaterial = baseMaterial;
        Pawn.DeleteMaterial = deleteMaterial;

        pawns.Add(Pawn);
    }

    public void BroadcastToOtherPawns(Pawn CallingPawn, bool isActivate)
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i] == CallingPawn)
                continue;

            if(isActivate)
                pawns[i].ChangeMaterialByConnectingState(activeConnectorMaterial);
            else
                pawns[i].ChangeMaterialByConnectingState(baseMaterial);
        }
    }
}
