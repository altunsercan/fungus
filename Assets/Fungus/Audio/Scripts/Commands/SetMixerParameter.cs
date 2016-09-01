// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    /// <summary>
    /// Sets parameter value for target AudioMixer.
    /// </summary>
    [CommandInfo("Audio",
                 "Set Mixer Parameter",
                 "Sets parameter value for target AudioMixer.")]
    [AddComponentMenu("")]
    public class SetMixerParameter : Command
    {
        [Tooltip("Target AudioMixer")]
        [SerializeField]
        protected AudioMixer mixer = null;

        [Tooltip("Name of exposed parameter")]
        [SerializeField]
        protected string parameterName = "";

        [Tooltip("New float value of the parameter")]
        [SerializeField] protected float parameterValue = 0f;
        
        public override void OnEnter()
        {
            mixer.SetFloat(parameterName, parameterValue);

            Continue();
        }

        public override string GetSummary()
        {
            if (mixer == null)
            {
                return "No target mixer selected";
            }

            return "Set " + parameterValue + " of " + mixer.name + " to " + parameterValue;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }
    }
}