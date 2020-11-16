using System.Collections.Generic;
using UnityEngine; 

public class Analyzer : MonoBehaviour
{
    public string proportionalDataBaseURL = "https://results.cec.gov.ge/assets/data/election_43/prop/districts/";
    public int startIndex = 0;
    public int endIndex = 30; 
    public bool useCacheIfAvailable = true; 
    public float delayBetweenRequests = 0.5f;
    void Start()
    { 
        ElectionDataGrabber.instance.Initialize(proportionalDataBaseURL,startIndex,endIndex,delayBetweenRequests); 
        ElectionDataGrabber.instance.onDistrictsDataGrabbed += OnDistrictsDataGrabbed; 
        
        ElectionDataGrabber.instance.StartGrabbingDistrictsData(useCacheIfAvailable); 
    }

    private void OnDistrictsDataGrabbed(List<ElectionDataGrabber.DistrictData> districtsData)
    {
        foreach (var districtData in districtsData)
        {
            Debug.Log(districtData.Name+" "+districtData.Number+" "+districtData.Divisions.Total);
        }
    }
    
    void Update()
    {
        
    }
}
