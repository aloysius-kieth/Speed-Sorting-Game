//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using DG.Tweening;

//public enum OBJECT_TYPE
//{
//    Huat_Kueh,
//    Ang_Pao,
//    Chun_Lian,
//    Firecracker,
//    Mandarin_Orange,
//    Pineapple_Tart,
//}

//[System.Serializable]
//public class ObjectPoolItem
//{
//    public GameObject obj;
//    public string poolName;
//    public int amount;
//    public bool expandable;
//}

//[System.Serializable]
//public class CurrentObjectAmount
//{
//    public int huat_kueh;
//    public int ang_pao;

//    public void Clear()
//    {
//        huat_kueh = 0;
//        ang_pao = 0;
//    }
//}

//public class ObjectSpawner : MonoBehaviour
//{
//    public bool IsReady { get; set; }

//    int min_HuatKueh = 2;
//    int min_AngPao = 2;

//    [Header("Object Amount and List")]
//    public CurrentObjectAmount currentObjectAmount;
//    public int amountToSpawn = 100;
//    int poolTotal = 0;
//    public List<GameObject> ObjectPoolList = new List<GameObject>();
//    public List<GameObject> CurrentObjectList = new List<GameObject>();

//    List<GameObject> tempList = new List<GameObject>();
//    List<GameObject> huat_kueh_List = new List<GameObject>();
//    List<GameObject> ang_pao_list = new List<GameObject>();

//    [Header("Pool Items")]
//    public List<ObjectPoolItem> ItemsToPool = new List<ObjectPoolItem>();

//    GameObject parentContainer = null;
//    int currentHighestSortingOrder = 0;

//    [Header("Component References")]
//    public Transform environmentContainer;
//    public ScreenBoundary screenBoundary;

//    void Awake()
//    {
//    }

//    void Start()
//    {
//        parentContainer = new GameObject("Object_Container");
//        parentContainer.transform.SetParent(environmentContainer);
//    }

//    public void Init()
//    {
//        IsReady = false;
//        CreatePool();
//        GameEvents.SpawnObjectEvent += SpawnObjectEvent;
//        IsReady = true;
//    }

//    void SpawnObjectEvent()
//    {
//        int index = Random.Range(0, (int)OBJECT_TYPE.Pineapple_Tart + 1);
//        Spawn(1, (OBJECT_TYPE)index);
//    }

//    void CreatePool()
//    {
//        foreach (ObjectPoolItem item in ItemsToPool)
//        {
//            for (int i = 0; i < item.amount; i++)
//            {
//                if (item.poolName == "Huat_Kueh_Container")
//                {
//                    huat_kueh_List.Add(item.obj);
//                }

//                else if (item.poolName == "Ang_Pao_Container")
//                {
//                    ang_pao_list.Add(item.obj);
//                }

//                CreatePooledObject(item, i);
//            }
//            poolTotal += item.amount;
//        }

//        for (int i = 0; i < ObjectPoolList.Count; i++)
//        {
//            GameObject obj = ObjectPoolList[i];
//            obj.SetActive(false);
//        }
//    }

//    Vector3 GetRandomPosition()
//    {
//        if (Random.value > 0.4f)
//        {
//            return RandomPointInBox(screenBoundary.spawnAreaBoundary[0].bounds.center, screenBoundary.spawnAreaBoundary[0].bounds.size);
//        }
//        else
//        {
//            return RandomPointInBox(screenBoundary.spawnAreaBoundary[1].bounds.center, screenBoundary.spawnAreaBoundary[1].bounds.size);
//        }
//    }

//    GameObject GetSubParentPoolContainer(string name)
//    {
//        GameObject subParentObj = null;
//        // use root object pool name if no name specifed
//        if (string.IsNullOrEmpty(name))
//        {
//            name = parentContainer.name;
//            subParentObj = GameObject.Find(name);
//            return subParentObj;
//        }
//        else
//        {
//            subParentObj = GameObject.Find(name);
//            if (subParentObj == null)
//            {
//                subParentObj = new GameObject(name);
//                // Must not be the same name as root object
//                if (name != parentContainer.name)
//                {
//                    subParentObj.transform.SetParent(parentContainer.transform, false);
//                }
//            }
//        }
//        return subParentObj;
//    }

//    // Create new pooled object with reference from sorting order of 0
//    // Use this to create initial pooled object
//    GameObject CreatePooledObject(ObjectPoolItem item, int i)
//    {
//        GameObject obj = Instantiate(item.obj, Vector3.zero, Quaternion.identity) as GameObject;
//        obj.transform.SetParent(GetSubParentPoolContainer(item.poolName).transform, false);
//        obj.GetComponent<SpriteRenderer>().sortingOrder += i;
//        obj.transform.localScale = Vector3.zero;
//        obj.SetActive(true);

//        ObjectPoolList.Add(obj);
//        return obj;
//    }

//    // Create new pooled object with reference from current highest sorting order
//    GameObject CreatePooledObject(ObjectPoolItem item)
//    {
//        GameObject obj = Instantiate(item.obj, Vector3.zero, Quaternion.identity) as GameObject;
//        obj.transform.SetParent(GetSubParentPoolContainer(item.poolName).transform, false); ;
//        // +1 to increment the order
//        obj.GetComponent<SpriteRenderer>().sortingOrder += GetHighestSortingOrder(ObjectPoolList) + 1;
//        obj.transform.localScale = Vector3.zero;
//        obj.SetActive(true);

//        ObjectPoolList.Add(obj);
//        return obj;
//    }

//    GameObject GetFromPool(OBJECT_TYPE type)
//    {
//        GameObject obj = null;
//        if (ObjectPoolList.Count > 0)
//        {
//            for (int i = 0; i < ObjectPoolList.Count; i++)
//            {
//                ObjectBase baseObj = ObjectPoolList[i].GetComponent<ObjectBase>();

//                if (!baseObj.gameObject.activeSelf && baseObj.type == type)
//                {
//                    Debug.Log(baseObj.type);
//                    return obj = baseObj.gameObject;
//                }
//                else
//                {
//                    Debug.Log("No such object in pool!");
//                    return null;
//                }
//            }
//        }
//        return null;
//    }

//    void ShuffleSortingOrder(List<GameObject> objectList)
//    {
//        if (objectList.Count > 0)
//        {
//            // Shuffle the entire list first
//            objectList.Shuffle();
//            for (int i = 0; i < objectList.Count; i++)
//            {
//                SpriteRenderer spriteR = objectList[i].GetComponent<SpriteRenderer>();
//                // Reorder the sorting order
//                spriteR.sortingOrder = i;
//            }
//        }
//        else
//        {
//            Debug.LogWarning("No objects to shuffle!");
//        }
//    }

//    int GetHighestSortingOrder(List<GameObject> objectList)
//    {
//        int highestOrder = int.MinValue;
//        if (objectList.Count > 0)
//        {
//            foreach (GameObject item in objectList)
//            {
//                int temp = item.GetComponent<SpriteRenderer>().sortingOrder;
//                if (temp > highestOrder)
//                {
//                    highestOrder = temp;
//                }
//            }
//        }
//        else
//        {
//            Debug.LogWarning("No object to get sorting order from");
//        }

//        return highestOrder;
//    }

//    void RandomObjectPositions(List<GameObject> objectList)
//    {
//        if (objectList.Count > 0)
//        {
//            for (int i = 0; i < objectList.Count; i++)
//            {
//                Transform t = objectList[i].transform;
//                t.position = GetRandomPosition();
//            }
//        }
//    }

//    void Update()
//    {
//        if (!IsReady) return;

//#if !UNITY_EDTIOR
//        if (Input.GetKeyDown(KeyCode.B))
//        {
//            ReturnToPool();
//        }

//        if (Input.GetKeyDown(KeyCode.C))
//        {
//            ShuffleSortingOrder(ObjectPoolList);
//        }

//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            CreatePooledObject(ItemsToPool[0]);
//        }

//        if (Input.GetKeyDown(KeyCode.E))
//        {
//            RandomObjectPositions(CurrentObjectList);
//        }
//#endif
//    }

//    public void PopulateObjectSettings(GameSettings settings)
//    {
//        if (ObjectPoolList.Count > 0)
//        {
//            for (int i = 0; i < ObjectPoolList.Count; i++)
//            {
//                ObjectBase obj = ObjectPoolList[i].GetComponent<ObjectBase>();
//                obj.PopulateSettings(settings);
//            }
//        }
//    }

//    OBJECT_TYPE GetRandomObjectType()
//    {
//        int type = Random.Range((int)OBJECT_TYPE.Huat_Kueh, /*(int)OBJECT_TYPE.KUEH_LAPIS + 1*/(int)OBJECT_TYPE.Ang_Pao + 1);
//        return (OBJECT_TYPE)type;
//    }

//    // This method spawns random objects but are stil inactive
//    public void SpawnAll(int amt)
//    {
//        for (int i = 0; i < huat_kueh_List.Count; i++)
//        {
//            GameObject obj = huat_kueh_List[i];
//            CurrentObjectList.Add(obj);
//            ObjectPoolList.Remove(obj);
//        }

//        for (int i = 0; i < ang_pao_list.Count; i++)
//        {
//            GameObject obj = ang_pao_list[i];
//            CurrentObjectList.Add(obj);
//            ObjectPoolList.Remove(obj);
//        }

//        // spawn all huat kueh and ang pao. then spawn the rest
//        for (int i = 0; i < amt; i++)
//        {
//            int index = Random.Range(0, ObjectPoolList.Count - 1);
//            GameObject obj = ObjectPoolList[index];
//            CurrentObjectList.Add(obj);

//            ObjectPoolList.Remove(obj);
//        }

//        GetNumberOfType();
//    }

//    public void Spawn(int amt, OBJECT_TYPE type)
//    {
//        Debug.Log(type.ToString());
//        int sortOrderIndex = 0;
//        if (type == OBJECT_TYPE.Huat_Kueh || type == OBJECT_TYPE.Ang_Pao) sortOrderIndex = -1;
//        else sortOrderIndex = 1;

//        if (amt == 0) return;
//        for (int i = 0; i < amt; i++)
//        {
//            GameObject obj = CreatePooledObject(ItemsToPool[(int)type], sortOrderIndex);
//            Debug.Log(obj.GetComponent<ObjectBase>().type.ToString());
//            obj.transform.localScale = Vector3.zero;
//            obj.transform.position = GetRandomPosition();
//            obj.SetActive(true);
//            obj.transform.DOScale(Vector3.one * 0.8f, 0.75f).SetEase(Ease.OutElastic).OnComplete(() => { obj.GetComponent<ObjectBase>().Draggable = true; });

//            tempList.Add(obj);
//        }

//        GetNumberOfType();
//    }

//    public void ReturnToPool()
//    {
//        if (CurrentObjectList.Count > 0)
//        {
//            for (int i = 0; i < CurrentObjectList.Count; i++)
//            {
//                GameObject obj = CurrentObjectList[i];
//                obj.SetActive(false);
//                ObjectPoolList.Add(obj);
//            }
//            CurrentObjectList.RemoveAll(x => x.gameObject);
//        }
//        else
//        {
//            Debug.Log("No object to return to pool!");
//            Debug.Log("Creating another pool...");
//            // This shouldn't ever happen...
//            CreatePool();
//        }

//        if (tempList.Count > 0)
//        {
//            for (int i = 0; i < tempList.Count; i++)
//            {
//                GameObject obj = tempList[i];
//                Destroy(obj);
//            }
//        }
//        tempList.Clear();
//    }

//    public async void EnableAllCurrentList(bool playAnim = false)
//    {
//        await new WaitForSeconds(.1f);

//        if (CurrentObjectList.Count > 0)
//        {
//            for (int i = 0; i < CurrentObjectList.Count; i++)
//            {
//                GameObject obj = CurrentObjectList[i];
//                //Transform t = CurrentObjectList[i].GetComponent<Transform>();
//                obj.transform.localScale = Vector3.zero;
//                obj.transform.localPosition = GetRandomPosition();
//                obj.SetActive(true);
//                //if (playAnim)
//                //{
//                //    obj.transform.DOScale(Vector3.one * 0.8f, 0.35f).SetEase(Ease.OutElastic).OnComplete(() => { obj.GetComponent<ObjectBase>().Draggable = true;  });
//                //    //await new WaitForSeconds(0.00001f);
//                //}
//            }
//            for (int i = 0; i < CurrentObjectList.Count; i++)
//            {
//                GameObject obj = CurrentObjectList[i];
//                if (i % 100 == 0)
//                {
//                    await new WaitForSeconds(1.2f);
//                }
//                obj.transform.DOScale(Vector3.one * 0.8f, 0.75f).SetEase(Ease.OutElastic).OnComplete(() => { obj.GetComponent<ObjectBase>().Draggable = true; });
//            }
//        }
//        else
//        {
//            Debug.Log("No objects to be enabled!");
//        }

//        await new WaitForSeconds(1f);

//        GameEvents.OnCountdownStartEvent?.Invoke();
//    }

//    public void DisableAllCurrentList()
//    {
//        if (CurrentObjectList.Count > 0)
//        {
//            for (int i = 0; i < CurrentObjectList.Count; i++)
//            {
//                GameObject obj = CurrentObjectList[i];
//                obj.GetComponent<ObjectBase>().Draggable = false;
//                obj.SetActive(false);
//            }
//        }
//        else
//        {
//            Debug.Log("No objects to be disable!");
//        }
//    }

//    Vector3 RandomPointInBox(Vector3 center, Vector3 size)
//    {
//        return center + new Vector3(
//           (Random.value - 0.5f) * size.x,
//           (Random.value - 0.5f) * size.y,
//           (Random.value - 0.5f) * size.z
//        );
//    }

//    void GetNumberOfType()
//    {
//        currentObjectAmount.Clear();

//        if (CurrentObjectList.Count > 0)
//        {
//            for (int i = 0; i < CurrentObjectList.Count; i++)
//            {
//                ObjectBase obj = CurrentObjectList[i].GetComponent<ObjectBase>();
//                if (obj.type == OBJECT_TYPE.Huat_Kueh)
//                {
//                    currentObjectAmount.huat_kueh++;
//                }
//                else if (obj.type == OBJECT_TYPE.Ang_Pao)
//                {
//                    currentObjectAmount.ang_pao++;
//                }
//            }
//        }
//        if (tempList.Count > 0)
//        {
//            for (int i = 0; i < tempList.Count; i++)
//            {
//                ObjectBase obj = tempList[i].GetComponent<ObjectBase>();
//                if (obj.type == OBJECT_TYPE.Huat_Kueh)
//                {
//                    currentObjectAmount.huat_kueh++;
//                }
//                else if (obj.type == OBJECT_TYPE.Ang_Pao)
//                {
//                    currentObjectAmount.ang_pao++;
//                }
//            }
//        }
//    }

//    public void UpdateObjectAmount(OBJECT_TYPE type)
//    {
//        switch (type)
//        {
//            case OBJECT_TYPE.Huat_Kueh:
//                currentObjectAmount.huat_kueh--;
//                if (currentObjectAmount.huat_kueh < min_HuatKueh)
//                {
//                    Spawn(Random.Range(5, 10), OBJECT_TYPE.Huat_Kueh);
//                }
//                break;
//            case OBJECT_TYPE.Ang_Pao:
//                currentObjectAmount.ang_pao--;
//                if (currentObjectAmount.huat_kueh < min_AngPao)
//                {
//                    Spawn(Random.Range(5, 10), OBJECT_TYPE.Ang_Pao);
//                }
//                break;
//            case OBJECT_TYPE.Mandarin_Orange:
//                break;
//            case OBJECT_TYPE.Pineapple_Tart:
//                break;
//            case OBJECT_TYPE.Chun_Lian:
//                break;
//            case OBJECT_TYPE.Firecracker:
//                break;
//        }
//    }

//}
