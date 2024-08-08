using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameSettings : MonoBehaviour
{
    public event Action MusicSwitch;
    public event Action SoundSwitch;
    public bool Music { get; set; }
    public bool Sound { get; set; }
    public bool Notifications { get; set; }
    public bool Chat_Pop_up { get; set; }
    public bool Grand_Prize_Sharing { get; set; }
    public bool Grand_Prize_Push_Notification { get; set; }
    public bool Level_Up_Push_Notification { get; set; }

    public static GameSettings instance;

    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameSettings>();

                if (instance == null)
                {
                    Debug.LogError($"The GameObject of type {typeof(GameSettings)} does not exist in the scene, yet its method is being called.\n" +
                                    $"Please add {typeof(GameSettings)} to the scene.");
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

        LoadSettings();
    }

    private void LoadSettings()
    {
        Music = PlayerPrefs.GetInt("Music", 1) == 1;
        Sound = PlayerPrefs.GetInt("Sound", 1) == 1;
        Notifications = PlayerPrefs.GetInt("Notifications", 1) == 1;
        Chat_Pop_up = PlayerPrefs.GetInt("Chat_Pop_up", 1) == 1;
        Grand_Prize_Sharing = PlayerPrefs.GetInt("Grand_Prize_Sharing", 1) == 1;
        Grand_Prize_Push_Notification = PlayerPrefs.GetInt("Grand_Prize_Push_Notification", 1) == 1;
        Level_Up_Push_Notification = PlayerPrefs.GetInt("Level_Up_Push_Notification", 1) == 1;
    //     Debug.Log($"Music: {Music}, Sound: {Sound}, Notifications: {Notifications}, " +
    //             $"Chat_Pop_up: {Chat_Pop_up}, Grand_Prize_Sharing: {Grand_Prize_Sharing}, " +
    //             $"Grand_Prize_Push_Notification: {Grand_Prize_Push_Notification}, " +
    //             $"Level_Up_Push_Notification: {Level_Up_Push_Notification}");
    }

    public void Save(string settingName)
    {
        switch (settingName)
        {
            case "Music":
                PlayerPrefs.SetInt("Music", Music ? 1 : 0);
                MusicSwitch?.Invoke();
                break;
            case "Sound":
                PlayerPrefs.SetInt("Sound", Sound ? 1 : 0);
                SoundSwitch?.Invoke();
                break;
            case "Notifications":
                PlayerPrefs.SetInt("Notifications", Notifications ? 1 : 0);
                break;
            case "Chat_Pop_up":
                PlayerPrefs.SetInt("Chat_Pop_up", Chat_Pop_up ? 1 : 0);
                break;
            case "Grand_Prize_Sharing":
                PlayerPrefs.SetInt("Grand_Prize_Sharing", Grand_Prize_Sharing ? 1 : 0);
                break;
            case "Grand_Prize_Push_Notification":
                PlayerPrefs.SetInt("Grand_Prize_Push_Notification", Grand_Prize_Push_Notification ? 1 : 0);
                break;
            case "Level_Up_Push_Notification":
                PlayerPrefs.SetInt("Level_Up_Push_Notification", Level_Up_Push_Notification ? 1 : 0);
                break;
        }
        PlayerPrefs.Save();
    
        // Debug.Log($"Music: {Music}, Sound: {Sound}, Notifications: {Notifications}, " +
        //         $"Chat_Pop_up: {Chat_Pop_up}, Grand_Prize_Sharing: {Grand_Prize_Sharing}, " +
        //         $"Grand_Prize_Push_Notification: {Grand_Prize_Push_Notification}, " +
        //         $"Level_Up_Push_Notification: {Level_Up_Push_Notification}");
    }

}