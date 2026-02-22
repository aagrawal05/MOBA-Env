
using UnityEngine;

public class AlwaysFaceCameraUI : MonoBehaviour
{
    private Camera mainCamera;
    private RectTransform rectTransform;

    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (mainCamera != null && rectTransform != null)
        {
            Vector3 directionToCamera = mainCamera.transform.position - rectTransform.position;
            /* directionToCamera.y = 0; */
            rectTransform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}
