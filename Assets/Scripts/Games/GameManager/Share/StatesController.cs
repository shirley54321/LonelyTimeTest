using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace SlotTemplate
{
    public class StatesController
    {

        // == Events ==
        public event EventHandler<StateChangedEventArgs> StateChanged;


        // == Public Variables ==
        public IEnumerable<State> states;
        public string nextStateName;

        public string currentStateName { get; private set; } = "";
        public bool IsRunning => _currentMainRunning != null;

        MonoBehaviour _coroutineCarrier;
        Coroutine _currentMainRunning;
        Coroutine _currentSubRunning;
        

        public StatesController(MonoBehaviour coroutineCarrier, IEnumerable<State> states)
        {
            _coroutineCarrier = coroutineCarrier;
            this.states = states;
        }


        public void Run(string startingStateName)
        {
            if (_currentMainRunning == null && _coroutineCarrier != null)
            {
                nextStateName = startingStateName;
                _currentMainRunning = _coroutineCarrier.StartCoroutine(Running());
            }
        }

        public void Shutdown()
        {
            if (_currentSubRunning != null)
            {
                _coroutineCarrier.StopCoroutine(_currentSubRunning);
            }
            if (_currentMainRunning != null)
            {
                _coroutineCarrier.StopCoroutine(_currentMainRunning);
            }
            _currentSubRunning = null;
            _currentMainRunning = null;
        }


        IEnumerator Running()
        {
            while (true)
            {
                State state = GetNextstate();

                if (state == null)
                {
                    Debug.LogError($"State name \"{nextStateName}\" not found.");
                }
                else
                {
                    currentStateName = state.name;
                }

                if (StateChanged != null)
                {
                    StateChanged(this, new StateChangedEventArgs { newStateName = currentStateName });
                }

                if (state.runningGetter != null)
                {
                    _currentSubRunning = _coroutineCarrier.StartCoroutine(state.runningGetter.Invoke(this));
                    yield return _currentSubRunning;
                }

            }

        }

        State GetNextstate()
        {
            return State.GetStateFromIEnumerableByName(states, nextStateName);
        }



        public class StateChangedEventArgs : EventArgs
        {
            public string newStateName;
        }

    }
}
