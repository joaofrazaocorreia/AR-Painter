using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneScanner : MonoBehaviour
{
    [SerializeField] private float planeCheckCooldown = 0.5f;
    private GameManager gameManager;
    private bool enabledScanning;
    public bool EnabledScanning { get => enabledScanning; set { enabledScanning = value; } }
    private float planeCheckTimer;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        planeCheckTimer = planeCheckCooldown;
    }

    private void Update()
    {
        if (EnabledScanning)
        {
            if (planeCheckTimer > 0)
            {
                planeCheckTimer -= Time.deltaTime;
            }

            else
            {
                if (FindObjectsByType<ARPlane>(FindObjectsSortMode.None).Count() > 0)
                {
                    gameManager.AdvanceActionMode();
                }

                else
                {
                    planeCheckTimer = planeCheckCooldown;
                }
            }
        }
    }
}
