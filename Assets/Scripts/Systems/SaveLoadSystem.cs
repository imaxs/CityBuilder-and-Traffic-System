using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace CityBuilder
{
    using static BasicBuilding;

    [RequireComponent(typeof(GameControlSystem))]
    public class SaveLoadSystem : MonoBehaviour
    {
        private GameControlSystem gameController = null;

        private string directoryPath
        {
            get
            {
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/CityBuilder";
            }
        }

        private void Start()
        {
            gameController = GetComponent<GameControlSystem>();
        }

        public void SaveCity()
        {
            var city = FindObjectsOfType<BasicBuilding>();
            if (city == null || city.Length == 0)
                return;

            var filePath = directoryPath + "/save.dat";
            if (File.Exists(filePath))
                File.Delete(filePath);

            var file = File.Create(filePath);
            file.Close();


            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(filePath, FileMode.Append, FileAccess.Write);

            // Save GridSettings
            // Save GameTimeOfDay
            foreach (var element in city)
                bf.Serialize(file, element.state);

            file.Close();
        }

        public void LoadCity()
        {
            List<BuildingState> structureInfos = new List<BuildingState>();

            var filePath = directoryPath + "/save.dat";
            if (!File.Exists(filePath))
            {
                Debug.Log("No saved game found!");
                return;
            }

            //
            // Extract Data
            //
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read);
            while (file.Position < (file.Length - 1))
            {
                structureInfos.Add((BuildingState)bf.Deserialize(file));
            }
            file.Close();

            //
            // Load Scene
            //
            // TODO: Load scene with terrain

            //
            // Create City
            //
            //foreach (var element in structureInfos)
            //    gameController.CreateStructure(element);
        }
    }
}
