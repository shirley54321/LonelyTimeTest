using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UserAccount;
namespace Loading{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Menu")]
        //主物件
        [SerializeField] private GameObject loadingScreen; 

        [Header("LoadingBar")]
        //被控制的進度條
        [SerializeField] private Slider loadingSlider;

        [Header("Image")]
        //被切換背景的圖片
        [SerializeField] private Image slideshow;
        //可切換的圖片選項
        [SerializeField] private Sprite[] images = new Sprite[3];


        [Header("MachineLobby")]
        //Wu>Chu>Ala>Lion>Pom>Clown>Moon>Eg>Pola
        [SerializeField] private GameObject GameloadingScreen; 
        [SerializeField] private Sprite[] Gameimages = new Sprite[9];

        [SerializeField] private Slider GameloadingSlider;
        [SerializeField] private Image Gameslideshow;
        //當前顯示的圖片
        private int currentImageIndex;

        //圖片切換的延遲時間
        private float imageChangeInterval = 6.0f;
        IEnumerator Start()
        {
            var InitAddressablesAsync = Addressables.InitializeAsync();
            yield return InitAddressablesAsync;
        }      

        //用於一般場景載入  
        public void LoadTheScreen(string leveltoLoad) 
        {
            if (loadingScreen != null)
            {
                loadingSlider.value = 0f;
                loadingScreen.SetActive(true);
                currentImageIndex = Random.Range(0, 3);
                StartCoroutine(ChangeSlideshowImage());
                StartCoroutine(LoadToSence(leveltoLoad)); 
            }
            else
            {
                Debug.Log("loadingScreen is null.");
            }
        }
        private IEnumerator LoadToSence(string leveltoLoad)
        {
            yield return LoadToSencelAsync(leveltoLoad);
        }
        public async Task LoadToSencelAsync(string leveltoLoad) {
            string result = await Data_Validation.instance.GenerateSHA256Hash();
            loadingSlider.value = 0.1f;
            Debug.Log("hash in LS : " + result);
            bool loginbool = Data_Validation.canlogin;

            if (loginbool) {
                var handle = Addressables.LoadSceneAsync(leveltoLoad);
                while (!handle.IsDone) {
                    loadingSlider.value = 0.1f + handle.PercentComplete * 0.9f;
                    await Task.Yield();
                }
            } else {
                string currentSceneName = SceneManager.GetActiveScene().name;
                Debug.LogError("資料不一致，currentSceneName : " + currentSceneName);
                if(currentSceneName=="Start"){
                    var handle = Addressables.LoadSceneAsync("Login");
                    while (!handle.IsDone) {
                    loadingSlider.value = 0.1f + handle.PercentComplete * 0.9f;
                    await Task.Yield();
                    }
                }
            }
            UserAccountManager.Instance.InvokeLoginSuccess();
            loadingScreen.SetActive(false);
        }
        IEnumerator ChangeSlideshowImage()
        {
            while (true)
            {
                slideshow.sprite = images[currentImageIndex];
                currentImageIndex = (currentImageIndex + 1) % images.Length;
                yield return new WaitForSeconds(imageChangeInterval);
            }
        }
        
        
        //用於老虎機 場景載入
        public void LoadTheGameScreen(string leveltoLoad) 
        {
            if (GameloadingScreen != null)
            {   //Wu>Chu>Ala>Lion>Pom>Clown>Moon>Eg>Pola
                switch (leveltoLoad)
                {
                    case "WuMayNiang":
                        Gameslideshow.sprite = Gameimages[0];
                        break;
                    case "ChuHeHanJie":
                        Gameslideshow.sprite = Gameimages[1];
                        break;
                    case "ALaDing":
                        Gameslideshow.sprite = Gameimages[2];
                        break;
                    case "LionDance":
                        Gameslideshow.sprite = Gameimages[3];
                        break;
                    case "Pompeii":
                        Gameslideshow.sprite = Gameimages[4];
                        break;
                    case "ClownTrick":
                        Gameslideshow.sprite = Gameimages[5];
                        break;
                    case "MoonWolf":
                        Gameslideshow.sprite = Gameimages[6];
                        break;
                    case "EgyptianDynasty":
                        Gameslideshow.sprite = Gameimages[7];
                        break;
                    case "PolarBear":
                        Gameslideshow.sprite = Gameimages[8];
                        break;
                    default:
                        Debug.LogError("錯誤場景名稱:"+leveltoLoad+"，請進來這裡面檢查上面文字對應圖片");
                        break;
                }                 
                GameloadingSlider.value = 0f;
                GameloadingScreen.SetActive(true);
                StartCoroutine(LoadToGame(leveltoLoad)); 
            }
            else
            {
                Debug.Log("loadingScreen is null.");
            }
        }
        public IEnumerator LoadToGame(string leveltoLoad) {
            var handle = Addressables.LoadSceneAsync(leveltoLoad);
            while (!handle.IsDone) {
                GameloadingSlider.value = handle.PercentComplete*0.95f;
                yield return null;
            }
            UserAccountManager.Instance.InvokeLoginSuccess();
            GameloadingScreen.SetActive(false);
        }        
    }

}
