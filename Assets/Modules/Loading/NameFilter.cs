using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Modules.Loading
{
    public class NameFilter : MonoBehaviour, IXRSelectFilter
    {
        public bool canProcess => isActiveAndEnabled;
        private string id => idFilter;

        [SerializeField]
        private string idFilter;

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            if (interactor is not XRSocketInteractor)
                return true;

            interactor.transform.TryGetComponent(out NameFilter interactorNameFilter);
            interactable.transform.TryGetComponent(out NameFilter interactableNameFilter);
            if (!interactorNameFilter || !interactableNameFilter)
                return false;

            if (!interactorNameFilter.canProcess || !interactableNameFilter.canProcess)
                return false;

            return interactorNameFilter.id == interactableNameFilter.id;
        }
    }
}
