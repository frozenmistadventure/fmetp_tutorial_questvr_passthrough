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
        [SerializeField] private PassthroughCameraAccess m_cameraAccess;
        [SerializeField] private Text m_debugText;

    [SerializeField] private PassthroughCameraAccess passthroughCameraAccess;
    [SerializeField] private GameViewEncoder gameViewEncoder;

    // [Space]
    // public UnityEvent<WebCamTexture> OnWebCamTextureReadyEvent = new UnityEvent<WebCamTexture>();

    [Space]
    [SerializeField] private FMNetworkManager fmnetwork;
    // private PassthroughCameraEye CameraEye => m_webCamTextureManager.Eye;
    // private Vector2Int CameraResolution => m_webCamTextureManager.RequestedResolution;
    private float horizontalFoVDegrees = 0f;
    private float verticalFoVDegrees = 0f;

    private void Start()
    {
        LoadWebCamTextureAsync();
    }

    private async void LoadWebCamTextureAsync()
    {
        var supportedResolutions = PassthroughCameraAccess.GetSupportedResolutions(PassthroughCameraAccess.CameraPositionType.Left);
        Assert.IsNotNull(supportedResolutions, nameof(supportedResolutions));
        Debug.Log($"PassthroughCameraAccess.GetSupportedResolutions(): {string.Join(", ", supportedResolutions)}");

        // while (passthroughCameraAccess.GetTexture() == null)
        while (!passthroughCameraAccess.IsPlaying)
        {
            await FMSolution.FMCoreTools.AsyncTask.Delay(10);
            await FMSolution.FMCoreTools.AsyncTask.Yield();
        }

        if (gameViewEncoder.WebcamTexture == null) gameViewEncoder.Action_SetWebcamTexture(passthroughCameraAccess.GetTexture());
        // OnWebCamTextureReadyEvent.Invoke(passthroughCameraAccess.GetTexture());
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
        gameViewEncoder.ViewScaleX = calibrationSettings.ViewScaleX;
        gameViewEncoder.ViewScaleY = calibrationSettings.ViewScaleY;
        gameViewEncoder.ViewOffsetX = calibrationSettings.ViewOffsetX;
        gameViewEncoder.ViewOffsetY = calibrationSettings.ViewOffsetY;

        gameViewEncoder.MixedRealityScaleX = calibrationSettings.MRScaleX;
        gameViewEncoder.MixedRealityScaleY = calibrationSettings.MRScaleY;
        gameViewEncoder.MixedRealityOffsetX = calibrationSettings.MROffsetX;
        gameViewEncoder.MixedRealityOffsetY = calibrationSettings.MROffsetY;

        //return back value
        if (cameraInfo == null) cameraInfo = new FMPassthroughCameraInfo();
        cameraInfo.WebcamFOV_h = horizontalFoVDegrees;
        cameraInfo.WebcamFOV_v = verticalFoVDegrees;
        cameraInfo.CamFOV_v = gameViewEncoder.MainCam.fieldOfView;
        cameraInfo.CamAspect = gameViewEncoder.MainCam.aspect;

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
