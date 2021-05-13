using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incentive
{
    public Incentive(GameObject m_incentive, Vector3 m_props)
    {
        incentive = m_incentive;

        smell = m_props.x;
        sound = m_props.y;
        visual = m_props.z;
    }

    public GameObject incentive = null;
    public float smell = 0.0f;
    public float sound = 0.0f;
    public float visual = 0.0f;
}
