using UnityEngine;

public class WallLaserSwitch : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject lasersParent; // obiectul părinte cu toate laserele
    [SerializeField] private bool oneTimeUse = true;   // switch-ul se poate folosi o singură dată
    [SerializeField] private Color activeColor = Color.green; // culoarea când e activat
    [SerializeField] private Color inactiveColor = Color.red; // culoarea inițială

    private bool isActivated = false;
    private Renderer switchRenderer;

    private void Awake()
    {
        switchRenderer = GetComponent<Renderer>();
        if (switchRenderer != null)
            switchRenderer.material.color = inactiveColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        // verificăm dacă obiectul care intră este jucătorul
        if (!other.CompareTag("Player") || isActivated && oneTimeUse) return;

        ActivateSwitch();
    }

    private void ActivateSwitch()
    {
        isActivated = true;

        if (switchRenderer != null)
            switchRenderer.material.color = activeColor;

        // Găsim toate LaserBeam instanțiate în scenă
        var allBeams = FindObjectsOfType<LaserBeam>();
        foreach (var beam in allBeams)
        {
            beam.gameObject.SetActive(false); // dezactivăm vizual beam-ul
        }
    }

    // Opțional: dacă vrei să fie toggle în loc de o singură utilizare
    public void ToggleSwitch()
    {
        if (lasersParent == null) return;

        isActivated = !isActivated;

        // schimbăm culoarea switch-ului
        if (switchRenderer != null)
            switchRenderer.material.color = isActivated ? activeColor : inactiveColor;

        foreach (Transform child in lasersParent.transform)
        {
            child.gameObject.SetActive(!isActivated);
        }
    }
}
