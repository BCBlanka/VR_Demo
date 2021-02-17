//-----------------------------------------------------------------------
// <copyright file="TwoFingerDragGesture.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

// Modifications copyright � 2020 Unity Technologies ApS

#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION

using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Gesture for when the user performs a two finger vertical swipe motion on the touch screen.
    /// </summary>
    public class TwoFingerDragGesture : Gesture<TwoFingerDragGesture>
    {
        /// <summary>
        /// Constructs a two finger drag gesture.
        /// </summary>
        /// <param name="recognizer">The gesture recognizer.</param>
        /// <param name="touch1">The first touch that started this gesture.</param>
        /// <param name="touch2">The second touch that started this gesture.</param>
        public TwoFingerDragGesture(
            TwoFingerDragGestureRecognizer recognizer, Touch touch1, Touch touch2) :
                base(recognizer)
        {
            FingerId1 = touch1.fingerId;
            StartPosition1 = touch1.position;
            FingerId2 = touch2.fingerId;
            StartPosition2 = touch2.position;
            Position = (StartPosition1 + StartPosition2) / 2;
        }

        /// <summary>
        /// (Read Only) The id of the first finger used in this gesture.
        /// </summary>
        public int FingerId1 { get; }

        /// <summary>
        /// (Read Only) The id of the second finger used in this gesture.
        /// </summary>
        public int FingerId2 { get; }

        /// <summary>
        /// (Read Only) The screen position of the first finger where the gesture started.
        /// </summary>
        public Vector2 StartPosition1 { get; }

        /// <summary>
        /// (Read Only) The screen position of the second finger where the gesture started.
        /// </summary>
        public Vector2 StartPosition2 { get; }

        /// <summary>
        /// (Read Only) The current screen position of the gesture.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// (Read Only) The delta screen position of the gesture.
        /// </summary>
        public Vector2 Delta { get; private set; }

        /// <inheritdoc />
        protected internal override bool CanStart()
        {
            if (GestureTouchesUtility.IsFingerIdRetained(FingerId1) ||
                GestureTouchesUtility.IsFingerIdRetained(FingerId2))
            {
                Cancel();
                return false;
            }

            Touch touch1, touch2;
            bool foundTouches = GestureTouchesUtility.TryFindTouch(FingerId1, out touch1);
            foundTouches =
                GestureTouchesUtility.TryFindTouch(FingerId2, out touch2) && foundTouches;

            if (!foundTouches)
            {
                Cancel();
                return false;
            }

            // Check that at least one finger is moving.
            if (touch1.deltaPosition == Vector2.zero && touch2.deltaPosition == Vector2.zero)
            {
                return false;
            }

            var pos1 = touch1.position;
            var diff1 = (pos1 - StartPosition1).magnitude;
            var pos2 = touch2.position;
            var diff2 = (pos2 - StartPosition2).magnitude;
            var slopInches = (m_Recognizer as TwoFingerDragGestureRecognizer).m_SlopInches;
            if (GestureTouchesUtility.PixelsToInches(diff1) < slopInches ||
                GestureTouchesUtility.PixelsToInches(diff2) < slopInches)
            {
                return false;
            }

            var recognizer = m_Recognizer as TwoFingerDragGestureRecognizer;

            // Check both fingers move in the same direction.
            var dot = Vector3.Dot(touch1.deltaPosition.normalized, touch2.deltaPosition.normalized);
            return !(dot < Mathf.Cos(recognizer.m_AngleThresholdRadians));
        }

        /// <inheritdoc />
        protected internal override void OnStart()
        {
            GestureTouchesUtility.LockFingerId(FingerId1);
            GestureTouchesUtility.LockFingerId(FingerId2);

            if (GestureTouchesUtility.RaycastFromCamera(StartPosition1, out var hit1))
            {
                var gameObject = hit1.transform.gameObject;
                var interactableObject = gameObject.GetComponentInParent<ARBaseGestureInteractable>();
                if (interactableObject != null)
                    TargetObject = interactableObject.gameObject;
            }
            else if (GestureTouchesUtility.RaycastFromCamera(StartPosition2, out var hit2))
            {
                var gameObject = hit2.transform.gameObject;
                var interactableObject = gameObject.GetComponentInParent<ARBaseGestureInteractable>();
                if (interactableObject != null)
                    TargetObject = interactableObject.gameObject;
            }

            GestureTouchesUtility.TryFindTouch(FingerId1, out var touch1);
            GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2);
            Position = (touch1.position + touch2.position) / 2;
        }

        /// <inheritdoc />
        protected internal override bool UpdateGesture()
        {
            var foundTouches = GestureTouchesUtility.TryFindTouch(FingerId1, out var touch1);
            foundTouches =
                GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2) && foundTouches;

            if (!foundTouches)
            {
                Cancel();
                return false;
            }

            if (touch1.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Canceled)
            {
                Cancel();
                return false;
            }

            if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
            {
                Complete();
                return false;
            }

            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                Delta = ((touch1.position + touch2.position) / 2) - Position;
                Position = (touch1.position + touch2.position) / 2;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected internal override void OnCancel()
        {
        }

        /// <inheritdoc />
        protected internal override void OnFinish()
        {
            GestureTouchesUtility.ReleaseFingerId(FingerId1);
            GestureTouchesUtility.ReleaseFingerId(FingerId2);
        }
    }
}

#endif
