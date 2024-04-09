using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score = 0;

    [Header("Combo")]
    public int Combo = 0;
    public float ScoreComboMulti = 1f;

    public bool IsReady = false;

    [Header("Score Event Values")]
    [SerializeField] private int _precisionBonusValue;
    [SerializeField] private int _fiveBonusValue;
    private bool _isMultiOfFive;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Invoke(nameof(ReadyUp), 1.5f);
    }

    public void AddScore(int score)
    {
        if (!IsReady) return;

        Score += ComboMulti(score);
    }

    private int ComboMulti(int value)
    {
        value = (int)Mathf.Ceil(value * ScoreComboMulti);
        return value;
    }

    #region Combo
    public void ComboIncrease()
    {
        if (!IsReady) return;

        Combo++;
        ScoreComboMulti += 0.05f;

        _isMultiOfFive = Combo % 5 == 0;
        if (_isMultiOfFive) MultiOfFiveBonus();
    }

    public void EndCombo()
    {
        Combo = 0;
        ScoreComboMulti = 1f;

        IsReady = false;
        Invoke(nameof(ReadyUp), 1.5f);
    }

    private void ReadyUp()
    {
        IsReady = true;
    }
    #endregion

    #region Score Bonus Events
    public void PrecisionBonus()
    {
        if (!IsReady) return;

        Score += ComboMulti(_precisionBonusValue);
    }

    public void MultiOfFiveBonus()
    {
        if (!IsReady) return;

        Score += ComboMulti(_fiveBonusValue);
    }
    #endregion
}
