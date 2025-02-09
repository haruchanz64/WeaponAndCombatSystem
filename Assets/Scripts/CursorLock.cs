using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        // Check if the Left Alt key is being held down
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            UnlockCursor();
        }
        else
        {
            LockCursor(); // Lock the cursor again when Left Alt is not held
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }
}