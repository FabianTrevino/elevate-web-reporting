using System.Collections.Generic;
using System;

namespace IRM_Library.Objects
{
    [Serializable]    
    public class CustomerScoringOptions
    {
        public CustomerScoringOptions()
        {
            DisplayFlags = new Dictionary<string, bool>();
        }

        public Dictionary<string, bool> DisplayFlags { get; set; }

        public int skillSetID { get; set; }
        public int wordskillSetID { get; set; }
        public int groupsetID { get; set; }
        public string groupsetCode { get; set; }
        public string gpd_groupsetcode { get; set; }
        public int defaultADMZ { get; set; }
        public string lpr_node_level { get; set; }
        public string lpr_node_list { get; set; }
        public string test_type { get; set; }
        public int allow_lexile_score { get; set; }
        public int allow_lpr_score { get; set; }
        public int allow_cathpriv_flag { get; set; }
        public int exclude_mathcomp_default { get; set; }
        public string pred_subtestgroup_type { get; set; }
        public string subtest_cutscorefamily_id { get; set; }
        public int longitudinal_flag { get; set; }
        public string default_cogat_diff { get; set; }
        public string predicted_subtest_acronym { get; set; }
        public int ccore_skillset_id { get; set; }
        public string ccore_subtestgroup_type { get; set; }
        public int ELATotal { get; set; }  
        public string splitDate { get; set; }
        public int has_items_flag { get; set; }
        public string alternativeNormYear { get; set; }
        public int AccountabilityFlag { get; set; }
    }
}
