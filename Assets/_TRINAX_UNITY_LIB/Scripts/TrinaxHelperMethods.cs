using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

using System;
using System.Linq;

public static class TrinaxHelperMethods
{
    public static void ChangeLevel(SCENE scene, Action callback = null)
    {
        TrinaxGlobal.Instance.scene = scene;

        TrinaxGlobal.Instance.isChangingLevels = true;
        //TrinaxGlobal.Instance.doneLoadComponentReferences = false;

        int index = (int)scene;
        TrinaxLoadSceneAsync.Instance.LoadLevel(index, ()=> 
        {
            callback?.Invoke();
            TrinaxGlobal.Instance.isChangingLevels = false;
        });
    }

    public static async void ChangeSceneToMain()
    {
        await new WaitForSeconds(5f);
        ChangeLevel(SCENE.MAIN, null);
    }

    static async void FadeText(TextMeshProUGUI text, string msg, float fadeInDuration, float fadeOutDuration, System.Action callback = null)
    {
        text.alpha = 0;
        text.text = "";

        text.text = msg;
        text.DOFade(1.0f, fadeInDuration);

        await new WaitForSeconds(2.0f);
        text.DOFade(0.0f, fadeOutDuration).OnComplete(() =>
        {
            callback?.Invoke();
        });
    }

    public static void ShowFadeText(TextMeshProUGUI text, string msg, float fadeInDuration, float fadeOutDuration, System.Action callback = null)
    {
        FadeText(text, msg, fadeInDuration, fadeOutDuration, callback);
    }

    public static string FloatToTime(float toConvert, string format)
    {
        switch (format)
        {
            case "00.0":
                return string.Format("{0:00}:{1:0}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
                break;
            case "#0.0":
                return string.Format("{0:#0}:{1:0}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
                break;
            case "00.00":
                return string.Format("{0:00}:{1:00}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
                break;
            case "00.000":
                return string.Format("{0:00}:{1:000}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                break;
            case "#00.000":
                return string.Format("{0:#00}:{1:000}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                break;
            case "#0:00":
                return string.Format("{0:#0}:{1:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60);//seconds
                break;
            case "#00:00":
                return string.Format("{0:#00}:{1:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60);//seconds
                break;
            case "0:00.0":
                return string.Format("{0:0}:{1:00}.{2:0}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
                break;
            case "#0:00.0":
                return string.Format("{0:#0}:{1:00}:{2:0}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
                break;
            case "0:00.00":
                return string.Format("{0:0} : {1:00} : {2:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
                break;
            case "#0:00.00":
                return string.Format("{0:#0}:{1:00}.{2:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
                break;
            case "0:00.000":
                return string.Format("{0:0}:{1:00}.{2:000}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                break;
            case "#0:00.000":
                return string.Format("{0:#0}:{1:00}.{2:000}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                break;
        }
        return "error";
    }

    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    // get max distance between 2 positions
    public static float getSqrDistance(Vector3 v1, Vector3 v2)
    {
        return (v1 - v2).sqrMagnitude;
    }

    // map values from 0 to 1 based on value and max value
    public static float mapValue(float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
    {
        return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
    }

    public static float Map(this float value, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
    {
        return (value - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
    }

    public static float Map01(this float value, float min, float max, bool clamp = false)
    {
        if (clamp) value = Math.Max(min, Math.Min(value, max));
        return (value - min) / (max - min);
    }

    public static float RoundDecimal(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, digits);
        return Mathf.Round(value * mult) / mult;
    }

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Vector3 LerpOverDistance(Vector3[] vectors, float time)
    {
        time = Mathf.Clamp01(time);
        if (vectors == null || vectors.Length == 0)
        {
            throw (new Exception("Vectors input must have at least one value"));
        }
        if (vectors.Length == 1)
        {
            return vectors[0];
        }

        if (time == 0)
        {
            return vectors[0];
        }

        if (time == 1)
        {
            return vectors[vectors.Length - 1];
        }

        float[] distances = new float[vectors.Length - 1];
        float total = 0;
        for (int i = 0; i < vectors.Length; i++)
        {
            distances[i] = (vectors[i] - vectors[i + 1]).sqrMagnitude;
            total += distances[i];
        }

        float current = total * time;
        int p = 0;
        while (current - distances[p] > 0)
        {
            current -= distances[p++];
        }
        if (distances[p] == 0) return vectors[p];

        return Vector3.Lerp(vectors[p], vectors[p + 1], current / distances[p]);
    }

    public static Vector3 LerpOverNumber(Vector3[] vectors, float time)
    {
        time = Mathf.Clamp01(time);
        if (vectors == null || vectors.Length == 0)
        {
            throw (new Exception("Vectors input must have at least one value"));
        }
        if (vectors.Length == 1)
        {
            return vectors[0];
        }

        if (time == 0)
        {
            return vectors[0];
        }

        if (time == 1)
        {
            return vectors[vectors.Length - 1];
        }

        float t = time * vectors.Length;
        int p = (int)Mathf.Floor(t);
        t -= p;
        return Vector3.Lerp(vectors[p], vectors[p + 1], t);
    }
}
