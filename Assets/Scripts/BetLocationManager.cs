/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür
 * 
 * Manages raycasting for bet locations and handles hover/click detection.
 * MAde with new Input system
 * 
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class BetLocationManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask betLocationLayer;

    private BetLocation currentHoveredLocation;

    void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // Get mouse position using new Input System
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, betLocationLayer))
        {
            BetLocation hitLocation = hit.collider.GetComponent<BetLocation>();

            if (hitLocation != null)
            {
                // Mouse is over a bet location
                if (currentHoveredLocation != hitLocation)
                {
                    // Entered new location
                    if (currentHoveredLocation != null)
                        currentHoveredLocation.OnHoverExit();

                    currentHoveredLocation = hitLocation;
                    currentHoveredLocation.OnHoverEnter();
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    hitLocation.OnClick();
                }
            }
        }
        else
        {
            if (currentHoveredLocation != null)
            {
                currentHoveredLocation.OnHoverExit();
                currentHoveredLocation = null;
            }
        }
    }
}