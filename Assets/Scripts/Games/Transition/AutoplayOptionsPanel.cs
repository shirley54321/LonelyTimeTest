using System;
using UnityEngine;

namespace SlotTemplate {

    public abstract class AutoplayOptionsPanel : MonoBehaviour {

        public event EventHandler<EventArgs> PanelOpended;
        public event EventHandler<AutoplayHandler.AutoplayEventArgs> StartAutoplayButtonClicked;


        protected AutoplayHandler.AutoPlayOptions _currentAutoplayOptions = AutoplayHandler.AutoPlayOptions.defaultValue;


        protected virtual void OnEnable () {
            if (PanelOpended != null) {
                PanelOpended(this, EventArgs.Empty);
            }
        }



        public void OnAdvanced () {
            SetupCurrentAutoplayOptions();

            if (StartAutoplayButtonClicked != null) {
                StartAutoplayButtonClicked(this, new AutoplayHandler.AutoplayEventArgs{ autoplayOptions = _currentAutoplayOptions });
            }

            gameObject.SetActive(false);
        }


        protected abstract void SetupCurrentAutoplayOptions ();


    }
}
