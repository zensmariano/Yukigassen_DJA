using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_menu : MonoBehaviour {

    float Shake;
    float shakeAmount = 0.1f;
    Vector3 curentPosition;
    bool doShake = false;
    int a;

    IEnumerator CreateNumber()
    {
        a = Random.Range(3, 9);
        yield return new WaitForSeconds(5);
    }

    public void Update()
    {
        if (a < 9)

            doShake = false;

        if (a == 9)

            doShake = true;

        /*if (Random.value <=0.0000001)
        doShake= false;


        if (Random.value > 0.0000001)
        doShake = true;
        */

        if (doShake)
        {
            curentPosition = transform.position;
            Shake = Random.value;
            Debug.Log(Shake);

            if (Shake < 0.5) {

                curentPosition = new Vector3(curentPosition.x, curentPosition.y + shakeAmount, curentPosition.z);
            }
            if (Shake > 0.5)

                curentPosition = new Vector3(curentPosition.x, curentPosition.y - shakeAmount, curentPosition.z);

            transform.position = curentPosition;
        }
    }
}
