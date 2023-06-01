using DM.WR.BL.Builders;
using Xunit;

namespace DM.WR.BL.Tests.Builders
{
    public class BuilderHelperTests
    {
        [Fact]
        public void PileLabel_GenderShouldResolveToGender()
        {
            var buildersHelper = new BuildersHelper();

            var result = buildersHelper.CreatePopulationFiltersPileLabel("Gender");

            Assert.Equal("Gender", result);
        }

        [Fact]
        public void PileLabel_FederalRaceEthnicityShouldResolveToEthnicity()
        {
            var buildersHelper = new BuildersHelper();

            var result = buildersHelper.CreatePopulationFiltersPileLabel("Federal Race/Ethnicity");

            Assert.Equal("Ethnicity", result);
        }

        [Fact]
        public void PileLabel_ProgramsShouldResolveToProgram()
        {
            var buildersHelper = new BuildersHelper();

            var result = buildersHelper.CreatePopulationFiltersPileLabel("Programs");

            Assert.Equal("Program", result);
        }

        [Fact]
        public void PileLabel_AdministratorCodesShouldResolveToAdminValue()
        {
            var buildersHelper = new BuildersHelper();

            var result = buildersHelper.CreatePopulationFiltersPileLabel("Administrator Codes");

            Assert.Equal("Admin Value", result);
        }

        [Fact]
        public void PileLabel_SomeValueShouldResolveToEmptyString()
        {
            var buildersHelper = new BuildersHelper();

            var result = buildersHelper.CreatePopulationFiltersPileLabel("Hello boys and girls!");

            Assert.Equal(string.Empty, result);
        }
    }
}
