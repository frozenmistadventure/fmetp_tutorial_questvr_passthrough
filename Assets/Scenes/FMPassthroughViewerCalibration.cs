using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMSolution.FMNetwork;
using UnityEngine.Events;

public class FMPassthroughViewerCalibration : MonoBehaviour
{
    [SerializeField] private FMNetworkManager fmnetwork;
    private void OnEnable()
    {
        fmnetwork.OnReceivedStringDataEvent.AddListener(action_processString = (s) => { OnReceivedStringData(s); });
    }
    private void OnDisable()
    {
        if (action_processString != null) fmnetwork.OnReceivedStringDataEvent.RemoveListener(action_processString);
    }

    private UnityAction<string> action_processString;
    private void OnReceivedStringData(string inputString)
    {
        if (inputString.Contains("FMPassthroughCameraInfo"))
        {
            FMPassthroughCameraInfo(inputString);
        }
    }
    [SerializeField] private float webcamFOV_h = 0f;
    [SerializeField] private float webcamFOV_v = 0f;
    [SerializeField] private float camFOV_v = 0f;
    [SerializeField] private float camFOV_h = 0f;
    [SerializeField] private float camAspect = 0f;

    [Header(" - Estimated ScaleXY from FOV")]
    [SerializeField] private float estimatedViewScaleX = 0f;
    [SerializeField] private float estimatedViewScaleY = 0f;
    [SerializeField] private bool AutoApply = false;

    private void FMPassthroughCameraInfo(string inputString)
    {
        string[] _data = inputString.Split(",");
        if (float.TryParse(_data[1], out float _hfov)) webcamFOV_h = _hfov;
        if (float.TryParse(_data[2], out float _vfov)) webcamFOV_v = _vfov;
        if (float.TryParse(_data[3], out float _camFOV_v)) camFOV_v = _camFOV_v;
        if (float.TryParse(_data[4], out float _camAspect)) camAspect = _camAspect;

        {
            float verticalFOVRad = camFOV_v * Mathf.PI / 180.0f;
            float tanHalfVFOV = Mathf.Tan(verticalFOVRad / 2.0f);

            float tanHalfHFOV = tanHalfVFOV * camAspect;
            float horizontalFOVRad = 2.0f * Mathf.Atan(tanHalfHFOV);
            camFOV_h = horizontalFOVRad * 180.0f / Mathf.PI;
        }
    }

    [Space]
    [Header(" - Remote Calibration Variables")]
    [SerializeField] [Range(0.01f, 2f)] private float ViewScaleX = 1f;
    [SerializeField] [Range(0.01f, 2f)] private float ViewScaleY = 1f;
    [SerializeField] [Range(-0.5f, 0.5f)] private float ViewOffsetX = 0f;
    [SerializeField] [Range(-0.5f, 0.5f)] private float ViewOffsetY = 0f;

    [Space]
    [SerializeField] [Range(0.01f, 2f)] private float MRScaleX = 1f;
    [SerializeField] [Range(0.01f, 2f)] private float MRScaleY = 1f;
    [SerializeField] [Range(-0.5f, 0.5f)] private float MROffsetX = 0f;
    [SerializeField] [Range(-0.5f, 0.5f)] private float MROffsetY = 0f;



    [SerializeField] private bool RemoteSyncData = true;

    [SerializeField] private float syncFPS = 8f;
    private float syncTimer = 0f;
    private void Update()
    {
        if (RemoteSyncData)
        {
            syncTimer += Time.deltaTime;
            if (syncTimer >= 1f / syncFPS)
            {
                syncTimer %= 1f / syncFPS;

                string _syncMessage = "FMPassthroughViewerCalibration";
                _syncMessage += "," + ViewScaleX.ToString();
                _syncMessage += "," + ViewScaleY.ToString();
                _syncMessage += "," + ViewOffsetX.ToString();
                _syncMessage += "," + ViewOffsetY.ToString();


                _syncMessage += "," + MRScaleX.ToString();
                _syncMessage += "," + MRScaleY.ToString();
                _syncMessage += "," + MROffsetX.ToString();
                _syncMessage += "," + MROffsetY.ToString();

                fmnetwork.SendToOthers(_syncMessage);
            }
        }

        {
            float wall_cam = Mathf.Tan((camFOV_v / 2f) * Mathf.PI / 180f);
            float wall_web = Mathf.Tan((webcamFOV_v / 2f) * Mathf.PI / 180f);
            estimatedViewScaleY = wall_web / wall_cam;
        }
        {
            float tmp_r = 1f;
            float wall_cam = Mathf.Tan((camFOV_h / 2f) * Mathf.PI / 180f) / tmp_r;
            float wall_web = Mathf.Tan((webcamFOV_h / 2f) * Mathf.PI / 180f) / tmp_r;
            estimatedViewScaleX = wall_web / wall_cam;
        }
        if (AutoApply)
        {
            ViewScaleX = estimatedViewScaleX;
            ViewScaleY = estimatedViewScaleY;
        }
    }
}
