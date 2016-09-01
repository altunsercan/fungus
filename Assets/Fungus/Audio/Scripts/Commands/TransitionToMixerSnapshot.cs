// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    /// <summary>
    /// Transitions mixer values to selected snapshot.
    /// </summary>
    [CommandInfo("Audio",
                 "Transition to Mixer Snapshot",
                 "Transition mixer values to selected snapshot.")]
    [AddComponentMenu("")]
    public class TransitionToMixerSnapshot : Command
    {
        [Tooltip("Target AudioMixer")]
        [SerializeField]
        protected AudioMixer mixer = null;

        [Tooltip("Name of snapshot")]
        [SerializeField]
        protected string snapshotName = "";

        [Range(0,60)]
        [Tooltip("Transition time")]
        [SerializeField] protected float transitionTime = 1f;

        public override void OnEnter()
        {
            AudioMixerSnapshot snapshot = mixer.FindSnapshot(snapshotName);
            if (snapshot == null)
            {
                Continue();
                return;
            }

            snapshot.TransitionTo(transitionTime);
            
        }

        public override string GetSummary()
        {
            if (mixer == null)
            {
                return "No target mixer selected";
            }

            return "Transition to " + snapshotName + " of " + mixer.name + " in " + transitionTime + "seconds";
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }
    }
}