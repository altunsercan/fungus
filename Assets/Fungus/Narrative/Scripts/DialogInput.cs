// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Fungus
{
    /// <summary>
    /// Interface for listening for dialogue input events.
    /// </summary>
    public interface IDialogInputListener
    {
        void OnNextLineEvent();
    }

    /// <summary>
    /// Input handler for say dialogues.
    /// </summary>
    public class DialogInput : MonoBehaviour
    {
        public enum ClickMode
        {
            Disabled,           // Clicking disabled
            ClickAnywhere,      // Click anywhere on screen to advance
            ClickOnDialog,      // Click anywhere on Say Dialog to advance
            ClickOnButton       // Click on continue button to advance
        }

        [Tooltip("Click to advance story")]
        [SerializeField] protected ClickMode clickMode;

        [Tooltip("Delay between consecutive clicks. Useful to prevent accidentally clicking through story.")]
        [SerializeField] protected float nextClickDelay = 0f;

        [Tooltip("Allow holding Cancel to fast forward text")]
        [SerializeField] protected bool cancelEnabled = true;

        [Tooltip("Ignore input if a Menu dialog is currently active")]
        [SerializeField] protected bool ignoreMenuClicks = true;

        protected bool dialogClickedFlag;

        protected bool nextLineInputFlag;

        protected float ignoreClickTimer;

        protected StandaloneInputModule currentStandaloneInputModule;

        /// <summary>
        /// Trigger next line input event from script.
        /// </summary>
        public void SetNextLineFlag()
        {
            nextLineInputFlag = true;
        }

        /// <summary>
        /// Set the dialog clicked flag (usually from an Event Trigger component in the dialog UI).
        /// </summary>
        public void SetDialogClickedFlag()
        {
            // Ignore repeat clicks for a short time to prevent accidentally clicking through the character dialogue
            if (ignoreClickTimer > 0f)
            {
                return;
            }
            ignoreClickTimer = nextClickDelay;

            // Only applies in Click On Dialog mode
            if (clickMode == ClickMode.ClickOnDialog)
            {
                dialogClickedFlag = true;
            }
        }

        public void SetButtonClickedFlag()
        {
            // Only applies if clicking is not disabled
            if (clickMode != ClickMode.Disabled)
            {
                SetNextLineFlag();
            }
        }

        protected virtual void Update()
        {
            if (EventSystem.current == null)
            {
                return;
            }

            if (currentStandaloneInputModule == null)
            {
                if (EventSystem.current == null)
                {
                    // Auto spawn an Event System from the prefab
                    GameObject prefab = Resources.Load<GameObject>("EventSystem");
                    if (prefab != null)
                    {
                        GameObject go = Instantiate(prefab) as GameObject;
                        go.name = "EventSystem";
                    }
                }

                currentStandaloneInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            }

            if (Input.GetButtonDown(currentStandaloneInputModule.submitButton) ||
                (cancelEnabled && Input.GetButton(currentStandaloneInputModule.cancelButton)))
            {
                SetNextLineFlag();
            }

            switch (clickMode)
            {
            case ClickMode.Disabled:
                break;
            case ClickMode.ClickAnywhere:
                if (Input.GetMouseButtonDown(0))
                {
                    SetNextLineFlag();
                }
                break;
            case ClickMode.ClickOnDialog:
                if (dialogClickedFlag)
                {
                    SetNextLineFlag();
                    dialogClickedFlag = false;
                }
                break;
            }

            if (ignoreClickTimer > 0f)
            {
                ignoreClickTimer = Mathf.Max (ignoreClickTimer - Time.deltaTime, 0f);
            }

            if (ignoreMenuClicks)
            {
                // Ignore input events if a Menu is being displayed
                if (MenuDialog.activeMenuDialog != null && 
                    MenuDialog.activeMenuDialog.gameObject.activeInHierarchy &&
                    MenuDialog.activeMenuDialog.DisplayedOptionsCount > 0)
                {
                    dialogClickedFlag = false;
                    nextLineInputFlag = false;
                }
            }

            // Tell any listeners to move to the next line
            if (nextLineInputFlag)
            {
                IDialogInputListener[] inputListeners = gameObject.GetComponentsInChildren<IDialogInputListener>();
                foreach (IDialogInputListener inputListener in inputListeners)
                {
                    inputListener.OnNextLineEvent();
                }
                nextLineInputFlag = false;
            }
        }
    }
}
