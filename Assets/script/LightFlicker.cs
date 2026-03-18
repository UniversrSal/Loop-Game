using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class LightFlicker : MonoBehaviour
{
    [Header("Intensity Settings")]
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 2f;

    [Header("Flicker Speed")]
    [Tooltip("Time (in seconds) between intensity changes")]
    [SerializeField] private float flickerInterval = 0.05f; // smaller = faster

    [Header("Optional Color Flicker")]
    [SerializeField] private bool flickerColor = false;
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = Color.cyan;

    [Header("Electric Sound Settings")]
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1f;

    private Light lightSource;
    private AudioSource audioSource;
    private float timer;

    void Awake()
    {
        lightSource = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();

        if (lightSource == null || audioSource == null)
        {
            Debug.LogError("FlickerWithSound requires both Light and AudioSource components!");
            enabled = false;
        }

        // Make sure the audio source is set to loop
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Pick new random intensity
            float newIntensity = Random.Range(minIntensity, maxIntensity);
            lightSource.intensity = newIntensity;

            // Optional color flicker
            if (flickerColor)
                lightSource.color = Color.Lerp(colorA, colorB, Random.value);

            // Map intensity to volume
            audioSource.volume = Mathf.InverseLerp(minIntensity, maxIntensity, newIntensity)
                                 * (maxVolume - minVolume) + minVolume;

            timer = flickerInterval;
        }
    }
}
