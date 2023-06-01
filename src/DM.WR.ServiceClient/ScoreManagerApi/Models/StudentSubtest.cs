using System.Collections.Generic;

namespace DM.WR.ServiceClient.ScoreManagerApi.Models
{
    public class StudentSubtest
    {
        public string Class_names { get; set; }
        public string Class_ids { get; set; }
        public string Building_name { get; set; }
        public string Building_id { get; set; }
        public string District_name { get; set; }
        public string District_id { get; set; }
        public int Grade { get; set; }
        public string Student_id { get; set; }
        public object Sis_id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Subtest_name { get; set; }
        public string Birth_date { get; set; }
        public object Birth_date_format { get; set; }
        public string Test_age_yy_mm_score { get; set; }
        public string Form { get; set; }
        public string Subtest_mininame { get; set; }
        public string Quartermonth_desc { get; set; }
        public string Rpt_normyear_desc { get; set; }
        public string Testinstance_test_date { get; set; }
        public string Level_number { get; set; }
        public string Norm_code { get; set; }
        public int? Apr_score { get; set; }
        public int? As_score { get; set; }
        public int? Gpr_score { get; set; }
        public int? Gs_score { get; set; }
        public int? Uss_score { get; set; }
        public int? Sas_score { get; set; }
        public int? Rs_score { get; set; }
        public int? Na_score { get; set; }
        public int? Total_item_score { get; set; }
        public int? Lpr_score { get; set; }
        public int? Ls_score { get; set; }
        public string Prof_score { get; set; }
        public int sas_flag { get; set; }
        public int tmo_flag { get; set; }
        public int chance_flag { get; set; }
        public int pr1_flag { get; set; }
        public int levelrange_flag { get; set; }
        public int levelassigned_flag { get; set; }
        public int ev_flag { get; set; }
        public int Cc { get; set; }
        public int Na { get; set; }
        public int Adm_z { get; set; }
        public int Exclude_flag { get; set; }
        public List<string> Battery { get; set; }
        public bool Is_alt { get; set; }
        public bool Age_unusual { get; set; }
    }
}