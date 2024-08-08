using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{

    public class State {


        public string name {get; private set;}
        public Func<StatesController, IEnumerator> runningGetter {get; private set;}


        public static State GetStateFromIEnumerableByName (IEnumerable<State> states, string name) {
            foreach (State state in states) {
                if (state.name == name) {
                    return state;
                }
            }
            return null;
        }


        public State (string name, Func<StatesController, IEnumerator> runningGetter) {
            this.name = name;
            this.runningGetter = runningGetter;
        }


    }

}
