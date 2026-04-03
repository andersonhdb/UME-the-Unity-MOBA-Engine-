using UnityEngine;

namespace UME.MapCreator
{
    /// <summary>
    /// ScriptableObject that saves a user-created map configuration.
    /// Create via Assets > Create > UME > Map > MapData.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMapData", menuName = "UME/Map/MapData")]
    public class MapData : ScriptableObject
    {
        [Header("Map Identity")]
        public string mapName = "Custom Map";
        [TextArea(2, 4)]
        public string description = "";
        public Sprite thumbnail;

        [Header("Dimensions")]
        public Vector2Int gridSize = new Vector2Int(20, 20);
        public float cellSize = 2f;

        [Header("Spawns")]
        public Vector3 blueTeamSpawn = Vector3.zero;
        public Vector3 redTeamSpawn  = new Vector3(40f, 0f, 40f);

        [Header("Jungle Camps")]
        public MapCampData[] jungleCamps;

        [Header("Objectives")]
        public MapObjectiveData[] objectives;
    }

    [System.Serializable]
    public class MapCampData
    {
        public string campName = "Camp";
        public Vector3 position;
        public string minionPrefabKey = "";
    }

    [System.Serializable]
    public class MapObjectiveData
    {
        public string objectiveName = "Objective";
        public Vector3 position;
        public int goldReward = 300;
        public float experienceReward = 500f;
    }
}
