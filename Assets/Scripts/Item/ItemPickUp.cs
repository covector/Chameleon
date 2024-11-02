using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    Transform cam;
    const float pickUpDistSqr = 9f;
    const float pickUpAngle = 0.98f;
    public GameObject scanner;
    public bool pickedUp = false;
    RandomAudio randomAudio;

    void Start()
    {
        cam = Camera.main.transform;
        randomAudio = GetComponent<RandomAudio>();
    }

    void Update()
    {
        if (Input.GetKeyDown(PlayerOptions.instance.KeyBinds["PickUp"]))
        {
            if (canBePickenUp())
            {
                scanner.SetActive(false);
                pickedUp = true;
                randomAudio.PlayRandomSound();
            }
        }
    }

    bool canBePickenUp()
    {
        Vector3 diff = transform.position - cam.position;
        float sqrMag = diff.sqrMagnitude;
        if (sqrMag < pickUpDistSqr)
        {
            float dot = Vector3.Dot(cam.forward, diff / Mathf.Sqrt(sqrMag));
            return dot > pickUpAngle;
        }
        return false;
    }
}
