using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSoundManager : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private bool m_isLeftStepping;
    [SerializeField] private bool m_isRightStepping;

    [SerializeField] private float m_leftUp;
    [SerializeField] private float m_rightUp;

    public Transform leftFoot;  // managed by NetworkPlayerManager
    public Transform rightFoot; // managed by NetworkPlayerManager

    [Header("Setting")]
    [SerializeField] private float m_stepHeight;

    [Header("Reference")]
    [SerializeField] private AudioClip m_footstepSound;

    public void Update()
    {
        if (leftFoot == null)
            return;

        if (rightFoot == null)
            return;

        m_leftUp = Vector3.Dot(leftFoot.position - transform.position, transform.up);
        m_rightUp = Vector3.Dot(rightFoot.position - transform.position, transform.up);

        if (m_leftUp <= m_stepHeight)
        {
            if (!m_isLeftStepping)
            {
                m_isLeftStepping = true;

                if (m_footstepSound != null)
                    AudioSource.PlayClipAtPoint(m_footstepSound, leftFoot.position);
            }
        }
        else
        {
            m_isLeftStepping = false;
        }

        if (m_rightUp <= m_stepHeight)
        {
            if (!m_isRightStepping)
            {
                m_isRightStepping = true;

                if (m_footstepSound != null)
                    AudioSource.PlayClipAtPoint(m_footstepSound, rightFoot.position);
            }
        }
        else
        {
            m_isRightStepping = false;
        }
    }
}
