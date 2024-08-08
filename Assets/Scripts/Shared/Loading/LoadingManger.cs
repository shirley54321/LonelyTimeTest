using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Loading{
    public class LoadingManger : MonoBehaviour
    {
        public GameObject LA;
        public LoadingScreen LoadingScreenA;
        public Loading_animator Loading_animatorA;
        public static LoadingManger instance;
        public static LoadingManger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LoadingManger>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject of type {typeof(LoadingManger)} does not exist in the scene, yet its method is being called.\n" +
                                        $"Please add {typeof(LoadingManger)} to the scene.");
                    }
                    DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

            LoadingScreenA = GetComponent<LoadingScreen>();
            if (LoadingScreenA == null)
            {
                Debug.LogError($"LoadingScreen component is missing on {gameObject.name}. Please attach the component.");
            }

            Loading_animatorA = GetComponent<Loading_animator>();
            if (Loading_animatorA == null)
            {
                Debug.LogError($"Loading_animator component is missing on {gameObject.name}. Please attach the component.");
            }
            Loading_animatorA.LaObject = LA;
            LA.SetActive(true);
            LA.SetActive(false);
        }
        public void SetLoadingNormolScreen(string leveltoLoad){
            LoadingScreenA.LoadTheScreen(leveltoLoad);
        }
        public void SetLoadingGameScreen(string leveltoLoad){
            LoadingScreenA.LoadTheGameScreen(leveltoLoad);
        }
        public void Open_Loading_animator(){
            Loading_animatorA.OpenObject();
        }
        public void Close_Loading_animator(){
            Loading_animatorA.CloseObject();
        }        

    }
}

