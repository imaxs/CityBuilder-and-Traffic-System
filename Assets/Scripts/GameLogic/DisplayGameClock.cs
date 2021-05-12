using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CityBuilder
{
    public class DisplayGameClock : MonoBehaviour
    {
        //
        // Configurable Parameters
        //
        public TextMeshProUGUI labelTimeOfDay = null;

        //
        // Cached References
        //
        private TimeKeeperSystem timeController = null;

        void Start()
        {
            timeController = FindObjectOfType<TimeKeeperSystem>();
            if (timeController != null)
                timeController.OnEventMinutesChanged += () => {
                    labelTimeOfDay.text = timeController.GetHours().ToString("D2") + " : " + timeController.GetMinutes().ToString("D2");
                };
        }
    }
}
