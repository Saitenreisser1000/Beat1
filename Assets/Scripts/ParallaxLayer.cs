using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxFactorX = 0.5f;
    [SerializeField] private float parallaxFactorY = 0f;

    private Vector3 initialOffset;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        initialOffset = transform.position - cameraTransform.position;
    }

    void LateUpdate()
    {
        float posX = cameraTransform.position.x + initialOffset.x - cameraTransform.position.x * parallaxFactorX;
        float posY = cameraTransform.position.y + initialOffset.y + cameraTransform.position.y * parallaxFactorY;

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
