using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private float cameraSmooth = 0.1f;
    [SerializeField] private bool cameraClamp = false;
    [SerializeField] private float minZ, maxZ = 0;
    [SerializeField] private float zOffset = 3;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = player.transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, cameraSmooth);
        transform.position = new Vector3(0, 10, player.position.z);

        if (cameraClamp)
        {
            transform.position = new Vector3(0, 10, Mathf.Clamp(transform.position.z + zOffset, minZ, maxZ));
        }
    }
}