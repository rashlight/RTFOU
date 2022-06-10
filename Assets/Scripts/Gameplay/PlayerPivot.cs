using UnityEngine;

public class PlayerPivot : MonoBehaviour
{
    public Player player;

    private void FixedUpdate()
    {
        gameObject.transform.position = player.transform.position;
    }
}
