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
            IObservable<string> resultAngleText)
        {
            var handAngleText = handAngles
                .Select(h =>
@$"Now: 
Left = {h.l.x:000.0}, {h.l.y:000.0}, {h.l.z:000.0}
Right = {h.r.x:000.0}, {h.r.y:000.0}, {h.r.z:000.0}");

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
    }
}