using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UME.MapCreator
{
    /// <summary>
    /// Runtime controller for the Map Creator scene.
    /// Provides a grid-based tile placement tool, spawn point editing, and save/load.
    /// </summary>
    public class MapCreatorController : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int   gridWidth  = 20;
        [SerializeField] private int   gridHeight = 20;
        [SerializeField] private float cellSize   = 2f;

        [Header("Tile Prefabs")]
        [SerializeField] private GameObject[] tilePrefabs;
        [SerializeField] private int          selectedTileIndex = 0;

        [Header("Spawn Markers")]
        [SerializeField] private GameObject blueSpawnMarker;
        [SerializeField] private GameObject redSpawnMarker;

        [Header("UI")]
        [SerializeField] private Button     saveButton;
        [SerializeField] private Button     clearButton;
        [SerializeField] private Button     exportButton;
        [SerializeField] private TMP_InputField mapNameField;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Camera")]
        [SerializeField] private Camera editorCamera;

        private GameObject[,] placedTiles;
        private MapData currentMapData;
        private bool isPlacingSpawn = false;
        private int spawnTeam = 0;

        private void Start()
        {
            placedTiles    = new GameObject[gridWidth, gridHeight];
            currentMapData = ScriptableObject.CreateInstance<MapData>();
            currentMapData.gridSize = new Vector2Int(gridWidth, gridHeight);
            currentMapData.cellSize = cellSize;

            DrawGridGizmos();

            if (saveButton    != null) saveButton.onClick.AddListener(SaveMap);
            if (clearButton   != null) clearButton.onClick.AddListener(ClearMap);
            if (exportButton  != null) exportButton.onClick.AddListener(ExportMap);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick(Input.mousePosition);
            }
            if (Input.GetMouseButtonDown(1))
            {
                EraseAtMouse(Input.mousePosition);
            }
        }

        private void HandleClick(Vector2 screenPos)
        {
            if (editorCamera == null) return;

            Ray ray = editorCamera.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            Vector2Int cell = WorldToCell(hit.point);
            if (!IsValidCell(cell)) return;

            if (isPlacingSpawn)
            {
                PlaceSpawnMarker(cell, spawnTeam);
                return;
            }

            PlaceTile(cell, selectedTileIndex);
        }

        private void EraseAtMouse(Vector2 screenPos)
        {
            if (editorCamera == null) return;

            Ray ray = editorCamera.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            Vector2Int cell = WorldToCell(hit.point);
            EraseTile(cell);
        }

        public void PlaceTile(Vector2Int cell, int tileIndex)
        {
            if (!IsValidCell(cell) || tilePrefabs == null || tileIndex >= tilePrefabs.Length) return;

            EraseTile(cell);

            Vector3 worldPos = CellToWorld(cell);
            GameObject tile = Instantiate(tilePrefabs[tileIndex], worldPos, Quaternion.identity);
            placedTiles[cell.x, cell.y] = tile;
        }

        public void EraseTile(Vector2Int cell)
        {
            if (!IsValidCell(cell)) return;
            if (placedTiles[cell.x, cell.y] != null)
            {
                Destroy(placedTiles[cell.x, cell.y]);
                placedTiles[cell.x, cell.y] = null;
            }
        }

        private void PlaceSpawnMarker(Vector2Int cell, int team)
        {
            Vector3 worldPos = CellToWorld(cell);
            if (team == 1 && blueSpawnMarker != null)
            {
                blueSpawnMarker.transform.position = worldPos;
                currentMapData.blueTeamSpawn = worldPos;
            }
            else if (team == 2 && redSpawnMarker != null)
            {
                redSpawnMarker.transform.position = worldPos;
                currentMapData.redTeamSpawn = worldPos;
            }
            isPlacingSpawn = false;
        }

        public void SelectTile(int index) => selectedTileIndex = index;

        public void BeginPlaceSpawn(int team)
        {
            isPlacingSpawn = true;
            spawnTeam = team;
            SetStatus($"Click on the map to place Team {team} spawn.");
        }

        private void ClearMap()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    EraseTile(new Vector2Int(x, z));
                }
            }
            SetStatus("Map cleared.");
        }

        private void SaveMap()
        {
            if (currentMapData == null) return;
            currentMapData.mapName = mapNameField != null ? mapNameField.text : "Untitled";

#if UNITY_EDITOR
            string path = $"Assets/ScriptableObjects/Maps/{currentMapData.mapName}.asset";
            UnityEditor.AssetDatabase.CreateAsset(currentMapData, path);
            UnityEditor.AssetDatabase.SaveAssets();
            SetStatus($"Map saved to {path}");
#else
            SetStatus("Map saving is only available in the Unity Editor.");
#endif
        }

        private void ExportMap()
        {
            string json = JsonUtility.ToJson(currentMapData, prettyPrint: true);
            Debug.Log($"[MapCreator] Export:\n{json}");
            SetStatus("Map exported to console.");
        }

        private void DrawGridGizmos()
        {
            // Grid visualisation is handled via OnDrawGizmos.
        }

        private Vector2Int WorldToCell(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(worldPos.x / cellSize);
            int z = Mathf.FloorToInt(worldPos.z / cellSize);
            return new Vector2Int(x, z);
        }

        private Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(cell.x * cellSize + cellSize * 0.5f, 0f, cell.y * cellSize + cellSize * 0.5f);
        }

        private bool IsValidCell(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < gridWidth && cell.y >= 0 && cell.y < gridHeight;
        }

        private void SetStatus(string msg)
        {
            if (statusText != null) statusText.text = msg;
            Debug.Log($"[MapCreator] {msg}");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
            for (int x = 0; x <= gridWidth; x++)
            {
                Gizmos.DrawLine(
                    new Vector3(x * cellSize, 0f, 0f),
                    new Vector3(x * cellSize, 0f, gridHeight * cellSize)
                );
            }
            for (int z = 0; z <= gridHeight; z++)
            {
                Gizmos.DrawLine(
                    new Vector3(0f, 0f, z * cellSize),
                    new Vector3(gridWidth * cellSize, 0f, z * cellSize)
                );
            }
        }
    }
}
