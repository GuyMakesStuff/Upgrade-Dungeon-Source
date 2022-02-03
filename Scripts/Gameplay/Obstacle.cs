using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UpgradeDungeon.Managers;

public class Obstacle : MonoBehaviour
{
    public string ObstacleTag;

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.tag == "Player")
        {
            Player player = other.collider.GetComponent<Player>();
            if(player.HasAbillity("Die To " + ObstacleTag))
            {
                GameManager.Instance.Kill();
            }
        }
    }
}
