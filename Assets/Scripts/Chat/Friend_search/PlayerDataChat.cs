using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDataChat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DeleteFriend(GameObject selfGameObject)
    {
        string playFabID = selfGameObject.GetComponent<TMP_Text>().text;
        PlaterSearch.RemoveFriend(playFabID);
    }
    
}
