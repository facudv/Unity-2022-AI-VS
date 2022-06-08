using UnityEngine;

public static class Utility
{
    public static bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & 1 << obj.layer) > 0;
}
