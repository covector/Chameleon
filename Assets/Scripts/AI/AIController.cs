using UnityEngine;

public class AIController : MonoBehaviour
{
    public TerrainGeneration tgen;
    public Transform player;
    public bool isHiding { get; private set; }
    public GameObject[] morphPrefabs;
    private GameObject currentMorph = null;
    public Vector2 goal { get; set; }
    public bool relativeGoal { get; set; }
    public bool isMoving { get; set; }
    public float baseSpeed = 5f;
    public float speed { get; set; }

    void Start()
    {
        speed = baseSpeed;
    }

    void Update()
    {
        if (isMoving && !isHiding)
        {
            Vector2 velocity = (
                    goal +
                    (relativeGoal ? new Vector2(player.position.x, player.position.z) : Vector2.zero) -
                    new Vector2(transform.position.x, transform.position.z)
                ).normalized * speed * Time.deltaTime;
            float x = transform.position.x + velocity.x;
            float z = transform.position.z + velocity.y;
            transform.position = new Vector3(
                x,
                tgen.GetGroudLevel(x, z, 1) + 1f,
                z
            );

            LoseCheck();
        }
    }

    public void ChangeMorph()
    {
        if (currentMorph != null)
        {
            Destroy(currentMorph);
        }
        currentMorph = Instantiate(morphPrefabs[0]);
        currentMorph.GetComponent<ProceduralAsset>().Generate(Random.Range(0, 10000));
        currentMorph.GetComponent<MeshRenderer>().enabled = false;
        currentMorph.transform.parent = transform;
        currentMorph.transform.localPosition = new Vector3(0f, -1f, 0f);
    }

    public void TryInitMorph()
    {
        if (currentMorph == null)
        {
            ChangeMorph();
        }
    }

    public void Hide()
    {
        if (isHiding) { return; }
        isHiding = true;
        TryInitMorph();
        GetComponent<MeshRenderer>().enabled = false;
        currentMorph.GetComponent<MeshRenderer>().enabled = true;
    }

    public void Unhide()
    {
        if (!isHiding) { return; }
        isHiding = false;
        TryInitMorph();
        GetComponent<MeshRenderer>().enabled = true;
        currentMorph.GetComponent<MeshRenderer>().enabled = false;
    }

    public void ApproachPlayer()
    {
        isMoving = true;
        relativeGoal = true;
        goal = Vector2.zero;
    }

    public void RunAway()
    {
        isMoving = true;
        relativeGoal = false;
        goal = new Vector2(transform.position.x - player.position.x, transform.position.z - player.position.z).normalized * 30f;
    }

    public void LoseCheck()
    {
        Vector2 diff = new Vector2(transform.position.x - player.position.x, transform.position.z - player.position.z);
        if (diff.SqrMagnitude() < 1f)
        {
            Debug.Log("YOU FUCKING LOST LOL");
        }
    }
}
