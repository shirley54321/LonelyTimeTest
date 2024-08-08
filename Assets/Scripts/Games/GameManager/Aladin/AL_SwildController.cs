using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System;

public class AL_SwildController : MonoBehaviour
{
    public float swildDisappearDelay = 1.0f;  
    [SerializeField] SwildReel[] showSwildAnimationObj;
    DOTweenPath path;
    public void StartWaitSmokeDisappearCoroutine(Vector2Int smokeID)
    {
        StartCoroutine(WaitSmokeDisappear(smokeID));
    }
    IEnumerator WaitSmokeDisappear(Vector2Int smokeID)
    {
        yield return new WaitForSeconds(swildDisappearDelay);
        GameObject SwildObj = showSwildAnimationObj[smokeID.x].SwildObj[smokeID.y];
        ParticleSystem smokeParticleSystem = SwildObj.transform.GetChild(0).GetComponent<ParticleSystem>();
        smokeParticleSystem.Stop();
        yield return new WaitUntil(() => smokeParticleSystem.particleCount == 0);  
        SwildObj.SetActive(false);
    }
    public void StartSmokeAnimationPlaying(Vector2Int[] smokeID)
    {
        for (int i = 0; i < smokeID.Length; i++)
        {
            if (showSwildAnimationObj[smokeID[i].x].SwildObj[smokeID[i].y] != null)
            {
                GameObject SwildObj = showSwildAnimationObj[smokeID[i].x].SwildObj[smokeID[i].y];
                SwildObj.SetActive(true);
                path = SwildObj.GetComponent<DOTweenPath>();
                path.DORestart();
                ParticleSystem smokeParticleSystem = SwildObj.transform.GetChild(0).GetComponent<ParticleSystem>();
                smokeParticleSystem.Play();
                StartWaitSmokeDisappearCoroutine(smokeID[i]);
            }
        }
        
        
    }
    private void Start()
    {
        //StartSmokeAnimationPlaying(id);
    }
    //private void OnEnable()
    //{
    //    StartSmokeAnimationPlaying(id);
    //}
}

[Serializable]
public class SwildReel
{
    public GameObject[] SwildObj;
}
