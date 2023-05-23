using System;
using UniRx;
using UnityEngine;

namespace BodyTrainerST.Models
{
    public enum AppState
    {
        Initializing,
        Explain,
        Dark,
        Result,
    }

    public class AppModel
    {
        public IReadOnlyReactiveProperty<AppState> State => state;
        private readonly ReactiveProperty<AppState> state = new(AppState.Initializing);

        public IReadOnlyReactiveProperty<float> PlayTime { get; }
        public ReadOnlyReactiveProperty<bool> IsEnabledFade { get; }
        public MainText MainText { get; }

        private readonly ReadOnlyReactiveProperty<float> startPlayTime;

        private readonly ReactiveProperty<(Vector3 l, Vector3 r)> handAngles = new();
        private readonly ReactiveProperty<(Vector3 l, Vector3 r)> resultAngles = new();

        public AppModel()
        {
            Debug.Log($"{this.GetType().Name} ctor 00");

            MainText = new(State, handAngles, resultAngles);

            State
                .Pairwise()
                .Subscribe(p =>
                    Debug.Log($"{this.GetType().Name} StateChange {p.Previous} -> {p.Current}"));

            startPlayTime = State
                   .Pairwise()
                   .Where(p => p.Previous == AppState.Initializing && p.Current == AppState.Explain)
                   .Select(p => Time.realtimeSinceStartup)
                   .ToReadOnlyReactiveProperty(0f);

            PlayTime = Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Where(_ => State.Value != AppState.Initializing)
                .Select(_ => Time.realtimeSinceStartup)
                .Select(t => t - startPlayTime.Value)
                .ToReadOnlyReactiveProperty();

            State
                .Where(x => x == AppState.Dark)
                .Delay(TimeSpan.FromSeconds(5))
                .Where(x => x == AppState.Dark)
                .Subscribe(_ => ShowResult());


            IsEnabledFade = State
                .Select(x => x == AppState.Dark)
                .ToReadOnlyReactiveProperty();

            State
                .Where(x => x == AppState.Result)
                .Subscribe(_ => resultAngles.Value = handAngles.Value);
        }


        public void Initialize()
        {
            Debug.Log($"{this.GetType().Name} {nameof(Initialize)} 00");

            state.Value = AppState.Explain;
        }

        internal void SetHandRotations(IObservable<Vector3> leftHandRx, IObservable<Vector3> rightHandRx)
        {
            Observable.CombineLatest(
                leftHandRx.Select(x => ChangeAngle(x)),
                rightHandRx.Select(x => ChangeAngle(x)),
                    (l, r) => (l, r))
                .Subscribe(a =>
                    handAngles.Value = (a.l, a.r));
        }
        private static Vector3 ChangeAngle(Vector3 angle) =>
                //�}180���̕\���ɕύX
                new Vector3(
                    Normalize180(angle.x * -1), //X�p�x(�V�n�����j�͘�p���}�C�i�X�ɂȂ�悤���]
                    Normalize180(angle.y),
                    Normalize180(angle.z));

        private static float Normalize180(float f) =>
            Mathf.Repeat(f + 180, 360) - 180;

        internal void NextMode()
        {
            if (state.Value is AppState.Explain or AppState.Result)
                state.Value = AppState.Dark;
        }

        private void ShowResult()
        {
            state.Value = AppState.Result;
        }
    }
}