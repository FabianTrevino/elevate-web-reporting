namespace DM.WR.Models.Options
{
    public class CheckboxOption : Option
    {
        public string SelectedValue { get; set; }
        public string DiselectedValue { get; set; }
        //TODO: Make sure to use the below value as oppose to check for Selected/Diselected all the time
        public new string Value => IsSelected ? SelectedValue : DiselectedValue;
    }
}