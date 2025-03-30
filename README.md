# fmetp_tutorial_questvr_passthrough
This example project demonstrates the low latency passthrough live stream to any platforms, via **FMETP STREAM 6**.

- Tested with Unity 2022.3.58f1 and 6000.0.39f1
- Asset Store link: https://assetstore.unity.com/packages/slug/307623
- Support: thelghome@gmail.com

# Live Stream Meta Quest Mixed Reality to any devices
For quick testing:
- Download this template and import FMETP STREAM 6
- Build & Deploy the scene "QuestVR_PassthroughCameraStreamer(UDP)" to Quest3/3S
- The viewer scene is "QuestVR_PassthroughCameraReceiver(UDP)"

| Low Latency | Cross Platforms | Build & Run |
|:-------------:|:-------------:|:-------------:|
| ![GIF 1](./Media/fmetp-stream-passthrough-test1.gif) | ![GIF 2](./Media/fmetp-stream-passthrough-test2.gif) | ![GIF 3](./Media/fmetp-stream-passthrough-test3.gif) |

# Reference
This example project refer to official oculus samples. For further setup requirement, please refer to this repo
- https://github.com/oculus-samples/Unity-PassthroughCameraApiSamples
