using System;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class CheatController : MonoBehaviour
    {
        [SerializeField] private PlayerThrustController playerThrustController;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private TextMeshProUGUI thrustAmount;
        [SerializeField] private TextMeshProUGUI brakeAmount;
        

        private void Awake()
        {
            thrustAmount.text = playerThrustController.fuelSpeed.ToString();
            brakeAmount.text = playerThrustController.brakeSpeed.ToString();
        }

        public void AddThrustForce()
        {
            playerThrustController.fuelSpeed += 0.1f;
            thrustAmount.text = playerThrustController.fuelSpeed.ToString();
        }

        public void ReduceThrustForce()
        {
            playerThrustController.fuelSpeed -= 0.1f;
            thrustAmount.text = playerThrustController.fuelSpeed.ToString();
        }

        public void AddBrakeForce()
        {
            playerThrustController.brakeSpeed += 1f;
            brakeAmount.text = playerThrustController.brakeSpeed.ToString();
        }

        public void ReduceBrakeForce()
        {
            playerThrustController.brakeSpeed -= 1f;
            brakeAmount.text = playerThrustController.brakeSpeed.ToString();
        }

        public void ZoomInCamera()
        {
            virtualCamera.m_Lens.OrthographicSize -= 1f;
        }

        public void ZoomOutCamera()
        {
            virtualCamera.m_Lens.OrthographicSize += 1f;
        }
    }
}
