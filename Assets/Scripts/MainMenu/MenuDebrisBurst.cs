using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace MainMenu
{
    /// Background set-dressing for the menu: shelves and loose items that topple toward camera
    /// as the kart drifts past. Purely cosmetic — no physics simulation, no gameplay coupling,
    /// so the menu costs nothing to keep running behind the UI.
    public class MenuDebrisBurst : MonoBehaviour
    {
        [System.Serializable]
        private struct DebrisPiece
        {
            public Transform target;
            [Tooltip("Seconds after the burst begins before this piece reacts.")]
            public float delay;
            [Tooltip("World-space displacement applied over the burst.")]
            public Vector3 travel;
            public Vector3 spin;
        }

        [Header("Pieces")]
        [SerializeField] private List<DebrisPiece> pieces = new List<DebrisPiece>();

        [Header("Timing")]
        [SerializeField] private float burstDuration = 1.6f;
        [SerializeField] private Ease travelEase = Ease.OutCubic;
        [Tooltip("Replays the burst on a timer. Off by default — a repeating topple reads as a " +
                 "glitch once the kart has settled, since nothing on screen causes it.")]
        [SerializeField] private bool loop = false;
        [SerializeField] private float loopInterval = 6f;
        [Header("Settle")]
        [Tooltip("Gentle drift applied after the burst so the scene never looks frozen.")]
        [SerializeField] private float settleSway = 1.4f;
        [SerializeField] private float settleDuration = 3.2f;

        private readonly List<Vector3> _restPositions = new List<Vector3>();
        private readonly List<Quaternion> _restRotations = new List<Quaternion>();
        private Sequence _sequence;

        private void Awake()
        {
            foreach (var piece in pieces)
            {
                _restPositions.Add(piece.target != null ? piece.target.position : Vector3.zero);
                _restRotations.Add(piece.target != null ? piece.target.rotation : Quaternion.identity);
            }
        }

        private void OnDestroy() => _sequence?.Kill();

        public void Burst()
        {
            _sequence?.Kill();
            ResetPieces();

            _sequence = DOTween.Sequence();

            for (int i = 0; i < pieces.Count; i++)
            {
                var piece = pieces[i];
                if (piece.target == null) continue;

                _sequence.Insert(piece.delay,
                    piece.target.DOMove(_restPositions[i] + piece.travel, burstDuration).SetEase(travelEase));
                _sequence.Insert(piece.delay,
                    piece.target.DORotate(piece.spin, burstDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));
            }

            if (loop)
            {
                _sequence.AppendInterval(loopInterval);
                _sequence.OnComplete(Burst);
            }
            else
            {
                _sequence.OnComplete(Settle);
            }
        }

        /// Leaves the wreckage where it landed, breathing slightly, instead of snapping back.
        private void Settle()
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                var piece = pieces[i];
                if (piece.target == null) continue;

                float dir = (i % 2 == 0) ? 1f : -1f;
                piece.target
                    .DORotate(new Vector3(0f, 0f, settleSway * dir), settleDuration, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }

        public void Stop()
        {
            _sequence?.Kill();
        }

        private void ResetPieces()
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                var piece = pieces[i];
                if (piece.target == null) continue;

                piece.target.position = _restPositions[i];
                piece.target.rotation = _restRotations[i];
            }
        }
    }
}
