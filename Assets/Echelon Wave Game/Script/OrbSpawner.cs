using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public GameObject orbPrefab;
    public Transform spawnPoint;

    void Start()
    {
        //StartCoroutine(SpawnOrbsRepeatedly());
        //SpawnOrbsBelow();
    }

    void Update()
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("orbs");
        foreach (GameObject orb in orbs)
        {
            if (Mathf.Abs(orb.transform.position.x - (-14f)) < 0.01f)
            {
                Destroy(orb);
            }
        }
    }

    // IEnumerator SpawnOrbsRepeatedly()
    // {

    //     while (true)
    //     {
    //         SpawnOrbsAbove();
    //         SpawnOrbsBelow();
    //         yield return new WaitForSeconds(4f);
    //     }
    // }

    // //this is for above orb instantiation
    // void SpawnOrbsAbove()
    // {
    //     SpawnOrb(new Vector3(10.52f, 2.62f, spawnPoint.position.z));
    //     SpawnOrb(new Vector3(10.99f, 3.1f, spawnPoint.position.z));
    //     SpawnOrb(new Vector3(11.44f, 2.6f, spawnPoint.position.z));
    //     SpawnOrb(new Vector3(11.81f, 2.05f, spawnPoint.position.z));
    // }

    // //this is for below orb instantiation
    // void SpawnOrbsBelow()
    // {
    //     SpawnOrb(new Vector3(14.05f, -1.97f, spawnPoint.position.z));
    //     SpawnOrb(new Vector3(14.42f, -2.49f, spawnPoint.position.z));
    //     SpawnOrb(new Vector3(14.81f, -3.01f, spawnPoint.position.z));
    //     SpawnOrb(new Vector3(15.39f, -2.51f, spawnPoint.position.z));
    // }

    // void SpawnOrb(Vector3 position)
    // {
    //     Instantiate(orbPrefab, position, Quaternion.identity);
    // }


}
