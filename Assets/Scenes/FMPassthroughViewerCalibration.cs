using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMSolution.FMNetwork;

public class FMPassthroughViewerCalibration : MonoBehaviour
{
    [SerializeField] private FMNetworkManager fmnetwork;

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
    }
}
