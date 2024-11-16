using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DeathCoordinator : MonoBehaviour
{
    [Serializable]
    public struct AudioClips
    {
        public AudioClip camera_knocking;
        public AudioClip camera_knocking_2;
        public AudioClip impact_dirt;
        public AudioClip chimpanzee_close;
    }
    public Transform cam;
    public PlayerControl playerControl;
    public Flashlight flashlight;
    public Transform monster;
    public AIController controller;
    public Transform cameraCoordinator;
    public Animator monsterAnimator;
    public GameObject scanner;
    public Volume volume;
    public Transform[] monsterEyes;
    public AudioSource audioSource;
    public AudioClips clips;

    public void StartDeathScene()
    {
        FindFirstObjectByType<PauseGame>().canPause = false;
        DepthOfField dof;
        volume.profile.TryGet(out dof);
        dof.focusDistance.value = 5f;
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
        audioSource.PlayOneShot(clips.camera_knocking, 1f);
        FindFirstObjectByType<RandomLocationSound>().GetComponent<AudioSource>().Stop();
    }

    public void OffFlashlight()
    {
        flashlight.ToggleFlashlight(false);
    }

    public void FallOnTheGround()
    {
        cam.localEulerAngles = new Vector3(0, 0, 0);
        FindFirstObjectByType<SceneTransition>().BlockScreen();
        audioSource.PlayOneShot(clips.impact_dirt, 1f);
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
        monster.GetComponent<AnimatedTexture>().SetTo(0f);
        monster.GetComponent<AudioSource>().PlayOneShot(clips.chimpanzee_close, 0.4f);
    }

    public void TextureChange()
    {
        monster.GetComponent<AnimatedTexture>().Play();
    }

    public void PlayFall()
    {
        audioSource.PlayOneShot(clips.camera_knocking, 1f);
    }

    public void Death()
    {
        FindFirstObjectByType<DeathScreen>().Play();
        audioSource.Stop();
        monster.GetComponent<AudioSource>().Stop();
    }

    private bool lookAt = false;
    private void LateUpdate()
    {
        if (lookAt)
        {
            Quaternion lookAtRotation = Quaternion.LookRotation(controller.centerOfMass.position - cam.position);
            cam.rotation = Quaternion.Slerp(cam.rotation, lookAtRotation, Time.deltaTime);
            Quaternion inverseLookAtRotation = Quaternion.LookRotation(cam.position - controller.centerOfMass.position);
            foreach (Transform eye in monsterEyes)
            {
                eye.rotation = inverseLookAtRotation;
            }
        }
    }

    public void StartLookAt() { lookAt = true; }
    public void StopLookAt() { lookAt = false; }
}
