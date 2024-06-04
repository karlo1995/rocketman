using DG.Tweening;
using Runtime.Levels;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private CollectibleData data;
    public CollectibleData Data => data;

    private void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().DOFade(0f, 0f);
        gameObject.SetActive(false);
    }

    public void Init(CollectibleData data)
    {
        this.data = data;
        SetActive(true);
    }

    public void SetActive(bool active, bool clearData = false)
    {
        if (clearData)
        {
            data = null;
        }
        
        if (!active)
        {
            gameObject.GetComponent<SpriteRenderer>().DOFade(0f, 0.3f).OnComplete(() => gameObject.SetActive(active));
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().DOFade(1f, 0.3f).OnComplete(() => gameObject.SetActive(active));
        }
    }
}