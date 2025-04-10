# other examples
- remote desktop example >>
https://github.com/frozenmistadventure/fmetp_tutorial_questvr_stream

# fmetp_tutorial_questvr_passthrough
This example project demonstrates the low latency passthrough live stream (In-Game camera + webcam access) to any platforms, via **FMETP STREAM 6**.
- supports: Android | iOS | Mac | PC | Linux | HoloLens 2 | Vision Pro | WebGL | StereoPi | and more
- Youtube Tutorial:
https://youtu.be/vJNRrGsxT-k

- Tested with Unity 2022.3.58f1 and 6000.0.39f1
- Asset Store link: https://assetstore.unity.com/packages/slug/307623
- Support: thelghome@gmail.com

 ![GIF 2](./Media/fmetp_stream_oculus_passthrough_optimised.gif)

# Live Stream Meta Quest Mixed Reality to any devices
For quick testing:
- Download this template and import FMETP STREAM 6(v6.038 or above)
- Build & Deploy the scene "QuestVR_PassthroughCameraStreamer(UDP)" to Quest3/3S
- The viewer scene is "QuestVR_PassthroughCameraReceiver(UDP)"

|   Low Latency   | Cross Platforms |   Build & Run   |
|:---------------:|:---------------:|:---------------:|
| ![GIF 1](./Media/fmetp-stream-passthrough-test1.gif) | ![GIF 2](./Media/fmetp-stream-passthrough-test2.gif) | ![GIF 3](./Media/fmetp-stream-passthrough-test3.gif) |

# Reference
This example project refer to official oculus samples. For further setup requirement, please refer to this repo
- https://github.com/oculus-samples/Unity-PassthroughCameraApiSamples
