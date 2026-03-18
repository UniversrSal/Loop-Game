using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillEventController : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Transform cameraTarget;
    public Animator enemyAnimator;
    public Transform enemyTransform;
    public Transform playerTransform;
    public AudioSource heartbeatSource;

    [Header("Player")]
    public MonoBehaviour playerMovementScript;
    public Animator playerAnimator;

    [Header("Settings")]
    public float enemySpeed = 5f;
    public float cameraMoveDuration = 1f;
    public float minPitch = 0.8f;

    [Header("Dialogue")]
    public bool dialogueEnded = false;

    [Header("QTE System")]
    public QTESystem qteSystem;

   
    public float qteTimeout = 3f;

    private bool eventActive = false;

    public void StartKillEvent()
    {
        if (!dialogueEnded) return;

        eventActive = true;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (enemyAnimator != null)
            enemyAnimator.SetTrigger("Run");

        StartQTE();

        if (cameraTarget != null)
            StartCoroutine(MoveCameraToTarget(cameraTarget.position, cameraTarget.rotation));
    }

    void Update()
    {
        if (!eventActive) return;

        if (enemyTransform != null && playerTransform != null)
        {
            Vector3 dir = (playerTransform.position - enemyTransform.position).normalized;
            enemyTransform.position += dir * enemySpeed * Time.unscaledDeltaTime;
            enemyTransform.forward = dir;
        }
    }

    void StartQTE()
    {
        OnQTEStart();

        qteSystem.StartTimingQTE(
            KeyCode.Space,
            qteTimeout,
            OnQTEResult
        );
    }

    void OnQTEStart()
    {
        if (heartbeatSource != null)
        {
            heartbeatSource.pitch = minPitch;
            heartbeatSource.Play();
        }
    }

    void OnQTEResult(bool success)
    {
        if (success)
            OnQTESuccess();
        else
            OnQTEFail();
    }

    void OnQTESuccess()
    {
        StartCoroutine(DrawAndShoot());
    }

    void OnQTEFail()
    {
        FailEvent();
    }

    IEnumerator DrawAndShoot()
    {
        if (heartbeatSource != null)
            heartbeatSource.Stop();

        Time.timeScale = 1f;

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("DrawGun");
            yield return new WaitForSeconds(0.05f);
            playerAnimator.SetTrigger("ShootGun");
            yield return new WaitForSeconds(0.3f);
        }

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        SceneManager.LoadScene("WinScene");
    }

    void FailEvent()
    {
        eventActive = false;
        Time.timeScale = 1f;

        if (heartbeatSource != null)
            heartbeatSource.Stop();

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        SceneManager.LoadScene("LoseScene");
    }

    IEnumerator MoveCameraToTarget(Vector3 targetPos, Quaternion targetRot)
    {
        if (mainCamera == null) yield break;

        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float t = 0f;

        while (t < cameraMoveDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / cameraMoveDuration;
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, lerp);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, lerp);
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
    }
}
