using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Raycaster : MonoBehaviour
{
    public ARRaycastManager raycaster;
    bool spawned;
    LayerMask mask = 1 << 29;
    public GameObject characterPrefab;

    // Update is called once per frame
    void Update()
    {
        
         if(Input.touchCount > 0)
        {
            List<ARRaycastHit> hitInfo = new List<ARRaycastHit>();
            Debug.Log("Touched");
            Touch tch = Input.GetTouch(0);
            if (!spawned)
            {
                Ray raycast = Camera.main.ScreenPointToRay(tch.position);
                if (raycaster.Raycast(raycast, hitInfo, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
                {
                    if (hitInfo[0].pose.up.Equals(Vector3.up))
                    {
                        spawned = true;
                        GameObject character = Instantiate(characterPrefab, hitInfo[0].pose.position, Quaternion.identity);
                        GameObject cam = Camera.main.gameObject;
                        character.transform.LookAt(new Vector3(cam.transform.position.x, character.transform.position.y, cam.transform.position.z));
                    }
                }
            }
        }
    }
}
