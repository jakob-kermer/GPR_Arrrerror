using UnityEngine;

public class DestroyAnimation : MonoBehaviour
{
    [SerializeField] private float timer = 1f;

    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
