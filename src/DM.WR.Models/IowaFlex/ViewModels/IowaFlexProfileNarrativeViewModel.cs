using System.Collections.Generic;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class IowaFlexProfileNarrativeViewModel
    {

        public IowaFlexProfileNarrativeViewModel()
        {
            Reports = new List<IowaFlexProfileNarrativeReport>();
            Ranges = new Dictionary<string, List<Band>>();
        }

        public List<IowaFlexProfileNarrativeReport> Reports { get; set; }
        public Dictionary<string, List<Band>> Ranges { get; set; }
    }
}