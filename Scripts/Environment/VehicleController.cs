using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{

    public void ReceiveDamage(MessageReceiveDamage message)
    {
        GameManager.instance.ShakeCamera();
    }

    public void DamagedEffect(MessageAttackEffect message)
    {
        Vector3 contactProximatePosition = (message.target + message.origin) / 2f;
        GameManager.instance.effectDisplayController.PlayBlastEffect(contactProximatePosition);
    }
}
