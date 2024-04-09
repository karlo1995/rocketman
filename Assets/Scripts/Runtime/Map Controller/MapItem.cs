using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Map_Controller
{
    public class MapItem : MonoBehaviour
    {
        [SerializeField] private Transform[] _steps;
        public Transform[] Step => _steps;

        private Vector3[] steps = {};
        
        private void Awake()
        {
            List<Vector3> steps = new();
            foreach (var step in _steps)
            {
                steps.Add(step.transform.position);
            }

            this.steps = steps.ToArray();
        }

        public void MoveRocketMan(Transform rocketIcon, Action doneCallback)
        {
            StartCoroutine(MoveInStep(rocketIcon, doneCallback));
        }

        private IEnumerator MoveInStep(Transform rocketIcon, Action doneCallback)
        {
            var currentStepCount = 1;
            var stillJumping = true;
            
            rocketIcon.DOMove(_steps[currentStepCount].position, 0f);

            while (currentStepCount < _steps.Length)
            {
                var currentStep = _steps[currentStepCount];
                stillJumping = true;
                
                rocketIcon.DOJump(currentStep.position, 0.5f, 1, 0.8f).OnComplete(() =>
                {
                    currentStepCount++;
                    stillJumping = false;

                    if (currentStepCount >= _steps.Length)
                    {
                        doneCallback.Invoke();
                    }
                });
                
                while (stillJumping)
                {
                    yield return null;
                }
            }
            //
            // while (stillJumping)
            // {
            //     yield return null;
            // }
            //
            // doneCallback.Invoke();
        }
    } 
}
