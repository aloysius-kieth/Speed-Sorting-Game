using UnityEngine;
using UnityEngine.UI;

public class ChunLian : ObjectBase
{
    public Sprite[] sprites;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        int index = Random.Range(0, sprites.Length);
        spriteRenderer.sprite = sprites[index];
    }
}
