using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DM.UI.Library.Models;
using DM.WR.Models.Config;

namespace DM.WR.Models.ViewModels
{
    public class EntranceViewModel
    {
        public EntranceViewModel()
        {
            ViewList = new List<DropdownItem>();
        }

        public bool IsAdaptive => View == WebReportingView.IowaFlex;
        public string View { get; set; }
        public List<DropdownItem> ViewList { get; set; }


        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "Root location ID (if multiple - comma separated)")]
        public string LocationIds { get; set; }
        [Display(Name = "Location level")]
        public string LocationLevel { get; set; }


        [Display(Name = "User Name")]
        public string LocationGuids { get; set; }
        [Display(Name = "Password")]
        public string ContractInstances { get; set; }
        [Display(Name = "Elevate Role")]
        public string RoleName { get; set; }


        public string RedirectToUrl { get; set; }
        public string ErrorMessage { get; set; }
    }
}