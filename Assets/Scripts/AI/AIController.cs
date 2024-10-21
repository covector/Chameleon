using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform player;
    public bool isHiding { get; private set; }
    public GameObject[] morphPrefabs;
    private GameObject currentMorph = null;
    public GameObject monster;
    public Vector2 goal { get; set; }
    public bool relativeGoal { get; set; }
    public bool isMoving { get; set; }
    public float baseSpeed = 4f;
    private float runningSpeed = 7f;
    public float speed { get; set; }
    GradientController gradient;
    public bool lost { get; private set; }

    void Start()
    {
        speed = baseSpeed;
        gradient = new GradientController();
        lost = false;
    }

    void Update()
    {
        if (isMoving && !isHiding)
        {
            Vector2 velocity = (
                    goal +
                    (relativeGoal ? Utils.ToVector2(player.position) : Vector2.zero) -
                    Utils.ToVector2(transform.position)
                ).normalized * speed * Time.deltaTime;
            transform.position = gradient.GetAdjustedPosition(transform, velocity.x, velocity.y, yOffset: 1f);
        }
        LoseCheck();
        if (lost)
        {
            Vector2 diff = Utils.ToVector2(transform.position - player.position);
            monster.transform.eulerAngles = new Vector3(0, Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg + 180f, 0);
        }
    }

    public void ChangeMorph()
    {
        monster.SetActive(false);
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
        currentMorph.GetComponent<MeshRenderer>().enabled = true;
    }

    public void Unhide()
    {
        if (!isHiding) { return; }
        isHiding = false;
        TryInitMorph();
        currentMorph.GetComponent<MeshRenderer>().enabled = false;
    }

    public void ApproachPlayer()
    {
        isMoving = true;
        relativeGoal = true;
        speed = baseSpeed;
        goal = Vector2.zero;
    }

    public void RunAway()
    {
        isMoving = false;
        relativeGoal = false;
        speed = runningSpeed;
        Unhide();
        Debug.Log("RUNNING AWAY");
        Vector2 diff = Utils.ToVector2(transform.position - player.position).normalized;
        goal = Utils.ToVector2(player.position) + diff * 100f;
        monster.SetActive(true);
        GetComponent<AnimatedTexture>().Play();
        monster.transform.eulerAngles = new Vector3(0, Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg + 180f, 0);
        monster.GetComponent<Animator>().ResetTrigger("Run");
        monster.GetComponent<Animator>().SetTrigger("Run");
        lost = false; // temp
    }

    public void StartRunning()
    {
        isMoving = true;
    }

    public void LoseCheck()
    {
        Vector2 diff = Utils.ToVector2(transform.position - player.position);
        if (diff.sqrMagnitude < 1f)
        {
            isMoving = false;
            lost = true;
            currentMorph.GetComponent<MeshRenderer>().enabled = false;
            monster.SetActive(true);
            monster.GetComponent<Animator>().ResetTrigger("TPose");
            monster.GetComponent<Animator>().SetTrigger("TPose");
            Debug.Log("YOU FUCKING LOST LOL");
        }
    }
}
