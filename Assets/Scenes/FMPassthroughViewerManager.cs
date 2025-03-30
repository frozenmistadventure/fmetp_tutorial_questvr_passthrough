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
        if (float.TryParse(_data[1], out float _scaleX)) gameViewEncoder.MixedRealityScaleX = _scaleX;
        if (float.TryParse(_data[2], out float _scaleY)) gameViewEncoder.MixedRealityScaleY = _scaleY;
        if (float.TryParse(_data[3], out float _offsetX)) gameViewEncoder.MixedRealityOffsetX = _offsetX;
        if (float.TryParse(_data[4], out float _offsetY)) gameViewEncoder.MixedRealityOffsetY = _offsetY;
    }
}
