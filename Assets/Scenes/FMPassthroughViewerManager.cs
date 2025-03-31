using UnityEngine;
using UnityEngine.Events;
using FMSolution.FMETP;

public class FMPassthroughCamera : MonoBehaviour
{
    [SerializeField] private PassthroughCameraSamples.WebCamTextureManager m_webCamTextureManager;
    [SerializeField] private GameViewEncoder gameViewEncoder;

    [Space]
    public UnityEvent<WebCamTexture> OnWebCamTextureReadyEvent = new UnityEvent<WebCamTexture>();

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
    }
}
