using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPULineRenderer : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Material lineMaterial;

    [Space]
    [Header("Line creator settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask interactableLayer;
    [Range(0.01f, 1f)]
    [SerializeField] private float lineWidth;

    private LineCreator lineCreator;
    private ComputeBuffer linesBuffer;

    [HideInInspector]
    public List<LineData> lines;

    public LineCreator LineCreator => lineCreator;

    public struct LineData
    {
        public Vector3 start;
        public Vector3 end;
    }

    private void Start()
    {
        if (linesBuffer != null)
            linesBuffer.Release();

        lines = new();
        lineCreator = new(this, mainCamera, interactableLayer);

        SetLineWidth(lineWidth);
        UpdateBuffer();
        UpdateMaterialProperties();
    }

    private void Update()
    {
        lineCreator.HandleLineDrawing();
    }

    private void UpdateMaterialProperties()
    {
        if (lineMaterial != null)
        {
            lineMaterial.SetFloat("_LineWidth", lineWidth);
            lineMaterial.SetBuffer("_Lines", linesBuffer);
        }
    }

    private void OnRenderObject()
    {
        if (lines.Count == 0 || lineMaterial == null) return;

        UpdateMaterialProperties();

        lineMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Lines, 2, lines.Count);
    }

    private void OnDestroy()
    {
        if (linesBuffer != null)
            linesBuffer.Release();
    }

    public void UpdateBuffer()
    {
        if (linesBuffer == null || linesBuffer.count != lines.Count)
        {
            if (linesBuffer != null)
                linesBuffer.Release();

            linesBuffer = new ComputeBuffer(lines.Count == 0 ? 1 : lines.Count, 24);
            lineMaterial.SetBuffer("_Lines", linesBuffer);
        }

        if (lines.Count > 0)
            linesBuffer.SetData(lines);
    }

    public void AddLine(Vector3 start, Vector3 end)
    {
        lines.Add(new LineData { start = start, end = end });
        UpdateBuffer();
    }

    public void SetLineWidth(float width)
    {
        if (lineMaterial != null)
            lineMaterial.SetFloat("_LineWidth", lineWidth);
    }

    public void ClearLines()
    {
        lines.Clear();
        UpdateBuffer();
    }
}
