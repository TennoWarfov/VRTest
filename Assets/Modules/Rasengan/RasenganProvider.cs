using UnityEngine;

namespace Modules.Rasengan
{
    public class RasenganProvider : MonoBehaviour
    {
        [SerializeField]
        private Rasengan spellPrefab;

        [SerializeField]
        private Transform spellPivot;

        private Rasengan _currentSpell;
        private Vector3 _pivotPreviousPos;
        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (_currentSpell)
            {
                _currentSpell.transform.position = spellPivot.position;

                var currentPos = spellPivot.position;
                _velocity = (currentPos - _pivotPreviousPos) / Time.deltaTime;
                if (_velocity.magnitude > 10f)
                {
                    Debug.Log(_velocity.magnitude);
                    ThrowSpell();
                }

                _pivotPreviousPos = currentPos;
            }
        }

        public void CreateSpell()
        {
            if (_currentSpell)
                return;

            _currentSpell = Instantiate(spellPrefab, spellPivot);
            Debug.LogError($"Spell created: {_currentSpell}");
        }

        public void ThrowSpell()
        {
            _currentSpell.transform.SetParent(null);
            _currentSpell.rb.isKinematic = false;
            _currentSpell.rb.linearVelocity = _velocity;
            _currentSpell = null;
            Debug.LogError("Spell thrown");
        }
    }
}
