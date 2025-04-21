using System.Collections.Generic;
using UnityEngine;
using static GPULineRenderer;

public class LineCreator
{
    private GPULineRenderer lineRenderer;
    private Camera mainCamera;
    private LayerMask interactableLayer;

    private GameObject startObject;
    private bool isDrawing = false;
    private List<LineConnection> activeConnections;

    public LineCreator(GPULineRenderer LineRenderer, Camera MainCamera, LayerMask InteractableLayer)
    {
        lineRenderer = LineRenderer;
        mainCamera = MainCamera;
        interactableLayer = InteractableLayer;

        activeConnections = new();
    }

    private class LineConnection
    {
        public Transform start;
        public Transform end;
        public int lineIndex;
    }

    public void HandleLineDrawing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
            {
                startObject = hit.collider.gameObject;
                isDrawing = true;
            }
        }

        if (isDrawing && Input.GetMouseButton(0))
        {
            // Удаляем временную линию, если она есть
            if (activeConnections.Count < lineRenderer.lines.Count)
                lineRenderer.lines.RemoveAt(lineRenderer.lines.Count - 1);

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                lineRenderer.AddLine(startObject.transform.position, hit.point);
            }
            else
            {
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.farClipPlane));
                lineRenderer.AddLine(startObject.transform.position, mouseWorldPos);
            }
        }

        if (isDrawing && Input.GetMouseButtonUp(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer) && startObject.transform.parent != hit.collider.transform.parent)
            {
                GameObject endObject = hit.collider.gameObject;

                // Удаляем временную линию
                lineRenderer.lines.RemoveAt(lineRenderer.lines.Count - 1);

                // Добавляем постоянное соединение
                int lineIndex = lineRenderer.lines.Count;
                lineRenderer.AddLine(startObject.transform.position, endObject.transform.position);

                activeConnections.Add(new LineConnection
                {
                    start = startObject.transform,
                    end = endObject.transform,
                    lineIndex = lineIndex
                });
            }
            else
            {
                // Удаляем временную линию, если соединение не было завершено
                if (lineRenderer.lines.Count > activeConnections.Count)
                    lineRenderer.lines.RemoveAt(lineRenderer.lines.Count - 1);
            }

            isDrawing = false;
            startObject = null;
        }
    }

    public void RemoveInvalidConnections()
    {
        // Создаем временный список для хранения валидных соединений
        List<LineConnection> validConnections = new List<LineConnection>();
        List<LineData> linesToKeep = new List<LineData>();

        foreach (var connection in activeConnections)
        {
            bool isValid = true;

            // Проверяем, существует ли объект и активен ли он
            if (connection.start == null || connection.end == null ||
                !connection.start.gameObject.activeInHierarchy ||
                !connection.end.gameObject.activeInHierarchy)
            {
                isValid = false;
            }

            if (isValid)
            {
                validConnections.Add(connection);
                linesToKeep.Add(lineRenderer.lines[connection.lineIndex]);
            }
        }

        // Если количество соединений изменилось, перестраиваем данные
        if (validConnections.Count != activeConnections.Count)
        {
            lineRenderer.lines = linesToKeep;
            lineRenderer.UpdateBuffer();

            // Обновляем индексы в соединениях
            for (int i = 0; i < validConnections.Count; i++)
                validConnections[i].lineIndex = i;

            activeConnections = validConnections;
        }
    }

    public void UpdateActiveLines()
    {
        for (int i = 0; i < activeConnections.Count; i++)
        {
            var connection = activeConnections[i];

            if (connection.start && connection.end)
            {
                lineRenderer.lines[i] = new GPULineRenderer.LineData
                {
                    start = connection.start.position,
                    end = connection.end.position
                };
            }
        }

        if (activeConnections.Count > 0)
            lineRenderer.UpdateBuffer();
    }

    public void ClearAllLines()
    {
        lineRenderer.ClearLines();
        activeConnections.Clear();
    }
}
