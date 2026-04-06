using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Modules.Loading
{
    public class ChildInteractabilityController : MonoBehaviour
    {
        [SerializeField]
        private XRSocketInteractor socket;

        private XRGrabInteractable _grab;
        private XRBaseInteractable _selectedInteractable;

        private void Awake() => _grab = GetComponent<XRGrabInteractable>();

        private void OnEnable()
        {
            _grab.selectEntered.AddListener(GrabSelectEntered);
            _grab.selectExited.AddListener(GrabSelectExited);
            socket.selectEntered.AddListener(SocketSelectEntered);
            socket.selectExited.AddListener(SocketSelectExited);
        }

        private void OnDisable()
        {
            _grab.selectEntered.RemoveListener(GrabSelectEntered);
            _grab.selectExited.RemoveListener(GrabSelectExited);
            socket.selectEntered.RemoveListener(SocketSelectEntered);
            socket.selectExited.RemoveListener(SocketSelectExited);
        }

        private void GrabSelectEntered(SelectEnterEventArgs arg0)
        {
            if (_selectedInteractable)
                _selectedInteractable.transform.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        private void GrabSelectExited(SelectExitEventArgs arg0)
        {
            if (_selectedInteractable)
                _selectedInteractable.transform.gameObject.layer = LayerMask.NameToLayer(
                    "IgnoreGrab"
                );
        }

        private void SocketSelectEntered(SelectEnterEventArgs arg0) =>
            _selectedInteractable = arg0.interactableObject as XRBaseInteractable;

        private void SocketSelectExited(SelectExitEventArgs arg0) => _selectedInteractable = null;

        private void OnValidate()
        {
            if (!socket)
                socket = GetComponentInChildren<XRSocketInteractor>();
        }
    }
}
