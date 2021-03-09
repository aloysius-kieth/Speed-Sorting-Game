using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    public OBJECT_TYPE type = 0;
    public int point { get; set; }
    public bool Draggable { get; set; }

    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb2d;

    public bool inUse = false;

    public virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        gameObject.name = type.ToString();
        PopulateSettings(TrinaxGlobal.Instance.gameSettings);
        Draggable = false;
    }

    public virtual void Start()
    {
    }

    public virtual void OnEnable()
    {
        transform.Rotate(new Vector3(0, 0, Random.Range(-360, 360)));
    }

    void OnDisable()
    {
        Draggable = false;
    }

    public void PopulateSettings(GameSettings settings)
    {
        switch (type)
        {
            case OBJECT_TYPE.Huat_Kueh:
                point = settings.huatkueh_points;
                break;
            case OBJECT_TYPE.Ang_Pao:
                point = settings.angpao_points;
                break;
            case OBJECT_TYPE.Mandarin_Orange:
                point = -settings.mandarinOrange_points;
                break;
            case OBJECT_TYPE.Pineapple_Tart:
                point = -settings.pineappleTart_points;
                break;
            case OBJECT_TYPE.Chun_Lian:
                point = -settings.chunLian_points;
                break;
            case OBJECT_TYPE.Firecracker:
                point = -settings.firecracker_points;
                break;
        }
    }

    Vector2 force;
    public void ApplyForce(Vector2 force)
    {
        this.force = force;
    }

    private void FixedUpdate()
    {
        if (force != Vector2.zero)
            force = force * 0.9f;

        rb2d.velocity = force;
    }

}
