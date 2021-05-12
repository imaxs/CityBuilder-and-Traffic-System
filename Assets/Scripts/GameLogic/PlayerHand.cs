using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CityBuilder
{
    public class PlayerHand : MonoBehaviour
    {
        //
        // Events
        //
        public UnityEvent GridEvent;

        //
        // Configurable Parameters
        //
        public Color colorInspect = Color.blue;
        public Color colorValid = Color.green;
        public Color colorInvalid = Color.red;

        [SerializeField]
        private MeshObject _handMesh = null;

        [SerializeField]
        private MeshObject _previewMesh = null;

        //
        // Properties
        //
        public MeshObject HandMesh { get { return _handMesh; } }
        public MeshObject PreviewMesh { get { return _previewMesh; } }
        public Vector3 GridPosition { get { return gridPos; } }

        //
        // Cached References
        //
        public Camera mainCamera = null;
        private BoxCollider p_boxCollider = null;

        //
        // Internal Variables
        //
        private BasicBuilding p_previewPrefab;
        private Vector3 worldPosition = Vector3.zero;
        private Vector3 gridPos = Vector3.zero;


        private void Start()
        {
            p_boxCollider = GetComponent<BoxCollider>();
        }

        public void ChangeSize(Vector3 size)
        {
            _handMesh.transform.localScale = size;
            p_boxCollider.size = size;
        }

        public Vector3 ColliderSize
        {
            get
            {
                return p_boxCollider.size;
            }
        }

        public BasicBuilding PreviewPrefab
        {
            set
            {
                p_previewPrefab = value;
            }
            get
            {
                return p_previewPrefab;
            }
        }

        public void UpdateWorldPosition()
        {
            if (!InputSystem.IsMouseOverGameWindow())
                return;
                
            RaycastHit hit;
            const float raycastDistance = 2000f;
            LayerMask layerMask = LayerMask.GetMask("Terrain");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
            {
                worldPosition = hit.point;
            }
        }

        public void UpdateGridPosition()
        {
            float gridSizeHalf = GameControlSystem.GRIDSIZE / 2;
            float xValue = Mathf.Floor((worldPosition.x + gridSizeHalf) / GameControlSystem.GRIDSIZE) * GameControlSystem.GRIDSIZE;
            float zValue = Mathf.Floor((worldPosition.z + gridSizeHalf) / GameControlSystem.GRIDSIZE) * GameControlSystem.GRIDSIZE;

            if (Mathf.Abs(xValue - transform.position.x) > 0 || Mathf.Abs(zValue - transform.position.z) > 0)
            {
                gridPos.x = xValue;
                gridPos.y = worldPosition.y;
                gridPos.z = zValue;
                transform.position = gridPos;

                if (GridEvent != null) GridEvent.Invoke();
            }
        }
    }
}
