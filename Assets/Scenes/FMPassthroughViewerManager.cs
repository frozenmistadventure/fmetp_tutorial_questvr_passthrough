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

    private void FMPassthroughViewerCalibration(string inputString)
    {
        string[] _data = inputString.Split(",");
        if (float.TryParse(_data[1], out float _vscaleX)) gameViewEncoder.ViewScaleX = _vscaleX;
        if (float.TryParse(_data[2], out float _vscaleY)) gameViewEncoder.ViewScaleY = _vscaleY;
        if (float.TryParse(_data[3], out float _voffsetX)) gameViewEncoder.ViewOffsetX = _voffsetX;
        if (float.TryParse(_data[4], out float _voffsetY)) gameViewEncoder.ViewOffsetY = _voffsetY;

        if (float.TryParse(_data[5], out float _mscaleX)) gameViewEncoder.MixedRealityScaleX = _mscaleX;
        if (float.TryParse(_data[6], out float _mscaleY)) gameViewEncoder.MixedRealityScaleY = _mscaleY;
        if (float.TryParse(_data[7], out float _moffsetX)) gameViewEncoder.MixedRealityOffsetX = _moffsetX;
        if (float.TryParse(_data[8], out float _moffsetY)) gameViewEncoder.MixedRealityOffsetY = _moffsetY;

        //return back value
        string _cameraInfo = $@"FMPassthroughCameraInfo,{horizontalFoVDegrees},{verticalFoVDegrees},{gameViewEncoder.MainCam.fieldOfView},{gameViewEncoder.MainCam.aspect}";
        fmnetwork.SendToServer(_cameraInfo);
    }
}
