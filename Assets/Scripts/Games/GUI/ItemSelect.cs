using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace SlotTemplate {
    public class ItemSelect : MonoBehaviour
    {
        bool hasOpen = false;
        bool isAni = false;
        [SerializeField] CanvasGroup[] canvasGroup;

        public void OnClick()
        {
            if (!isAni) StartCoroutine(Show());
        }
        IEnumerator Show()
        {
            isAni = true;

            for (int i = 0; i < canvasGroup.Length; i++)
            {
                canvasGroup[i].GetComponent<Button>().enabled = true;

                Sequence seq = DOTween.Sequence();
                if (hasOpen) canvasGroup[i].DOFade(0, 0.3f);
                else canvasGroup[i].DOFade(1, 0.2f);
                seq.Append(canvasGroup[i].transform.DOMove(new Vector3(canvasGroup[i].transform.position.x, canvasGroup[i].transform.position.y + 0.2f, canvasGroup[i].transform.position.z), 0.1f));
                seq.Append(canvasGroup[i].transform.DOMove(new Vector3(canvasGroup[i].transform.position.x, canvasGroup[i].transform.position.y, canvasGroup[i].transform.position.z), 0.1f));
                canvasGroup[i].GetComponent<Button>().enabled = hasOpen ? false : true; 
                yield return new WaitForSeconds(0.1f);
            }

            hasOpen = !hasOpen;
            yield return new WaitForSeconds(0.2f);
            isAni = false;
        }
    }
}
