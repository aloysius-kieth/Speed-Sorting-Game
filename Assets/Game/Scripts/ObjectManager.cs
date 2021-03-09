using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public enum OBJECT_TYPE
{
    Huat_Kueh,
    Ang_Pao,
    Chun_Lian,
    Firecracker,
    Mandarin_Orange,
    Pineapple_Tart,
}

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject obj;
    public string poolName;
    public int amount;
    public bool Expandable;
}

public class ObjectManager : MonoBehaviour
{
    public bool IsReady { get; set; }

    [SerializeField]
    float boxOffsetXRight;
    [SerializeField]
    float boxOffSetXLeft;

    [Header("Lists")]
    public List<GameObject> Huat_Kueh_List = new List<GameObject>();
    public List<GameObject> Ang_Pao_List = new List<GameObject>();
    public List<GameObject> Chun_Lian_List = new List<GameObject>();
    public List<GameObject> Firecracker_List = new List<GameObject>();
    public List<GameObject> Mandarin_Oranges_List = new List<GameObject>();
    public List<GameObject> Pineapple_List = new List<GameObject>();

    public List<ObjectPoolItem> ItemsToPool = new List<ObjectPoolItem>();

    [Header("Amounts")]
    public int huat_kueh_amount;
    public int ang_pao_amount;
    int non_scoreable_amt = 100;

    int current_non_scoreable_amt;
    int current_huat_kueh_amt;
    int current_ang_pao_amt;

    [Header("Components")]
    public ScreenBoundary screenBoundary;
    public Transform environmentContainer;
    GameObject parentContainer = null;

    int non_scoreable_amt_min, non_scoreable_amt_max;

    public void Init()
    {
        IsReady = false;
        parentContainer = new GameObject("Object_Container");
        parentContainer.transform.SetParent(environmentContainer);
        CreatePool();
        GameEvents.SpawnObjectEvent += SpawnObjectEvent;
        IsReady = true;
    }

    public void Populate(GameSettings settings)
    {
        huat_kueh_amount = settings.numberOfHuatKuehToSpawn;
        ang_pao_amount = settings.numberOfAngPaoToSpawn;
        non_scoreable_amt = settings.objectTotalAmount;
        non_scoreable_amt_min = settings.non_scoreable_amt_min;
        non_scoreable_amt_max = settings.non_scoreable_amt_max;
    }

    void CreatePool()
    {
        foreach (ObjectPoolItem item in ItemsToPool)
        {
            for (int i = 0; i < item.amount; i++)
            {
                GameObject obj = CreatePooledObject(item, i);
                obj.SetActive(false);
            }
        }
    }

    GameObject GetFromPool(List<GameObject> pool, string poolName = "")
    {
        if (pool.Count > 0)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeSelf && !pool[i].GetComponent<ObjectBase>().inUse)
                {
                    pool[i].GetComponent<ObjectBase>().inUse = true;
                    //Debug.Log("Getting from pool " + poolName);
                    return pool[i];
                }
            }

            foreach (ObjectPoolItem item in ItemsToPool)
            {
                if (item.poolName == poolName)
                {
                    if (item.Expandable)
                    {
                        item.obj.GetComponent<ObjectBase>().inUse = true;
                        return CreatePooledObject(item, GetHighestSortingOrder(pool) + 1);
                    }
                }
            }
        }
        return null;
    }

    public async void Spawn()
    {
        current_non_scoreable_amt = non_scoreable_amt;

        List<GameObject> spawnList = new List<GameObject>();
        // spawn all huat kueh and ang pao first
        if (huat_kueh_amount <= Huat_Kueh_List.Count)
        {
            for (int i = 0; i < huat_kueh_amount; i++)
            {
                GameObject obj = GetFromPool(Huat_Kueh_List);
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
                spawnList.Add(obj);
            }
        }

        if (ang_pao_amount <= Ang_Pao_List.Count)
        {
            for (int i = 0; i < ang_pao_amount; i++)
            {
                GameObject obj = GetFromPool(Ang_Pao_List);
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
                spawnList.Add(obj);
            }
        }

        // set initial count
        current_huat_kueh_amt = huat_kueh_amount;
        current_ang_pao_amt = ang_pao_amount;

        //current_non_scoreable_amt = current_non_scoreable_amt - (ang_pao_amount + huat_kueh_amount);

        for (int i = 0; i < current_non_scoreable_amt; i++)
        {
            int index = Random.Range((int)OBJECT_TYPE.Chun_Lian, (int)OBJECT_TYPE.Pineapple_Tart + 1);
            if ((OBJECT_TYPE)index == OBJECT_TYPE.Chun_Lian)
            {
                GameObject obj = GetFromPool(Chun_Lian_List);
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
                spawnList.Add(obj);
            }
            else if ((OBJECT_TYPE)index == OBJECT_TYPE.Firecracker)
            {
                GameObject obj = GetFromPool(Firecracker_List);
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
                spawnList.Add(obj);
            }
            else if ((OBJECT_TYPE)index == OBJECT_TYPE.Mandarin_Orange)
            {
                GameObject obj = GetFromPool(Mandarin_Oranges_List);
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
                spawnList.Add(obj);
            }
            else if ((OBJECT_TYPE)index == OBJECT_TYPE.Pineapple_Tart)
            {
                GameObject obj = GetFromPool(Pineapple_List);
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
                spawnList.Add(obj);
            }
        }

        int playIndex = 0;
        if (spawnList.Count > 0)
        {
            for (int i = 0; i < spawnList.Count; i++)
            {
                Transform t = spawnList[i].transform;
                t.GetComponent<SpriteRenderer>().sortingOrder += i;
                if (i % 100 == 0)
                {
                    await new WaitForSeconds(1.2f);
                    playIndex = Random.Range((int)TrinaxAudioManager.AUDIOS.POP_1, (int)TrinaxAudioManager.AUDIOS.POP_3 + 1);

                    TrinaxAudioManager.Instance.PlaySFX((TrinaxAudioManager.AUDIOS)playIndex, TrinaxAudioManager.AUDIOPLAYER.SFX);
                }
                t.DOScale(Vector3.one * 0.8f, 0.75f).SetEase(Ease.OutElastic).OnComplete(() => { t.GetComponent<ObjectBase>().Draggable = true; });
            }
        }

        await new WaitForSeconds(1f);
        spawnList.Clear();
        GameEvents.OnCountdownStartEvent?.Invoke();
    }

    void SpawnScorables(OBJECT_TYPE type, int amt)
    {
        List<GameObject> temp = new List<GameObject>();
        if (type == OBJECT_TYPE.Huat_Kueh)
            current_huat_kueh_amt += amt;
        else
            current_ang_pao_amt += amt;

        for (int i = 0; i < amt; i++)
        {
            GameObject obj = null;
            if (type == OBJECT_TYPE.Huat_Kueh)
                obj = GetFromPool(Huat_Kueh_List, "huatkuehpool");
            else
                obj = GetFromPool(Ang_Pao_List, "angpaopool");

            temp.Add(obj);
        }

        if (temp.Count > 0)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                GameObject obj = temp[i];
                if (obj != null)
                {
                    obj.transform.position = GetRandomPosition();
                    obj.transform.localScale = Vector3.zero;
                    obj.SetActive(true);
                    obj.transform.DOScale(Vector3.one * 0.8f, 0.75f).SetEase(Ease.OutElastic).OnComplete(() => { obj.GetComponent<ObjectBase>().Draggable = true; });
                    //Debug.Log("spawned " + obj.name);
                }
            }
        }
        temp.Clear();
        temp = null;
    }

    void SpawnUnScoreables(int amt)
    {
        List<GameObject> temp = new List<GameObject>();
        List<int> SortOrders = new List<int>();

        for (int i = 0; i < amt; i++)
        {
            GameObject obj = null;
            int index = Random.Range((int)OBJECT_TYPE.Chun_Lian, (int)OBJECT_TYPE.Pineapple_Tart + 1);
            OBJECT_TYPE type = (OBJECT_TYPE)index;
            if (type == OBJECT_TYPE.Chun_Lian)
            {
                SortOrders.Add(GetHighestSortingOrder(Chun_Lian_List));
                obj = GetFromPool(Chun_Lian_List, "chunlianpool");
            }
            else if (type == OBJECT_TYPE.Mandarin_Orange)
            {
                SortOrders.Add(GetHighestSortingOrder(Mandarin_Oranges_List));
                obj = GetFromPool(Mandarin_Oranges_List, "orangepool");
            }
            else if (type == OBJECT_TYPE.Firecracker)
            {
                SortOrders.Add(GetHighestSortingOrder(Firecracker_List));
                obj = GetFromPool(Firecracker_List, "firecrackerpool");
            }
            else if (type == OBJECT_TYPE.Pineapple_Tart)
            {
                SortOrders.Add(GetHighestSortingOrder(Pineapple_List));
                obj = GetFromPool(Pineapple_List, "tartpool");
            }
            temp.Add(obj);
        }

        if (temp.Count > 0)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                GameObject obj = temp[i];
                if (obj != null)
                {
                    obj.GetComponent<SpriteRenderer>().sortingOrder = SortOrders[i] + 1;
                    obj.transform.position = GetRandomPosition();
                    obj.SetActive(true);
                    obj.transform.DOScale(Vector3.one * 0.8f, 0.75f).SetEase(Ease.OutElastic).OnComplete(() => { obj.GetComponent<ObjectBase>().Draggable = true; });
                    //Debug.Log("spawned " + obj.name);
                }
            }
        }
        SortOrders.Clear();
        SortOrders = null;
        temp.Clear();
        temp = null;
    }

    GameObject CreatePooledObject(ObjectPoolItem item, int sortIndex)
    {
        GameObject obj = Instantiate(item.obj) as GameObject;
        obj.transform.SetParent(GetSubParentPoolContainer(item.poolName).transform, false);
        //obj.GetComponent<SpriteRenderer>().sortingOrder += sortIndex;
        obj.transform.localScale = Vector3.zero;
        obj.SetActive(true);

        SortIntoLists(obj, obj.GetComponent<ObjectBase>().type);
        return obj;
    }

    public void ReturnAllToPool()
    {
        DeactivateList(Huat_Kueh_List);
        DeactivateList(Ang_Pao_List);
        DeactivateList(Chun_Lian_List);
        DeactivateList(Firecracker_List);
        DeactivateList(Mandarin_Oranges_List);
        DeactivateList(Pineapple_List);
    }

    void DeactivateList(List<GameObject> list)
    {
        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                GameObject obj = list[i];
                obj.transform.localScale = Vector3.zero;
                obj.GetComponent<SpriteRenderer>().sortingOrder = 0;
                obj.GetComponent<ObjectBase>().inUse = false;
                obj.SetActive(false);
            }
        }
    }

    void SortIntoLists(GameObject obj, OBJECT_TYPE type)
    {
        switch (type)
        {
            case OBJECT_TYPE.Huat_Kueh:
                Huat_Kueh_List.Add(obj);
                break;
            case OBJECT_TYPE.Ang_Pao:
                Ang_Pao_List.Add(obj);
                break;
            case OBJECT_TYPE.Chun_Lian:
                Chun_Lian_List.Add(obj);
                break;
            case OBJECT_TYPE.Firecracker:
                Firecracker_List.Add(obj);
                break;
            case OBJECT_TYPE.Mandarin_Orange:
                Mandarin_Oranges_List.Add(obj);
                break;
            case OBJECT_TYPE.Pineapple_Tart:
                Pineapple_List.Add(obj);
                break;
        }
    }

    GameObject GetSubParentPoolContainer(string name)
    {
        GameObject subParentObj = null;
        // use root object pool name if no name specifed
        if (string.IsNullOrEmpty(name))
        {
            name = parentContainer.name;
            subParentObj = GameObject.Find(name);
            return subParentObj;
        }
        else
        {
            subParentObj = GameObject.Find(name);
            if (subParentObj == null)
            {
                subParentObj = new GameObject(name);
                // Must not be the same name as root object
                if (name != parentContainer.name)
                {
                    subParentObj.transform.SetParent(parentContainer.transform, false);
                }
            }
        }
        return subParentObj;
    }

    int GetHighestSortingOrder(List<GameObject> objectList)
    {
        int highestOrder = int.MinValue;
        if (objectList.Count > 0)
        {
            foreach (GameObject item in objectList)
            {
                int temp = item.GetComponent<SpriteRenderer>().sortingOrder;
                if (temp > highestOrder)
                {
                    highestOrder = temp;
                }
            }
        }
        else
        {
            Debug.LogWarning("No object to get sorting order from");
        }

        return highestOrder;
    }

    Vector3 GetRandomPosition()
    {
        //if (Random.value > 0.4f)
        //{
            return RandomPointInBox(screenBoundary.spawnAreaBoundary[0].bounds.center, screenBoundary.spawnAreaBoundary[0].bounds.size);
        //}
        //else
        //{
        //    return RandomPointInBox(screenBoundary.spawnAreaBoundary[1].bounds.center, screenBoundary.spawnAreaBoundary[1].bounds.size);
        //}
    }

    Vector3 RandomPointInBox(Vector3 center, Vector3 size)
    {
        float offsetX = 0;
        // is right side
        if (AppManager.Instance.uiManager.rightHandSelectionToggle.isOn)
            offsetX = boxOffsetXRight;
        else
            offsetX = boxOffSetXLeft;

        return center + new Vector3(
           (Random.value - offsetX) * size.x,
           (Random.value - 0.5f) * size.y,
           (Random.value - 0.5f) * size.z
        );
    }

    void SpawnObjectEvent()
    {
        if (AppManager.Instance.gameManager.IsGameover) return;
        SpawnUnScoreables(Random.Range(50, 50));
    }

    public void UpdateScoreableAmount(OBJECT_TYPE type)
    {
        if (type == OBJECT_TYPE.Huat_Kueh)
            current_huat_kueh_amt--;
        else if (type == OBJECT_TYPE.Ang_Pao)
            current_ang_pao_amt--;

        if (current_huat_kueh_amt <= (huat_kueh_amount / 2)) SpawnScorables(OBJECT_TYPE.Huat_Kueh, Random.Range(non_scoreable_amt_min, non_scoreable_amt_max));
        else if(current_ang_pao_amt <= (ang_pao_amount / 2)) SpawnScorables(OBJECT_TYPE.Ang_Pao, Random.Range(5, 10));
    }
}
