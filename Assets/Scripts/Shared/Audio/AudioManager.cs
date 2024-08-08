using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    [Header("主題曲")]
    public AudioClip LoginmusicClip;
    public AudioClip GameLobbymusicClip;
    public AudioClip MachineLobbymusicClip;

    [Header("Source")]
    public AudioSource LoginmusicSource;
    public AudioSource GameLobbymusicSource;
    public AudioSource MachineLobbymusicSource;
    private static AudioManager current;

    public static AudioManager Current
    {
        get
        {
            if (current == null)
            {
                current = FindObjectOfType<AudioManager>();

                if (current == null)
                {
                    Debug.LogError($"The GameObject with {typeof(AudioManager)} does not exist in the scene, yet its method is being called.\n" +
                                    $"Please add {typeof(AudioManager)} to the scene.");
                }
            }

            return current;
        }
    }

    private void Awake() {

        if (current == null)
        {
            current = this;
            DontDestroyOnLoad(gameObject);
            LoginmusicSource=gameObject.AddComponent<AudioSource>();
            GameLobbymusicSource=gameObject.AddComponent<AudioSource>();
            MachineLobbymusicSource = gameObject.AddComponent<AudioSource>();
            
            PlayLoginBGM();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameSettings.instance.MusicSwitch += HandleMusicSwitch;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameSettings.instance.MusicSwitch -= HandleMusicSwitch;
    }
    private void HandleMusicSwitch()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        StopMusic();
        if(sceneName=="Login"||sceneName=="Start"){PlayLoginBGM();}
        else if(sceneName=="GameLobby"){PlayGameLobbyBGM();}
        else if (sceneName == "MachineLobby"){PlayMachineLobbyBGM();}
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(!(IsMusicPlaying()==LoginmusicSource.isPlaying && (scene.name=="Login"||scene.name=="Start"))){
            StopMusic();
            if(scene.name=="Login"||scene.name=="Start"){PlayLoginBGM();}
            else if(scene.name=="GameLobby"){PlayGameLobbyBGM();}
            else if (scene.name == "MachineLobby"){PlayMachineLobbyBGM();}
        }
    }
    //背景音樂
    public void PlayLoginBGM() => PlayBGM(LoginmusicSource, LoginmusicClip);
    public void PlayGameLobbyBGM() => PlayBGM(GameLobbymusicSource, GameLobbymusicClip);
    public void PlayMachineLobbyBGM() => PlayBGM(MachineLobbymusicSource, MachineLobbymusicClip);        
    private void PlayBGM(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.loop = true;
        source.volume = GameSettings.instance.Music ? 1f : 0f;
        source.Play();
    }

    private void StopMusic(){
        LoginmusicSource.Stop();
        GameLobbymusicSource.Stop();
        MachineLobbymusicSource.Stop();
    }
    public bool IsMusicPlaying()
    {
        return LoginmusicSource.isPlaying || GameLobbymusicSource.isPlaying || MachineLobbymusicSource.isPlaying;
    }

}
