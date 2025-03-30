using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMSolution.FMNetwork;

public class FMPassthroughViewerCalibration : MonoBehaviour
{
    [SerializeField] private FMNetworkManager fmnetwork;
    [SerializeField] [Range(0.01f, 2f)] private float ScaleX = 1f;
    [SerializeField] [Range(0.01f, 2f)] private float ScaleY = 1f;
    [SerializeField] [Range(-0.5f, 0.5f)] private float OffsetX = 0f;
    [SerializeField] [Range(-0.5f, 0.5f)] private float OffsetY = 0f;

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
                _syncMessage += "," + ScaleX.ToString();
                _syncMessage += "," + ScaleY.ToString();
                _syncMessage += "," + OffsetX.ToString();
                _syncMessage += "," + OffsetY.ToString();

                fmnetwork.SendToOthers(_syncMessage);
            }
        }
    }
}
