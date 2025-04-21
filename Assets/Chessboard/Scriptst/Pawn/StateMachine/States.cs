using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class IdleState : IState
{
    private Pawn pawn;

    public IdleState(Pawn Pawn) => pawn = Pawn;

    public void Enter() => Debug.Log("Вошел в Idle");
    public void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if(RaycastToLayer("Connector"))
                pawn.SetState(new ConnectingState(pawn));

            else if (RaycastToLayer("Pawn"))
                pawn.SetState(new DraggedState(pawn));
        }
    }
    public void Exit() => Debug.Log("Вышел из Idle");

    private bool RaycastToLayer(string layerName)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask(layerName)))
        {
            Pawn hitPawn = hit.collider.GetComponentInParent<Pawn>();

            if (hitPawn != null && hitPawn == pawn)
                return true;
        }

        return false;
    }
}

public class DraggedState : IState
{
    private Pawn pawn;

    public DraggedState(Pawn Pawn) => pawn = Pawn;

    public void Enter() => Debug.Log("Вошел в Dragged");
    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Chessboard")))
        {
            pawn.transform.position = new Vector3(hit.point.x, pawn.transform.position.y, hit.point.z);
            pawn.LineRenderer.LineCreator.UpdateActiveLines();
        }

        else
            pawn.SetState(new OutOfBoundsState(pawn));

        if (Input.GetMouseButtonUp(0))
            pawn.SetState(new IdleState(pawn));
    }
    public void Exit() => Debug.Log("Вышел из Dragged");
}

public class OutOfBoundsState : IState
{
    private Pawn pawn;

    public OutOfBoundsState(Pawn Pawn) => pawn = Pawn;

    public void Enter() => Debug.Log("Вошел в OutOfBounds");
    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, pawn.transform.position.y, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Chessboard")))
            pawn.SetState(new DraggedState(pawn));

        else if (plane.Raycast(ray, out float distance))
        {
            pawn.transform.position = ray.GetPoint(distance);
            pawn.LineRenderer.LineCreator.UpdateActiveLines();
        }

        if (Input.GetMouseButtonUp(0))
        {
            pawn.gameObject.SetActive(false);
            pawn.LineRenderer.LineCreator.RemoveInvalidConnections();
        }
    }
    public void Exit() => Debug.Log("Вышел из OutOfBounds");
}

public class ConnectingState : IState
{
    private Pawn pawn;

    public ConnectingState(Pawn Pawn) => pawn = Pawn;

    public void Enter()
    {
        pawn.Parent?.BroadcastToOtherPawns(pawn, true);
    }
    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            pawn.SetState(new IdleState(pawn));
        }

        Debug.Log("Занят соединением с другой фигурой");
    }
    public void Exit()
    {
        pawn.Parent?.BroadcastToOtherPawns(pawn, false);
    }
}
