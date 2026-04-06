using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace Modules.Loading
{
    public class LoadingController : LocomotionProvider
    {
        [Header("Buttons")]
        [SerializeField]
        private Button loadingButton;

        [SerializeField]
        private Button cancelButton;

        [Header("Player")]
        [SerializeField]
        private Transform player;

        [Header("Loading")]
        [SerializeField]
        private Spawner spawner;

        [SerializeField]
        private GameObject loadingRoot;

        [SerializeField]
        private TextMeshProUGUI loadingText;

        [SerializeField]
        private Transform defaultPlayerPivot;

        [SerializeField]
        private Transform loadingPlayerPivot;

        private CancellationTokenSource _cts;

        protected override void Awake()
        {
            base.Awake();

            loadingButton.onClick.AddListener(Load);
            cancelButton.onClick.AddListener(Cancel);

            loadingRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            loadingButton.onClick.RemoveListener(Load);
            cancelButton.onClick.RemoveListener(Cancel);

            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async void Load()
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = new CancellationTokenSource();
                await LoadingProcess(_cts.Token);
            }
            catch
            {
                TryStartLocomotionImmediately();
                await Task.Delay(TimeSpan.FromSeconds(1));
                player.SetPositionAndRotation(
                    defaultPlayerPivot.position,
                    defaultPlayerPivot.rotation
                );
                TryEndLocomotion();
                spawner.Clear();
                loadingRoot.SetActive(false);
            }
        }

        private async Task LoadingProcess(CancellationToken ct)
        {
            loadingRoot.SetActive(true);
            spawner.Spawn();

            TryStartLocomotionImmediately();
            await Task.Delay(TimeSpan.FromSeconds(1), ct);
            player.SetPositionAndRotation(loadingPlayerPivot.position, loadingPlayerPivot.rotation);
            TryEndLocomotion();

            var elapsed = 0f;
            const float target = 100f;
            while (elapsed < target && !ct.IsCancellationRequested)
            {
                elapsed += Time.deltaTime;
                loadingText.text = $"Loading {Mathf.RoundToInt(elapsed * 10)}%";
                await Task.Yield();
            }

            TryStartLocomotionImmediately();
            await Task.Delay(TimeSpan.FromSeconds(1), ct);
            player.SetPositionAndRotation(defaultPlayerPivot.position, defaultPlayerPivot.rotation);
            TryEndLocomotion();
            spawner.Clear();
            loadingRoot.SetActive(false);
        }

        private void Cancel()
        {
            _cts?.Cancel();
        }
    }
}
