

using System;
using UnityEngine;

namespace Assets.Scripts.Ingame.Entities.util {
    public class util_variableTransition {
        public float var;

        private bool _transition;

        private float _originalVar;
        private float _targetVar;
        private float _startTime;

        private float _speed;
        private Action _onDone;

        public util_variableTransition(float initial) {
            this.var = initial;
        }

        public void transition(float newValue, float speed, Action onDone = null) {
            this._originalVar = this.var;
            this._targetVar = newValue;

            this._speed = speed;
            this._startTime = Time.time;

            this._transition = true;
            this._onDone = onDone;
        }

        public float getVar() {
            if (this._transition) {
                float distCovered = (Time.time - this._startTime) * this._speed;
                this.var = Mathf.Lerp(this._originalVar, this._targetVar, distCovered);

                if (distCovered >= 1f) {
                    this._transition = false;
                    this.var = this._targetVar;
                    if (this._onDone != null) this._onDone();
                }
            }

            return this.var;
        }
    }
}
