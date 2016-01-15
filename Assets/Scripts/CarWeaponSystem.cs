using UnityEngine;
using System.Collections;

public class CarWeaponSystem : MonoBehaviour {

    [HideInInspector]
    public float inputX;


    //射出去的炮弹
    public GameObject fireMe;
    //根据车的位置生成炮弹
    public GameObject carBody;

    private GameObject physics;
    //车的刚体
    private Rigidbody carRealRigidbody;
    private const float mineCoolDownCount = 1.5f;
    private float mineCoolDownCounter;

    // Use this for initialization
	void Start () {
        //获得刚体
        mineCoolDownCounter = mineCoolDownCount;
        physics = GameObject.Find(gameObject.name+" physics");
        if(physics)
        {
            carRealRigidbody = physics.GetComponent<Rigidbody>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        mineCoolDownCounter -= Time.deltaTime;
        //fire
        if (Input.GetKeyUp (KeyCode.LeftControl))
        {
            if(mineCoolDownCounter <=0 )
            {
                mineCoolDownCounter = mineCoolDownCount;
                StartCoroutine(FireMine(3));
            }
        }
	}

    public IEnumerator FireMine(int time)
    {
        while (time > 0)
        {
            time--;
            ShootMineOut();
            yield return new WaitForSeconds(0.13f);
        }
    }

    private void ShootMineOut()
    {
        GameObject biubiu = Instantiate(
                fireMe,
                carBody.transform.position + new Vector3(0,2f,0),
                carBody.transform.rotation
        ) as GameObject;

        //方向随机性
        float randomY = Random.Range(-25f,25f);
        biubiu.transform.Rotate(Vector3.up*randomY);
        //calRotation = Quaternion.Euler(0f,randomY,0f);
        CanBeShootOut canBeShootOut = biubiu.GetComponent<CanBeShootOut>();
        canBeShootOut.Fire(carRealRigidbody.velocity.magnitude);
    }
}
