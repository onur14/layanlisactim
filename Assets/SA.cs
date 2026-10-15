using UnityEngine;
using UnityEngine.InputSystem;

public class SA : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0.01f)] private float jumpHeight = 2f;
    [SerializeField, Min(0.01f)] private float jumpDuration = 0.8f;

    private float startY;
    private float jumpTime;
    private bool isJumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("selam");
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            float horizontal = (keyboard.dKey.isPressed ? 1f : 0f)
                - (keyboard.aKey.isPressed ? 1f : 0f);
            float vertical = (keyboard.wKey.isPressed ? 1f : 0f)
                - (keyboard.sKey.isPressed ? 1f : 0f);

            // Capraz hareketin daha hizli olmamasi icin girdiyi sinirla.
            Vector3 input = Vector3.ClampMagnitude(new Vector3(horizontal, 0f, vertical), 1f);
            Quaternion heading = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            transform.position += heading * input * Mathf.Max(0f, moveSpeed) * Time.deltaTime;
        }

        if (!isJumping && keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            startY = transform.position.y;
            jumpTime = 0f;
            isJumping = true;
        }

        if (!isJumping)
            return;

        jumpTime += Time.deltaTime;
        float progress = Mathf.Clamp01(jumpTime / Mathf.Max(0.01f, jumpDuration));
        Vector3 position = transform.position;
        // Parabolic arc: return exactly to the starting height on landing.
        position.y = startY + 4f * jumpHeight * progress * (1f - progress);
        transform.position = position;

        if (progress >= 1f)
            isJumping = false;
    }
}
