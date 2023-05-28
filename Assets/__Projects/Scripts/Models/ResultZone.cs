using System.Linq;
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
                .Select(h => $"Results：\nLeft = {ToResultAngleText(h.l, currentStage.Value.TargetLeft)}, Right = {ToResultAngleText(h.r, currentStage.Value.TargetRight)}")
                .ToReadOnlyReactiveProperty();
        }


        private string ToResultAngleText(Vector3 angle, Vector3 target)
        {
            var diffV = angle - target;

            float diff = new[] { diffV.x, diffV.y, diffV.z }
            .Where(a => !float.IsNaN(a))
            .Select(a => Mathf.Abs(a))
            .Average();

            string resultComment = diff switch
            {
                > 20 => "いまいち",
                > 5 => "おしい",
                > 2 => "もうちょい",
                > 1 => "すごい",
                _ => "ロボット級"
            };

            return $"{resultComment} ({angle.x:0.0}, {angle.y:0.0})";
        }
    }
}