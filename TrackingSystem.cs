using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSystem : MonoBehaviour
{
    GameObject m_target;
    Vector3 m_lastKnownPos;

    void Update()
    {
        if (m_target)
        {
            if (m_lastKnownPos != m_target.transform.position)
            {
                m_lastKnownPos = m_target.transform.position;
            }
        }
    }

    void SetTarget(GameObject target)
    {
        if (!target)
        {
            m_target = target;
        }
    }
}
