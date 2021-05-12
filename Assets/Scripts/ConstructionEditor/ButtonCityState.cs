using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [RequireComponent(typeof(BasicMouseUI))]
    public class ButtonCityState : MonoBehaviour
    {
        public void SaveCityState()
        {
            var cityState = FindObjectOfType<SaveLoadSystem>();
            if (cityState == null)
                return;

            cityState.SaveCity();
        }

        public void LoadCityState()
        {
            var cityState = FindObjectOfType<SaveLoadSystem>();
            if (cityState == null)
                return;

            cityState.LoadCity();
        }
    }
}
