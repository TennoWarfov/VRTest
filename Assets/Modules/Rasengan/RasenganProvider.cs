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

        public void CreateSpell()
        {
            if (_currentSpell)
                return;

            _currentSpell = Instantiate(spellPrefab, spellPivot);
        }

        public void ThrowSpell()
        {
            _currentSpell = null;
        }
    }
}
