using UniRx;
using UnityEngine;

namespace BodyTrainerST.Models
{
    internal class ResultZone
    {
        private IReadOnlyReactiveProperty<AppState> state;
        private IReadOnlyReactiveProperty<TrainingStage> currentStage;
        private ReactiveProperty<(Vector3 l, Vector3 r)> resultAngles;
        public IReadOnlyReactiveProperty<string> CurrentResult { get; }

        public ResultZone(IReadOnlyReactiveProperty<AppState> state, IReadOnlyReactiveProperty<TrainingStage> currentStage, ReactiveProperty<(Vector3 l, Vector3 r)> resultAngles)
        {
            this.state = state;
            this.currentStage = currentStage;
            this.resultAngles = resultAngles;


            CurrentResult = resultAngles
                .Select(h => $"Results：\nLeft = {ToResultAngleText(h.l)}, Right = {ToResultAngleText(h.r)}")
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