// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when a key press event occurs.
    /// </summary>
    [EventHandlerInfo("Input",
                      "Key Pressed",
                      "The block will execute when a key press event occurs.")]
    [AddComponentMenu("")]
    public class KeyPressed : EventHandler
    {   
        public enum KeyPressType
        {
            KeyDown,    // Execute once when the key is pressed down
            KeyUp,      // Execute once when the key is released
            KeyRepeat   // Execute once per frame when key is held down
        }

        [Tooltip("The type of keypress to activate on")]
        [SerializeField] protected KeyPressType keyPressType;

        [Tooltip("Keycode of the key to activate on")]
        [SerializeField] protected KeyCode keyCode;

        protected virtual void Update()
        {
            switch (keyPressType)
            {
            case KeyPressType.KeyDown:
                if (Input.GetKeyDown(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            case KeyPressType.KeyUp:
                if (Input.GetKeyUp(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            case KeyPressType.KeyRepeat:
                if (Input.GetKey(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            }
        }

        public override string GetSummary()
        {
            return keyCode.ToString();
        }
    }
}