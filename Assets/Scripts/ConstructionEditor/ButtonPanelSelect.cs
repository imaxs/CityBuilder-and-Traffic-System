using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [RequireComponent(typeof(BasicMouseUI))]
    public class ButtonPanelSelect : MonoBehaviour
    {
        //
        // Configurable Paramters
        //
        public CanvasMonitor.ToolboxPanels panelType = CanvasMonitor.ToolboxPanels.Root;

        //
        // Internal Variables
        //
        private CanvasMonitor canvasMonitor = null;

        private void Start()
        {
            canvasMonitor = GetComponentInParent<CanvasMonitor>();
        }

        public void SelectActivePanel()
        {
            if (canvasMonitor == null)
            {
                Debug.LogError("Error: could not find the Canvas Monitor.");
                return;
            }

            canvasMonitor.SetActivePanel(panelType);
        }
    }
}
