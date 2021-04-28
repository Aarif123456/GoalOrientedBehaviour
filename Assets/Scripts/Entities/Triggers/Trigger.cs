using UnityEngine;

namespace Entities.Triggers {
    public abstract class Trigger : Entity {
        private bool _isActive;

        public bool IsActive {
            get => _isActive;

            protected set {
                _isActive = value;
                GetComponent<Renderer>().enabled = value;
            }
        }

        public Agent TriggeringAgent { get; protected set; }

        public override void Awake(){
            base.Awake();
            IsActive = true;
        }
    }
}