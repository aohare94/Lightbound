using UnityEngine;
using System.Collections;

public class WillowAttackController : MonoBehaviour
{
    private Animator animator;
    private bool isTransforming;
    private float holdTime;
    private const float maxHoldTime = 1.0f; // Maximum time to hold the F key for a full spin
    private const int maxFrames = 18; // Total frames in the transform animation

    void Start()
    {
        animator = GetComponent<Animator>();
        isTransforming = false;
        holdTime = 0;
        StartCoroutine(VerifyParameters());
    }

    IEnumerator VerifyParameters()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second to ensure Animator setup completes
        Debug.Log($"Animator has CurrentFrame parameter: {animator.HasParameter("CurrentFrame")}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Start the transformation
            Debug.Log("StartTransform triggered");
            animator.SetTrigger("StartTransform");
            isTransforming = true;
            holdTime = 0;
            StartCoroutine(UpdateHoldTime());
        }

        if (Input.GetKey(KeyCode.F))
        {
            if (isTransforming)
            {
                // Increment the hold time
                holdTime += Time.deltaTime;
                holdTime = Mathf.Clamp(holdTime, 0, maxHoldTime); // Clamp to maxHoldTime

                // Calculate the current frame based on hold time
                int currentFrame = Mathf.FloorToInt((holdTime / maxHoldTime) * maxFrames);
                if (animator.HasParameter("CurrentFrame"))
                {
                    animator.SetFloat("CurrentFrame", currentFrame);
                }
                Debug.Log($"CurrentFrame set to {currentFrame}");
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            if (isTransforming)
            {
                int currentFrame = Mathf.FloorToInt((holdTime / maxHoldTime) * maxFrames);
                HandleFrameBasedAction(currentFrame);
                isTransforming = false;
            }
        }
    }

    private IEnumerator UpdateHoldTime()
    {
        while (Input.GetKey(KeyCode.F))
        {
            holdTime += Time.deltaTime;
            holdTime = Mathf.Clamp(holdTime, 0, maxHoldTime); // Clamp to maxHoldTime

            // Calculate the current frame based on hold time
            int currentFrame = Mathf.FloorToInt((holdTime / maxHoldTime) * maxFrames);
            if (animator.HasParameter("CurrentFrame"))
            {
                animator.SetFloat("CurrentFrame", currentFrame);
            }
            yield return null;
        }
    }

    private void HandleFrameBasedAction(int frame)
    {
        Debug.Log($"Handling frame {frame}");
        if (frame >= 1 && frame <= 7)
        {
            // Reverse transform back to idle
            ReverseTransform();
        }
        else if (frame == 8)
        {
            // 50% speed spin
            StartSpinAttack(0.5f);
        }
        else if (frame == 9)
        {
            // 75% speed spin
            StartSpinAttack(0.75f);
        }
        else if (frame == 10)
        {
            // 100% speed spin
            StartSpinAttack(1.0f);
        }
        else if (frame == 11)
        {
            // 200% speed spin and spins twice
            StartSpinAttack(2.0f, true);
        }
        else if (frame >= 12 && frame <= 17)
        {
            // Force finish the animation without spinning, then go back to idle/walk
            animator.SetTrigger("StartReverse");
            Debug.Log("StartReverse triggered for frames 12-17");
        }
    }

    private void StartSpinAttack(float speedMultiplier, bool doubleSpin = false)
    {
        if (animator.HasParameter("SpinDuration"))
        {
            animator.SetFloat("SpinDuration", 1.0f / speedMultiplier); // Adjust the duration to make the spin faster
        }
        animator.SetTrigger("StartSpin");
        Debug.Log($"StartSpin triggered with speedMultiplier {speedMultiplier}");

        if (doubleSpin)
        {
            // Schedule the double spin
            StartCoroutine(DoubleSpin(1.0f / speedMultiplier));
        }
        else
        {
            // Schedule the reverse transform after the spin attack
            StartCoroutine(ReverseTransformAfterSpin(1.0f / speedMultiplier));
        }
    }

    private IEnumerator DoubleSpin(float spinDuration)
    {
        // Wait for the first spin duration
        yield return new WaitForSeconds(spinDuration);

        // Trigger the spin again
        animator.SetTrigger("StartSpin");
        Debug.Log("Second StartSpin triggered for double spin");

        // Wait for the second spin duration
        yield return new WaitForSeconds(spinDuration);

        // Start the reverse transform
        animator.SetTrigger("StartReverse");
        Debug.Log("StartReverse triggered after double spin");
    }

    private IEnumerator ReverseTransformAfterSpin(float spinDuration)
    {
        // Wait for the spin duration
        yield return new WaitForSeconds(spinDuration);

        // Start the reverse transform
        animator.SetTrigger("StartReverse");
        Debug.Log("StartReverse triggered after spin");
    }

    private void ReverseTransform()
    {
        // Logic to reverse the transformation animation
        Debug.Log("Reversing transformation");
        animator.SetTrigger("StartReverse");
        Debug.Log("StartReverse triggered");
    }
}
