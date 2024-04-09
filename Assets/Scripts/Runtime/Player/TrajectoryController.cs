using System;
using UnityEngine;

public class TrajectoryController : MonoBehaviour
{
    public static TrajectoryController Instance;
    [SerializeField] private int dotsNumber;
    [SerializeField] private GameObject dotsParent;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private float dotSpacing;
    [SerializeField] [Range(0.01f, 0.3f)] private float dotMinScale;
    [SerializeField] [Range(0.3f, 1f)] float dotMaxScale;

    private Transform[] dotsList;

    private Vector2 pos;

    //dot pos
    private float timeStamp;

    //--------------------------------

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //hide trajectory in the start
        Hide();
        //prepare dots
        PrepareDots();
    }

    private void PrepareDots()
    {
        dotsList = new Transform[dotsNumber];
        dotPrefab.transform.localScale = Vector3.one * dotMaxScale;

        var scale = dotMaxScale;
        var scaleFactor = scale / dotsNumber;

        for (var i = 0; i < dotsNumber; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, null).transform;
            dotsList[i].parent = dotsParent.transform;

            dotsList[i].localScale = Vector3.one * scale;
            if (scale > dotMinScale)
            {
                scale -= scaleFactor;
            }
        }
    }

    public void UpdateDots(Vector3 ballPos, Vector2 forceApplied)
    {
        timeStamp = dotSpacing;
        for (var i = 0; i < dotsNumber; i++)
        {
            pos.x = ballPos.x + forceApplied.x * timeStamp;
            pos.y = ballPos.y + forceApplied.y * timeStamp - Physics2D.gravity.magnitude * timeStamp * timeStamp / 2f;

            //you can simplify this 2 lines at the top by:
            //pos = (ballPos+force*time)-((-Physics2D.gravity*time*time)/2f);
            //
            //but make sure to turn "pos" in Ball.cs to Vector2 instead of Vector3	

            dotsList[i].position = pos;
            timeStamp += dotSpacing;
        }
    }

    public void Show()
    {
        dotsParent.SetActive(true);
    }

    public void Hide()
    {
        dotsParent.SetActive(false);
    }
}