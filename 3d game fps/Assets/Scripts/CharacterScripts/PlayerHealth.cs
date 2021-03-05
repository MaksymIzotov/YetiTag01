using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviour
{
    PhotonView pv;

    [SerializeField] float frozenTime;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void TakeDamage(int p_actor)
    {
        if (pv.IsMine)
        {
            gameObject.GetComponent<PlayerController>().isFrozen = true;
            StartCoroutine("Frozen");
        }
    }

    public void Unfreeze(int p_actor)
    {
        if (pv.IsMine)
        {
            gameObject.GetComponent<PlayerController>().isFrozen = false;
            StopCoroutine("Frozen");
        }
    }

    void BecomeYeti()
    {
        //TODO: Respawn as a Yeti
        gameObject.GetComponent<PlayerController>().isFrozen = false;
        gameObject.GetComponent<PlayerController>().LoadYetiSettings();
        gameObject.layer = 10;
        GameObject.Find("Spawner").GetComponent<PlayerSpawner>().role = 10;
        GameObject.Find("Spawner").GetComponent<PlayerSpawner>().yetiAmount++;
        GameObject.Find("Spawner").GetComponent<PlayerSpawner>().UpdateRoleText();
    }

    public IEnumerator Frozen()
    {
        float timer = 0;

        while (timer <= 1)
        {
            timer += Time.deltaTime / frozenTime;
            yield return null;
        }
        BecomeYeti();
    }
}
