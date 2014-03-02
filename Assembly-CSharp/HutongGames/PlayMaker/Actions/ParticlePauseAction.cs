﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Effects), Tooltip("Pause a Particle System.")]
    public class ParticlePauseAction : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault m_GameObject;
        [Tooltip("Run this action on all child objects' Particle Systems.")]
        public FsmBool m_IncludeChildren;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
                if (component == null)
                {
                    Debug.LogWarning(string.Format("ParticlePauseAction.OnEnter() - GameObject {0} has no ParticleSystem component", ownerDefaultTarget));
                    base.Finish();
                }
                else
                {
                    component.Pause(this.m_IncludeChildren.Value);
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.m_GameObject = null;
            this.m_IncludeChildren = 0;
        }
    }
}

