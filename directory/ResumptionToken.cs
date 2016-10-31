using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace registry
{
    class ResumptionToken
    {
        public String set { get; set; }
        public DateTime? from {get; set;}
        public DateTime? until { get; set; }
        public String metadataPrefix {get; set;}
        public DateTime expirationDate { get; set; }
        public String tokenValue { get; set; }
        public int cursor { get; set; }
        public int completeListSize {get; set;}

        public ResumptionToken(String tokenValue, DateTime expirationDate, DateTime from, DateTime until, String set, String metadataPrefix, int start, int completeListSize)
        {
            this.tokenValue = tokenValue;
            this.expirationDate = expirationDate;
            this.from = from;
            this.until = until;
            this.set = set;
            this.metadataPrefix = metadataPrefix;
            this.cursor = start;
            this.completeListSize = completeListSize;
        }

        public ResumptionToken()
        {
        }

    }
}
