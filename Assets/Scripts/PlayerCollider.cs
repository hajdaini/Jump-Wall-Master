using UnityEngine;
using System.Collections;

public class PlayerCollider : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other){
        if(Player.instance.isDead) return;
        Vector2 collisionPos = other.contacts[0].point;
        AudioManager.instance.PlayCollisionAudio();
        if (other.transform.CompareTag(Global.tagWall)){
            StartCoroutine(Player.instance.SpawnJumpParticle(collisionPos));
            if (Player.instance.isRight && other.gameObject.name == "Right") CollideWithWall(collisionPos, false);
            if (!Player.instance.isRight && other.gameObject.name == "Left") CollideWithWall(collisionPos, true);
        }

        if (other.transform.CompareTag(Global.tagObstacle) && Player.instance.isDead == false){
            AudioManager.instance.PlayDeadAudio();
            Player.instance.isDead = true;
            GameObject effectObj = Instantiate(Player.instance.DeadEffectObj, other.contacts[0].point, Quaternion.identity);
            Destroy(effectObj, 0.5f);
            GameManager.instance.Gameover();
            StartCoroutine(Player.instance.cameraShake.Shake(0.2f, 0.3f));
            Player.instance.StopPlayer();
        }
    }

    void CollideWithWall(Vector3 collisionPos, bool isRight){
        Player.instance.isRight = isRight;
        GameManager.instance.AddScore();
        GameManager.instance.GetComponent<TriangleManager>().WallTouched(isRight);
        GameObject effectObj = Instantiate(Player.instance.WallBounceEffectObj, collisionPos, Quaternion.identity);
        Destroy(effectObj, 0.5f);
    }
}
