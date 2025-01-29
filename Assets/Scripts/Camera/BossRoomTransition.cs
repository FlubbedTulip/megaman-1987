using System.Collections;
using Managers;
using Mega_man;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Camera
{
    public class BossRoomTransition : MonoBehaviour
    {
        [Header("Gate Settings")]
        [SerializeField] private GameObject[] doorRows;  // Each element is one horizontal row of tiles
        [SerializeField] private float rowDisableDelay = 0.2f; // time between disabling rows

        [FormerlySerializedAs("lowerCamera")]
        [Header("Camera Settings")]
        [SerializeField] private CinemachineCamera leftCamera;
        [SerializeField] private CinemachineCamera rightCamera;

        [Header("Transition Timings")]
        public float pauseDuration = 0.5f;
        public float transitionDuration = 1f;

        [Header("Player References")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private PlayerController player;
        
        [Header("Audio")]
        [SerializeField] private AudioClip openGateSound;

        private bool _isSwitching;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !_isSwitching)
            {
                StartCoroutine(BossRoomSequence());
            }
        }

        private IEnumerator BossRoomSequence()
        {
            _isSwitching = true;

            // Pause game fully
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(pauseDuration);

            Time.timeScale = 1f;

            // Open gate
            yield return StartCoroutine(OpenGateCoroutine());

            // Pause again before camera transition
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(pauseDuration);
            Time.timeScale = 1f;

            // Disable player input & set kinematic
            playerInput.actions.Disable();
            player.Rb.bodyType = RigidbodyType2D.Kinematic;
            player.Rb.linearVelocity = Vector2.zero;

            // 6) Switch to boss camera
            leftCamera.Priority = 0;
            rightCamera.Priority = 1;
            
            // 7) Manually move the player
            yield return StartCoroutine(ManualMovePlayer());

            // 8) Pause again if you want
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(pauseDuration);
            Time.timeScale = 1f;

            // 9) Re-enable physics & input
            player.Rb.bodyType = RigidbodyType2D.Dynamic;
            playerInput.actions.Enable();

            _isSwitching = false;
            ReEnableGate();
        }

        private IEnumerator OpenGateCoroutine()
        {
            for (int i = 0; i < doorRows.Length; i++)
            {
                doorRows[i].SetActive(false);
                SoundManager.Instance.PlaySound(openGateSound);
                yield return new WaitForSeconds(rowDisableDelay);
            }
        }

        private IEnumerator ManualMovePlayer()
        {
            float elapsed = 0f;
            Vector3 startPos = player.transform.position;
            // Move right 4 units 
            Vector3 targetPos = startPos +  Vector3.right * 4f;
            while (elapsed < transitionDuration)
            {
                player.Animator.SetBool("IsRunning", true);
                float t = elapsed / transitionDuration;
                player.transform.position = Vector3.Lerp(startPos, targetPos, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            player.Animator.SetBool("IsRunning", false);

            // Snap final
            player.transform.position = targetPos;
        }
        
        private void ReEnableGate()
        {
            foreach (var row in doorRows)
            {
                row.SetActive(true);
            }
        }
    }
}
