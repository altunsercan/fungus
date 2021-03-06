// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    public enum StageDisplayType
    {
        None,
        Show,
        Hide,
        Swap,
        MoveToFront,
        UndimAllPortraits,
        DimNonSpeakingPortraits
    }

    /// <summary>
    /// Controls the stage on which character portraits are displayed.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Control Stage",
                 "Controls the stage on which character portraits are displayed.")]
    public class ControlStage : ControlWithDisplay<StageDisplayType> 
    {
        [Tooltip("Stage to display characters on")]
        [SerializeField] protected Stage stage;
        public virtual Stage _Stage { get { return stage; } }

        [Tooltip("Stage to swap with")]
        [SerializeField] protected Stage replacedStage;

        [Tooltip("Use Default Settings")]
        [SerializeField] protected bool useDefaultSettings = true;
        public virtual bool UseDefaultSettings { get { return useDefaultSettings; } }

        [Tooltip("Fade Duration")]
        [SerializeField] protected float fadeDuration;
        
        [Tooltip("Wait until the tween has finished before executing the next command")]
        [SerializeField] protected bool waitUntilFinished = false;
        
        public override void OnEnter()
        {
            // If no display specified, do nothing
            if (IsDisplayNone(display))
            {
                Continue();
                return;
            }

            // Selected "use default Portrait Stage"
            if (stage == null)           
            {
                // If no default specified, try to get any portrait stage in the scene
                stage = FindObjectOfType<Stage>();

                // If portrait stage does not exist, do nothing
                if (stage == null)
                {
                    Continue();
                    return;
                }
            }
           
            // Selected "use default Portrait Stage"
            if (display == StageDisplayType.Swap)            // Default portrait stage selected
            {
                if (replacedStage == null)        // If no default specified, try to get any portrait stage in the scene
                {
                    replacedStage = GameObject.FindObjectOfType<Stage>();
                }
                // If portrait stage does not exist, do nothing
                if (replacedStage == null)
                {
                    Continue();
                    return;
                }
            }
            // Use default settings
            if (useDefaultSettings)
            {
                fadeDuration = stage.FadeDuration;
            }
            switch(display)
            {
            case (StageDisplayType.Show):
                Show(stage, true);
                break;
            case (StageDisplayType.Hide):
                Show(stage, false);
                break;
            case (StageDisplayType.Swap):
                Show(stage, true);
                Show(replacedStage, false);
                break;
            case (StageDisplayType.MoveToFront):
                MoveToFront(stage);
                break;
            case (StageDisplayType.UndimAllPortraits):
                UndimAllPortraits(stage);
                break;
            case (StageDisplayType.DimNonSpeakingPortraits):
                DimNonSpeakingPortraits(stage);
                break;
            }

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        protected void Show(Stage stage, bool visible) 
        {
            float duration = (fadeDuration == 0) ? float.Epsilon : fadeDuration;
            float targetAlpha = visible ? 1f : 0f;

            CanvasGroup canvasGroup = stage.GetComponentInChildren<CanvasGroup>();
            if (canvasGroup == null)
            {
                Continue();
                return;
            }
            
            LeanTween.value(canvasGroup.gameObject, canvasGroup.alpha, targetAlpha, duration).setOnUpdate( (float alpha) => {
                canvasGroup.alpha = alpha;
            }).setOnComplete( () => {
                OnComplete();
            });
        }

        protected void MoveToFront(Stage stage)
        {
            foreach (Stage s in Stage.activeStages)
            {
                if (s == stage)
                {
                    s.PortraitCanvas.sortingOrder = 1;
                }
                else
                {
                    s.PortraitCanvas.sortingOrder = 0;
                }
            }
        }

        protected void UndimAllPortraits(Stage stage) 
        {
            stage.DimPortraits = false;
            foreach (Character character in stage.CharactersOnStage)
            {
                stage.SetDimmed(character, false);
            }
        }

        protected void DimNonSpeakingPortraits(Stage stage) 
        {
            stage.DimPortraits = true;
        }

        protected void OnComplete() 
        {
            if (waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            string displaySummary = "";
            if (display != StageDisplayType.None)
            {
                displaySummary = StringFormatter.SplitCamelCase(display.ToString());
            }
            else
            {
                return "Error: No display selected";
            }
            string stageSummary = "";
            if (stage != null)
            {
                stageSummary = " \"" + stage.name + "\"";
            }
            return displaySummary + stageSummary;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(230, 200, 250, 255);
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            //Default to display type: show
            display = StageDisplayType.Show;
        }
    }
}