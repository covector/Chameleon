using UnityEngine;
using UnityEngine.InputSystem.XR;

public class DeathCoordinator : MonoBehaviour
{
    public Transform cam;
    public PlayerControl playerControl;
    public Flashlight flashlight;
    public Transform monster;
    public AIController controller;
    public Transform cameraCoordinator;
    public Animator monsterAnimator;
    public GameObject scanner;

    public void StartDeathScene()
    {
        scanner.SetActive(false);
        playerControl.enabled = false;
        transform.position = cam.position + Vector3.down;
        float yaw = cam.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, yaw, 0);
        cam.parent = cameraCoordinator;
        cam.localEulerAngles = new Vector3(cam.localEulerAngles.x, 0, cam.localEulerAngles.z);
        //cam.localPosition = Vector3.up;
        GetComponent<Animator>().ResetTrigger("Knocked");
        GetComponent<Animator>().SetTrigger("Knocked");
    }

    public void OffFlashlight()
    {
        flashlight.ToggleFlashlight(false);
    }

    public void FallOnTheGround()
    {
        cam.localEulerAngles = new Vector3(0, 0, 0);
        FindFirstObjectByType<SceneTransition>().BlockScreen();
    }

    public void LookUp()
    {
        flashlight.ToggleFlashlight(true);
        monster.position = transform.position;
        monster.rotation = transform.rotation;
        FindFirstObjectByType<SceneTransition>().UnblockScreen();
    }

    public void PlayMonsterAnim()
    {
        controller.ToggleMonster(true);
        monsterAnimator.ResetTrigger("Kill");
        monsterAnimator.SetTrigger("Kill");
    }

    public void Death()
    {
        FindFirstObjectByType<SceneTransition>().BlockScreen();
    }

    private bool lookAt = false;
    private void Update()
    {
        if (lookAt)
        {
            Quaternion lookAtRotation = Quaternion.LookRotation(controller.centerOfMass.position - cam.position);
            cam.rotation = Quaternion.Slerp(cam.rotation, lookAtRotation, Time.deltaTime);
        }
    }
    public void StartLookAt() { lookAt = true; }
    public void StopLookAt() { lookAt = false; }
}
