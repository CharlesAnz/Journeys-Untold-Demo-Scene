using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Transform target;
    PlayerManager playerManager;

    public Vector3 offset;
    public float zoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    public float pitch = 2f;

    public float yawSpeed = 100f;

    [SerializeField]
    private float currentZoom = 15f;
    [SerializeField]
    private float currentYaw = 0f;

    private void Start()
    {
        playerManager = PlayerManager.instance;

        target = playerManager.activePerson.transform;
    }

    void Update()
    {
        if (playerManager.gameOver) return;
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        target = playerManager.activePerson.gameObject.transform;

        //currentYaw = target.rotation.y;

        currentYaw -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;


    }


    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position - offset * currentZoom;
        transform.LookAt(target.position + Vector3.up * pitch);
        transform.RotateAround(target.position, Vector3.up, currentYaw);

        //transform.Rotate(new Vector3(transform.rotation.x, currentYaw, transform.rotation.z), currentYaw);
    }
}
