using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMSolution.FMNetwork;
using UnityEngine.Events;
using System;

[Serializable]
public class FMPassthroughViewerCalibrationSettings
{
    [Range(0.01f, 2f)] public float ViewScaleX = 1f;
    [Range(0.01f, 2f)] public float ViewScaleY = 1f;
    [Range(-0.5f, 0.5f)] public float ViewOffsetX = 0f;
    [Range(-0.5f, 0.5f)] public float ViewOffsetY = 0f;

    [Space]
    [Range(0.01f, 2f)] public float MRScaleX = 1f;
    [Range(0.01f, 2f)] public float MRScaleY = 1f;
    [Range(-0.5f, 0.5f)] public float MROffsetX = 0f;
    [Range(-0.5f, 0.5f)] public float MROffsetY = 0f;
}
[Serializable]
public class FMPassthroughCameraInfo
{

    public float WebcamFOV_h = 0f;
    public float WebcamFOV_v = 0f;
    public float CamFOV_v = 0f;
    public float CamFOV_h = 0f;
    public float CamAspect = 0f;
}
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

    [SerializeField] private FMPassthroughCameraInfo cameraInfo;

    [Header(" - Estimated ScaleXY from FOV")]
    [SerializeField] private float estimatedViewScaleX = 0f;
    [SerializeField] private float estimatedViewScaleY = 0f;
    [SerializeField] private float additionalViewScaleX = 0f;
    [SerializeField] private float additionalViewScaleY = 0f;
    [SerializeField] private bool AutoApply = false;

    private void FMPassthroughCameraInfo(string inputString)
    {
        string _json = inputString.Replace("FMPassthroughCameraInfo","");
        cameraInfo = JsonUtility.FromJson<FMPassthroughCameraInfo>(_json);
    }

    [Space]
    [Header(" - Remote Calibration Variables")]

    [SerializeField] private FMPassthroughViewerCalibrationSettings calibrationSettings;

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
                string _syncMessage = "FMPassthroughViewerCalibration" + JsonUtility.ToJson(calibrationSettings, false);

                fmnetwork.SendToOthers(_syncMessage);
            }
        }

        {
            float wall_cam = Mathf.Tan((cameraInfo.CamFOV_v / 2f) * Mathf.PI / 180f);
            float wall_web = Mathf.Tan((cameraInfo.WebcamFOV_v / 2f) * Mathf.PI / 180f);
            estimatedViewScaleY = wall_web / wall_cam;
        }
        {
            float tmp_r = 1f;
            float wall_cam = Mathf.Tan((cameraInfo.CamFOV_h / 2f) * Mathf.PI / 180f) / tmp_r;
            float wall_web = Mathf.Tan((cameraInfo.WebcamFOV_h / 2f) * Mathf.PI / 180f) / tmp_r;
            estimatedViewScaleX = wall_web / wall_cam;
        }
        if (AutoApply)
        {
            calibrationSettings.ViewScaleX = estimatedViewScaleX + additionalViewScaleX;
            calibrationSettings.ViewScaleY = estimatedViewScaleY + additionalViewScaleY;
        }
    }
}
