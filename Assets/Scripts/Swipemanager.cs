using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeSceneChanger : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float minSwipeDistance = 100f;
    
    private Vector2 startTouchPosition;
    private bool isDragging = false;

    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Check for mouse input first (works in Editor)
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                startTouchPosition = mouse.position.ReadValue();
                isDragging = true;
            }
            else if (mouse.leftButton.wasReleasedThisFrame && isDragging)
            {
                Vector2 endPosition = mouse.position.ReadValue();
                ProcessSwipe(endPosition);
                isDragging = false;
            }
        }
        
        // Check for touch input (for mobile)
        var touchscreen = UnityEngine.InputSystem.Touchscreen.current;
        if (touchscreen != null)
        {
            var touch = touchscreen.primaryTouch;
            
            if (touch.press.wasPressedThisFrame)
            {
                startTouchPosition = touch.position.ReadValue();
                isDragging = true;
            }
            else if (touch.press.wasReleasedThisFrame && isDragging)
            {
                Vector2 endPosition = touch.position.ReadValue();
                ProcessSwipe(endPosition);
                isDragging = false;
            }
        }
    }
    
    void ProcessSwipe(Vector2 endPosition)
    {
        Vector2 swipeDelta = endPosition - startTouchPosition;
        
        // Only horizontal swipes
        if (Mathf.Abs(swipeDelta.x) > minSwipeDistance)
        {
            if (swipeDelta.x < 0) // Swipe LEFT
            {
                LoadNextScene();
            }
            else // Swipe RIGHT
            {
                LoadPreviousScene();
            }
        }
    }
    
    void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Loading scene {nextIndex}");
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("Already at last scene!");
        }
    }
    
    void LoadPreviousScene()
    {
        int prevIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (prevIndex >= 0)
        {
            Debug.Log($"Loading scene {prevIndex}");
            SceneManager.LoadScene(prevIndex);
        }
        else
        {
            Debug.Log("Already at first scene!");
        }
    }
}