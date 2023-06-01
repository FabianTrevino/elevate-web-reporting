using System.Collections.Generic;
using DM.UI.Library.Models;

namespace DM.WR.Models.ViewModels
{
    public class LocationChangeModalModel : ModalModel
    {
        public LocationChangeModalModel() : base(SectionColor.Green)
        {
            Locations = new List<DropdownItem>();
        }

        public List<DropdownItem> Locations { get; set; }
        public string Label { get; set; }
    }
}