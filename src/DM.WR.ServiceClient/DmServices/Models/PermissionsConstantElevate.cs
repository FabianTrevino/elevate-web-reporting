using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.ServiceClient.DmServices.Models
{
    public class PermissionsConstantElevate
    {
        public const int CleverAdministrationEnterDistrictAppToken = 21; //Permission allows a user to enter the district clever app token
        public const int CleverAdministrationFileImportStatus = 22; //Permission allows a user to get file import status
        public const int CleverAdministrationFileInformation = 23; //Permission allows a user to get file information
        public const int SelfServiceRosterAdministrationImportStaffStudents = 24; //Permission allows a user to import staff and students
        public const int SelfServiceRosterAdministrationViewImportReports = 25; //Permission allows a user to view import reports
        public const int SelfServiceRosterAdministrationErrorCorrection = 26; //Permission allows a user to correct errors
        public const int SettingsPreferencesAdministrationTestAssignment = 27; //Permission allows a user to make test assignments
        public const int SettingsPreferencesAdministrationCustomizeRolesRights = 28; //Permission allows a user to customize a users roles and rights
        public const int SettingsPreferencesAdministrationDataPrivacy = 29; //Permission allows a user to set data privacy and security
        public const int OrderHistoryAdministrationViewLicenseBalance = 30; //Permission allows a user to view their licenses by product
        public const int OrderHistoryAdministrationViewUsageReport = 31; //Permission allows a user to view their license usage report
        public const int OrderHistoryAdministrationDownloadOrders = 32; //Permission allows a user to download their orders
        public const int UserSearchAdministrationSeeStaffProfiles = 33; //Permission allows a user to see (and edit?) staff user profiles
        public const int UserSearchAdministrationSeeStudentProfiles = 34; //Permission allows a user to see (and edit?) student user profiles
        public const int UserSearchAdministrationExtendTime = 35; //Permission allows a user to extend time before/after testing (on student portfolio pages)
        public const int UserSearchAdministrationDeleteTest = 36; //Permission allows a user to delete test instances after administration (on student portfolio pages)
        public const int DashboardAdministrationViewTestStatus = 37; //Permission allows an admin user to view test status
        public const int DashboardAdministrationViewUsageMetrics = 38; //Permission allows an admin user to view usage metrics
        public const int DashboardAdministrationViewAssessmentReports = 39; //Permission allows an admin user to view assessment reports
        public const int DashboardAdministrationViewProfileInfo = 40; //Permission allows an admin user to view profile information
        public const int TestAssignmentAdministrationCreate = 41; //Permission allows a user to create a new assignment
        public const int TestAssignmentAdministrationEdit = 42; //Permission allows a user to edit an existing assignment
        public const int TestAssignmentAdministrationDelete = 43; //Permission allows a user to delete an existing assignment
        public const int TestAssignmentAdministrationSelectTestAttributes = 44; //Permission allows a user to select test attributes/forms and levels
        public const int TestAssignmentAdministrationPrintTickets = 45; //Permission allows a user to print test taker tickets
        public const int ProctoringAdministrationApproveDenyStudents = 46; //Permission allows a user to approve/deny students
        public const int ProctoringAdministrationPauseTest = 47; //Permission allows a user to pause a test
        public const int ProctoringAdministrationCancelTest = 48; //Permission allows a user to cancel a test
        public const int ProctoringAdministrationExitSaveTest = 49; //Permission allows a user to exit/save a test
        public const int ProctoringAdministrationExtendTime = 50; //Permission allows a user to extend time for a test
        public const int ProctoringAdministrationDistanceProctoring = 51; //Permission allows a user to perform distance proctoring
        public const int ProctoringAdministrationEnableSecureBrowser = 52; //Permission allows a user to enable the secure browser
        public const int ProctoringAdministrationSelectAudioLanguage = 53; //Permission allows a user to select the audio language for a test
        public const int ReportingAdministrationRunProductAssessmentReports = 54; //Permission allows a user to run assessment reports by product (IowaFlex and CogAT)
        public const int ReportingAdministrationRunTestStatusReports = 55; //Permission allows a user to run test status reports
        public const int TestAssignmentAdministrationEnableOutOfGradeLevelAssignments = 56; //Permission allows a user to enable out of grade/level assignments
        public const int TestAssignmentAdministrationEnableOutOfSequenceSubtests = 57; //Permission allows a user to enable out of sequence subtest selection by students
        public const int TestAssignmentAdministrationAllowNormSeasonSelection = 58; //Permission allows a user to allow norm season selection
        public const int TestAssignmentAdministrationOptOutDataSharing = 59; //Permission allows a user to opt out of sharing de-identified data with Riverside insights for research purposes
        public const int TestAssignmentAdministrationDualFactorAuth = 60; //Permission allows a user to require dual factor authentication
        public const int TestAssignmentAdministrationUseNISTIdentityGuidelines = 61; //Permission allows a user to use NIST-800-63B digital identity guidelines for passwords
        public const int ProctoringAdministrationResetProctorCode = 66; //Permission allows a user to reset a proctor code
        public const int AdminCleverSyncs = 67; //Role allows user to perform Clever syncs
        public const int AdminLicenseInfo = 68; //Role allows user to view license information
        public const int AdminOrderHistory = 69; //Role allows user to view order history
        public const int AdminOrderAdditionalTests = 70; //Role allows user to order additional tests
        public const int AdminCustomizeRolesRights = 71; //Role allows user to customize user roles and rights
        public const int TACRUDAssignments = 72; //Role allows user to create/edit/delete test assignments
        public const int ProctorDistance = 73; //Role allows distance proctoring (if enabled for my district)
        public const int ProctorExtendTime = 74; //Role allows user to extend time for students after test administration
        public const int ProctorDeleteTestInstance = 75; //Role allows user to cancel assignments on student portfolio page
        public const int ReportsViewProducts = 76; //Role allows user to view reports (assessment products)
        public const int ReportsViewActivity = 77; //Role allows user to view system activity reports
        public const int AdminNegativeLicenseNotAllowed = 89; //Permission means a customer can not have a negative license count
        public const int AdminImportsErrorCorrection = 90; //Permission allows users to import, edit and delete student, staff and location records in rosters.
        public const int AdminDataPrivacySuppressProgramDescriptionsFromReports = 105; //Permission suppress labels for program descriptions in reports
        public const int TAViewAssignmentsStatus = 109; //Role allows a user to view assignment status
        public const int ProctorAssessments = 110; //Role allows for proctoring assessments
        public const int DataIntegrationClever = 151; //District uses Clever for Data Integration
    }
}
