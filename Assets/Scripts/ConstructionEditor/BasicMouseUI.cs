using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CityBuilder
{
    public class BasicMouseUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        //
        // Internal Variables
        //
        private CanvasMonitor canvasMonitor = null;

        private void Start()
        {
            canvasMonitor = GetComponentInParent<CanvasMonitor>();
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (canvasMonitor == null)
                Debug.LogError("Error: could not find the Canvas Monitor.");
            else
                canvasMonitor.isPointerOverGUI = true;
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (canvasMonitor == null)
                Debug.LogError("Error: could not find the Canvas Monitor.");
            else
                canvasMonitor.isPointerOverGUI = false;
        }
    }
}
