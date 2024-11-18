using System;
using UnityEngine;
using static Utils;

public class AIController : MonoBehaviour
{
    public Transform player;
    public GameObject[] morphPrefabs;
    public GameObject currentMorph { get; private set; }
    public GameObject monster;
    GradientController gradient;
    public Transform centerOfMass;

    void Start()
    {
        if (!GameSettings.includeMonster) {
            FindFirstObjectByType<RandomLocationSound>().gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        currentMorph = null;
        gradient = new GradientController();
    }

    // Use rock location
    public void MoveRock(Vector2 velocity)
    {
        transform.position = gradient.GetAdjustedPosition(transform, velocity.x, velocity.y, yOffset: 1f);
    }

    // Use monster mesh location
    public void MoveMonster(Vector2 velocity)
    {
        Vector3 withAccurateY = gradient.GetAdjustedPosition(centerOfMass, velocity.x, velocity.y, yOffset: 1f);
        transform.position = new Vector3(
            transform.position.x - centerOfMass.position.x + withAccurateY.x,
            withAccurateY.y,
            transform.position.z - centerOfMass.position.z + withAccurateY.z
        );
    }

    // Use rock location
    public void MoveToRock(Vector2 position)
    {
        transform.position = ProjectToGround(position.x, position.y) + Vector3.up;
    }

    // Use monster mesh location
    public void MoveToMonster(Vector2 position)
    {
        Vector3 withAccurateY = ProjectToGround(centerOfMass.position.x, centerOfMass.position.z) + Vector3.up;
        transform.position = new Vector3(
            position.x,
            withAccurateY.y,
            position.y
        );
    }

    // correct y position for monster mesh
    public void CorrectToMonster()
    {
        Vector3 withAccurateY = ProjectToGround(centerOfMass.position.x, centerOfMass.position.z) + Vector3.up;
        transform.position = new Vector3(
            transform.position.x,
            withAccurateY.y,
            transform.position.z
        );
    }

    public Vector2 GetDiff(bool normalized = true)
    {
        Vector2 diff = ToVector2(player.position) - ToVector2(transform.position);
        return normalized ? diff.normalized : diff;
    }

    public Vector3 GetMorphPosition()
    {
        return currentMorph == null ? Vector3.zero : currentMorph.transform.position;
    }

    public void ChangeMorph()
    {
        if (currentMorph == null)
        {
            currentMorph = Instantiate(morphPrefabs[0]);
        }
        ProceduralAsset pa = currentMorph.GetComponent<ProceduralAsset>();
        pa.enabled = true;
        bool haveCandidate = false;
        for (int i = 0; i < 15; i++)
        {
            pa.Generate(UnityEngine.Random.Range(0, 10000));
            if (pa.MaxDim() > 0.5f) { haveCandidate = true; break; }
        }
        if (!haveCandidate)
        {
            int startInd = UnityEngine.Random.Range(0, pa.PreGenCount());
            for (int i = 0; i < pa.PreGenCount(); i++) 
            {
                pa.Generate(startInd + i);
                if (pa.MaxDim() > 0.5f) { break; }
            }
        }
        currentMorph.GetComponent<MeshRenderer>().enabled = false;
        currentMorph.GetComponent<ProceduralAsset>().enabled = false;
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
    public void ToggleMonster(bool toggle)
    {
        monster.SetActive(toggle);
    }
    public void ToggleMorph(bool toggle)
    {
        TryInitMorph();
        currentMorph.GetComponent<MeshRenderer>().enabled = toggle;
    }

    private Action TurnCallback;
    public void PlayMonsterRunAnimation(Action TurnCallback)
    {
        this.TurnCallback = TurnCallback;
        ToggleMonster(true);
        Vector2 diff = GetDiff();
        GetComponent<AnimatedTexture>().Play();
        monster.transform.eulerAngles = new Vector3(0, Mathf.Atan2(-diff.x, -diff.y) * Mathf.Rad2Deg + 180f, 0);
        monster.GetComponent<Animator>().ResetTrigger("Run");
        monster.GetComponent<Animator>().SetTrigger("Run");
    }
    public void StartRunning()
    {
        if (TurnCallback != null) { TurnCallback(); }
    }

    public bool IsLost()
    {
        return GetDiff(normalized: false).sqrMagnitude < 1f;
    }
}
