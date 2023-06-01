using DM.WR.Models.Options;
using DM.WR.Models.Types;

namespace DM.WR.BL.Builders
{
    public interface IActuateQueryStringBuilder
    {
        string BuildQueryString(OptionPage page, UserData userData, bool runInBackground, string lastName = null, object extraQueryParams = null);
        string BuildMultimeasureQueryString(OptionBook book, UserData userData, bool runInBackground, string lastName = null);
        string BuildBackgroundReportParameters(int criteriaId, string criteriaName, OptionPage page, UserData userData);
    }
}