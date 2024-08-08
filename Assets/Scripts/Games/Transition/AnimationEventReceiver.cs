using System;
using UnityEngine;

namespace SlotTemplate {

    public class AnimationEventReceiver : MonoBehaviour {

        public event EventHandler<AnimationEventReceivedEventArgs> AnimationEventReceived;

        void OnAnimationEvent (string stringParameter) {
            if (AnimationEventReceived != null) {
                AnimationEventReceived(this, new AnimationEventReceivedEventArgs{ stringParameter = stringParameter });
            }
        }


        public class AnimationEventReceivedEventArgs : EventArgs {
            public string stringParameter;
        }

    }

}
