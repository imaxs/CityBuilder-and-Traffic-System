using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace CityBuilder
{
    public class GameControlSystem : MonoBehaviour
    {
        //
        // Configurable Parameters
        //
        public static float GRIDSIZE = 1.0f;

        //
        // Cached References
        //
        [SerializeField]
        private CityBuildingLibrary buildingLibrary = null;
        private InputSystem inputSystem = null;
        private CanvasMonitor canvasMonitor = null;
        private PlayerHand playerHand = null;
        private CameraRig cameraRig = null;
        private PlayerCharacter playerCharacter = null;

        //
        // Internal Variables
        //
        private Dictionary<int, BasicBuilding> buildingHashMap;
        private BasicBuilding structureToBuild = null, structureAtLocation = null;
        private bool isConstructionAvailable = true;

        private void Start()
        {
            buildingHashMap = buildingLibrary.GetHashMap();
            inputSystem = GetComponent<InputSystem>();
            playerHand = FindObjectOfType<PlayerHand>();
            canvasMonitor = FindObjectOfType<CanvasMonitor>();

            //
            // Camera Preset
            //
            {
                cameraRig = FindObjectOfType<CameraRig>();
                playerCharacter = FindObjectOfType<PlayerCharacter>();
                playerCharacter.Active = false;
                playerHand.mainCamera = cameraRig.camera;
            }

            playerHand.GridEvent.AddListener(GridUpdateEvent);
        }


        private void FixedUpdate()
        {
            playerHand.UpdateWorldPosition();
            playerHand.UpdateGridPosition();
        }


        //
        // TODO
        //
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                cameraRig.Active = true;
                playerCharacter.Active = false;
                playerHand.mainCamera = cameraRig.camera;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                cameraRig.Active = false;
                playerCharacter.Active = true;
                playerHand.mainCamera = playerCharacter.camera;
            }

            inputSystem.UpdateInput();

            if (cameraRig.Active == true) HandleCameraMovement();
            if (structureToBuild != null) UpdateHandles();

            //Ray ray = playerHand.mainCamera.ScreenPointToRay(Input.mousePosition);
            //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        }

        private void UpdateHandles()
        {
            if (inputSystem.GetMouseInput().actionType != InputSystem.ActionType.UI)
            {
                switch (canvasMonitor.GetActivePanel())
                {
                    case CanvasMonitor.ToolboxPanels.Construction:
                        HandleConstructionActions();
                        break;

                    case CanvasMonitor.ToolboxPanels.Eletricity:
                        HandleElectricityActions();
                        break;

                    case CanvasMonitor.ToolboxPanels.Aqueduct:
                        HandleAqueductActions();
                        break;

                    default: // None
                        HandleInspectActions();
                        break;
                }
            }
        }

        private void GridUpdateEvent()
        {
            if (inputSystem.GetMouseInput().actionType != InputSystem.ActionType.UI)
            {
                switch (canvasMonitor.GetActivePanel())
                {
                    case CanvasMonitor.ToolboxPanels.Construction:
                        {
                            if (structureToBuild == null)
                            {
                                playerHand.HandMesh.meshRenderer.material.color = playerHand.colorInspect;
                                return;
                            }

                            isConstructionAvailable = true;
                            structureAtLocation = null;

                            CheckPlaceAvailableToBuild();

                            //
                            // Update Cursor Color
                            //
                            switch (structureToBuild.descriptor.category)
                            {
                                case BuildingDescriptor.Category.Building:
                                case BuildingDescriptor.Category.Roadway:
                                case BuildingDescriptor.Category.PowerLine:
                                case BuildingDescriptor.Category.Aqueduct:
                                    {
                                        if (isConstructionAvailable)
                                            playerHand.HandMesh.meshRenderer.material.color = playerHand.colorValid;
                                        else
                                            playerHand.HandMesh.meshRenderer.material.color = playerHand.colorInvalid;

                                        break;
                                    }

                                default: // Eraser
                                    {
                                        if (isConstructionAvailable)
                                            playerHand.HandMesh.meshRenderer.material.color = playerHand.colorInvalid;
                                        else
                                            playerHand.HandMesh.meshRenderer.material.color = playerHand.colorValid;

                                        break;
                                    }
                            }
                            break;
                        }
                    case CanvasMonitor.ToolboxPanels.Eletricity:
                    case CanvasMonitor.ToolboxPanels.Aqueduct:
                    default: // None
                        break;
                }
            }
        }

        //
        // The function creates a Raycast, projecting upward from the position of the object's current position
        // with a downward shift along the Y-axis by 5 units, extending for 50 units.
        // Notes: Raycasts will not detect Colliders for which the Raycast origin is inside the Collider.
        //
        private void CheckPlaceAvailableToBuild()
        {
            var size = playerHand.ColliderSize;
            LayerMask layerMask = LayerMask.GetMask("Buildings", "Roads");

            float halfXSize = size.x / 2.0f,
                  halfZSize = size.z / 2.0f,
                  halfGridSize = GRIDSIZE / 2.0f;

            int zNum = Convert.ToInt32(size.z / GRIDSIZE),
                xNum = Convert.ToInt32(size.x / GRIDSIZE);

            for (int zi = 0; zi < zNum; zi++)
            {
                float posZ = playerHand.transform.position.z + (GRIDSIZE * zi) - halfZSize + halfGridSize;
                for (int xi = 0; xi < xNum; xi++)
                {
                    float posX = playerHand.transform.position.x + (GRIDSIZE * xi) - halfXSize + halfGridSize;
                    Vector3 pos = new Vector3(posX, playerHand.transform.position.y - 5, posZ);
                    if (Physics.Raycast(pos, Vector3.up, out RaycastHit hitResult, 50.0f, layerMask))
                    {
                        isConstructionAvailable = false;
                        structureAtLocation = hitResult.collider.transform.root.GetComponent<BasicBuilding>();
                        return;
                    }
                }
            }
        }

        //
        // TODO: Create system to map input to game action
        //
        private void HandleCameraMovement()
        {
            InputSystem.MouseInput mouseInput = inputSystem.GetMouseInput();

            //
            // Reset Orientation
            //
            if (inputSystem.GetButtonState(KeyCode.Home) == InputSystem.ButtonState.wasJustPressed)
            {
                cameraRig.ResetOrientation();
                return;
            }


            //
            // Zooming
            //
            if (mouseInput.actionType != InputSystem.ActionType.UI && mouseInput.deltaScroll.y != 0)
            {
                float zoomAxis = Mathf.Clamp(mouseInput.deltaScroll.y, -1, 1);
                cameraRig.Zoom(zoomAxis);
            }


            //
            // Rotating
            //
            bool isRotating = false;
            if (inputSystem.GetButtonState(KeyCode.Q) == InputSystem.ButtonState.isHeldDown
                || inputSystem.GetButtonState(KeyCode.E) == InputSystem.ButtonState.isHeldDown)
                isRotating = true;

            if (isRotating)
            {
                float axisRight = 0;
                if (inputSystem.GetButtonState(KeyCode.E) == InputSystem.ButtonState.isHeldDown)
                    axisRight = 1;

                float axisLeft = 0;
                if (inputSystem.GetButtonState(KeyCode.Q) == InputSystem.ButtonState.isHeldDown)
                    axisLeft = 1;

                float rotateAxis = Mathf.Clamp(axisRight - axisLeft, -1, 1);
                cameraRig.Rotate(rotateAxis);
            }


            //
            // Panning
            //
            bool isKeyboardPan = false;
            if (inputSystem.GetButtonState(KeyCode.W) == InputSystem.ButtonState.isHeldDown
                || inputSystem.GetButtonState(KeyCode.A) == InputSystem.ButtonState.isHeldDown
                || inputSystem.GetButtonState(KeyCode.S) == InputSystem.ButtonState.isHeldDown
                || inputSystem.GetButtonState(KeyCode.D) == InputSystem.ButtonState.isHeldDown)
                isKeyboardPan = true;

            bool isMousePan = false;
            if (mouseInput.actionType != InputSystem.ActionType.UI
                && mouseInput.dragStartPosition != null 
                && inputSystem.GetButtonState(KeyCode.Mouse1) == InputSystem.ButtonState.isHeldDown)
                isMousePan = true;

            if (isKeyboardPan)
            {
                float axisUp = 0;
                if (inputSystem.GetButtonState(KeyCode.W) == InputSystem.ButtonState.isHeldDown)
                    axisUp = 1;

                float axisDown = 0;
                if (inputSystem.GetButtonState(KeyCode.S) == InputSystem.ButtonState.isHeldDown)
                    axisDown = 1;

                float axisRight = 0;
                if (inputSystem.GetButtonState(KeyCode.D) == InputSystem.ButtonState.isHeldDown)
                    axisRight = 1;

                float axisLeft = 0;
                if (inputSystem.GetButtonState(KeyCode.A) == InputSystem.ButtonState.isHeldDown)
                    axisLeft = 1;

                Vector2 panAxis = Vector2.zero;
                panAxis.x = Mathf.Clamp(axisRight - axisLeft, -1, 1);
                panAxis.y = Mathf.Clamp(axisUp - axisDown, -1, 1);
                cameraRig.Pan(panAxis);
            }
            else if(isMousePan)
            {
                float mouseDeltaX = inputSystem.GetMouseInput().currentScreenPosition.x - inputSystem.GetMouseInput().dragStartPosition.Value.x;
                float mouseDeltaY = inputSystem.GetMouseInput().currentScreenPosition.y - inputSystem.GetMouseInput().dragStartPosition.Value.y;

                float axisRight = (mouseDeltaX / Screen.width) * 2;
                float axisUp = (mouseDeltaY / Screen.height) * 2;

                Vector2 panAxis = Vector2.zero;
                panAxis.x = Mathf.Clamp(axisRight, -1, 1);
                panAxis.y = Mathf.Clamp(axisUp, -1, 1);
                cameraRig.Pan(panAxis * 3);
            }
        }

        private void HandleConstructionActions()
        {
            //
            // Handle Click Events
            //
            switch (structureToBuild.descriptor.category)
            {
                case BuildingDescriptor.Category.Building:
                {
                    if (inputSystem.GetButtonState(KeyCode.Mouse0) == InputSystem.ButtonState.wasJustPressed && isConstructionAvailable)
                    {
                        CreateStructure(structureToBuild, playerHand.GridPosition);
                        GridUpdateEvent();
                    }
                    break;
                }

                case BuildingDescriptor.Category.Roadway:
                case BuildingDescriptor.Category.PowerLine:
                case BuildingDescriptor.Category.Aqueduct:
                {
                    if (inputSystem.GetButtonState(KeyCode.Mouse0) == InputSystem.ButtonState.wasJustPressed && isConstructionAvailable)
                    {
                        CreateStructure(structureToBuild, playerHand.GridPosition);
                        GridUpdateEvent();
                    }
                    break;
                }

                default: // Eraser
                {
                    if (inputSystem.GetButtonState(KeyCode.Mouse0) == InputSystem.ButtonState.wasJustPressed && isConstructionAvailable)
                    {
                        structureAtLocation.OnErase();
                        Destroy(structureAtLocation.gameObject);
                    }
                    break;
                }
            }
        }

        private void HandleElectricityActions()
        {

        }

        private void HandleAqueductActions()
        {

        }

        private void HandleInspectActions()
        {

        }

        public void SetSelectedStructure(int uuid)
        {
            if (!buildingHashMap.ContainsKey(uuid))
                return;

            structureToBuild = buildingHashMap[uuid];

            if (playerHand.PreviewPrefab != null)
            {
                if(playerHand.PreviewPrefab.name.IndexOf(structureToBuild.name) < 0){
                    // Delete previous build previews
                    Debug.Log("Destroy " + playerHand.PreviewPrefab.name);
                    Destroy(playerHand.PreviewPrefab.gameObject);
                }
                else
                    return;
            }

            ///
            // Build Preview
            //
            BasicBuilding prefabPreview = Instantiate(structureToBuild, playerHand.GridPosition, Quaternion.identity);
            var prefabCollider = prefabPreview.transform.GetChild(0);
            if (prefabCollider != null || prefabCollider.name == "Collider")
            {
                Debug.Log("prefabCollider.name = " + prefabCollider.name);
                BoxCollider prefabBoxCollider = prefabCollider.GetComponent<BoxCollider>();
                prefabCollider.gameObject.layer = LayerMask.NameToLayer("BuildPreview");
                playerHand.ChangeSize(prefabBoxCollider.size);
                playerHand.PreviewPrefab = prefabPreview;
                prefabPreview.transform.parent = playerHand.transform;
            }

            //
            // Settings
            //
            switch (structureToBuild.descriptor.category)
            {
                case BuildingDescriptor.Category.Building:
                case BuildingDescriptor.Category.PowerLine:
                case BuildingDescriptor.Category.Aqueduct:
                    GRIDSIZE = 0.25f;
                    break;
                case BuildingDescriptor.Category.Roadway:
                default:
                    GRIDSIZE = 1.0f;
                    break;
            }
        }

        //public void CreateStructure(BasicBuilding.BuildingState structureInfo)
        //{
        //    Vector3 gridPos = new Vector3(structureInfo.position.x, structureInfo.position.y, structureInfo.position.z);
        //    var newStructure = Instantiate(buildingHashMap[structureInfo.uuid], gridPos, Quaternion.identity);
        //    newStructure.GetComponent<BasicBuilding>().OnBuild();
        //}

        public void CreateStructure(BasicBuilding structurePrefab, Vector3 position)
        {
            var newStructure = Instantiate(structurePrefab, position, Quaternion.identity);
            newStructure.OnBuild();
        }
    }
}
