using DM.WR.Data.Repository.Types;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using DM.WR.Data.Config;

namespace DM.WR.Data.Repository
{
    public class ReportCriteriaClient : IReportCriteriaClient
    {
        private readonly IDataAccessHelpers _helpers;

        public ReportCriteriaClient(IDataAccessHelpers helpers)
        {
            _helpers = helpers;
        }

        private ReportCriteria ReportCriteria_ReadRecordSet(OracleDataReader dataReader)
        {
            ReportCriteria criteria = new ReportCriteria
            {
                CriteriaId = (int) (long) dataReader["CRITERIA_ID"],
                DmUserId = dataReader["DM_USER_ID"].ToString(),
                LocationGuid = dataReader["ORIGINAL_REFERENCE"].ToString(),
                AssessmentId = (int) (long) dataReader["TESTFAMILYGROUP_ID"],
                AssessmentGroupCode = dataReader["MENU_TESTFAMILYGROUP_CODE"].ToString(),
                DisplayType = dataReader["CATEGORY_CODE"].ToString(),
                CriteriaName = dataReader["CRITERIA_NAME"].ToString(),
                CriteriaDescription = dataReader["CRITERIA_DESC"].ToString(),
                CreatedDate = (DateTime) dataReader["LAST_UPDATE_DATE"],
                OptionsXml = null
            };
            return criteria;
        }

        public string ReportCriteria_LoadOptions(int criteriaId)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_IRMREPORT_CRITERIA_OPTIONS"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("CRITERIA_ID", OracleDbType.Int32)).Value = criteriaId;
                cmd.Parameters.Add(new OracleParameter("OUT_OPTIONS", OracleDbType.Clob)).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                return ((OracleClob)cmd.Parameters["OUT_OPTIONS"].Value).Value;
            }
        }

        public List<ReportCriteria> ReportCriteria_Select(string dmUserId, string locationGuid, object assessmentId = null, object displayType = null, object criteriaName = null)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("GET_IRMREPORT_CRITERIA"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_DM_USER_ID", OracleDbType.Varchar2)).Value = dmUserId;
                cmd.Parameters.Add(new OracleParameter("IN_ORIGINAL_REFERENCE", OracleDbType.Varchar2)).Value = locationGuid;
                cmd.Parameters.Add(new OracleParameter("IN_INTERNAL_FLAG", OracleDbType.Byte)).Value = 0;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Int32)).Value = assessmentId ?? DBNull.Value;
                cmd.Parameters.Add(new OracleParameter("IN_CATEGORY_CODE", OracleDbType.Varchar2)).Value = displayType ?? DBNull.Value;
                cmd.Parameters.Add(new OracleParameter("IN_CRITERIA_NAME", OracleDbType.Varchar2)).Value = criteriaName ?? DBNull.Value;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    var result = new List<ReportCriteria>();

                    if (!odr.HasRows) //Returns empty List<ReportCriteria> in case nothing was found
                        return result;

                    while (odr.Read())
                        result.Add(ReportCriteria_ReadRecordSet(odr));

                    return result;
                }
            }
        }

        //If successful returns new ReportCriteria, otherwise return null; 
        public ReportCriteria ReportCriteria_Insert(ReportCriteria inCriteria)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("INSERT_IRMREPORT_CRITERIA"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_DM_USER_ID", OracleDbType.Varchar2)).Value = inCriteria.DmUserId;
                cmd.Parameters.Add(new OracleParameter("IN_ORIGINAL_REFERENCE", OracleDbType.Varchar2)).Value = inCriteria.LocationGuid;
                cmd.Parameters.Add(new OracleParameter("IN_TESTFAMILYGROUP_ID", OracleDbType.Int32)).Value = inCriteria.AssessmentId;
                cmd.Parameters.Add(new OracleParameter("IN_CATEGORY_CODE", OracleDbType.Varchar2)).Value = inCriteria.DisplayType;
                cmd.Parameters.Add(new OracleParameter("IN_CRITERIA_NAME", OracleDbType.Varchar2)).Value = inCriteria.CriteriaName;
                cmd.Parameters.Add(new OracleParameter("IN_CRITERIA_DESC", OracleDbType.Varchar2)).Value = inCriteria.CriteriaDescription;
                cmd.Parameters.Add(new OracleParameter("IN_OPTIONS", OracleDbType.Clob)).Value = inCriteria.OptionsXml;
                cmd.Parameters.Add(new OracleParameter("IN_INTERNAL_FLAG", OracleDbType.Int32)).Value = inCriteria.SubmitToReportCenter ? 1 : 0;
                cmd.Parameters.Add(new OracleParameter("IN_MENU_TESTFAMILYGROUP_CODE", OracleDbType.Varchar2)).Value = inCriteria.AssessmentGroupCode;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    if (!odr.HasRows)
                        return null; //UNIQUE CRITERIA NAME VIOLATION

                    odr.Read();
                    return ReportCriteria_ReadRecordSet(odr);
                }
            }
        }

        public ReportCriteria ReportCriteria_Update(Int32 inCriteriaId, string newName, string newDescription, string newOptionsXml)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("UPDATE_IRMREPORT_CRITERIA"), con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_CRITERIA_ID", OracleDbType.Varchar2)).Value = inCriteriaId;
                cmd.Parameters.Add(new OracleParameter("IN_CRITERIA_NAME", OracleDbType.Varchar2)).Value = newName;
                cmd.Parameters.Add(new OracleParameter("IN_CRITERIA_DESC", OracleDbType.Varchar2)).Value = newDescription;
                cmd.Parameters.Add(new OracleParameter("IN_OPTIONS", OracleDbType.Clob)).Value = (object)newOptionsXml ?? DBNull.Value;
                cmd.Parameters.Add(new OracleParameter("IN_INTERNAL_FLAG", OracleDbType.Byte)).Value = 0;
                cmd.Parameters.Add(new OracleParameter("OUT_REF_CURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                using (OracleDataReader odr = cmd.ExecuteReader())
                {
                    if (!odr.HasRows) //UNIQUE CRITERIA NAME VIOLATION
                        return null;

                    odr.Read();
                    return ReportCriteria_ReadRecordSet(odr);
                }
            }
        }

        public void ReportCriteria_Delete(List<int> criteriaIds)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            {
                con.Open();

                using (OracleCommand cmd = new OracleCommand(_helpers.GetFullPackageName("DELETE_IRMREPORT_CRITERIA"), con))
                using (OracleTransaction trans = con.BeginTransaction())
                {

                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("IN_CRITERIA_ID", OracleDbType.Int32));
                    foreach (int criteriaId in criteriaIds)
                    {
                        cmd.Parameters[0].Value = criteriaId;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
            }
        }
    }
}
