using UnityEngine;

public static class BasketHelpers
{
    public static void ApplyFacing(Transform transform, bool facesRight)
    {
        float xSign = facesRight ? 1f : -1f;
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(scale.x) * xSign, scale.y, scale.z);
    }
    public static T GetRandomElement<T>(T[] Collection)
    {
        int index = Random.Range(0, Collection.Length);
        return Collection[index];
    }
}
