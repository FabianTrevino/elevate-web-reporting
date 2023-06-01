using System.Collections.Generic;
using System.Linq;

namespace DM.WR.Models.BackgroundReport
{
    public class GetReportsResponse
    {
        public GetReportsResponse()
        {
            Reports = new List<ReportMeta>();
        }

        public int Count => Reports == null ? 0 : Reports.Count();

        public IEnumerable<ReportMeta> Reports { get; set; }

        public override string ToString()
        {
            return $"Reports Count: {Count}";
        }
    }
}
