using UnityEngine;

namespace SlotTemplate {

    [DisallowMultipleComponent]
    public class LineDisplayController : MonoBehaviour {

        LineRenderer _lineRendererStored;
        LineRenderer _attachedLineRenderer {
            get {
                if (_lineRendererStored == null) {
                    _lineRendererStored = GetComponent<LineRenderer>();
                }
                return _lineRendererStored;
            }
        }


        public void DrawLineShape (Vector3[] positions) {

            _attachedLineRenderer.positionCount = positions.Length;

            for (int i = 0 ; i < positions.Length ; i++) {
                _attachedLineRenderer.SetPosition(i, positions[i]);
            }
        }

    }
}
