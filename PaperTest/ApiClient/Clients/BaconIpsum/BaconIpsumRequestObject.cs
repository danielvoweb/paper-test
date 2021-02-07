using System.Collections.Generic;

namespace ApiClient.Clients.BaconIpsum
{
    public class BaconIpsumRequestObject
    {
        public BaconIpsumRequestObject(BaconIpsumFillerType fillerType)
        {
            FillerType = FillerTypeLookup[fillerType];
        }
        
        public string FillerType { get; }

        public int ParagraphCount { get; set; }
        
        public bool StartWithLorem { get; set; }
        
        public int SentenceCount { get; set; }

        public string RequestPart()
        {
            var requestPart = $"?type={FillerType}";
            if (ParagraphCount != default)
                requestPart += $"&paras={ParagraphCount}";
            if (SentenceCount != default)
                requestPart += $"&sentences={SentenceCount}";
            if (StartWithLorem)
                requestPart += "&start-with-lorem=1";
            
            return requestPart;
        }

        private static readonly Dictionary<BaconIpsumFillerType, string> FillerTypeLookup = new()
        {
            {BaconIpsumFillerType.AllMeat, "all-meat"},
            {BaconIpsumFillerType.MeatAndFiller, "meat-and-filler"},
        };
        
    }

    public enum BaconIpsumFillerType
    {
        AllMeat,
        MeatAndFiller
    }
}