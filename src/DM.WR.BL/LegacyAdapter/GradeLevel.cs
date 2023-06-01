using System;

namespace IRM_Library.Objects
{
    [Serializable]
    public class GradeLevel
    {
        public int Level { get; set; }
        public string Grade { get; set; }
        public string GradeText { get; set; }
        public string Battery { get; set; }
    }

    [Serializable]
    public class Grade
    {
        public string GradeId { get; set; }
        public string GradeText { get; set; }
    }
}
