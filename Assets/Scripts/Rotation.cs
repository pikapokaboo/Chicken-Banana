using UnityEngine;

public class Rotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 90f;

    private Renderer[] objectRenderers;
    private Collider2D[] objectColliders2D;
    private Collider[] objectColliders;

    void Start()
    {
        objectRenderers = GetComponentsInChildren<Renderer>();
        objectColliders2D = GetComponentsInChildren<Collider2D>();
        objectColliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        transform.RotateAround(GetModelCenter(), Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private Vector3 GetModelCenter()
    {
        Bounds bounds;

        if (TryGetObjectBounds(out bounds))
        {
            return bounds.center;
        }

        return transform.position;
    }

    private bool TryGetObjectBounds(out Bounds bounds)
    {
        bool hasBounds = false;
        bounds = new Bounds(transform.position, Vector3.zero);

        for (int i = 0; i < objectRenderers.Length; i++)
        {
            if (objectRenderers[i] == null || !objectRenderers[i].enabled)
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = objectRenderers[i].bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(objectRenderers[i].bounds);
            }
        }

        for (int i = 0; i < objectColliders2D.Length; i++)
        {
            if (objectColliders2D[i] == null || !objectColliders2D[i].enabled)
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = objectColliders2D[i].bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(objectColliders2D[i].bounds);
            }
        }

        for (int i = 0; i < objectColliders.Length; i++)
        {
            if (objectColliders[i] == null || !objectColliders[i].enabled)
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = objectColliders[i].bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(objectColliders[i].bounds);
            }
        }

        return hasBounds;
    }
}
