using DM.WR.Models.CogAt;

namespace DM.WR.BL.Builders
{
    public class BuildersHelper
    {
        public string CreatePopulationFiltersPileLabel(string nameFromDatabase)
        {
            switch (nameFromDatabase)
            {
                case "Gender":
                    return "Gender";
                case "Ethnicity":
                    return "Ethnicity";
                case "Race":
                    return "Race";
                case "Programs":
                    return "Program";
                case "Admin Codes":
                    return "Admin Value";
                case "Office Use":
                    return "Office Use";
                case "Test Admin Code":
                    return "Test Admin Code";

                default:
                    return "";
            }
        }

        public string CreatePopulationFiltersPileKey(string nameFromDatabase)
        {
            switch (nameFromDatabase)
            {
                case "Gender":
                    return PileKey.GenderList;
                case "Race":
                    return PileKey.RaceList;
                case "Ethnicity":
                    return PileKey.EthnicityList;
                case "Programs":
                    return PileKey.ProgramList;
                case "Admin Codes":
                    return PileKey.AdminValueList;
                case "Office Use":
                    return PileKey.OfficeUseList;
                case "Test Admin Code":
                    return PileKey.TestAdminCodeList;

                default:
                    return "";
            }
        }
    }
}