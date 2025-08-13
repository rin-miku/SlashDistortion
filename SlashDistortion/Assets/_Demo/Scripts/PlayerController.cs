using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject slashPrefab;
    public bool showSlash;

    public void Attack()
    {
        Slash slash = Instantiate(slashPrefab).GetComponent<Slash>();
        slash.EnableSlash(showSlash);
    }
}
