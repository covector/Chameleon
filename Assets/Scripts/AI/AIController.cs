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

    void Start()
    {
        currentMorph = null;
        gradient = new GradientController();
    }

    public void Move(Vector2 velocity)
    {
        transform.position = gradient.GetAdjustedPosition(transform, velocity.x, velocity.y, yOffset: 1f);
    }
    public void MoveTo(Vector2 position)
    {
        transform.position = ProjectToGround(position.x, position.y) + Vector3.up;
    }
    public Vector2 GetDiff(bool normalized = true)
    {
        Vector2 diff = ToVector2(player.position) - ToVector2(transform.position);
        return normalized ? diff.normalized : diff;
    }

    public void ChangeMorph()
    {
        if (currentMorph == null)
        {
            currentMorph = Instantiate(morphPrefabs[0]);
        }
        currentMorph.GetComponent<ProceduralAsset>().enabled = true;
        for (int i = 0; i < 10; i++)
        {
            currentMorph.GetComponent<ProceduralAsset>().Generate(UnityEngine.Random.Range(0, 10000));
            if (currentMorph.GetComponent<ProceduralAsset>().MaxDim() > 0.5f) { break; }
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
