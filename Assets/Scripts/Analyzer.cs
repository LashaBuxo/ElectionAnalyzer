using BarGraph.VittorCloud;  
using System.Collections;
using System.Collections.Generic; 
using UnityEngine; 

public class Analyzer : MonoBehaviour
{
    public string proportionalDataBaseURL = "https://results.cec.gov.ge/assets/data/election_43/prop/districts/";
    public int startIndex = 0;
    public int endIndex = 30;
    public bool useCacheIfAvailable = true;
    public float delayBetweenRequests = 0.5f;
 
    [System.Serializable]
    public struct Party
    {
        public string name;
        public string number;
        public BarGraphGenerator BarGraphGenerator;
    }

    public List<Party> Parties;

    void Start()
    {
        ElectionDataGrabber.instance.Initialize(proportionalDataBaseURL, startIndex, endIndex, delayBetweenRequests);
        ElectionDataGrabber.instance.onDistrictsDataGrabbed += OnDistrictsDataGrabbed;

        ElectionDataGrabber.instance.StartGrabbingDistrictsData(useCacheIfAvailable);
    }

    public int[] GetVotesNumericsCountForParty(List<ElectionDataGrabber.DistrictData> districtsData, string partyNumber)
    {
        int[] counts = new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        foreach (var districtData in districtsData)
        {
            foreach (var districtPointData in districtData.Items)
            {
                foreach (var subject in districtPointData.Subjects)
                {
                    string numStr = subject.Vote.ToString();
                    if (subject.Number == partyNumber && numStr.Length>1)
                        counts[numStr[numStr.Length-2] - '0']++;
                }
            }
        }

        return counts;
    }

    public List<float> BenfordsValues;

    private void showPartyAgainstBenford(Party party, List<ElectionDataGrabber.DistrictData> districtsData)
    {
        List<BarGraphDataSet> barGraphDataSets = new List<BarGraphDataSet>();


        BarGraphDataSet benfordDataSet = new BarGraphDataSet();
        benfordDataSet.ListOfBars = new List<XYBarValues>();
        benfordDataSet.GroupName = "სტანდარტული განაწილება";
        int ind = 0;
        foreach (var benfordsValue in BenfordsValues)
        {
            XYBarValues xyBarValues = new XYBarValues();
            xyBarValues.YValue = benfordsValue;
            ind++;
            xyBarValues.XValue = ind.ToString();
            benfordDataSet.ListOfBars.Add(xyBarValues);
        }


        barGraphDataSets.Add(benfordDataSet);

        int[] counts = GetVotesNumericsCountForParty(districtsData, party.number);
        BarGraphDataSet partyDataSet = new BarGraphDataSet();
        partyDataSet.GroupName = party.name;
        partyDataSet.ListOfBars = new List<XYBarValues>();

        int total = 0;
        for (int num = 1; num < 10; num++)
        {
            total += counts[num];
        }

        for (int num = 1; num < 10; num++)
        {
            XYBarValues xyBarValues = new XYBarValues();
            xyBarValues.XValue = num.ToString();
            xyBarValues.YValue = ((int) (1000.0f * counts[num] / total) / 10.0f);
            partyDataSet.ListOfBars.Add(xyBarValues);
        }

        barGraphDataSets.Add(partyDataSet);

        party.BarGraphGenerator.GeneratBarGraph(barGraphDataSets);
    }

    private void OnDistrictsDataGrabbed(List<ElectionDataGrabber.DistrictData> districtsData)
    {
        StartCoroutine(StartShowCase(districtsData));
    }
 
    private IEnumerator StartShowCase(List<ElectionDataGrabber.DistrictData> districtsData)
    {
        foreach (var party in Parties)
        {
           // barGraphGenerator.res
            showPartyAgainstBenford(party, districtsData);
            yield return new WaitForSeconds(3);
            party.BarGraphGenerator.gameObject.SetActive(false);
        }
        yield break;
    }
}