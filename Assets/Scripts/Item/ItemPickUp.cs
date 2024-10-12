using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    Transform cam;
    const float pickUpDistSqr = 9f;
    const float pickUpAngle = 0.98f;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            if (canBePickenUp())
            {
                GetComponent<MeshRenderer>().enabled = false;
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
