using CrazyPawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyPawn
{
    public class GameController : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private CrazyPawnSettings _settings;
        [SerializeField] private GPULineRenderer _lineRenderer;

        [Space]
        [Header("Chessboard")]
        [SerializeField] private GameObject _boardPrefab;
        [SerializeField] private Shader _boardShader;

        [Space]
        [Header("Pawn")]
        [SerializeField] private GameObject _pawnPrefab;

        private BoardController chessboard;
        private PawnController pawnController;

        private void Start()
        {
            SpawnBoard();
            SpawnPawns();
        }

        public void SpawnBoard()
        {
            var board = Instantiate(_boardPrefab);
            chessboard = new(board, _settings, _boardShader);
        }

        public void SpawnPawns()
        {
            pawnController = new(_settings, _lineRenderer);
            float spawnRadius = _settings.InitialZoneRadius;

            GameObject pawn;
            Vector3 spawnPosition;

            for (int i = 0; i < _settings.InitialPawnCount; i++)
            {
                spawnPosition = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    _boardPrefab.transform.localScale.y / 2,
                    Random.Range(-spawnRadius, spawnRadius)
                    );
                pawn = Instantiate(_pawnPrefab, spawnPosition, Quaternion.identity);
                pawnController.AddPawn(pawn?.GetComponent<Pawn>());
            }
        }
    }
}
