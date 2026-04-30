using UnityEngine;

public class NewComponent : MonoBehaviour
{
    [Header("Bounce Settings")]
    public Camera targetCamera;
    public float speed = 3f;
    public Vector2 startDirection = new Vector2(1f, 1f);
    public float screenPadding = 8f;

    private Vector2 velocity;
    private Renderer[] objectRenderers;
    private Collider2D[] objectColliders2D;
    private Collider[] objectColliders;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        objectRenderers = GetComponentsInChildren<Renderer>();
        objectColliders2D = GetComponentsInChildren<Collider2D>();
        objectColliders = GetComponentsInChildren<Collider>();

        if (startDirection == Vector2.zero)
        {
            startDirection = Random.insideUnitCircle.normalized;
        }

        velocity = startDirection.normalized * speed;
    }

    void Update()
    {
        if (targetCamera == null)
        {
            return;
        }

        Vector3 position = transform.position;

        position.x += velocity.x * Time.deltaTime;
        position.y += velocity.y * Time.deltaTime;

        position = ClampInsideCamera(position);
        transform.position = position;
    }

    private Vector3 ClampInsideCamera(Vector3 position)
    {
        Bounds objectBounds = GetObjectBoundsAtPosition(position);
        Vector2 objectOffset = new Vector2(objectBounds.center.x - position.x, objectBounds.center.y - position.y);
        Vector2 objectHalfSize = new Vector2(objectBounds.extents.x, objectBounds.extents.y);
        Vector2 padding = GetWorldPadding(objectBounds.center.z);
        Vector4 cameraBounds = GetCameraWorldBounds(objectBounds.center.z);

        float minX = cameraBounds.x + padding.x + objectHalfSize.x - objectOffset.x;
        float maxX = cameraBounds.y - padding.x - objectHalfSize.x - objectOffset.x;
        float minY = cameraBounds.z + padding.y + objectHalfSize.y - objectOffset.y;
        float maxY = cameraBounds.w - padding.y - objectHalfSize.y - objectOffset.y;

        if (position.x <= minX || position.x >= maxX)
        {
            velocity.x = -velocity.x;
            position.x = Mathf.Clamp(position.x, minX, maxX);
        }

        if (position.y <= minY || position.y >= maxY)
        {
            velocity.y = -velocity.y;
            position.y = Mathf.Clamp(position.y, minY, maxY);
        }

        return position;
    }

    private Bounds GetObjectBoundsAtPosition(Vector3 position)
    {
        Bounds bounds;

        if (TryGetObjectBounds(out bounds))
        {
            bounds.center += position - transform.position;
            return bounds;
        }

        return new Bounds(position, Vector3.zero);
    }

    private Vector2 GetWorldPadding(float zPosition)
    {
        Vector4 cameraBounds = GetCameraWorldBounds(zPosition);
        float worldWidth = cameraBounds.y - cameraBounds.x;
        float worldHeight = cameraBounds.w - cameraBounds.z;

        return new Vector2(
            screenPadding / targetCamera.pixelWidth * worldWidth,
            screenPadding / targetCamera.pixelHeight * worldHeight
        );
    }

    private Vector4 GetCameraWorldBounds(float zPosition)
    {
        float distanceFromCamera = Mathf.Abs(zPosition - targetCamera.transform.position.z);

        if (targetCamera.orthographic)
        {
            float height = targetCamera.orthographicSize * 2f;
            float width = height * targetCamera.aspect;
            Vector3 center = targetCamera.transform.position;

            return new Vector4(
                center.x - width * 0.5f,
                center.x + width * 0.5f,
                center.y - height * 0.5f,
                center.y + height * 0.5f
            );
        }

        Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceFromCamera));
        Vector3 topRight = targetCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceFromCamera));

        return new Vector4(bottomLeft.x, topRight.x, bottomLeft.y, topRight.y);
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
