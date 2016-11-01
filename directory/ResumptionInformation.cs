using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace registry
{
    public class ResumptionInformation
    {
        public String set { get; set; }
        public DateTime? from {get; set;}
        public DateTime? until { get; set; }
        public String metadataPrefix {get; set;}
        public DateTime expirationDate { get; set; }
        public String tokenValue { get; set; }
        public int startIdx { get; set; }
        public int completeListSize {get; set;}

        public ResumptionInformation(String tokenValue, DateTime expirationDate, DateTime? from, DateTime? until, String set, String metadataPrefix, int start, int completeListSize)
        {
            this.tokenValue = tokenValue;
            this.expirationDate = expirationDate;
            this.from = from;
            this.until = until;
            this.set = set;
            this.metadataPrefix = metadataPrefix;
            this.startIdx = start;
            this.completeListSize = completeListSize;
        }

        public ResumptionInformation()
        {
        }

    }
}
