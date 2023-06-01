using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;

namespace DM.WR.BL.Builders
{
    public interface IOptionsBuilder
    {
        OptionGroup InvalidGroup { get; }
        OptionPage BuildOptions(OptionPage currentPage, XMLGroupType updatedGroupType, UserData userData, int pageIndex, bool isActuateHyperLink = false);
        OptionBook SyncUpPagesForMultimeasure(OptionBook book);
        OptionPage UpdateReportOptions(OptionPage page, string queryString, UserData userData, out OptionGroup topChangedGroup, out string extraQueryParams);
    }
}