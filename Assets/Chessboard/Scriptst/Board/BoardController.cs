using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyPawn
{
    public class BoardController
    {
        private int boardSize;
        private float cellSize;

        private Color blackCellColor;
        private Color whiteCellColor;

        private Shader boardShader;
        private GameObject board;

        public BoardController(GameObject Board, CrazyPawnSettings Settings, Shader BoardShader)
        {
            board = Board;
            cellSize = Settings.CellSize;
            boardShader = BoardShader;

            boardSize = Settings.CheckerboardSize;
            blackCellColor = Settings.BlackCellColor;
            whiteCellColor = Settings.WhiteCellColor;

            Build();
        }

        private void Build()
        {
            ResizeBoard();
            SetShader();
        }

        private void ResizeBoard()
        {
            var sideLength = boardSize * cellSize;
            board.transform.localScale = new Vector3(sideLength, board.transform.localScale.y, sideLength);
        }

        private void SetShader()
        {
            Renderer renderer = board.GetComponent<Renderer>();

            if (renderer == null)
                renderer = board.AddComponent<MeshRenderer>();

            Material newMaterial = new Material(boardShader);

            newMaterial.SetFloat("_CellSize", cellSize);
            newMaterial.SetInt("_BoardSize", boardSize);
            newMaterial.SetColor("_Color1", blackCellColor);
            newMaterial.SetColor("_Color2", whiteCellColor);

            renderer.material = newMaterial;
        }

        public void UpdateBoard()
        {
            Build();
        }
    }
}
