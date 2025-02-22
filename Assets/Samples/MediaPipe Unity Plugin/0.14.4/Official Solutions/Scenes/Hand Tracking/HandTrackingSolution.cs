// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity.CoordinateSystem;
using UnityEngine.UI;

namespace Mediapipe.Unity.Sample.HandTracking
{
  public class HandTrackingSolution : ImageSourceSolution<HandTrackingGraph>
  {
    [SerializeField] private MultiHandLandmarkListAnnotationController _handLandmarksAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _handRectsFromLandmarksAnnotationController;

        [SerializeField] HandInstrument _HandInstrument;
    public HandTrackingGraph.ModelComplexity modelComplexity
    {
      get => graphRunner.modelComplexity;
      set => graphRunner.modelComplexity = value;
    }

    public int maxNumHands
    {
      get => graphRunner.maxNumHands;
      set => graphRunner.maxNumHands = value;
    }

    public float minDetectionConfidence
    {
      get => graphRunner.minDetectionConfidence;
      set => graphRunner.minDetectionConfidence = value;
    }

    public float minTrackingConfidence
    {
      get => graphRunner.minTrackingConfidence;
      set => graphRunner.minTrackingConfidence = value;
    }

    protected override void OnStartRun()
    {
      if (!runningMode.IsSynchronous())
      {
        graphRunner.OnHandLandmarksOutput += OnHandLandmarksOutput;
        // TODO: render HandWorldLandmarks annotations
        graphRunner.OnHandRectsFromLandmarksOutput += OnHandRectsFromLandmarksOutput;
        graphRunner.OnHandednessOutput += OnHandednessOutput;
      }

      var imageSource = ImageSourceProvider.ImageSource;
      SetupAnnotationController(_handLandmarksAnnotationController, imageSource, true);
      SetupAnnotationController(_handRectsFromLandmarksAnnotationController, imageSource, true);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      var task = graphRunner.WaitNext();
      yield return new WaitUntil(() => task.IsCompleted);

      var result = task.Result;
      _handLandmarksAnnotationController.DrawNow(result.handLandmarks, result.handedness);
      // TODO: render HandWorldLandmarks annotations
      _handRectsFromLandmarksAnnotationController.DrawNow(result.handRectsFromLandmarks);
    }
    private void OnHandLandmarksOutput(object stream, OutputStream<List<NormalizedLandmarkList>>.OutputEventArgs eventArgs)
    {
      var packet = eventArgs.packet;
      var value = packet == null ? default : packet.Get(NormalizedLandmarkList.Parser);
      _handLandmarksAnnotationController.DrawLater(value);
    }

    private void OnHandRectsFromLandmarksOutput(object stream, OutputStream<List<NormalizedRect>>.OutputEventArgs eventArgs)
    {
      var packet = eventArgs.packet;
      var value = packet == null ? default : packet.Get(NormalizedRect.Parser);
      _handRectsFromLandmarksAnnotationController.DrawLater(value);
      _HandInstrument.updateListRects(value);
    }

    private void OnHandednessOutput(object stream, OutputStream<List<ClassificationList>>.OutputEventArgs eventArgs)
    {
      var packet = eventArgs.packet;
      var value = packet == null ? default : packet.Get(ClassificationList.Parser);
      _handLandmarksAnnotationController.DrawLater(value);
    }
  }
}
