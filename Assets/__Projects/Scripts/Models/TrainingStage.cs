using UnityEngine;

namespace BodyTrainerST.Models
{
    public record TrainingStage
    {
        public string Explain;

        public Vector3 TargetLeft;
        public Vector3 TargetRight;
    }
}