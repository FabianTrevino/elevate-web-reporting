using DM.WR.BL.Builders;
using DM.WR.Models.Config;
using DM.WR.Data.Repository;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.Types;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DM.WR.BL.Managers
{
    public class UserDataManager : IUserDataManager
    {
        private readonly ISessionManager _sessionManager;
        private readonly IDbClient _dbClient;

        private readonly DbTypesMapper _typesMapper;

        public UserDataManager(ISessionManager sessionManager, IDbClient dbClient)
        {
            _sessionManager = sessionManager;
            _dbClient = dbClient;

            _typesMapper = new DbTypesMapper();
        }

        public UserData GetUserData()
        {
            return _sessionManager.Retrieve(SessionKey.UserData) as UserData;
        }

        public void StoreUserData(UserData userData)
        {
            _sessionManager.Store(userData, SessionKey.UserData);
            List<DbAssessment> assessments = new List<DbAssessment>();
            DbAssessment cogatAssement = new DbAssessment();
            cogatAssement.TestFamilyGroupId = 1;
            cogatAssement.TestFamilyName = "'COGAT'";
            cogatAssement.TestFamilyDesc = "CogAT Assessments";
            cogatAssement.TestFamilyGroupCode = "COGAT";
            cogatAssement.SmVersion = "";
            assessments.Add(cogatAssement);
           
            if (!userData.IsAdaptive)
            {


                var dbTypesMapper = new DbTypesMapper();
                var dbCustomerInfo = dbTypesMapper.Map<DbCustomerInfo>(userData.CurrentCustomerInfo);

            /*
              Assesments are only cogat  removed as it is a Orcale connection
            */
                //    var dbAssessments = _dbClient.GetAllAssessments(dbCustomerInfo, userData.ContractInstances);

                userData.Assessments = dbTypesMapper.Map<List<Assessment>>(assessments);
            }


        }

        public void StoreUserDataElevate(UserData userData)
        {
            _sessionManager.Store(userData, SessionKey.UserData);

            if (!userData.IsAdaptive)
            {
                var dbTypesMapper = new DbTypesMapper();
                var dbCustomerInfo = dbTypesMapper.Map<DbCustomerInfo>(userData.CurrentCustomerInfo);
                var dbAssessments = _dbClient.GetAllAssessments(dbCustomerInfo, userData.ContractInstances);

                userData.Assessments = dbTypesMapper.Map<List<Assessment>>(dbAssessments);
            }
        }

        public UserData ChangeLocation(int nodeId)
        {
            var userData = GetUserData();

            var customerInfoToMakeCurrent = userData.CustomerInfoList.FirstOrDefault(ci => Convert.ToInt32(ci.NodeId) == nodeId);
            //TODO:  Should probably throw some kind of message saying "Could not switch location"
            if (customerInfoToMakeCurrent != null) userData.CurrentGuid = customerInfoToMakeCurrent.Guid;

            StoreUserData(userData);

            return userData;
        }

        //public void AddLocation(UserDetails userDetails)
        //{
        //    var currentUserData = GetUserData();

        //    var newGuids = userDetails.LocationGuids.Split(',').ToList();
        //    var oldGuids = currentUserData.CustomerInfoList.Select(ci => ci.Guid).ToList();
        //    var newGuid = newGuids.FirstOrDefault(ng => oldGuids.All(og => og != ng));

        //    if (newGuid == null) throw new ApplicationException("Unable to find new location guid to add on the location");

        //    var newCustomerInfo = _dbClient.GetCustomerInfo(newGuid, currentUserData.ContractInstances);

        //    currentUserData.CustomerInfoList.Add(_typesMapper.Map<CustomerInfo>(newCustomerInfo));
        //    currentUserData.CurrentGuid = newGuid;

        //    StoreUserData(currentUserData);
        //}
    }
}