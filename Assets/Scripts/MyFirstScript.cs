using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    [Header("Glitch Settings")]
    public float jitterIntensity = 0.05f; // How much it shakes
    public float glitchFrequency = 0.1f; // How fast it shakes
    
    private Vector3 originalPosition;

    void Start()
    {
        print("Step Bro im stuck in the wall");
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Calculate a random offset to simulate physics jitter
        Vector3 randomOffset = new Vector3(
            Random.Range(-jitterIntensity, jitterIntensity),
            Random.Range(-jitterIntensity, jitterIntensity),
            Random.Range(-jitterIntensity, jitterIntensity)
        );

        // Apply jitter relative to the stuck position
        transform.localPosition = originalPosition + randomOffset;
    }
}