using DG.Tweening;
using UnityEngine;

public class DragVisualController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer arrowToolSpriteRenderer;
    [SerializeField] private Sprite[] powerArrowSprites;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        arrowToolSpriteRenderer.DOFade(0f, 0f);
        canvasGroup.DOFade(0f, 0f);
    }

    public void SetRadarDirectionAndArrow(Vector3 finalForce, Vector3 trajectoryDistance, float angle)
    {
        //set flip x 
        PlayerAnimationController.Instance.FlipX(finalForce);

        //arrow scale
        if (trajectoryDistance.x > 3f)
        {
            trajectoryDistance.x = 3f;
        }

        if (trajectoryDistance.x < -3f)
        {
            trajectoryDistance.x = -3f;
        }
        
        //set angle text
        var adjustedAngle = Mathf.Abs(angle);

        if (adjustedAngle >= 90f)
        {
            adjustedAngle -= 180f;
        }
        
        var angleValue = Mathf.Abs((int)(adjustedAngle)) + "°";
        // angleTextRight.text = angleValue;
        //angleTextLeft.text = angleValue;

        //set drag arrow rotation
        angle += 500f;
        arrowToolSpriteRenderer.transform.DORotate(new Vector3(0f, 0f, angle), 0.2f);

        //set power value
        var power = (int)(finalForce.magnitude * 10);
        //  powerTextRight.text = power.ToString();
        //   powerTextLeft.text = power.ToString();
        

        var modifiedFinalForceMagnitude = finalForce.magnitude * 1f;
        var index = Mathf.Floor(modifiedFinalForceMagnitude);
        
        if (index >= powerArrowSprites.Length)
        {
            index = powerArrowSprites.Length - 1;
        }

        arrowToolSpriteRenderer.sprite = powerArrowSprites[(int)index];
    }

    public void SetActiveRadar(bool active)
    {
        if (active)
        {
            canvasGroup.DOFade(1f, 0.3f);
            arrowToolSpriteRenderer.DOFade(1f, 0.3f);
        }
        else
        {
            canvasGroup.DOFade(0f, 0.3f);
            arrowToolSpriteRenderer.DOFade(0f, 0.3f);
        }
    }
}