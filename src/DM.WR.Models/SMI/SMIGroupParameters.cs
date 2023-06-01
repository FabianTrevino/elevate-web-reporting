using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.Models.SMI
{
    public class SMIGroupParameters
    {
        public bool GroupScoresByForm { get; set; }
        public bool GroupScoresByLevel { get; set; }
        public bool GroupScoresByBattery { get; set; }
        public string CogatProfileGroupScoreMode { get; set; }
    }
}
