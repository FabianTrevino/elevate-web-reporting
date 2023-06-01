using DM.WR.Data.Config;
using DM.WR.Data.Logging;
using DM.WR.Data.Logging.Types;
using DM.WR.Data.Repository.Types;
using HandyStuff;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DM.WR.Data.Repository
{
    public interface IDbClient
    {
        DbCustomerInfo GetCustomerInfo(string guid, string testFamilies);
        List<DbAssessment> GetAllAssessments(DbCustomerInfo custInfo, string testFamilies);
        List<Location> GetChildLocations(int nodeId, string nodeType, DbCustomerInfo custInfo, int assessmentId, int gradeLevelId, int testAdminId, int custScoresetId, bool prependAll);
        List<Location> GetChildLocations_Paper(int nodeId, string nodeType, DbCustomerInfo custInfo, int assessmentId, List<Grade> grades, int testAdminId, int custScoresetId, bool prependAll);
        List<Location> GetNodeHierarchy(int customerScoresetId, int nodeId, string nodeType, int assessmentId, int gradeLevelId);
        List<DbContentScope> GetContentArea(int nodeId, string nodeType, int assessmentId, int testAdminId, int gradeLevel, int custScoresetId, int excludeMathComputeFlag, int excludeWaListening, string subtestGroupType, string scoreType = "NPR");
        List<DbContentScope> GetContentArea_WordSkills(DbCustomerInfo custInfo, int assessmentId, int gradeLevel, int custScoresetId, int wordSkillSetId);
        List<DbContentScope> GetContentArea_Paper(DbCustomerInfo custInfo, int assessmentId, int testAdminId, List<Grade> grades, int custScoresetId, int excludeMathComputeFlag, int excludeWaListening, string subtestGroupType, string scoreType = "NPR");
        string GetCustomerDisplayOptionsString(int customerScoresetId);
        Dictionary<string, List<Disaggregation>> GetDisaggregation(int groupsetId, string nodeIds, string nodeType, int assessmentId, int testAdminId, int gradeLevelId, int custScoresetId);
        Dictionary<string, List<Disaggregation>> GetDisaggregation_Paper(int groupsetId, string nodeIds, string nodeType, int assessmentId, int testAdminId, List<Grade> grades, int custScoresetId, DbCustomerInfo custInfo);
        string GetFormsForMultimeasureColumn(int customerScoresetId, int nodeId, string nodeType, int testFamilyGroupId, int testGradeLevelId);
        List<GradeLevel> GetGradeLevels(int nodeId, string nodeType, int assessmentId, int testAdminId, int custScoresetId, string reportCode = null);
        List<Grade> GetGrades_Paper(DbCustomerInfo custInfo, int assessmentId, int testAdminId, int custScoresetId);
        List<LongTestAdminScoreset> GetGroupLongTestAdminScoresets(int customerScoresetId, int nodeId, string nodeType, string testFamilyGroupCodes, int testGradeLevelId, string longitudinalType, string battery, int skillReportFlag, string splitDate, int basReportFlag, string testFamilyGroupCode, bool isCovid);
        List<GroupPopulation> GetGroupPopulation(int customerScoresetId, int nodeId, string nodeType);
        List<GrowthEndPoint> GetGrowthEndPoints(int customerScoresetId, int testGradeLevelId);
        List<CompositeType> GetNodeCompositeTypes_Paper(DbCustomerInfo custInfo, int assessmentId, int testAdminId, List<Grade> grades, int custScoresetId, string subtestGroupType);
        List<SubtestFamily> GetNodeSubtestFamilies_Paper(DbCustomerInfo custInfo, int assessmentId, int testAdminId, List<Grade> grades, int custScoresetId, int excludeMathComputeFlag, int excludeWaListening, string subtestGroupType);
        DbScoringOptions GetCustomerScoringOptions(int custScoresetId, int testFamilyGroupId);
        List<DbScoreWarning> GetScoreWarnings(int customerScoresetId, string nodeIds, string nodeType, int testFamilyGroupId, int testGradeLevelId, string subtestAcronyms, int accountabilityFlag);
        List<Location> GetStudentLocationHierarchy(int customerScoresetId, string guid, int testInstanceId, int accountabilityFlag);
        List<LongTestAdminScoreset> GetStudentLongTestAdminScoresets(int customerScoresetId, int testInstanceId, string testFamilyGroupCodes, string battery, int skillReportFlag, int accountabilityFlag, string testFamilyGroupCode, bool isCovid);
        List<LongTestAdminScoreset> GetStudentLongTestAdminScoresetsAll(int customerScoresetId, string nodeType, int nodeId, string testFamilyGroupCodes, int gradeLevelId, string battery, int skillReportFlag, string splitDate, int basReportFlag, int accountabilityFlag, string testFamilyGroupCode, bool isCovid);
        List<Student> GetStudents(int custScoresetId, string nodeIds, string nodeType, int assessmentId, int gradeLevelId, int accountabilityFlag);
        List<SubContentArea> GetSubContentArea(DbCustomerInfo custInfo, int assessmentId, int testAdminId, int skillSetId, int gradeLevel, int custScoresetId, string subtestGroupType, string contentAreaList, int excludeMathComputeFlag, int excludeWordAnalysisListening, string reportCode);
        List<Disaggregation> GetSubgroupDisaggregation(List<int> nodeIds, string nodeType, int assessmentId, int gradeLevelId, int customerScoresetId, int displayNone);
        List<NodeLevel> GetNodeLevels(int customerScoresetId, int nodeLevel);
        List<SkillSet> GetSkillSets(string customerId, int testAdmin, int? gradeLevel, string reportCode);
        List<TestAdmin> GetTestAdmins(int customerId, string nodeId, string nodeType, string nodeName, int testFamilyGroupId, string testFamilyGroupCode, string testFamilies);
        TestAdminScoreset GetTestAdminScoreset(int customerId, int nodeId, string nodeType, int assessmentId, int testAdminId);
        SuppressedDimensions GetDimensionsToSuppress(int scoresetId, string list, string v);
    }

    public class DbClient : IDbClient
    {
        private readonly IDbLogger _dbLogger;
        private readonly IDataAccessHelpers _helpers;

        public DbClient(IDbLogger dbLogger, IDataAccessHelpers helpers)
        {
            _dbLogger = dbLogger;
            _helpers = helpers;
        }

        public DbCustomerInfo GetCustomerInfo(string guid, string testFamilies)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_LOGIN_ROOT_NODE"), con))
            {
                try
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("IN_NODE_REFERENCE", OracleDbType.Varchar2, 40)).Value = guid;
                    cmd.Parameters.Add(new OracleParameter("IN_ORIG_TESTADMIN_REF_LIST", OracleDbType.Varchar2, 4000)).Value = testFamilies;
                    cmd.Parameters.Add(new OracleParameter("OUT_NODE_TYPE", OracleDbType.Varchar2, 8)).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new OracleParameter("OUT_NODE_LEVEL", OracleDbType.Decimal)).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new OracleParameter("OUT_TOTAL_LEVELS", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new OracleParameter("OUT_NODE_ID", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new OracleParameter("OUT_NODE_NAME", OracleDbType.Varchar2, 30)).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new OracleParameter("OUT_SOURCE_CUSTOMER_ID", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new OracleParameter("OUT_CUSTOMER_ID", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    return new DbCustomerInfo
                    {
                        NodeType = cmd.Parameters["OUT_NODE_TYPE"].Value.ToString(),
                        NodeId = ((OracleDecimal)cmd.Parameters["OUT_NODE_ID"].Value).ToString(),
                        CustomerIdMorse = ((OracleDecimal)cmd.Parameters["OUT_SOURCE_CUSTOMER_ID"].Value).ToInt32(),
                        CustomerId = ((OracleDecimal)cmd.Parameters["OUT_CUSTOMER_ID"].Value).ToString(),
                        NodeName = cmd.Parameters["OUT_NODE_NAME"].Value.ToString(),
                        TotalLevels = ((OracleDecimal)cmd.Parameters["OUT_TOTAL_LEVELS"].Value).ToInt32(),
                        Guid = guid,
                        NodeLevel = ((OracleDecimal)cmd.Parameters["OUT_NODE_LEVEL"].Value).ToInt32()
                    };
                }
                catch (Exception exc)
                {
                    var fancyMessage = _helpers.BuildFancyErrorMessage(cmd, "GetCustomerInfo", exc.Message);
                    _dbLogger.LogMessageToDb(null, guid, DbLogType.Error, fancyMessage);

                    return null;
                }
            }
        }

        public List<DbAssessment> GetAllAssessments(DbCustomerInfo customerInfo, string testFamilies)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_TESTFAMILYGROUPS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMER_ID", OracleDbType.Decimal)).Value = customerInfo.CustomerId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = customerInfo.NodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = customerInfo.NodeId;
                cmd.Parameters.Add(new OracleParameter("IN_ORIG_TESTADMIN_REF_LIST", OracleDbType.Varchar2, 4000)).Value = testFamilies;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetAllAssessments");

                    var result = new List<DbAssessment>();
                    while (odr.Read())
                    {
                        result.Add(new DbAssessment
                        {
                            TestFamilyGroupId = Convert.ToInt32(odr["TESTFAMILYGROUP_ID"]),
                            TestFamilyName = odr["LIST_OF_TESTFAMILIES"].ToString(),
                            TestFamilyDesc = odr["TESTFAMILYGROUP_DESC"].ToString(),
                            TestFamilyGroupCode = odr["TESTFAMILYGROUP_CODE"].ToString(),
                            SmVersion = odr["SCORE_MANAGER_VERSION"].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        public List<TestAdmin> GetTestAdmins(int customerId, string nodeId, string nodeType, string nodeName, int testFamilyGroupId, string testFamilyGroupCode, string testFamilies)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_TESTADMINS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMER_ID", OracleDbType.Decimal)).Value = customerId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_MENU_TESTFAMILYGROUP_CODE", OracleDbType.Varchar2)).Value = testFamilyGroupCode;
                cmd.Parameters.Add(new OracleParameter("IN_ORIG_TESTADMIN_REF_LIST", OracleDbType.Varchar2)).Value = testFamilies;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetTestAdmins");

                    var result = new List<TestAdmin>();
                    while (odr.Read())
                    {
                        result.Add(new TestAdmin
                        {
                            Name = odr["TESTADMIN_NAME"].ToString(),
                            Date = odr["TEST_DATE"].ToString(),
                            Id = Convert.ToInt32(odr["TESTADMIN_ID"]),
                            NodeId = nodeId,
                            NodeType = nodeType,
                            NodeName = nodeName,
                            //CustomerId = customerId,
                            AllowCovidReportFlag = odr["ALLOW_COVID_REPORT_FLAG"].ToBoolean()
                        });
                    }
                    return result;
                }
            }
        }

        public TestAdminScoreset GetTestAdminScoreset(int customerId, int nodeId, string nodeType, int testFamilyGroupId, int testAdminId)
        {
            using (OracleConnection conn = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_SCORESETS"), conn))
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMER_ID", OracleDbType.Decimal)).Value = customerId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTADMIN_ID", OracleDbType.Decimal)).Value = testAdminId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetTestAdminScoreset");

                    while (odr.Read())
                    {
                        return new TestAdminScoreset
                        {
                            ScoresetId = Convert.ToInt32(odr["SCORESET_ID"].ToString()),
                            CustScoresetId = Convert.ToInt32(odr["CUSTOMERSCORESET_ID"].ToString()),
                            ProgramId = Convert.ToInt32(odr["PROGRAM_ID"].ToString())
                        };
                    }
                    return null;
                }
            }
        }

        public List<GrowthEndPoint> GetGrowthEndPoints(int customerScoresetId, int testGradeLevelId)
        {
            using (OracleConnection conn = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_LONG_GROWTH_POINTS"), conn))
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Int32)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Int32)).Value = testGradeLevelId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetGrowthEndPoints");

                    var result = new List<GrowthEndPoint>();
                    while (odr.Read())
                    {
                        result.Add(new GrowthEndPoint
                        {
                            ExpectedGrowthDesc = odr["EXPECTED_GROWTH_DESC"].ToString(),
                            ExpectedGrowthParameters = odr["EXPECTED_GROWTH_PARAMETERS"].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        public List<GroupPopulation> GetGroupPopulation(int customerScoresetId, int nodeId, string nodeType)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_GROUP_POPULATION"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetGroupPopulation");

                    var result = new List<GroupPopulation>();
                    while (odr.Read())
                    {
                        result.Add(new GroupPopulation
                        {
                            NodeRank = odr["NODE_RANK"].ToString(),
                            NodeType = odr["NODE_TYPE"].ToString(),
                            NodeValue = odr["PARAMETER_VALUE"].ToString(),
                            IsDefault = Convert.ToBoolean(odr["DEFAULT_FLAG"])
                        });
                    }
                    return result;
                }
            }
        }

        public List<SkillSet> GetSkillSets(string customerId, int testAdmin, int? gradeLevel, string reportCode)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_CUSTOMER_SKILLSETS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_ORIG_CUSTOMER_REFERENCE", OracleDbType.Varchar2, 30)).Value = customerId;
                cmd.Parameters.Add(new OracleParameter("IN_ORIG_TESTADMIN_REFERENCE", OracleDbType.Varchar2, 30)).Value = testAdmin;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevel;
                cmd.Parameters.Add(new OracleParameter("IN_REPORT_CODE", OracleDbType.Varchar2, 6)).Value = reportCode;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetSkillSets");

                    var result = new List<SkillSet>();
                    while (odr.Read())
                    {
                        result.Add(new SkillSet
                        {
                            Id = odr["SKILLSET_ID"].ToString(),
                            Description = odr["SKILLSET_DESC"].ToString(),
                            IsDefaultSkill = odr["DEFAULT_FLAG"].ToBoolean(),
                            SubtestGroupType = odr["SUBTESTGROUP_TYPE"].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        public List<GradeLevel> GetGradeLevels(int nodeId, string nodeType, int testFamilyGroupId, int testAdminId, int custScoresetId, string reportCode = null)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_GRADES_LEVELS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_REPORTCODE", OracleDbType.Varchar2, 8)).Value = reportCode;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetGradeLevels");

                    var result = new List<GradeLevel>();
                    while (odr.Read())
                    {
                        result.Add(new GradeLevel
                        {
                            Level = Convert.ToInt32(odr["TESTGRADELEVEL_ID"]),
                            Grade = odr["VISUAL_LEVEL"].ToString(),
                            GradeText = odr["DISPLAY_VALUE"].ToString(),
                            Battery = odr["BATTERY"].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        public List<Grade> GetGrades_Paper(DbCustomerInfo custInfo, int testFamilyGroupId, int testAdminId, int custScoresetId)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("BATCH_GET_NODE_GRADES"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = custInfo.NodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = custInfo.NodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetGrades_Paper");

                    var result = new List<Grade>();
                    while (odr.Read())
                    {
                        result.Add(new Grade
                        {
                            GradeId = odr["GRADE"].ToString(),
                            GradeText = odr["GRADE_DESC"].ToString(),
                        });
                    }
                    return result;
                }
            }
        }

        public List<DbContentScope> GetContentArea(int nodeId, string nodeType, int assessmentId, int testAdminId, int gradeLevel, int custScoresetId, int excludeMathComputeFlag, int excludeWaListening, string subtestGroupType, string scoreType = "NPR")
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_CONTENT_AREAS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 8)).Value = nodeId.ToString();
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevel;
                cmd.Parameters.Add(new OracleParameter("IN_EXCLUDE_MATHCOMP_FLAG", OracleDbType.Decimal)).Value = excludeMathComputeFlag;
                cmd.Parameters.Add(new OracleParameter("IN_EXCLUDE_WANA_LISTEN_FLAG", OracleDbType.Decimal)).Value = excludeWaListening;
                cmd.Parameters.Add(new OracleParameter("IN_SUBTESTGROUP_TYPE", OracleDbType.Varchar2, 8)).Value = subtestGroupType;
                cmd.Parameters.Add(new OracleParameter("IN_SCORE_TYPE", OracleDbType.Varchar2, 8)).Value = scoreType;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetContentArea");

                    var result = new List<DbContentScope>();
                    while (odr.Read())
                    {
                        result.Add(new DbContentScope
                        {
                            Acronym = odr["ACRONYM"].ToString(),
                            Battery = odr["BATTERY"].ToString(),
                            SubtestName = odr["SUBTEST_NAME"].ToString(),
                            IsDefault = odr["DEFAULT_SELECTED_FLAG"].ToBoolean()
                        });
                    }
                    return result;
                }
            }
        }

        public List<DbContentScope> GetContentArea_WordSkills(DbCustomerInfo custInfo, int assessmentId, int gradeLevel, int custScoresetId, int wordSkillSetId)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_WORDSKILLS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = custInfo.NodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 8)).Value = custInfo.NodeId.ToString();
                cmd.Parameters.Add(new OracleParameter("IN_WORDSKILLSET_ID", OracleDbType.Decimal)).Value = wordSkillSetId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevel;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    var result = new List<DbContentScope>();
                    while (odr.Read())
                    {
                        result.Add(new DbContentScope
                        {
                            Acronym = odr["wordskill_name_param"].ToString(),
                            Battery = "",
                            SubtestName = odr["wordskill_name"].ToString(),
                            IsDefault = true
                        });
                    }
                    return result;
                }
            }
        }

        public List<DbContentScope> GetContentArea_Paper(DbCustomerInfo custInfo, int assessmentId, int testAdminId, List<Grade> grades, int custScoresetId, int excludeMathComputeFlag, int excludeWaListening, string subtestGroupType, string scoreType = "NPR")
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("batch_get_node_content_areas"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = custInfo.NodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 8)).Value = custInfo.NodeId.ToString();
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_GRADE_LIST", OracleDbType.Varchar2)).Value = string.Join(",", grades.ConvertAll(e => e.GradeId));
                cmd.Parameters.Add(new OracleParameter("IN_EXCLUDE_MATHCOMP_FLAG", OracleDbType.Decimal)).Value = excludeMathComputeFlag;
                cmd.Parameters.Add(new OracleParameter("IN_EXCLUDE_WANA_LISTEN_FLAG", OracleDbType.Decimal)).Value = excludeWaListening;
                cmd.Parameters.Add(new OracleParameter("IN_SUBTESTGROUP_TYPE", OracleDbType.Varchar2, 8)).Value = subtestGroupType;
                cmd.Parameters.Add(new OracleParameter("IN_SCORE_TYPE", OracleDbType.Varchar2, 8)).Value = scoreType;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetContentArea_Paper");

                    var result = new List<DbContentScope>();
                    while (odr.Read())
                    {
                        result.Add(new DbContentScope
                        {
                            Acronym = odr["ACRONYM"].ToString(),
                            Battery = odr["BATTERY"].ToString(),
                            SubtestName = odr["SUBTEST_NAME"].ToString(),
                            IsDefault = odr["DEFAULT_SELECTED_FLAG"].ToBoolean()
                        });
                    }
                    return result;
                }
            }
        }

        public List<SubtestFamily> GetNodeSubtestFamilies_Paper(DbCustomerInfo custInfo, int assessmentId, int testAdminId, List<Grade> grades, int custScoresetId, int excludeMathComputeFlag, int excludeWaListening, string subtestGroupType)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("batch_get_node_subtestfamilies"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = custInfo.NodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Varchar2, 8)).Value = custInfo.NodeId.ToString();
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_GRADE_LIST", OracleDbType.Varchar2)).Value = string.Join(",", grades.ConvertAll(e => e.GradeId));
                cmd.Parameters.Add(new OracleParameter("IN_EXCLUDE_MATHCOMP_FLAG", OracleDbType.Decimal)).Value = excludeMathComputeFlag;
                cmd.Parameters.Add(new OracleParameter("IN_EXCLUDE_WANA_LISTEN_FLAG", OracleDbType.Decimal)).Value = excludeWaListening;
                cmd.Parameters.Add(new OracleParameter("IN_SUBTESTGROUP_TYPE", OracleDbType.Varchar2, 8)).Value = subtestGroupType;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetNodeSubtestFamilies_Paper");

                    var result = new List<SubtestFamily>();
                    while (odr.Read())
                    {
                        result.Add(new SubtestFamily
                        {
                            SubtestFamilyId = odr["SUBTESTFAMILY_ID"].ToString(),
                            SubtestName = odr["SUBTEST_NAME"].ToString(),
                            SequenceNo = odr["SEQUENCE_NO"].ToString(),
                            DefaultSelectedFlag = odr["DEFAULT_SELECTED_FLAG"].ToBoolean()
                        });
                    }
                    return result;
                }
            }
        }

        public List<CompositeType> GetNodeCompositeTypes_Paper(DbCustomerInfo custInfo, int assessmentId, int testAdminId, List<Grade> grades, int custScoresetId, string subtestGroupType)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("BATCH_GET_NODE_COG_COMPOSITES"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = custInfo.NodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 8)).Value = custInfo.NodeId.ToString();
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_GRADE_LIST", OracleDbType.Varchar2)).Value = string.Join(",", grades.ConvertAll(e => e.GradeId));
                cmd.Parameters.Add(new OracleParameter("IN_SUBTESTGROUP_TYPE", OracleDbType.Varchar2, 8)).Value = subtestGroupType;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetNodeCompositeTypes_Paper");

                    var result = new List<CompositeType>();
                    while (odr.Read())
                    {
                        result.Add(new CompositeType
                        {
                            Acronym = odr["COGAT_COMPOSITE"].ToString(),
                            SubtestName = odr["SUBTEST_NAME"].ToString(),
                            SequenceNo = odr["SEQUENCE_NO"].ToString(),
                            DefaultSelectedFlag = odr["DEFAULT_SELECTED_FLAG"].ToBoolean()
                        });
                    }
                    return result;
                }
            }
        }

        public List<SubContentArea> GetSubContentArea(DbCustomerInfo custInfo, int assessmentId, int testAdminId, int skillSetId, int gradeLevel, int custScoresetId, string subtestGroupType, string contentAreaList, int excludeMathComputeFlag, int excludeWordAnalysisListening, string reportCode)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_SKILLS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = custInfo.NodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 8)).Value = custInfo.NodeId.ToString();
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_SKILLSET_ID", OracleDbType.Decimal)).Value = skillSetId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevel;
                cmd.Parameters.Add(new OracleParameter("IN_EXCLUDE_MATHCOMP_FLAG", OracleDbType.Decimal)).Value = excludeMathComputeFlag;
                cmd.Parameters.Add(new OracleParameter("IN_EXCLUDE_WANA_LISTEN_FLAG", OracleDbType.Decimal)).Value = excludeWordAnalysisListening;
                cmd.Parameters.Add(new OracleParameter("IN_SUBTESTGROUP_TYPE", OracleDbType.Varchar2, 8)).Value = subtestGroupType;
                cmd.Parameters.Add(new OracleParameter("IN_SUBTESTACRONYMS", OracleDbType.Varchar2, 255)).Value = contentAreaList;
                cmd.Parameters.Add(new OracleParameter("IN_REPORT_CODE", OracleDbType.Varchar2, 6)).Value = reportCode;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    if (!odr.HasRows)
                        return new List<SubContentArea> { new SubContentArea() };

                    var result = new List<SubContentArea>();
                    while (odr.Read())
                    {
                        result.Add(new SubContentArea
                        {
                            Acronym = odr["ACRONYM"].ToString(),
                            SkillId = odr["EXTERNAL_SKILL_ID"].ToString(),
                            SkillName = odr["SKILL_DESC"].ToString(),
                            ParentFlag = odr["PARENT_SKILL_FLAG"].ToString(),
                            CognitiveSkillFlag = odr["COGNITIVE_SKILL_FLAG"].ToString(),
                            IsSelectedByDefault = true
                        });
                    }
                    return result;
                }
            }
        }

        public List<Student> GetStudents(int custScoresetId, string nodeIds, string nodeType, int assessmentId, int gradeLevelId, int accountabilityFlag)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_STUDENTS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 5000)).Value = nodeIds;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevelId;
                cmd.Parameters.Add(new OracleParameter("IN_ACCOUNTABILITY_FLAG", OracleDbType.Decimal)).Value = accountabilityFlag;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    if (!odr.HasRows)
                        return new List<Student> { new Student() };

                    var result = new List<Student>();
                    while (odr.Read())
                    {
                        result.Add(new Student
                        {
                            Id = odr["STUDENT_ID"].ToString(), // This is used for student longitudinal reports (for future reports)
                            Name = $"{odr["LAST_NAME"]}, {odr["FIRST_NAME"]}", //display name
                            TestInstanceId = odr["TESTINSTANCE_ID"].ToString() //value which gets passed to the report
                        });
                    }
                    return result;
                }
            }
        }

        public DbScoringOptions GetCustomerScoringOptions(int customerScoresetId, int testFamilyGroupId)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_CUSTOMER_SCORING_OPTIONS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("OUT_SKILLSET_ID", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_WORDSKILLSET_ID", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_GROUPSET_ID", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_GROUPSET_CODE", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_LPR_NODE_LEVEL", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_LPR_NODE_LIST", OracleDbType.Varchar2, 1024)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_ACCOUNTABILITY_FLAG", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_ALLOW_LEXILE_FLAG", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_ALLOW_LPR_FLAG", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_ALLOW_LS_FLAG", OracleDbType.Decimal)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_ALLOW_CATHPRIV_FLAG", OracleDbType.Decimal)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_ALLOW_SCHPR_FLAG", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_ALLOW_05NORMS_FLAG", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_EXCLUDE_OUZ_FLAG", OracleDbType.Decimal)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_ADMINISTRATION_TYPE", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_EXCLUDE_MATHCOMP_DEFAULT", OracleDbType.Decimal)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_EXCLUDE_WANA_LISTEN_DEF", OracleDbType.Decimal)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_PRED_SUBTESTGROUP_TYPE", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_SUBTEST_CUTSCOREFAMILY_ID", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_LONGITUDINAL_FLAG", OracleDbType.Decimal)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_PREDICTED_SUBTEST_ACRONYM", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_COGAT_DIFFERENCES", OracleDbType.Decimal)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_GPD_GROUPSET_CODE", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_CCORE_SUBTESTGROUP_TYPE", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_CCORE_SKILLSET_ID", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_SPLIT_MMDD", OracleDbType.Varchar2, 20)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_HAS_ITEMS_FLAG", OracleDbType.Decimal, 22)).Direction = ParameterDirection.Output;
                if (ConfigSettings.IsWebReportingLiteFeatureEnabled)
                    cmd.Parameters.Add(new OracleParameter("OUT_PRINTONLY_REPORTCODES", OracleDbType.Varchar2, 200)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_REPORT_FORMAT", OracleDbType.Varchar2, 25)).Direction = ParameterDirection.Output;
                using (cmd.ExecuteReader())
                {
                    var alternativeNormYearValue = ((OracleDecimal)cmd.Parameters["OUT_ALLOW_05NORMS_FLAG"].Value).IsNull ?
                                                    null :
                                                    cmd.Parameters["OUT_ALLOW_05NORMS_FLAG"].Value.ToString();
                    var displayAlternativeScores = alternativeNormYearValue != null;

                    return new DbScoringOptions
                    {
                        //TODO:  Convert the following line to an extension (look at ToBoolean)
                        SkillSetId = ((OracleDecimal)cmd.Parameters["OUT_SKILLSET_ID"].Value).IsNull ? 0 : int.Parse(cmd.Parameters["OUT_SKILLSET_ID"].Value.ToString()),
                        WordskillSetId = int.Parse(cmd.Parameters["OUT_WORDSKILLSET_ID"].Value.ToString()),
                        GroupsetId = int.Parse(cmd.Parameters["OUT_GROUPSET_ID"].Value.ToString()),
                        GroupsetCode = cmd.Parameters["OUT_GROUPSET_CODE"].Value.ToString(),
                        GpdGroupsetCode = cmd.Parameters["OUT_GPD_GROUPSET_CODE"].Value.ToString(),
                        DefaultAdmz = int.Parse(cmd.Parameters["OUT_EXCLUDE_OUZ_FLAG"].Value.ToString()),
                        LprNodeLevel = cmd.Parameters["OUT_LPR_NODE_LEVEL"].Value.ToString(),
                        LprNodeList = cmd.Parameters["OUT_LPR_NODE_LIST"].Value.ToString(),
                        TestType = cmd.Parameters["OUT_ADMINISTRATION_TYPE"].Value.ToString(),
                        AllowLexileScore = int.Parse(cmd.Parameters["OUT_ALLOW_LEXILE_FLAG"].Value.ToString()),
                        AllowLprScore = int.Parse(cmd.Parameters["OUT_ALLOW_LPR_FLAG"].Value.ToString()),
                        AllowCathprivFlag = int.Parse(cmd.Parameters["OUT_ALLOW_CATHPRIV_FLAG"].Value.ToString()),
                        ExcludeMathcompDefault = int.Parse(cmd.Parameters["OUT_EXCLUDE_MATHCOMP_DEFAULT"].Value.ToString()),
                        PredSubtestGroupType = cmd.Parameters["OUT_PRED_SUBTESTGROUP_TYPE"].Value.ToString(),
                        SubtestCutscoreFamilyId = cmd.Parameters["OUT_SUBTEST_CUTSCOREFAMILY_ID"].Value.ToString(),
                        LongitudinalFlag = int.Parse(cmd.Parameters["OUT_LONGITUDINAL_FLAG"].Value.ToString()),
                        DefaultCogatDiff = cmd.Parameters["OUT_COGAT_DIFFERENCES"].Value.ToString(),
                        PredictedSubtestAcronym = cmd.Parameters["OUT_PREDICTED_SUBTEST_ACRONYM"].Value.ToString(),
                        CcoreSkillsetId = 0,
                        CcoreSubtestGroupType = cmd.Parameters["OUT_CCORE_SUBTESTGROUP_TYPE"].Value.ToString(),
                        ElaTotal = int.Parse(cmd.Parameters["OUT_EXCLUDE_WANA_LISTEN_DEF"].Value.ToString()),
                        AccountabilityFlag = int.Parse(cmd.Parameters["OUT_ACCOUNTABILITY_FLAG"].Value.ToString()),
                        AlternativeNormYear = alternativeNormYearValue,
                        SplitDate = cmd.Parameters["OUT_SPLIT_MMDD"].Value.ToString(),
                        HasItemsFlag = cmd.Parameters["OUT_HAS_ITEMS_FLAG"].Value.ToBoolean(),
                        PrintOnlyReports = ConfigSettings.IsWebReportingLiteFeatureEnabled ?
                            ((OracleString)cmd.Parameters["OUT_PRINTONLY_REPORTCODES"].Value).IsNull ?
                                "" :
                               cmd.Parameters["OUT_PRINTONLY_REPORTCODES"].Value.ToString() : "",
                       RFormat = cmd.Parameters["OUT_REPORT_FORMAT"].Value.ToString(),
                        DisplayFlags = new Dictionary<string, bool> {
                            { "AltSS", displayAlternativeScores },
                            { "AltPR1", displayAlternativeScores },
                            { "AltST1", displayAlternativeScores },
                            { "AltGE", displayAlternativeScores },
                            { "LEXILE", cmd.Parameters["OUT_ALLOW_LEXILE_FLAG"].Value.ToBoolean() },
                            { "LPR", cmd.Parameters["OUT_ALLOW_LPR_FLAG"].Value.ToBoolean() },
                            { "LS", cmd.Parameters["OUT_ALLOW_LS_FLAG"].Value.ToBoolean() },
                            { "PRIV", cmd.Parameters["OUT_ALLOW_CATHPRIV_FLAG"].Value.ToBoolean() },
                            { "SCHPR", cmd.Parameters["OUT_ALLOW_SCHPR_FLAG"].Value.ToBoolean() },
                        }
                    };
                }
            }
        }

        public Dictionary<string, List<Disaggregation>> GetDisaggregation(int groupsetId, string nodeIds, string nodeType, int assessmentId, int testAdminId, int gradeLevelId, int custScoresetId)
        {
            SuppressedDimensions suppressedDimensions = GetDimensionsToSuppress(custScoresetId, nodeIds, nodeType);

            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_DIMENSION_GROUPS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 32000)).Value = nodeIds;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevelId;
                cmd.Parameters.Add(new OracleParameter("IN_GROUPSET_ID", OracleDbType.Decimal)).Value = groupsetId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetDisAggregation: Dimension Groups");

                    var result = new Dictionary<string, List<Disaggregation>>();
                    while (odr.Read())
                    {
                        var groupDescription = odr["GROUP_DESC"].ToString();
                        var dimensionGroupId = Convert.ToInt32(odr["DIMENSIONGROUP_ID"]);
                        var nodeDimensions = GetNodeDimensions(custScoresetId, nodeType, nodeIds, assessmentId, dimensionGroupId,
                                                               gradeLevelId, groupsetId, suppressedDimensions.SuppressionSql);

                        result[groupDescription] = nodeDimensions;
                    }
                    return result;
                }
            }
        }

        public Dictionary<string, List<Disaggregation>> GetDisaggregation_Paper(int groupsetId, string nodeIds, string nodeType, int assessmentId, int testAdminId, List<Grade> grades, int custScoresetId, DbCustomerInfo custInfo)
        {
            SuppressedDimensions suppressedDimensions = GetDimensionsToSuppress(custScoresetId, nodeIds, nodeType);

            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("BATCH_GET_NODE_DIMENSION_GRPS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = custScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 4000)).Value = nodeIds;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_GRADE_LIST", OracleDbType.Varchar2)).Value = string.Join(",", grades.ConvertAll(e => e.GradeId));
                cmd.Parameters.Add(new OracleParameter("IN_GROUPSET_ID", OracleDbType.Decimal)).Value = groupsetId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetDisAggregation_Paper: Dimension Groups");

                    var disaggregationDict = new Dictionary<string, List<Disaggregation>>();
                    while (odr.Read())
                    {
                        var groupDescription = odr["GROUP_DESC"].ToString();
                        var dimensionGroupId = Convert.ToInt32(odr["DIMENSIONGROUP_ID"]);
                        var nodeDimensions = GetNodeDimensions_Paper(custScoresetId, nodeType, nodeIds, assessmentId, dimensionGroupId,
                                                                     grades, groupsetId, suppressedDimensions.SuppressionSql);

                        disaggregationDict[groupDescription] = nodeDimensions;
                    }
                    return disaggregationDict;
                }
            }
        }

        public List<Disaggregation> GetSubgroupDisaggregation(List<int> nodeIds, string nodeType, int assessmentId, int gradeLevelId, int customerScoresetId, int displayNone)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_SUBGROUPS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 4000)).Value = string.Join(",", nodeIds);
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevelId;
                cmd.Parameters.Add(new OracleParameter("IN_DISPLAY_NONE_SUBGROUP", OracleDbType.Decimal)).Value = displayNone;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetSubgroupDisaggregation");

                    var result = new List<Disaggregation>();
                    while (odr.Read())
                    {
                        result.Add(new Disaggregation
                        {
                            GroupValue = odr["groupset_code"].ToString(),
                            GroupLabel = odr["groupset_desc"].ToString(),
                            DefaultFlag = Convert.ToInt32(odr["DEFAULT_FLAG"])
                        });
                    }
                    return result;
                }
            }
        }

        public List<Location> GetChildLocations(int nodeId, string nodeType, DbCustomerInfo custInfo, int assessmentId, int gradeLevelId, int testAdminId, int customerScoresetId, bool prependAll)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_CHILDREN"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 4000)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevelId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetChildLocations");

                    var result = new List<Location>();
                    while (odr.Read())
                    {
                        result.Add(new Location
                        {
                            Id = Convert.ToInt32(odr["NODE_ID"]),
                            NodeType = odr["NODE_TYPE"].ToString(),
                            NodeName = odr["NODE_NAME"].ToString(),
                            Guid = odr["GUID"].ToString(),
                            NodeTypeDisplay = odr["NODE_TYPE_DISPLAY"].ToString()
                        });
                    }

                    if (result.Count > 0 && prependAll)
                        result.Insert(0, new Location
                        {
                            Id = -1,
                            NodeType = result[0].NodeType,
                            NodeName = "All",
                            Guid = "",
                            NodeTypeDisplay = result[0].NodeTypeDisplay
                        });

                    return result;
                }
            }
        }

        public List<Location> GetNodeHierarchy(int customerScoresetId, int nodeId, string nodeType, int assessmentId, int gradeLevelId)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_HIERARCHY"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 4000)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevelId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetNodeHierarchy");

                    var result = new List<Location>();
                    while (odr.Read())
                    {
                        result.Add(new Location
                        {
                            Id = Convert.ToInt32(odr["NODE_ID"]),
                            NodeType = odr["NODE_TYPE"].ToString(),
                            NodeName = odr["NODE_NAME"].ToString(),
                            Guid = odr["GUID"].ToString(),
                            ParentGuid = odr["PARENT_GUID"].ToString(),
                            NodeTypeDisplay = odr["NODE_TYPE_DISPLAY"].ToString()
                        });
                    }

                    return result;
                }
            }
        }

        public List<Location> GetChildLocations_Paper(int nodeId, string nodeType, DbCustomerInfo custInfo, int assessmentId, List<Grade> grades, int testAdminId, int customerScoresetId, bool prependAll)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("batch_get_node_children"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 4000)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = assessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_GRADE_LIST", OracleDbType.Varchar2)).Value = string.Join(",", grades.ConvertAll(e => e.GradeId));
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetChildLocations_Paper");

                    var result = new List<Location>();
                    while (odr.Read())
                    {
                        result.Add(new Location
                        {
                            Id = Convert.ToInt32(odr["NODE_ID"]),
                            NodeType = odr["NODE_TYPE"].ToString(),
                            NodeName = odr["NODE_NAME"].ToString(),
                            Guid = odr["GUID"].ToString(),
                            NodeTypeDisplay = odr["NODE_TYPE_DISPLAY"].ToString()
                        });
                    }

                    if (result.Count > 0 && prependAll)
                        result.Insert(0, new Location
                        {
                            Id = -1,
                            NodeType = result[0].NodeType,
                            NodeName = "All",
                            Guid = "",
                            NodeTypeDisplay = result[0].NodeTypeDisplay
                        });

                    return result;
                }
            }
        }

        public string GetCustomerDisplayOptionsString(int customerScoresetId)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_CUSTOMER_DISPLAY_OPTIONS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    var result = new StringBuilder();

                    while (odr.Read())
                        result.Append(odr["OPTION_STRING"]);

                    return result.ToString();
                }
            }
        }

        public List<LongTestAdminScoreset> GetGroupLongTestAdminScoresets(int customerScoresetId, int nodeId, string nodeType, string testFamilyGroupCodes, int testGradeLevelId, string longitudinalType, string battery, int skillReportFlag, string splitDate, int basReportFlag, string testFamilyGroupCode, bool isCovid)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_LONG_SCORESETS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_LONGITUDINAL_TYPE", OracleDbType.Varchar2, 8)).Value = longitudinalType;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_CODE_LIST", OracleDbType.Varchar2)).Value = testFamilyGroupCodes;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = testGradeLevelId;
                cmd.Parameters.Add(new OracleParameter("IN_INTERIMS_BATTERY", OracleDbType.Varchar2, 20)).Value = battery;
                cmd.Parameters.Add(new OracleParameter("IN_SKILL_REPORT_FLAG", OracleDbType.Decimal)).Value = skillReportFlag;
                cmd.Parameters.Add(new OracleParameter("IN_BAS_REPORT_FLAG", OracleDbType.Int32)).Value = basReportFlag;
                cmd.Parameters.Add(new OracleParameter("IN_SPLIT_MMDD", OracleDbType.Varchar2, 20)).Value = splitDate;
                cmd.Parameters.Add(new OracleParameter("IN_MENU_TESTFAMILYGROUP_CODE", OracleDbType.Varchar2, 20)).Value = testFamilyGroupCode;
                cmd.Parameters.Add(new OracleParameter("IN_COVID_REPORT_FLAG", OracleDbType.Decimal)).Value = isCovid ? 1 : 0;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetGroupLongTestAdminScoresets");

                    var result = new List<LongTestAdminScoreset>();
                    while (odr.Read())
                        result.Add(PopulateLongTestAdminScoreset(odr));

                    return result;
                }
            }
        }

        public List<LongTestAdminScoreset> GetStudentLongTestAdminScoresets(int customerScoresetId, int testInstanceId, string testFamilyGroupCodes, string battery, int skillReportFlag, int accountabilityFlag, string testFamilyGroupCode, bool isCovid)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_STUDENT_LONG_SCORESETS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTINSTANCE_ID", OracleDbType.Decimal)).Value = testInstanceId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_CODE_LIST", OracleDbType.Varchar2)).Value = testFamilyGroupCodes;
                cmd.Parameters.Add(new OracleParameter("IN_INTERIMS_BATTERY", OracleDbType.Varchar2, 20)).Value = battery;
                cmd.Parameters.Add(new OracleParameter("IN_SKILL_REPORT_FLAG", OracleDbType.Decimal)).Value = skillReportFlag;
                cmd.Parameters.Add(new OracleParameter("IN_ACCOUNTABILITY_FLAG", OracleDbType.Decimal)).Value = accountabilityFlag;
                cmd.Parameters.Add(new OracleParameter("IN_MENU_TESTFAMILYGROUP_CODE", OracleDbType.Varchar2, 20)).Value = testFamilyGroupCode;
                cmd.Parameters.Add(new OracleParameter("IN_COVID_REPORT_FLAG", OracleDbType.Decimal)).Value = isCovid ? 1 : 0;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetStudentLongTestAdminScoresets");

                    var result = new List<LongTestAdminScoreset>();
                    while (odr.Read())
                    {
                        result.Add(PopulateLongTestAdminScoreset(odr));
                    }
                    return result;
                }
            }
        }

        public List<LongTestAdminScoreset> GetStudentLongTestAdminScoresetsAll(int customerScoresetId, string nodeType, int nodeId, string testFamilyGroupCodes, int gradeLevelId, string battery, int skillReportFlag, string splitDate, int basReportFlag, int accountabilityFlag, string testFamilyGroupCode, bool isCovid)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_STUDENT_LONG_SCORESETS_ALL"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_CODE_LIST", OracleDbType.Varchar2)).Value = testFamilyGroupCodes;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = gradeLevelId;
                cmd.Parameters.Add(new OracleParameter("IN_INTERIMS_BATTERY", OracleDbType.Varchar2, 20)).Value = battery;
                cmd.Parameters.Add(new OracleParameter("IN_SKILL_REPORT_FLAG", OracleDbType.Decimal)).Value = skillReportFlag;
                cmd.Parameters.Add(new OracleParameter("IN_BAS_REPORT_FLAG", OracleDbType.Int32)).Value = basReportFlag;
                cmd.Parameters.Add(new OracleParameter("IN_SPLIT_MMDD", OracleDbType.Varchar2, 20)).Value = splitDate;
                cmd.Parameters.Add(new OracleParameter("IN_ACCOUNTABILITY_FLAG", OracleDbType.Decimal)).Value = accountabilityFlag;
                cmd.Parameters.Add(new OracleParameter("IN_MENU_TESTFAMILYGROUP_CODE", OracleDbType.Varchar2, 20)).Value = testFamilyGroupCode;
                cmd.Parameters.Add(new OracleParameter("IN_COVID_REPORT_FLAG", OracleDbType.Decimal)).Value = isCovid ? 1 : 0;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetStudentLongTestAdminScoresetsAll");

                    var result = new List<LongTestAdminScoreset>();
                    while (odr.Read())
                    {
                        result.Add(PopulateLongTestAdminScoreset(odr));
                    }
                    return result;
                }
            }
        }

        private LongTestAdminScoreset PopulateLongTestAdminScoreset(OracleDataReader odr)
        {
            return new LongTestAdminScoreset
            {
                TestAdminGradeLevelId = _helpers.ColumnExists(odr, "testadmingradelevel_id") ? odr["testadmingradelevel_id"].ToString() : "",
                TestGradeId = _helpers.ColumnExists(odr, "testadmingradelevel_id") ? odr["testadmingradelevel_id"].ToString() : "",
                IsDefaultTestGrade = _helpers.ColumnExists(odr, "default_flag") && odr["default_flag"].ToBoolean(),
                CustomerScoresetId = Convert.ToInt32(odr["customerscoreset_id"]),
                TestAdminId = Convert.ToInt32(odr["testadmin_id"]),
                ProgramId = Convert.ToInt32(odr["program_id"]),
                ScoresetId = Convert.ToInt32(odr["scoreset_id"]),
                ScoresetDesc = odr["scoreset_desc"].ToString(),
                TestDate = Convert.ToDateTime(odr["test_date"]),
                Battery = odr["battery"].ToString(),
            };
        }

        private List<Disaggregation> GetNodeDimensions(int customerScoresetId, string nodeType, string nodeIds, int testFamilyGroupId, int dimensionGroupId, int testGradeLevelId, int groupSetId, string suppressSql)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_DIMENSIONS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 32000)).Value = nodeIds;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = testGradeLevelId;
                cmd.Parameters.Add(new OracleParameter("IN_DIMENSIONGROUP_ID", OracleDbType.Decimal)).Value = dimensionGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_GROUPSET_ID", OracleDbType.Decimal)).Value = groupSetId;
                cmd.Parameters.Add(new OracleParameter("IN_SUPPRESSION_SQL", OracleDbType.Varchar2, 800)).Value = suppressSql;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetNodeDimensions");

                    var result = new List<Disaggregation>();
                    while (odr.Read())
                    {
                        result.Add(new Disaggregation
                        {
                            GroupKey = odr["DIMENSION_KEY"].ToString(),
                            GroupValue = odr["DISPLAY_VALUE"].ToString(),
                            GroupLabel = odr["RPT_DISAGG_LABEL"].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        private List<Disaggregation> GetNodeDimensions_Paper(int customerScoresetId, string nodeType, string nodeIds, int testFamilyGroupId, int dimensionGroupId, List<Grade> grades, int groupSetId, string suppressSql)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("BATCH_GET_NODE_DIMENSIONS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 4000)).Value = nodeIds;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_GRADE_LIST", OracleDbType.Varchar2)).Value = string.Join(",", grades.ConvertAll(e => e.GradeId));
                cmd.Parameters.Add(new OracleParameter("IN_DIMENSIONGROUP_ID", OracleDbType.Decimal)).Value = dimensionGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_GROUPSET_ID", OracleDbType.Decimal)).Value = groupSetId;
                cmd.Parameters.Add(new OracleParameter("IN_SUPPRESSION_SQL", OracleDbType.Varchar2, 800)).Value = suppressSql;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    _helpers.ValidateOracleDataReaderWithFancyMessage(odr, cmd, "GetNodeDimensions_Paper");

                    var result = new List<Disaggregation>();
                    while (odr.Read())
                    {
                        result.Add(new Disaggregation
                        {
                            GroupKey = odr["DIMENSION_KEY"].ToString(),
                            GroupValue = odr["DISPLAY_VALUE"].ToString(),
                            GroupLabel = odr["rpt_disagg_label"].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        public List<Location> GetStudentLocationHierarchy(int customerScoresetId, string guid, int testInstanceId, int accountabilityFlag)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_STUDENT_LOC_HIERARCHY"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_REFERENCE", OracleDbType.Varchar2, 40)).Value = guid;
                cmd.Parameters.Add(new OracleParameter("IN_TESTINSTANCE_ID", OracleDbType.Decimal)).Value = testInstanceId;
                cmd.Parameters.Add(new OracleParameter("IN_ACCOUNTABILITY_FLAG", OracleDbType.Decimal)).Value = accountabilityFlag;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    //Original did not validate

                    var result = new List<Location>();
                    while (odr.Read())
                    {
                        result.Add(new Location
                        {
                            Id = Convert.ToInt32(odr["NODE_ID"]),
                            NodeType = odr["NODE_TYPE"].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        public List<NodeLevel> GetNodeLevels(int customerScoresetId, int nodeLevel)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("BATCH_GET_NODE_LEVELS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Int32)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_LEVEL", OracleDbType.Int32)).Value = nodeLevel;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    //Original did not validate
                    //TODO: get values by column name, not index
                    var result = new List<NodeLevel>();
                    while (odr.Read())
                    {
                        result.Add(new NodeLevel
                        {
                            NodeType = odr[0].ToString(),
                            NodeDescription = odr[1].ToString(),
                            NodeRank = odr[2].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        public SuppressedDimensions GetDimensionsToSuppress(int customerScoresetId, string nodeIds, string nodeType)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_DIMENSIONS_TO_SUPPRESS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 25)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDs", OracleDbType.Varchar2, 4000)).Value = nodeIds;
                cmd.Parameters.Add(new OracleParameter("OUT_SUPPRESSION_SQL", OracleDbType.Varchar2, 800)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("OUT_SUPPRESS_PROGRAM_LABELS", OracleDbType.Varchar2, 100)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                return new SuppressedDimensions
                {
                    SuppressionSql = ((OracleString)cmd.Parameters["OUT_SUPPRESSION_SQL"].Value).IsNull ?
                                     "" :
                                     cmd.Parameters["OUT_SUPPRESSION_SQL"].Value.ToString(),
                    SuppressProgramLabels = ((OracleString)cmd.Parameters["OUT_SUPPRESS_PROGRAM_LABELS"].Value).IsNull ?
                                            "" : cmd.Parameters["OUT_SUPPRESS_PROGRAM_LABELS"].Value.ToString(),
                };
            }
        }

        public List<DbScoreWarning> GetScoreWarnings(int customerScoresetId, string nodeIds, string nodeType,
            int testFamilyGroupId, int testGradeLevelId, string subtestAcronyms, int accountabilityFlag)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_SCORE_WARNINGS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 8)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_IDS", OracleDbType.Varchar2, 6000)).Value = nodeIds;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = testGradeLevelId;
                cmd.Parameters.Add(new OracleParameter("IN_SUBTESTACRONYMS", OracleDbType.Varchar2)).Value = subtestAcronyms;
                cmd.Parameters.Add(new OracleParameter("IN_ACCOUNTABILITY_FLAG", OracleDbType.Decimal)).Value = accountabilityFlag;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {
                    //Original does not validate
                    //TODO: column names
                    var result = new List<DbScoreWarning>();
                    while (dataReader.Read())
                    {
                        result.Add(new DbScoreWarning
                        {
                            DisplaySeq = dataReader[0].ToString(),
                            FilterDesc = $"{dataReader[4]} = {dataReader[1]}",
                            ExcludeFilter = dataReader[2].ToString(),
                            IncludeFilter = dataReader[3].ToString()
                        });
                    }
                    return result;
                }
            }
        }

        public string GetFormsForMultimeasureColumn(int customerScoresetId, int nodeId, string nodeType, int testFamilyGroupId, int testGradeLevelId)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_NODE_FORMS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CUSTOMERSCORESET_ID", OracleDbType.Decimal)).Value = customerScoresetId;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_TYPE", OracleDbType.Varchar2, 20)).Value = nodeType;
                cmd.Parameters.Add(new OracleParameter("IN_NODE_ID", OracleDbType.Decimal)).Value = nodeId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_CODE_LIST", OracleDbType.Decimal)).Value = testFamilyGroupId;
                cmd.Parameters.Add(new OracleParameter("IN_TESTGRADELEVEL_ID", OracleDbType.Decimal)).Value = testGradeLevelId;
                cmd.Parameters.Add(new OracleParameter("OUT_FORMS", OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                return cmd.Parameters["OUT_FORMS"].Value.ToString();
            }
        }
    }
}