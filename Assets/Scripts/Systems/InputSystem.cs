using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CityBuilder
{
    public class InputSystem : MonoBehaviour
    {
        public enum ActionType
        {
            None,
            World,
            UI
        }

        public enum ButtonState
        {
            isHeldUp,
            wasJustPressed,
            isHeldDown,
            wasJustReleased
        }

        [System.Serializable]
        public struct MouseInput
        {
            public ActionType actionType;
            public ButtonState leftClick;
            public ButtonState middleClick;
            public ButtonState rightClick;
            public Vector2 currentScreenPosition;
            public Vector2 deltaScroll;
            public Vector2? dragStartPosition;

            public bool AnyPressed()
            {
                if (leftClick == ButtonState.wasJustPressed)
                    return true;

                if (middleClick == ButtonState.wasJustPressed)
                    return true;

                if (rightClick == ButtonState.wasJustPressed)
                    return true;

                return false;
            }

            public bool AnyHeldDown()
            {
                if (leftClick == ButtonState.isHeldDown)
                    return true;

                if (middleClick == ButtonState.isHeldDown)
                    return true;

                if (rightClick == ButtonState.isHeldDown)
                    return true;

                return false;
            }

            public bool AllHeldUp()
            {
                if (leftClick != ButtonState.isHeldUp)
                    return false;

                if (middleClick != ButtonState.isHeldUp)
                    return false;

                if (rightClick != ButtonState.isHeldUp)
                    return false;

                return true;
            }
        }


        //
        // Cached References
        //
        private CanvasMonitor canvasMonitor = null;


        //
        // Internal Variables
        //
        private MouseInput mouseInput;
        private Dictionary<KeyCode, ButtonState> keyboardInput = new Dictionary<KeyCode, ButtonState>();


        //
        // Public Interface
        //
        public static bool IsMouseOverGameWindow()
        {
            if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width )
                return false;

            if (Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
                return false;

            return true;
        }

        public MouseInput GetMouseInput()
        { 
            return mouseInput;
        }

        public ButtonState GetButtonState(KeyCode keyCode)
        {
            //
            // Mouse Button
            //
            switch(keyCode)
            {
                case KeyCode.Mouse0:
                    return mouseInput.leftClick;
                case KeyCode.Mouse1:
                    return mouseInput.rightClick;
                case KeyCode.Mouse2:
                    return mouseInput.middleClick;
            }

            //
            // Keyboard Button
            //
            if (keyboardInput.ContainsKey(keyCode))
                return keyboardInput[keyCode];
            else
                return ButtonState.isHeldUp;
        }

        public void UpdateInput()
        {
            //
            // Keyboard Input
            //
            foreach (var kvp in keyboardInput.ToArray())
                keyboardInput[kvp.Key] = GetKeyboardButtonState(kvp.Key);

            //
            // Mouse Input
            //
            mouseInput.currentScreenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            mouseInput.deltaScroll = IsMouseOverGameWindow() ? Input.mouseScrollDelta : Vector2.zero;
            mouseInput.leftClick = GetMouseButtonState(KeyCode.Mouse0);
            mouseInput.rightClick = GetMouseButtonState(KeyCode.Mouse1);
            mouseInput.middleClick = GetMouseButtonState(KeyCode.Mouse2);

            //
            // Mouse Drag detection
            //
            if (mouseInput.rightClick == ButtonState.wasJustPressed)
                mouseInput.dragStartPosition = mouseInput.currentScreenPosition;
            else if (mouseInput.rightClick == ButtonState.wasJustReleased)
                mouseInput.dragStartPosition = null;

            //
            // Mouse Action Type
            //
            if (mouseInput.AnyPressed())
                mouseInput.actionType = canvasMonitor.isPointerOverGUI ? ActionType.UI : ActionType.World;
            else if (mouseInput.AllHeldUp())
                mouseInput.actionType = ActionType.None;
        }




        //
        // Private Methods
        //
        private void Start()
        {
            canvasMonitor = FindObjectOfType<CanvasMonitor>();
            InitializeKeyboardInput();
        }

        private void InitializeKeyboardInput()
        {
            keyboardInput.Add(KeyCode.W, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.A, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.S, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.D, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.Q, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.E, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.UpArrow, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.RightArrow, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.DownArrow, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.LeftArrow, ButtonState.isHeldUp);
            keyboardInput.Add(KeyCode.Home, ButtonState.isHeldUp);
        }

        private ButtonState GetKeyboardButtonState(KeyCode keyCode)
        {
            if (Input.GetKeyDown(keyCode))
                return ButtonState.wasJustPressed;
            else if (Input.GetKeyUp(keyCode))
                return ButtonState.wasJustReleased;
            else if (Input.GetKey(keyCode))
                return ButtonState.isHeldDown;
            else
                return ButtonState.isHeldUp;
        }

        private ButtonState GetMouseButtonState(KeyCode keyCode)
        {
            if (Input.GetKeyDown(keyCode) && IsMouseOverGameWindow())
                return ButtonState.wasJustPressed;
            else if (Input.GetKeyUp(keyCode))
                return ButtonState.wasJustReleased;
            else if (Input.GetKey(keyCode))
                return ButtonState.isHeldDown;
            else
                return ButtonState.isHeldUp;
        }
    }
}
