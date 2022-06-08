using UnityEngine;

public class Shield : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var bullet = other.gameObject.GetComponent<Bullet>();
        if (!bullet) return;
        if (bullet.TargetLayerMask.value != 1<<Layers.iaBlue) Destroy(other.gameObject);
    }
}
