using UnityEngine;

public class DestroyAnimation : MonoBehaviour
{
    [Header("Seconds after which this object gets destroyed")]
    [SerializeField] private float timer = 1f;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
