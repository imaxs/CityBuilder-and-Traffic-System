using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CityBuilder
{
    public class BuildingDescriptorGenerator : MonoBehaviour
    {
        [MenuItem("CityBuilderTools/Generate Uuids")]
        public static void GenerateBuildingUUIDs()
        {
            const int maxAttemps = 100;
            int newID = 0;
            int count = 0;

            string[] guids = AssetDatabase.FindAssets("t:" + typeof(BuildingDescriptor));
            BuildingDescriptor[] descriptorsArray = new BuildingDescriptor[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                descriptorsArray[i] = AssetDatabase.LoadAssetAtPath<BuildingDescriptor>(path);
            }

            Dictionary<int, BuildingDescriptor> descriptorsHashMap = new Dictionary<int, BuildingDescriptor>();
            foreach (var element in descriptorsArray)
            {
                if (element.uuid != 0 && !descriptorsHashMap.ContainsKey(element.uuid))
                {
                    descriptorsHashMap.Add(element.uuid, element);
                    continue;
                }

                newID = 0;
                count = 0;
                while (newID == 0)
                {
                    if (count > maxAttemps)
                    {
                        Debug.LogError("Error assigning UUID");
                        break;
                    }

                    count++;
                    newID = UnityEngine.Random.Range(0, int.MaxValue);
                    if (descriptorsHashMap.ContainsKey(element.uuid))
                        newID = 0;
                }
                element.uuid = newID;
                descriptorsHashMap.Add(element.uuid, element);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
