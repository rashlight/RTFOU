using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BulletSummoner : MonoBehaviour
{
    [HideInInspector] public GameObject summonBullet;

    public void Summon()
    {
        GameObject obj = Instantiate(summonBullet, transform.position, summonBullet.transform.rotation);
    }
}
