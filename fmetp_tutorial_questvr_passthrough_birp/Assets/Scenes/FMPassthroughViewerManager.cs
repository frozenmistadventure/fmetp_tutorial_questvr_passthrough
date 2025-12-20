using UnityEngine;
using UnityEngine.Events;
using FMSolution.FMETP;
using FMSolution.FMNetwork;

using System.Collections;
using Meta.XR;
using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class FMPassthroughCamera : MonoBehaviour
{
    [SerializeField] private PassthroughCameraAccess passthroughCameraAccess_left;
    [SerializeField] private PassthroughCameraAccess passthroughCameraAccess_right;
    [SerializeField] private GameViewEncoder[] gameViewEncoders;
    [SerializeField] private GameViewEncoder gameViewEncoder_left;
    [SerializeField] private GameViewEncoder gameViewEncoder_right;
    [SerializeField] private GameViewEncoder gameViewEncoder_stereo;

    [Space]
    [SerializeField] private FMNetworkManager fmnetwork;
    private float horizontalFoVDegrees = 0f;
    private float verticalFoVDegrees = 0f;

    private void Start()
    {

        if (gameViewEncoder_left != null) 
        {
            gameViewEncoders = gameViewEncoder_left.gameObject.GetComponents<GameViewEncoder>();
            for(int i = 0; i < gameViewEncoders.Length; i++)
            {
                Debug.Log("gameViewEncoders: " + i + " : " + gameViewEncoders[i].ViewTargetIndex);
                if (gameViewEncoders[i].ViewTargetIndex == FMSolution.FMMedia.FMMediaViewTargetIndex.Left && gameViewEncoder_left == null) gameViewEncoder_left = gameViewEncoders[i];
                if (gameViewEncoders[i].ViewTargetIndex == FMSolution.FMMedia.FMMediaViewTargetIndex.Right && gameViewEncoder_right == null) gameViewEncoder_right = gameViewEncoders[i];
            }
        }

        if (gameViewEncoder_stereo == null)
        {
            if (gameViewEncoder_left != null && passthroughCameraAccess_left != null) LoadWebCamTextureAsync(gameViewEncoder_left, passthroughCameraAccess_left);
            if (gameViewEncoder_right != null && passthroughCameraAccess_right != null) LoadWebCamTextureAsync(gameViewEncoder_right, passthroughCameraAccess_right);
        }
        if (gameViewEncoder_stereo != null && passthroughCameraAccess_right != null) LoadWebCamTextureStereoAsync(gameViewEncoder_stereo, passthroughCameraAccess_left, passthroughCameraAccess_right);
    }

    private async void LoadWebCamTextureStereoAsync(GameViewEncoder gameViewEncoder, PassthroughCameraAccess passthroughCameraAccessL, PassthroughCameraAccess passthroughCameraAccessR)
    {
        var supportedResolutions = PassthroughCameraAccess.GetSupportedResolutions(PassthroughCameraAccess.CameraPositionType.Left);
        Assert.IsNotNull(supportedResolutions, nameof(supportedResolutions));
        Debug.Log($"PassthroughCameraAccess.GetSupportedResolutions(): {string.Join(", ", supportedResolutions)}");

        while (!passthroughCameraAccessL.IsPlaying)
        {
            await FMSolution.FMCoreTools.AsyncTask.Delay(10);
            await FMSolution.FMCoreTools.AsyncTask.Yield();
        }
        while (!passthroughCameraAccessR.IsPlaying)
        {
            await FMSolution.FMCoreTools.AsyncTask.Delay(10);
            await FMSolution.FMCoreTools.AsyncTask.Yield();
        }

        if (gameViewEncoder.WebcamTexture == null) gameViewEncoder.Action_SetWebcamTexture(passthroughCameraAccessL.GetTexture(), passthroughCameraAccessR.GetTexture());
        // m_image.texture = passthroughCameraAccess.GetTexture();

        //
        Ray leftSidePointInCamera = passthroughCameraAccessL.ViewportPointToRay(new Vector2(0f, 0.5f));
        Ray rightSidePointInCamera = passthroughCameraAccessL.ViewportPointToRay(new Vector2(1f, 0.5f));
        Ray topSidePointInCamera = passthroughCameraAccessL.ViewportPointToRay(new Vector2(0.5f, 1f));
        Ray bottomSidePointInCamera = passthroughCameraAccessL.ViewportPointToRay(new Vector2(0.5f, 0f));
        horizontalFoVDegrees = Vector3.Angle(leftSidePointInCamera.direction, rightSidePointInCamera.direction);
        verticalFoVDegrees = Vector3.Angle(topSidePointInCamera.direction, bottomSidePointInCamera.direction);
    }

    private async void LoadWebCamTextureAsync(GameViewEncoder gameViewEncoder, PassthroughCameraAccess passthroughCameraAccess)
    {
        var supportedResolutions = PassthroughCameraAccess.GetSupportedResolutions(PassthroughCameraAccess.CameraPositionType.Left);
        Assert.IsNotNull(supportedResolutions, nameof(supportedResolutions));
        Debug.Log($"PassthroughCameraAccess.GetSupportedResolutions(): {string.Join(", ", supportedResolutions)}");

        while (!passthroughCameraAccess.IsPlaying)
        {
            await FMSolution.FMCoreTools.AsyncTask.Delay(10);
            await FMSolution.FMCoreTools.AsyncTask.Yield();
        }

        if (gameViewEncoder.WebcamTexture == null) gameViewEncoder.Action_SetWebcamTexture(passthroughCameraAccess.GetTexture());
        // m_image.texture = passthroughCameraAccess.GetTexture();

        //
        Ray leftSidePointInCamera = passthroughCameraAccess.ViewportPointToRay(new Vector2(0f, 0.5f));
        Ray rightSidePointInCamera = passthroughCameraAccess.ViewportPointToRay(new Vector2(1f, 0.5f));
        Ray topSidePointInCamera = passthroughCameraAccess.ViewportPointToRay(new Vector2(0.5f, 1f));
        Ray bottomSidePointInCamera = passthroughCameraAccess.ViewportPointToRay(new Vector2(0.5f, 0f));
        horizontalFoVDegrees = Vector3.Angle(leftSidePointInCamera.direction, rightSidePointInCamera.direction);
        verticalFoVDegrees = Vector3.Angle(topSidePointInCamera.direction, bottomSidePointInCamera.direction);
    }

    public void Action_DecodeMessage(string inputString)
    {
        if (inputString.Contains("FMPassthroughViewerCalibration"))
        {
            FMPassthroughViewerCalibration(inputString);
        }
    }

    private FMPassthroughViewerCalibrationSettings calibrationSettings;
    private FMPassthroughCameraInfo cameraInfo;
    private void FMPassthroughViewerCalibration(string inputString)
    {
        string _json = inputString.Replace("FMPassthroughViewerCalibration", "");
        calibrationSettings = JsonUtility.FromJson<FMPassthroughViewerCalibrationSettings>(_json);
        if (gameViewEncoder_left != null)
        {
            gameViewEncoder_left.ViewScaleX = calibrationSettings.ViewScaleX;
            gameViewEncoder_left.ViewScaleY = calibrationSettings.ViewScaleY;
            gameViewEncoder_left.ViewOffsetX = calibrationSettings.ViewOffsetX;
            gameViewEncoder_left.ViewOffsetY = calibrationSettings.ViewOffsetY;

            gameViewEncoder_left.MixedRealityScaleX = calibrationSettings.MRScaleX;
            gameViewEncoder_left.MixedRealityScaleY = calibrationSettings.MRScaleY;
            gameViewEncoder_left.MixedRealityOffsetX = calibrationSettings.MROffsetX;
            gameViewEncoder_left.MixedRealityOffsetY = calibrationSettings.MROffsetY;
        }
        if (gameViewEncoder_right != null)
        {
            gameViewEncoder_right.ViewScaleX = calibrationSettings.ViewScaleX;
            gameViewEncoder_right.ViewScaleY = calibrationSettings.ViewScaleY;
            gameViewEncoder_right.ViewOffsetX = calibrationSettings.ViewOffsetX * -1f; //Notes: Opposite offset direction
            gameViewEncoder_right.ViewOffsetY = calibrationSettings.ViewOffsetY;

            gameViewEncoder_right.MixedRealityScaleX = calibrationSettings.MRScaleX;
            gameViewEncoder_right.MixedRealityScaleY = calibrationSettings.MRScaleY;
            gameViewEncoder_right.MixedRealityOffsetX = calibrationSettings.MROffsetX;
            gameViewEncoder_right.MixedRealityOffsetY = calibrationSettings.MROffsetY;
        }
        if (gameViewEncoder_stereo != null)
        {
            gameViewEncoder_stereo.ViewScaleX = calibrationSettings.ViewScaleX;
            gameViewEncoder_stereo.ViewScaleY = calibrationSettings.ViewScaleY;
            gameViewEncoder_stereo.ViewOffsetX = calibrationSettings.ViewOffsetX;
            gameViewEncoder_stereo.ViewOffsetY = calibrationSettings.ViewOffsetY;

            gameViewEncoder_stereo.MixedRealityScaleX = calibrationSettings.MRScaleX;
            gameViewEncoder_stereo.MixedRealityScaleY = calibrationSettings.MRScaleY;
            gameViewEncoder_stereo.MixedRealityOffsetX = calibrationSettings.MROffsetX;
            gameViewEncoder_stereo.MixedRealityOffsetY = calibrationSettings.MROffsetY;
        }

        //return back value
        if (cameraInfo == null) cameraInfo = new FMPassthroughCameraInfo();
        cameraInfo.WebcamFOV_h = horizontalFoVDegrees;
        cameraInfo.WebcamFOV_v = verticalFoVDegrees;
        cameraInfo.CamFOV_v = gameViewEncoder_stereo != null ? gameViewEncoder_stereo.MainCam.fieldOfView : gameViewEncoder_left.MainCam.fieldOfView;
        cameraInfo.CamAspect = gameViewEncoder_stereo != null ? gameViewEncoder_stereo.MainCam.aspect : gameViewEncoder_left.MainCam.aspect;

        {
            float verticalFOVRad = cameraInfo.CamFOV_v * Mathf.PI / 180.0f;
            float tanHalfVFOV = Mathf.Tan(verticalFOVRad / 2.0f);

            float tanHalfHFOV = tanHalfVFOV * cameraInfo.CamAspect;
            float horizontalFOVRad = 2.0f * Mathf.Atan(tanHalfHFOV);
            cameraInfo.CamFOV_h = horizontalFOVRad * 180.0f / Mathf.PI;
        }

        string _cameraInfoJson = JsonUtility.ToJson(cameraInfo, true);
        fmnetwork.SendToServer(_cameraInfoJson);
    }
}
