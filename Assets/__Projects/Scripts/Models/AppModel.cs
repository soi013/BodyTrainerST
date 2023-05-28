using System;
using System.Collections.Generic;
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

        private readonly IReadOnlyList<TrainingStage> stages = CreateStages();

        private static IReadOnlyList<TrainingStage> CreateStages() =>
            new TrainingStage[]
            {
                new() {
                    Explain = "暗くなったら、左右にまっすぐ手を伸ばして水平にしてください。",
                    TargetLeft = new(0,0,float.NaN),
                    TargetRight = new(0,0,float.NaN),
                },
                new() {
                    Explain = "次は真上に手を上げてください",
                    TargetLeft = new(90,float.NaN,float.NaN),
                    TargetRight = new(90,float.NaN,float.NaN),
                },
            };

        private readonly ReactiveProperty<int> currentStageIndex = new(0);
        private readonly IReadOnlyReactiveProperty<TrainingStage> currentStage;

        private ResultZone resultZone;


        public AppModel()
        {
            Debug.Log($"{this.GetType().Name} ctor 00");

            currentStage = currentStageIndex
                .Select(i => stages[i])
                .ToReadOnlyReactiveProperty();

            resultZone = new(State, currentStage, resultAngles);

            MainText = new(State, currentStage, handAngles, resultZone.CurrentResult);

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

            State
                .Pairwise()
                .Where(p => p.Previous == AppState.Result && p.Current == AppState.Explain)
                .Subscribe(p => ChangeStage());


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
                //±180°の表示に変更
                new Vector3(
                    Normalize180(angle.x * -1), //X角度(天地方向）は俯角がマイナスになるよう反転
                    Normalize180(angle.y),
                    Normalize180(angle.z));

        private static float Normalize180(float f) =>
            Mathf.Repeat(f + 180, 360) - 180;

        internal void NextMode()
        {
            if (state.Value is AppState.Explain)
            {
                state.Value = AppState.Dark;
            }
            else if (state.Value is AppState.Result)
            {
                state.Value = AppState.Explain;
            }
        }

        private void ChangeStage()
        {
            currentStageIndex.Value++;
        }

        private void ShowResult()
        {
            state.Value = AppState.Result;
        }
    }
}
