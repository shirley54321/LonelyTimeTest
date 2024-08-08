using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WushihFakeReelLocator : MonoBehaviour
{
   public GameObject targetReel;
    // Update is called once per frame
    void Update()
    {
        transform.position = targetReel.transform.position;
    }
}
