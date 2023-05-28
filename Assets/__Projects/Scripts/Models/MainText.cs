using System;
using UniRx;
using UnityEngine;

namespace BodyTrainerST.Models
{
    public class MainText
    {
        public IReadOnlyReactiveProperty<string> Text { get; }

        public MainText(IReadOnlyReactiveProperty<AppState> state,
            IObservable<TrainingStage> stage,
            IObservable<(Vector3 l, Vector3 r)> handAngles,
            IObservable<(Vector3 l, Vector3 r)> resultAngles)
        {
            var handAngleText = handAngles
                .Select(h => $"Now：\nLeft = {h.l.x:000.0}, Right = {h.r.x:000.0}");

            var resultAngleText = resultAngles
                .Select(h => $"Results：\nLeft = {ToResultAngleText(h.l)}, Right = {ToResultAngleText(h.r)}")
                .ToReadOnlyReactiveProperty();

            Text = Observable.CombineLatest(state, stage, resultAngleText, handAngleText,
                (s, g, r, h) => ConcateParamerterText(s, g, r) + '\n' + h)
               //.Do(t => Debug.Log(t))
               .ToReadOnlyReactiveProperty();
        }

        private static string ConcateParamerterText(AppState state, TrainingStage stage, string resultText) =>
            state switch
            {
                AppState.Explain => stage.Explain,
                AppState.Result => resultText,
                _ => "...処理中..."
            };

        //(state == AppState.Explain ? stage.Explain : resultText);

        private string ToResultAngleText(Vector3 angle)
        {
            string resultComment = Mathf.Abs(angle.x) switch
            {
                > 5 => "おしい",
                > 2 => "もうちょい",
                > 1 => "すごい",
                _ => "ロボット級"
            };

            return $"{resultComment} ({angle.x:0.0})";
        }
    }
}