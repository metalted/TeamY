using Shpleeble;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamYClient.Game
{
    public class LocalPlayerTracker
    {
        public event Action<LocalPlayerSnapshot> OnSnapshot;

        private LocalPlayerSnapshot current;
        private LocalPlayerSnapshot lastSent;

        private readonly float positionThreshold = 0.02f;
        private readonly float rotationThreshold = 0.5f;

        public void SetTransform(Vector3 position, Quaternion rotation)
        {
            current.Position = position;
            current.EulerRotation = rotation.eulerAngles;

            TryEmit();
        }

        public void SetMode(CharacterMode mode)
        {
            if (current.Mode == mode)
                return;

            current.Mode = mode;
            Emit();
        }

        // Emission logic
        private void TryEmit()
        {
            if (!HasMeaningfulDelta(lastSent, current))
                return;

            Emit();
        }

        private void Emit()
        {
            lastSent = current;
            OnSnapshot?.Invoke(current);
        }

        private bool HasMeaningfulDelta(LocalPlayerSnapshot a, LocalPlayerSnapshot b)
        {
            if (Vector3.Distance(a.Position, b.Position) > positionThreshold)
                return true;

            if (Vector3.Distance(a.EulerRotation, b.EulerRotation) > rotationThreshold)
                return true;

            return false;
        }
    }
}
