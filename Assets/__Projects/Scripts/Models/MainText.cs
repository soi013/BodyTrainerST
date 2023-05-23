using System;
using UniRx;
using UnityEngine;

namespace BodyTrainerST.Models
{
    public class MainText
    {
        public IReadOnlyReactiveProperty<string> Text { get; }

        public MainText(IReadOnlyReactiveProperty<AppState> state,
            IObservable<(Vector3 l, Vector3 r)> handAngles,
            IObservable<(Vector3 l, Vector3 r)> resultAngles)
        {
            var handAngleText = handAngles
                .Select(h => $"Left = {h.l.x:000.0}, Right = {h.r.x:000.0}");

            var resultAngleText = resultAngles
                .Select(h => $"Left = {ToResultAngleText(h.l)}, Right = {ToResultAngleText(h.r)}")
                .ToReadOnlyReactiveProperty();

            string explainText = $"手の角度が表示されています。\n暗くなったら、左右にまっすぐ手を伸ばして水平にしてください。";

            var stateText = state
                .Select(s =>
                {
                    return s == AppState.Explain
                                            ? explainText
                                            : $"結果\n{resultAngleText.Value}";
                });

            Text = Observable.CombineLatest(state, handAngleText, resultAngleText,
                (s, h, r) => (s == AppState.Explain ? explainText : r) + '\n' + h)
               //.Do(t => Debug.Log(t))
               .ToReadOnlyReactiveProperty();
        }

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