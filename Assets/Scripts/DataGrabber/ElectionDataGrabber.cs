using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

public partial class ElectionDataGrabber : MonoBehaviour
{
    public struct DistrictPointData
    {
        public struct Subject
        {
            public string Name;
            public string Number;
            public float Percent;
            public int Vote;
        }

        public string Name;
        public string Number;
        public bool Canceled;
        public string Info;
        public string ProtId;

        public List<Subject> Subjects;
    }

    public class DistrictData
    {
        public struct Summary
        {
            public int Total;
            public int Counted;
            public float Percent;
            public int Canceled;
            public int Foreign;
        }

        public string Type;
        public string Name;

        public int Number;

        public Summary Divisions;
        public List<DistrictPointData> Items;
    }

    private string baseURL;
    private int startIndex;
    private int endIndex;
    private float delayBetweenRequests;

    public event Action<List<DistrictData>> onDistrictsDataGrabbed;

    private List<DistrictData> districtsData;

    public static ElectionDataGrabber instance; 
    private void Awake()
    {
        instance = this;
    }

    public void Initialize(string baseURL, int startIndex, int endIndex, float delayBetweenRequests = 0.5f)
    {
        this.baseURL = baseURL;
        this.startIndex = startIndex;
        this.endIndex = endIndex;
        this.delayBetweenRequests = delayBetweenRequests;
        if (districtsData == null)
        {
            districtsData = new List<DistrictData>();
        }
        else
        {
            districtsData.Clear();
        }
    }

    public void StartGrabbingDistrictsData(bool useCacheIfAvailable = true)
    {
        StartCoroutine(RetrieveJsonSteps(useCacheIfAvailable));
    }

    private IEnumerator RetrieveJsonSteps(bool useCacheIfAvailable)
    {
        InitializeOrLoadCache();

        for (int i = startIndex; i <= endIndex; i++)
        {
            string curUrl = $"{baseURL}{i}.json";

            if (useCacheIfAvailable && cachedElectionData.ContainsKey(curUrl))
            {
                Debug.Log("Used cached data on district with index: " + i);
                districtsData.Add(cachedElectionData[curUrl]);
                continue;
            }

            using (UnityWebRequest request = UnityWebRequest.Get(curUrl))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log($"{request.error}: {request.downloadHandler.text}");
                }
                else
                {
                    Debug.Log("Grabbed fresh data on district with index: " + i);
                    DistrictData data = JsonConvert.DeserializeObject<DistrictData>(request.downloadHandler.text);
                    districtsData.Add(data);
                    AddDataInCache(curUrl, data);
                }
                
                yield return new WaitForSeconds(delayBetweenRequests);
            } 
        }

        onDistrictsDataGrabbed.Invoke(districtsData);
    }
}