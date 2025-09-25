using UnityEngine;

public class WaterParticle : MonoBehaviour
{
    public float waterPerHit = 5f; // Amount added per collision

    void OnParticleCollision(GameObject other)
    {
        PlantGrowth plant = other.GetComponent<PlantGrowth>();
        if (plant != null)
        {
            plant.AddWater(waterPerHit);
        }
    }
}