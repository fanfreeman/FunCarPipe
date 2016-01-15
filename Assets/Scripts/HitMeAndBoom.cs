using UnityEngine;
using System.Collections;

//有了这个脚本 装车直接炸车。。。
public class HitMeAndBoom : MonoBehaviour {

	void OnTriggerEnter(Collider collision)
    {
        CarExploderRigidbodyTrigger trigger = null;

        if(collision.transform.parent)
            trigger = collision.transform.parent.transform.
            gameObject.GetComponent<CarExploderRigidbodyTrigger>();

        if(trigger)
        trigger.Exploder();
	}
}
