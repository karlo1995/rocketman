using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragVisualController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer arrowToolSpriteRenderer;
    [SerializeField] private Image radarLeft;
    [SerializeField] private Image radarRight;
    [SerializeField] private CanvasGroup leftLabelGroup;
    [SerializeField] private CanvasGroup rightLabelGroup;
    [SerializeField] private TextMeshProUGUI angleTextRight;
    [SerializeField] private TextMeshProUGUI powerTextRight;
    [SerializeField] private TextMeshProUGUI angleTextLeft;
    [SerializeField] private TextMeshProUGUI powerTextLeft;
    private void Awake()
    {
        radarRight.DOFade(0f, 0f);
        radarLeft.DOFade(0f, 0f);
        arrowToolSpriteRenderer.DOFade(0f, 0f);
        leftLabelGroup.DOFade(0f, 0f);
        rightLabelGroup.DOFade(0f, 0f);
    }
    
    public void SetRadarDirectionAndArrow(Vector3 finalForce, Vector3 trajectoryDistance)
    {
        //set flip x 
        PlayerAnimationController.Instance.FlipX(finalForce);
        
        //radar direction
        if (finalForce.x < 0)
        {
            radarRight.DOFade(1f, 0.3f);
            radarLeft.DOFade(0f, 0.3f);

            leftLabelGroup.DOFade(0f, 0.3f);
            rightLabelGroup.DOFade(1f, 0.3f);
        }
        else
        {
            radarLeft.DOFade(1f, 0.3f);
            radarRight.DOFade(0f, 0.3f);
            
            leftLabelGroup.DOFade(1f, 0.3f);
            rightLabelGroup.DOFade(0f, 0.3f);
        }
        
        //arrow scale
        if (trajectoryDistance.x > 3f)
        {
            trajectoryDistance.x = 3f;
        }

        if (trajectoryDistance.x < -3f)
        {
            trajectoryDistance.x = -3f;
        }

        var angle = Mathf.Atan2(trajectoryDistance.y, trajectoryDistance.x) * Mathf.Rad2Deg;
        
        //set angle text
        var adjustedAngle = Mathf.Abs(angle);
        if (adjustedAngle >= 90f)
        {
            adjustedAngle -= 180f;
        }
        
        var angleValue = Mathf.Abs((int)(adjustedAngle)) + "°";
        angleTextRight.text = angleValue;
        angleTextLeft.text = angleValue;

        //set drag arrow rotation
        angle += 500f;
        arrowToolSpriteRenderer.transform.DORotate(new Vector3(0f, 0f, angle), 0.2f);
        
        //set power value
        var power = (int)(finalForce.magnitude * 10);
        powerTextRight.text = power.ToString();
        powerTextLeft.text = power.ToString();

        //set drag arrow scale
        var scale = finalForce.magnitude * 0.1f;
        if (scale < 0.2f)
        {
            scale = 0.2f;
        }

        if (scale > 0.7f)
        {
            scale = 0.7f;
        }
        
        arrowToolSpriteRenderer.transform.DOScale(new Vector2(scale, scale), 0.3f);
    }

    public void SetActiveRadar(bool active)
    {
        if (active)
        {
            arrowToolSpriteRenderer.DOFade(1f, 0.3f);
        }
        else
        {
            arrowToolSpriteRenderer.DOFade(0f, 0.3f);
            radarLeft.DOFade(0f, 0.3f);
            radarRight.DOFade(0f, 0.3f);
            
            leftLabelGroup.DOFade(0f, 0.3f);
            rightLabelGroup.DOFade(0f, 0.3f);
        }
    }
}
