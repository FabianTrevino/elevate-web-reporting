using System;

namespace IRM_Library.Objects
{
    [Serializable]
    public class Student
    {
        public Student()
        {
            Id = "-99";
            Name = "NONE";
            TestInstanceId = "-99";
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string TestInstanceId { get; set; }
    }
}
