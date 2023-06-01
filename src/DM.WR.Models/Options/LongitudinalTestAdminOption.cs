using System.Collections.Generic;
using System.Linq;
using DM.UI.Library.Models;

namespace DM.WR.Models.Options
{
    public class LongitudinalTestAdminOption : Option
    {
        public List<Option> GradeLevels;

        public IEnumerable<DropdownItem> GetGradeLevelsDropdownList()
        {
            return GradeLevels.Select(o => new DropdownItem
            {
                Text = o.Text,
                Value = o.Value,
                AltValue = o.AltValue,
                Selected = o.IsSelected,
                Disabled = o.IsDisabled,
            });
        }
    }
}