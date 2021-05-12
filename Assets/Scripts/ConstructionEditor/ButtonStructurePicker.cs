using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CityBuilder
{
    [RequireComponent(typeof(BasicMouseUI))]
    public class ButtonStructurePicker : MonoBehaviour
    {
        //
        // Configurable Parameter
        //
        public BasicBuilding buildingPrefab = null;

        //
        // Internal Variables
        //
        private GameControlSystem constructionEditor = null;
        private TextMeshProUGUI buttonText = null;

        private void Start()
        {
            constructionEditor = FindObjectOfType<GameControlSystem>();
            buttonText = GetComponentInChildren<TextMeshProUGUI>(true);
            if (buttonText == null)
            {
                Debug.LogError("Error: button is missing a TMPro Text object.");
                return;
            }

            if (buildingPrefab != null)
                buttonText.text = buildingPrefab.name;
            else
                buttonText.text = "Eraser";
        }

        public void SetEditorStructure()
        {
            if (constructionEditor == null)
            {
                Debug.LogError("Error: could not find the Game Controller.");
                return;
            }

            constructionEditor.SetSelectedStructure(buildingPrefab.descriptor.uuid);
        }
    }
}
