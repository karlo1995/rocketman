using System.Collections;
using DG.Tweening;
using Script.Misc;
using UnityEngine;
using UnityEngine.UI;

public class FuelController : Singleton<FuelController>
{
    private const float THRUST_DEDUCTION = 0.05f;
    private const float BRAKE_DEDUCTION = 0.05f;
    private const float DRAG_DEDUCTION = 0.02f;

    [SerializeField] private Sprite[] fuelSprites;
    [SerializeField] private Image realFuelProgressBar;
    [SerializeField] private Image predictedFuelProgressBar;
    [SerializeField] private Image adjustingFuelProgressBar;

    private float fuelAmount = 1f;
    public float FuelAmount => fuelAmount;
    
    public float predictionFuelAmount = 1f;

    private bool thrustingAlreadyStarted;

    private void Awake()
    {
        realFuelProgressBar.transform.DOScaleX(1f, 0f);
        realFuelProgressBar.transform.DOScaleX(1f, 0f);
        adjustingFuelProgressBar.transform.DOScaleX(1f, 0f);

        adjustingFuelProgressBar.DOFade(0f, 0f);
        predictedFuelProgressBar.DOFade(0f, 0f);

        UpdateFuelSprite();
    }

    public bool IsFuelEnoughToThrust()
    {
        return fuelAmount > THRUST_DEDUCTION;
    }
    
    public bool IsFuelEnoughToBrake()
    {
        return fuelAmount > BRAKE_DEDUCTION;
    }
    
    public bool IsFuelEnoughToDrag(float dragDistance)
    {
        var predictedFuel = predictionFuelAmount - dragDistance * DRAG_DEDUCTION;
        return predictionFuelAmount > 0f;
    }
    public void UseThrustFuel()
    {
        if (IsFuelEnoughToThrust())
        {
            if (!thrustingAlreadyStarted)
            {
                thrustingAlreadyStarted = true;
                StartCoroutine(StartThrusterUseFuel());
            }
        }
    }
    
    public void UseBrakeFuel()
    {
        if (IsFuelEnoughToBrake())
        {
            fuelAmount -= BRAKE_DEDUCTION;
            predictionFuelAmount = fuelAmount;

            realFuelProgressBar.transform.DOScaleX(fuelAmount, 1f);
            predictedFuelProgressBar.transform.DOScaleX(fuelAmount, 0f);

            UpdateFuelSprite();
        }
    }
    
    private IEnumerator StartThrusterUseFuel()
    {
        while (PlayerThrustController.Instance.IsThrusting)
        {
            fuelAmount -= THRUST_DEDUCTION;
            predictionFuelAmount = fuelAmount;

            realFuelProgressBar.transform.DOScaleX(fuelAmount, 1f);
            predictedFuelProgressBar.transform.DOScaleX(fuelAmount, 0f);
            
            UpdateFuelSprite();
            yield return new WaitForSeconds(1f);
        }

        while (PlayerThrustController.Instance.IsThrusting)
        {
            yield return null;
        }

        thrustingAlreadyStarted = false;
    }

    public void UsePredictionFuel(float dragDistance)
    {
        predictedFuelProgressBar.transform.DOScaleX(fuelAmount, 0f);
        
        adjustingFuelProgressBar.DOFade(1f, 0f);
        realFuelProgressBar.DOFade(0f, 0f);
        
        predictionFuelAmount = fuelAmount - dragDistance * DRAG_DEDUCTION;
        predictedFuelProgressBar.DOFade(1f, 0f);
        
        adjustingFuelProgressBar.DOFillAmount(predictionFuelAmount, 0.3f);
        UpdateFuelSprite();
    }
    
    public void ResetPredictionFuel()
    {
        predictedFuelProgressBar.DOFade(0f, 0f);
        adjustingFuelProgressBar.DOFade(0f, 0f);
        realFuelProgressBar.DOFade(1f, 0f);
        
        UpdateFuelSprite();
    }
    
    public void ApplyPredictionFuel()
    {
        ResetPredictionFuel();
        
        fuelAmount = predictionFuelAmount;
        realFuelProgressBar.transform.DOScaleX(fuelAmount, 0.3f);
        
        UpdateFuelSprite();
    }

    private void UpdateFuelSprite()
    {
        switch (fuelAmount)
        {
            // more than 50%
            case > 0.5f:
                realFuelProgressBar.sprite = fuelSprites[0];
                adjustingFuelProgressBar.sprite = fuelSprites[0];
                predictedFuelProgressBar.sprite = fuelSprites[0];
                break;
            
            // more than 10%
            case > 0.1f:
                realFuelProgressBar.sprite = fuelSprites[1];
                adjustingFuelProgressBar.sprite = fuelSprites[1];
                predictedFuelProgressBar.sprite = fuelSprites[1];
                break;
            
            // less than 10%
            default:
                realFuelProgressBar.sprite = fuelSprites[2];
                adjustingFuelProgressBar.sprite = fuelSprites[2];
                predictedFuelProgressBar.sprite = fuelSprites[2];
                break;
        }
    }
}