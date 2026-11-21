using System.Collections.Generic;
using UnityEngine;

// little burst of glowing shards, they fly out and shrink away (physical bolt hits and mechanic landings)
public class BoltImpact : MonoBehaviour
{
    private const int shardCount = 6; // how many bits fly out
    private const float radius = 0.8f; // how far they get
    private const float duration = 0.3f; // how long the burst lasts

    // one shared material per colour, so we dont make a new one every hit
    // (and a brown bolt burst and a grey mechanic landing at the same time keep their own colours)
    private static Dictionary<Color, Material> shardMaterials = new Dictionary<Color, Material>();
    private float timer = 0f;

    // makes a burst at a position, used for bolt hits and mechanic landings
    public static void Spawn(Vector3 position, Color colour)
    {
        Material shardMaterial;

        if (!shardMaterials.TryGetValue(colour, out shardMaterial))
        {
            shardMaterial = new Material(Shader.Find("Standard"));
            shardMaterial.color = colour;
            shardMaterial.EnableKeyword("_EMISSION");
            shardMaterial.SetColor("_EmissionColor", colour * 2f);
            shardMaterials[colour] = shardMaterial;
        }

        GameObject burst = new GameObject("Bolt Impact");
        burst.transform.position = position;
        burst.AddComponent<BoltImpact>();

        for (int i = 0; i < shardCount; i++)
        {
            GameObject shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(shard.GetComponent<Collider>()); // just for looks
            shard.transform.SetParent(burst.transform, false);
            shard.transform.rotation = Random.rotation; // random tilt so they look like broken bits
            shard.GetComponent<Renderer>().sharedMaterial = shardMaterial;
        }
    }

    // spread the shards round a circle, moving out and shrinking, then remove the burst
    void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / duration);

        for (int i = 0; i < transform.childCount; i++)
        {
            float angle = i * Mathf.PI * 2f / shardCount;
            transform.GetChild(i).localPosition = new Vector3(Mathf.Cos(angle), 0.3f, Mathf.Sin(angle)) * radius * progress;
            transform.GetChild(i).localScale = Vector3.one * 0.15f * (1f - progress);
        }

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
