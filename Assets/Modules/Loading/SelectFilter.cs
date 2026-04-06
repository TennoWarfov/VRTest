using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Modules.Loading
{
    public class SelectFilter : MonoBehaviour, IXRSelectFilter
    {
        public bool canProcess { get; private set; }

        private XRBaseInteractable _interactable;
        private CancellationTokenSource _cts;
        private XRSocketInteractor _currentSocket;

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return true;
        }

        private void Awake() => _interactable = GetComponent<XRBaseInteractable>();

        private void OnEnable()
        {
            _interactable.selectEntered.AddListener(SelectEntered);
            _interactable.selectExited.AddListener(SelectExited);
        }

        private void OnDisable()
        {
            _interactable.selectEntered.RemoveListener(SelectEntered);
            _interactable.selectExited.RemoveListener(SelectExited);
            _cts?.Cancel();
        }

        private void SelectEntered(SelectEnterEventArgs arg0)
        {
            if (arg0.interactorObject is XRSocketInteractor interactor)
            {
                _currentSocket = interactor;
                var myCollider = GetComponent<Collider>();
                var socketCollider = interactor.parentInteractable.colliders[0];
                Physics.IgnoreCollision(myCollider, socketCollider, true);
                return;
            }

            _cts?.Cancel();
        }

        private async void SelectExited(SelectExitEventArgs arg0)
        {
            try
            {
                if (
                    arg0.interactorObject is XRSocketInteractor interactor
                    && interactor == _currentSocket
                )
                {
                    var myCollider = GetComponent<Collider>();
                    var socketCollider = interactor.parentInteractable.colliders[0];
                    Physics.IgnoreCollision(myCollider, socketCollider, false);
                    _currentSocket = null;
                    return;
                }

                _cts?.Cancel();
                _cts?.Dispose();
                _cts = new CancellationTokenSource();
                await DeselectProcess(_cts.Token);
            }
            catch (Exception)
            {
                canProcess = false;
            }
        }

        private async Task DeselectProcess(CancellationToken ctsToken)
        {
            canProcess = true;
            await Task.Delay(TimeSpan.FromSeconds(.5f), ctsToken);
            canProcess = false;
        }
    }
}
