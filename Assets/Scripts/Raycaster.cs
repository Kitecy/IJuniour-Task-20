using UnityEngine;

public class Raycaster : MonoBehaviour
{
    public bool Raycast<T>(Ray ray, out T obj) where T : MonoBehaviour
    {
        obj = null;

        if (Physics.Raycast(ray, out RaycastHit hit) == false)
            return false;

        if (hit.collider.TryGetComponent(out T component))
            obj = component;

        return true;
    }

    public bool Raycast(Ray ray, out Vector3 point, LayerMask detectableMasks)
    {
        point = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hit, int.MaxValue, detectableMasks) == false)
            return false;

        point = hit.point;

        return true;
    }
}
