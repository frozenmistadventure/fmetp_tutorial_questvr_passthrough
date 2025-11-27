using UnityEngine;
using UnityEngine.Events;
using FMSolution.FMETP;
using FMSolution.FMNetwork;
using PassthroughCameraSamples;

public class FMPassthroughCamera : MonoBehaviour
{
    [SerializeField] private PassthroughCameraSamples.WebCamTextureManager m_webCamTextureManager;
    [SerializeField] private GameViewEncoder gameViewEncoder;

    [Space]
    public UnityEvent<WebCamTexture> OnWebCamTextureReadyEvent = new UnityEvent<WebCamTexture>();

    [Space]
    [SerializeField] private FMNetworkManager fmnetwork;
    private PassthroughCameraEye CameraEye => m_webCamTextureManager.Eye;
    private Vector2Int CameraResolution => m_webCamTextureManager.RequestedResolution;
    private float horizontalFoVDegrees = 0f;
    private float verticalFoVDegrees = 0f;

    private void Start()
    {
        LoadWebCamTextureAsync();
    }

    private async void LoadWebCamTextureAsync()
    {
        while (m_webCamTextureManager.WebCamTexture == null)
        {
            await FMSolution.FMCoreTools.AsyncTask.Delay(10);
            await FMSolution.FMCoreTools.AsyncTask.Yield();
        }

        if (gameViewEncoder.WebcamTexture == null) gameViewEncoder.Action_SetWebcamTexture(m_webCamTextureManager.WebCamTexture);
        OnWebCamTextureReadyEvent.Invoke(m_webCamTextureManager.WebCamTexture);

        //
        Ray leftSidePointInCamera = PassthroughCameraUtils.ScreenPointToRayInCamera(CameraEye, new Vector2Int(0, CameraResolution.y / 2));
        Ray rightSidePointInCamera = PassthroughCameraUtils.ScreenPointToRayInCamera(CameraEye, new Vector2Int(CameraResolution.x, CameraResolution.y / 2));
        Ray topSidePointInCamera = PassthroughCameraUtils.ScreenPointToRayInCamera(CameraEye, new Vector2Int(CameraResolution.x / 2, CameraResolution.y));
        Ray bottomSidePointInCamera = PassthroughCameraUtils.ScreenPointToRayInCamera(CameraEye, new Vector2Int(CameraResolution.x / 2, 0));
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

        string _cameraInfoJson = JsonUtility.ToJson(cameraInfo, false);
        fmnetwork.SendToServer(_cameraInfoJson);
    }
}
