using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DragVisualController : MonoBehaviour
{
    [SerializeField] private Image arrowBodySpriteRenderer;
    [SerializeField] private Image arrowHeadSpriteRenderer;
    [SerializeField] private Transform arrowHeadGuide;

    [SerializeField] private Color[] powerArrowColors;
    [SerializeField] private CanvasGroup canvasGroup;

    private float originalFillAmountOfArrowBody; 
    private float originalScaleValueOfArrowGuide;
    private const float maxPower = 17.5f;
    
    private void Awake()
    {
        arrowBodySpriteRenderer.DOFade(0f, 0f);
        arrowHeadSpriteRenderer.DOFade(0f, 0f);
        canvasGroup.DOFade(0f, 0f);

        originalScaleValueOfArrowGuide = arrowHeadGuide.transform.localPosition.y;
        originalFillAmountOfArrowBody = arrowBodySpriteRenderer.fillAmount;
    }

    public void SetRadarDirectionAndArrow(Vector3 finalForce, Vector3 trajectoryDistance, float angle)
    {
        //set flip x 
        PlayerAnimationController.Instance.ByRotationPlayerFlipX(finalForce);

        //arrow scale
        // if (trajectoryDistance.x > 3f)
        // {
        //     trajectoryDistance.x = 3f;
        // }
        //
        // if (trajectoryDistance.x < -3f)
        // {
        //     trajectoryDistance.x = -3f;
        // }

        //set angle value
        // var adjustedAngle = Mathf.Abs(angle);
        //
        // if (adjustedAngle >= 90f)
        // {
        //     adjustedAngle -= 180f;
        // }
        //var angleValue = Mathf.Abs((int)(adjustedAngle)) + "°";
        // angleTextRight.text = angleValue;
        //angleTextLeft.text = angleValue;

        //set drag arrow rotation

        angle += 90f;
        arrowBodySpriteRenderer.transform.DORotate(new Vector3(0f, 0f, angle), 0.2f);
        arrowHeadSpriteRenderer.transform.position = arrowHeadGuide.transform.position;
        arrowHeadSpriteRenderer.transform.rotation = arrowBodySpriteRenderer.transform.rotation;

        //set power value
        //var power = (int)(finalForce.magnitude * 10);
        //  powerTextRight.text = power.ToString();
        //   powerTextLeft.text = power.ToString();

        var modifiedFinalForceMagnitude = finalForce.magnitude * 1f;
        var index = Mathf.Floor(modifiedFinalForceMagnitude);

        if (index >= powerArrowColors.Length)
        {
            index = powerArrowColors.Length - 1;
        }

        var color = powerArrowColors[(int)index];
        arrowBodySpriteRenderer.DOColor(color, 0.1f);
        arrowHeadSpriteRenderer.DOColor(color, 0.1f);


        //set drag arrow scale
        var powerDiff = Mathf.Abs(maxPower - finalForce.magnitude);
        var powerPercentageDiff = maxPower / powerDiff;
        var fillAmountPercentageDiff = originalFillAmountOfArrowBody / powerPercentageDiff;
        var fillAmountToApply = originalFillAmountOfArrowBody - fillAmountPercentageDiff;
        var arrowGuideDiff = originalFillAmountOfArrowBody / fillAmountToApply;
        var arrowGuidePercentageDiff = originalScaleValueOfArrowGuide / arrowGuideDiff;
        
        arrowHeadGuide.transform.DOLocalMoveY((originalScaleValueOfArrowGuide + arrowGuidePercentageDiff) - 66f, 0f);
        arrowBodySpriteRenderer.DOFillAmount(fillAmountToApply, 0f);
    }

    public void SetActiveRadar(bool active)
    {
        if (active)
        {
            canvasGroup.DOFade(1f, 0.3f);
            arrowBodySpriteRenderer.DOFade(1f, 0.3f);
            arrowHeadSpriteRenderer.DOFade(1f, 0.3f);
        }
        else
        {
            canvasGroup.DOFade(0f, 0.3f);
            arrowBodySpriteRenderer.DOFade(0f, 0.3f);
            arrowHeadSpriteRenderer.DOFade(0f, 0.3f);
        }
    }
}