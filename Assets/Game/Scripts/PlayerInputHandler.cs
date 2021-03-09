using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public bool IsReady { get; set; }

    Camera camera;
    GameObject currentSelected = null;

    Vector3 offset;

    float touchTime = 0;
    const float MIN_DELAY_TOUCH = 0.1f;

    bool startDrag = false;
    public bool useMouseControl = true;

    [Header("Component References")]
    public ScreenBoundary screenBoundary;

    void Start()
    {
        IsReady = false;
        Input.multiTouchEnabled = false;
    }

    public void Init()
    {
#if UNITY_EDITOR
        useMouseControl = true;
#else
        useMouseControl = false;
#endif
        camera = Camera.main;
        screenBoundary.Init();
        IsReady = true;
        Debug.Log("<color=green> Player input ready! </color>");
    }

    void Update()
    {
        if (!IsReady || AppManager.Instance.gameManager.IsGameover || TrinaxGlobal.Instance.state != STATE.GAME) return;

        HandleOnClick();
        if (currentSelected != null)
        {
            Vector2 currentSize = currentSelected.GetComponent<SpriteRenderer>().size;

            Vector3 pos = currentSelected.transform.position;
            pos.x = Mathf.Clamp(currentSelected.transform.position.x, -screenBoundary.HorizontalExtent + currentSize.x, screenBoundary.HorizontalExtent - currentSize.x);
            pos.y = Mathf.Clamp(currentSelected.transform.position.y, -screenBoundary.VerticalExtent + currentSize.y, screenBoundary.VerticalExtent - currentSize.y);
            currentSelected.transform.position = pos;
        }
    }

    Vector2 currentTouchPos;
    Vector2 prevTouchPos;
    Vector2 force;
    void HandleOnClick()
    {
        #region MOUSE CLICK
        if (useMouseControl)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentSelected = CheckForObjectUnderMouse();
                if (currentSelected == null || !currentSelected.GetComponent<ObjectBase>().Draggable)
                    return;
                //Debug.Log("nothing selected by mouse");
                else
                {
                    startDrag = true;
                    offset = currentSelected.transform.position - camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                    //Debug.Log(currentSelected.gameObject);
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                startDrag = false;
                currentSelected = null;
            }
            if (currentSelected != null && startDrag)
            {
                Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 objPosition = camera.ScreenToWorldPoint(mousePos) + offset;
                currentSelected.transform.position = objPosition;
            }
        }
        #endregion 
        // touch input
        else
        {
            //Debug.Log(Input.touchCount);
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    prevTouchPos = touch.position;

                    float lastTouchTime = Time.time - touchTime;
                    if (lastTouchTime <= MIN_DELAY_TOUCH)
                    {
                        //Debug.Log("tapping too fast");
                        return;
                    }
                    //Debug.Log(lastTouchTime);
                    currentSelected = CheckForObjectUnderMouse();
                    if (currentSelected == null || !currentSelected.GetComponent<ObjectBase>().Draggable)
                        return;
                    //Debug.Log("nothing selected by mouse");
                    else
                    {
                        startDrag = true;
                        offset = currentSelected.transform.position - camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                        //Debug.Log(currentSelected.gameObject);
                    }
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    touchTime = 0;
                    if (currentSelected != null && startDrag)
                    {
                        currentTouchPos = touch.position;
                        force = (currentTouchPos - prevTouchPos);

                        prevTouchPos = currentTouchPos;

                        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                        Vector2 objPosition = camera.ScreenToWorldPoint(mousePos) + offset;
                        currentSelected.transform.position = objPosition;
                    }
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    touchTime = Time.time;
                    //Debug.Log("Touch time: " + touchTime);
                    startDrag = false;
                    if(currentSelected != null)
                    {
                       // Debug.Log(currentSelected.GetComponent<Rigidbody2D>().velocity.magnitude);
                        if (currentSelected.GetComponent<Rigidbody2D>().velocity.magnitude > 3f)
                        {
                            //Debug.Log("above threshold");
                            force = currentSelected.GetComponent<Rigidbody2D>().velocity.normalized * 3f;
                            currentSelected.GetComponent<ObjectBase>().ApplyForce(force);
                        }
                        else
                        {
                            //Debug.Log("just nice");
                            currentSelected.GetComponent<ObjectBase>().ApplyForce(force * 1.5f);
                        }
                    }
                    currentTouchPos = Vector2.zero;
                    prevTouchPos = Vector2.zero;
                    force = Vector2.zero;
                    currentSelected = null;
                }
            }
        }
    }

    GameObject CheckForObjectUnderMouse()
    {
        Vector2 touchPostion = camera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] allCollidersAtTouchPosition = Physics2D.RaycastAll(touchPostion, Vector2.zero);

        SpriteRenderer closest = null; //Cache closest sprite renderer so we can assess sorting order
        foreach (RaycastHit2D hit in allCollidersAtTouchPosition)
        {
            if (closest == null) // if there is no closest assigned, this must be the closest
            {
                closest = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                continue;
            }

            SpriteRenderer hitSprite = hit.collider.gameObject.GetComponent<SpriteRenderer>();
            if (hitSprite == null)
                continue; //If the object has no sprite go on to the next hitobject

            if (hitSprite.sortingOrder > closest.sortingOrder)
                closest = hitSprite;
        }

        return closest != null && closest.tag != "Pocket" ? closest.gameObject : null;
    }
}
