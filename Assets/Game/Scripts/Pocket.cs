using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Pocket : MonoBehaviour
{
    Vector3 originalScale;
    public Transform graphic;
    public ParticleSystem glowPS;
    public ParticleSystem two_pointsPS;
    public ParticleSystem five_pointsPS;
    public ParticleSystem minus_two_pointsPS;

    void Awake()
    {
        originalScale = graphic.transform.localScale;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        //graphic.transform.localScale = Vector3.zero;
        //graphic.DOScale(originalScale, 0.25f).SetEase(Ease.OutElastic);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (AppManager.Instance.gameManager.IsGameover) return;

        if (graphic.transform.localScale != originalScale)
            graphic.transform.localScale = originalScale;

        graphic.DOScale(Vector3.one * 1.1f, 0.25f).OnComplete(() =>
        {
            graphic.DOScale(originalScale, 0.1f);
        }).SetEase(Ease.OutElastic);

        if (col.tag == "scoreableObj")
        {
            glowPS.Play();
            if (col.GetComponent<ObjectBase>().type == OBJECT_TYPE.Huat_Kueh) five_pointsPS.Play();
            else if (col.GetComponent<ObjectBase>().type == OBJECT_TYPE.Ang_Pao) two_pointsPS.Play();

            //Debug.Log("Scored: " + col.name);
            int pointsToAdd = col.GetComponent<ObjectBase>().point;
            col.gameObject.SetActive(false);
            col.GetComponent<ObjectBase>().inUse = false;
            AppManager.Instance.scoreManager.AddScore(pointsToAdd);

            GameEvents.OnScoredEvent?.Invoke(col.GetComponent<ObjectBase>().type);

            if (!AppManager.Instance.gameManager.comboCounter.ComboStarted)
            {
                GameEvents.OnComboStartEvent?.Invoke();
            }

            // if combo event already started
            if (AppManager.Instance.gameManager.comboCounter.ComboStarted)
            {
                GameEvents.OnComboUpdateEvent?.Invoke();
            }
        }
        if (col.tag == "non-scoreableObj")
        {
            minus_two_pointsPS.Play();
            int pointsToMinus = col.GetComponent<ObjectBase>().point;
            col.gameObject.SetActive(false);
            col.GetComponent<ObjectBase>().inUse = false;
            AppManager.Instance.scoreManager.MinusScore(-pointsToMinus);

            if (AppManager.Instance.gameManager.comboCounter.ComboStarted)
            {
                GameEvents.OnComboCancelledEvent?.Invoke();
            }
        }
    }
}
