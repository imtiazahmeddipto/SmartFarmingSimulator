using UnityEngine;

public class FertilizerParticle : MonoBehaviour
{
    public float fertilizerPerHit = 10f; // each particle adds this much

    void OnParticleCollision(GameObject other)
    {
        PlantGrowth plant = other.GetComponent<PlantGrowth>();
        if (plant != null)
        {
            plant.AddFertilizer(fertilizerPerHit);
        }
    }
}
