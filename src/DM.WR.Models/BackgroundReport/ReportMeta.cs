using System;
using System.Collections.Generic;

namespace DM.WR.Models.BackgroundReport
{
    public class ReportMeta
    {
        public DateTimeOffset CreatedAt { get; set; }

        public string Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> References
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "delete",  $"api/reports/{UserId}/delete/{Id}" },
                    { "download",  $"api/reports/{UserId}/id/{Id}" },
                };
            }
        }

        public ReportFormat ReportFormat { get; set; }

        public string ReportType { get; set; }

        public ReportStatus Status { get; set; }

        public string TimeToProcess => Status == ReportStatus.Completed ?
                                            (UpdatedAt.Value - CreatedAt).ToString(@"mm\:ss") :
                                            (DateTime.Now - CreatedAt).ToString(@"mm\:ss");

        public DateTimeOffset? UpdatedAt { get; set; }

        public string UserId { get; set; }

        // WebReporting specific
        public bool DisplayLinkForReport => Status == ReportStatus.Completed;
    }
}
