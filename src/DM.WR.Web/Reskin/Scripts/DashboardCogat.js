var Dashboard = function () {
    var private = {
        convertToPdfImmediately: true, //print profile narrative generates PDF or HTML
        MaxRosterShownColumnsNum: 3 + 6, //Roster Table Columns: Location or Student Name, NPR, SS + 6 Domains (max)
        EmptyDomainsColumnContent: '<div class="dm-ui-card" tabindex="3" data-tabindex="3"><div class="spinner"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div></div></div>',
        StanineInitialZeroData: {}, //json object with zero values to initialize Stanine Chart before getting new json values
        RosterInitialZeroData: {}, //json object with zero values to initialize Roster before getting new json values
        StanineTableInitialZeroData: {}, //json object with zero values to initialize Stanine Table before getting new json values
        RightCardTableInitialZeroData: {}, //json object with zero values to initialize Right Card Table before getting new json values
        RightCardTable2InitialZeroData: {}, //json object with zero values to initialize Right Card Table2 Table before getting new json values
        RosterDropdownOptionsContentArea: "", //Roster Filters Popup options for dropdown Content Area
        RosterDropdownOptionsScore: "", //Roster Filters Popup options for dropdown Score
        RosterDropdownOptionsCondition: '<option data-alt-value="" value="eq">is equal to</option><option data-alt-value="" value="gt">is greater than</option><option data-alt-value="" value="lt">is less than</option><option data-alt-value="" value="gte">is greater than or equal to</option><option data-alt-value="" value="lte">is less than or equal to</option><option data-alt-value="" value="is_in_between" selected="selected">is in between</option>', //Roster Filters Popup options for dropdown Condition
        WarningsDropdownOptions: "", //Warnings Filters Popup options for dropdowns
        modelRoster: {}, //last successfully received Roster
        modelRosterGroupTotals: {}, //last successfully received Roster GroupTotal
        modelAgeStanine: {}, //last successfully received Age Stanine
        modelCutScore: {}, //last successfully received Cut Score
        modelFilteredRoster: {}, //filtered Roster
        modelForSearchFilteredRoster: {}, //filtered Roster model for autocomplete suggestions for filtered data only
        modelFilteredRosterRightTopCard: {}, //filtered Roster for right top card
        modelRosterAllColumns: {}, //all Roster columns structure
        CurrentRosterFilter: {}, //current Roster filter applied (by Ability, School or Class, Stanine/Content Area)
        CurrentPopupCriteriaRosterFilter: {}, //filter Criteria applied in Popup
        arrayAppliedRosterFilterDropdownsCondition: [],
        arrayAppliedRosterWarningFilterDropdownsCondition: [],
        selectedContentStanineCard: "",
        //arrRosterSelectedStudents: [],
        RosterGridNavigationEvent: false,
        RosterGridKeyEvent: false,
        RosterGridOrderingEvent: false,
        LastFocusedElement: {},
        SearchAutocompleteFoundNum: 0,
        LastRosterTableKeyPressedCode: 0,
        debugDateTime: "",
        isPopupFiltersOfRightCard: false,
        arrColumnsSelectedInRoster: [],
        arrColumnsSelectedInRightCardPopupFilter: [],
        arrSelectedTopFilterScores: [],
        RowsNumberStanineCard: 0,
        RowsNumberRightCard: 0,
        RowsNumberRightCard2: 0,
        paginationRightCard: 5,
        paginationRightCard2: 12,
        isRightCardPaginationVisible: false,
        isRightCard2PaginationVisible: false,
        LoggedUserLevel: "",
        isScreener: false,
        TestFamilyGroupCode: "CogAT",
        SuppressedTextBuilding: "Building",
        SuppressedTextClass: "Class",
        CutScoreAppliedFiltersLabelText: "",
        DifferentiatedReportDistrictName: "",
        DifferentiatedReportBuildingName: "",
        DifferentiatedReportClassName: "",
        RosterPageSize: 25,
        RosterCurrentPageNumber: 1,
        UseServerPaging: false,
        isCogatFormNormYearUpdated: false,
        UseHybridMode: true,
        UseHybridModeV2: true,
        HybridCardAjaxRequestCounter: 0,
        HybridGetAllRosterFlag: false,
        isHybridAllDataReceived: false,
        HybridProgressBarRosterCount: false,
        CurrentRosterTotalCountNum: 0,
        HybridMinRosterTotalCountNum: 500,
        HybridModeRequestDelayInterval: 0,
        isCutScoreCardGenerated: false,
        EnforcedCutScoreResults: false,
        objLastSort: {},
        isTabNavigationOn: false,
        defaultTabIndex: "0",
        isProdEnvironment: true,
        printReportGrades: [],
        printReportBuilding: [],
        printReportClasses: [],
        printReportStudents: [],
    };

    var siteRoot = "";
    if (private.isTabNavigationOn) {
        private.defaultTabIndex = "-1";
    }

    DmUiLibrary.AbortAllAjaxRequests(); //abort all Ajax requests

    private.StanineInitialZeroData = getEmptyJsonStanine();
    private.StanineTableInitialZeroData = getEmptyJsonStanineTable(private.isScreener);
    //private.StanineTableInitialZeroData = getSampleJsonStanineTable();
    private.RightCardTableInitialZeroData = getSampleJsonRightCardTable();
    //private.RightCardTable2InitialZeroData = getSampleJsonRightCardTable2();
    private.RightCardTable2InitialZeroData = getEmptyJsonRightCardTable2();
    private.RosterInitialZeroData = getEmptyJsonRosterTable();

    return {
        Init: function (isProdEnvironment,returnUrl) {
            //console.log(window.location);
            if (window.location.search.indexOf("use=server") !== -1) {
                private.UseServerPaging = true;
                private.UseHybridMode = false;
            }
            var uiSettings = DmUiLibrary.GetUiSettings();
            siteRoot = uiSettings.SiteRoot;

            //Fix for svg href clicks: Set a split prototype on the SVGAnimatedString object which will return the split of the baseVal on this
            SVGAnimatedString.prototype.split = function () { return String.prototype.split.apply(this.baseVal, arguments); };

            this.InitDebuggingQueries(isProdEnvironment);
            this.AssignSelectGroupsHandlers();
            this.FiltersChangeHandler();
            this.AssignAllEventHandlers();
            this.ValidateReportName();
            this.GetFilters(false);
            Dashboard.PreventSignOut();
            Dashboard.appendFeedBackButtonAndForm();

            DmUiLibrary.HideOverlaySpinner();
            //$(".dm-ui-overlay-spinner-container.placed-in-block").css("display", "block");

            setInterval(function () { DmUiLibrary.NoAjaxResetSessionTimeOut(); }, 60000);
        },

        IsTabNavigationOn: function () {
            return private.isTabNavigationOn;
        },

        InitDebuggingQueries: function (isProdEnvironment) {
            if (typeof isProdEnvironment === "string") {
                //Debugging DIV for GraphQL queries
                if (isProdEnvironment === "False") {
                    private.isProdEnvironment = false;
                    window.addEventListener("click", function (evt) {
                        if (evt.target.nodeName === "BODY" && evt.detail === 3) {
                            if ($("#debug-api:visible").length) {
                                //$("#debug-api").hide("slide", { direction: "right" }, 1000);
                                $("#debug-api").hide();
                            } else {
                                //$("#debug-api").show("slide", { direction: "right" }, 1000);
                                $("#debug-api").show();
                            }
                        }
                    });
                    $(document).on("click", "#debug-api textarea", function () {
                        $("#debug-api textarea").blur();
                        $(this).select();
                        document.execCommand("copy");
                    });
                }
            }
        },

        PreventSignOut: function () {
            var uiDashboardClicksNum = 1;
            $(document).on("click touchstart", "a, button, .dm-ui-dropdown-button, .dm-ui-button-primary", function () {
                uiDashboardClicksNum++;
            });
            function checkClicksOnLinkOrButtons() {
                //console.log('uiDashboardClicksNum(before)=' + uiDashboardClicksNum);
                if (uiDashboardClicksNum) {
                    uiDashboardClicksNum = 0;
                    DmUiLibrary.NoAjaxResetSessionTimeOut();
                    //DmUiLibrary.InitSessionTimeOut();
                }
                setTimeout(checkClicksOnLinkOrButtons, 60000);
            }
            checkClicksOnLinkOrButtons();
        },

        DebugRememberTime: function () {
            private.debugDateTime = new Date();
        },

        DebugShowTime: function (text) {
            var endDateTime = new Date();
            //console.log("DEBUG (" + text + "): " + (endDateTime.getTime() - private.debugDateTime.getTime()) / 1000 + " sec");
        },

        AssignSelectGroupsHandlers: function () {
            $(document).on("change", ".dashboard-filter > select", function () {
                DmUiLibrary.AbortAllAjaxRequests(); //abort all Ajax requests
                private.HybridGetAllRosterFlag = false;
                Dashboard.StopRosterProgressBar();
                var result = $.map($(this).find("option:selected"), function (elem) {
                    return elem.value;
                });
                $(this).siblings("input.dm-ui-hidden-input").val(JSON.stringify(result)).trigger("change");
            });
        },

        FiltersChangeHandler: function () {
            var self = this;
            $(document).on("change", ".dm-ui-hidden-input", function () {
                Dashboard.ClearAllErrorsMessages();
                Dashboard.DisableRosterScoreWarningFiltersPopupButton();
                $("#total-students-num-quartiles").empty();
                $("#total-students-num-domains").empty();
                $("#total-students-num-roster").empty();

                if (this.id === "1") {
                    $("#test-event-subject-name").html("&nbsp;");
                }

                var filterType = this.id;
                var values = $(this).val();
                var data = JSON.stringify({ filterType: filterType, values: JSON.parse(values) });

                $.ajax({
                    async: true,
                    type: "POST",
                    url: siteRoot + "/DashboardCogat/UpdateFilters",
                    data: data,
                    dataType: "json",
                    beforeSend: function () {
                        //DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        if (data === "Unauthorized") {
                            window.location.href = returnUrl;
                        }
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            self.GetFilters(false);
                        }
                        
                    },
                    error: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });
        },

        CheckIsResetPageButtonDisabled: function () {
            var isResetPageDisabled = true;
            if ($(".bread-crumbs a.location-drill").length > 0) {
                isResetPageDisabled = false;
            }
            /*
            if (isResetPageDisabled) {
                if ($("#roster-name").text() === "Student Roster") {
                    isResetPageDisabled = false;
                }
            }
            */
            if (isResetPageDisabled) {
                $(".filters .dashboard-filter select.dm-ui-single-select option:first-child").each(function () {
                    if (!$(this).prop("selected")) {
                        isResetPageDisabled = false;
                    }
                });
            }
            if (isResetPageDisabled) {
                $(".filters .dashboard-filter select.dm-ui-multi-select:not(#PopulationFilters_Control) option").each(function () {
                    if (!$(this).prop("selected")) {
                        isResetPageDisabled = false;
                    }
                });
            }
            if (isResetPageDisabled) {
                $(".filters .dashboard-filter select.dm-ui-group-multi-select:not(#PopulationFilters_Control) option").each(function () {
                    if (!$(this).prop("selected")) {
                        isResetPageDisabled = false;
                    }
                });
            }
            if (isResetPageDisabled) {
                $(".filters .dashboard-filter select.dm-ui-group-multi-select#PopulationFilters_Control option").each(function () {
                    if ($(this).prop("selected")) {
                        isResetPageDisabled = false;
                    }
                });
            }
            if (isResetPageDisabled) {
                $(".dashboard-filter.button button").addClass("disabled-element");
            } else {
                $(".dashboard-filter.button button").removeClass("disabled-element");
            }
        },

        CapitalizeText: function (text) {
            return text.toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase() });
        },

        GetFilters: function (hideSpinnerOnComplete) {
            Dashboard.InitAllCards();
            private.UseHybridMode = true;
            private.isCutScoreCardGenerated = false;
            private.RosterCurrentPageNumber = 1;
            Dashboard.DisableRightCardPopupButton();
            private.modelCutScore = {};
            private.objLastSort = {};
            $.ajax({
                async: true,
                type: "GET",
                dataType: "html",
                url: siteRoot + "/DashboardCogat/GetFilters",
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        Dashboard.GetBackgroundLocation();
                        $(".filters .root-tab-hidden-content").html(data);
                        $(".filters .root-tab-hidden-content").append('<div class="dashboard-filter button"><button class="dm-ui-button-primary dm-ui-button-small">Reset Page</button></div>');
                        if (private.isTabNavigationOn) {
                            $(".filters").attr("tabindex", "1");
                            $('.filters [tabindex="0"], .filters a, .filters button').each(function () {
                                $(this).attr("tabindex", "-1");
                            });
                            //$(".filters [role=alert]").attr("tabindex", "-1").attr("aria-hidden", "true");
                            //$("#modal-dashboard-report-criteria [role=alert]").attr("tabindex", "-1").attr("aria-hidden", "true");
                        }
                        $(".bread-crumbs").html($("#root-location").val());

                        //Dashboard.CheckIsResetPageButtonDisabled();

                        if ($("#test-family-group-code").val() === "CCAT") {
                            private.TestFamilyGroupCode = "CCAT";
                        } else {
                            private.TestFamilyGroupCode = "CogAT";
                        }
                        if (private.TestFamilyGroupCode !== $("#cogat-header-type").text()) {
                            $("head title").text(private.TestFamilyGroupCode + " Dashboard");
                            $("#cogat-header-type").text(private.TestFamilyGroupCode);
                        }

                        private.SuppressedTextBuilding = Dashboard.CapitalizeText($("#Label_Building_Control").text());
                        if (private.SuppressedTextBuilding === "") {
                            private.SuppressedTextBuilding = "School";
                        }
                        if (private.TestFamilyGroupCode === "CCAT") {
                            private.SuppressedTextBuilding = "Building";
                        }
                        private.SuppressedTextClass = Dashboard.CapitalizeText($("#Label_Class_Control").text());
                        if (private.SuppressedTextClass === "") {
                            private.SuppressedTextClass = "Class";
                        }
                        private.SuppressedTextClass = "Section";
                        $("#student-info-school").prev().text(private.SuppressedTextBuilding);
                        $("#student-info-class").prev().text(private.SuppressedTextClass);

                        if ($("#battery").val() === "Screener") {
                            private.isScreener = true;
                            $("#modal-dashboard-student-info section > div.block:last-child").hide();
                        } else {
                            private.isScreener = false;
                            $("#modal-dashboard-student-info section div.block:last-child").show();
                        }
                        //console.log("SCREENER: " + private.isScreener);

                        if (document.cookie.split(";").filter((item) => item.trim().startsWith("currentStaffRole=")).length) {
                            currentStaffRole = document.cookie.replace(/(?:(?:^|.*;\s*)currentStaffRole\s*\=\s*([^;]*).*$)|^.*$/, "$1");
                            if (currentStaffRole === "district_admin") private.LoggedUserLevel = "DISTRICT";
                            if (currentStaffRole === "staff") private.LoggedUserLevel = "BUILDING";
                            if (currentStaffRole === "teacher") private.LoggedUserLevel = "CLASS";
                            console.log("USER LEVEL (currentStaffRole): " + private.LoggedUserLevel);
                        } else {
                            private.LoggedUserLevel = $("#root-location-type").val();
                            console.log("USER LEVEL (#root-location-type): " + private.LoggedUserLevel);
                        }
                        //private.LoggedUserLevel = "BUILDING";
                        $("#dashboard-right-column, .filters").removeClass("level-building level-class");
                        $("#dashboard-right-column, .filters").addClass("level-" + private.LoggedUserLevel.toLowerCase());

                        if (private.LoggedUserLevel !== "BUILDING" && private.LoggedUserLevel !== "CLASS") {
                            $("#right-card1").attr("aria-label", "Identified Students by " + private.SuppressedTextBuilding);
                            $("#right-card1 .right-card-title").text("Identified Students by " + private.SuppressedTextBuilding);
                            $("#right-card-overlay-type").text(private.SuppressedTextBuilding);
                        }

                        if (private.LoggedUserLevel === "BUILDING") {
                            $("#right-card1").attr("aria-label", "Identified Students by " + private.SuppressedTextClass);
                            $("#right-card1 .right-card-title").text("Identified Students by " + private.SuppressedTextClass);
                            $("#right-card-overlay-type").text(private.SuppressedTextClass);
                        }

                        if (private.LoggedUserLevel !== "CLASS") {
                            $("#right-card1").show();
                        } else {
                            $("#right-card1").hide();
                        }

                        /*
                                                if (private.LoggedUserLevel === "BUILDING" || private.LoggedUserLevel === "CLASS") {
                                                    private.UseHybridMode = false;
                                                }
                        */

                        if ((private.LoggedUserLevel !== "CLASS" && private.LoggedUserLevel !== "BUILDING") || private.isScreener) {
                            $("#print-report-type option[value=DifferentiatedInstructionReport]").remove();
                        } else {
                            if (!$("#print-report-type option[value=DifferentiatedInstructionReport]").length) {
                                var options = $("#print-report-type").html();
                                //var begin = options.substr(0, options.indexOf("Student Profile") + 24);
                                //var end = options.substr(options.indexOf("Student Profile") + 24);
                                var begin = options.substr(0, options.indexOf(">Dashboard<") + 19);
                                var end = options.substr(options.indexOf(">Dashboard<") + 19);
                                options = begin + '<option data-alt-value="" value="DifferentiatedInstructionReport">Differentiated Instruction Report</option>' + end;
                                $("#print-report-type").html(options);
                            }
                        }
                        $("#print-report-type").trigger("DmUi:updated");

                        if ((private.LoggedUserLevel === "BUILDING" || private.LoggedUserLevel === "CLASS") && !private.isScreener) {
                            $("#right-card2").show();
                        } else {
                            $("#right-card2").hide();
                        }

                        $("#right-card-reset-button").click();

                        if (private.UseHybridMode) {
                            private.UseServerPaging = true;
                            //Dashboard.GetRosterRowsCount();
                            Dashboard.initSelectedContentArea();
                            Dashboard.initDropdownsValuesOfRosterSearchPopup();
                            Dashboard.resetInputValuesOfRosterSearchPopup();
                            Dashboard.resetInputValuesOfRosterWarningSearchPopup();
                            private.UseHybridMode = false;
                            private.UseServerPaging = false;

                            Dashboard.GetGroupTotals();
                            Dashboard.GetRoster();
                        } else {
                            Dashboard.GetOnlyRosterOrHybridMode();
                        }

                        private.paginationRightCard = 5;
                        private.paginationRightCard2 = 12;
                        private.RowsNumberRightCard = 0;
                        private.RowsNumberRightCard2 = 0;

                        private.isCogatFormNormYearUpdated = false;

                        $(".stanine-card .card-header-content").removeClass("visible");
                        $("#cogat-header-form").text("XX");
                        $("#cogat-header-year").text("XX");
                    } else {
                        if (data.indexOf("current location does not contain any CogAT assessments") !== -1 || data.indexOf("currently could not find any data") !== -1) {
                            $(".filters").hide();
                            Dashboard.redrawBarChartStanine(private.StanineInitialZeroData);
                            Dashboard.redrawTableStanine(private.StanineTableInitialZeroData);
                            $(".stanine-card").addClass("empty-json-overlay");

                            $("#right-card1").show();
                            $("#right-card2").show();
                            Dashboard.redrawRightCardTable2("", data.values);
                            $("#dashboard-right-column").addClass("empty-json-overlay");

                            Dashboard.redrawTableRoster("empty");
                        } else {
                            Dashboard.ShowRosterError();
                        }
                    }
                },
                error: function (data) {
                    Dashboard.ShowElevateError(data);
                },
                complete: function () {
                    if (hideSpinnerOnComplete)
                        DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        GenerateStanineGridFromAjaxRequest: function (data) {
            Dashboard.DebugRememberTime();

            var i = 0, result = getEmptyJsonStanineTable(private.isScreener);

            var resultValuesSelected = [];
            for (i = 0; i < 7; i++) {
                if ($.inArray(i, private.arrSelectedTopFilterScores) !== -1) {
                    resultValuesSelected.push(result.values[i]);
                }
            }
            result.values = resultValuesSelected;

            var sumAllStanineNum = {};
            data.map(function (x) {
                if (x["Bin_number"] !== 0) {
                    if (typeof sumAllStanineNum[x["Subtest_name"]] === "undefined") {
                        sumAllStanineNum[x["Subtest_name"]] = 0;
                    }
                    if (x["Cutscore_number_included"] > 0) {
                        sumAllStanineNum[x["Subtest_name"]] += x["Cutscore_number_included"];
                    }
                }
            });

            var percentValue;
            data.map(function (x) {
                for (i = 0; i < result.values.length; i++) {
                    if (x["Subtest_name"] === result.values[i]["content_area"]) {
                        result.values[i]["stanine" + x["Bin_number"] + "_num"] = x["Cutscore_number_included"];
                        //result.values[i]["stanine" + x["Bin_number"] + "_per"] = Math.round(x["Cutscore_number_included"] / x["Cutscore_total_included"] * 100);
                        if (sumAllStanineNum[x["Subtest_name"]] === 0) {
                            result.values[i]["stanine" + x["Bin_number"] + "_per"] = 0;
                        } else {
                            percentValue = x["Cutscore_number_included"] / sumAllStanineNum[x["Subtest_name"]] * 100;
                            if (Math.round(percentValue) > 0) {
                                result.values[i]["stanine" + x["Bin_number"] + "_per"] = Math.round(percentValue);
                            } else {
                                result.values[i]["stanine" + x["Bin_number"] + "_per"] = percentValue.toFixed(1);
                            }
                        }
                        break;
                    }
                }
            });

            private.RosterDropdownOptionsContentArea = "";
            for (i = 0; i < private.arrSelectedTopFilterScores.length; i++) {
                private.RosterDropdownOptionsContentArea += '<option value="' + result.values[i].content_area + '">' + result.values[i].content_area + "</option>";
            }

            Dashboard.DebugShowTime("calculate table stanine");

            Dashboard.DebugRememberTime();
            $("#dropdown-graph-content").html(private.RosterDropdownOptionsContentArea);
            if (private.selectedContentStanineCard === "" || private.RosterDropdownOptionsContentArea.indexOf('<option value="' + private.selectedContentStanineCard + '"') === -1) {
                private.selectedContentStanineCard = $("#dropdown-graph-content option:last-child").val();
            }
            //$("#dropdown-graph-content option:last-child").prop("selected", true);
            $('#dropdown-graph-content option[value="' + private.selectedContentStanineCard + '"]').prop("selected", true);
            $("#dropdown-graph-content").trigger("DmUi:updated");


            private.modelAgeStanine.content_area = result;
            //Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", "VQN Composite");
            //Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", "Composite (VQN)");
            //Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", $("#dropdown-graph-content option:last-child").val());
            Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", $('#dropdown-graph-content option[value="' + private.selectedContentStanineCard + '"]').val());
            Dashboard.redrawTableStanine(private.modelAgeStanine.content_area);
            $("#stanine-table-wrapper table tr").removeClass("stanine-selected");
            $("#stanine-table-wrapper table tr:last-child").addClass("stanine-selected");
            Dashboard.DebugShowTime("update top card (from ajax request)");

            //Dashboard.initDropdownsValuesOfRosterSearchPopup();

            $(".dropdown-graph-content-wrapper > select").trigger("change");
        },


        GenerateStanineGridFromStudentRoster: function () {
            Dashboard.DebugRememberTime();

            var i = 0, j, result = getEmptyJsonStanineTable(private.isScreener);
            var studentsNum = private.modelRoster.values.length;
            var isAllAsNull;

            private.arrSelectedTopFilterScores = [];
            $(".filters select#Content_Control option").each(function () {
                if ($(this).prop("selected")) {
                    private.arrSelectedTopFilterScores.push(i);
                }
                i++;
            });
            //console.log(private.arrSelectedTopFilterScores);

            var resultValuesSelected = [];
            for (i = 0; i < 7; i++) {
                if ($.inArray(i, private.arrSelectedTopFilterScores) !== -1) {
                    //console.log("Add " + i);
                    resultValuesSelected.push(result.values[i]);
                }
            }
            //console.log(resultValuesSelected);
            result.values = resultValuesSelected;

            var sumAllStanineNum = {};

            private.modelRoster.values.map(function (x) {
                isAllAsNull = true;
                //for (i = 0; i < 7; i++) {
                for (i = 0; i < private.arrSelectedTopFilterScores.length; i++) {
                    if (x["AS" + i] !== null) {
                        isAllAsNull = false;
                        result.values[i]["stanine" + x["AS" + i] + "_num"] += 1;

                        if (typeof sumAllStanineNum["AS" + i] === "undefined") {
                            sumAllStanineNum["AS" + i] = 0;
                        }
                        sumAllStanineNum["AS" + i] += 1;
                    }
                }
                if (isAllAsNull) {
                    studentsNum--;
                }
            });

            private.RosterDropdownOptionsContentArea = "";
            //for (i = 0; i < 7; i++) {
            for (i = 0; i < private.arrSelectedTopFilterScores.length; i++) {
                //if ($.inArray(i, private.arrSelectedTopFilterScores) !== -1) {
                private.RosterDropdownOptionsContentArea += '<option value="' + result.values[i].content_area + '">' + result.values[i].content_area + "</option>";
                //}
                for (j = 1; j <= 9; j++) {
                    if (result.values[i]["stanine" + j + "_num"] !== 0) {
                        //result.values[i]["stanine" + j + "_per"] = Math.round(result.values[i]["stanine" + j + "_num"] / studentsNum * 100);
                        result.values[i]["stanine" + j + "_per"] = Math.round(result.values[i]["stanine" + j + "_num"] / sumAllStanineNum["AS" + i] * 100);
                    }
                }
            }

            Dashboard.DebugShowTime("calculate table stanine");

            Dashboard.DebugRememberTime();
            $("#dropdown-graph-content").html(private.RosterDropdownOptionsContentArea);
            if (private.selectedContentStanineCard === "" || private.RosterDropdownOptionsContentArea.indexOf('<option value="' + private.selectedContentStanineCard + '"') === -1) {
                private.selectedContentStanineCard = $("#dropdown-graph-content option:last-child").val();
            }
            //$("#dropdown-graph-content option:last-child").prop("selected", true);
            $('#dropdown-graph-content option[value="' + private.selectedContentStanineCard + '"]').prop("selected", true);
            $("#dropdown-graph-content").trigger("DmUi:updated");

            private.modelAgeStanine.content_area = result;
            //Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", "VQN Composite");
            //Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", "Composite (VQN)");
            //Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", $("#dropdown-graph-content option:last-child").val());
            Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", $('#dropdown-graph-content option[value="' + private.selectedContentStanineCard + '"]').val());
            Dashboard.redrawTableStanine(private.modelAgeStanine.content_area);
            $("#stanine-table-wrapper table tr").removeClass("stanine-selected");
            $("#stanine-table-wrapper table tr:last-child").addClass("stanine-selected");
            Dashboard.DebugShowTime("update top card (from student roster)");

            //Dashboard.initDropdownsValuesOfRosterSearchPopup();

            $(".dropdown-graph-content-wrapper > select").trigger("change");

            /*
                        //this.redrawBarChartStanine(private.StanineInitialZeroData, "init");
                        //$("#dropdown-graph-content").html("");
                        //Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", "VQN Composite");
            
                        var contentArea, contentArea2, tmp, arrContentArea = [];
                        var data = getSampleJsonStanineTable();
                        private.RosterDropdownOptionsContentArea = "";
            console.log(data);
            console.log(data.values.length);
                        var i;
                        for (i = 0; i < data.values.length; i++) {
                            contentArea = data.values[i]["content_area"];
                            contentArea2 = contentArea;
                            if (contentArea === "Nonverbal") contentArea2 = "Nonverbal";
                            if (contentArea === "VQ Composite") contentArea2 = "Composite (VQ)";
                            if (contentArea === "VN Composite") contentArea2 = "Composite (VN)";
                            if (contentArea === "QN Composite") contentArea2 = "Composite (QN)";
                            if (contentArea === "VQN Composite") contentArea2 = "Composite (VQN)";
                            private.RosterDropdownOptionsContentArea += '<option value="' + contentArea + '">' + contentArea + "</option>";
            
                            if (private.modelAgeStanine.content_area !== undefined) {
                                for (var j = 0; j < 9; j++) {
                                    tmp = private.modelAgeStanine.content_area[contentArea2]["" + (j + 1)];
                                    if (tmp !== undefined) {
                                        data.values[i]["stanine" + (j + 1) + "_per"] = tmp;
                                        data.values[i]["stanine" + (j + 1) + "_num"] = tmp;
                                    } else {
                                        data.values[i]["stanine" + (j + 1) + "_per"] = 0;
                                        data.values[i]["stanine" + (j + 1) + "_num"] = 0;
                                    }
                                }
                            }
                        }
                        Dashboard.initDropdownsValuesOfRosterSearchPopup(arrContentArea);
            
                        private.modelAgeStanine.content_area = data;
                        Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", "VQN Composite");
                        Dashboard.redrawTableStanine(private.modelAgeStanine.content_area);
            
                        //update Stanine Dropdown options
                        for (i = 0; i < data.values.length; i++) {
                            $("#dropdown-graph-content").append('<option data-alt-value="" value="' + data.values[i]["content_area"] + '">' + data.values[i]["content_area"] + "</option>");
                        }
                        $("#dropdown-graph-content option:last-child").prop("selected", true);
                        $("#dropdown-graph-content").trigger("DmUi:updated");
            
                        //temporarily select Stanine Table row with 'VQN Composite' & cell with '20'
                        //var vqnCell = $("#stanine-table-wrapper tr").find("td:contains('VQN Composite')");
                        //vqnCell.parent().addClass("stanine-selected");
                        //vqnCell.parent().find("td:contains('20')").addClass("selected-content-stanine");
            */
        },

        InitAllCards: function () {
            this.ClearAllErrorsMessages();
            this.redrawBarChartStanine(private.StanineInitialZeroData, "init");
            this.redrawTableStanine(private.StanineTableInitialZeroData, "init");
            $("#stanine-table-wrapper").removeClass("k-grid k-widget k-display-block");
            this.redrawTableRoster("init");
            $("#roster-table-wrapper").removeClass("k-grid k-widget k-display-block");
            this.InitRightCards();
        },

        InitRightCards: function () {
            this.redrawRightCardTable("init");
            $("#right-card-table-wrapper").removeClass("k-grid k-widget k-display-block");
            $("#right-card-table-number").text("0");
            this.redrawRightCardTable2("init");
            $("#right-card-table2-wrapper").removeClass("k-grid k-widget k-display-block");
            $("#right-card-table2-number").text("0");
        },

        isAll3AjaxRequestFinished: function () {
            /*
                        if (private.UseHybridMode && !private.HybridGetAllRosterFlag) {
                            private.HybridCardAjaxRequestCounter++;
            
                            var hybridModeRequestsNum = 2;
                            if ((private.LoggedUserLevel === "BUILDING" || private.LoggedUserLevel === "CLASS") && !private.isScreener) {
                                hybridModeRequestsNum = 3;
                            }
            
                            //if (private.HybridCardAjaxRequestCounter >= 3) {
                            if (private.HybridCardAjaxRequestCounter >= hybridModeRequestsNum) {
                                if (!private.UseHybridModeV2) {
                                    private.UseServerPaging = false;
                                }
                                private.HybridGetAllRosterFlag = true;
                                Dashboard.GetRoster();
                            }
                        }
            */
        },


        StartRosterProgressBar: function () {
            if (private.HybridProgressBarRosterCount > 100 && !private.isProdEnvironment) {
                var studentsNum = 1824;
                var seconds = 62;
                //var studentsNum = 7828;
                //var seconds = 232;
                var transition = Math.round(private.HybridProgressBarRosterCount / studentsNum * seconds);
                var el = $("#progress-bar span");
                el.show();
                el.css("transition", "width " + transition + "s linear");
                el.css("width", "100%");
                //console.log("transition=" + transition + "s");
            }
        },

        StopRosterProgressBar: function () {
            var el = $("#progress-bar span");
            el.hide();
            el.css("transition", "none");
            el.css("width", "0%");
        },

        GetAbilityProfiles: function () {
            if (private.UseServerPaging) {
                if (!private.isProdEnvironment) {
                    $("#debug-api-ability-profile textarea").text("");
                    $("#debug-api-ability-profile span").text("");
                }
                /*
                                $("#stanine-table-selected-label").empty();
                                Dashboard.CheckAgeStanineEnabled();
                */
                $.ajax({
                    async: true,
                    type: "GET",
                    dataType: "json",
                    url: siteRoot + "/DashboardCogat/GetAbilityProfiles",
                    success: function (data) {
                        if (!private.isProdEnvironment) {
                            $("#debug-api-ability-profile textarea").text(JSON.stringify(data.api_params));
                            $("#debug-api-ability-profile span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        //DmUiLibrary.HideOverlaySpinner();
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            Dashboard.redrawRightCardTable2("", data.values);
                            Dashboard.isAll3AjaxRequestFinished();
                        } else {
                            Dashboard.ShowRosterError();
                        }
                    },
                    error: function () {
                        //showRosterError();
                    },
                    complete: function (jqXhr, textStatus) {
                        //console.log("GetAgeStanines complete");
                    }
                });
            }
        },

        GetOnlyRosterOrHybridMode: function () {
            if (private.UseServerPaging) {
                //$(".filters #Content_Control_dm_ui .dm-ui-dropdown-items li:last-child input").attr("disabled", "True");

                private.RowsNumberStanineCard = $(".filters select#Content_Control option:selected").length - 1;
                private.arrSelectedTopFilterScores = [];
                $(".filters select#Content_Control option").each(function (i) {
                    if ($(this).prop("selected")) {
                        private.arrSelectedTopFilterScores.push(i);
                    }
                });
                //console.log(private.arrSelectedTopFilterScores);
                private.HybridCardAjaxRequestCounter = 0;
                var delay = private.HybridModeRequestDelayInterval;
                Dashboard.GetGroupTotals();
                setTimeout(function () {
                    Dashboard.GetAgeStanines();
                }, delay);

                if ((private.LoggedUserLevel === "BUILDING" || private.LoggedUserLevel === "CLASS") && !private.isScreener) {
                    delay += private.HybridModeRequestDelayInterval;
                    setTimeout(function () {
                        Dashboard.GetAbilityProfiles();
                    }, delay);
                }
                if (private.LoggedUserLevel !== "CLASS") {
                    delay += private.HybridModeRequestDelayInterval;
                    setTimeout(function () {
                        Dashboard.AutomaticCutScoreRequest();
                    }, delay);
                }
                delay += private.HybridModeRequestDelayInterval;
                setTimeout(function () {
                    Dashboard.GetRoster();
                }, delay);

                setTimeout(function () {
                    private.HybridGetAllRosterFlag = true;
                    Dashboard.GetRoster();
                }, delay + 300);
            } else {
                Dashboard.GetGroupTotals();
                Dashboard.GetRoster();
            }
        },

        ShowRosterError: function () {
            DmUiLibrary.HideOverlaySpinner();
            //console.log("GetStudentRoster Error");
            private.modelRoster = private.RosterInitialZeroData;
            $(".section-card.stanine-card, #dashboard-right-column, #print-report-center").addClass("empty-json-overlay json-error");
            Dashboard.redrawBarChartStanine(private.StanineInitialZeroData);
            Dashboard.redrawTableStanine(private.StanineTableInitialZeroData);
            Dashboard.redrawTableRoster("error");
            $("#right-card1").removeClass("overlayed");
            Dashboard.redrawRightCardTable();
            Dashboard.redrawRightCardTable2();
        },

        ShowElevateError: function (data) {
            DmUiLibrary.HideOverlaySpinner();
            console.error("GetFilter Error");
            let isNoData = false;
            let errorObj = data.responseText.trim();
            errorObj = errorObj.substr(errorObj.indexOf("isNoData") - 2);
            errorObj = errorObj.substr(0, errorObj.indexOf("}") + 1);
            errorObj = JSON.parse(errorObj);
            isNoData = errorObj.isNoData;
            let template =
                '<div class="app-layout-root app-layout-app-error">' +
                    '<div class="page-wrapper user-page">' +
                        '<div class="page-wrapper error-page-wrapper">' +
                            '<div class="error-page-contents">' +
                                '<div class="error-page-body not-found-page-body">' +
                                    '<div class="error-page-contents">';
                                        if (isNoData) {
                                            template += '<h2 class="error-page-title">No Data</h2>';
                                            template += '<div class="error-page-message">Testing has not yet completed or scoring is still in process. Please check back later.</div>';
                                        } else {
                                            template += '<h2 class="error-page-title">Data Processing Error</h2>';
                                            template += '<div class="error-page-message">The server encountered a problem while fetching the data for your request.</div>';
                                        }
                                        //if (document.referrer !== "") {
                                        if (history.length) {
                                            template +=
                                            '<div class="error-page-actions">' +
                                                '<button id="elevate-home-button" class="MuiButton-root MuiButton-outlined app-button app-button-invert MuiButton-outlinedSizeLarge MuiButton-sizeLarge" tabindex="0" type="button">' +
                                                    '<span class="MuiButton-label">Go Back</span>' +
                                                    '<span class="MuiTouchRipple-root"></span>' +
                                                '</button>' +
                                            '</div>';
                                        }
                                        template +=
                                    '</div>' +
                                    '<div class="error-page-icon">';
                                        if (isNoData) {
                                            //template += '<svg xmlns="http://www.w3.org/2000/svg" width="320" height="320" viewBox="0 0 320 320"><g id="no_data" data-name="no data" transform="translate(47 47)"><path id="circle" d="M160,0A160,160,0,1,1,0,160,160,160,0,0,1,160,0Z" transform="translate(-47 -47)" fill="#065b8e"/><path id="no_data_icon" data-name="no data icon" d="M-958.375,39a10.253,10.253,0,0,1-7.526-3.093,10.221,10.221,0,0,1-3.1-7.513V-148.393a10.219,10.219,0,0,1,3.1-7.513A10.253,10.253,0,0,1-958.375-159h99.167a25.588,25.588,0,0,1,9.74,2.21,25.748,25.748,0,0,1,8.411,5.3l34.531,34.473a25.722,25.722,0,0,1,5.313,8.4A25.449,25.449,0,0,1-799-98.893V28.393a10.223,10.223,0,0,1-3.1,7.513A10.254,10.254,0,0,1-809.625,39Zm3.542-14.143h141.666V-88.286h-46.041a10.253,10.253,0,0,1-7.526-3.093,10.219,10.219,0,0,1-3.1-7.513v-45.965h-85Zm99.167-127.286h41.615a12.061,12.061,0,0,0-2.435-4.53l-34.642-34.584a12.1,12.1,0,0,0-4.538-2.431Zm-12.721,86.245-15.92-15.92-15.921,15.92a5,5,0,0,1-7.076,0l-3.539-3.538a5.006,5.006,0,0,1,0-7.077l15.921-15.92-15.921-15.92a5.006,5.006,0,0,1,0-7.077l3.539-3.538a5,5,0,0,1,7.076,0l15.921,15.92,15.92-15.92a5,5,0,0,1,7.076,0l3.539,3.538a5.006,5.006,0,0,1,0,7.077l-15.921,15.92,15.921,15.92a5.006,5.006,0,0,1,0,7.077l-3.539,3.538a4.987,4.987,0,0,1-3.538,1.465A4.987,4.987,0,0,1-868.388-16.184Z" transform="translate(997.308 172.719)" fill="#003e64"/></g></svg>';
                                            template += '<svg xmlns="http://www.w3.org/2000/svg" width="180" height="180" viewBox="0 0 180 180"><g id="Data_Processing_Error" data-name="Data Processing Error" transform="translate(-23 -23)"><path id="Data_Processing_Error-2" data-name="Data Processing Error" d="M193.328,199.772h0L180.26,186.724v6.505H122.353V128.9L90.266,96.865H45.146V51.811L20,26.7l6.672-6.663L45.146,38.486v-.118l19.3,19.273v.118l19.3,19.273v-.118l19.3,19.272v.119l19.863,19.832h.118l19.3,19.273h-.118l18.742,18.713v-.118l19.3,19.273v.118L200,193.109l-6.671,6.662Zm-51.673-51.6h0v25.781h19.3V167.45l-19.3-19.274ZM64.448,71.084h0v6.507h6.516l-6.516-6.507Zm38.6,122.144H45.146V173.957h19.3V135.41h-19.3V116.137h38.6v57.82h19.3v19.271Zm77.209-31.61,0,0-19.3-19.271V135.41h-6.946l-19.3-19.273H180.26v45.482Zm0-64.754H122.353V77.592h19.3V39.046h-19.3V19.772h38.6v57.82h19.3V96.864ZM103.051,84.527l0,0-19.3-19.27V39.046h-19.3v6.936l-19.3-19.273V19.772h57.906V84.527Z" transform="translate(3 3.228)" fill="#c4dae9"/></g></svg>';
                                        } else {
                                            template += '<svg xmlns="http://www.w3.org/2000/svg" width="170" height="198" viewBox="0 0 170 198"><g id="no_data" data-name="no data" transform="translate(-28.308 -13.718)"><path id="no_data_icon" data-name="no data icon" d="M-958.375,39a10.253,10.253,0,0,1-7.526-3.093,10.221,10.221,0,0,1-3.1-7.513V-148.393a10.219,10.219,0,0,1,3.1-7.513A10.253,10.253,0,0,1-958.375-159h99.167a25.588,25.588,0,0,1,9.74,2.21,25.748,25.748,0,0,1,8.411,5.3l34.531,34.473a25.722,25.722,0,0,1,5.313,8.4A25.449,25.449,0,0,1-799-98.893V28.393a10.223,10.223,0,0,1-3.1,7.513A10.254,10.254,0,0,1-809.625,39Zm3.542-14.143h141.666V-88.286h-46.041a10.253,10.253,0,0,1-7.526-3.093,10.219,10.219,0,0,1-3.1-7.513v-45.965h-85Zm99.167-127.286h41.615a12.061,12.061,0,0,0-2.435-4.53l-34.642-34.584a12.1,12.1,0,0,0-4.538-2.431Zm-12.721,86.245-15.92-15.92-15.921,15.92a5,5,0,0,1-7.076,0l-3.539-3.538a5.006,5.006,0,0,1,0-7.077l15.921-15.92-15.921-15.92a5.006,5.006,0,0,1,0-7.077l3.539-3.538a5,5,0,0,1,7.076,0l15.921,15.92,15.92-15.92a5,5,0,0,1,7.076,0l3.539,3.538a5.006,5.006,0,0,1,0,7.077l-15.921,15.92,15.921,15.92a5.006,5.006,0,0,1,0,7.077l-3.539,3.538a4.987,4.987,0,0,1-3.538,1.465A4.987,4.987,0,0,1-868.388-16.184Z" transform="translate(997.308 172.719)" fill="#c4dae9"/></g></svg>';
                                        }
                                    template +=
                                    '</div>';
                                    if (!isNoData) {
                                        template += '<div class="error-page-contact-email">If this problem persists, please contact our <a href="mailto:techsupport@service.riversideinsights.com">Riverside Insights Technical Support Team</a></div>';
                                    }
                                template +=
                                '</div>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>';
            $("body").append(template);
            $(".app-header-title-section").text("No Data");
            $("html").addClass("elevate-custom-error");

            $(document).on("click", "#elevate-home-button", function () {
/*
                var location = window.location.href;
                location = location.replace('webreport.', 'www.');
                location = location.replace('/dashboard/cogat', '');
                window.location.href = location;
*/
                history.back();
            });
        },

        NoData: function () {
            if ($(".dm-ui-alert.dm-ui-alert-info").length === 0) {
                DmUiLibrary.DisplayAlert({ "Message": "There is no data available for the selection, please re-select your criteria and try again.", "HtmlClass": "dm-ui-alert dm-ui-alert-info", "IsDismissable": true }, "json");
                $(".dm-ui-page-container").hide();
            }
        },

        GetScoreWarnings: function () {
            $.ajax({
                async: true,
                type: "GET",
                dataType: "json",
                url: siteRoot + "/DashboardCogat/GetScoreWarnings",
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        console.log(data);
                    }
                }
            });
        },

        GetRosterRowsCount: function () {
            private.HybridProgressBarRosterCount = 0;
            $.ajax({
                async: true,
                type: "GET",
                dataType: "json",
                //url: siteRoot + "/DashboardCogat/GetRosterRowsCount?testFetchSize=1&smFetchSize=1",
                url: siteRoot + "/DashboardCogat/GetRosterRowsCountNew",
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        DmUiLibrary.HideOverlaySpinner();

                        Dashboard.initSelectedContentArea();
                        Dashboard.initDropdownsValuesOfRosterSearchPopup();
                        Dashboard.resetInputValuesOfRosterSearchPopup();
                        Dashboard.resetInputValuesOfRosterWarningSearchPopup();
                        //console.log("count=" + data.values.NumberOfEntityBatches);
                        //private.CurrentRosterTotalCountNum = data.values.NumberOfEntityBatches;
                        if (data.values.length) {
                            private.CurrentRosterTotalCountNum = data.values[0]["Test_count"];
                            private.HybridProgressBarRosterCount = private.CurrentRosterTotalCountNum;
                            if (private.CurrentRosterTotalCountNum < private.HybridMinRosterTotalCountNum) {
                                private.UseHybridMode = false;
                                private.UseServerPaging = false;
                                Dashboard.GetGroupTotals();
                                Dashboard.GetRoster();
                            } else {
                                private.isHybridAllDataReceived = false;
                                Dashboard.GetOnlyRosterOrHybridMode();
                            }
                        } else {
                            Dashboard.NoData();
                        }
                    } else {
                        Dashboard.ShowRosterError();
                    }
                },
                error: function () {
                    //showRosterError();
                },
                complete: function (jqXhr, textStatus) {
                    //console.log("GetRosterRowsCount complete");
                }
            });
        },

        GetCutScore: function (groupingType, filter) {
            Dashboard.redrawRightCardTable("init");
            $.ajax({
                async: true,
                type: "GET",
                dataType: "json",
                url: siteRoot + "/DashboardCogat/GetCutScore?groupingType=" + groupingType + "&filter=" + filter,
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        private.modelCutScore = data.values;
                        Dashboard.EnableRightCardPopupButton();
                        Dashboard.UpdateRightCard();
                    } else {
                        Dashboard.ShowRosterError();
                    }
                },
                error: function () {
                    //showRosterError();
                },
                complete: function (jqXhr, textStatus) {
                    //console.log("GetRosterRowsCount complete");
                }
            });
        },

        GetBackgroundLocation: function () {
            console.log('called');
            $.ajax({
                async: true,
                type: "GET",
                dataType: "json",
                url: siteRoot + "/api/CogatDataToJson/GetBackGroundGradeLocations",
                success: function (data) {
                  
                    var [gradeData] = data.Filters.filter((item) => item.DisplayName === 'Grades');
                    var [buildingData] = data.Filters.filter((item) => item.DisplayName === 'Building');
                    var [classData] = data.Filters.filter((item) => item.DisplayName === 'Section');
                    var [studentData] = data.Filters.filter((item) => item.DisplayName === 'Students');
                    var gradeItems = gradeData.Items;
                    private.printReportBuilding = buildingData.Items;
                    var classItems = classData.Items;
/*
                    private.printReportClasses = classItems.filter((item, index, selfArray) =>
                        index === selfArray.findIndex((p) => (
                            p.ParentLocationId === item.ParentLocationId && p.Value === item.Value
                        ))
                    );
*/
                    private.printReportClasses = [];
                    for (var i = 0; i < classItems.length; ++i) {
	                let isExist = false;
	                for (var j = 0; j < private.printReportClasses.length; ++j) {
                            if (private.printReportClasses[j].ParentLocationId === classItems[i].ParentLocationId && private.printReportClasses[j].Value === classItems[i].Value) {
                                isExist = true;
                                break;
                            }
                        }
                        if (!isExist) {
                            private.printReportClasses.push(classItems[i]);
                        } else {
                            let arrGrades = private.printReportClasses[j].GradeId.split(",");
                            if (arrGrades.indexOf(classItems[i].GradeId) === -1) {
                                private.printReportClasses[j].GradeId += "," + classItems[i].GradeId;
                            }
                        }
                    }

                    if (private.LoggedUserLevel === 'CLASS' || private.LoggedUserLevel === 'BUILDING') {
                        private.printReportStudents = studentData.Items;
                    }
                    //const groups = Object.create(null);
                    //const grouped = [];

                    //filteredClassItems.forEach(function (o) {
                    //    if (!groups[o.ParentLocationId]) {
                    //        groups[o.ParentLocationId] = [];
                    //        grouped.push({ buildingId: o.ParentLocationId, buildingName: o.ParentLocationName, classes: groups[o.group] });
                    //    }
                    //    groups[o.group].push(o.color);
                    //});
                    private.printReportGrades = [];
                    for (var i = 0; i < gradeItems.length; ++i) {
                        private.printReportGrades.push(gradeItems[i]);
                    }
                    //for (var i = 0; i < buildingItems.length; ++i) {
                    //    var buildingId = buildingItems[i].Value;
                    //    var classList = [];
                    //    for (var j = 0; j < filteredClassItems.length; ++j) {
                    //        if (buildingId === filteredClassItems[j].ParentLocationId) {
                    //            classList.push(filteredClassItems[j]);
                    //        }
                    //    }
                    //    private.printReportBuildingAndClasses.push({ ...buildingItems[i]})
                    //}
                },
                error: function () {
                    console.log("private.printReportGrades Failed, private.printReportGrades", private.printReportGrades);
                },
            });
        },

        GetAgeStanines: function () {
            if (private.UseServerPaging) {
                if (!private.isProdEnvironment) {
                    $("#debug-api-age-stanine textarea").text("");
                    $("#debug-api-age-stanine span").text("");
                }

                $("#stanine-table-selected-label").empty();
                Dashboard.CheckAgeStanineEnabled();

                $.ajax({
                    async: true,
                    type: "GET",
                    dataType: "json",
                    url: siteRoot + "/DashboardCogat/GetAgeStanines",
                    success: function (data) {
                        if (!private.isProdEnvironment) {
                            $("#debug-api-age-stanine textarea").text(JSON.stringify(data.api_params));
                            $("#debug-api-age-stanine span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        //DmUiLibrary.HideOverlaySpinner();

                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            Dashboard.GenerateStanineGridFromAjaxRequest(data.values);
                            Dashboard.isAll3AjaxRequestFinished();
                        } else {
                            Dashboard.ShowRosterError();
                        }
                    },
                    error: function () {
                        //showRosterError();
                    },
                    complete: function (jqXhr, textStatus) {
                        //console.log("GetAgeStanines complete");
                    }
                });
            }
        },

        GetGroupTotals: function () {
            if (!private.isProdEnvironment) {
                $("#debug-api-roster-group-total textarea").text("");
                $("#debug-api-roster-group-total span").text("");
            }

            private.modelRosterGroupTotals = {};

            var numStudentsUnchecked = $("#Student_Control option:not(:selected)").length;
            if (!numStudentsUnchecked) {
                $.ajax({
                    async: true,
                    type: "GET",
                    dataType: "json",
                    url: siteRoot + "/DashboardCogat/GetGroupTotals",
                    success: function (data) {
                        if (!private.isProdEnvironment) {
                            $("#debug-api-roster-group-total textarea").text(JSON.stringify(data.api_params));
                            $("#debug-api-roster-group-total span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            for (var key in data.group_total) {
                                if (key.substr(0, 2) === "RS") {
                                    data.group_total[key] = "";
                                }
                                if (key.substr(0, 4) === "NANI" && data.group_total[key] === "0/0") {
                                    data.group_total[key] = "";
                                }
                            }

                            private.modelRosterGroupTotals = data;
                            if (!$.isEmptyObject(private.modelRoster) && typeof private.modelRoster.values !== "undefined" && private.modelRoster.values.length) {
                                //Dashboard.UpdateRosterGrid();
                                Dashboard.UpdateRosterGroupTotal();
                            }
                        } else {
                            Dashboard.ShowRosterError();
                        }
                    },
                    error: function (data) {
                        if (data.responseText === "Unauthorized") {
                            window.location.href = returnUrl;
                        }
                    },
                    complete: function (jqXhr, textStatus) {
                        //console.log("GetGroupTotals complete");
                    }
                });
            }
        },

        GetRoster: function () {
            if (private.LoggedUserLevel === "CLASS") {
                private.DifferentiatedReportDistrictName = "";
                private.DifferentiatedReportBuildingName = "";
                private.DifferentiatedReportClassName = "";
            }

            if (!private.isProdEnvironment) {
                $("#debug-api-roster textarea").text("");
                $("#debug-api-roster span").text("");
            }

            private.CurrentRosterFilter = {};
            //private.CurrentPopupCriteriaRosterFilter = {};
            //private.modelRoster = {};
            private.modelFilteredRoster = {};
            private.modelFilteredRosterRightTopCard = {};
            //private.modelRosterAllColumns = {};
            //private.arrColumnsSelectedInRoster = [];
            private.arrColumnsSelectedInRightCardPopupFilter = [];

            Dashboard.ClearAllFiltersSelection();
            Dashboard.EnableRosterFiltersPopupButton();
            Dashboard.DisableRosterResetButton();
            Dashboard.DisableRosterScoreWarningFiltersPopupButton();

            if (!private.HybridGetAllRosterFlag) {
                //Dashboard.InitAllCards();
                private.modelRoster = {};
            }

            if (private.UseServerPaging && !private.HybridGetAllRosterFlag) {
                //Dashboard.UpdateRosterGrid();
                DmUiLibrary.HideOverlaySpinner();
                $("#roster-table-wrapper").empty();

                if (private.arrColumnsSelectedInRoster.length === 0) {
                    private.arrColumnsSelectedInRoster = ["APR"];
                }
                /*
                                private.arrSelectedTopFilterScores = [];
                                $(".filters select#Content_Control option").each(function (i) {
                                    if ($(this).prop("selected")) {
                                        private.arrSelectedTopFilterScores.push(i);
                                    }
                                });
                */

                var data2 = getRosterAllColumnsStructure(private.arrSelectedTopFilterScores);
                private.modelRoster = data2;
                private.modelRosterAllColumns = data2.columns;

                Dashboard.UpdateRosterGrid();


                /*
                                $("#roster-table-wrapper").empty(); //must to clear before recreate data!
                                $("#roster-table-wrapper").GenerateRosterTable({
                                    //'data': getSampleJsonRosterTable(studentsNum),
                                    //'data': data,
                                    'data': private.modelRoster,
                                    'class_of_table': "dm-ui-table"
                                });
                */
            } else {
                if (private.HybridGetAllRosterFlag) {
                    Dashboard.StartRosterProgressBar();
                    //private.HybridGetAllRosterFlag = false;
                }
                $("#roster-search-field").addClass("disabled-element");
                $.ajax({
                    async: true,
                    type: "GET",
                    dataType: "json",
                    //url: siteRoot + "/DashboardCogat/GetStudentRoster?take=99999&skip=0&filter=",
                    url: siteRoot + "/DashboardCogat/GetAllData",
                    success: function (data) {
                        private.isHybridAllDataReceived = true;
                        if (!private.isProdEnvironment) {
                            $("#debug-api-roster textarea").text(JSON.stringify(data.api_params));
                            $("#debug-api-roster span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        DmUiLibrary.HideOverlaySpinner();

                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            if (data.values.length) {
                                $("#roster-table-wrapper").empty();
                                /*
                                 //uncomment if you wanna delete null field & fields props oc columns in json
                                for (var i = 0; i < data.columns.length; ++i) {
                                    if (data.columns[i].field === null) {
                                        delete data.columns[i].field;
                                    }
                                    if (data.columns[i].fields === null) {
                                        delete data.columns[i].fields;
                                    }
                                }
                                //uncomment if you wanna replace null props of values to -1
                                for (var i = 0; i < data.values.length; ++i) {
                                    for (key in data.values[i]) {
                                        if (data.values[i][key] === null) {
                                            data.values[i][key] = -1;
                                        }
                                    }
                                }
                                */

                                if (private.arrColumnsSelectedInRoster.length === 0) {
                                    private.arrColumnsSelectedInRoster = ["APR"];
                                }
                                private.HybridGetAllRosterFlag = false;
                                private.modelRoster = data;
                                private.modelRosterAllColumns = data.columns;

                                //$(".filters #Content_Control_dm_ui .dm-ui-dropdown-items li:last-child input").attr("disabled", "True");

                                if (private.UseHybridModeV2) {
                                    private.UseServerPaging = false;
                                }
                                if ($("#stanine-table-selected-label").text().trim() === "" && $("#right-card-selected-label").text().trim() === "") {
                                    Dashboard.GenerateStanineGridFromStudentRoster();
                                }
                                Dashboard.UpdateCogatFormNormYear();
                                Dashboard.UpdateRosterGrid();
                                $("#roster-table-wrapper").data("kendoGrid").pager.page(private.RosterCurrentPageNumber);

                                //if (private.LoggedUserLevel !== "CLASS") {
                                if (true) {
                                    //Dashboard.UpdateRightCard();
                                }
                                if ((private.LoggedUserLevel === "BUILDING" || private.LoggedUserLevel === "CLASS") && !private.isScreener) {
                                    //if (true) {
                                    Dashboard.UpdateRightCard2();
                                }
                                Dashboard.StopRosterProgressBar();
                                if (private.LoggedUserLevel === "CLASS" || private.LoggedUserLevel === "BUILDING") {
                                    for (var i = 0; i < data.values.length; ++i) {
                                        if (data.values[i].building) {
                                            private.DifferentiatedReportDistrictName = data.values[i].district;
                                            private.DifferentiatedReportBuildingName = data.values[i].building;
                                            private.DifferentiatedReportClassName = data.values[i].class_name;
                                            break;
                                        }
                                    }
                                }
                                $("#roster-search-field").removeClass("disabled-element");

                                if (!private.isCutScoreCardGenerated) {
                                    Dashboard.AutomaticCutScoreRequest();
                                }
                                if (!$.isEmptyObject(private.objLastSort)) {
                                    $("#roster-table-wrapper").data("kendoGrid").dataSource.sort(private.objLastSort);
                                }
                            } else {
                                Dashboard.NoData();
                            }
                        } else {
                            //Dashboard.NoData();
                            Dashboard.ShowRosterError();
                        }
                    },
                    error: function () {
                        //showRosterError();
                    },
                    complete: function (jqXhr, textStatus) {
                        //console.log("GetStudentRoster complete");
                    }
                });
            }
        },

        AutomaticCutScoreRequest: function () {
            Dashboard.resetInputValuesOfRosterSearchPopup();
            Dashboard.RestoreAppliedPopupFiltersFromCookie();
            private.isPopupFiltersOfRightCard = true;
            private.EnforcedCutScoreResults = true;
            $("#modal-dashboard-report-criteria #apply-dashboard-report-button").click();
            private.EnforcedCutScoreResults = false;
        },

        setGridCellCurrentState: function (element) {
            if (element !== undefined && element !== null) {
                var grid;
                if (element.parents("#roster-table-wrapper").length) {
                    grid = $("#roster-table-wrapper").data("kendoGrid");
                } else {
                    grid = $(element.parents(".dashboard-table")).data("kendoGrid");
                }
                grid.current(element);
                element.removeAttr("role");
            }
        },

        GenerateRightCardTable: function (element, data) {
            var arrValues = [];
            var tmpObj;
            var i;
            var name, id;
            var arrBuildingCount = {};
            var arrId = {};
            var arrName = {};
            var collectField = "building";
            var collectFieldId = "building_id";
            var arr

            if (private.UseServerPaging) {
                collectField = "Building_name";
                collectFieldId = "Building_id";
            }
            var columnTitle = private.SuppressedTextBuilding;

            if (private.LoggedUserLevel === "BUILDING") {
                collectField = "class_name";
                collectFieldId = "class_id";
                if (private.UseServerPaging) {
                    collectField = "Class_name";
                    collectFieldId = "Class_id";
                }
                columnTitle = private.SuppressedTextClass;
            }

            data = {
                "columns": [
                    { "title": columnTitle, "field": "content_area" },
                    { "title": "No.", "field": "number" },
                    { "title": "", "field": "percent" }
                ],
                "values": [
                ]
            };

            if (private.UseServerPaging) {
                for (i = 0; i < private.modelCutScore.length; ++i) {
                    name = private.modelCutScore[i][collectField];
                    id = private.modelCutScore[i][collectFieldId];
                    if (!arrBuildingCount.hasOwnProperty(name)) {
                        arrBuildingCount[id] = private.modelCutScore[i]["Test_count"];
                        arrId[id] = id;
                        arrName[id] = name;
                    }
                }
            } else {
                //for (i = 0; i < private.modelRoster.values.length; ++i) {
                for (i = 0; i < private.modelFilteredRosterRightTopCard.length; ++i) {
                    //name = private.modelRoster.values[i][collectField];
                    name = private.modelFilteredRosterRightTopCard[i][collectField];
                    id = private.modelFilteredRosterRightTopCard[i][collectFieldId];
                    if (arrBuildingCount.hasOwnProperty(id)) {
                        arrBuildingCount[id]++;
                    } else {
                        arrBuildingCount[id] = 1;
                        arrId[id] = id;
                        //arrName[id] = name;
                        arrName[id] = Dashboard.FirstLocationOnly(name);
                    }
                }
            }

            for (var key in arrBuildingCount) {
                if (arrBuildingCount.hasOwnProperty(key)) {
                    tmpObj = {
                        "content_area": arrName[key],
                        "number": arrBuildingCount[key],
                        "id": arrId[key]
                    };
                    arrValues.push(tmpObj);
                }
            }
            //arrValues.sort(function (obj1, obj2) { return obj2.number - obj1.number; }); //ordering by number
            arrValues.sort(function (obj1, obj2) { //ordering by content_area
                if (obj1.content_area < obj2.content_area) {
                    return -1;
                }
                if (obj1.content_area > obj2.content_area) {
                    return 1;
                }
                return 0;
            });

            var studentsTotalNum = 0;
            for (i = 0; i < arrValues.length; ++i) {
                studentsTotalNum += arrValues[i].number;
            }
            //console.log(arrValues);
            private.RowsNumberRightCard = arrValues.length;

            $("#right-card-table-number").text(studentsTotalNum);


            data.values = arrValues;

            var tmpValue;
            var rosterColumns = [];
            for (i = 0; i < data.columns.length; ++i) {
                //rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: "<a href='#:link#' class='location-drill' tabindex='-1'>#:" + data.columns[i].title + "#</a>" });
                if (i === 2) {
                    //rosterColumns.push({ field: data.columns[i].field, headerTemplate: '<span class="scale-column-title">1 &nbsp;&nbsp;&nbsp;&nbsp; 25 &nbsp;&nbsp;&nbsp; 50+</span>', template: '<span class="kendogrid-chart-line" data-width="#:number#"></span>', sortable: false });
                    //rosterColumns.push({ field: data.columns[i].field, headerTemplate: '<span class="scale-column-title"><span>1&nbsp;&nbsp;</span>25<span>50+</span></span>', template: '<span class="kendogrid-chart-line" style="width:#= (number > 50) ? "100" : number*2 #%;"></span>', sortable: false });
                    if (private.LoggedUserLevel === "BUILDING") {
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: '<span class="scale-column-title"><span>1</span><span>25</span><span>50+</span></span>', template: '<span class="kendogrid-chart-line" style="width:#= (number > 50) ? "100" : number*2 #%;"></span><span class="sr-only">Scale value #=number#</span>', sortable: false });
                    } else {
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: '<span class="scale-column-title"><span>1</span><span>50</span><span>100+</span></span>', template: '<span class="kendogrid-chart-line" style="width:#= (number > 100) ? "100" : number #%;"></span><span class="sr-only">Scale value #=number#</span>', sortable: false });
                    }
                } else if (i === 1) {
                    //rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: '<a href="\\#" class="stanine-table-link" data-id="#:id#" tabindex="-1">#:number#</a>' });
                    if (private.isTabNavigationOn) {
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: '<span class="stanine-table-link" data-id="#:id#">#:number#</span>' });
                    } else {
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: '<span class="stanine-table-link tab" tabindex="0" data-id="#:id#">#:number#</span>' });
                    }
                } else {
                    rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title });
                }
            }

            try {
                $(element).kendoGrid({
                    dataSource: {
                        data: data.values,
                        //pageSize: 20
                        //pageSize: 6
                        pageSize: private.paginationRightCard
                    },
                    pageable: {
                        //pageSizes: [5, 10, 25, 50],
                        input: false,
                        numeric: true,
                        buttonCount: 4,
                        info: false,
                        alwaysVisible: false,
                        change: function () {
                            //console.log("pager change event");
                            Dashboard.WcagPagination("#right-card-table-wrapper");
                            if ($("body").hasClass("wcag_focuses_on") && $("#right-card1.root-tab-element").attr("tabindex") === "-1" && private.isTabNavigationOn) {
                                $(".dashboard-table input.k-textbox, .dashboard-table .k-widget.k-dropdown, .dashboard-table a.k-pager-nav").removeClass("tab first-tab-element last-tab-element");
                                setTimeout(function () {
                                    $("#right-card1.root-tab-element").rootMakeContentTabbable();
                                    var focusedElement = $(":focus");
                                    if (focusedElement.hasClass("k-state-disabled")) {
                                        $("#right-card1 .last-tab-element").focus();
                                    }
                                }, 100);
                            }
                        }
                    },
                    dataBound: function () {
                        $("#right-card-table-wrapper > table tbody tr td:first-child").attr("scope", "row");
                        $("#right-card-table-wrapper > table tbody tr td:first-child").each(function () {
                            $(this).replaceWith($(this)[0].outerHTML.replace("<td ", "<th ").replace("</td>", "</th>"));
                        });
                        setTimeout(function () {
                            Dashboard.WcagPagination("#right-card-table-wrapper");
                        }, 100);
                    },
                    //resizable: true,
                    scrollable: false,
                    persistSelection: true, //see parameter in schema->model-> id: "node_id",
                    sortable: {
                        mode: "single",
                        showIndexes: true,
                        allowUnsort: true
                    },
                    sort: function () {
                        $(".kendo_sorted_column").addClass("grid-sort-filter-icon k-icon");
                    },
                    navigatable: true,
                    navigate: function (e) {
                        Dashboard.setGridCellCurrentState(e.element);
                        $("#right-card1.root-tab-element").rootMakeContentTabbable();
                        e.element.focus();
                        private.RosterGridNavigationEvent = true;
                    },
                    columns: rosterColumns
                });

                Dashboard.RecalculatePageSizeOfRightCards();
                /*
                                var lineWidth;
                                $(".kendogrid-chart-line").each(function () {
                                    lineWidth = Number($(this).data("width"));
                                    if (lineWidth > 50) {
                                        lineWidth = 50;
                                    }
                                    $(this).css("width", lineWidth/50*100 + "%");
                                });
                */
            } catch (e) {
                $("body").append('<div class="kendo-errors">Error RightCardTable=' + e.name + ": " + e.message + " | " + e.stack + "</div>");
            }
            $("#right-card-table-wrapper > table").removeAttr("tabindex"); //if not removed -kendo table will be the last focused element on page
            $("#right-card-table-wrapper > table").removeAttr("role");
            $("#right-card-table-wrapper > table thead th[data-role=columnsorter] a").attr("role", "button");
            setTimeout(function () {
                $("#right-card-table-wrapper > table td, #right-card-table-wrapper > table tbody th").removeAttr("role");
                if (!$("#right-card1").hasClass("overlayed")) {
                    if (private.isTabNavigationOn) {
                        $("#right-card-table-wrapper > table td, #right-card-table-wrapper > table th").attr("tabindex", "0").addClass("tab");
                    } else {
                        $("#right-card-table-wrapper > table thead th[data-role=\"columnsorter\"]").addClass("tab").attr("tabindex", "0");
                    }
                }
            }, 1000);
        },

        GenerateAbilityProfileLink: function (text) {
            if (text === null) return "#";
            var arrProfile = {
                "A": "1",
                "B": "2",
                "C": "3",
                "E": "4"
            };
            var stanine = text.substr(0, 1);
            var profile = arrProfile[text.substr(1, 1)];
            var strength = "0";
            if (text.indexOf("V+)") !== -1 || text.indexOf("(V+") !== -1) strength = "1";
            if (text.indexOf("Q+)") !== -1 || text.indexOf("(Q+") !== -1) strength = "2";
            if (text.indexOf("N+)") !== -1 || text.indexOf("(N+") !== -1) strength = "3";
            var weakness = "0";
            if (text.indexOf("V-)") !== -1 || text.indexOf("(V-") !== -1) weakness = "1";
            if (text.indexOf("Q-)") !== -1 || text.indexOf("(Q-") !== -1) weakness = "2";
            if (text.indexOf("N-)") !== -1 || text.indexOf("(N-") !== -1) weakness = "3";

            return "https://www.riversideinsights.com/apps/cogat?stanine=" + stanine + "&profile=" + profile + "&strength=" + strength + "&weakness=" + weakness;
        },

        GenerateRightCardTable2: function (element, dataAjax) {
            var arrValues = [];
            var tmpObj;
            var i, key;

            var ability;
            var arrAbilityCount = {};

            /*
                        //if wanna use 'ability_profile_distribution' prop of roster json
                        var model = private.modelRoster.ability_profile_distribution;
                        for (var key in model) {
                            if (model.hasOwnProperty(key)) {
                                if (key !== "") {
                                    tmpObj = {
                                        "ability_profile": key,
                                        "number": model[key],
                                        "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
                                    };
                                    arrValues.push(tmpObj);
                                }
                            }
                        }
            */

            var data = {
                "columns": [
                    { "title": "Ability Profile", "field": "ability_profile" },
                    { "title": "Number of Students", "field": "number" }
                ],
                "values": [
                ]
            };

            if (private.UseServerPaging) {
                for (i = 0; i < dataAjax.length; ++i) {
                    ability = dataAjax[i].Profile_display;
                    arrAbilityCount[ability] = dataAjax[i].Profile_number_included;
                }
            } else {
                //for (i = 0; i < private.modelRoster.values.length; ++i) {
                for (i = 0; i < private.modelFilteredRoster.length; ++i) {
                    //ability = private.modelRoster.values[i].ability_profile;
                    ability = private.modelFilteredRoster[i].ability_profile;
                    if (ability !== "") {
                        if (arrAbilityCount.hasOwnProperty(ability)) {
                            arrAbilityCount[ability]++;
                        } else {
                            arrAbilityCount[ability] = 1;
                        }
                    }
                }
            }
            for (key in arrAbilityCount) {
                if (arrAbilityCount.hasOwnProperty(key)) {
                    tmpObj = {
                        "ability_profile": key,
                        "number": arrAbilityCount[key],
                        //"link": Dashboard.GenerateAbilityProfileLink(key)
                        "link": "#"
                    };
                    arrValues.push(tmpObj);
                }
            }

            //arrValues.sort(function (obj1, obj2) { return obj2.number - obj1.number; }); //ordering by number
            arrValues.sort(function (obj1, obj2) { //ordering by ability_profile
                if (obj1.ability_profile < obj2.ability_profile) {
                    return -1;
                }
                if (obj1.ability_profile > obj2.ability_profile) {
                    return 1;
                }
                return 0;
            });

            //grouping by ability
            var arrGroupedAbility = [], arrGroupedSum = [], arrGroupedValues = [];
            for (i = 0; i < arrValues.length; ++i) {
                key = findAbilityGroup(arrValues[i].ability_profile);
                //if ($.inArray(key, arrGroupedAbility) !== -1) {
                if (arrGroupedAbility.indexOf(key) !== -1) {
                    arrGroupedSum[key] += arrValues[i].number;
                } else {
                    arrGroupedAbility.push(key);
                    arrGroupedSum[key] = arrValues[i].number;
                }
            }
            for (i = 0; i < arrGroupedAbility.length; ++i) {
                tmpObj = {
                    "ability_profile": arrGroupedAbility[i],
                    "number": arrGroupedSum[arrGroupedAbility[i]],
                    "link": "#"
                };
                arrGroupedValues.push(tmpObj);
            }
            arrValues = arrGroupedValues;
            arrValues.sort(function (obj1, obj2) { //ordering by ability_profile
                if (obj1.ability_profile < obj2.ability_profile) {
                    return -1;
                }
                if (obj1.ability_profile > obj2.ability_profile) {
                    return 1;
                }
                return 0;
            });

            $("#right-card-table2-number").text(arrValues.length);


            data.values = arrValues;

            private.RowsNumberRightCard2 = arrValues.length;

            function splitAbilities(str) {
                return str;
            }
            var rosterColumns = [];
            var tmpValue, j, tmpArr;
            for (i = 0; i < data.columns.length; ++i) {
                if (i === 0) {
                    //rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, sortable: false, template: '<a href="#:link#" class="ability-profile-link" tabindex="-1" target="_blank">#:ability_profile#</a>' });
                    rosterColumns.push({
                        field: data.columns[i].field, headerTemplate: data.columns[i].title, sortable: false, template:
                            function (dataItem) {
                                tmpArr = dataItem.ability_profile.split(", ");
                                tmpValue = "";
                                for (j = 0; j < tmpArr.length; ++j) {
                                    if (tmpValue !== "") {
                                        tmpValue += ", ";
                                    }
                                    //tmpValue += '<a href="#" class="ability-profile-link" tabindex="-1" target="_blank">' + tmpArr[j] + "</a>";
                                    if (private.isTabNavigationOn) {
                                        tmpValue += '<span class="ability-profile-link">' + tmpArr[j] + "</span>";
                                    } else {
                                        tmpValue += '<span class="ability-profile-link tab" tabindex="0">' + tmpArr[j] + "</span>";
                                    }
                                }
                                return tmpValue;
                            }
                    });
                } else {
                    //if (private.UseHybridMode && private.UseServerPaging && !private.UseHybridModeV2) {
                    if (private.UseServerPaging) {
                        //rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: '<span class="stanine-table-link">#:number#</span>' });
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: "#:number#" });
                    } else {
                        //rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: '<a href="\\#" class="stanine-table-link" tabindex="-1">#:number#</a>' });
                        if (private.isTabNavigationOn) {
                            rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: '<span class="stanine-table-link">#:number#</span>' });
                        } else {
                            rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title, template: '<span class="stanine-table-link tab" tabindex="0">#:number#</span>' });
                        }
                    }
                }
            }

            try {
                $(element).kendoGrid({
                    dataSource: {
                        data: data.values,
                        //pageSize: 20
                        //pageSize: 6
                        pageSize: private.paginationRightCard2
                    },
                    pageable: {
                        //pageSizes: [5, 10, 25, 50],
                        input: false,
                        numeric: true,
                        buttonCount: 4,
                        info: false,
                        alwaysVisible: false,
                        change: function () {
                            //console.log("pager change event");
                            Dashboard.WcagPagination("#right-card-table2-wrapper");
                            if ($("body").hasClass("wcag_focuses_on") && $("#right-card2.root-tab-element").attr("tabindex") === "-1" && private.isTabNavigationOn) {
                                $(".dashboard-table input.k-textbox, .dashboard-table .k-widget.k-dropdown, .dashboard-table a.k-pager-nav").removeClass("tab first-tab-element last-tab-element");
                                setTimeout(function () {
                                    $("#right-card2.root-tab-element").rootMakeContentTabbable();
                                    var focusedElement = $(":focus");
                                    if (focusedElement.hasClass("k-state-disabled")) {
                                        $("#right-card2 .last-tab-element").focus();
                                    }
                                }, 100);
                            }
                        }
                    },
                    dataBound: function () {
                        $("#right-card-table2-wrapper > table tbody tr td:first-child").attr("scope", "row");
                        $("#right-card-table2-wrapper > table tbody tr td:first-child").each(function () {
                            $(this).replaceWith($(this)[0].outerHTML.replace("<td ", "<th ").replace("</td>", "</th>"));
                        });
                        setTimeout(function () {
                            Dashboard.WcagPagination("#right-card-table2-wrapper");
                        }, 100);
                    },
                    //resizable: true,
                    scrollable: false,
                    persistSelection: true, //see parameter in schema->model-> id: "node_id",
                    sortable: {
                        mode: "single",
                        showIndexes: true,
                        allowUnsort: true
                    },
                    sort: function () {
                        $(".kendo_sorted_column").addClass("grid-sort-filter-icon k-icon");
                    },
                    navigatable: true,
                    navigate: function (e) {
                        Dashboard.setGridCellCurrentState(e.element);
                        $("#right-card2.root-tab-element").rootMakeContentTabbable();
                        e.element.focus();
                        private.RosterGridNavigationEvent = true;
                    },
                    columns: rosterColumns
                });
                Dashboard.RecalculatePageSizeOfRightCards();
            } catch (e) {
                $("body").append('<div class="kendo-errors">Error RightCardTable2=' + e.name + ": " + e.message + " | " + e.stack + "</div>");
            }
            $("#right-card-table2-wrapper > table").removeAttr("tabindex"); //if not removed -kendo table will be the last focused element on page
            $("#right-card-table2-wrapper > table").removeAttr("role");
            $("#right-card-table2-wrapper > table thead th[data-role=columnsorter] a").attr("role", "button");
            setTimeout(function () {
                $("#right-card-table2-wrapper > table td, #right-card-table2-wrapper > table tbody th").removeAttr("role");
                if (private.isTabNavigationOn) {
                    $("#right-card-table2-wrapper > table td, #right-card-table2-wrapper > table th").attr("tabindex", "0").addClass("tab");
                } else {
                    $("#right-card-table2-wrapper > table thead th[data-role=\"columnsorter\"]").addClass("tab").attr("tabindex", "0");
                }
            }, 1000);
        },

        WcagPagination: function (element) {
            if (typeof $(element) !== "undefined") {
                $(element + " .k-pager-wrap > a.k-pager-nav").removeAttr("aria-disabled");
                $(element + " .k-pager-wrap > a.k-pager-nav.k-state-disabled").attr("aria-disabled", "true").attr("tabindex", "-1");
                if (!private.isTabNavigationOn) {
                    $(element + " .k-pager-wrap > a.k-pager-nav").attr("tabindex", "0");
                    $(element + " .k-pager-wrap > a.k-pager-nav.k-state-disabled").attr("tabindex", "-1");
                }
                var pageAriaLabel = $(element + " .k-pager-wrap > .k-pager-input .k-textbox").attr("aria-label");
                if (typeof pageAriaLabel !== "undefined") {
                    if (pageAriaLabel.indexOf("Page ") === -1) {
                        //$(element + " .k-pager-wrap > .k-pager-input .k-textbox").attr("aria-label", "Page " + pageAriaLabel);
                        $(element + " .k-pager-wrap > .k-pager-input .k-textbox").attr("aria-label", "Page");
                    }
                }
                var dropdownHeader = $(element + " .k-pager-wrap > .k-pager-sizes.k-label");
                if (dropdownHeader.length) {
                    if (dropdownHeader.html().indexOf("items per page</label>") === -1) {
                        dropdownHeader.append('<label for="roster-paging-select" id="roster-paging-label" style="display: none">items per page</label>');
                        dropdownHeader.find("select").attr("id", "roster-paging-select");
                        dropdownHeader.find("> .k-dropdown").attr("aria-labelledby", "roster-paging-label");
                    }
                }
            }
        },

        RosterLegendVisibleValuesIOnly: function () {
            Dashboard.AddWarningsLegend($("#roster-table-wrapper").data("kendoGrid").dataSource.view());
        },

        ChangePageSizeCardTable: function (pageSize) {
            if (typeof $("#right-card-table-wrapper").data("kendoGrid") !== "undefined") {
                $("#right-card-table-wrapper").data("kendoGrid").dataSource.pageSize(pageSize);
                $("#right-card-table-wrapper").data("kendoGrid").refresh();
            }
        },

        ChangePageSizeCardTable2: function (pageSize) {
            if (typeof $("#right-card-table2-wrapper").data("kendoGrid") !== "undefined") {
                $("#right-card-table2-wrapper").data("kendoGrid").dataSource.pageSize(pageSize);
                $("#right-card-table2-wrapper").data("kendoGrid").refresh();
            }
        },

        RecalculatePageSizeOfRightCards: function () {
            //console.log("---------------------------------");
            //console.log("RowsNumberStanineCard=" + private.RowsNumberStanineCard);
            //console.log("RowsNumberRightCard=" + private.RowsNumberRightCard);
            //console.log("RowsNumberRightCard2=" + private.RowsNumberRightCard2);
            //console.log("---------------------------------");
            if ($("#right-card-table-wrapper table tr.selected-right-card-tr").length === 0) {  //to prevent cards recalculation when one of right top card link is clicked
                var correction = 0;

                var correctionStanineLegend = 0;
                for (var i = 3; i <= 6; ++i) {
                    if (private.arrSelectedTopFilterScores.indexOf(i) !== -1) {
                        correctionStanineLegend++;
                    }
                }

                if ($("#right-card1").is(":visible") && $("#right-card2").is(":visible")) {
                    if (private.RowsNumberStanineCard + 5 >= private.RowsNumberRightCard + private.RowsNumberRightCard2) {
                        if (private.RowsNumberRightCard > 0) {
                            private.paginationRightCard = private.RowsNumberRightCard;
                        }
                        if (private.RowsNumberRightCard2 > 0) {
                            private.paginationRightCard2 = private.RowsNumberRightCard2;
                        }
                    } else {
                        if (private.RowsNumberRightCard > private.RowsNumberStanineCard - 2) {
                            private.paginationRightCard = private.RowsNumberStanineCard - 2;
                            if (private.paginationRightCard <= private.RowsNumberRightCard) {
                                private.paginationRightCard = private.RowsNumberRightCard;
                            }
                        } else {
                            private.paginationRightCard = private.RowsNumberRightCard;
                            if (private.RowsNumberRightCard > 0) {
                                if (private.RowsNumberStanineCard === 4) {
                                    correction = correction + 2;
                                } else {
                                    correction = correction + 1;
                                }
                            }
                        }
                        private.paginationRightCard2 = 13 + correction - private.paginationRightCard;

                        if (correctionStanineLegend < 2) {
                            if (private.RowsNumberStanineCard === 1) {
                                private.paginationRightCard2 = private.paginationRightCard2 - 7;
                            } else if (private.RowsNumberStanineCard === 4) {
                                private.paginationRightCard2 = private.paginationRightCard2 - 5;
                            } else {
                                private.paginationRightCard2 = private.paginationRightCard2 - 6;
                            }
                        }
                        if (correctionStanineLegend === 2) {
                            private.paginationRightCard2 = private.paginationRightCard2 - 3;
                        }
                        if (correctionStanineLegend === 3) {
                            private.paginationRightCard2 = private.paginationRightCard2 - 2;
                        }
                    }
                } else {
                    if ($("#right-card1").is(":visible")) {
                        if (private.RowsNumberStanineCard + 5 >= private.RowsNumberRightCard) {
                            if (private.RowsNumberRightCard > 0) {
                                private.paginationRightCard = private.RowsNumberRightCard;
                            }
                        } else {
                            if (private.RowsNumberRightCard > private.RowsNumberStanineCard + 14) {
                                private.paginationRightCard = private.RowsNumberStanineCard + 14;
                            } else {
                                private.paginationRightCard = private.RowsNumberRightCard;
                            }
                        }
                    } else {
                        if (private.RowsNumberStanineCard + 5 >= private.RowsNumberRightCard2) {
                            if (private.RowsNumberRightCard2 > 0) {
                                private.paginationRightCard2 = private.RowsNumberRightCard2;
                            }
                        } else {
                            if (private.RowsNumberRightCard2 > private.RowsNumberStanineCard + 14) {
                                private.paginationRightCard2 = private.RowsNumberStanineCard + 14;
                            } else {
                                private.paginationRightCard2 = private.RowsNumberRightCard2;
                            }
                        }
                    }

                    if (correctionStanineLegend < 2 && private.RowsNumberRightCard > 2) {
                        private.paginationRightCard = private.paginationRightCard - 2;
                    }
                    if (correctionStanineLegend < 2 && private.RowsNumberRightCard2 > 2) {
                        private.paginationRightCard2 = private.paginationRightCard2 - 2;
                    }
                    if ((correctionStanineLegend === 2 || correctionStanineLegend === 3) && private.RowsNumberRightCard > 1) {
                        private.paginationRightCard--;
                    }
                    if ((correctionStanineLegend === 2 || correctionStanineLegend === 3) && private.RowsNumberRightCard2 > 1) {
                        private.paginationRightCard2--;
                    }
                }

                if (private.paginationRightCard === private.RowsNumberRightCard) {
                    private.paginationRightCard++;
                }
                if (private.paginationRightCard2 === private.RowsNumberRightCard2) {
                    private.paginationRightCard2++;
                }

                Dashboard.ChangePageSizeCardTable(private.paginationRightCard);
                Dashboard.ChangePageSizeCardTable2(private.paginationRightCard2);

                //console.log("private.paginationRightCard=" + private.paginationRightCard);
                //console.log("private.paginationRightCard2=" + private.paginationRightCard2);
            }
        },

        ValidateReportName: function () {
            $(document).on("keyup", "#background-repot-pdf-file-name", function (e) {
                if ($(this).val().indexOf("/") != -1 || $(this).val().indexOf("*") != -1
                    || $(this).val().indexOf("<") != -1 || $(this).val().indexOf(">") != -1 ||
                    $(this).val().indexOf("|") != -1 || $(this).val().indexOf("?") != -1
                    || $(this).val().indexOf("\\") != -1 || $(this).val().indexOf("\"") != -1 ||
                    $(this).val().indexOf("+") != -1 || $(this).val().indexOf(":") != -1
                    || $(this).val().indexOf("#") != -1 || $(this).val().indexOf("%") != -1
                    || $(this).val().indexOf(",") != -1) {
                    $(this).addClass('invalid dm-ui-error');
                    $("#apply-dashboard-print-students").prop("disabled", true);
                    $(".dm-ui-error-text").show();
                } else {
                    $(this).removeClass('invalid dm-ui-error');
                    $("#apply-dashboard-print-students").prop("disabled", false);
                }
            });
        },
        GenerateStanineTable: function (element, data) {
            private.RowsNumberStanineCard = data.values.length;
            //console.log("private.RowsNumberStanineCard=" + private.RowsNumberStanineCard);

            var rosterColumns = [];
            var tmpValue;
            for (var i = 0; i < data.columns.length; ++i) {
                if (i === 0) {
                    rosterColumns.push({ field: data.columns[i].field, headerTemplate: data.columns[i].title });
                } else {
                    tmpValue = data.columns[i].field;

                    if (private.UseHybridMode && private.UseServerPaging && !private.UseHybridModeV2) {
                        if (private.isTabNavigationOn) {
                            rosterColumns.push(
                                {
                                    field: data.columns[i].field,
                                    headerTemplate: data.columns[i].title,
                                    template: "<#=(" + tmpValue + " == 0) ? 'span' : 'span'# class='stanine-table-link'>#=" + tmpValue + "#</#=(" + tmpValue + " == 0) ? 'span' : 'span'#>"
                                });
                        } else {
                            rosterColumns.push(
                                {
                                    field: data.columns[i].field,
                                    headerTemplate: data.columns[i].title,
                                    template: "<#=(" + tmpValue + " == 0) ? 'span' : 'span'# class='stanine-table-link tab' tabindex=\"0\">#=" + tmpValue + "#</#=(" + tmpValue + " == 0) ? 'span' : 'span'#>"
                                });
                        }
                    } else {
                        if (private.isTabNavigationOn) {
                            rosterColumns.push(
                                {
                                    field: data.columns[i].field,
                                    headerTemplate: data.columns[i].title,
                                    //template: "<#=(" + tmpValue + " == 0) ? 'span' : 'a href=\"\\#\" tabindex=\"-1\"'# class='stanine-table-link'>#=(" + tmpValue + " == 0) ? '*' : " + tmpValue + "#</#=(" + tmpValue + " == 0) ? 'span' : 'a'#>"
                                    //template: "<#=(" + tmpValue + " == 0) ? 'span' : 'a href=\"\\#\" tabindex=\"-1\"'# class='stanine-table-link'>#=" + tmpValue + "#</#=(" + tmpValue + " == 0) ? 'span' : 'a'#>"
                                    template: "<#=(" + tmpValue + " == 0) ? 'span' : 'span class=\"stanine-table-link\"'# >#=" + tmpValue + "#</#=(" + tmpValue + " == 0) ? 'span' : 'span'#>"
                                });
                        } else {
                            rosterColumns.push(
                                {
                                    field: data.columns[i].field,
                                    headerTemplate: data.columns[i].title,
                                    template: "<#=(" + tmpValue + " == 0) ? 'span' : 'span class=\"stanine-table-link tab\" tabindex=\"0\"'# >#=" + tmpValue + "#</#=(" + tmpValue + " == 0) ? 'span' : 'span'#>"
                                });
                        }
                    }
                }
            }

            try {
                $(element).kendoGrid({
                    dataSource: {
                        data: data.values
                    },
                    dataBound: function () {
                        $("#stanine-table-wrapper > table tbody tr td:first-child").attr("scope", "row");
                        $("#stanine-table-wrapper > table tbody tr td:first-child").each(function () {
                            $(this).replaceWith($(this)[0].outerHTML.replace("<td ", "<th ").replace("</td>", "</th>"));
                        });
                    },
                    //resizable: true,
                    scrollable: false,
                    persistSelection: true, //see parameter in schema->model-> id: "node_id",
                    sort: function () {
                        //$('.kendo_sorted_column').addClass("grid-sort-filter-icon k-icon");
                    },
                    navigatable: true,
                    navigate: function (e) {
                        Dashboard.setGridCellCurrentState(e.element);
                        $(".stanine-card.root-tab-element").rootMakeContentTabbable();
                        e.element.focus();
                        private.RosterGridNavigationEvent = true;
                    },
                    columns: rosterColumns
                });
            } catch (e) {
                $("body").append('<div class="kendo-errors">Error StanineTable=' + e.name + ": " + e.message + " | " + e.stack + "</div>");
            }

            //generate LEGEND under the Stanine Table
            var legend = '<h3 class="legend-header">Legend</h3>';
            legend += '<dl class="legend">';
            if (private.arrSelectedTopFilterScores.indexOf(3) !== -1) {
                legend += "<dt>VQ:</dt><dd>Composite Verbal and Quantitative</dd>";
            }
            if (private.arrSelectedTopFilterScores.indexOf(4) !== -1) {
                legend += "<dt>VN:</dt><dd>Composite Verbal and Nonverbal</dd>";
            }
            if (private.arrSelectedTopFilterScores.indexOf(5) !== -1) {
                legend += "<dt>QN:</dt><dd>Composite Quantitative and Nonverbal</dd>";
            }
            if (private.arrSelectedTopFilterScores.indexOf(6) !== -1) {
                legend += "<dt>VQN:</dt><dd>Overall composite score for Verbal, Quantitative, and Nonverbal</dd>";
            }
            legend += '<dt><span class="sr-only">Graph plot points:</span><span class="legend-scatter-plot"></span></dt><dd>Percent of Students Nationally</dd>';
            legend += '<dt></dt><dd>Numbers may not sum to 100% due to rounding</dd>';
            legend += "</dl>";
            $("#stanine-table-legend").html(legend);
            $("#stanine-table-wrapper > table").removeAttr("tabindex"); //if not removed -kendo table will be the last focused element on page
            $("#stanine-table-wrapper > table").removeAttr("role");
            $("#stanine-table-wrapper > table thead th[data-role=columnsorter] a").attr("role", "button");
            setTimeout(function () {
                $("#stanine-table-wrapper > table td, #stanine-table-wrapper > table tbody th").removeAttr("role");
                if (private.isTabNavigationOn) {
                    $("#stanine-table-wrapper > table td, #stanine-table-wrapper > table th").attr("tabindex", "0").addClass("tab");
                }
            }, 1000);
            if (private.isTabNavigationOn) {
                $("#dropdown-graph-content_dm_ui > button.dm-ui-dropdown-button").attr("tabindex", "-1");
            }
        },

        ConvertAppliedFilterObjectToString: function () {
            var parameters = "";
            var arrContent2 = ["Verbal", "Quant", "NonVerb", "CompVQ", "CompVN", "CompQN", "CompVQN"];
            var arrContent = ["VERBAL", "QUANT", "NONVERB", "COMPVQ", "COMPVN", "COMPQN", "COMPVQN"];

            var i, j, k, s, arrTmp = [], arrTmp2 = [];
            for (i = 0; i < private.arrSelectedTopFilterScores.length; i++) {
                arrTmp.push(arrContent[private.arrSelectedTopFilterScores[i]]);
                arrTmp2.push(arrContent2[private.arrSelectedTopFilterScores[i]]);
            }
            arrContent = arrTmp;
            arrContent2 = arrTmp2;

            var arrOperator = {
                "eq": "=",
                "gt": ">",
                "lt": "<",
                "gte": ">=",
                "lte": "<=",
                "contains": "CONTAINS",
                "startswith": "STARTSWITH"
            };

            var filter, filterLogic0 = "", filterLogic1 = "", filterLogic2 = "", filterLogic3 = "";

            function parseFilterObj(filter) {
                var contentType = filter.field.replace(/[0-9]/g, "");
                contentType = contentType.replace(/[-]/g, "");
                var contentNum = parseInt(filter.field.match(/\d+/));
                //return "S:" + contentType + ":'" + arrContent[contentNum] + "':" + arrOperator[filter.operator] + ":" + filter.value;
                if (filter.field === "building_id" || filter.field === "class_id") {
                    return "S:" + contentType + ":" + arrOperator[filter.operator] + ":" + filter.value;
                }
                if (isNaN(contentNum)) {
                    //return "S:" + contentType + ":" + arrOperator[filter.operator] + ":" + filter.value;
                    return "S:" + contentType + ":" + arrOperator[filter.operator] + ":" + Dashboard.CorrectAbilityProfileFilerValue(filter.value);
                }
                return "S:" + contentType + ":" + arrContent2[contentNum] + ":" + arrOperator[filter.operator] + ":" + filter.value;
            }
            function getLogicOperator(operator) {
                if (operator === "or") {
                    return "|OR|";
                }
                return "|AND|";
            }
            /*
                        function addDivider(parameters) {
                            if (parameters === "") {
                                parameters += "?";
                            } else {
                                parameters += "&";
                            }
                            return parameters;
                        }
            */
            //add custom filters (from cards or popup)
            if (typeof private.CurrentRosterFilter.filters !== "undefined") {
                if (private.CurrentRosterFilter.filters.length) {
                    if (typeof private.CurrentRosterFilter.logic !== "undefined") {
                        filterLogic0 = getLogicOperator(private.CurrentRosterFilter.logic);
                    } else {
                        filterLogic0 = getLogicOperator("and");
                    }
                    for (i = 0; i < private.CurrentRosterFilter.filters.length; i++) {
                        if (i === 0) {
                            //parameters = addDivider(parameters);
                            parameters += "filter=";
                        }
                        if (typeof private.CurrentRosterFilter.filters[i].field !== "undefined") {
                            filter = private.CurrentRosterFilter.filters[i];
                            if (filter.operator !== "isnotnull") {
                                parameters += filterLogic0 + parseFilterObj(filter);
                            }
                        } else {
                            if (typeof private.CurrentRosterFilter.filters[i].filters !== "undefined") {
                                if (typeof private.CurrentRosterFilter.filters[i].logic !== "undefined") {
                                    filterLogic1 = getLogicOperator(private.CurrentRosterFilter.filters[i].logic);
                                } else {
                                    filterLogic1 = filterLogic0;
                                }
                                for (j = 0; j < private.CurrentRosterFilter.filters[i].filters.length; j++) {
                                    if (typeof private.CurrentRosterFilter.filters[i].filters[j].field !== "undefined") {
                                        filter = private.CurrentRosterFilter.filters[i].filters[j];
                                        if (filter.operator !== "isnotnull") {
                                            if (j === 0) {
                                                parameters += filterLogic0 + parseFilterObj(filter);
                                            } else {
                                                parameters += filterLogic1 + parseFilterObj(filter);
                                            }
                                        }
                                    } else {
                                        if (typeof private.CurrentRosterFilter.filters[i].filters[j].filters !== "undefined") {
                                            if (typeof private.CurrentRosterFilter.filters[i].filters[j].logic !== "undefined") {
                                                filterLogic2 = getLogicOperator(private.CurrentRosterFilter.filters[i].filters[j].logic);
                                            } else {
                                                filterLogic2 = filterLogic1;
                                            }
                                            for (k = 0; k < private.CurrentRosterFilter.filters[i].filters[j].filters.length; k++) {
                                                if (typeof private.CurrentRosterFilter.filters[i].filters[j].filters[k].field !== "undefined") {
                                                    filter = private.CurrentRosterFilter.filters[i].filters[j].filters[k];
                                                    if (filter.operator !== "isnotnull") {
                                                        if (k === 0) {
                                                            parameters += filterLogic1 + parseFilterObj(filter);
                                                        } else {
                                                            parameters += filterLogic2 + parseFilterObj(filter);
                                                        }
                                                    }
                                                } else {
                                                    if (typeof private.CurrentRosterFilter.filters[i].filters[j].filters[k].filters !== "undefined") {
                                                        if (typeof private.CurrentRosterFilter.filters[i].filters[j].filters[k].logic !== "undefined") {
                                                            filterLogic3 = getLogicOperator(private.CurrentRosterFilter.filters[i].filters[j].filters[k].logic);
                                                        } else {
                                                            filterLogic3 = filterLogic2;
                                                        }
                                                        for (s = 0; s < private.CurrentRosterFilter.filters[i].filters[j].filters[k].filters.length; s++) {
                                                            if (typeof private.CurrentRosterFilter.filters[i].filters[j].filters[k].filters[s].field !== "undefined") {
                                                                filter = private.CurrentRosterFilter.filters[i].filters[j].filters[k].filters[s];
                                                                if (filter.operator !== "isnotnull") {
                                                                    if (s === 0) {
                                                                        parameters += filterLogic2 + parseFilterObj(filter);
                                                                    } else {
                                                                        parameters += filterLogic3 + parseFilterObj(filter);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    parameters = parameters.replace("filter=|OR|", "filter=");
                    parameters = parameters.replace("filter=|AND|", "filter=");
                    parameters = parameters.replace("filter=", "");
                    parameters = Dashboard.ReplaceAll(parameters, ":ability_profile:=:", ":PROF:CompVQN:=:");
                    parameters = Dashboard.CorrectBetweenLogic(parameters);
                    //console.log("parameters:" + parameters);
                }
            }
            return parameters;
        },

        GenerateRosterTable: function (element, data) {
            Dashboard.UpdateRosterSearchFilteredModel();

            var rosterColumns;
            var filterableType = false;

            filterableType = {
                mode: "row",
                operators: {
                    string: {
                        contains: "Contains",
                        startswith: "Starts with",
                        eq: "Is equal to",
                        neq: "Is not equal to"
                    },
                    number: {
                        eq: "is equal to",
                        //neq: "Not equal to",
                        gt: "is greater than",
                        lt: "is less than",
                        gte: "is greater than or equal to",
                        lte: "is less than or equal to"
                    }
                }
            }

            if ($("#roster-switcher-2:checked").val() === "Number") {
                rosterColumns = Dashboard.GenerateKendoRosterColumnStructure(data.columns, "Number");
                $("#roster-table-wrapper").addClass("font-resizer");
            } else {
                $("#roster-table-wrapper").removeClass("font-resizer");
                rosterColumns = Dashboard.GenerateKendoRosterColumnStructure(data.columns, "Percent");
            }

            try {
                var objData = "";
                var objTotal = "";
                var objTransport = "";
                if (private.UseServerPaging) {
                    //pageSize = 2;
                    objData = function (response) {
                        return response.values;
                    }
                    //objTotal = "total";
                    objTotal = function () {
                        //return 999;
                        if ($("#stanine-table-selected-label").text().trim() !== "") {
                            return $("#stanine-table-wrapper .selected-content-stanine .stanine-table-link").text();
                        } else if ($("#right-card-selected-label").text().trim() !== "") {
                            return $("#right-card-table-wrapper .selected-content-stanine .stanine-table-link").text();
                        } else if ($("#right-card2-selected-label").text().trim() !== "") {
                            return $("#right-card-table2-wrapper .selected-content-stanine .stanine-table-link").text();
                        } else {
                            return private.CurrentRosterTotalCountNum;
                        }
                    }
                    objTransport = {
                        /*
                        read: {
                            //type: "POST",
                            //url: "http://localhost:50000/Reskin/Scripts/JsonRosterTest.json",
                            url: siteRoot + "/DashboardCogat/GetStudentRoster",
                            dataType: "json" // "jsonp" is required for cross-domain requests; use "json" for same-domain requests
                            //data: { q: "html5" } // search for tweets that contain "html5"
                        }
                        */
                        read: function (operation) {
                            //console.log(operation);
                            //console.log("-------------");
                            var parameters = "";
                            //console.log(operation.data);
                            //console.log(private.CurrentRosterFilter);
                            //console.log(private.arrSelectedTopFilterScores);
                            if (typeof operation.data.take !== "undefined") {
                                parameters += "?take=" + operation.data.take;
                            }

                            function addDivider(parameters) {
                                if (parameters === "") {
                                    parameters += "?";
                                } else {
                                    parameters += "&";
                                }
                                return parameters;
                            }

                            var arrContent2 = ["Verbal", "Quant", "NonVerb", "CompVQ", "CompVN", "CompQN", "CompVQN"];
                            var arrContent = ["VERBAL", "QUANT", "NONVERB", "COMPVQ", "COMPVN", "COMPQN", "COMPVQN"];

                            var i, j, k, s, arrTmp = [], arrTmp2 = [];
                            for (i = 0; i < private.arrSelectedTopFilterScores.length; i++) {
                                arrTmp.push(arrContent[private.arrSelectedTopFilterScores[i]]);
                                arrTmp2.push(arrContent2[private.arrSelectedTopFilterScores[i]]);
                            }
                            arrContent = arrTmp;
                            arrContent2 = arrTmp2;

                            if (typeof operation.data.skip !== "undefined") {
                                parameters = addDivider(parameters);
                                parameters += "skip=" + operation.data.skip;
                            }

                            if (typeof operation.data.take !== "undefined" && typeof operation.data.skip !== "undefined") {
                                private.RosterCurrentPageNumber = (operation.data.take + operation.data.skip) / operation.data.take;
                                private.RosterPageSize = operation.data.take;
                            }

                            if (typeof operation.data.sort !== "undefined") {
                                if (operation.data.sort.length) {
                                    parameters = addDivider(parameters);
                                    var contentNum = parseInt(operation.data.sort[0].field.match(/\d+/));
                                    //parameters += "sort_field=" + operation.data.sort[0].field;
                                    parameters += "score=" + arrContent2[contentNum];
                                    parameters += "&orderType=" + operation.data.sort[0].dir.toUpperCase();
                                    private.LastFocusedElement = $("#roster-table-wrapper table th.k-state-focused");
                                }
                            }

                            parameters += "&filter=" + Dashboard.ConvertAppliedFilterObjectToString();

                            //add search by student name filter
                            if (typeof operation.data.filter !== "undefined") {
                                console.log("TODO: Search by Student Name");
                                parameters = addDivider(parameters);
                                //parameters += parseFilterObj(operation.data.filter.filters[0]);
                            } else {
                                //parameters += "&filter=";
                            }

                            //var url = "http://localhost:50000/Reskin/Scripts/JsonRosterTest2.json";
                            //var url = "http://localhost:50000/Reskin/Scripts/JsonRosterTest.json";
                            var url = siteRoot + "/DashboardCogat/GetStudentRoster" + parameters;

                            if (!private.isProdEnvironment) {
                                $("#debug-api-roster textarea").text("");
                                $("#debug-api-roster span").text("");
                            }

                            if ($(".roster-table-card").hasClass("empty-json-overlay")) {
                                $("#roster-table-wrapper .k-loading-mask").hide();
                            } else {
                                $.ajax({
                                    async: true,
                                    dataType: "json",
                                    url: url,
                                    //data: {},
                                    success: function (data) {
                                        if (!private.isProdEnvironment) {
                                            $("#debug-api-roster textarea").text(JSON.stringify(data.api_params));
                                            $("#debug-api-roster span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                                        }

                                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                                            if (!private.HybridGetAllRosterFlag && !private.isHybridAllDataReceived) {
                                                Dashboard.isAll3AjaxRequestFinished();
                                            }
                                            if (!private.isHybridAllDataReceived) {
                                                private.modelRoster = data;
                                                private.modelRosterAllColumns = data.columns;
                                                operation.success(data);
                                                Dashboard.UpdateCogatFormNormYear();
                                                Dashboard.FocusLastFocusedElement();
                                            }
                                        } else {
                                            Dashboard.ShowRosterError();
                                            //Dashboard.redrawTableRoster("error");
                                            //Dashboard.addViewElements("#roster-table-wrapper", "error");
                                        }
                                    },
                                    error: function () {
                                        //DmUiLibrary.HideOverlaySpinner();
                                    }
                                });
                            }

                        }
                    };
                }

                var self = this;
                $(element).kendoGrid({
                    dataSource: {
                        data: data.values,
                        //data: function (d) {
                        //return data.values;
                        //},

                        schema: {
                            model: {
                                id: "node_id",
                                fields: { node_name: { type: "string" }, link: { type: "string" } }
                            },
                            //data: "values",
                            data: objData,
                            total: objTotal
                        },
                        /*
                                                aggregate: [
                                                    { field: "node_name", aggregate: "count" },
                                                    { field: "AS0", aggregate: "average" }, { field: "AS1", aggregate: "average" }, { field: "AS2", aggregate: "average" }, { field: "AS3", aggregate: "average" }, { field: "AS4", aggregate: "average" }, { field: "AS5", aggregate: "average" }, { field: "AS6", aggregate: "average" },
                                                    { field: "APR0", aggregate: "average" }, { field: "APR1", aggregate: "average" }, { field: "APR2", aggregate: "average" }, { field: "APR3", aggregate: "average" }, { field: "APR4", aggregate: "average" }, { field: "APR5", aggregate: "average" }, { field: "APR6", aggregate: "average" },
                                                    { field: "GPR0", aggregate: "average" }, { field: "GPR1", aggregate: "average" }, { field: "GPR2", aggregate: "average" }, { field: "GPR3", aggregate: "average" }, { field: "GPR4", aggregate: "average" }, { field: "GPR5", aggregate: "average" }, { field: "GPR6", aggregate: "average" },
                                                    { field: "GS0", aggregate: "average" }, { field: "GS1", aggregate: "average" }, { field: "GS2", aggregate: "average" }, { field: "GS3", aggregate: "average" }, { field: "GS4", aggregate: "average" }, { field: "GS5", aggregate: "average" }, { field: "GS6", aggregate: "average" },
                                                    { field: "USS0", aggregate: "average" }, { field: "USS1", aggregate: "average" }, { field: "USS2", aggregate: "average" }, { field: "USS3", aggregate: "average" }, { field: "USS4", aggregate: "average" }, { field: "USS5", aggregate: "average" }, { field: "USS6", aggregate: "average" },
                                                    { field: "SAS0", aggregate: "average" }, { field: "SAS1", aggregate: "average" }, { field: "SAS2", aggregate: "average" }, { field: "SAS3", aggregate: "average" }, { field: "SAS4", aggregate: "average" }, { field: "SAS5", aggregate: "average" }, { field: "SAS6", aggregate: "average" },
                                                    { field: "RS0", aggregate: "average" }, { field: "RS1", aggregate: "average" }, { field: "RS2", aggregate: "average" }, { field: "RS3", aggregate: "average" }, { field: "RS4", aggregate: "average" }, { field: "RS5", aggregate: "average" }, { field: "RS6", aggregate: "average" },
                                                    { field: "NANI0", aggregate: "average" }, { field: "NANI1", aggregate: "average" }, { field: "NANI2", aggregate: "average" }, { field: "NANI3", aggregate: "average" }, { field: "NANI4", aggregate: "average" }, { field: "NANI5", aggregate: "average" }, { field: "NANI6", aggregate: "average" }
                                                ],
                        */
                        //pageSize: 25,
                        pageSize: private.RosterPageSize,

                        //type: "json",
                        transport: objTransport,
                        //serverAggregates: private.UseServerPaging,
                        serverPaging: private.UseServerPaging,
                        serverFiltering: private.UseServerPaging,
                        serverSorting: private.UseServerPaging
                    },
                    dataBound: function () {
                        //if (private.UseServerPaging) {
                            Dashboard.UpdateRosterGroupTotal();
                        //}
                        Dashboard.IsNeedToHideRosterGroupTotal();
                        if (private.isTabNavigationOn) {
                            if ($(".roster-table-card.root-tab-element").attr("tabindex") === "-1") {
                                $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                            } else {
                                $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable();
                            }
                        }
                        $("#roster-table-wrapper > table tbody tr td:first-child").attr("scope", "row");
                        $("#roster-table-wrapper > table tbody tr td:first-child").each(function () {
                            $(this).replaceWith($(this)[0].outerHTML.replace("<td ", "<th ").replace("</td>", "</th>"));
                        });
                        setTimeout(function () {
                            Dashboard.WcagPagination("#roster-table-wrapper");
                        }, 100);
                        Dashboard.RosterLegendVisibleValuesIOnly();
                    },
                    dataBinding: function (e) {
                        // call e.preventDefault() if you want to cancel binding.
                        private.RosterPageSize = e.sender.dataSource.pageSize();
                    },
                    //resizable: true,
                    //reorderable: true,
                    //groupable: true,
                    //columnMenu: true,
                    scrollable: false,
                    /*
                    scrollable: {
                        endless: true
                    },
                    height: 200,
                    */
                    /*
                    serverPaging: true,
                    serverSorting: true,                
                    */
                    persistSelection: true, //see parameter in schema->model-> id: "node_id",
                    //change: onChange,
                    //dataBound: onDataBound(),
                    sortable: {
                        mode: "single",
                        //mode: "multiple",
                        showIndexes: true,
                        allowUnsort: true
                    },
                    //filterable: false,
                    filterable: filterableType,
                    //pageable: true,
                    pageable: {
                        input: true,
                        numeric: false,
                        alwaysVisible: true,
                        pageSizes: [5, 10, 25, 50],
                        //pageSizes: [5, 10, 25, 50, 100, 1000],
                        change: function () {
                            //console.log("pager change event");
                            Dashboard.WcagPagination("#roster-table-wrapper");
                            Dashboard.RosterLegendVisibleValuesIOnly();
                            if ($("body").hasClass("wcag_focuses_on") && $(".roster-table-card.root-tab-element").attr("tabindex") === "-1" && private.isTabNavigationOn) {
                                $("#roster-table-wrapper input.k-textbox, #roster-table-wrapper .k-widget.k-dropdown, #roster-table-wrapper a.k-pager-nav").removeClass("tab first-tab-element last-tab-element");
                                setTimeout(function () {
                                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                                    var focusedElement = $(":focus");
                                    if (focusedElement.hasClass("k-state-disabled")) {
                                        $(".roster-table-card .last-tab-element").focus();
                                    }
                                }, 100);
                            }
                            if (private.isTabNavigationOn) {
                                $("#roster-table-wrapper .k-pager-wrap input.k-textbox").attr("tabindex", "-1");
                            }
                        }
                    },
                    sort: function (e) {
                        //$('.kendo_sorted_column').addClass("grid-sort-filter-icon k-icon");
                        if (e.sort.dir === "asc" || e.sort.dir === "desc") {
                            private.objLastSort = {
                                "field": e.sort.field,
                                "dir": e.sort.dir
                            };
                        } else {
                            private.objLastSort = {};
                        }
                    },
                    //editable: true,
                    navigatable: true,
                    navigate: function (e) {
                        Dashboard.setGridCellCurrentState(e.element);
                        $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                        e.element.focus();
                        private.RosterGridNavigationEvent = true;
                    },
                    //                noRecords: {
                    //                    template: self.noRecordMsg(),
                    //                },
                    columns: rosterColumns
                });

                Dashboard.moveOutRosterFilters();
                //Dashboard.ChangeWarningsDropdownOptions(data);
                Dashboard.insertTooltipsToHeaderOfRosterTable(data.columns);
                Dashboard.AddWarningsLegend(data);
                $("#roster-table-wrapper table").append($("#roster-table-wrapper tfoot")); //move Group Total' footer to the bottom of table

                if (data.values.length <= 0 && !private.UseServerPaging) {
                    $("#roster-top-buttons-wrapper").hide();
                } else {
                    $("#roster-top-buttons-wrapper").show();
                }
            } catch (e) {
                $("body").append('<div class="kendo-errors">Error Roster=' + e.name + ": " + e.message + " | " + e.stack + "</div>");

                if (e.name === "ReferenceError" && e.message.indexOf("is not defined") !== -1) {
                    var field = e.message.substring(0, e.message.indexOf("is not defined")).trim();
                    var wrongDataList = [];
                    console.log("field=" + field);
                    wrongDataList = data.values.filter((item) => !item.hasOwnProperty(field));
                    console.log(wrongDataList);
                    var wrongDataFormattedList = "";
                    for (var i = 0; i < wrongDataList.length; i++) {
                        wrongDataFormattedList += (i + 1) + ")";
                        wrongDataFormattedList += " node_id=" + wrongDataList[i].node_id;
                        wrongDataFormattedList += ", node_name=" + wrongDataList[i].node_name;
                        wrongDataFormattedList += ", district=" + wrongDataList[i].district;
                        wrongDataFormattedList += ", district_id=" + wrongDataList[i].district_id;
                        wrongDataFormattedList += ", building=" + wrongDataList[i].building;
                        wrongDataFormattedList += ", building_id=" + wrongDataList[i].building_id;
                        wrongDataFormattedList += ", class_name=" + wrongDataList[i].class_name;
                        wrongDataFormattedList += ", class_id=" + wrongDataList[i].class_id;
                        wrongDataFormattedList += "\n\n";
                    }
                    $("#debug-api-roster-data-issues textarea").text(wrongDataFormattedList);
                }
            }
        },

        ChangeWarningsDropdownOptions: function (data) {
            private.WarningsDropdownOptions = "";
            if (typeof data !== "undefined") {
                if (typeof data.extra_params !== "undefined") {
                    if (data.extra_params.warnings_list.length) {

                        if (data.extra_params.warnings_list.indexOf("el") !== -1) {
                            private.WarningsDropdownOptions += '<option value="el">~ Estimated level</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("lucg") !== -1) {
                            private.WarningsDropdownOptions += '<option value="lucg">§ Level unusual for coded grade</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("aucl") !== -1) {
                            private.WarningsDropdownOptions += '<option value="aucl">ã Age unusual for coded level</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("aoor") !== -1) {
                            private.WarningsDropdownOptions += '<option value="aoor">« Age is out-of-range</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("scni") !== -1) {
                            private.WarningsDropdownOptions += '<option value="scni">¥ Sentence Completion is not included in the Verbal Battery score</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("vats") !== -1) {
                            private.WarningsDropdownOptions += '<option value="vats">¥ Verbal Analogies is not included in the Total Score</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("ts") !== -1) {
                            private.WarningsDropdownOptions += '<option value="ts">• Targeted score</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("irp") !== -1) {
                            private.WarningsDropdownOptions += '<option value="irp">‡ Inconsistent response pattern</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("mio") !== -1) {
                            private.WarningsDropdownOptions += '<option value="mio">^ Many items omitted</option>';
                        }
                        if (data.extra_params.warnings_list.indexOf("tfia") !== -1) {
                            private.WarningsDropdownOptions += '<option value="tfia"># Too few items attempted</option>';
                        }
                    }
                }
                var obj, index;
                var isNullExist = false;
                for (var key in data.values) {
                    if (data.values.hasOwnProperty(key)) {
                        if ($.isNumeric(key)) {
                            obj = data.values[key];
                            if (!isNullExist) {
                                for (var i = 0; i < private.arrColumnsSelectedInRoster.length; ++i) {
                                    for (var j = 0; j < private.arrSelectedTopFilterScores.length; ++j) {
                                        if (private.modelRoster.columns[j + 1].title === "V" || private.modelRoster.columns[j + 1].title === "Q" || private.modelRoster.columns[j + 1].title === "N" || private.modelRoster.columns[j + 1].title === "Total Score") {
                                            //if (obj["ts"] === 0 && obj["irp"] === 0 && obj["mio"] === 0 && obj["tfia"] === 0 && obj["scni"] === 0 && obj["vats"] === 0) {
                                            if (obj["irp" + j] === 0 && obj["mio" + j] === 0 && obj["tfia" + j] === 0 && obj["scni"] === 0 && obj["vats"] === 0) {
                                            //if (!obj["irp" + j] && !obj["mio" + j] && obj["tfia" + j] === 0 && obj["scni"] === 0 && obj["vats"] === 0) {
                                                index = private.arrColumnsSelectedInRoster[i] + j;
                                                if (obj[index] === null && obj["RS" + j] === null) {
                                                    isNullExist = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (isNullExist) {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (isNullExist) private.WarningsDropdownOptions += '<option value="test_not_taken">* Test not taken</option>';
            }
            $("#dropdown-warning-score-1, #dropdown-warning-score-2, #dropdown-warning-score-3").empty();
            $("#dropdown-warning-score-1, #dropdown-warning-score-2, #dropdown-warning-score-3").append(private.WarningsDropdownOptions);
            $("#dropdown-warning-score-1 option:first-child, #dropdown-warning-score-2 option:first-child, #dropdown-warning-score-3 option:first-child").prop("selected", true);
            $("#dropdown-warning-score-1, #dropdown-warning-score-2, #dropdown-warning-score-3").trigger("DmUi:updated");

            if (private.WarningsDropdownOptions === "") {
                Dashboard.DisableRosterScoreWarningFiltersPopupButton();
            } else {
                Dashboard.EnableRosterScoreWarningFiltersPopupButton();
            }
        },

        AddWarningsLegend: function (data) {
            $("#roster-legend-wrapper dt.warning, #roster-legend-wrapper dd.warning").remove();
            if (typeof data !== "undefined") {
                if (typeof data.extra_params !== "undefined") {
                    if (data.extra_params.warnings_list.length) {
                        if (data.extra_params.warnings_list.indexOf("el") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">~:</dt><dd class="warning"> Estimated level</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("lucg") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">§:</dt><dd class="warning"> Level unusual for coded grade</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("aucl") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">ã:</dt><dd class="warning"> Age unusual for coded level</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("aoor") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">«:</dt><dd class="warning"> Age is out-of-range</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("scni") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">¥:</dt><dd class="warning"> Sentence Completion is not included in the Verbal Battery score</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("vats") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">¥:</dt><dd class="warning"> Verbal Analogies is not included in the Total Score</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("ts") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">•:</dt><dd class="warning"> Targeted score</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("irp") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">‡:</dt><dd class="warning"> Inconsistent response pattern</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("mio") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">^:</dt><dd class="warning"> Many items omitted</dd>');
                        }
                        if (data.extra_params.warnings_list.indexOf("tfia") !== -1) {
                            $("#roster-legend-wrapper dl").append('<dt class="warning">#:</dt><dd class="warning"> Too few items attempted</dd>');
                        }
                    }
                } else {
                    var obj, index;
                    var isEl = false;
                    var isLucg = false;
                    var isAucl = false;
                    var isAoor = false;
                    var isScni = false;
                    var isVats = false;
                    var isTs = false;
                    var isIrp = false;
                    var isMio = false;
                    var isTfia = false;
                    var isNullExist = false;

                    for (var key in data) {
                        if (data.hasOwnProperty(key)) {
                            if ($.isNumeric(key)) {
                                obj = data[key];
                                if (!isEl && obj["el"] === 1) isEl = true;
                                if (!isLucg && obj["lucg"] === 1) isLucg = true;
                                if (!isAucl && obj["aucl"] === 1) isAucl = true;
                                if (!isAoor && obj["aoor"] === 1) isAoor = true;
                                if (!isScni && obj["scni"] === 1) isScni = true;
                                if (!isVats && obj["vats"] === 1) isVats = true;
                                if (!isTs && obj["ts"] === 1 && private.arrColumnsSelectedInRoster.indexOf("RS") !== -1) isTs = true;
                                if (!isIrp && obj["irp"] === 1) isIrp = true;
                                if (!isMio && obj["mio"] === 1) isMio = true;
                                if (!isTfia && obj["tfia"] === 1) isTfia = true;
                                if (!isNullExist) {
                                    for (var i = 0; i < private.arrColumnsSelectedInRoster.length; ++i) {
                                        for (var j = 0; j < private.arrSelectedTopFilterScores.length; ++j) {
                                            if (private.modelRoster.columns[j + 1].title === "V" || private.modelRoster.columns[j + 1].title === "Q" || private.modelRoster.columns[j + 1].title === "N" || private.modelRoster.columns[j + 1].title === "Total Score") {
                                                //if (obj["ts"] === 0 && obj["irp"] === 0 && obj["mio"] === 0 && obj["tfia"] === 0 && obj["scni"] === 0 && obj["vats"] === 0) {
                                                if (obj["irp" + j] === 0 && obj["mio" + j] === 0 && obj["tfia" + j] === 0 && obj["scni"] === 0 && obj["vats"] === 0) {
                                                //if (!obj["irp" + j] && !obj["mio" + j] && !obj["tfia" + j] && !obj["scni"] && !obj["vats"]) {
                                                    index = private.arrColumnsSelectedInRoster[i] + j;
                                                    if (obj[index] === null && obj["RS" + j] === null) {
                                                        isNullExist = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (isNullExist) {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (isEl) $("#roster-legend-wrapper dl").append('<dt class="warning">~:</dt><dd class="warning"> Estimated level</dd>');
                    if (isLucg) $("#roster-legend-wrapper dl").append('<dt class="warning">§:</dt><dd class="warning"> Level unusual for coded grade</dd>');
                    if (isAucl) $("#roster-legend-wrapper dl").append('<dt class="warning">ã:</dt><dd class="warning"> Age unusual for coded level</dd>');
                    if (isAoor) $("#roster-legend-wrapper dl").append('<dt class="warning">«:</dt><dd class="warning"> Age is out-of-range</dd>');
                    if (isScni) $("#roster-legend-wrapper dl").append('<dt class="warning">¥:</dt><dd class="warning"> Sentence Completion is not included in the Verbal Battery score</dd>');
                    if (isVats) $("#roster-legend-wrapper dl").append('<dt class="warning">¥:</dt><dd class="warning"> Verbal Analogies is not included in the Total Score</dd>');
                    if (isTs) $("#roster-legend-wrapper dl").append('<dt class="warning">•:</dt><dd class="warning"> Targeted score</dd>');
                    if (isIrp) $("#roster-legend-wrapper dl").append('<dt class="warning">‡:</dt><dd class="warning"> Inconsistent response pattern</dd>');
                    if (isMio) $("#roster-legend-wrapper dl").append('<dt class="warning">^:</dt><dd class="warning"> Many items omitted</dd>');
                    if (isTfia) $("#roster-legend-wrapper dl").append('<dt class="warning">#:</dt><dd class="warning"> Too few items attempted</dd>');
                    if (isNullExist) $("#roster-legend-wrapper dl").append('<dt class="warning">*:</dt><dd class="warning"> Test not taken</dd>');
                }
            }
        },

        IsNeedToHideRosterGroupTotal: function () {
            var numStudentsUnchecked = $("#Student_Control option:not(:selected) ").length;
            if (numStudentsUnchecked) {
                $("#roster-table-wrapper .k-footer-template").hide();
            } else {
                if (typeof private.CurrentRosterFilter.filters !== "undefined") {
                    if (private.CurrentRosterFilter.filters.length) {
                        $("#roster-table-wrapper .k-footer-template").hide();
                    }
                }
            }
        },

        UpdateRosterGroupTotal: function () {
            if (typeof private.modelRosterGroupTotals.group_total !== "undefined") {
                var columnNum;
                for (var i = 0; i < private.arrSelectedTopFilterScores.length; ++i) {
                    for (var j = 0; j < private.arrColumnsSelectedInRoster.length; ++j) {
                        columnNum = 1 + i * private.arrColumnsSelectedInRoster.length + j;
                        $("#roster-table-wrapper .k-footer-template td:eq(" + columnNum + ")").text(private.modelRosterGroupTotals.group_total[private.arrColumnsSelectedInRoster[j] + i]);
                    }
                }
            }
        },

        clearQuantileSelection: function () {
            $(".quantile-band").removeClass("grayscale");
            $("#barChart4 .rect-label").removeClass("grayscale");
            $("#roster-table-wrapper").removeClass("color-event-range-1 color-event-range-2 color-event-range-3 color-event-range-4");
            this.clearDomainSelection();
        },

        clearDomainSelection: function () {
            $("#dashboard-right-column .dm-ui-card").removeClass("grayscale");
            $("#dashboard-right-column .dm-ui-card .domain-band").removeClass("grayscale");
            $("#dashboard-right-column .rect-label").removeClass("grayscale");
            $("#roster-table-wrapper").removeClass("color-domain-num-1 color-domain-num-2 color-domain-num-3 color-domain-num-4 color-domain-num-5 color-domain-num-6 color-domain-num-7");
            $("#roster-table-wrapper").removeClass("color-domain-range-1 color-domain-range-2 color-domain-range-3");
            $("#roster-selected-domain-label").empty();
        },


        CorrectBetweenLogic: function (str) {
            var begin, end, pos, param, str2 = "", val1, val2;
            while (str.indexOf(":>=:") !== -1) {
                pos = str.indexOf(":>=:");
                begin = str.substr(0, pos);
                end = str.substr(pos + 4);
                if (begin.lastIndexOf("|") > begin.lastIndexOf("=")) {
                    param = begin.substr(begin.lastIndexOf("|") + 1);
                } else {
                    param = begin.substr(begin.lastIndexOf("=") + 1);
                }
                if (str.indexOf("|AND|" + param + ":<=:") !== -1) {
                    val1 = end.substr(0, end.indexOf("|"));
                    end = end.substr(end.indexOf("|AND|" + param + ":<=:"));
                    end = end.substr(end.indexOf(":<=:") + 4);
                    if (end.indexOf("|") >= 0) {
                        val2 = end.substr(0, end.indexOf("|"));
                        end = end.substr(end.indexOf("|"));
                    } else {
                        val2 = end;
                        end = "";
                    }
                    str2 += begin + ":BETWEEN:" + val1 + "," + val2;
                } else {
                    str2 += begin + ":>=:";
                }
                str = end;
            }
            str2 += str;
            return str2;
        },

        ReplaceAll: function (str, find, replace) {
            if (str === null) str = "";
            if ($.isNumeric(str)) {
                str = "" + str;
            }

            function escapeRegExp(str) {
                return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
            }

            return str.replace(new RegExp(escapeRegExp(find), "g"), replace);
        },

        FirstLocationOnly: function (str) {
            var arrAllNames = str.split("|");
            return arrAllNames.length ? arrAllNames[0] : "";
        },

        LocationsWithoutDuplicates: function (str) {
            var arrUniqueNames = [];
            var arrAllNames = [];
            var result = "";
            arrUniqueNames = [];
            arrAllNames = str.split("|");
            for (var j = 0; j < arrAllNames.length; j++) {
                if (arrUniqueNames.indexOf(arrAllNames[j]) == -1) {
                    arrUniqueNames.push(arrAllNames[j]);
                }
            }
            result = arrUniqueNames.length ? arrUniqueNames.join(", ") : "";
            return result;
        },

        CorrectAbilityProfileFilerValue: function (text) {
            //text = text.replace("(V+ Q-)", "(V+Q-)");
            text = Dashboard.ReplaceAll(text, "(V+ Q-)", "(V+Q-)");

            //text = text.replace("(Q+ V-)", "(Q+V-)");
            //text = Dashboard.ReplaceAll(text, "(Q+ V-)", "(Q+V-)");
            text = Dashboard.ReplaceAll(text, "(Q+ V-)", "(V-Q+)");

            //text = text.replace("(N+ V-)", "(V-N+)");
            text = Dashboard.ReplaceAll(text, "(N+ V-)", "(V-N+)");

            //text = text.replace("(N+ Q-)", "(Q-N+)");
            text = Dashboard.ReplaceAll(text, "(N+ Q-)", "(Q-N+)");

            //text = text.replace("(Q+ N-)", "(Q+N-)");
            text = Dashboard.ReplaceAll(text, "(Q+ N-)", "(Q+N-)");

            //text = text.replace("(V+ N-)", "(N-V+)");
            //text = Dashboard.ReplaceAll(text, "(V+ N-)", "(N-V+)");
            text = Dashboard.ReplaceAll(text, "(V+ N-)", "(V+N-)");

            return text;
        },

        CorrectReverseAbilityProfileFilerValue: function (text) {
            //text = text.replace("(Q-V+)", "(V+ Q-)");
            text = Dashboard.ReplaceAll(text, "(V+Q-)", "(V+ Q-)");

            //text = text.replace("(Q+V-)", "(V- Q+)");
            text = Dashboard.ReplaceAll(text, "(V-Q+)", "(Q+ V-)");

            //text = text.replace("(V-N+)", "(N+ V-)");
            text = Dashboard.ReplaceAll(text, "(V-N+)", "(N+ V-)");

            //text = text.replace("(N-V+)", "(V+ N-)");
            text = Dashboard.ReplaceAll(text, "(V+N-)", "(V+ N-)");

            //text = text.replace("(Q-N+)", "(N+ Q-)");
            text = Dashboard.ReplaceAll(text, "(Q-N+)", "(N+ Q-)");

            //text = text.replace("(Q+N-)", "(Q+ N-)");
            text = Dashboard.ReplaceAll(text, "(Q+N-)", "(Q+ N-)");

            return text;
        },

        UpdateCurrentRosterFiltersAdd: function (field, filterValue) {
            var obj = private.CurrentRosterFilter;
            if ($.isEmptyObject(obj)) {
                obj = {
                    "logic": "and",
                    "filters": [
                        {
                            "field": field,
                            "operator": "eq",
                            "value": filterValue
                        }
                    ]
                }
            } else {
                var isFieldAlreadyExist = false;
                //console.log(obj);
                for (var i = 0; i < obj.filters.length; ++i) {
                    if (obj.filters[i].field === field) {
                        obj.filters[i].value = filterValue;
                        isFieldAlreadyExist = true;
                        break;
                    }

                    if (obj.filters[i].filters !== undefined) {
                        if (obj.filters[i].filters[0].field === field) {
                            obj.filters[i].filters[0].value = filterValue;
                            isFieldAlreadyExist = true;
                            break;
                        }
                    }
                }

                if (!isFieldAlreadyExist) {
                    obj.filters.push({
                        //"logic": "and",
                        "filters": [
                            {
                                "field": field,
                                "operator": "eq",
                                "value": filterValue
                            }
                        ]
                    });
                }
            }
            //console.log(private.modelRoster);
            //console.log(obj);
            private.CurrentRosterFilter = obj;
        },


        UpdateCurrentRosterFiltersAddOrLogic: function (field, filterValue) {
            var obj = private.CurrentRosterFilter;
            var isFieldAlreadyExist = false;
            var i;

            if ($.isEmptyObject(obj)) {
                obj = {
                    "logic": "or",
                    "filters": [
                        {
                            "field": field,
                            "operator": "eq",
                            "value": filterValue
                        }
                    ]
                }
            } else {
                //adding to second level if exist
                for (i = 0; i < obj.filters.length; ++i) {
                    if (obj.filters[i].filters !== undefined) {
                        if (obj.filters[i].filters[0].field === field) {
                            obj.filters[i].filters.push({
                                "field": field,
                                "operator": "eq",
                                "value": filterValue
                            });
                            isFieldAlreadyExist = true;
                            break;
                        }
                    }
                }

                //adding to first level if exist
                if (!isFieldAlreadyExist) {
                    for (i = 0; i < obj.filters.length; ++i) {
                        if (obj.filters[i].field === field) {
                            obj.filters.push({
                                "field": field,
                                "operator": "eq",
                                "value": filterValue
                            });
                            isFieldAlreadyExist = true;
                            break;
                        }
                    }
                }

                if (!isFieldAlreadyExist) {
                    obj.filters.push({
                        "logic": "or",
                        "filters": [
                            {
                                "field": field,
                                "operator": "eq",
                                "value": filterValue
                            }
                        ]
                    });
                }
            }
            //console.log(obj);
            private.CurrentRosterFilter = obj;
        },

        UpdateCurrentRosterFiltersRemove: function (field) {
            var obj = private.CurrentRosterFilter;
            if (obj.filters !== undefined) {
                for (var i = 0; i < obj.filters.length; ++i) {
                    if (obj.filters[i].filters !== undefined) {
                        if (obj.filters[i].filters[0].field === field) {
                            obj.filters.splice(i, 1);
                            break;
                        }
                    }
                }
            }
            //console.log(obj);
            private.CurrentRosterFilter = obj;
        },

        UpdateCurrentRosterFiltersRemoveTotal: function (field, flagNotUpdate) {
            var obj = private.CurrentRosterFilter;
            var i, j;
            if (obj.filters !== undefined) {
                for (i = obj.filters.length - 1; i >= 0; --i) {
                    if (obj.filters[i].filters !== undefined) {
                        for (j = obj.filters[i].filters.length - 1; j >= 0; --j) {
                            if (obj.filters[i].filters[j].field === field) {
                                obj.filters[i].filters.splice(j, 1);
                                //break;
                            }
                        }
                    }
                    if (obj.filters[i].filters !== undefined) {
                        if (obj.filters[i].filters.length === 0) {
                            obj.filters.splice(i, 1);
                        }
                    }
                    if (obj.filters[i] !== undefined) {
                        if (obj.filters[i].field === field) {
                            obj.filters.splice(i, 1);
                            //break;
                        }
                    }
                }
            }
            if (obj.filters !== undefined) {
                if (obj.filters.length === 0) {
                    obj = {};
                }
            }
            //console.log(obj);
            if (typeof flagNotUpdate === "undefined") {
                //console.log(obj);
                private.CurrentRosterFilter = obj;
            } else {
                if (flagNotUpdate !== true) {
                    //console.log(obj);
                    private.CurrentRosterFilter = obj;
                } else {
                    return obj;
                }
            }
        },

        CreateRosterPopupFilters: function () {
            var objFilters = { "logic": "or", "filters": [] };
            var objAndFilters = { "logic": "and", "filters": [] };
            var objOrFilters = { "logic": "or", "filters": [] };
            var objInBetween;
            var field, operator, columnNum, value, value2, obj, obj2;
            var rowNumber;
            var booleanRowOperator;
            Dashboard.DebugRememberTime();
            var arrAppliedScores = [];
            var appliedFiltersNum = $(".table-report-filters tr:not(.filter-row-disabled) td.column-score select").length;
            var flagBooleanOrAnd = false;
            if (appliedFiltersNum === 3) {
                if ($("#dropdown-boolean-1").val() === "or" && $("#dropdown-boolean-2").val() === "and") {
                    flagBooleanOrAnd = true;
                }
            }

            $(".table-report-filters tr:not(.filter-row-disabled) td.column-score select").each(function (i) {
                rowNumber = $(this).attr("id");
                rowNumber = rowNumber.substr(rowNumber.length - 1).trim();

                operator = $("#dropdown-condition-" + rowNumber).val();
                columnNum = $("#dropdown-content-area-" + rowNumber + " option:selected").index();
                value = Number($("#input-score-" + rowNumber + "-val1").val());
                value2 = Number($("#input-score-" + rowNumber + "-val2").val());

                booleanRowOperator = "and";
                if (appliedFiltersNum === 2) {
                    booleanRowOperator = $("#dropdown-boolean-1").val();
                }
                if (appliedFiltersNum === 3) {
                    if (i === 0) {
                        booleanRowOperator = $("#dropdown-boolean-1").val();
                    }
                    if (i === 1) {
                        if (flagBooleanOrAnd) {
                            booleanRowOperator = $("#dropdown-boolean-1").val();
                        } else {
                            if ($("#dropdown-boolean-1").val() === "or" && $("#dropdown-boolean-2").val() === "or") {
                                booleanRowOperator = "or";
                            }
                        }
                    }
                    if (i === 2) {
                        if (flagBooleanOrAnd) {
                            booleanRowOperator = $("#dropdown-boolean-2").val();
                        } else {
                            if ($("#dropdown-boolean-2").val() === "or") {
                                booleanRowOperator = "or";
                            }
                        }
                    }
                }

                obj = new Object();

                if ($(this).prop("disabled", false)) {
                    field = $(this).val() + columnNum;
                    arrAppliedScores.push($(this).val());
                    if (operator === "is_in_between") {
                        objInBetween = { "logic": "and", "filters": [] };

                        obj["field"] = field;
                        obj["operator"] = "gte";
                        obj["value"] = value;
                        objInBetween.filters.push(obj);

                        obj = new Object();
                        obj["field"] = field;
                        obj["operator"] = "lte";
                        obj["value"] = value2;
                        objInBetween.filters.push(obj);

                        if (booleanRowOperator === "or") {
                            objOrFilters.filters.push(objInBetween);
                        } else {
                            objAndFilters.filters.push(objInBetween);
                        }
                    } else {
                        obj["field"] = field;
                        obj["operator"] = operator;
                        obj["value"] = value;
                        if (operator === "lt" || operator === "lte" || operator === "gt" || operator === "gte") {
                            obj2 = { "logic": "and", "filters": [] };
                            obj2.filters.push(obj);
                            obj2.filters.push({
                                "field": field,
                                "operator": "isnotnull"
                            });
                            obj = obj2;
                        }
                        if (booleanRowOperator === "or") {
                            objOrFilters.filters.push(obj);
                        } else {
                            objAndFilters.filters.push(obj);
                        }
                    }
                }
            });

            if (appliedFiltersNum === 3 && flagBooleanOrAnd) {
                if (objOrFilters.filters.length) {
                    objAndFilters.filters.push(objOrFilters);
                }
                objFilters.filters.push(objAndFilters);
            } else {
                if (objAndFilters.filters.length) {
                    objFilters.filters.push(objAndFilters);
                }
                if (objOrFilters.filters.length) {
                    objFilters.filters.push(objOrFilters);
                }
            }

            objFilters = { "logic": "and", "filters": [objFilters] };
            //console.log("CreatedPopupFilters::::::::::::::::");
            //console.log(objFilters);

            if (private.isPopupFiltersOfRightCard) {
                private.arrColumnsSelectedInRightCardPopupFilter = arrAppliedScores;
            } else {
                var isAllAppliedScoresAlreadyIncluded = true;
                for (var i = 0; i < arrAppliedScores.length; ++i) {
                    if (private.arrColumnsSelectedInRoster.indexOf(arrAppliedScores[i]) === -1) {
                        isAllAppliedScoresAlreadyIncluded = false;
                        break;
                    }
                }
                if (!isAllAppliedScoresAlreadyIncluded) {
                    private.arrColumnsSelectedInRoster = arrAppliedScores;
                }
            }
            return objFilters;
        },

        CreateRosterWarningsPopupFilters: function () {
            var objFilters = { "logic": "or", "filters": [] };
            var objAndFilters = { "logic": "and", "filters": [] };
            var objOrFilters = { "logic": "or", "filters": [] };
            var field, obj, obj2;
            var rowNumber;
            var booleanRowOperator;
            Dashboard.DebugRememberTime();
            var appliedFiltersNum = $(".table-warning-report-filters tr:not(.filter-row-disabled) td.column-warning-score select").length;
            var flagBooleanOrAnd = false;
            if (appliedFiltersNum === 3) {
                if ($("#dropdown-warning-boolean-1").val() === "or" && $("#dropdown-warning-boolean-2").val() === "and") {
                    flagBooleanOrAnd = true;
                }
            }

            $(".table-warning-report-filters tr:not(.filter-row-disabled) td.column-warning-score select").each(function (i) {
                rowNumber = $(this).attr("id");
                rowNumber = rowNumber.substr(rowNumber.length - 1).trim();

                booleanRowOperator = "and";
                if (appliedFiltersNum === 2) {
                    booleanRowOperator = $("#dropdown-warning-boolean-1").val();
                }
                if (appliedFiltersNum === 3) {
                    if (i === 0) {
                        booleanRowOperator = $("#dropdown-warning-boolean-1").val();
                    }
                    if (i === 1) {
                        if (flagBooleanOrAnd) {
                            booleanRowOperator = $("#dropdown-warning-boolean-1").val();
                        } else {
                            if ($("#dropdown-warning-boolean-1").val() === "or" && $("#dropdown-warning-boolean-2").val() === "or") {
                                booleanRowOperator = "or";
                            }
                        }
                    }
                    if (i === 2) {
                        if (flagBooleanOrAnd) {
                            booleanRowOperator = $("#dropdown-warning-boolean-2").val();
                        } else {
                            if ($("#dropdown-warning-boolean-2").val() === "or") {
                                booleanRowOperator = "or";
                            }
                        }
                    }
                }

                obj = new Object();

                if ($(this).prop("disabled", false)) {
                    field = $(this).val();
                    if (field !== "test_not_taken") {
                        obj["field"] = field;
                        obj["operator"] = "eq";
                        obj["value"] = 1;
                        if (booleanRowOperator === "or") {
                            objOrFilters.filters.push(obj);
                        } else {
                            objAndFilters.filters.push(obj);
                        }
                    } else {
                        //var objNullFilter = { "logic": "or", "filters": [] };
                        var objNullFilter = {
                            "logic": "and",
                            "filters": [
                                {
                                    "logic": "or",
                                    "filters": [
                                    ]
                                },
                                /*
                                                                {
                                                                    "field": "ts",
                                                                    "operator": "eq",
                                                                    "value": 0
                                                                },
                                */
                                /*
                                                                {
                                                                    "field": "irp",
                                                                    "operator": "eq",
                                                                    "value": 0
                                                                },
                                                                {
                                                                    "field": "mio",
                                                                    "operator": "eq",
                                                                    "value": 0
                                                                },
                                                                {
                                                                    "field": "tfia",
                                                                    "operator": "eq",
                                                                    "value": 0
                                                                },
                                */
                                {
                                    "field": "scni",
                                    "operator": "eq",
                                    "value": 0
                                },
                                {
                                    "field": "vats",
                                    "operator": "eq",
                                    "value": 0
                                }
                            ]
                        };
                        for (var k = 0; k < private.arrColumnsSelectedInRoster.length; ++k) {
                            for (var j = 0; j < private.arrSelectedTopFilterScores.length; ++j) {
                                if (private.modelRoster.columns[j + 1].title === "V" || private.modelRoster.columns[j + 1].title === "Q" || private.modelRoster.columns[j + 1].title === "N" || private.modelRoster.columns[j + 1].title === "Total Score") {
                                    /*
                                                                        obj = new Object();
                                                                        obj["field"] = private.arrColumnsSelectedInRoster[k] + j;
                                                                        obj["operator"] = "eq";
                                                                        obj["value"] = null;
                                                                        //objNullFilter.filters.push(obj);
                                                                        objNullFilter.filters[0].filters.push(obj);
                                    */
                                    obj2 = { "logic": "and", "filters": [] };

                                    obj = new Object();
                                    obj["field"] = private.arrColumnsSelectedInRoster[k] + j;
                                    obj["operator"] = "eq";
                                    obj["value"] = null;
                                    obj2.filters.push(obj);

                                    obj = new Object();
                                    obj["field"] = "irp" + j;
                                    obj["operator"] = "eq";
                                    obj["value"] = 0;
                                    obj2.filters.push(obj);

                                    obj = new Object();
                                    obj["field"] = "mio" + j;
                                    obj["operator"] = "eq";
                                    obj["value"] = 0;
                                    obj2.filters.push(obj);

                                    obj = new Object();
                                    obj["field"] = "tfia" + j;
                                    obj["operator"] = "eq";
                                    obj["value"] = 0;
                                    obj2.filters.push(obj);

                                    objNullFilter.filters[0].filters.push(obj2);
                                }
                            }
                        }
                        if (booleanRowOperator === "or") {
                            objOrFilters.filters.push(objNullFilter);
                        } else {
                            objAndFilters.filters.push(objNullFilter);
                        }
                    }
                }
            });

            if (appliedFiltersNum === 3 && flagBooleanOrAnd) {
                if (objOrFilters.filters.length) {
                    objAndFilters.filters.push(objOrFilters);
                }
                objFilters.filters.push(objAndFilters);
            } else {
                if (objAndFilters.filters.length) {
                    objFilters.filters.push(objAndFilters);
                }
                if (objOrFilters.filters.length) {
                    objFilters.filters.push(objOrFilters);
                }
            }

            objFilters = { "logic": "and", "filters": [objFilters] };
            //console.log("CreatedWarningsPopupFilters::::::::::::::::");
            //console.log(objFilters);

            return objFilters;
        },

        UpdateRosterSearchFilteredModel: function () {
            if (typeof $("#roster-table-wrapper").data("kendoGrid") !== "undefined") {
                var dataSource = $("#roster-table-wrapper").data("kendoGrid").dataSource;
                //var filters = dataSource.filter();
                var filters = private.CurrentRosterFilter;
                var allData = dataSource.data();
                if ($.isEmptyObject(filters)) {
                    private.modelForSearchFilteredRoster = {};
                } else {
                    var query = new kendo.data.Query(allData);
                    private.modelForSearchFilteredRoster = query.filter(filters).data;
                }
            }
        },

        CountRowsInFilteredData: function (filters) {
            Dashboard.DebugRememberTime();
            var dataSource = $("#roster-table-wrapper").data("kendoGrid").dataSource;
            var allData = dataSource.data();
            var query = new kendo.data.Query(allData);
            var filteredData = query.filter(filters).data;
            Dashboard.DebugShowTime("checking is filtered data exist");
            return filteredData.length;
        },

        ClearAllErrorsMessages: function () {
            //remove all errors messages and empty data overlays
            $(".empty-json-overlay").removeClass("empty-json-overlay");
            $(".json-error").removeClass("json-error");
            $(".dm-ui-alert.dm-ui-alert-info").remove();
            $(".dm-ui-alert.dm-ui-alert-error").remove();
            $(".undefined").remove();
            $(".debug-warning").empty();
            $(".kendo-errors").empty();
            $(".dm-ui-page-container").show();
        },

        addViewElements: function (selector, viewType) {
            var spinner = '<div class="spinner"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div></div>';
            var spinnerShifted = '<div class="spinner shifted"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div></div>';
            $(selector).empty();
            if (viewType === "init") {
                if (selector === "#barChart4") {
                    $("#total-students-num-quartiles").empty();
                    $(selector).html(spinnerShifted);
                } else {
                    $(selector).html(spinner);
                }
            }
            //if (viewType === "empty" && (selector !== "#average-standard-score" && selector !== "#pie-chart-score")) {
            if (viewType === "empty") {
                //$(selector).addClass("empty-json-overlay");
                if (selector === "#barChart4" || selector === "#roster-table-wrapper") {
                    $(selector).parents(".section-card").addClass("empty-json-overlay");
                }
            }
            if (viewType === "error") {
                //$(selector).addClass("empty-json-overlay");
                //$(selector).addClass("json-error");
                if (selector === "#barChart4" || selector === "#roster-table-wrapper") {
                    $(selector).parents(".section-card").addClass("empty-json-overlay");
                    $(selector).parents(".section-card").addClass("json-error");
                }
                if (selector === "#right-card-table-wrapper" || selector === "#right-card-table2-wrapper") {
                    $(selector).parents(".right-column-card").removeClass("overlayed");
                    $(selector).parents(".right-column-card").addClass("empty-json-overlay");
                    $(selector).parents(".right-column-card").addClass("json-error");
                }
            }
        },

        redrawBarChartStanine: function (data, viewType, stanineType) {
            this.addViewElements("#barChart4", viewType);
            $("#barChart4").BarChartStanine({
                'data': data,
                'stanineType': stanineType
            });
            Dashboard.WcagBarChartStanineTable(data, viewType, stanineType);
        },

        WcagBarChartStanineTable: function (data, viewType, stanineType) {
            if (viewType === "init") {
                $("#wcag-stanine-table-wrapper tbody td").text("0%");
            }
            if (typeof stanineType !== "undefined") {
                $("#wcag-stanine-table-wrapper tbody th").text(stanineType);
                for (var i = 0; i < data.values.length; i++) {
                    if (data.values[i].content_area === stanineType) {
                        for (var j = 1; j <= 9; j++) {
                            $("#wcag-stanine-table-wrapper tbody td:nth-of-type(" + j + ")").text(data.values[i]["stanine" + j + "_per"] + "%");
                        }
                    }
                }
            }
        },

        redrawTableStanine: function (data, viewType) {
            this.addViewElements("#stanine-table-wrapper", viewType);
            if (viewType !== "init") {
                $("#stanine-table-wrapper").GenerateStanineTable({
                    'data': data,
                    'class_of_table': "dm-ui-table"
                });
            }
        },

        redrawRightCardTable: function (viewType) {
            this.addViewElements("#right-card-table-wrapper", viewType);
            if (viewType !== "init") {
                $("#right-card-table-wrapper").GenerateRightCardTable({
                    //'data': private.RightCardTableInitialZeroData,
                    'class_of_table': "dm-ui-table"
                });
            } else {
                $("#right-card-table-wrapper").removeClass("k-grid k-widget k-display-block");
                $("#right-card-table-number").text("0");
            }
            $("#right-card1").rootMakeContentNotTabbable();
        },

        redrawRightCardTable2: function (viewType, data) {
            this.addViewElements("#right-card-table2-wrapper", viewType);
            if (viewType !== "init") {
                $("#right-card-table2-wrapper").GenerateRightCardTable2({
                    //'data': private.RightCardTable2InitialZeroData,
                    'data': data,
                    'class_of_table': "dm-ui-table"
                });
            } else {
                $("#right-card-table2-wrapper").removeClass("k-grid k-widget k-display-block");
                $("#right-card-table2-number").text("0");
            }
        },

        redrawTableRoster: function (viewType) {
            this.addViewElements("#roster-table-wrapper", viewType);
            if (viewType !== "init") {
                $("#roster-table-wrapper").GenerateRosterTable({
                    'data': private.RosterInitialZeroData,
                    'class_of_table': "dm-ui-table"
                });
            }
        },

        CalculateRosterFontResizingForNumberValues: function () {
            //count font size of table for switcher Number position
            var maxRow = 0, numRow, index, numOfExtraDigits;
            for (var i = 0; i < private.modelRoster.values.length; i++) {
                numRow = 0;
                index = 0;
                var values = private.modelRoster.values[i];
                for (var key in values) {
                    if (values.hasOwnProperty(key)) {
                        if (key.indexOf("_num_") > 0 && key.indexOf("_num_0") === -1) {
                            index++;
                            if (index > 6 * 3) {
                                break;
                            }
                            if (values[key] !== "" && values[key] != null) {
                                numOfExtraDigits = ("" + values[key]).length - 2;
                                if (numOfExtraDigits) {
                                    numRow += numOfExtraDigits;
                                }
                            }
                        }
                    }
                }
                if (numRow > maxRow) {
                    maxRow = numRow;
                }
            }

            var fontSwitcherNumberSizeClass = "";
            if (maxRow > 75) {
                fontSwitcherNumberSizeClass = "font-numbers-9";
            }
            else if (maxRow > 60) {
                fontSwitcherNumberSizeClass = "font-numbers-10";
            }
            else if (maxRow > 45) {
                fontSwitcherNumberSizeClass = "font-numbers-11";
            }
            else if (maxRow > 30) {
                fontSwitcherNumberSizeClass = "font-numbers-12";
            }
            else if (maxRow > 15) {
                fontSwitcherNumberSizeClass = "font-numbers-13";
            }
            $("#roster-table-wrapper").removeClass("font-numbers-9 font-numbers-10 font-numbers-11 font-numbers-12 font-numbers-13");
            $("#roster-table-wrapper").addClass(fontSwitcherNumberSizeClass);
        },

        GenerateKendoRosterColumnStructure: function (rosterStructure, valuesType) {
            if (typeof valuesType === "undefined") {
                valuesType = "Percent";
            }
            var kendoColumns = [];

            var kendoColumn, multiColumn, tmpColumn;
            var column;
            var arr;
            var index = 0;
            var key;
            var i;
            var arrLegendShort = [];
            var arrLegendFull = [];
            //var rosterTitle = "";
            var ariaRangeTitle;
            var tmpValue, tmpColumnNum;

            $("#print-student-profile").hide();
            //$("#print-dashboard").addClass("last-tab-element");
            //$("#roster-switcher-wrapper").removeClass("hidden");

            for (key in rosterStructure) {
                if (rosterStructure.hasOwnProperty(key)) {
                    index++;
                    if (index >= private.MaxRosterShownColumnsNum) {
                        break;
                    }
                    column = rosterStructure[key];
                    kendoColumn = {};

                    /*
                                        if (index === 1) {
                                            if (column["title"] === "System Name" || column["title"] === "System Comparison") {
                                                rosterTitle = "Score Comparison";
                                            }
                                            if (column["title"] === "District Name" || column["title"] === "District Comparison") {
                                                rosterTitle = "District Score Comparison";
                                            }
                                            if (column["title"] === "Building Name" || column["title"] === "Building Comparison") {
                                                rosterTitle = "Building Score Comparison";
                                            }
                                            if (column["title"] === "Class Name" || column["title"] === "Class Comparison") {
                                                rosterTitle = "Class Score Comparison";
                                            }
                                        }
                    */

                    if (column["multi"]) {
                        multiColumn = [];
                        arr = column["fields"];
                        /*
                                                if (valuesType === "Percent") {
                                                    arr = column["fields_per"];
                                                } else {
                                                    arr = column["fields_num"];
                                                }
                        
                        */
                        //arr = column["fields_num"];
                        for (i = 0; i < arr.length; i++) {
                            if (i === 3) {
                                /*
                                                            if (arr.length > 4) {
                                                                $("#roster-debug-warning").html("Roster multi columns structure is wrong! (<strong>" + arr.length + " columns</strong>)");
                                                            }
                                                            break;
                                */
                            }
                            tmpColumn = {};
                            tmpColumn["filterable"] = false;
                            tmpColumn["title"] = column.fields[i].title;
                            ariaRangeTitle = column.fields[i].title_full;

                            if (valuesType === "Percent") {
                                tmpValue = column.fields[i].field;
                                tmpColumnNum = tmpValue.substr(tmpValue.length - 1);
                                //if (column.fields[i].title === "AS") {
                                if (column.fields[i].title === "APR") {
                                    //tmpColumn["template"] = "<span class='roster-highlighted-cell-bg'></span><span class='roster-highlighted-cell-content'>#:" + column.fields[i].field + "#</span>";
                                    //tmpColumn["template"] = "<span class='roster-highlighted-cell-bg'></span><span class='roster-highlighted-cell-content'>#= (" + tmpValue + " == null) ? '*' : " + tmpValue + " #</span>";
                                    //tmpColumn["template"] = "<span class='roster-highlighted-cell-bg'></span><span class='roster-highlighted-cell-content'>#= (" + tmpValue + " == null) ? '' : " + tmpValue + " #</span>";
                                    //tmpColumn["template"] = "<span class='roster-highlighted-cell-bg'></span><span class='roster-highlighted-cell-content'><span class='warnings'>" +
                                    tmpColumn["template"] = "<span class='roster-highlighted-cell-bg'></span><span class='roster-highlighted-cell-content'>" +
                                        //"<span class='warnings'>" +
                                        "#=(" + tmpValue + " == null) ?  '<span class=\"warnings\">' : '<span class=\"warnings not-null\">'#" +
                                        //"#=(ts" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">•<span class=\"tooltiptext\" role=\"tooltip\">• Targeted score</span></span>' : ''#" +
                                        "#=(irp" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">‡<span class=\"tooltiptext\" role=\"tooltip\">‡ Inconsistent response pattern</span></span>' : ''#" +
                                        "#=(mio" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">^<span class=\"tooltiptext\" role=\"tooltip\">^ Many items omitted</span></span>' : ''#" +
                                        "#=(tfia" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">\\#<span class=\"tooltiptext\" role=\"tooltip\">\\# Too few items attempted</span></span>' : ''#" +
                                        "#=('" + column.title + "'== 'V' && scni" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">\\¥<span class=\"tooltiptext\" role=\"tooltip\">\\¥ Sentence Completion is not included in the Verbal Battery score</span></span>' : ''#" +
                                        "#=('" + column.title + "'== 'Total Score' && vats == 1) ? '<span class=\"tooltip\">\\¥<span class=\"tooltiptext\" role=\"tooltip\">\\¥ Verbal Analogies is not included in the Total Score</span></span>' : ''#" +
                                        //"</span>#=(" + tmpValue + " == null) ? '<span class=\"empty tooltip\">*<span class=\"tooltiptext\" role=\"tooltip\">\\* Test not taken</span></span>' : '<span class=\"value\">' + " + tmpValue + " + '</span>'#<span class='warnings invisible'>" +
                                        "</span>" +
                                        "#=(" + tmpValue + " != null) ?  '<span class=\"value\">' + " + tmpValue + " + '</span><span class=\"warnings invisible\">' : ''#" +
                                        "#=(" + tmpValue + " == null && (ts" + tmpColumnNum + " == 1 || irp" + tmpColumnNum + " == 1 || mio" + tmpColumnNum + " == 1 || tfia" + tmpColumnNum + " == 1)) ? '<span class=\"warnings invisible none\">' : ''#" +
                                        "#=(" + tmpValue + " == null && RS" + tmpColumnNum + " == null && ('" + column.title + "'== 'V' || '" + column.title + "'== 'Q' || '" + column.title + "'== 'N' || '" + column.title + "'== 'Total Score') && ts" + tmpColumnNum + " == 0 && irp" + tmpColumnNum + " == 0 && mio" + tmpColumnNum + " == 0 && tfia" + tmpColumnNum + " == 0) ? '<span class=\"empty tooltip\">*<span class=\"tooltiptext\" role=\"tooltip\">\\* Test not taken</span></span><span class=\"warnings invisible\">' : ''#" +
                                        "#=(" + tmpValue + " == null && ('" + column.title + "'== 'VQ' || '" + column.title + "'== 'VN' || '" + column.title + "'== 'QN' || '" + column.title + "'== 'VQN') && ts" + tmpColumnNum + " == 0 && irp" + tmpColumnNum + " == 0 && mio" + tmpColumnNum + " == 0 && tfia" + tmpColumnNum + " == 0) ? '<span class=\"warnings invisible\">' : ''#" +
                                        //"<span class='warnings invisible'>" +
                                        //"#=(ts" + tmpColumnNum + " == 1) ? '<span>•</span>' : ''#" +
                                        "#=(irp" + tmpColumnNum + " == 1) ? '<span>‡</span>' : ''#" +
                                        "#=(mio" + tmpColumnNum + " == 1) ? '<span>^</span>' : ''#" +
                                        "#=(tfia" + tmpColumnNum + " == 1) ? '<span>\\#</span>' : ''#" +
                                        "#=('" + column.title + "'== 'V' && scni" + tmpColumnNum + " == 1) ? '<span>\\¥</span>' : ''#" +
                                        "#=('" + column.title + "'== 'Total Score' && vats == 1) ? '<span>\\¥</span>' : ''#" +
                                        "</span></span>";
                                    //console.log(tmpColumn["template"]);
                                    //                              } else if (column.fields[i].title === "RS" || column.fields[i].title === "NA/NI") {
                                    //                                  tmpColumn["template"] = "#=(" + tmpValue + " == null) ? '<span class=\"empty tooltip\">*<span class=\"tooltiptext\" role=\"tooltip\">\\* Test not taken</span></span>' : " + tmpValue + " #";
                                } else {
                                    //tmpColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + " (Low): #:" + column.fields_per[0] + "#'>#:" + column.fields_per[0] + "#</span>";
                                    //tmpColumn["template"] = "#:" + column.fields_per[i] + "#";
                                    //tmpColumn["template"] = "#:" + column.fields[i].field + "#";
                                    //tmpColumn["template"] = "#= (" + tmpValue + " == null) ? '' : " + tmpValue + " #";
                                    //tmpColumn["template"] = "<span class='warnings'>" +
                                    tmpColumn["template"] = "#=(" + tmpValue + " == null) ?  '<span class=\"warnings\">' : '<span class=\"warnings not-null\">'#" +
                                        "#=(ts" + tmpColumnNum + " == 1 && '" + column.fields[i].title + "' == 'RS') ? '<span class=\"tooltip\">•<span class=\"tooltiptext\" role=\"tooltip\">• Targeted score</span></span>' : ''#" +
                                        "#=(irp" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">‡<span class=\"tooltiptext\" role=\"tooltip\">‡ Inconsistent response pattern</span></span>' : ''#" +
                                        "#=(mio" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">^<span class=\"tooltiptext\" role=\"tooltip\">^ Many items omitted</span></span>' : ''#" +
                                        "#=(tfia" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">\\#<span class=\"tooltiptext\" role=\"tooltip\">\\# Too few items attempted</span></span>' : ''#" +
                                        "#=('" + column.title + "'== 'V' && scni" + tmpColumnNum + " == 1) ? '<span class=\"tooltip\">\\¥<span class=\"tooltiptext\" role=\"tooltip\">\\¥ Sentence Completion is not included in the Verbal Battery score</span></span>' : ''#" +
                                        "#=('" + column.title + "'== 'Total Score' && vats == 1) ? '<span class=\"tooltip\">\\¥<span class=\"tooltiptext\" role=\"tooltip\">\\¥ Verbal Analogies is not included in the Total Score</span></span>' : ''#" +
                                        //"</span>#=(" + tmpValue + " == null) ? '<span class=\"empty tooltip\">*<span class=\"tooltiptext\" role=\"tooltip\">\\* Test not taken</span></span>' : '<span class=\"value\">' + " + tmpValue + " + '</span>'#<span class='warnings invisible'>" +
                                        "</span>" +
                                        "#=(" + tmpValue + " != null) ?  '<span class=\"value\">' + " + tmpValue + " + '</span><span class=\"warnings invisible\">' : ''#" +
                                        "#=(" + tmpValue + " == null && (ts" + tmpColumnNum + " == 1 || irp" + tmpColumnNum + " == 1 || mio" + tmpColumnNum + " == 1 || tfia" + tmpColumnNum + " == 1)) ? '<span class=\"warnings invisible none\">' : ''#" +
                                        "#=(" + tmpValue + " == null && RS" + tmpColumnNum + " == null && ('" + column.title + "'== 'V' || '" + column.title + "'== 'Q' || '" + column.title + "'== 'N' || '" + column.title + "'== 'Total Score') && ts" + tmpColumnNum + " == 0 && irp" + tmpColumnNum + " == 0 && mio" + tmpColumnNum + " == 0 && tfia" + tmpColumnNum + " == 0) ? '<span class=\"empty tooltip\">*<span class=\"tooltiptext\" role=\"tooltip\">\\* Test not taken</span></span><span class=\"warnings invisible\">' : ''#" +
                                        "#=(" + tmpValue + " == null && ('" + column.title + "'== 'VQ' || '" + column.title + "'== 'VN' || '" + column.title + "'== 'QN' || '" + column.title + "'== 'VQN') && ts" + tmpColumnNum + " == 0 && irp" + tmpColumnNum + " == 0 && mio" + tmpColumnNum + " == 0 && tfia" + tmpColumnNum + " == 0) ? '<span class=\"warnings invisible\">' : ''#" +
                                        //"<span class='warnings invisible'>" +
                                        "#=(ts" + tmpColumnNum + " == 1 && '" + column.fields[i].title + "' == 'RS') ? '<span>•</span>' : ''#" +
                                        "#=(irp" + tmpColumnNum + " == 1) ? '<span>‡</span>' : ''#" +
                                        "#=(mio" + tmpColumnNum + " == 1) ? '<span>^</span>' : ''#" +
                                        "#=(tfia" + tmpColumnNum + " == 1) ? '<span>\\#</span>' : ''#" +
                                        "#=('" + column.title + "'== 'V' && scni" + tmpColumnNum + " == 1) ? '<span>\\¥</span>' : ''#" +
                                        "#=('" + column.title + "'== 'Total Score' && vats == 1) ? '<span>\\¥</span>' : ''#" +
                                        "</span>";
                                    //console.log(tmpColumn["template"]);
                                }
                            }
                            //tmpColumn["field"] = arr[i];
                            tmpColumn["field"] = arr[i].field;
                            //tmpColumn["aggregates"] = ["sum"];
                            //tmpColumn["footerTemplate"] = "#=sum#";
                            //!!!tmpColumn["aggregates"] = ["average"];
                            //tmpColumn["footerTemplate"] = "#=average#";
                            //tmpColumn["footerTemplate"] = "#: kendo.toString(average, 'n2') #";
                            //tmpColumn["footerTemplate"] = "#: kendo.toString(average, 'n0') #";
                            tmpValue = column.fields[i].field;
                            //tmpColumn["footerTemplate"] = "#=(average == null) ? '' : kendo.toString(average, 'n0') #";
                            if (typeof private.modelRosterGroupTotals.group_total === "undefined" || private.modelRosterGroupTotals.group_total[tmpValue] === 0 || private.modelRosterGroupTotals.group_total[tmpValue] === null) {
                                tmpColumn["footerTemplate"] = "";
                                //tmpColumn["footerTemplate"] = "#: (typeof data.group_total !== 'undefined') ? data.group_total.AS0 : 1 #";
                                //tmpColumn["footerTemplate"] = "#: kendo.toString(typeof data.group_total) #";
                            } else {
                                tmpColumn["footerTemplate"] = "" + private.modelRosterGroupTotals.group_total[tmpValue];
                            }
                            multiColumn.push(tmpColumn);
                            //tmpColumn["template"] = "<span class='roster-highlighted-cell-bg'></span><span class='roster-highlighted-cell-content'>#= (" + tmpValue + " == null) ? '' : " + tmpValue + " #</span>";
                        }
                        kendoColumn["columns"] = multiColumn;
                    } else {
                        kendoColumn["field"] = column["field"];
                        //!!!kendoColumn["aggregates"] = ["count"];
                        //kendoColumn["footerTemplate"] = "Group Total (#=count#)";
                        kendoColumn["footerTemplate"] = "Group Total";
                    }

                    kendoColumn["title"] = column["title"];

                    if (index > 1) {
                        kendoColumn["filterable"] = false;
                    } else {
                        if ($.isEmptyObject(private.modelForSearchFilteredRoster)) {
                            kendoColumn["filterable"] = {
                                cell: {
                                    suggestionOperator: "contains"
                                }
                            };

                        } else {
                            kendoColumn["filterable"] = {
                                cell: {
                                    dataSource: private.modelForSearchFilteredRoster,
                                    suggestionOperator: "contains"
                                }
                            };
                        }
                    }

                    var wcagClass = "";
                    var wcagTabIndex = "";
                    if (!private.isTabNavigationOn) {
                        wcagClass = " tab";
                        wcagTabIndex = ' tabindex="0"';
                    }
                    if (typeof column["field"] != "undefined") {

                        if (column["field"] === "node_name") {
                            //kendoColumn["template"] = "<a href='\\#' class='student-info' tabindex='-1' tabindex-important='true'>#:node_name#</a><span class='icon-student-info' alt='Student Detail' title='Student Detail' data-age='#:birth_date#' data-class='#:class_name#' data-school='#:building#' data-ability-profile='#:ability_profile#'></span>";
                            //kendoColumn["template"] = "<a href='\\#' class='student-info' tabindex='-1' tabindex-important='true' data-dob='#:birth_date#' data-node-id='#:node_id#' data-age='#:age#' data-class='#:class_name#' data-school='#:building#' data-ability-profile='#:ability_profile#'>#:node_name#</a><span class='icon-student-info' alt='Student Detail' title='Student Detail'></span>";
                            //kendoColumn["template"] = "<span class='student-info' data-dob='#:birth_date#' data-node-id='#:node_id#' data-age='#:age#' data-class='#:class_name#' data-school='#:building#' data-ability-profile='#:ability_profile#'>#:node_name#</span><span class='icon-student-info' alt='Student Detail' title='Student Detail'></span>";
                            kendoColumn["template"] = "<span class='student-info" + wcagClass + "'" + wcagTabIndex + " data-dob='#:birth_date#' data-node-id='#:node_id#' data-age='#:age#' data-class='#:class_name#' data-school='#:building#' data-ability-profile='#:ability_profile#'>#:node_name#</span><span class='warnings'>" +
                                "#= (el == 1) ? '<span class=\"tooltip\">~<span class=\"tooltiptext\" role=\"tooltip\">~ Estimated level</span></span>' : '' #" +
                                "#= (lucg == 1) ? '<span class=\"tooltip\">§<span class=\"tooltiptext\" role=\"tooltip\">§ Level unusual for coded grade</span></span>' : '' #" +
                                "#= (aucl == 1) ? '<span class=\"tooltip\">ã<span class=\"tooltiptext\" role=\"tooltip\">ã Age unusual for coded level</span></span>' : '' #" +
                                "#= (aoor == 1) ? '<span class=\"tooltip\">«<span class=\"tooltiptext\" role=\"tooltip\">« Age is out-of-range</span></span>' : '' #" +
                                "</span>";
                            //kendoColumn["template"] = "<a href='\\#' class='student-info' tabindex='-1' tabindex-important='true' data-age='#:birth_date#' data-class='#:class_name#' data-school='#:building#' data-ability-profile='#= (" + tmpValue + " == null) ? '' : " + tmpValue + " #'>#:node_name#</a><span class='icon-student-info' alt='Student Detail' title='Student Detail'></span>";
                            //kendoColumn["template"] = "<a href='\\#' class='student-info' tabindex='-1' tabindex-important='true' data-age='' data-class='' data-school='' data-ability-profile=''>#:node_name#</a><span class='icon-student-info' alt='Student Detail' title='Student Detail'></span>";
                            //kendoColumn["template"] = "<a href='#:link#' class='location-drill' tabindex='-1' tabindex-important='true'>#:node_name#</a><img src='Reskin/Content/img/icon-student-info.svg' class='icon-student-info' alt='Student Detail' title='Student Detail' />";
                        } else {
                            //kendoColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + ": #:" + column["field"] + "#'>#:" + column["field"] + "#</span>";
                            kendoColumn["template"] = "#:" + column["field"] + "#";
                        }
                    }

                    if (column["multi"]) {
                        //kendoColumn["headerTemplate"] = "<span class='kendo-grid-cell' aria-label='" + kendoColumn["title"] + ": " + column["title_full"] + "'>" + kendoColumn["title"] + "</span>";
                        kendoColumn["headerTemplate"] = kendoColumn["title"];
                    }
                    kendoColumns.push(kendoColumn);

                    if (kendoColumn["title"] !== column["title_full"]) {
                        arrLegendShort.push(kendoColumn["title"]);
                        arrLegendFull.push(column["title_full"]);
                    }
                }
            }

            //generate LEGEND under the Roster Table (compare roster type)
            //$("#roster-name").attr("aria-label", rosterTitle);
            //$(".section-card.roster-table-card").attr("aria-label", rosterTitle.replace("Score ", "") + " Table");
            var legend = '<h3 class="roster-legend-header">Legend</h3>';
            legend += '<dl class="legend">';
            for (i = 0; i < arrLegendShort.length; i++) {
                legend += "<dt>" + arrLegendShort[i] + ":</dt><dd>" + Dashboard.ConvertLegendText(arrLegendFull[i]) + "</dd>";
            }
            legend += "</dl>";
            $("#roster-legend-wrapper").html(legend);

            return kendoColumns;
        },

        ConvertLegendText: function (text) {
            if (text === "Nonverbal") {
                text = "Nonverbal";
            }
            if (text === "Composite (VQ)") {
                text = "Composite Verbal and Quantitative";
            }
            if (text === "Composite (VN)") {
                text = "Composite Verbal and Nonverbal";
            }
            if (text === "Composite (QN)") {
                text = "Composite Quantitative and Nonverbal";
            }
            if (text === "Composite (VQN)") {
                text = "Overall composite score for Verbal, Quantitative, and Nonverbal";
            }
            return text;
        },

        insertTooltipsToHeaderOfRosterTable: function (rosterStructure) {
            var arrTooltips = [];
            var column;
            var cardIndex = 0;
            var breakIndex = private.MaxRosterShownColumnsNum;
            for (var key in rosterStructure) {
                if (rosterStructure.hasOwnProperty(key)) {
                    cardIndex++;
                    if (cardIndex >= breakIndex) {
                        break;
                    }
                    column = rosterStructure[key];
                    if (column["title_full"] !== column["title"]) {
                        arrTooltips.push(column["title_full"]);
                        //arr_legend_short = column["title"];
                    } else {
                        arrTooltips.push("");
                    }
                }
            }

            var i = 0;
            $("#roster-table-wrapper table thead tr:first-child th").each(function () {
                if (typeof arrTooltips[i] != "undefined") {
                    if (arrTooltips[i] !== "") {
                        $(this).addClass("tooltip");
                        $(this).append('<span class="tooltiptext" role="tooltip">' + Dashboard.ConvertLegendText(arrTooltips[i]) + "</span>");
                    }
                }
                i++;
            });

            //adding tooltips to second header row
            var arrTooltips2 = [];
            var arrTooltipsShort2 = [];
            for (var j = 0; j < rosterStructure.length; j++) {
                if (rosterStructure[j].fields !== undefined && rosterStructure[j].fields !== null) {
                    if (rosterStructure[j].fields.length) {
                        for (var k = 0; k < rosterStructure[j].fields.length; k++) {
                            arrTooltips2.push(rosterStructure[j].fields[k].title_full);
                            arrTooltipsShort2.push(rosterStructure[j].fields[k].title);
                        }
                        break;
                    }
                }
            }
            if (arrTooltips2.length >= 1) {
                $("#roster-table-wrapper table thead tr:nth-child(2) th:nth-child(" + arrTooltips2.length + "n+1)").each(function () {
                    $(this).addClass("tooltip");
                    $(this).append('<span class="tooltiptext" role="tooltip">' + arrTooltips2[0] + "</span>");
                });
            }
            if (arrTooltips2.length >= 2) {
                $("#roster-table-wrapper table thead tr:nth-child(2) th:nth-child(" + arrTooltips2.length + "n+2)").each(function () {
                    $(this).addClass("tooltip");
                    $(this).append('<span class="tooltiptext" role="tooltip">' + arrTooltips2[1] + "</span>");
                });
            }
            if (arrTooltips2.length >= 3) {
                $("#roster-table-wrapper table thead tr:nth-child(2) th:nth-child(" + arrTooltips2.length + "n+3)").each(function () {
                    $(this).addClass("tooltip");
                    $(this).append('<span class="tooltiptext" role="tooltip">' + arrTooltips2[2] + "</span>");
                });
            }

            for (var k = 0; k < arrTooltips2.length; k++) {
                $("#roster-legend-wrapper dl").append("<dt>" + arrTooltipsShort2[k] + ":</dt><dd> " + arrTooltips2[k] + "</dd>");
            }

            var numScoreWarnings = 0;
            if ($.inArray("NANI", private.arrColumnsSelectedInRoster) !== -1) {
                numScoreWarnings++;
            }
            if (private.WarningsDropdownOptions.indexOf('value="ts"') !== -1) numScoreWarnings++;
            if (private.WarningsDropdownOptions.indexOf('value="irp"') !== -1) numScoreWarnings++;
            if (private.WarningsDropdownOptions.indexOf('value="mio"') !== -1) numScoreWarnings++;
            if (private.WarningsDropdownOptions.indexOf('value="tfia"') !== -1) numScoreWarnings++;
            //if (private.WarningsDropdownOptions.indexOf('value="scni"') !== -1) numScoreWarnings++;
            if ((private.arrSelectedTopFilterScores.length === 7 && ((arrTooltips2.length === 2 && numScoreWarnings > 3) || (arrTooltips2.length === 3 && numScoreWarnings > 1))) ||
                (private.arrSelectedTopFilterScores.length === 6 && ((arrTooltips2.length === 2 && numScoreWarnings > 3) || (arrTooltips2.length === 3 && numScoreWarnings > 2))) ||
                (private.arrSelectedTopFilterScores.length === 5 && ((arrTooltips2.length === 2 && numScoreWarnings > 3) || (arrTooltips2.length === 3 && numScoreWarnings > 3))) ||
                (private.arrSelectedTopFilterScores.length === 4 && (arrTooltips2.length === 3 && numScoreWarnings > 3))) {
                $("#roster-table-wrapper").addClass("warnings-type2");
            } else {
                $("#roster-table-wrapper").removeClass("warnings-type2");
            }
            $("#roster-table-wrapper").removeClass("subcolumns1 subcolumns2 subcolumns3");
            if (private.isScreener) {
                $("#roster-table-wrapper").addClass("subcolumns1");
            } else {
                $("#roster-table-wrapper").addClass("subcolumns" + arrTooltips2.length);
            }
        },

        resetInputValuesOfRosterSearchPopup: function () {
            //Dashboard.initDropdownsValuesOfRosterSearchPopup();
            $("#modal-dashboard-report-criteria .dm-ui-alert-error").remove();

            $("#dropdown-score-1 option:first-child, #dropdown-score-2 option:first-child, #dropdown-score-3 option:first-child").prop("selected", true);
            $("#dropdown-score-1 option:first-child, #dropdown-score-2 option:first-child, #dropdown-score-3 option:first-child").trigger("change");
            $("#dropdown-score-1 option:first-child, #dropdown-score-2 option:first-child, #dropdown-score-3 option:first-child").trigger("DmUi:updated");

            $("#dropdown-content-area-1 option:first-child, #dropdown-content-area-2 option:first-child, #dropdown-content-area-3 option:first-child").prop("selected", true);
            $("#dropdown-content-area-1 option:first-child, #dropdown-content-area-2 option:first-child, #dropdown-content-area-3 option:first-child").trigger("change");
            $("#dropdown-content-area-1 option:first-child, #dropdown-content-area-2 option:first-child, #dropdown-content-area-3 option:first-child").trigger("DmUi:updated");

            $("#dropdown-condition-1 option:first-child, #dropdown-condition-2 option:first-child, #dropdown-condition-3 option:first-child").prop("selected", true);
            $("#dropdown-condition-1 option:first-child, #dropdown-condition-2 option:first-child, #dropdown-condition-3 option:first-child").trigger("change");
            $("#dropdown-condition-1 option:first-child, #dropdown-condition-2 option:first-child, #dropdown-condition-3 option:first-child").trigger("DmUi:updated");

            $("#input-score-1-val1, #input-score-1-val2, #input-score-2-val1, #input-score-2-val2, #input-score-3-val1, #input-score-3-val2").val("");

            $("#dropdown-boolean-1 option:first-child").prop("selected", true);
            $("#dropdown-boolean-1").trigger("change");
            $("#dropdown-boolean-1").trigger("DmUi:updated");
            $("#dropdown-boolean-2 option:first-child").prop("selected", true);
            $("#dropdown-boolean-2").trigger("change");
            $("#dropdown-boolean-2").trigger("DmUi:updated");
        },

        resetInputValuesOfRosterWarningSearchPopup: function () {
            //Dashboard.initDropdownsValuesOfRosterSearchPopup();
            $("#modal-dashboard-score-warning-report-criteria .dm-ui-alert-error").remove();

            $("#dropdown-warning-score-1 option:first-child, #dropdown-warning-score-2 option:first-child, #dropdown-warning-score-3 option:first-child").prop("selected", true);
            $("#dropdown-warning-score-1 option:first-child, #dropdown-warning-score-2 option:first-child, #dropdown-warning-score-3 option:first-child").trigger("change");
            $("#dropdown-warning-score-1 option:first-child, #dropdown-warning-score-2 option:first-child, #dropdown-warning-score-3 option:first-child").trigger("DmUi:updated");

            $("#dropdown-warning-boolean-1 option:first-child").prop("selected", true);
            $("#dropdown-warning-boolean-1").trigger("change");
            $("#dropdown-warning-boolean-1").trigger("DmUi:updated");
            $("#dropdown-warning-boolean-2 option:first-child").prop("selected", true);
            $("#dropdown-warning-boolean-2").trigger("change");
            $("#dropdown-warning-boolean-2").trigger("DmUi:updated");
        },

        appendFeedBackButtonAndForm: function () {
            if (!$("feedback-button-wrapper").length) {
                if (private.isTabNavigationOn) {
                    $(".body-content").append('<div id="feedback-button-wrapper"><button class="dm-ui-button-primary dm-ui-button-small" id="feedback-button" tabindex="4">Feedback</button></div>');
                } else {
                    $(".body-content").append('<div id="feedback-button-wrapper"><button class="dm-ui-button-primary dm-ui-button-small" id="feedback-button">Feedback</button></div>');
                }
            }
        },

        initSelectedContentArea: function () {
            var i = 0, j, result = getEmptyJsonStanineTable(private.isScreener);

            private.arrSelectedTopFilterScores = [];
            $(".filters select#Content_Control option").each(function () {
                if ($(this).prop("selected")) {
                    private.arrSelectedTopFilterScores.push(i);
                }
                i++;
            });
            //console.log(private.arrSelectedTopFilterScores);

            var resultValuesSelected = [];
            for (i = 0; i < 7; i++) {
                if ($.inArray(i, private.arrSelectedTopFilterScores) !== -1) {
                    //console.log("Add " + i);
                    resultValuesSelected.push(result.values[i]);
                }
            }
            //console.log(resultValuesSelected);
            result.values = resultValuesSelected;

            private.RosterDropdownOptionsContentArea = "";
            for (i = 0; i < private.arrSelectedTopFilterScores.length; i++) {
                private.RosterDropdownOptionsContentArea += '<option value="' + result.values[i].content_area + '">' + result.values[i].content_area + "</option>";
            }
        },

        initDropdownsValuesOfRosterSearchPopup: function () {
            $("#dropdown-content-area-1").empty();
            $("#dropdown-content-area-1").append(private.RosterDropdownOptionsContentArea);
            $("#dropdown-content-area-1").trigger("DmUi:updated");

            private.RosterDropdownOptionsScore = "";
            $("#dropdown-content-area-2").empty();
            //$("#dropdown-content-area-2").append(private.RosterDropdownOptionsScore);
            $("#dropdown-content-area-2").append(private.RosterDropdownOptionsContentArea);
            $("#dropdown-content-area-2").trigger("DmUi:updated");
            $("#dropdown-content-area-3").empty();
            //$("#dropdown-content-area-3").append(private.RosterDropdownOptionsScore);
            $("#dropdown-content-area-3").append(private.RosterDropdownOptionsContentArea);
            $("#dropdown-content-area-3").trigger("DmUi:updated");

            $("#dropdown-condition-1").empty();
            $("#dropdown-condition-1").append(private.RosterDropdownOptionsCondition);
            $("#dropdown-condition-1 option:first-child").prop("selected", true);
            $("#dropdown-condition-1").trigger("DmUi:updated");
            $("#dropdown-condition-2").empty();
            $("#dropdown-condition-2").append(private.RosterDropdownOptionsCondition);
            //$("#dropdown-condition-2").find("option:last-child").remove();
            $("#dropdown-condition-2 option:first-child").prop("selected", true);
            $("#dropdown-condition-2").trigger("DmUi:updated");
            $("#dropdown-condition-3").empty();
            $("#dropdown-condition-3").append(private.RosterDropdownOptionsCondition);
            //$("#dropdown-condition-3").find("option:last-child").remove();
            $("#dropdown-condition-3 option:first-child").prop("selected", true);
            $("#dropdown-condition-3").trigger("DmUi:updated");
        },

        moveOutRosterFilters: function () {
            var fieldFullTitle;
            if ($("#roster-table-wrapper .k-grid-header .k-filter-row").length && typeof private.modelRoster.columns !== "undefined") {
                $("#roster-search-field").empty();
                $("#roster-search-field").append($("#roster-table-wrapper .k-grid-header .k-filter-row th .k-filtercell"));

                $("#roster-search-field input.k-input").each(function (i) {
                    if ($(this).attr("aria-label") != null) {
                        if (i === 0) {
                            fieldFullTitle = private.modelRoster.columns[i]["title_full"];
                        }
                        if (i === 2) {
                            fieldFullTitle = private.modelRoster.columns[i - 1]["title_full"] + " score";
                        }
                        if (i === 0) {
                            $(this).attr("placeholder", "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name");
                            $(this).attr("aria-label", "Search");
                            $(this).attr("id", "roster-search");
                            $(this).attr("aria-describedby", "wcag-autocomplete-search-instructions");
                            $("#roster-search-field").prepend('<label class="floating-label" id="roster-search-label" for="roster-search">' + "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name" + '</label>');
                            $(this).prev("input").attr("placeholder", "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name");
                        }
                        $(this).attr("data-full-title", fieldFullTitle);
                        $(this).prev("input").attr("data-full-title", fieldFullTitle);

                        //$(this).attr("tabindex", $(this).parents(".root-tab-element").data("tabindex")); //add tabindex for search field
                        $(this).addClass("tab");
                        if (private.isTabNavigationOn) {
                            $(this).parents(".k-filtercell").find(".k-dropdown-operator").attr("tabindex", $(this).parents(".root-tab-element").data("tabindex")); //add tabindex for search criteria icon
                        }
                        $(this).parents(".k-filtercell").find(".k-dropdown-operator").addClass("tab");
                    }
                });
                var resetRosterButtonDisabledClass = "disabled-element";
                if ($("#roster-selected-labels-wrapper .arrow").length > 0) {
                    resetRosterButtonDisabledClass = "";
                }
                if ($("#modal-dashboard-print-students").is(":visible")) {
                    $("#modal-dashboard-print-students").fadeOut("fast", function () { });
                }

                //clear empty filter cells
                $("#roster-search-field .k-filter-row th").each(function () {
                    if (!$(this).text().trim()) {
                        $(this).remove();
                    }
                });
                /*
                                //move all filters except by title to popup
                                $("#roster-search-field .k-filter-row th").each(function (i) {
                                    if (i === 1) {
                                        $(".table-report-filters tbody").empty();
                                    }
                                    if (i >= 1) {
                                        $(".table-report-filters").append("<tr id='popup_filter_" + i + "' class='k-filter-row'><th></th></tr>");
                                        $(".table-report-filters #popup_filter_" + i).append($(this));
                                        $(".table-report-filters #popup_filter_" + i + " th:nth-child(2)").prepend('<label class="filter-condition-label">Condition</label>');
                                        $(".table-report-filters #popup_filter_" + i + " th:nth-child(2) .k-numerictextbox").prepend('<label class="filter-score-label">Score</label>');
                                        //$(".table-report-filters #popup_filter_" + i + " th:first-child").append("Filter " + $(".table-report-filters #popup_filter_" + i + " th input").data("full-title") + ":");
                                        $(".table-report-filters #popup_filter_" + i + " th:first-child").append($(".table-report-filters #popup_filter_" + i + " th input").data("full-title").replace(" score", "") + ":");
                                    }
                                });
                */
            }
        },

        ClearRosterGridActiveCells: function () {
            var activeCellElements = $("#roster-table-wrapper_active_cell");
            if (activeCellElements.length) {
                console.log("REMOVE active cell:");
                activeCellElements.removeAttr("id");
            }
            /*
            activeCellElements = $("#roster-table-wrapper .k-state-focused");
            if (activeCellElements.length) {
                activeCellElements.removeClass("k-state-focused");
            }
            */
        },

        FocusLastFocusedElement: function () {
            //console.log("trying to focus last focused element!!!");
            if (!$.isEmptyObject(private.LastFocusedElement)) {
                //console.log("set focus to:");
                //console.log(private.LastFocusedElement);
                private.LastFocusedElement.focus();
                //$("body").addClass("wcag_focuses_on");
            } else {
                //console.log("empty object!!!");
            }
        },

        MakeContentTabbableWithDelay: function (element, delay) {
            function makeElementContentTabbableWithDelay() {
                element.rootMakeContentTabbable();
            }
            setTimeout(makeElementContentTabbableWithDelay, delay);
        },

        FocusElementWithDelay: function (element, delay) {
            function focusElement() {
                $("body").addClass("wcag_focuses_on");
                //$("#print-report-center").focus();
                element.focus();
                /*
                                $(this).trigger({
                                    type: "keypress",
                                    which: 9
                                });
                */
            }
            setTimeout(focusElement, delay);
        },

        AddCssClassToElementWithDelay: function (element, cssClass, delay) {
            function addCssClassToElement() {
                $(element).addClass(cssClass);
            }
            setTimeout(addCssClassToElement, delay);
        },

        KendoGridChangeAttributesForBetterWcag: function () {
            $("#roster-table-wrapper td[aria-describedby]").removeAttr("aria-describedby");
            $(".dashboard-table td[aria-describedby]").removeAttr("aria-describedby");
        },

        CloneFilteredDataRightTopCard: function () {
            if (typeof $("#roster-table-wrapper").data("kendoGrid") !== "undefined") {
                Dashboard.DebugRememberTime();

                var dataSource = $("#roster-table-wrapper").data("kendoGrid").dataSource;
                var allData = dataSource.data();
                var query = new kendo.data.Query(allData);
                private.modelFilteredRosterRightTopCard = query.filter(private.CurrentPopupCriteriaRosterFilter).data;

                //console.log(private.modelFilteredRoster);
                Dashboard.DebugShowTime("clone roster (right top card)");
            }
        },

        CloneFilteredDataRoster: function () {
            Dashboard.DebugRememberTime();

            var dataSource = $("#roster-table-wrapper").data("kendoGrid").dataSource;
            var filters = dataSource.filter();
            var allData = dataSource.data();
            var query = new kendo.data.Query(allData);
            private.modelFilteredRoster = query.filter(filters).data;

            //console.log(private.modelFilteredRoster);
            Dashboard.DebugShowTime("clone filtered roster data");
        },

        UpdateRightCard: function () {
            if (!private.UseServerPaging) {
                Dashboard.CloneFilteredDataRightTopCard();
            }
            Dashboard.redrawRightCardTable();
            private.isCutScoreCardGenerated = true;
        },

        UpdateRightCard2: function () {
            Dashboard.CloneFilteredDataRoster();
            Dashboard.redrawRightCardTable2();
        },

        CheckAgeStanineEnabled: function () {
            if ($("#stanine-table-selected-label").text() !== "") {
                $("#Score_Control_dm_ui .dm-ui-dropdown-items li:nth-child(2) input").attr("disabled", "True");
            } else {
                $("#Score_Control_dm_ui .dm-ui-dropdown-items li:nth-child(2) input").removeAttr("disabled");
            }
        },

        UpdateCogatFormNormYear: function () {
            if (!private.isCogatFormNormYearUpdated && typeof private.modelRoster.extra_params !== "undefined") {
                var form = private.modelRoster.extra_params.form;
                var arr = form.split(",");
                for (var i = 0; i < arr.length; ++i) {
                    arr[i] = Number(arr[i]);
                }
                arr.sort();
                form = arr.join(",");
                $("#cogat-header-form").text(form);
                $("#cogat-header-year").text(private.modelRoster.extra_params.norm_year);
                $(".stanine-card .card-header-content").addClass("visible");
                private.isCogatFormNormYearUpdated = true;
                Dashboard.UpdateRosterScoresDropdown();
            }
            Dashboard.ChangeWarningsDropdownOptions(private.modelRoster);
        },

        UpdateRosterScoresDropdown: function () {
            if (typeof private.modelRoster.extra_params !== "undefined") {
                var optionsToInsert = "";
                if (private.modelRoster.extra_params.has_lpr_score) {
                    if (!$("#Score_Control option[value=LPR]").length) {
                        //$("#Score_Control").append('<option value="LPR">Local Percentile Rank</option>');
                        optionsToInsert += '<option value="LPR">Local Percentile Rank</option>';
                    }
                } else {
                    $("#Score_Control option[value=LPR]").remove();
                    $("#dropdown-score-1 option[value=LPR], #dropdown-score-2 option[value=LPR], #dropdown-score-3 option[value=LPR]").remove();
                }

                if (private.modelRoster.extra_params.has_ls_score) {
                    if (!$("#Score_Control option[value=LS]").length) {
                        //$("#Score_Control").append('<option value="LS">Local Stanine</option>');
                        optionsToInsert += '<option value="LS">Local Stanine</option>';
                    }
                } else {
                    $("#Score_Control option[value=LS]").remove();
                    $("#dropdown-score-1 option[value=LS], #dropdown-score-2 option[value=LS], #dropdown-score-3 option[value=LS]").remove();
                }

                if (optionsToInsert !== "") {
                    var options = $("select#Score_Control").html();
                    var begin = options.substr(0, options.indexOf("Raw Score") + 18);
                    var end = options.substr(options.indexOf("Raw Score") + 18);
                    options = begin + optionsToInsert + end;
                    $("select#Score_Control").html(options);
                    $("select#Score_Control").trigger("DmUi:updated");

                    $("#dropdown-score-1, #dropdown-score-2, #dropdown-score-3").append(optionsToInsert);
                    $("#dropdown-score-1, #dropdown-score-2, #dropdown-score-3").trigger("DmUi:updated");
                }
            }
        },

        UpdateRosterGrid: function () {
            //Dashboard.UpdateRosterSearchFilteredModel();

            Dashboard.DebugRememberTime();

            //checking if "APR" column exist in selected columns
            if (private.arrColumnsSelectedInRoster.length === 0) {
                private.arrColumnsSelectedInRoster = ["APR"];
            } else {
                /*
                                if (private.arrColumnsSelectedInRoster.indexOf("APR") === -1) {
                                    if (private.arrColumnsSelectedInRoster.length >= 3) {
                                        private.arrColumnsSelectedInRoster = ["APR"];
                                    } else {
                                        private.arrColumnsSelectedInRoster.push("APR");
                                    }
                                }
                */
            }
            //console.log("private.arrColumnsSelectedInRoster");
            //console.log(private.arrColumnsSelectedInRoster);

            //update roster score columns small dropdown
            $("select#Score_Control option").prop("selected", false);
            $("select#Score_Control option").each(function () {
                //if ($.inArray($(this).val(), private.arrColumnsSelectedInRoster) !== -1) {
                if (private.arrColumnsSelectedInRoster.indexOf($(this).val()) !== -1) {
                    $(this).prop("selected", true);
                }
            });
            $("select#Score_Control").trigger("DmUi:updated");
            Dashboard.CheckAgeStanineEnabled();

            //update columns in roster model object
            var columns = JSON.parse(JSON.stringify(private.modelRosterAllColumns)); //clone object
            var field;
            for (var i = 0; i < columns.length; ++i) {
                if (columns[i].multi === 1) {
                    var arrFieldsSelected = [];
                    for (var j = 0; j < columns[i].fields.length; ++j) {
                        field = columns[i].fields[j].field;
                        if ($.inArray(field.substring(0, field.length - 1), private.arrColumnsSelectedInRoster) !== -1) {
                            arrFieldsSelected.push(columns[i].fields[j]);
                        }
                    }
                    columns[i].fields = arrFieldsSelected;
                }
            }
            private.modelRoster.columns = columns;

            if (typeof private.modelRoster.values !== "undefined") {
                //recreate grid with updated shown columns
                $("#roster-table-wrapper").empty(); //must to clear before recreate data!
                $("#roster-table-wrapper").GenerateRosterTable({
                    //'data': getSampleJsonRosterTable(studentsNum),
                    //'data': data,
                    'data': private.modelRoster,
                    'class_of_table': "dm-ui-table"
                });

                if (!private.UseServerPaging) {
                    //apply current roster filters
                    $("#roster-table-wrapper").data("kendoGrid").dataSource.filter(private.CurrentRosterFilter);
                    //$("#roster-table-wrapper table").show();
                }
            }

            //if (private.CurrentRosterFilter.filters.length) {
            if (private.CurrentRosterFilter.filters !== undefined) {
                Dashboard.EnableRosterResetButton();
            } else {
                Dashboard.DisableRosterResetButton();
            }

            $("#roster-table-wrapper > table").removeAttr("tabindex"); //if not removed -kendo table will be the last focused element on page
            $("#roster-table-wrapper > table").removeAttr("role");
            $("#roster-table-wrapper > table thead th[data-role=columnsorter] a").attr("role", "button");
            setTimeout(function () {
                $("#roster-table-wrapper > table td, #roster-table-wrapper > table tbody th").removeAttr("role");
                $("#roster-table-wrapper > table thead tr:first-child th:not(:first-child)").attr("scope", "colgroup");
                if (private.isTabNavigationOn) {
                    $("#roster-table-wrapper > table td, #roster-table-wrapper > table th").attr("tabindex", "0").addClass("tab");
                } else {
                    $("#roster-table-wrapper > table thead th[data-role=\"columnsorter\"]").addClass("tab").attr("tabindex", "0");
                }
            }, 1000);
            Dashboard.DebugShowTime("update roster");
        },

        PrintPopupEqualizeTabHeight: function () {
            $("#print-report-center #content-tab1, #print-report-center #content-tab2").css("height", "");
            var height;
            if ($("#print-report-center #content-tab2").height() >= $("#print-report-center #content-tab1").height()) {
                //$("#print-report-center #content-tab1").height($("#print-report-center #content-tab2").height());
                height = $("#print-report-center #content-tab2").height();
            } else {
                //$("#print-report-center #content-tab2").height($("#print-report-center #content-tab1").height());
                height = $("#print-report-center #content-tab1").height();
            }
            height = Number(height) + 24;
            $("#print-report-center #content-tab1, #print-report-center #content-tab2").css("height", height + "px");
        },

        ClearAllFiltersSelection: function () {
            $("#stanine-table-selected-label").empty();
            Dashboard.CheckAgeStanineEnabled();
            $("#right-card-selected-label").empty();
            $("#right-card2-selected-label").empty();
            $("#roster-card-selected-popup-filters-label").empty();
            $("#roster-card-selected-popup-warning-filters-label").empty();
        },

        ValidateReportFilters: function () {
            var val, isValidated;
            $(".table-report-filters tr:not(.filter-row-disabled) td.column-val1 input, .table-report-filters tr:not(.filter-row-disabled) td.column-val2 input:visible").each(function () {
                val = $(this).val();
                isValidated = false;
                if (!$(this).prop("disabled")) {
                    if ($.isNumeric(val)) {
                        if (Number(val) >= 0 && Number(val) <= 400) {
                            if (val === "0" || (val.substr(0, 1) !== "0" && val.substr(0, 2) !== "00")) {
                                if ($(this).parents(".column-val2").length) {
                                    //in between validation
                                    if (Number(val) >= Number($(this).parents(".column-val2").prev().find("input").val())) {
                                        isValidated = true;
                                    }
                                } else {
                                    isValidated = true;
                                }
                            }
                        }
                    }
                    if (!isValidated) {
                        return false;
                    }
                }
            });
            if (isValidated) {
                $("#apply-dashboard-report-button").prop("disabled", false);
                $("#cancel-dashboard-report-button").removeClass("last-tab-element wcag-modal-last-element");
                $("#apply-dashboard-report-button").addClass("last-tab-element wcag-modal-last-element");
            } else {
                $("#apply-dashboard-report-button").prop("disabled", true);
                $("#cancel-dashboard-report-button").addClass("last-tab-element wcag-modal-last-element");
                $("#apply-dashboard-report-button").removeClass("last-tab-element wcag-modal-last-element");
            }
        },

        CutScoreLabelText: function () {
            var filtersLabelText = "<span><b>" + $("#modal-dashboard-report-criteria #dropdown-score-1 option:selected").text();
            filtersLabelText += " " + $("#modal-dashboard-report-criteria #dropdown-content-area-1 option:selected").text() + "</b>";
            filtersLabelText += " " + $("#modal-dashboard-report-criteria #dropdown-condition-1 option:selected").text();
            filtersLabelText += " <b>" + $("#modal-dashboard-report-criteria #input-score-1-val1").val() + "</b>";
            if ($("#modal-dashboard-report-criteria #dropdown-condition-1 option:selected").text() === "is in between") {
                filtersLabelText += " and <b>" + $("#modal-dashboard-report-criteria #input-score-1-val2").val() + "</b>";
            }
            $("#modal-dashboard-report-criteria #dropdown-boolean-1, #modal-dashboard-report-criteria #dropdown-boolean-2").each(function (index) {
                if ($(this).val() !== "") {
                    filtersLabelText += " " + $(this).val().toUpperCase();
                    filtersLabelText += "</span>";
                    filtersLabelText += "<br><span><b>" + $("#modal-dashboard-report-criteria #dropdown-score-" + (index + 2) + " option:selected").text();
                    filtersLabelText += " " + $("#modal-dashboard-report-criteria #dropdown-content-area-" + (index + 2) + " option:selected").text() + "</b>";
                    filtersLabelText += " " + $("#modal-dashboard-report-criteria #dropdown-condition-" + (index + 2) + " option:selected").text();
                    filtersLabelText += " <b>" + $("#modal-dashboard-report-criteria #input-score-" + (index + 2) + "-val1").val() + "</b>";
                    if ($("#modal-dashboard-report-criteria #dropdown-condition-" + (index + 2) + " option:selected").text() === "is in between") {
                        filtersLabelText += " and <b>" + $("#modal-dashboard-report-criteria #input-score-" + (index + 2) + "-val2").val() + "</b>";
                    }
                }
            });
            filtersLabelText += "</b></span>";
            return filtersLabelText;
        },

        CutScoreWarningsLabelText: function () {
            var filtersLabelText = "<span><b>" + $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-score-1 option:selected").text() + "</b>";
            $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-boolean-1, #modal-dashboard-score-warning-report-criteria #dropdown-warning-boolean-2").each(function (index) {
                if ($(this).val() !== "") {
                    filtersLabelText += " " + $(this).val().toUpperCase();
                    filtersLabelText += "</span>";
                    filtersLabelText += "<br><span><b>" + $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-score-" + (index + 2) + " option:selected").text() + "</b>";
                }
            });
            filtersLabelText += "</b></span>";
            return filtersLabelText;
        },

        AppliedPopupFiltersToArray: function () {
            var arrFilters = [];
            var objFilter;
            $("#modal-dashboard-report-criteria #dropdown-score-1_dm_ui, #modal-dashboard-report-criteria #dropdown-score-2_dm_ui, #modal-dashboard-report-criteria #dropdown-score-3_dm_ui").each(function (i) {
                if (!$(this).hasClass("dm-ui-disabled")) {
                    objFilter = {};
                    objFilter["score"] = $("#modal-dashboard-report-criteria #dropdown-score-" + (i + 1)).val();
                    objFilter["content"] = $("#modal-dashboard-report-criteria #dropdown-content-area-" + (i + 1)).val();
                    objFilter["condition"] = $("#modal-dashboard-report-criteria #dropdown-condition-" + (i + 1)).val();
                    objFilter["val1"] = $("#modal-dashboard-report-criteria #input-score-" + (i + 1) + "-val1").val();
                    if (objFilter["condition"] === "is_in_between") {
                        objFilter["val2"] = $("#modal-dashboard-report-criteria #input-score-" + (i + 1) + "-val2").val();
                    } else {
                        objFilter["val2"] = "";
                    }
                    if (i < 2) {
                        objFilter["boolean"] = $("#modal-dashboard-report-criteria #dropdown-boolean-" + (i + 1)).val();
                    }
                    arrFilters.push(objFilter);
                }
            });
            return arrFilters;
        },

        AppliedPopupWarningFiltersToArray: function () {
            var arrFilters = [];
            var objFilter;
            $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-score-1_dm_ui, #modal-dashboard-score-warning-report-criteria #dropdown-warning-score-2_dm_ui, #modal-dashboard-score-warning-report-criteria #dropdown-warning-score-3_dm_ui").each(function (i) {
                if (!$(this).hasClass("dm-ui-disabled")) {
                    objFilter = {};
                    objFilter["score"] = $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-score-" + (i + 1)).val();
                    if (i < 2) {
                        objFilter["boolean"] = $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-boolean-" + (i + 1)).val();
                    }
                    arrFilters.push(objFilter);
                }
            });
            return arrFilters;
        },

        SaveAppliedPopupFiltersToCookie: function () {
            Dashboard.SetCookie("CutScoreFilters", JSON.stringify(Dashboard.AppliedPopupFiltersToArray()), 30);
        },

        RestoreAppliedPopupFiltersFromArray: function (arrFilters) {
            var objFilter, element;
            if (arrFilters.length) {
                for (var i = 0; i < arrFilters.length; i++) {
                    objFilter = arrFilters[i];
                    element = $("#modal-dashboard-report-criteria #dropdown-score-" + (i + 1));
                    element.val(objFilter["score"]).change();
                    element.trigger("DmUi:updated");

                    element = $("#modal-dashboard-report-criteria #dropdown-content-area-" + (i + 1));
                    element.val(objFilter["content"]).change();
                    element.trigger("DmUi:updated");

                    element = $("#modal-dashboard-report-criteria #dropdown-condition-" + (i + 1));
                    element.val(objFilter["condition"]).change();
                    element.trigger("DmUi:updated");

                    element = $("#modal-dashboard-report-criteria #input-score-" + (i + 1) + "-val1");
                    element.val(objFilter["val1"]);

                    element = $("#modal-dashboard-report-criteria #input-score-" + (i + 1) + "-val2");
                    if (objFilter["condition"] === "is_in_between") {
                        element.val(objFilter["val2"]);
                    } else {
                        element.val("");
                    }
                    if (i < 2) {
                        element = $("#modal-dashboard-report-criteria #dropdown-boolean-" + (i + 1));
                        element.val(objFilter["boolean"]).change();
                        element.trigger("DmUi:updated");
                    }
                }
                Dashboard.ValidateReportFilters();
            } else {
                Dashboard.resetInputValuesOfRosterSearchPopup();
            }
        },

        RestoreAppliedPopupWarningFiltersFromArray: function (arrFilters) {
            var objFilter, element;
            if (arrFilters.length) {
                for (var i = 0; i < arrFilters.length; i++) {
                    objFilter = arrFilters[i];
                    element = $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-score-" + (i + 1));
                    element.val(objFilter["score"]).change();
                    element.trigger("DmUi:updated");

                    if (i < 2) {
                        element = $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-boolean-" + (i + 1));
                        element.val(objFilter["boolean"]).change();
                        element.trigger("DmUi:updated");
                    }
                }
            } else {
                Dashboard.resetInputValuesOfRosterWarningSearchPopup();
            }
        },

        RestoreAppliedPopupFiltersFromCookie: function () {
            var arrFilters = [];
            var cookieValue = Dashboard.GetCookie("CutScoreFilters");
            if (cookieValue !== null && cookieValue !== "") {
                arrFilters = JSON.parse(cookieValue);
            }
            Dashboard.RestoreAppliedPopupFiltersFromArray(arrFilters);
            if (!arrFilters.length) {
                private.isCutScoreCardGenerated = true;
            }
            Dashboard.EnableRightCardPopupButton();
        },

        EnableRosterResetButton: function () {
            $("#roster-reset-button").removeClass("disabled-element");
            $("#roster-reset-button").makeElementTabbable();
        },

        DisableRosterResetButton: function () {
            $("#roster-reset-button").addClass("disabled-element");
            $("#roster-reset-button").makeElementNotTabbable();
        },

        EnableRosterFiltersPopupButton: function () {
            $("#roster-filters-popup-button").removeClass("disabled-element");
            $("#roster-filters-popup-button").makeElementTabbable();
            $("#roster-filters-popup-button").removeClass("applied");
        },

        DisableRosterFiltersPopupButton: function () {
            $("#roster-filters-popup-button").addClass("disabled-element");
            $("#roster-filters-popup-button").makeElementNotTabbable();
            $("#roster-filters-popup-button").removeClass("applied");
        },

        EnableRosterScoreWarningFiltersPopupButton: function () {
            /*
                        $("#roster-score-warning-filters-popup-button").removeClass("disabled-element");
                        $("#roster-score-warning-filters-popup-button").makeElementTabbable();
                        $("#roster-score-warning-filters-popup-button").removeClass("applied");
            */
            $("#roster-score-warning-filters-popup-button").addClass("visible");
            if (!private.isHybridAllDataReceived) {
                $("#roster-score-warning-filters-popup-button").addClass("disabled-element");
            } else {
                $("#roster-score-warning-filters-popup-button").removeClass("disabled-element");
            }
        },

        DisableRosterScoreWarningFiltersPopupButton: function () {
            /*
                        $("#roster-score-warning-filters-popup-button").addClass("disabled-element");
                        $("#roster-score-warning-filters-popup-button").makeElementNotTabbable();
                        $("#roster-score-warning-filters-popup-button").removeClass("applied");
            */
            $("#roster-score-warning-filters-popup-button").removeClass("visible");
        },

        EnableRightCardResetButton: function () {
            $("#right-card-reset-button").removeClass("disabled-element");
            $("#right-card-reset-button").makeElementTabbable();
        },

        DisableRightCardResetButton: function () {
            $("#right-card-reset-button").addClass("disabled-element");
            $("#right-card-reset-button").makeElementNotTabbable();
        },

        EnableRightCardPopupButton: function () {
            $("#right-card-popup-button").removeClass("disabled-element");
            $("#right-card-popup-button").makeElementTabbable();
            $("#right-card-popup-button").removeClass("applied");
        },

        DisableRightCardPopupButton: function () {
            $("#right-card-popup-button").addClass("disabled-element");
            $("#right-card-popup-button").makeElementNotTabbable();
            $("#right-card-popup-button").removeClass("applied");
        },

        SetCookie: function (name, value, days) {
            var d = new Date();
            d.setTime(d.getTime() + (days * 24 * 60 * 60 * 1000));
            var expires = "expires=" + d.toUTCString();
            document.cookie = name + "=" + value + ";" + expires + ";path=/";
        },

        GetCookie: function (name) {
            name = name + "=";
            var ca = document.cookie.split(";");
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) === " ") {
                    c = c.substring(1);
                }
                if (c.indexOf(name) === 0) {
                    return c.substring(name.length, c.length);
                }
            }
            return "";
        },

        DeleteCookie: function (name) {
            document.cookie = name + "=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;";
        },

        AssignAllEventHandlers: function () {
            //***** Print Dashboard ***** 
            $(document).on("click touchstart", "#print-dashboard, #print-reports-button", function (e) {
                e.preventDefault();
                //printDashboard();

                $("select#print-score-type").html($(".roster-table-card select#Score_Control").html());
                $("select#print-score-type").trigger("DmUi:updated");

                $("select#print-content-type").html($(".dashboard-filter select#Content_Control").html());
                $("select#print-content-type").trigger("DmUi:updated");

                //$("#print-student-profile").click();
                //$("#print-student-profile").trigger("click");
                showModalPrint(e);
                $("select#print-report-type").trigger("change");
                //Dashboard.PrintPopupEqualizeTabHeight();
                setTimeout(focusFirstPrintReports, 100);
            });
            $(document).on("keydown", "#print-reports-button", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    $(this).click();
                }
            });
            $(document).on("keyup", "#print-dashboard", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    printDashboard();
                }
            });
            function printDashboard() {
                /*
                    $("body").addClass("printStyles");
                    window.print();
                */
                //$("#form-print-dashboard #input").val(JSON.stringify({ html: $("body").html() }));
                //$("#form-print-dashboard #input").val(JSON.stringify($("body").html()));

                var isRosterSearchValueEntered = false;
                var inputRosterSearch = $("#roster-top-buttons-wrapper span.k-filtercell input.k-input");
                if (inputRosterSearch.val()) {
                    isRosterSearchValueEntered = true;
                    inputRosterSearch.attr("value", inputRosterSearch.val());
                    inputRosterSearch.addClass("value-entered");
                }
                $("#roster-table-wrapper input.k-textbox").attr("value", $("#roster-table-wrapper input.k-textbox").val());
                $("#form-print-dashboard #input").val($(".body-content").html().replace(/href=/g, "data-href=")); //prevent hyperlinks in pdf
                var newPopupTarget = "print_preview_popup" + DmUiLibrary.RandomStringGenerator(5);
                $("#form-print-dashboard").attr("target", newPopupTarget);
                //window.open("", newPopupTarget, "width=1000,height=600");
                window.open($("#form-print-dashboard").attr("action"), newPopupTarget, "width=1340,height=700,left=0,top=5");
                $("#form-print-dashboard").submit();
                if (isRosterSearchValueEntered) {
                    inputRosterSearch.removeAttr("value");
                    inputRosterSearch.removeClass("value-entered");
                }
            }
            function updateAbilityProfileDropdown(isMultiSelect) {
                console.log('called-differential-report');
                var dataSource = $("#right-card-table2-wrapper").data("kendoGrid").dataSource;
                var filters = dataSource.filter();
                var allData = dataSource.data();
                var query = new kendo.data.Query(allData);
                var filteredDataAllPages = query.filter(filters).data;

                $("#print-ability-profile").empty();
                filteredDataAllPages.map(function (x) {
                    if (x.ability_profile !== "") {
                        $("#print-ability-profile").append('<option value="' + x.ability_profile + '">' + x.ability_profile + "</option>");
                    }
                });

                if (!isMultiSelect) {
                    $("#print-ability-profile option:first-child").prop("selected", true);
                }
                $("#print-ability-profile").trigger("DmUi:updated");

                if (isMultiSelect) {
                    $("#testTreeMultiSelectDropdownList").empty();
                    $("#testTreeMultiSelectDropdownList").trigger("DmUi:updated");
                } else {
                    $("#print-ability-profile").trigger("change");
                }
            }

            function validateInputFileName() {
                if ($("select#print-report-type").val() === "ListOfStudentReport" || $("select#print-report-type").val() === "StudentProfileNarrative" || $("select#print-report-type").val() === "CatalogExporter") {
                    if ($("#background-repot-pdf-file-name").val().trim() == "") {
                        $("#apply-dashboard-print-students").prop("disabled", true);
                    } else {
                        $("#apply-dashboard-print-students").prop("disabled", false);
                    }
                }
            }

            $(document).on("keyup", "#background-repot-pdf-file-name", function (e) {
                validateInputFileName();
            });

            function validateSortDirection() {
                if ($("select#print-report-type").val() === "ListOfStudentReport") {
                    if ($("#print-sort-direction").val() === "LastName") {
                        $(".print-sort-by-subtest").hide();
                    } else {
                        $(".print-sort-by-subtest").show();
                    }
                }
            }

            $(document).on("change", "select#print-sort-direction", function (e) {
                validateSortDirection();
            });

            $(document).on("change", "select#print-report-type", function (e) {
                $("#apply-dashboard-print-students").prop("disabled", false);
                $("#cancel-dashboard-print-students").removeClass("last-tab-element wcag-modal-last-element");
                $("#apply-dashboard-print-students").addClass("last-tab-element wcag-modal-last-element");

                $("#testTreeMultiSelectDropdownList").empty();
                $("#testTreeMultiSelectDropdownList").trigger("DmUi:updated");
                $(".print-grade").hide();
                $(".print-display-options_ListOfStudentReport").hide();
                $(".print-display-options_StudentProfileNarrative").hide();
                $(".print-exporter-template").hide();
                $(".print-export-format").hide();
                
                $(".print-ability-profile").hide();
                $(".print-score-type").hide();
                $(".print-composite-types").hide();
                $(".print-sort-direction").hide();
                $(".print-sort-by-subtest").hide();
                $(".print-sort-type").hide();
                $(".print-ability-profile_StudentProfileNarrative").hide();
                $(".print-home-reporting").hide();
                $(".print-report-grouping").hide();
                $(".print-background-report-pdf-name").hide();
                //$("#print-score-type_dm_ui").addClass("dm-ui-disabled");
                //$("#print-composite-types_dm_ui").addClass("dm-ui-disabled");
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").addClass("dm-ui-disabled"); //'Select All' checkbox disabled
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox input").attr("disabled", "disabledb"); //'Select All' checkbox disabled
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").attr("tabindex", -1); //'Select All' checkbox disabled
                
                //$(".students-wrapper .dm-ui-checkbox-container > .dm-ui-checkbox").addClass("dm-ui-disabled");
                validateInputFileName();
                if ($(this).val() === "ListOfStudentReport") {
                    console.log('this', this);
                    var optionValue;
                    var optionText;
                    $("#apply-dashboard-print-students").text("Generate PDF");
                    //REPORT NAME input
                    $("input#background-repot-pdf-file-name").val("List of Student Scores Report");

                    //GRADE dropdown
                    $("select#print-grade").html($(".dashboard-filter select#Grade_Control").html());
                    $("select#print-grade").empty();
                    var option = '';
                    for (var i = 0; i < private.printReportGrades.length; i++) {
                        const gradeLevel = private.printReportGrades[i].Text.split('/')[0].split(' ').reverse()[0];
                        //const value = gradeLevel === 'K' ? null : Number(gradeLevel);
                        const value = gradeLevel === 'K' ? 0 : Number(gradeLevel);
                        option = option.concat('<option selected value="' + value + '">' + private.printReportGrades[i].Text.split('/')[0] + '</option>');
                    }
                    $("select#print-grade").append(option);
                    //$("select#print-grade option").each(function (val, index) {
                    //    console.log('index', index);
                    //    //$(this).removeAttr("data-alt-value");
                    //    optionText = $(this).text();
                    //    console.log('optionText', optionText);
                    //    console.log('optionValue', optionValue);

                    //    optionText = optionText.substring(0, optionText.indexOf("/")).trim();
                    //    optionValue = optionText.replace("Grade ", "")
                    //    $(this).text(optionText);
                    //    $(this).val(optionValue);
                    //});
                    $("select#print-grade").trigger("DmUi:updated");

                    //SCORE(S) dropdown
                    $("select#print-score-type").html($(".roster-table-card select#Score_Control").html());
                    $("select#print-score-type option").each(function () {
                        if ($(this).val() === "RS" || $(this).val() === "NANI") {
                            $(this).remove();
                        }
                        if ($(this).val() === "LPR" || $(this).val() === "LS") {
                            $(this).prop("selected", false);
                        } else {
                            $(this).prop("selected", true);
                        }
                    });
                    $("select#print-score-type").trigger("DmUi:updated");
                    $("#Label_print-score-type").text("SCORE(S)");

                    //COMPOSITE TYPE(S) dropdown
                    $("select#print-composite-types").html($(".dashboard-filter select#Content_Control").html());
                    $("select#print-composite-types option").each(function (i) {
                        if ($(this).val().indexOf("Verbal") !== -1 || $(this).val().indexOf("Quant") !== -1 || $(this).val().indexOf("NonVerb") !== -1) {
                            $(this).remove();
                        }
                        if ($(this).val().indexOf("CompVQN") !== -1) {
                            $(this).text("Composite (VQN) / Total Score");
                        }
                    });
                    $("select#print-composite-types option").each(function (i) {
                        if (i < $("select#print-composite-types option").length - 1) $(this).prop("selected", false);
                    });
                    $("select#print-composite-types").trigger("DmUi:updated");
                    $("#Label_print-composite-types").text("COMPOSITE TYPE(S)");

                    $(".print-grade").show();
                    $(".print-display-options_ListOfStudentReport").show();
                    $(".print-score-type").show();
                    $(".print-composite-types").show();
                    $(".print-sort-direction").show();
                    //if ($("#print-sort-direction").val() !== "LastName") $(".print-sort-by-subtest").show();
                    validateSortDirection();
                    $(".print-sort-type").show();
                    $(".print-background-report-pdf-name").show();
                    createDistrictOrClassLocationsTreeListStructure();
                    $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
                } else if ($(this).val() === "StudentProfileNarrative") {
                    var optionValue;
                    var optionText;
                    $("#apply-dashboard-print-students").text("Generate PDF");
                    //REPORT NAME input
                    $("input#background-repot-pdf-file-name").val("Student Profile Narrative Report");

                    //GRADE dropdown
                    $("select#print-grade").html($(".dashboard-filter select#Grade_Control").html());
                    $("select#print-grade").empty();
                    //console.log('kjk', $("select#print-grade option"));
                    var option = '';
                    for (var i = 0; i < private.printReportGrades.length; i++) {
                        const gradeLevel = private.printReportGrades[i].Text.split('/')[0].split(' ').reverse()[0];
                        //const value = gradeLevel === 'K' ? null : Number(gradeLevel);
                        const value = gradeLevel === 'K' ? 0 : Number(gradeLevel);
                        option = option.concat('<option selected value="' + value + '">' + private.printReportGrades[i].Text.split('/')[0] + '</option>');
                    }
                    $("select#print-grade").append(option);
                    //$("select#print-grade option").each(function () {
                    //    //$(this).removeAttr("data-alt-value");
                    //    optionText = $(this).text();
                    //    optionText = optionText.substring(0, optionText.indexOf("/")).trim();
                    //    optionValue = optionText.replace("Grade ", "")
                    //    $(this).text(optionText);
                    //    $(this).val(optionValue);
                    //});
                    $("select#print-grade").trigger("DmUi:updated");

                    //SCORE(S) dropdown
                    $("select#print-score-type").html($(".roster-table-card select#Score_Control").html());
                    $("select#print-score-type option").each(function () {
                        if ($(this).val() === "RS" || $(this).val() === "NANI" || $(this).val() === "USS") {
                            $(this).remove();
                        }
                        if ($(this).val() === "LPR" || $(this).val() === "LS") {
                            $(this).prop("selected", false);
                        } else {
                            $(this).prop("selected", true);
                        }
                    });
                    $("select#print-score-type").trigger("DmUi:updated");
                    $("#Label_print-score-type").text("SCORE(S)");

                    $(".print-grade").show();
                    $(".print-display-options_StudentProfileNarrative").show();
                    $(".print-ability-profile_StudentProfileNarrative").show();
                    $(".print-home-reporting").show();
                    $(".print-report-grouping").show();
                    $(".print-score-type").show();
                    //$(".print-composite-types").show();
                    $(".print-background-report-pdf-name").show();
                    createDistrictOrClassLocationsTreeListStructure();
                    $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
                } else if ($(this).val() === "CatalogExporter") {
                    var optionValue;
                    var optionText;

                    //REPORT NAME input
                    $("input#background-repot-pdf-file-name").val("Data Exporter");
                    $("#apply-dashboard-print-students").text("Generate");
                    //GRADE dropdown
                    $("select#print-grade").html($(".dashboard-filter select#Grade_Control").html());
                    $("select#print-grade").empty();
                    //console.log('kjk', $("select#print-grade option"));
                    var option = '';
                    for (var i = 0; i < private.printReportGrades.length; i++) {
                        const gradeLevel = private.printReportGrades[i].Text.split('/')[0].split(' ').reverse()[0];
                        //const value = gradeLevel === 'K' ? null : Number(gradeLevel);
                        const value = gradeLevel === 'K' ? 0 : Number(gradeLevel);
                        option = option.concat('<option selected value="' + value + '">' + private.printReportGrades[i].Text.split('/')[0] + '</option>');
                    }
                    $("select#print-grade").append(option);
                    //$("select#print-grade option").each(function () {
                    //    //$(this).removeAttr("data-alt-value");
                    //    optionText = $(this).text();
                    //    optionText = optionText.substring(0, optionText.indexOf("/")).trim();
                    //    optionValue = optionText.replace("Grade ", "")
                    //    $(this).text(optionText);
                    //    $(this).val(optionValue);
                    //});
                    $("select#print-grade").trigger("DmUi:updated");

                    $(".print-grade").show();
                    $(".print-exporter-template").show();
                    $(".print-export-format").show();
                    //$(".print-composite-types").show();
                    $(".print-background-report-pdf-name").show();
                    createDistrictOrClassLocationsTreeListStructure();
                    //$("#students-treeview-wrapper-background").show();
                    $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
                }else if ($(this).val() === "StudentProfile") {
                    $(".print-score-type").show();
                    $(".print-content-type").show();
                    //$("#print-score-type_dm_ui").removeClass("dm-ui-disabled");
                    //$("#print-content-type_dm_ui").removeClass("dm-ui-disabled");
                    //$("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").removeClass("dm-ui-disabled"); //'Select All' checkbox enabled
                    createStudentsTreeListStructure();
                    $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
                } else if ($(this).val() === "DifferentiatedInstructionReport") {
                    console.log('select-differential-report')
                    $(".print-ability-profile").show();
                    updateAbilityProfileDropdown(true);
                    $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
                    //$(".students-wrapper .dm-ui-checkbox-container > .dm-ui-checkbox").removeClass("dm-ui-disabled");
                }
            });

            $(document).on("change", "select#print-ability-profile", function (e) {
                //appendToPopupAllRosterStudents($(this).val());
                //appendToPopupAllRosterStudents(Dashboard.CorrectAbilityProfileFilerValue($(this).val()));

                var selectedAbilityProfiles = [];
                $("#print-ability-profile option").each(function () {
                    if ($(this).prop("selected")) {
                        selectedAbilityProfiles.push($(this).val());
                    }
                });
                createStudentsTreeListStructure(selectedAbilityProfiles);

                //Dashboard.PrintPopupEqualizeTabHeight();
            });

            function monthDiff(d1, d2) {
                var months, result;
                /*
                                months = (d2.getFullYear() - d1.getFullYear()) * 12;
                                months -= d1.getMonth() + 1;
                                months += d2.getMonth();
                */
                months = d2.getMonth() - d1.getMonth() + (12 * (d2.getFullYear() - d1.getFullYear()));
                //console.log(months);

                //return months <= 0 ? 0 : months;
                months = months <= 0 ? 0 : months;
                //console.log(months);
                var years = Math.floor(months / 12);
                months = Math.floor(months % 12);
                result = years + " years";
                if (months === 1) {
                    result += ", " + months + " month";
                } else if (months > 1) {
                    result += ", " + months + " months";
                }
                return result;
            }

            function ageText(age) {
                var result = "";
                age = "" + age;
                var years = Number(age.substr(0, 2));
                var months = Number(age.substr(2, 2));
                if (years === 1) {
                    result += years + " year";
                } else if (years > 1) {
                    result += years + " years";
                }
                if (years > 0) {
                    result += ", ";
                }
                if (months === 1) {
                    result += months + " month";
                } else {
                    result += months + " months";
                }
                return result;
            }

            //***** Student Info Popup *****
            $(document).on("click touchstart", ".icon-student-info, .student-info", function (e) {
                e.preventDefault();
                $("#student-info-name").text($(this).text());
                if ($(this).data("age")) {
                    //$("#student-info-age").text(monthDiff(new Date($(this).data("dob")), new Date()));
                    $("#student-info-age").html(ageText($(this).data("age")));
                } else {
                    $("#student-info-age").html("&nbsp;");
                }
                if ($(this).data("dob")) {
                    $("#student-info-dob").html($(this).data("dob").split(" ")[0]);
                } else {
                    $("#student-info-dob").html("&nbsp;");
                }
                $("#student-info-class").text($(this).data("class").replace("|", ", "));
                //$("#student-info-school").text($(this).data("school").replace("|", ", "));
                $("#student-info-school").text(Dashboard.FirstLocationOnly($(this).data("school")));
                //$("#student-info-ability-profile").text($(this).data("ability-profile"));
                $("#student-info-ability-profile").text(Dashboard.CorrectReverseAbilityProfileFilerValue($(this).data("ability-profile")));
                //$("#student-info-ability-profile").text(findAbilityGroup($(this).data("ability-profile")));

                $("#student-info-ability-profile").attr("href", Dashboard.GenerateAbilityProfileLink($(this).data("ability-profile")));
                $("#modal-dashboard-student-info").fadeIn("fast");

                if ($(this).data("ability-profile") !== "") {
                    $("#student-info-ability-profile").removeAttr("tabindex");
                    $("#student-info-ability-profile").addClass("last-tab-element wcag-modal-last-element");
                    $("#modal-dashboard-student-info .close_icon").removeClass("last-tab-element wcag-modal-last-element");
                } else {
                    $("#student-info-ability-profile").attr("tabindex", "-1");
                    $("#student-info-ability-profile").removeClass("last-tab-element wcag-modal-last-element");
                    $("#modal-dashboard-student-info .close_icon").addClass("last-tab-element wcag-modal-last-element");
                }
                /*
                                if (e.which === 1 && e.originalEvent.screenX === 0 && e.originalEvent.screenY === 0) {
                                    Dashboard.FocusElementWithDelay($("#modal-dashboard-student-info .first-tab-element"), 1);
                                } else {
                                    $("#modal-dashboard-student-info .first-tab-element").focus();
                                    $("#modal-dashboard-student-info .first-tab-element").blur();
                                }
                */
                if (private.isTabNavigationOn) {
                    private.LastFocusedElement = $(this).parent();
                } else {
                    private.LastFocusedElement = $(this);
                }
                setTimeout(function () {
                    $("#modal-dashboard-student-info .first-tab-element").focus();
                }, 300);
            });
            $(document).on("click touchstart", "#modal-dashboard-student-info .close_icon", function () {
                $("#modal-dashboard-student-info").fadeOut("fast");
                Dashboard.FocusLastFocusedElement();
            });
            $(document).on("keyup", "#modal-dashboard-student-info .close_icon", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    $(this).click();
                }
            });
            $(document).on("keyup", "#modal-dashboard-student-info", function (e) {
                if (e.keyCode === 27) {
                    $("#modal-dashboard-student-info").fadeOut("fast", function () { });
                    Dashboard.FocusLastFocusedElement();
                }
            });
            $(document).on("click touchstart", "#student-info-ability-profile", function (e) {
                /*
                                e.preventDefault();
                                $("#modal-dashboard-student-info").fadeOut("fast");
                                $("#right-card-table2-wrapper td:first-child a:contains('" + $(this).text() + "')").parent().next().find("a").click();
                */
            });

            $(document).on("keydown", "#student-info-ability-profile", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    window.open($(this).attr("href"));
                }
            });

            //***** Print Students *****
            function showModalPrint(e) {
                //Dashboard.GetBackgroundLocation();
                //appendToPopupAllRosterStudents();
                //createStudentsTreeListStructure();
                $("select#print-report-type").trigger("change");
                $("#modal-dashboard-print-students").fadeIn("fast");
                if (e.which === 1 && e.originalEvent.screenX === 0 && e.originalEvent.screenY === 0) {
                    Dashboard.FocusElementWithDelay($("#modal-dashboard-print-students .first-tab-element"), 1);
                } else {
                    $("#modal-dashboard-print-students .first-tab-element").focus();
                    $("#modal-dashboard-print-students .first-tab-element").blur();
                }
            }

            $(document).on("click touchstart", "#print-student-profile", function (e) {
                showModalPrint(e);
            });
            $(document).on("keyup", "#print-student-profile", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    appendToPopupAllRosterStudents();
                    $("#modal-dashboard-print-students").fadeIn("fast", function () { });
                    private.LastFocusedElement = $(this);
                    Dashboard.FocusElementWithDelay($("#modal-dashboard-print-students .first-tab-element"), 1);
                }
            });
            $(document).on("click touchstart", "#modal-dashboard-print-students .close_icon, #cancel-dashboard-print-students", function () {
                $("#modal-dashboard-print-students").fadeOut("fast", function () { });
                $("#print-reports-button").focus();
            });

            $(document).on("keyup", "body, .root-tab-element", function (e) {
                if (e.keyCode === 27) {
                    if ($("#modal-dashboard-report-criteria").is(":visible")) {
                        if ($.inArray("dm-ui-li-item", e.target.classList) !== -1) {
                            $(e.target).parents(".dm-ui-dropdown-content").prev().focus();
                        } else {
                            cancelDashboardReportButtonClick();
                        }
                    }
                    if ($("#modal-dashboard-score-warning-report-criteria").is(":visible")) {
                        if ($.inArray("dm-ui-li-item", e.target.classList) !== -1) {
                            $(e.target).parents(".dm-ui-dropdown-content").prev().focus();
                        } else {
                            cancelDashboardWarningFiltersButtonClick();
                        }
                    }
                    if ($("#modal-dashboard-print-students").is(":visible")) {
                        if (($.inArray("dm-ui-li-item", e.target.classList) !== -1) || ($.inArray("dm-ui-select-all", e.target.classList) !== -1) || ($.inArray("dm-ui-select-none", e.target.classList) !== -1) || ($.inArray("dm-ui-menuitem-checkbox", e.target.classList) !== -1) || ($.inArray("dm-ui-cancel-button", e.target.classList) !== -1) || ($.inArray("dm-ui-apply-button", e.target.classList) !== -1)) {
                            $(e.target).parents(".dm-ui-dropdown-content").prev().focus();
                        } else {
                            $("#cancel-dashboard-print-students").click();
                        }
                    }
                    if ($("#modal-dashboard-student-info").is(":visible")) {
                        $("#modal-dashboard-student-info").fadeOut("fast");
                    }
                }
            });
            $(document).on("change", "#check-all-students-modal", function () {
                let optionsCount = $("#students-treeview-wrapper #testTreeMultiSelectDropdownList > option").length;
                const maxOptionsCountWithoutFreezes = 100;
                if ($(this).prop("checked")) {
                    if (optionsCount < maxOptionsCountWithoutFreezes) {
                        $("#students-treeview-wrapper .dm-ui-select-all-group").click();
                    } else {
                        $("#students-treeview-wrapper #testTreeMultiSelectDropdownList option:not(:selected)").each(function () {
                            $(this).attr('selected', true);
                        });
                        $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
                        $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("DmUi:updated");
                    }
                    /*
                                        $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                                            if (!$(this).prop("checked")) {
                                                $(this).parents(".dm-ui-checkbox").attr("aria-checked", "true");
                                                $(this).prop("checked", true);
                                            }
                                        });
                    */
                } else {
                    if (optionsCount < maxOptionsCountWithoutFreezes) {
                        $("#students-treeview-wrapper .dm-ui-select-none-group").click();
                    } else {
                        $("#students-treeview-wrapper #testTreeMultiSelectDropdownList option:selected").each(function () {
                            $(this).attr('selected', false);
                        });
                        $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
                        $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("DmUi:updated");
                    }
                    /*
                                        $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                                            if ($(this).prop("checked")) {
                                                $(this).parents(".dm-ui-checkbox").attr("aria-checked", "false");
                                                $(this).prop("checked", false);
                                            }
                                        });
                    */
                }
            });
            $(document).on("change", "#students-treeview-wrapper #testTreeMultiSelectDropdownList", function () {
                if ($(this).find("option:not(:selected)").length) {
                    $("#check-all-students-modal").parents(".dm-ui-checkbox").attr("aria-checked", "false");
                    $("#check-all-students-modal").prop("checked", false);
                } else {
                    $("#check-all-students-modal").parents(".dm-ui-checkbox").attr("aria-checked", "true");
                    $("#check-all-students-modal").prop("checked", true);
                }

                if ($("select#print-report-type").val() === "DifferentiatedInstructionReport" || $("select#print-report-type").val() === "StudentProfile" || $("select#print-report-type").val() === "ListOfStudentReport" || $("select#print-report-type").val() === "StudentProfileNarrative" || $("select#print-report-type").val() === "CatalogExporter") {
                    if ($(this).find("option:selected").length) {
                        $("#apply-dashboard-print-students").prop("disabled", false);
                        $("#cancel-dashboard-print-students").removeClass("last-tab-element wcag-modal-last-element");
                        $("#apply-dashboard-print-students").addClass("last-tab-element wcag-modal-last-element");
                    } else {
                        $("#apply-dashboard-print-students").prop("disabled", true);
                        $("#cancel-dashboard-print-students").addClass("last-tab-element wcag-modal-last-element");
                        $("#apply-dashboard-print-students").removeClass("last-tab-element wcag-modal-last-element");
                    }
                }
            });
            $(document).on("change", "#modal-dashboard-print-students .students-wrapper input[type=checkbox]", function () {
                var isAllStudentsSelected = true;
                $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                    if (!$(this).prop("checked")) {
                        isAllStudentsSelected = false;
                        return false;
                    }
                });
                if (isAllStudentsSelected) {
                    $("#check-all-students-modal").parents(".dm-ui-checkbox").attr("aria-checked", "true");
                    $("#check-all-students-modal").prop("checked", true);
                } else {
                    $("#check-all-students-modal").parents(".dm-ui-checkbox").attr("aria-checked", "false");
                    $("#check-all-students-modal").prop("checked", false);
                }
            });
            function compareNames(obj1, obj2) {
                if (typeof obj1.node_name !== "undefined" && typeof obj2.node_name !== "undefined") {
                    var nameA = obj1.node_name.toUpperCase();
                    var nameB = obj2.node_name.toUpperCase();
                    if (nameA < nameB) {
                        return -1;
                    }
                    if (nameA > nameB) {
                        return 1;
                    }
                    return 0;
                }
                return 0;
            }

            function classLocationTreeListStructure() {
                var arrSelectedGrades = [];
                $("#print-grade option").each(function () {
                    if ($(this).prop("selected")) arrSelectedGrades.push(Number($(this).val()));
                });
                //console.log(arrSelectedGrades);

                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").removeClass("dm-ui-disabled"); //'Select All' checkbox enabled
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox input").removeAttr("disabled"); //'Select All' checkbox enabled
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").attr("tabindex", 0); //'Select All' checkbox enabled
                var arrTreeSelectOptions = [];
                //console.log('1', private.printReportBuilding);
                //console.log('2', private.printReportClasses);
                //console.log('3', private.printReportStudents);

                for (var j = 0; j < private.printReportBuilding.length; ++j) {
                    for (var i = 0; i < private.printReportClasses.length; ++i) {
                        if (private.printReportBuilding[j].Value === private.printReportClasses[i].ParentLocationId) {
                            console.log('called1');
                            for (var k = 0; k < private.printReportStudents.length; ++k) {
                                //if (Number(private.printReportClasses[i].Value) === private.printReportStudents[k].ClassIds) {
                                if (Number(private.printReportClasses[i].Value) === private.printReportStudents[k].ClassIds && arrSelectedGrades.indexOf(Number(private.printReportStudents[k].GradeId)) !== -1) {
                                    console.log('called2');
                                    //const option = '<option selected data-district-id="' + private.printReportBuilding[j].DistrictIds + '" data-building-id="' + private.printReportClasses[i].ParentLocationId + '" data-class-id="' + private.printReportClasses[i].Value + '" data-student-id="' + private.printReportStudents[k].Value + '" data-group="' + private.printReportBuilding[j].Text + ' | ' + private.printReportClasses[i].Text + '" value="' + private.printReportStudents[k].Text + '">' + private.printReportStudents[k].Text + "</option>";
                                    const option = '<option selected value="' + private.printReportStudents[k].Text + '" data-district-id="' + private.printReportBuilding[j].DistrictIds + '" data-building-id="' + private.printReportClasses[i].ParentLocationId + '" data-class-id="' + private.printReportClasses[i].Value + '" data-student-id="' + private.printReportStudents[k].Value + '" data-group="' + private.printReportBuilding[j].Text + ' | ' + private.printReportClasses[i].Text + '">' + private.printReportStudents[k].Text + "</option>";
                                    arrTreeSelectOptions.push(option);
                                }
                            }
                        }
                    }
                }
                console.log('arrTreeSelectOptions', arrTreeSelectOptions);
                arrTreeSelectOptions.sort();
                $("#testTreeMultiSelectDropdownList").empty();
                $("#testTreeMultiSelectDropdownList").append(arrTreeSelectOptions.join(""));
                $("#testTreeMultiSelectDropdownList").trigger("DmUi:updated");
                $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
            }

            function districtLocationTreeListStructure() {
                var arrSelectedGrades = [];
                $("#print-grade option").each(function () {
                    if ($(this).prop("selected")) arrSelectedGrades.push($(this).val());
                });
                //console.log(arrSelectedGrades);

                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").removeClass("dm-ui-disabled"); //'Select All' checkbox enabled
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox input").removeAttr("disabled"); //'Select All' checkbox enabled
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").attr("tabindex", 0); //'Select All' checkbox enabled
                var arrTreeSelectOptions = [];
                //for (var i = 0; i < private.printReportBuilding.length; ++i) {
                //    const option = '<option data-district-id="' + private.printReportBuilding[i].DistrictIds + '" data-building-id="' + private.printReportBuilding[i].ParentLocationId + '" data-class-id="' + [1, 2] + '" data-group="' + private.printReportBuilding[i].ParentLocationName + '" value="' + [1, 2] + '">' + [1, 2] + "</option>";
                //    arrTreeSelectOptions.push(option);
                //    arrTreeSelectOptions.push(option);
                //    //const option = '<option data-district-id="' + private.printReportBuilding[y].DistrictIds + '" data-building-id="' + private.printReportClasses[i].ParentLocationId + '" data-class-id="' + private.printReportClasses[i].Value + '" data-group="' + private.printReportClasses[i].ParentLocationName + '" value="' + private.printReportClasses[i].Value + '">' + private.printReportClasses[i].Text + "</option>";
                //}
                //console.log('1',private.printReportBuilding);
                //console.log('2',private.printReportClasses);
                //console.log('3',private.printReportStudents);
                //console.log(private.printReportBuilding);
                //console.log(private.printReportClasses);
                for (var j = 0; j < private.printReportBuilding.length; ++j) {
                    for (var i = 0; i < private.printReportClasses.length; ++i) {
                        //if (private.printReportBuilding[j].Value === private.printReportClasses[i].ParentLocationId) {
                        //if (private.printReportBuilding[j].Value === private.printReportClasses[i].ParentLocationId && arrSelectedGrades.indexOf(Number(private.printReportClasses[i].GradeId)) !== -1) {
                        if (private.printReportBuilding[j].Value === private.printReportClasses[i].ParentLocationId && arrSelectedGrades.some(r => private.printReportClasses[i].GradeId.split(",").includes(r))) {
                            //const option = '<option selected data-district-id="' + private.printReportBuilding[j].DistrictIds + '" data-building-id="' + private.printReportClasses[i].ParentLocationId + '" data-class-id="' + private.printReportClasses[i].Value + '" data-group="' + private.printReportClasses[i].ParentLocationName + '" value="' + private.printReportClasses[i].Text + '">' + private.printReportClasses[i].Text + "</option>";
                            const option = '<option selected data-group="' + private.printReportClasses[i].ParentLocationName + '" value="' + private.printReportClasses[i].Text + '" data-district-id="' + private.printReportBuilding[j].DistrictIds + '" data-building-id="' + private.printReportClasses[i].ParentLocationId + '" data-class-id="' + private.printReportClasses[i].Value + '">' + private.printReportClasses[i].Text + "</option>";
                            arrTreeSelectOptions.push(option);
                        }
                    }
                }
                //for (var k = 0; k < private.printReportStudents; ++k) {
                //    if (private.printReportClasses[i].Value === private.printReportStudents[k].ClassIds) {
                //        const option = '<option selected data-building-id="' + private.printReportClasses[i].ParentLocationId + '" data-class-id="' + private.printReportStudents[k].ClassIds + '" data-student-id="' + private.printReportStudents[k].Value + '" data-group="' + private.printReportClasses[i].Text + '" value="' + private.printReportStudents[k].Text + '">' + private.printReportStudents[k].Text + "</option>";
                //        arrTreeSelectOptions.push(option);
                //    }
                //}
                arrTreeSelectOptions.sort();
                //console.log(arrTreeSelectOptions);

                $("#testTreeMultiSelectDropdownList").empty();
                $("#testTreeMultiSelectDropdownList").append(arrTreeSelectOptions.join(""));
                $("#testTreeMultiSelectDropdownList").trigger("DmUi:updated");
                $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
            }

            function createStudentsTreeListStructure(arrSelectedAbilityProfileGroup) {
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").removeClass("dm-ui-disabled"); //'Select All' checkbox enabled
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox input").removeAttr("disabled"); //'Select All' checkbox enabled
                $("#print-report-center .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").attr("tabindex", 0); //'Select All' checkbox enabled
                var buildingName = "";
                var className = "";
                var districtId = "";
                var buildingId = "";
                var classId = "";
                var option = "";

                var isAbilityProfileNeeded = false;
                if (typeof arrSelectedAbilityProfileGroup !== "undefined" && arrSelectedAbilityProfileGroup !== null) {
                    isAbilityProfileNeeded = true;
                }
                var isWithoutStudents = false;
                if ($("select#print-report-type").val() === "ListOfStudentReport" || $("select#print-report-type").val() === "StudentProfileNarrative" || $("select#print-report-type").val() === "CatalogExporter" ) {
                    isWithoutStudents = true;
                }

                $("#modal-dashboard-print-students .students-wrapper").empty();
                var dataSource = $("#roster-table-wrapper").data("kendoGrid").dataSource;
                var filters = Dashboard.UpdateCurrentRosterFiltersRemoveTotal("ability_profile", true);
                //filters = dataSource.filter();
                var allData = dataSource.data();
                var query = new kendo.data.Query(allData);
                var filteredDataAllPages = query.filter(filters).data;
                var arrTreeSelectOptions = [];
                var arrSelectedStudentsId = [];
                filteredDataAllPages.map(function (x) {
                    arrSelectedStudentsId.push(x.node_id);
                });
                //var arrUniqueClasses = [];
                var arrStudentClasses = [];
                var arrStudents = $("#roster-table-wrapper").data("kendoGrid").dataSource.data();
                console.log('arrStudents', arrStudents);
                arrStudents.sort(compareNames);
                console.log('private.LoggedUserLevel', private.LoggedUserLevel);
                arrStudents.map(function (x) {
                    buildingName = Dashboard.FirstLocationOnly(x.building);
                    //className = Dashboard.LocationsWithoutDuplicates(x.class_name);
                    arrStudentClasses = Dashboard.LocationsWithoutDuplicates(x.class_name).split(",")                    
                    districtId = Dashboard.FirstLocationOnly(x.district_id);
                    buildingId = Dashboard.FirstLocationOnly(x.building_id);
                    classId = Dashboard.FirstLocationOnly(x.class_id);
                    
                    for (var i = 0; i < arrStudentClasses.length; i++) {
                        className = arrStudentClasses[i];
                      //if (arrUniqueClasses.indexOf(className) === -1) {
                          //arrUniqueClasses.push(className);

                            if (arrSelectedStudentsId.indexOf(x.node_id) === -1) {
                                if (isAbilityProfileNeeded) {
                                    if (x.ability_profile !== "" && arrSelectedAbilityProfileGroup.indexOf(findAbilityGroup(x.ability_profile)) !== -1) {
                                        if (private.LoggedUserLevel === "CLASS") {
                                            arrTreeSelectOptions.push('<option data-group="' + findAbilityGroup(x.ability_profile) + '" value="' + x.node_name + '">' + x.node_name + "</option>");
                                        } else {
                                            arrTreeSelectOptions.push('<option data-group="' + findAbilityGroup(x.ability_profile) + "|" + className + '" value="' + x.node_name + '">' + x.node_name + "</option>");
                                        }
                                    }
                                } else {
                                    if (isWithoutStudents) {
                                        option = '<option data-district-id="' + districtId + '" data-building-id="' + buildingId + '" data-class-id="' + classId + '" data-group="' + buildingName + '" value="' + className + '">' + className + "</option>";
                                        if (arrTreeSelectOptions.indexOf(option) === -1) {
                                            arrTreeSelectOptions.push(option);
                                        }
                                    } else {
                                        arrTreeSelectOptions.push('<option data-group="' + buildingName + "|" + className + '" value="' + x.node_name + '">' + x.node_name + "</option>");
                                    }
                                }
                            } else {
                                if (isAbilityProfileNeeded) {
                                    if (x.ability_profile !== "" && arrSelectedAbilityProfileGroup.indexOf(findAbilityGroup(x.ability_profile)) !== -1) {
                                        if (private.LoggedUserLevel === "CLASS") {
                                            arrTreeSelectOptions.push('<option data-group="' + findAbilityGroup(x.ability_profile) + '" selected="selected" value="' + x.node_name + '">' + x.node_name + "</option>");
                                        } else {
                                            arrTreeSelectOptions.push('<option data-group="' + findAbilityGroup(x.ability_profile) + "|" + className + '" selected="selected" value="' + x.node_name + '">' + x.node_name + "</option>");
                                        }
                                    }
                                } else {
                                    if (isWithoutStudents) {
                                        option = '<option data-district-id="' + districtId + '" data-building-id="' + buildingId + '" data-class-id="' + classId + '" data-group="' + buildingName + '" selected="selected" value="' + className + '">' + className + "</option>";
                                        if (arrTreeSelectOptions.indexOf(option) === -1) {
                                            arrTreeSelectOptions.push(option);
                                        }
                                    } else {
                                        arrTreeSelectOptions.push('<option data-group="' + buildingName + "|" + className + '" selected="selected" value="' + x.node_name + '">' + x.node_name + "</option>");
                                    }
                                }
                            }
                      //}
                        if (isAbilityProfileNeeded) break;
                    }
                });
                arrTreeSelectOptions.sort();
                console.log('arrTreeSelectOptions.join("")', arrTreeSelectOptions.join(""));
                $("#testTreeMultiSelectDropdownList").empty();
                $("#testTreeMultiSelectDropdownList").append(arrTreeSelectOptions.join(""));
                $("#testTreeMultiSelectDropdownList").trigger("DmUi:updated");
                $("#students-treeview-wrapper #testTreeMultiSelectDropdownList").trigger("change");
            }

            function createDistrictOrClassLocationsTreeListStructure() {
                if (private.LoggedUserLevel === 'DISTRICT' || private.LoggedUserLevel === 'BUILDING') {
                    districtLocationTreeListStructure();
                } else {
                    classLocationTreeListStructure();
                }
            }









            function appendToPopupAllRosterStudents(abilityProfileGroup, isAllRoster) {
                var arrAbilityProfile = [];
                if (typeof isAllRoster === "undefined") {
                    isAllRoster = false;
                }
                if (typeof abilityProfileGroup !== "undefined" && abilityProfileGroup !== null) {
                    arrAbilityProfile = abilityProfileGroup.split(", ");
                }
                $("#modal-dashboard-print-students .students-wrapper").empty();
                var dataSource = $("#roster-table-wrapper").data("kendoGrid").dataSource;
                var filters;
                //filters = dataSource.filter();
                filters = Dashboard.UpdateCurrentRosterFiltersRemoveTotal("ability_profile", true);
                var allData = dataSource.data();
                var query = new kendo.data.Query(allData);
                var filteredDataAllPages = query.filter(filters).data;
                var container = $("#modal-dashboard-print-students .students-wrapper");
                //zzz
                //container.append('<select class="dm-ui-group-multi-select tree" id="testTreeMultiSelectDropdownList" multiple="multiple" name="testTreeMultiSelectDropdownList[]" style="display:none2;">');
                var arrTreeSelectOptions = [];
                var numTableColumns = 4, i = 0, tmp;
                var isAllStudentsSelected = true;
                var arrSelectedStudentsId = [];
                var rowsNumber = 0;
                filteredDataAllPages.map(function (x) {
                    arrSelectedStudentsId.push(x.node_id);
                });
                //tmp = '<table border="0" cellpadding="0" cellspacing="0" role="presentation">';
                var arrStudents = $("#roster-table-wrapper").data("kendoGrid").dataSource.data();
                arrStudents.sort(compareNames);
                arrStudents.map(function (x) {
                    //if ($.inArray(x.node_id, arrSelectedStudentsId) === -1) {
                    if (arrSelectedStudentsId.indexOf(x.node_id) === -1) {
                        if (x.ability_profile !== "") {
                            arrTreeSelectOptions.push('<option data-group="' + findAbilityGroup(x.ability_profile) + "|" + x.building + "|" + x.class_name + '" value="' + x.node_name + '">' + x.node_name + "</option>");
                        }
                    } else {
                        if (x.ability_profile !== "") {
                            arrTreeSelectOptions.push('<option data-group="' + findAbilityGroup(x.ability_profile) + "|" + x.building + "|" + x.class_name + '" selected="selected" value="' + x.node_name + '">' + x.node_name + "</option>");
                        }
                    }
                });
                arrTreeSelectOptions.sort();
                $("#testTreeMultiSelectDropdownList").empty();

                $("#testTreeMultiSelectDropdownList").append(arrTreeSelectOptions.join(""));
                $("#testTreeMultiSelectDropdownList").trigger("DmUi:updated");
            }
            function getCurrentClassInfoParameters(i) {
                var params = "";
                /*
                                var arrLocation = JSON.parse($("#locations-path").val());
                                if (arrLocation.length >= 3) {
                                    params += "&mod[" + i + "][district]=" + arrLocation[arrLocation.length - 3]["Text"];
                                }
                                if (arrLocation.length >= 2) {
                                    params += "&mod[" + i + "][school]=" + arrLocation[arrLocation.length - 2]["Text"];
                                }
                                if (arrLocation.length >= 1) {
                                    params += "&mod[" + i + "][class]=" + arrLocation[arrLocation.length - 1]["Text"];
                                }
                                if ($("#GradeLevel_Control_dm_ui button").length) {
                                    params += "&mod[" + i + "][grade]=" + $("#GradeLevel_Control_dm_ui button").text();
                                }
                                if ($("#test-event-subject-name").length) {
                                    params += "&mod[" + i + "][test_event]=" + $("#test-event-subject-name").text();
                                }
                */
                return params;
            }

            function callBackgroundReportPdfGenerator() {
                
                var data;
                if ($("select#print-report-type").val() === "ListOfStudentReport") {
                    var arrContentId = [1, 2, 3];
                    var arrSelectedGrades = [];
                    var arrSelectedScores = [];
                    var arrSelectedCompositeTypes = [];
                    var arrSelectedDistrictId = [];
                    var arrSelectedBuildingId = [];
                    var arrSelectedClassId = [];
                    var arrSelectedStudentId = [];
                    var reportCriteria = "";
                    var rankingSubtest = "";

                    

                    $("#print-grade option").each(function () {
                        if ($(this).prop("selected")) arrSelectedGrades.push(Number($(this).val()));
                    });
                    //console.log(arrSelectedGrades);

                    $("#print-score-type option").each(function () {
                        if ($(this).prop("selected")) arrSelectedScores.push($(this).val());
                    });
                    //console.log(arrSelectedScores);

                    $("#print-composite-types option").each(function () {
                        if ($(this).prop("selected")) arrSelectedCompositeTypes.push($(this).val().replace("Comp", "").replaceAll("'", ""));
                    });
                    //console.log(arrSelectedCompositeTypes.join(","));

                    $("#testTreeMultiSelectDropdownList option").each(function () {
                        if ($(this).prop("selected")) {
                            arrSelectedDistrictId.push($(this).data("district-id"));
                            arrSelectedBuildingId.push($(this).data("building-id"));
                            arrSelectedClassId.push($(this).data("class-id"));
                            if (private.LoggedUserLevel === "CLASS") {
                                arrSelectedStudentId.push($(this).data("student-id"));
                            }
                        }
                    });
                    //console.log(arrSelectedDistrictId);
                    //console.log(arrSelectedBuildingId);
                    //console.log(arrSelectedClassId);

                    var selectedOptionValue = "";
                    var topFilterAltOptionValue = "";
                    $("#print-composite-types_dm_ui .dm-ui-dropdown-items .dm-ui-menuitem-checkbox input").each(function () {
                        if ($(this).prop("checked")) {
                            selectedOptionValue = $(this).val();
                            topFilterAltOptionValue = "";
                            $("#Content_Control_dm_ui .dm-ui-dropdown-items .dm-ui-menuitem-checkbox input").each(function () {
                                if ($(this).val() === selectedOptionValue) {
                                    topFilterAltOptionValue = $(this).data("alt-value");
                                }
                            });
                            if (topFilterAltOptionValue) arrContentId.push(topFilterAltOptionValue);
                        }
                    });
                    //console.log(arrContentId);

                    reportCriteria = $("#print-sort-direction").val();
                    if (reportCriteria === "LastName") {
                        reportCriteria = "ASC|" + reportCriteria;
                    } else {
                        reportCriteria += "|" + $("#print-sort-by-subtest").val().substring(1);
                        rankingSubtest = $("#print-sort-by-subtest").val().substring(1);
                    }
                    reportCriteria += "|" + $("#print-sort-type").val();
                    //console.log(reportCriteria);

                    ECogatRequest = {
                        FileName: $("#background-repot-pdf-file-name").val().trim(),
                        reportTemplate: "CLSS",
                        suppressProgramLabel: "",
                        cogatComposite: arrSelectedCompositeTypes.join(","),
                        rankingDirection: "LastName",
                        graphType: $("#print-display-options_ListOfStudentReport").val(),
                        rankingSubtest: rankingSubtest,
                        rankingScore: $("#print-sort-type").val(),
                        //buildingLabel: "Building",
                        buildingLabel: private.SuppressedTextBuilding,
                        //classLabel: "Class",
                        classLabel: private.SuppressedTextClass,
                        districtLabel: "District",
                        queryParameters: {
                            districtIds: arrSelectedDistrictId,
                            buildingIds: arrSelectedBuildingId,
                            classIds: arrSelectedClassId,
                            studentIds: arrSelectedStudentId,
                            gradeLevels: arrSelectedGrades,
                            contentIds: arrContentId,
                            scores: arrSelectedScores,
                            //reportCriteria: "ASC|LastName|GS"
                            reportCriteria: reportCriteria
                        }
                    }
                } else if ($("select#print-report-type").val() === "StudentProfileNarrative") {
                    var arrContentId = [];
                    var arrSelectedGrades = [];
                    var arrSelectedScores = [];
                    var arrSelectedDistrictId = [];
                    var arrSelectedBuildingId = [];
                    var arrSelectedClassId = [];
                    var arrSelectedStudentId = [];

                    

                    $("#print-grade option").each(function () {
                        if ($(this).prop("selected")) arrSelectedGrades.push(Number($(this).val()));
                    });
                    //console.log(arrSelectedGrades);

                    $("#print-score-type option").each(function () {
                        if ($(this).prop("selected")) arrSelectedScores.push($(this).val());
                    });
                    //console.log(arrSelectedScores);

                    $("#testTreeMultiSelectDropdownList option").each(function () {
                        if ($(this).prop("selected")) {
                            arrSelectedDistrictId.push($(this).data("district-id"));
                            arrSelectedBuildingId.push($(this).data("building-id"));
                            arrSelectedClassId.push($(this).data("class-id"));
                            if (private.LoggedUserLevel == "CLASS") {
                                arrSelectedStudentId.push($(this).data("student-id"));
                            }
                        }
                    });
                    if (private.LoggedUserLevel !== "CLASS") {
                        for (var k = 0; k < private.printReportStudents.length; ++k) {
                            //if (arrSelectedClassId.some(r => private.printReportStudents[k].ClassIds.split(",").includes(r))) {
                            if (arrSelectedClassId.indexOf(private.printReportStudents[k].ClassIds) !== -1) {
                                arrSelectedStudentId.push(Number(private.printReportStudents[k].Value));
                            }
                        }
                    }
                    //console.log(arrSelectedDistrictId);
                    //console.log(arrSelectedBuildingId);
                    //console.log(arrSelectedClassId);
                    //console.log(arrSelectedStudentId);
                    //console.log('--------------');
                    //console.log(private.printReportClasses);
                    //console.log(private.printReportStudents);

                    var arrAvoidCompositeContent = ["CompVQ", "CompVN", "CompQN", "'CompVQ'", "'CompVN'", "'CompQN'"];
                    $("#Content_Control_dm_ui .dm-ui-dropdown-items .dm-ui-menuitem-checkbox input").each(function () {
                        if ($(this).prop("checked") && arrAvoidCompositeContent.indexOf($(this).val()) === -1) arrContentId.push($(this).data("alt-value"));
                    });
                    //console.log(arrContentId);

                    ECogatRequest = {
                        FileName: $("#background-repot-pdf-file-name").val().trim(),
                        ReportTemplate: "CPN",
                        ReportFormat: "COGAT",
                        RFormat: "Catalog",
                        suppressProgramLabel: "",
                        cogatComposite: null,
                        graphType: $("#print-display-options_StudentProfileNarrative").val(),
                        HomeReporting: $("#print-home-reporting").val(),
                        ReportGrouping: $("#print-report-grouping").val(),
                        DisaggLabel: "",
                        //buildingLabel: "Building",
                        buildingLabel: private.SuppressedTextBuilding,
                        //classLabel: "Class",
                        classLabel: private.SuppressedTextClass,
                        districtLabel: "District",
                        RegionLabel: "SUPPRESS",
                        StateLabel: "SUPPRESS",
                        SystemLabel: "SUPPRESS",
                        QueryParameters: {
                            districtIds: arrSelectedDistrictId,
                            buildingIds: arrSelectedBuildingId,
                            classIds: arrSelectedClassId,
                            studentIds: arrSelectedStudentId,
                            gradeLevels: arrSelectedGrades,
                            contentIds: arrContentId,
                            scores: arrSelectedScores,
                            homeReporting: $("#print-home-reporting").val(),
                            includeAbilityProfile: $("#print-ability-profile_StudentProfileNarrative").val(),
                        }
                    }
                }
                else if ($("select#print-report-type").val() === "CatalogExporter") {
                    var arrContentId = [];
                    var arrSelectedGrades = [];
                    var arrSelectedScores = [];
                    var arrSelectedDistrictId = [];
                    var arrSelectedBuildingId = [];
                    var arrSelectedClassId = [];
                    var arrSelectedStudentId = [];

                    $("#print-grade option").each(function () {
                        if ($(this).prop("selected")) arrSelectedGrades.push(Number($(this).val()));
                    });



                    $("#testTreeMultiSelectDropdownList option").each(function () {
                        if ($(this).prop("selected")) {
                            arrSelectedDistrictId.push($(this).data("district-id"));
                            arrSelectedBuildingId.push($(this).data("building-id"));
                            arrSelectedClassId.push($(this).data("class-id"));
                            if (private.LoggedUserLevel == "CLASS") {
                                arrSelectedStudentId.push($(this).data("student-id"));
                            }
                        }
                    });
                    if (private.LoggedUserLevel !== "CLASS") {
                        for (var k = 0; k < private.printReportStudents.length; ++k) {
                            //if (arrSelectedClassId.some(r => private.printReportStudents[k].ClassIds.split(",").includes(r))) {
                            if (arrSelectedClassId.indexOf(private.printReportStudents[k].ClassIds) !== -1) {
                                arrSelectedStudentId.push(Number(private.printReportStudents[k].Value));
                            }
                        }
                    }
                    ECogatRequest = {
                        FileName: $("#background-repot-pdf-file-name").val().trim(),
                        ReportTemplate: "DE",
                        ReportFormat: "COGAT",
                        RFormat: "Catalog",
                        suppressProgramLabel: "",
                        cogatComposite: null,
                        ExportFormat: $("#print-export-format").val(),
                        ExporterTemplate: $("#print-exporter-template_dm_ui").val(),
                        DisaggLabel: "",
                        //buildingLabel: "Building",
                        buildingLabel: private.SuppressedTextBuilding,
                        //classLabel: "Class",
                        classLabel: private.SuppressedTextClass,
                        districtLabel: "District",
                        RegionLabel: "SUPPRESS",
                        StateLabel: "SUPPRESS",
                        SystemLabel: "SUPPRESS",
                        QueryParameters: {
                            districtIds: arrSelectedDistrictId,
                            buildingIds: arrSelectedBuildingId,
                            classIds: arrSelectedClassId,
                            studentIds: arrSelectedStudentId,
                            gradeLevels: arrSelectedGrades,
                        }
                    }
                }
                console.log(ECogatRequest);

                $.ajax({
                    async: true,
                    type: "POST",
                    url: siteRoot + "/DashboardCogat/SendToBackground",
                    data: JSON.stringify(ECogatRequest),
                    dataType: "html",
                    success: function (ECogatRequest) {
                        console.log("RESPONSE:");
                        console.log(ECogatRequest);
                        $("#tabstrip").show();
                        $("#tabreports").show();
                        $("#gridCompleted").show();
                        DashBoardReportCeneter.GetReportCenter();
                        console.log("reports reloaded");
                        //DashBoardReportCeneter.GetReportCenter();
                    },
                    error: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            };

            $(document).on("click touchstart", "#apply-dashboard-print-students", function (e) {
                e.preventDefault();
                var params = "";
                var tmp, tmpFirstName, tmpLastName;

                if ($("select#print-report-type").val() === "Dashboard") {
                    $("#modal-dashboard-print-students").hide();
                    printDashboard();
                    $("#modal-dashboard-print-students").show();
                } else if ($("select#print-report-type").val() === "ListOfStudentReport" || $("select#print-report-type").val() === "StudentProfileNarrative" || $("select#print-report-type").val() === "CatalogExporter") {

                    callBackgroundReportPdfGenerator();
                   
                    $("#modal-dashboard-print-students").fadeOut("fast", function () { });
                    $("#print-reports-button").focus();
                } else if ($("select#print-report-type").val() === "DifferentiatedInstructionReport") {
                    var i = 0;
                    if ($("#print-ability-profile").hasClass("dm-ui-multi-select")) {
                        //*** MultiSelectAbilityProfileDropdown ***
                        //parameters as json object
                        tmp = $(".dashboard-filter #TestEvent_Control option:selected").text();
                        tmp = tmp.substr(0, tmp.lastIndexOf(" - "));
                        var arrNodes = [];
                        var arrUniqueNodes = [];
                        $("#testTreeMultiSelectDropdownList option:selected").each(function () {
                            arrNodes.push($(this).attr("data-group"));
                        });
                        arrUniqueNodes = $.grep(arrNodes, function (v, k) {
                            return $.inArray(v, arrNodes) === k;
                        });
                        var index = 0, lastUniqueAbilityProfileGroup = "";
                        var mod = [], objStudentName = {};
                        $("#testTreeMultiSelectDropdownList option:selected").each(function () {
                            if (lastUniqueAbilityProfileGroup !== $(this).attr("data-group")) {
                                lastUniqueAbilityProfileGroup = $(this).attr("data-group");
                                if (!$.isEmptyObject(objStudentName)) {
                                    mod.push(objStudentName);
                                }
                                objStudentName = {};
                                index = 0;
                            }
                            objStudentName["" + index] = $(this).val();
                            index++;
                        });
                        if (!$.isEmptyObject(objStudentName)) {
                            mod.push(objStudentName);
                        }
                        var obj = {
                            "Grade": $(".dashboard-filter #Grade_Control option:selected").text(),
                            "TestEvent": tmp,
                            "TestFamilyGroupCode": private.TestFamilyGroupCode,
                            "District": Dashboard.FirstLocationOnly(private.DifferentiatedReportDistrictName),
                            "SchoolName": Dashboard.FirstLocationOnly(private.DifferentiatedReportBuildingName),
                            "SchoolLabel": private.SuppressedTextBuilding,
                            "ClassName": private.DifferentiatedReportClassName,
                            "ClassLabel": private.SuppressedTextClass,
                            "node": arrUniqueNodes,
                            "mod": mod
                        };
                        params = encodeURIComponent(JSON.stringify(obj));

                        /*
                                                //parameters as multi inputs
                                                params += "Grade=" + encodeURIComponent($(".dashboard-filter #Grade_Control option:selected").text());
                                                tmp = $(".dashboard-filter #TestEvent_Control option:selected").text();
                                                tmp = tmp.substr(0, tmp.lastIndexOf(" - "));
                                                params += "&TestEvent=" + encodeURIComponent(tmp);
                                                //params += "&District=" + encodeURIComponent($(".bread-crumbs").text());
                                                params += "&District=" + encodeURIComponent(private.DifferentiatedReportDistrictName);
                                                params += "&SchoolName=" + encodeURIComponent(private.DifferentiatedReportBuildingName);
                                                params += "&ClassName=" + encodeURIComponent(private.DifferentiatedReportClassName);
                        
                                                var arrNodes = [];
                                                var arrUniqueNodes = [];
                                                $("#testTreeMultiSelectDropdownList option:selected").each(function () {
                                                    arrNodes.push($(this).attr("data-group"));
                                                });
                                                arrUniqueNodes = $.grep(arrNodes, function (v, k) {
                                                    return $.inArray(v, arrNodes) === k;
                                                });
                        
                                                arrUniqueNodes.map(function (x, i) {
                                                    params += "&node[" + i + "]=" + x;
                                                });
                        
                                                var index, lastUniqueAbilityProfileGroup = "";
                                                $("#testTreeMultiSelectDropdownList option:selected").each(function () {
                                                    if (lastUniqueAbilityProfileGroup !== $(this).attr("data-group")) {
                                                        lastUniqueAbilityProfileGroup = $(this).attr("data-group");
                                                        i = 0;
                                                    }
                                                    index = arrUniqueNodes.indexOf($(this).attr("data-group"));
                                                    params += "&mod[" + index + "][" + i + "]=" + $(this).val();
                                                    i++;
                                                });
                                                //console.log("params=" + params);
                        */
                    } else {
                        //*** SingleSelectAbilityProfileDropdown ***
                        $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                            if ($(this).prop("checked")) {
                                if (params !== "") {
                                    params += "&";
                                } else {
                                    params += "&Profiles=" + encodeURIComponent($("#print-ability-profile").val());
                                    params += "&Grade=" + encodeURIComponent($(".dashboard-filter #Grade_Control option:selected").text());
                                    tmp = $(".dashboard-filter #TestEvent_Control option:selected").text();
                                    tmp = tmp.substr(0, tmp.lastIndexOf(" - "));
                                    params += "&TestEvent=" + encodeURIComponent(tmp);
                                    params += "&District=" + encodeURIComponent($(".bread-crumbs").text());
                                    params += "&SchoolName=" + encodeURIComponent($(this).data("building"));
                                    params += "&ClassName=" + encodeURIComponent($(this).data("class-name"));
                                    params += "&";
                                }
                                tmp = $(this).next().text();
                                if (tmp.indexOf(",") !== -1) {
                                    tmpLastName = tmp.substr(0, tmp.indexOf(",")).trim();
                                    tmpFirstName = tmp.substr(tmp.indexOf(",") + 1).trim();
                                } else {
                                    tmpFirstName = tmp;
                                    tmpLastName = "";
                                }
                                params += "mod[" + i + "][FirstName]=" + tmpFirstName;
                                params += "&mod[" + i + "][LastName]=" + tmpLastName;
                                i++;
                            }
                        });
                    }
                    if (params !== "") {
                        if (private.convertToPdfImmediately) {
                            postPdfProfileNarrative(params);
                        } else {
                            window.open("Dashboard/StudentProfile?" + params, "_blank").focus();
                        }
                    }
                } else {
                    var grid = $("#roster-table-wrapper").data("kendoGrid");
                    var gridData = grid.dataSource.data();
                    //var arrSelected = grid.selectedKeyNames();
                    var arrSelected = [];
                    $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                        if ($(this).prop("checked")) {
                            arrSelected.push($(this).data("grid-id"));
                        }
                    });
                    var studentIds = "";
                    arrSelected.forEach(function (item, i) {
                        if (params !== "") {
                            params += "&";
                        }
                        tmp = gridData[item].node_name;
                        if (tmp.indexOf(",") !== -1) {
                            tmpLastName = tmp.substr(0, tmp.indexOf(",")).trim();
                            tmpFirstName = tmp.substr(tmp.indexOf(",") + 1).trim();
                        } else {
                            tmpFirstName = tmp;
                            tmpLastName = "";
                        }
                        params += "mod[" + i + "][FirstName]=" + tmpFirstName;
                        params += "&mod[" + i + "][LastName]=" + tmpLastName;
                        params += "&mod[" + i + "][NPR]=" + gridData[item].NPR;
                        params += "&mod[" + i + "][SS]=" + gridData[item].SS;
                        params += "&mod[" + i + "][id]=" + gridData[item].node_id;
                        params += getCurrentClassInfoParameters(i);

                        if (studentIds !== "") {
                            studentIds += ",";
                        }
                        studentIds += gridData[item].node_id;
                        //params = encodeURIComponent(params);
                    });

                    if (params !== "") {
                        params += "&StudentIds=" + studentIds;
                        //window.location.href = "/Dashboard/StudentProfile?" + params;
                        if (private.convertToPdfImmediately) {
                            postPdfProfileNarrative(params);
                        } else {
                            window.open("Dashboard/StudentProfile?" + params, "_blank").focus();
                        }
                    }
                }
            });
            $(document).on("click touchstart", ".student-link", function (e) {
                e.preventDefault();
                var params = "";
                var elements = $(this).parents("tr").find("a");
                var i = 0;
                var tmpFirstName, tmpLastName, tmp;
                elements.each(function () {
                    if (i > 0) {
                        params += "&";
                    }
                    //params += $(this).attr("data-name").split(' ').join('') + "=" + $(this).attr("data-value").toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase() });
                    //params += "mod[0][" + $(this).attr("data-name").split(' ').join('') + "]=" + $(this).attr("data-value").toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase() });
                    tmp = $(this).data("value");
                    if (tmp.indexOf(",") !== -1) {
                        tmpLastName = tmp.substr(0, tmp.indexOf(",")).trim();
                        tmpFirstName = tmp.substr(tmp.indexOf(",") + 1).trim();
                    } else {
                        tmpFirstName = tmp;
                        tmpLastName = "";
                    }
                    params += "mod[" + i + "][FirstName]=" + tmpFirstName;
                    params += "&mod[" + i + "][LastName]=" + tmpLastName;
                    params += "&mod[" + i + "][NPR]=" + $(this).data("npr");
                    params += "&mod[" + i + "][SS]=" + $(this).data("ss");
                    params += "&mod[" + i + "][id]=" + $(this).data("node-id");
                    params += "&StudentIds=" + + $(this).data("node-id");
                    params += getCurrentClassInfoParameters(i);
                    return false;
                    /*
                        i++;
                        if (i >= 2) {
                            return false;
                        }
                    */
                });
                if (private.convertToPdfImmediately) {
                    postPdfProfileNarrative(params);
                } else {
                    //window.location.href = "/Dashboard/StudentProfile?" + params;
                    window.open("Dashboard/StudentProfile?" + params, "_blank").focus();
                }
            });
            function postPdfProfileNarrative(params) {
                //var 2) pdf conversion with POST request
                $("#pdf_post_profile_narrative_parameters").empty();
                //parameters as multi inputs
                /*
                                params = params.split("&");
                                for (var index = 0; index < params.length; index++) {
                                    var valZ = params[index].split("=");
                                    $("#pdf_post_profile_narrative_parameters").append("<input type=\"hidden\" name=\"" + valZ[0] + '" value="' + valZ[1] + '">');
                                }
                */
                //parameters as json object
                $("#pdf_post_profile_narrative_parameters").append('<input type="hidden" name="input" value="' + params + '">');

                var newPopupTarget = "print_preview_popup" + DmUiLibrary.RandomStringGenerator(5);
                $("#print_profile_narrative_form").attr("target", newPopupTarget);
                //window.open("", newPopupTarget, "width=1000,height=600");
                window.open($("#print_profile_narrative_form").attr("action"), newPopupTarget, "width=1340,height=700,left=0,top=5");
                $("#print_profile_narrative_form").submit();
            }



            //***** WCAG tab key navigation behavior *****
            if (private.isTabNavigationOn) {
                $('header.main-header-container [tabindex="0"], header a, .main-menu-container [tabindex="0"], .main-menu-container a').each(function () {
                    $(this).attr("tabindex", "1");
                });
                $('.filters [tabindex="0"], .filters a, .filters button').each(function () {
                    $(this).attr("tabindex", "-1");
                });
                $('footer [tabindex="0"], footer a').each(function () {
                    $(this).attr("tabindex", "5");
                });
            }

            $(document).on("click touchstart", ".root-tab-element", function (e) {
                //e.stopPropagation();
                if ($.inArray("root-tab-element", e.target.classList) === -1) {
                    //if (e.originalEvent !== undefined && e.originalEvent.screenX > 0 && e.originalEvent.screenY > 0) {
                    if (e.originalEvent !== undefined) {
                        if (e.originalEvent.screenX !== 0 && e.originalEvent.screenY !== 0) {
                            if (e.originalEvent.mozInputSource === undefined || e.originalEvent.mozInputSource === 1) {
                                //console.log("rootMakeContentTabbable after .root-tab-element CLICK [mouse]");
                                $(this).rootMakeContentTabbable();
                            } else {
                                //console.log("rootMakeContentTabbable after .root-tab-element CLICK [keyboard FF with started NVDA]");
                                $(this).rootMakeContentTabbable(true);
                            }
                        }
                    }
                } else {
                    e.stopPropagation();
                    //console.log("rootMakeContentTabbable after .root-tab-element CLICK [keyboard]");
                    $(this).rootMakeContentTabbable(true);
                }

                var clickedGridElement;
                if (($(e.target).attr("id") === "roster-table-wrapper_active_cell") || ($(e.target).attr("id") === "stanine-table-wrapper_active_cell") || ($(e.target).attr("id") === "right-card-table-wrapper_active_cell") || ($(e.target).attr("id") === "right-card-table2-wrapper_active_cell")) {
                    clickedGridElement = $(e.target);
                } else {
                    //clickedGridElement = $(e.target).parents("#roster-table-wrapper td, #roster-table-wrapper th, .dashboard-table td, .dashboard-table th");
                    clickedGridElement = $(e.target).parents("#roster-table-wrapper td, #roster-table-wrapper th, #stanine-table-wrapper td, #stanine-table-wrapper th, #right-card-table-wrapper td, #right-card-table-wrapper th, #right-card-table2-wrapper td, #right-card-table2-wrapper th");
                }
                if (clickedGridElement.length) {
                    if (typeof e.originalEvent === "object" || typeof e.originalEvent === "undefined") {
                        //checking is click event from Mouse or from JS method? and focusing only if this is Mouse event
                        clickedGridElement.focus();
                    }
                }
                //Dashboard.ClearRosterGridActiveCells();
            });

            $(document).on("keydown", "#modal-dashboard-print-students .students-wrapper .dm-ui-checkbox", function (e) {
                if (e.keyCode >= 37 && e.keyCode <= 40) {
                    var nextFocusedCheckbox, currentColumnNumber;
                    if (e.keyCode === 37) {
                        nextFocusedCheckbox = $(this).parents("td").prev().find(".dm-ui-checkbox");
                    }
                    if (e.keyCode === 39) {
                        nextFocusedCheckbox = $(this).parents("td").next().find(".dm-ui-checkbox");
                    }
                    if (e.keyCode === 38) {
                        currentColumnNumber = $(this).parents("td").data("col");
                        nextFocusedCheckbox = $(this).parents("tr").prev().find("td:nth-child(" + Number(currentColumnNumber + 1) + ") .dm-ui-checkbox");
                    }
                    if (e.keyCode === 40) {
                        currentColumnNumber = $(this).parents("td").data("col");
                        nextFocusedCheckbox = $(this).parents("tr").next().find("td:nth-child(" + Number(currentColumnNumber + 1) + ") .dm-ui-checkbox");
                    }
                    if (nextFocusedCheckbox !== undefined && nextFocusedCheckbox !== null && nextFocusedCheckbox.length) {
                        nextFocusedCheckbox.focus();
                    }
                }
            });
            $(document).on("change", "#roster-table-wrapper .k-pager-sizes select", function (e) {
                $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
            });
            $(document).on("click keyup", "#roster-table-wrapper .k-pager-nav", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32 || e.which === 1) {
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                }
            });
            $(document).on("click keyup", ".master-of-roots", function (e) {
                if ((e.keyCode === 13 || e.keyCode === 32 || e.which === 1) && ($.inArray("master-of-roots", e.target.classList) !== -1)) {
                    $(this).masterMakeInnerRootsTabbable(true);
                }
                if (e.keyCode === 27) {
                    $(this).masterMakeInnerRootsNotTabbable();
                }
            });
            $(document).on("keydown", ".root-tab-element", function (e) {
                if ((e.keyCode === 13 || e.keyCode === 32) && ($.inArray("root-tab-element", e.target.classList) !== -1)) {
                    e.stopPropagation();
                    e.preventDefault();
                    $(this).rootMakeContentTabbable(true);

                    if ($(this).attr("id") === "right-card1") {
                        if ($("#right-card1").attr("tabindex") === "-1" && $("#right-card-table-wrapper thead tr th:first-child").length) {
                            Dashboard.setGridCellCurrentState($("#right-card-table-wrapper thead tr th:first-child"));
                            $("#right-card-table-wrapper thead tr th:first-child").removeClass("k-state-focused");
                        }
                    }
                    if ($(this).attr("id") === "right-card2") {
                        if ($("#right-card2").attr("tabindex") === "-1" && $("#right-card-table2-wrapper thead tr th:first-child").length) {
                            Dashboard.setGridCellCurrentState($("#right-card-table2-wrapper thead tr th:first-child"));
                            $("#right-card-table2-wrapper thead tr th:first-child").removeClass("k-state-focused");
                        }
                    }

                }
                if (e.keyCode === 27) {
                    //if (!$(this).hasClass("filters")) {
                    if ($(this).hasClass("filters") ||
                        ($(this).hasClass("stanine-card") && $(this).find("#dropdown-graph-content").next().find(".dm-ui-dropdown-content").is(":visible")) ||
                        ($(this).hasClass("roster-table-card") && $(this).find("#Score_Control").next().find(".dm-ui-dropdown-content").is(":visible"))) {
                    } else {
                        if ($(this).attr("tabindex") === "-1") {
                            e.stopPropagation();
                        }
                        $(this).rootMakeContentNotTabbable(true);
                        DmUiLibrary.StartBrowserScrolling();
                    }
                }
            });
            $(document).on("keyup", ".filters.root-tab-element .dm-ui-dropdown-button, .dashboard-filter.button .dm-ui-button-primary", function (e) {
                if (e.keyCode === 27) {
                    $(this).rootMakeContentNotTabbable(true);
                }
            });
            $(document).on("keyup", ".filters.root-tab-element .dm-ui-dropdown-content", function (e) {
                if (e.keyCode === 27) {
                    $(this).prev().focus();
                }
            });
            $(document).on("keydown", ".first-root-tab-element", function (e) {
                if (e.keyCode === 9 && e.shiftKey && ($.inArray("root-tab-element", e.target.classList) !== -1)) {
                    $(this).parents(".master-of-roots").masterMakeInnerRootsNotTabbable();
                }
            });
            $(document).on("keydown", ".last-root-tab-element", function (e) {
                if (e.keyCode === 9 && !e.shiftKey && ($.inArray("root-tab-element", e.target.classList) !== -1)) {
                    $(this).parents(".master-of-roots").masterMakeInnerRootsNotTabbable();
                }
            });
            $(document).on("keydown", ".last-tab-element", function (e) {
                if ((e.keyCode === 9 && !e.shiftKey) || e.keyCode === 40) {
                    if ($(this).hasClass("wcag-modal-last-element")) {
                        e.preventDefault();
                        $(this).parents(".dm-ui-modal-container").find(".wcag-modal-first-element").focus();
                    } else {
                        if ($(this).parents(".root-tab-element").hasClass("filters")) {
                            if (($(this).hasClass("dm-ui-expanded") && e.keyCode === 9) || e.keyCode === 40) {
                            } else {
                                $(this).rootMakeContentNotTabbable(true);
                                DmUiLibrary.StartBrowserScrolling();
                            }
                        } else {
                            $(this).rootMakeContentNotTabbable(true);
                            DmUiLibrary.StartBrowserScrolling();
                        }
                    }
                }
            });
            $(document).on("keydown", ".first-tab-element", function (e) {
                if ((e.keyCode === 9 && e.shiftKey) || e.keyCode === 38) {
                    if ($(this).hasClass("wcag-modal-first-element")) {
                        e.preventDefault();
                        $(this).parents(".dm-ui-modal-container").find(".wcag-modal-last-element").focus();
                    } else {
                        $(this).rootMakeContentNotTabbable(true);
                        DmUiLibrary.StartBrowserScrolling();
                    }
                }
            });
            $(document).on("keydown", ".tab:not(.disabled-element):not([aria-autocomplete=list]):not(.k-pager-sizes .k-dropdown):not(.k-state-disabled)", function (e) {
                if (e.keyCode === 13 && e.target.className !== undefined) {
                    if (e.target.className === "k-textbox tab") {
                        $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                        //$(this).focus();
                        Dashboard.FocusElementWithDelay($(".roster-table-card.root-tab-element .k-pager-input input"), 1);
                    }
                }
                if (e.keyCode === 38 || e.keyCode === 40) {
                    var allElementsInBlock = $(this).parents(".root-tab-element").find(".tab:not(.disabled-element)");
                    var index = allElementsInBlock.index(this);

                    if (e.keyCode === 38) {
                        if (index > 0) {
                            allElementsInBlock[index - 1].focus();
                            DmUiLibrary.StopBrowserScrolling();
                            e.stopPropagation();
                            e.preventDefault();
                        }
                    }
                    if (e.keyCode === 40) {
                        if (index < allElementsInBlock.length - 1) {
                            allElementsInBlock[index + 1].focus();
                            DmUiLibrary.StopBrowserScrolling();
                            e.preventDefault();
                            e.stopPropagation();
                        }
                    }
                }
            });
            $(document).on("keydown", ".root-tab-element", function (e) {
                if (e.keyCode === 38 || e.keyCode === 40) {
                    var allElementsInBlock = $(this).parents(".master-of-roots").find(".root-tab-element");
                    var index = allElementsInBlock.index(this);

                    if (e.keyCode === 38) {
                        if (index > 0) {
                            allElementsInBlock[index - 1].focus();
                            DmUiLibrary.StopBrowserScrolling();
                        }
                    }
                    if (e.keyCode === 40) {
                        if (index < allElementsInBlock.length - 1) {
                            allElementsInBlock[index + 1].focus();
                            DmUiLibrary.StopBrowserScrolling();
                        }
                    }
                }
            });
            $(document).on("keyup", ".section-card.roster-table-card", function (e) {
                if (e.keyCode === 9) {
                    if ($(this).attr("tabindex") !== undefined) {
                        /*
                                                $("html , body").stop().animate({
                                                    scrollTop: $(this).offset().top - 60
                                                }, 200);
                        */
                    }
                }
            });
            //$(document).on("keydown", ".section-card.roster-table-card .k-grid-header th a, #roster-top-buttons-wrapper .k-input", function (e) {
            $(document).on("keydown", "#roster-top-buttons-wrapper .k-input", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $("body").removeClass("wcag_focuses_on");
                    //$(".roster-table-card.root-tab-element").rootMakeContentNotTabbable(false);
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    var self = $(this);
                    function setFocusBack() {
                        self.focus();
                        $("body").addClass("wcag_focuses_on");
                    }
                    setTimeout(setFocusBack, 100);
                }
            });
            $(document).on("keydown", "#roster-top-buttons-wrapper .k-button.k-button-icon, #apply-dashboard-report-button, #cancel-dashboard-report-button", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $("body").removeClass("wcag_focuses_on");
                    $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable(false);
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    function setFocusBack() {
                        $("#roster-filters-popup-button").focus();
                        $("body").addClass("wcag_focuses_on");
                        $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    }

                    if (!$("#modal-dashboard-report-criteria").is(":visible")) {
                        setTimeout(setFocusBack, 100);
                    }
                }
            });

            $(document).on("blur", "#roster-table-wrapper td, #roster-table-wrapper th, .dashboard-table td, .dashboard-table th", function (e) {
                //if (!private.RosterGridNavigationEvent && !private.RosterGridKeyEvent) { 
                if (true) {
                    if (e.relatedTarget !== null && e.relatedTarget.nodeName === "TABLE") {
                        $(e.target).focus();

                        function setFocusBack(element) {
                            if (!private.RosterGridNavigationEvent && !private.RosterGridOrderingEvent) {
                                element.focus();
                            }
                            private.RosterGridOrderingEvent = false;
                        }

                        if ((private.LastRosterTableKeyPressedCode >= 37 && private.LastRosterTableKeyPressedCode <= 40) || private.LastRosterTableKeyPressedCode === 9 || private.LastRosterTableKeyPressedCode === 16 || private.LastRosterTableKeyPressedCode === 13 || private.LastRosterTableKeyPressedCode === 0) {
                            setTimeout(setFocusBack, 1, $(e.target)); //for Firefox additional focus event
                        }
                        private.LastRosterTableKeyPressedCode = 0;
                    }
                }
                private.RosterGridNavigationEvent = false;
                private.RosterGridKeyEvent = false;
            });

            $(document).on("keyup", "#roster-table-wrapper td, #roster-table-wrapper th, .dashboard-table td, .dashboard-table th", function (e) {
                private.LastRosterTableKeyPressedCode = e.keyCode;
                private.RosterGridKeyEvent = true;
                if (e.keyCode === 9) {
                    Dashboard.setGridCellCurrentState($(this));
                }
            });
            $(document).on("keydown", "#roster-table-wrapper th.k-header, .dashboard-table th.k-header", function (e) {
                if (e.keyCode === 9) {
                    Dashboard.setGridCellCurrentState($(this));
                }
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    var orderingLink = $(this).find(".k-link");
                    if (orderingLink.length) {
                        $(this).find(".k-link").click();
                        Dashboard.setGridCellCurrentState(e.element);
                        private.RosterGridOrderingEvent = true;
                    }
                }
            });
            $(document).on("keydown", "#roster-table-wrapper td#roster-table-wrapper_active_cell, #roster-table-wrapper tbody th#roster-table-wrapper_active_cell", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    var link = $(this).find(".location-drill, .student-link, .student-info");
                    if (link.length) {
                        link.click();
                    }
                }
            });

            //***** Roster warning popup filters behavior *****
            $(document).on("change", ".table-warning-report-filters td:last-child select", function () {
                var rowNumber = $(this).attr("id");
                rowNumber = rowNumber.substr(rowNumber.length - 1).trim();
                var trNext = $(this).parents("tr").next();
                //console.log("rowNumber=" + rowNumber);
                //console.log(trNext);
                var selectedValue = $(this).val();
                if (Number(rowNumber) < 3) {
                    rowNumber = "" + (Number(rowNumber) + 1);
                    if (selectedValue === "and" || selectedValue === "or") {
                        if (trNext.hasClass("filter-row-disabled")) {
                            trNext.removeClass("filter-row-disabled");
                            $("#dropdown-warning-score-" + rowNumber).prop("disabled", false);
                            $("#dropdown-warning-boolean-" + rowNumber).prop("disabled", false);
                        }
                    } else {
                        trNext.addClass("filter-row-disabled");
                        $("#dropdown-warning-score-" + rowNumber).prop("disabled", true);
                        $("#dropdown-warning-boolean-" + rowNumber).prop("disabled", true);
                        if (trNext.next().length) {
                            trNext.next().addClass("filter-row-disabled");
                            $("#dropdown-warning-score-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-warning-boolean-" + rowNumber + " option").removeAttr("selected");
                            $("#dropdown-warning-boolean-" + rowNumber + " option:first-child").prop("selected", true);
                            $("#dropdown-warning-boolean-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-warning-score-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
                            $("#dropdown-warning-boolean-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
                        }
                    }
                    $("#dropdown-warning-score-" + rowNumber).trigger("DmUi:updated");
                    $("#dropdown-warning-boolean-" + rowNumber).trigger("DmUi:updated");
                }
            });

            //***** Roster popup filters behavior *****
            $(document).on("change", ".table-report-filters td:last-child select", function () {
                var rowNumber = $(this).attr("id");
                rowNumber = rowNumber.substr(rowNumber.length - 1).trim();
                var trNext = $(this).parents("tr").next();
                var selectedValue = $(this).val();
                if (Number(rowNumber) < 3) {
                    rowNumber = "" + (Number(rowNumber) + 1);
                    if (selectedValue === "and" || selectedValue === "or") {
                        if (trNext.hasClass("filter-row-disabled")) {
                            trNext.removeClass("filter-row-disabled");
                            $("#dropdown-score-" + rowNumber).prop("disabled", false);
                            $("#dropdown-content-area-" + rowNumber).prop("disabled", false);
                            $("#dropdown-condition-" + rowNumber).prop("disabled", false);
                            $("#input-score-" + rowNumber + "-val1").prop("disabled", false);
                            $("#input-score-" + rowNumber + "-val2").prop("disabled", false);
                            $("#dropdown-score-range-" + rowNumber).prop("disabled", false);
                            $("#dropdown-boolean-" + rowNumber).prop("disabled", false);
                        }
                    } else {
                        trNext.addClass("filter-row-disabled");
                        $("#dropdown-score-" + rowNumber).prop("disabled", true);
                        $("#dropdown-content-area-" + rowNumber).prop("disabled", true);
                        $("#dropdown-condition-" + rowNumber).prop("disabled", true);
                        $("#input-score-" + rowNumber + "-val1").prop("disabled", true);
                        $("#input-score-" + rowNumber + "-val2").prop("disabled", true);
                        $("#dropdown-score-range-" + rowNumber).prop("disabled", true);
                        $("#dropdown-boolean-" + rowNumber).prop("disabled", true);
                        if (trNext.next().length) {
                            trNext.next().addClass("filter-row-disabled");
                            $("#dropdown-score-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-content-area-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-condition-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#input-score-" + (Number(rowNumber) + 1) + "-val1").prop("disabled", true);
                            $("#input-score-" + (Number(rowNumber) + 1) + "-val2").prop("disabled", true);
                            $("#dropdown-score-range-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-boolean-" + rowNumber + " option").removeAttr("selected");
                            $("#dropdown-boolean-" + rowNumber + " option:first-child").prop("selected", true);
                            $("#dropdown-boolean-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-score-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
                            $("#dropdown-content-area-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
                            $("#dropdown-condition-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
                            $("#dropdown-score-range-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
                            $("#dropdown-boolean-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
                        }
                    }
                    $("#dropdown-score-" + rowNumber).trigger("DmUi:updated");
                    $("#dropdown-content-area-" + rowNumber).trigger("DmUi:updated");
                    $("#dropdown-condition-" + rowNumber).trigger("DmUi:updated");
                    $("#dropdown-score-range-" + rowNumber).trigger("DmUi:updated");
                    $("#dropdown-boolean-" + rowNumber).trigger("DmUi:updated");
                }
                Dashboard.ValidateReportFilters();
            });
            $(document).on("change", ".table-report-filters td:nth-child(3) select", function () {
                checkIsSelectedAnyInBetweenConditionOption();
                Dashboard.ValidateReportFilters();
            });
            $(document).on("keyup", ".table-report-filters tr:not(.filter-row-disabled) td.column-val1 input, .table-report-filters tr:not(.filter-row-disabled) td.column-val2 input, .table-report-filters tr:not(.filter-row-disabled) td:first-child select", function () {
                Dashboard.ValidateReportFilters();
            });
            function checkIsSelectedAnyInBetweenConditionOption() {
                var rowNumber;
                var isAnyInBetweenOptionSelected = false;
                $(".table-report-filters td.column-val1").removeClass("with-val2");
                $(".table-report-filters tr:not(.filter-row-disabled) td:nth-child(3) select").each(function () {
                    rowNumber = $(this).attr("id");
                    rowNumber = rowNumber.substr(rowNumber.length - 1).trim();
                    if (!$(this).prop("disabled") && $(this).val() === "is_in_between") {
                        isAnyInBetweenOptionSelected = true;
                        $("#input-score-" + rowNumber + "-val2").prop("disabled", false);
                        $("#wrapper-input-score-" + rowNumber + "-val2").removeClass("hidden");
                    } else {
                        $("#input-score-" + rowNumber + "-val2").prop("disabled", true);
                        $("#wrapper-input-score-" + rowNumber + "-val2").addClass("hidden");
                    }
                });
                if (isAnyInBetweenOptionSelected) {
                    $(".table-report-filters td.column-val1").addClass("with-val2");
                    $(".table-report-filters td.column-val2").removeClass("hidden");
                } else {
                    $(".table-report-filters td.column-val1").removeClass("with-val2");
                    $(".table-report-filters td.column-val2").addClass("hidden");
                }
            }
            function cancelDashboardReportButtonClick() {
                $(".table-report-filters .k-filtercell > span > .k-button").click();
                $("#modal-dashboard-report-criteria").fadeOut("fast");
                //$("#roster-reset-button").click();
                $("#roster-filters-popup-button").focus();
                if (private.isPopupFiltersOfRightCard) {
                    $("#right-card-popup-button").focus();
                } else {
                    $("#roster-filters-popup-button").focus();
                }
            }
            function cancelDashboardWarningFiltersButtonClick() {
                //Dashboard.resetInputValuesOfRosterWarningSearchPopup();
                $("#modal-dashboard-score-warning-report-criteria").fadeOut("fast");
                $("#roster-score-warning-filters-popup-button").focus();
            }
            $(document).on("click touchstart", "#cancel-dashboard-report-button", function () {
                cancelDashboardReportButtonClick();
            });
            $(document).on("click touchstart", "#cancel-dashboard-warning-report-button", function () {
                cancelDashboardWarningFiltersButtonClick();
            });
            $(document).on("keydown", "#modal-dashboard-report-criteria .column-val1 input, #modal-dashboard-report-criteria .column-val2 input", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    $("#apply-dashboard-report-button").click();
                }
            });
            $(document).on("click touchstart", "#apply-dashboard-report-button", function () {
                $("#modal-dashboard-report-criteria .dm-ui-alert-error").remove();
                var popupFilters = Dashboard.CreateRosterPopupFilters();
                var isFilteredResultExist = false;
                if (private.UseServerPaging) {
                    isFilteredResultExist = true;
                } else {
                    if (Dashboard.CountRowsInFilteredData(popupFilters) > 0) {
                        isFilteredResultExist = true;
                    }
                }
                if (isFilteredResultExist || private.EnforcedCutScoreResults) {
                    var dropdownContentAreaFailed = false;
                    $("#modal-dashboard-report-criteria #dropdown-content-area-1:enabled, #modal-dashboard-report-criteria #dropdown-content-area-2:enabled, #modal-dashboard-report-criteria #dropdown-content-area-3:enabled").each(function () {
                        if (!$(this).find("option:selected").length) {
                            dropdownContentAreaFailed = true;
                            return false;
                        }
                    });

                    if (dropdownContentAreaFailed && ((!isFilteredResultExist && private.EnforcedCutScoreResults) || private.UseServerPaging)) {
                        //reset cutscore card if the user unchecked one of the 'CONTENT AREA' filter option existing in the saved cutscore filter
                        $("#right-card-reset-button").click();
                        Dashboard.DeleteCookie("CutScoreFilters");
                        return;
                    }
                    var filtersLabelText = Dashboard.CutScoreLabelText();
                    $("#modal-dashboard-report-criteria").fadeOut("fast");
                    if (private.isPopupFiltersOfRightCard) {
                        /*
                                                if ($("#right-card-selected-label").text().trim() !== "") {
                                                    $("#right-card-selected-label .close").click();
                                                }
                        */
                        private.CutScoreAppliedFiltersLabelText = filtersLabelText;
                        private.CurrentPopupCriteriaRosterFilter = popupFilters;
                        if (private.UseServerPaging) {
                            if (private.LoggedUserLevel !== "CLASS") {
                                Dashboard.DisableRightCardPopupButton();
                                private.CurrentRosterFilter = popupFilters;
                                if (private.LoggedUserLevel === "BUILDING") {
                                    Dashboard.GetCutScore("CLASS", Dashboard.ConvertAppliedFilterObjectToString());
                                } else {
                                    Dashboard.GetCutScore("BUILDING", Dashboard.ConvertAppliedFilterObjectToString());
                                }
                            }
                        } else {
                            Dashboard.UpdateRightCard();
                        }
                        $("#right-card1").removeClass("overlayed");
                        Dashboard.EnableRightCardResetButton();
                        if (private.EnforcedCutScoreResults) {
                            $("#right-card1").rootMakeContentNotTabbable();
                        }
                        $("#right-card-popup-button").addClass("applied");
                        Dashboard.SaveAppliedPopupFiltersToCookie();
                        if (!private.EnforcedCutScoreResults) {
                            $("#right-card-popup-button").focus();
                        }
                        if (!isFilteredResultExist && private.EnforcedCutScoreResults) {
                            $("#modal-dashboard-report-criteria .table-report-filters").before('<div class="dm-ui-alert dm-ui-alert-error" role="alert"><a href="#" class="dm-ui-alert-close" aria-label="Close alert message" role="button">×</a>No data resulted in your search criteria, please change criteria and try again.</div>');
                        }
                        if ($("body").hasClass("wcag_focuses_on") && !private.EnforcedCutScoreResults) {
                            $("#right-card1.root-tab-element").rootMakeContentTabbable();
                        }
                        private.EnforcedCutScoreResults = false;
                    } else {
                        Dashboard.ClearAllFiltersSelection();
                        private.CurrentRosterFilter = popupFilters;
                        private.arrayAppliedRosterFilterDropdownsCondition = Dashboard.AppliedPopupFiltersToArray();
                        Dashboard.UpdateRosterGrid();
                        Dashboard.UpdateRightCard2();
                        Dashboard.EnableRosterFiltersPopupButton();
                        $("#roster-filters-popup-button").addClass("applied");
                        $("#right-card-popup-button").removeClass("applied");
                        Dashboard.DisableRightCardPopupButton();
                        $("#roster-filters-popup-button").focus();
                        $(".roster-table-card.root-tab-element").rootMakeContentTabbable();

                        $("#roster-card-selected-popup-filters-label").html('<div class="arrow"></div><i>Identified students by<br>' + filtersLabelText + '</i><div class="close tab" tabindex="' + private.defaultTabIndex + '" aria-label="Clear filter" role="button">X</div>');
                        $("#roster-selected-labels-wrapper").show();
                    }
                    //private.isPopupFiltersOfRightCard = false;
                } else {
                    $("#modal-dashboard-report-criteria .table-report-filters").before('<div class="dm-ui-alert dm-ui-alert-error" role="alert"><a href="#" class="dm-ui-alert-close" aria-label="Close alert message" role="button">×</a>No data resulted in your search criteria, please change criteria and try again.</div>');
                }
            });

            $(document).on("click touchstart", "#apply-dashboard-warning-report-button", function () {
                $("#modal-dashboard-score-warning-report-criteria .dm-ui-alert-error").remove();
                var popupWarningsFilters = Dashboard.CreateRosterWarningsPopupFilters();
                if ($("#stanine-table-selected-label").text().trim() === "" && $("#right-card-selected-label").text().trim() === "" && $("#roster-card-selected-popup-filters-label").text().trim() === "" &&
                    $("#roster-card-selected-popup-warning-filters-label").text().trim() !== "" && $("#right-card2-selected-label").text().trim() !== "") {
                    $("#roster-reset-button").click();
                } else {
                    if (!$.isEmptyObject(private.CurrentRosterFilter)) {
                        var cloneCurrentRosterFilter = JSON.parse(JSON.stringify(private.CurrentRosterFilter)); //clone object
                        if ($("#roster-card-selected-popup-warning-filters-label").text().trim() !== "") {
                            if (cloneCurrentRosterFilter.filters.length >= 2) {
                                cloneCurrentRosterFilter.filters.splice(cloneCurrentRosterFilter.filters.length - 1, 1);
                                cloneCurrentRosterFilter.filters.push(popupWarningsFilters.filters[0]);
                                popupWarningsFilters = cloneCurrentRosterFilter;
                            }
                        } else {
                            cloneCurrentRosterFilter.filters.push(popupWarningsFilters.filters[0]);
                            popupWarningsFilters = cloneCurrentRosterFilter;
                        }
                    }
                }
                if (Dashboard.CountRowsInFilteredData(popupWarningsFilters) > 0) {
                    private.CurrentRosterFilter = popupWarningsFilters;
                    private.arrayAppliedRosterWarningFilterDropdownsCondition = Dashboard.AppliedPopupWarningFiltersToArray();
                    var filtersLabelText = Dashboard.CutScoreWarningsLabelText();
                    $("#modal-dashboard-score-warning-report-criteria").fadeOut("fast");
                    //Dashboard.ClearAllFiltersSelection();
                    Dashboard.UpdateRosterGrid();
                    Dashboard.UpdateRightCard2();
                    $("#roster-score-warning-filters-popup-button").focus();
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    if ($("#stanine-table-selected-label").text() === "" && $("#right-card-selected-label").text() === "" && $("#right-card2-selected-label").text() === "" && $("#roster-card-selected-popup-filters-label").text() === "") {
                        $("#roster-card-selected-popup-warning-filters-label").html('<div class="arrow"></div><i>Identified students by<br>' + filtersLabelText + '</i><div class="close tab" tabindex="' + private.defaultTabIndex + '" aria-label="Clear filter" role="button">X</div>');
                    } else {
                        $("#roster-card-selected-popup-warning-filters-label").html('<div class="arrow"></div><span>' + filtersLabelText + '</span><div class="close shifted tab" tabindex="' + private.defaultTabIndex + '" aria-label="Clear filter" role="button">X</div>');
                        if ($("#right-card2-selected-label").text() !== "") {
                            //swap elements
                            $("#right-card2-selected-label").insertBefore($("#roster-card-selected-popup-warning-filters-label"));
                        }
                    }
                    $("#roster-selected-labels-wrapper").show();
                } else {
                    $("#modal-dashboard-score-warning-report-criteria .table-warning-report-filters").before('<div class="dm-ui-alert dm-ui-alert-error" role="alert"><a href="#" class="dm-ui-alert-close" aria-label="Close alert message" role="button">×</a>Score warning indicator is not present with this selection, please try again.</div>');
                }
            });

            $(document).on("blur keypress", ".table-report-filters input.k-input, .table-report-filters .k-button.k-button-icon", function () {
                var someFiltersApplied = false;
                $(".table-report-filters .k-button.k-button-icon").each(function () {
                    if ($(this).attr("style") !== "display: none;") {
                        someFiltersApplied = true;
                        return false;
                    }
                });
                if (someFiltersApplied) {
                    $("#apply-dashboard-report-button").prop("disabled", false);
                } else {
                    $("#apply-dashboard-report-button").prop("disabled", true);
                }
            });


            //***** Filters button 'Reset Page' *****
            $(document).on("click touchstart", ".dashboard-filter.button button", function () {
                DmUiLibrary.AbortAllAjaxRequests(); //abort all Ajax requests
                private.HybridGetAllRosterFlag = false;
                Dashboard.StopRosterProgressBar();

                private.arrColumnsSelectedInRoster = ["APR"];
                //$(".bread-crumbs .location-drill:first-child").click();
                $(this).addClass("disabled-element");
                $(this).makeElementNotTabbable();
                $.ajax({
                    async: true,
                    type: "POST",
                    url: siteRoot + "/DashboardCogat/ResetPage",
                    //data: data,
                    //dataType: "json",
                    dataType: "html",
                    beforeSend: function () {
                        //DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        //if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        if (data === '"Success"') {
                            Dashboard.GetFilters(false);
                        }
                    },
                    error: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });


            //***** Roster other buttons, filters & popup behavior *****
            $(document).on("keyup", "#right-card-reset-button", function (e) {
                if (e.keyCode === 13 || e.which === 32) {
                    $("#right-card-reset-button").click();
                    Dashboard.DeleteCookie("CutScoreFilters");
                }
            });

            $(document).on("keyup", "#roster-reset-button", function (e) {
                if (e.keyCode === 13 || e.which === 32) {
                    $("#roster-reset-button").click();
                }
            });

            $(document).on("click touchstart", "#right-card-reset-button", function (e) {
                if ($("#right-card-selected-label").text().trim() !== "") {
                    $("#right-card-selected-label .close").click();
                }
                private.CurrentPopupCriteriaRosterFilter = {};
                private.modelFilteredRosterRightTopCard = {};
                private.modelCutScore = {};
                //Dashboard.UpdateRightCard();
                Dashboard.resetInputValuesOfRosterSearchPopup();
                Dashboard.redrawRightCardTable();
                Dashboard.DisableRightCardResetButton();
                $("#right-card1").addClass("overlayed");
                if (e.originalEvent !== undefined) {
                    Dashboard.EnableRightCardPopupButton();
                    if ($("body").hasClass("wcag_focuses_on")) {
                        $("#right-card-popup-button").focus();
                    } else {
                        $("#right-card1").rootMakeContentNotTabbable();
                    }
                    Dashboard.DeleteCookie("CutScoreFilters");
                }
            });

            $(document).on("click touchstart", "#roster-reset-button", function (e) {
                e.preventDefault();

                if (!$("#roster-filters-popup-button").hasClass("disabled-element")) {
                    Dashboard.resetInputValuesOfRosterSearchPopup();
                }
                if (!$("#roster-score-warning-filters-popup-button").hasClass("disabled-element")) {
                    Dashboard.resetInputValuesOfRosterWarningSearchPopup();
                }

                private.CurrentRosterFilter = {};
                private.arrColumnsSelectedInRoster = ["APR"];
                Dashboard.UpdateRosterGrid();
                Dashboard.UpdateRightCard2();

                $("#stanine-table-wrapper table td, #right-card-table-wrapper table td, #right-card-table2-wrapper table td").removeClass("selected-content-stanine");
                $("#right-card-table-wrapper table tr, #right-card-table2-wrapper table tr").removeClass("selected-right-card-tr");
                //$("#right-card1").addClass("overlayed");
                Dashboard.EnableRosterFiltersPopupButton();
                Dashboard.DisableRosterResetButton();
                Dashboard.EnableRightCardPopupButton();

                //private.isPopupFiltersOfRightCard = false;

                $("#roster-selected-labels-wrapper").hide();
                Dashboard.ClearAllFiltersSelection();
                $("#stanine-table-wrapper table td, #right-card-table-wrapper table td").removeClass("selected-content-stanine");
            });
            function checkIsResetRosterButtonShouldBeEnabledOrNot() {
                if ($("#roster-selected-labels-wrapper .arrow").length === 0) {
                    Dashboard.DisableRosterResetButton();
                }
            }
            $(document).on("click touchstart", "#roster-top-buttons-wrapper span.k-filtercell button", function () {
                checkIsResetRosterButtonShouldBeEnabledOrNot();
            });
            function focusFirstFilterReportCriteria() {
                $("#modal-dashboard-report-criteria #dropdown-score-1_dm_ui .dm-ui-dropdown-button").focus();
                if (!$("body").hasClass("wcag_focuses_on")) {
                    $("#modal-dashboard-report-criteria #dropdown-score-1_dm_ui .dm-ui-dropdown-button").blur();
                }
            }
            function focusFirstWarningFilterReportCriteria() {
                $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-score-1_dm_ui .dm-ui-dropdown-button").focus();
                if (!$("body").hasClass("wcag_focuses_on")) {
                    $("#modal-dashboard-score-warning-report-criteria #dropdown-warning-score-1_dm_ui .dm-ui-dropdown-button").blur();
                }
            }
            function focusFirstPrintReports() {
                $("#modal-dashboard-print-students #print-report-type_dm_ui .dm-ui-dropdown-button").focus();
                if (!$("body").hasClass("wcag_focuses_on")) {
                    $("#modal-dashboard-print-students #print-report-type_dm_ui .dm-ui-dropdown-button").blur();
                }
            }
            $(document).on("click touchstart", "#roster-filters-popup-button", function () {
                private.isPopupFiltersOfRightCard = false;
                if ($("#roster-card-selected-popup-filters-label").text().trim() === "") {
                    private.arrayAppliedRosterFilterDropdownsCondition = [];
                }
                Dashboard.resetInputValuesOfRosterSearchPopup();
                Dashboard.RestoreAppliedPopupFiltersFromArray(private.arrayAppliedRosterFilterDropdownsCondition);

                $("#modal-dashboard-report-criteria").fadeIn("fast", function () { });
                //setTimeout(focusFirstFilterReportCriteria, 100);
                $("#dropdown-score-1_dm_ui > button.dm-ui-dropdown-button").addClass("first-tab-element wcag-modal-first-element");
                $("#dropdown-score-1_dm_ui > button.dm-ui-dropdown-button").focus();
            });
            $(document).on("keydown", "#roster-filters-popup-button", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    $("#roster-filters-popup-button").click();
                }
            });
            $(document).on("click touchstart", "#roster-score-warning-filters-popup-button", function () {
                //Dashboard.GetScoreWarnings();
                if ($("#roster-card-selected-popup-warning-filters-label").text().trim() === "") {
                    private.arrayAppliedRosterWarningFilterDropdownsCondition = [];
                }
                Dashboard.resetInputValuesOfRosterWarningSearchPopup();
                Dashboard.RestoreAppliedPopupWarningFiltersFromArray(private.arrayAppliedRosterWarningFilterDropdownsCondition);

                $("#modal-dashboard-score-warning-report-criteria").fadeIn("fast", function () { });
                //setTimeout(focusFirstWarningFilterReportCriteria, 100);
                $("#dropdown-warning-score-1_dm_ui > button.dm-ui-dropdown-button").addClass("first-tab-element wcag-modal-first-element");
                $("#dropdown-warning-score-1_dm_ui > button.dm-ui-dropdown-button").focus();
            });
            $(document).on("keydown", "#roster-score-warning-filters-popup-button", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    $("#roster-score-warning-filters-popup-button").click();
                }
            });
            $(document).on("click", "#right-card-popup-button", function () {
                //if (!private.isPopupFiltersOfRightCard) {
                Dashboard.resetInputValuesOfRosterSearchPopup();
                Dashboard.RestoreAppliedPopupFiltersFromCookie();
                //}
                private.isPopupFiltersOfRightCard = true;
                $("#modal-dashboard-report-criteria").fadeIn("fast", function () { });
                setTimeout(focusFirstFilterReportCriteria, 100);
                $("#dropdown-score-1_dm_ui > button.dm-ui-dropdown-button").addClass("first-tab-element wcag-modal-first-element");
                $("#dropdown-score-1_dm_ui > button.dm-ui-dropdown-button").focus();
            });
            $(document).on("keydown", "#right-card-popup-button", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    $("#right-card-popup-button").click();
                }
            });
            function applySearchRosterByName() {
                $("#roster-top-buttons-wrapper .k-filtercell > span > .k-button").each(function () {
                    Dashboard.EnableRosterResetButton();

                    if ($(this).attr("title") === "Clear") {
                        if ($(this).is(":visible")) {
                            $(this).makeElementTabbable();
                            $("#roster-search-label").addClass("visible");
                        } else {
                            $(this).makeElementNotTabbable();
                            $("#roster-search-label").removeClass("visible");
                        }
                        return false;
                    }
                });
            }
            $(document).on("keyup", "#roster-top-buttons-wrapper span.k-filtercell input.k-input", function (e) {
                //checking Enter & ESC keys pressing in Roster Search field & making focusable or not kendo grid filter icon 'Clear search criteria'
                if (e.keyCode === 13 || e.which === 27) {
                    applySearchRosterByName();
                }
            });
            $(document).on("click touchstart", ".k-animation-container .k-list-scroller ul li", function () {
                //user clicked on some search autofill list value
                applySearchRosterByName();
                $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
            });
            $(document).on("click touchstart", "#roster-search-field button.k-button-icon", function () {
                //Clear Search field button click
                $("#roster-search-label").removeClass("visible");
            });
            $(document).on("click touchstart", "#modal-dashboard-report-criteria .close_icon", function () {
                /*
                if ($("#modal-dashboard-report-criteria .dm-ui-alert-error").is(":visible")) {
                    $("#cancel-dashboard-report-button").click();
                } else {
                    $("#modal-dashboard-report-criteria").fadeOut("fast", function () { });
                }
                */
                $("#cancel-dashboard-report-button").click();
            });
            $(document).on("click touchstart", "#modal-dashboard-score-warning-report-criteria .close_icon", function () {
                $("#cancel-dashboard-warning-report-button").click();
            });
            $(document).on("click touchstart", "#stanine-table-selected-label .close, #right-card-selected-label .close, #roster-card-selected-popup-filters-label .close", function () {
                $("#roster-reset-button").click();
                if ($("body").hasClass("wcag_focuses_on")) {
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    $("#right-card1.root-tab-element").rootMakeContentNotTabbable();
                    $("#right-card2.root-tab-element").rootMakeContentNotTabbable();
                    $("#Score_Control_dm_ui > button").focus();
                } else {
                    $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable();
                }
            });
            $(document).on("keydown", "#stanine-table-selected-label .close, #right-card-selected-label .close, #right-card2-selected-label .close, #roster-card-selected-popup-filters-label .close, #roster-card-selected-popup-warning-filters-label .close", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    e.preventDefault();
                    $(this).click();
                }
            });
            $(document).on("click touchstart", "#right-card2-selected-label .close", function () {
                if ($("#stanine-table-selected-label").text().trim() === "" && $("#right-card-selected-label").text().trim() === "" && $("#roster-card-selected-popup-filters-label").text().trim() === "" && $("#roster-card-selected-popup-warning-filters-label").text().trim() === "") {
                    $("#roster-reset-button").click();
                } else {
                    if ($("#stanine-table-selected-label").text().trim() !== "") {
                        $("#stanine-table-wrapper .selected-content-stanine .stanine-table-link").click();
                    }
                    if ($("#right-card-selected-label").text().trim() !== "") {
                        $("#right-card-table-wrapper .selected-content-stanine .stanine-table-link").click();
                    }
                    if ($("#roster-card-selected-popup-filters-label").text().trim() !== "") {
                        $("#modal-dashboard-report-criteria #apply-dashboard-report-button").click();
                    }
                    if ($("#roster-card-selected-popup-warning-filters-label").text().trim() !== "") {
                        if ($("#stanine-table-selected-label").text().trim() === "" && $("#right-card-selected-label").text().trim() === "" && $("#roster-card-selected-popup-filters-label").text().trim() === "") {
                            $("#roster-reset-button").click();
                        }
                        $("#modal-dashboard-score-warning-report-criteria #apply-dashboard-warning-report-button").click();
                    }
                }
                if ($("body").hasClass("wcag_focuses_on")) {
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    $("#right-card1.root-tab-element").rootMakeContentNotTabbable();
                    $("#right-card2.root-tab-element").rootMakeContentNotTabbable();
                    $("#Score_Control_dm_ui > button").focus();
                } else {
                    $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable();
                }
            });
            $(document).on("click touchstart", "#roster-card-selected-popup-warning-filters-label .close", function () {
                if ($("#stanine-table-selected-label").text().trim() === "" && $("#right-card-selected-label").text().trim() === "" && $("#right-card2-selected-label").text().trim() === "" && $("#roster-card-selected-popup-filters-label").text().trim() === "") {
                    $("#roster-reset-button").click();
                } else {
                    if ($("#stanine-table-selected-label").text().trim() !== "") {
                        $("#stanine-table-wrapper .selected-content-stanine .stanine-table-link").click();
                    }
                    if ($("#right-card-selected-label").text().trim() !== "") {
                        $("#right-card-table-wrapper .selected-content-stanine .stanine-table-link").click();
                    }
                    if ($("#roster-card-selected-popup-filters-label").text().trim() !== "") {
                        $("#modal-dashboard-report-criteria #apply-dashboard-report-button").click();
                    }
                    if ($("#right-card2-selected-label").text().trim() !== "") {
                        //TODO remember selected row text 1A, 2A, 3A, use timer, reset roster, find page with this text & click
                        $("#roster-reset-button").click();
                        //$("#right-card-table2-wrapper .selected-content-stanine .stanine-table-link").click();
                    }
                }
                if ($("body").hasClass("wcag_focuses_on")) {
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    $("#right-card1.root-tab-element").rootMakeContentNotTabbable();
                    $("#right-card2.root-tab-element").rootMakeContentNotTabbable();
                    $("#Score_Control_dm_ui > button").focus();
                } else {
                    $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable();
                }
            });

            $(document).on("change", "#dropdown-graph-content", function () {
                $("#stanine-table-wrapper table tr").removeClass("stanine-selected");
                var valSelected = $(this).val();
                var idSelected = 0;
                var i = 0;
                private.selectedContentStanineCard = valSelected;
                Dashboard.redrawBarChartStanine(private.modelAgeStanine.content_area, "", valSelected);
                $(this).find("option").each(function () {
                    i++;
                    if ($(this).attr("value") === valSelected) {
                        idSelected = i;
                    }
                });
                $("#stanine-table-wrapper table tr:nth-of-type(" + idSelected + ")").addClass("stanine-selected");
                $(".quantiles-header-text").text(valSelected + " Students by Age Stanine");
                //$(".quantiles-header-text").attr("aria-label", valSelected + " Students by Age Stanine");
            });

            $(document).on("click", "#stanine-table-wrapper table td .stanine-table-link", function (e) {
                e.preventDefault();
                var element = $(this).parent();
                $(".roster-table-card").fadeIn();
                var content = element.parent("tr").find("th:first-child").text();
                var stanine = element.parents("tbody").prev().find("tr th:nth-child(" + (element.index() + 1) + ")").text();
                if (element.index() >= 1) {
                    $("#stanine-table-wrapper table td, #right-card-table-wrapper table td").removeClass("selected-content-stanine");
                    Dashboard.ClearAllFiltersSelection();
                    $("#right-card-table-wrapper table tr").removeClass("selected-right-card-tr");
                    element.addClass("selected-content-stanine");
                    $("#stanine-table-selected-label").html('<div class="arrow"></div><i>Identified students by<br><span><b>' + content + "</b> and <b>Age Stanine " + stanine + '</b></span></i><div class="close tab" tabindex="' + private.defaultTabIndex + '" aria-label="Clear Age Stanine filter" role="button">X</div>');
                    Dashboard.CheckAgeStanineEnabled();
                    $("#roster-selected-labels-wrapper").show();

                    var field = "AS" + element.parent().index();
                    var value = Number(stanine);
                    private.CurrentRosterFilter = {};
                    Dashboard.UpdateCurrentRosterFiltersAdd(field, value);
                    //private.arrColumnsSelectedInRoster = ["APR", "AS"];
                    private.arrColumnsSelectedInRoster = ["AS"];
                    Dashboard.UpdateRosterGrid();
                    Dashboard.UpdateRightCard2();
                    scrollToRosterCard($(this));
                    Dashboard.DisableRosterFiltersPopupButton();
                }
            });
            $(document).on("keydown", "#stanine-table-wrapper table td", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    if ($(this).find(".stanine-table-link").length) {
                        $(this).find(".stanine-table-link").click();
                    }
                }
            });

            function changeRightCardTableSelectedCell(element) {
                var wasSelected = element.parent("tr").hasClass("selected-content-stanine");
                if (!wasSelected) {
                    $(".roster-table-card").fadeIn();
                    //var content = element.parent("tr").find("td:first-child").text();
                    //var type = element.parents("table").find("thead tr th:first-child").text();
                    if (element.index() >= 1) {
                        $("#stanine-table-wrapper table td, #right-card-table-wrapper table td, #right-card-table2-wrapper table td").removeClass("selected-content-stanine");
                        $("#right-card-table-wrapper table tr, #right-card-table2-wrapper table tr").removeClass("selected-right-card-tr");
                        element.addClass("selected-content-stanine");
                        element.parent("tr").addClass("selected-right-card-tr");
                    }
                }
            }
            function changeRightCardTable2SelectedCell(element) {
                var wasSelected = element.parent("tr").hasClass("selected-content-stanine");
                if (!wasSelected) {
                    //$(".roster-table-card").fadeIn();
                    //var content = element.parent("tr").find("td:first-child").text();
                    //var type = element.parents("table").find("thead tr th:first-child").text();
                    if (element.index() >= 1) {
                        $("#right-card-table2-wrapper table td").removeClass("selected-content-stanine");
                        $("#right-card-table2-wrapper table tr").removeClass("selected-right-card-tr");
                        element.addClass("selected-content-stanine");
                        element.parent("tr").addClass("selected-right-card-tr");
                    }
                }
            }
            function scrollToRosterCard(element) {
                $("html , body").stop().animate({
                    scrollTop: $(".roster-table-card").offset().top - 60
                }, 200);
                setTimeout(function () {
                    element.parents(".root-tab-element").rootMakeContentNotTabbable();
                    $(".roster-table-card.root-tab-element").focus();
                }, 200);
            }
            $(document).on("click", "#right-card-table-wrapper table td .stanine-table-link", function (e) {
                e.preventDefault();

                var filterText = $(this).parents("tr").find("th:first-child").text();
                var filterValue = $(this).data("id");

                private.arrColumnsSelectedInRoster = JSON.parse(JSON.stringify(private.arrColumnsSelectedInRightCardPopupFilter)); //clone object

                //private.CurrentRosterFilter = {};
                private.CurrentRosterFilter = JSON.parse(JSON.stringify(private.CurrentPopupCriteriaRosterFilter)); //clone object
                if (private.LoggedUserLevel === "BUILDING") {
                    //Dashboard.UpdateCurrentRosterFiltersAdd("class_name", filterValue);
                    Dashboard.UpdateCurrentRosterFiltersAdd("class_id", filterValue);
                } else {
                    //Dashboard.UpdateCurrentRosterFiltersAdd("building", filterValue);
                    Dashboard.UpdateCurrentRosterFiltersAdd("building_id", filterValue);
                }
                //Dashboard.UpdateCurrentRosterFiltersRemove("ability_profile");

                changeRightCardTableSelectedCell($(this).parent());
                Dashboard.UpdateRosterGrid();
                Dashboard.UpdateRightCard2();

                Dashboard.ClearAllFiltersSelection();
                if (private.LoggedUserLevel === "BUILDING") {
                    $("#right-card-selected-label").html('<div class="arrow"></div><i>Identified students by<br><span><b>' + private.SuppressedTextClass + "</b>: " + filterText + "<br>" + private.CutScoreAppliedFiltersLabelText + '</span></i><div class="close tab" tabindex="' + private.defaultTabIndex + '" aria-label="Clear filter" role="button">X</div>');
                } else {
                    $("#right-card-selected-label").html('<div class="arrow"></div><i>Identified students by<br><span><b>' + private.SuppressedTextBuilding + "</b>: " + filterText + "<br>" + private.CutScoreAppliedFiltersLabelText + '</span></i><div class="close tab" tabindex="' + private.defaultTabIndex + '" aria-label="Clear filter" role="button">X</div>');
                }
                $("#roster-selected-labels-wrapper").show();

                Dashboard.DisableRosterFiltersPopupButton();
                Dashboard.EnableRightCardPopupButton();
                scrollToRosterCard($(this));
            });
            $(document).on("keydown", "#right-card-table-wrapper table td", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    if ($(this).find(".stanine-table-link").length) {
                        $(this).find(".stanine-table-link").click();
                    }
                }
            });

            $(document).on("keyup", "#stanine-table-wrapper table td .stanine-table-link, #right-card-table-wrapper table td .stanine-table-link, #right-card-table2-wrapper table td .stanine-table-link, #right-card-table2-wrapper table th .ability-profile-link, #roster-table-wrapper table th .student-info", function (e) {
                if (!private.isTabNavigationOn && (e.keyCode === 13 || e.keyCode === 32)) {
                    e.preventDefault();
                    $(this).click();
                }
            });

            $(document).on("click", "#right-card-table2-wrapper table td .ability-profile-link, #right-card-table2-wrapper table tbody th .ability-profile-link, #right-card-table2-wrapper table td .stanine-table-link", function (e) {
                if ($(this).hasClass("stanine-table-link")) {
                    e.preventDefault();
                    var filterValue;
                    //filterValue = $(this).parents("tr").find("td:first-child a").text();

                    filterValue = "";
                    Dashboard.UpdateCurrentRosterFiltersRemoveTotal("ability_profile");
                    $(this).parents("tr").find("th:first-child .ability-profile-link").each(function () {
                        if (filterValue !== "") {
                            filterValue += ", ";
                        }
                        filterValue += $(this).text();
                        //Dashboard.UpdateCurrentRosterFiltersAddOrLogic("ability_profile", $(this).text());
                        Dashboard.UpdateCurrentRosterFiltersAddOrLogic("ability_profile", Dashboard.CorrectAbilityProfileFilerValue($(this).text()));
                    });
                    if (private.CurrentRosterFilter.logic === "or") {
                        var objCorrection = { "logic": "and", "filters": [] };
                        objCorrection.filters.push(private.CurrentRosterFilter);
                        private.CurrentRosterFilter = objCorrection;
                    }
                    //console.log(private.CurrentRosterFilter);
                    //Dashboard.UpdateCurrentRosterFiltersAdd("ability_profile", filterValue);

                    changeRightCardTable2SelectedCell($(this).parent());
                    Dashboard.UpdateRosterGrid();

                    if ($("#stanine-table-selected-label").text() === "" && $("#right-card-selected-label").text() === "" && $("#roster-card-selected-popup-filters-label").text() === "" && $("#roster-card-selected-popup-warning-filters-label").text() === "") {
                        $("#right-card2-selected-label").html('<div class="arrow"></div><i>Identified students by<br><span><b>Ability Profile</b>: ' + filterValue + '</span></i><div class="close tab" tabindex="' + private.defaultTabIndex + '" aria-label="Clear filter" role="button">X</div>');
                    } else {
                        $("#right-card2-selected-label").html('<div class="arrow"></div><span><b>Ability Profile</b>: ' + filterValue + '</span><div class="close shifted tab" tabindex="' + private.defaultTabIndex + '" aria-label="Clear filter" role="button">X</div>');
                        if ($("#roster-card-selected-popup-warning-filters-label").text() !== "") {
                            //swap elements
                            $("#roster-card-selected-popup-warning-filters-label").insertBefore($("#right-card2-selected-label"));
                        }
                    }
                    $("#roster-selected-labels-wrapper").show();

                    Dashboard.DisableRosterFiltersPopupButton();
                    scrollToRosterCard($(this));
                } else {
                    e.preventDefault();
                    var href = Dashboard.GenerateAbilityProfileLink($(this).text());
                    window.open(href);
                    /*
                                        if (e.isTrigger !== undefined) { //only for keyboard events
                                            window.open($(this).attr("href"));
                                        }
                    */
                }
            });
            $(document).on("keydown", "#right-card-table2-wrapper table td, #right-card-table2-wrapper table tbody th", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    if ($(this).find(".stanine-table-link").length) {
                        $(this).find(".stanine-table-link").click();
                    }
                    if ($(this).find(".ability-profile-link").length) {
                        $(this).find(".ability-profile-link").first().click();
                    }
                }
            });

            /*
                        $(document).on("keydown", "#right-card-table-wrapper table td, #right-card-table2-wrapper table td", function (e) {
                            if (e.keyCode === 13) {
                                var link = $(this).find("a").first();
                                if (link.length) {
                                    link.click();
                                }
                            }
                        });
            */

            $(document).on("change", "select#Score_Control", function () {
                private.arrColumnsSelectedInRoster = [];
                $("select#Score_Control option").each(function () {
                    if ($(this).prop("selected")) {
                        private.arrColumnsSelectedInRoster.push($(this).val());
                    }
                });
                Dashboard.UpdateRosterGrid();
            });

            $(document).on("change", "select#print-grade", function () {
                createDistrictOrClassLocationsTreeListStructure();
            });

            $(document).on("click touchstart", ".svg-tooltip-wrapper", function () {
                $("#stanine-table-wrapper tr.stanine-selected td:nth-child(" + ($(this).data("stanine") + 1) + ") .stanine-table-link").click();
            });

            function wcagSearchAutocompleteResultsAnnounce() {
                var tmpLength = $("div.k-animation-container ul.k-list.k-reset li.k-item").length;
                if (tmpLength !== private.SearchAutocompleteFoundNum) {
                    private.SearchAutocompleteFoundNum = tmpLength;
                    $("#wcag-autocomplete-search-results").text(private.SearchAutocompleteFoundNum + " results available.");
                }
            }
            $(document).on("DOMSubtreeModified", "div.k-animation-container ul.k-list.k-reset", function () {
                wcagSearchAutocompleteResultsAnnounce();
            });
            $(document).on("keyup", "#roster-search", function (e) {
                if ($("#roster-search").val().length) {
                    $("#roster-search-label").addClass("visible");
                    $("body").addClass("wcag_focuses_on");
                    if ($(".roster-table-card.root-tab-element").attr("tabindex") === "-1" && private.isTabNavigationOn) {
                        $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    }
                    if ($("#roster-search").val().length) {
                        setTimeout(function () {
                            wcagSearchAutocompleteResultsAnnounce();
                        }, 500);
                    }
                } else {
                    $("#roster-search-label").removeClass("visible");
                }
            });

            $(document).on("click touchstart", "#feedback-button", function () {
                $("#modal-feedback section.form").show();
                $("#modal-feedback section.success").hide();
                $("#modal-feedback").fadeIn("fast");

                EnableDisableFeedbackForm();
                if ($("#feedbackFirstName:enabled").length) {
                    $("#feedbackFirstName").focus();
                } else {
                    $("#modal-feedback .close_icon").focus();
                }

                $("#feedback-chk").on("click", function () {
                    EnableDisableFeedbackForm();
                });
            });
            $(document).on("click touchstart", "#modal-feedback .close_icon, #modal-feedback section.success button", function () {
                $("#modal-feedback .ajax-error-message").empty();
                $("#modal-feedback").fadeOut("fast");
                $("#feedback-button").focus();
            });
            $(document).on("keyup", "body, .root-tab-element", function (e) {
                if (e.keyCode === 27) {
                    if ($("#modal-feedback").is(":visible")) {
                        $("#modal-feedback").fadeOut("fast");
                        $("#feedback-button").focus();
                    }
                }
            });
            function EnableDisableFeedbackForm() {
                if ($("#feedback-chk").is(":checked")) {
                    $("#form-feedback *").prop("disabled", false);
                    $("#modal-feedback a").removeClass("last-tab-element wcag-modal-last-element");
                    $("#form-feedback button").addClass("last-tab-element wcag-modal-last-element");
                } else {
                    $("#form-feedback *").prop("disabled", true);
                    $("#modal-feedback a").addClass("last-tab-element wcag-modal-last-element");
                    $("#form-feedback button").removeClass("last-tab-element wcag-modal-last-element");
                }
            }
            function clearFeedbackForm() {
                $("#feedbackFirstName, #feedbackLastName, #feedbackEmailAddress, #feedbackDistrict, #feedbackSchool, #feedbackRole, #feedbackMessage").val("");
            }
            function validateEmail($email) {
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,6})?$/;
                return emailReg.test($email);
            }
            function validateFeedbackForm(isFocusErrorField) {
                var isValidated = true;
                var focusedElement;
                if ($("#feedbackMessage").val().trim() === "") {
                    isValidated = false;
                    $("#feedbackMessage").addClass("validation-error");
                    focusedElement = $("#feedbackMessage");
                } else {
                    $("#feedbackMessage").removeClass("validation-error");
                }
                if ($("#feedbackEmailAddress").val().trim() === "" || !validateEmail($("#feedbackEmailAddress").val())) {
                    isValidated = false;
                    $("#feedbackEmailAddress").addClass("validation-error");
                    focusedElement = $("#feedbackEmailAddress");
                } else {
                    $("#feedbackEmailAddress").removeClass("validation-error");
                }
                if ($("#feedbackLastName").val().trim() === "") {
                    isValidated = false;
                    $("#feedbackLastName").addClass("validation-error");
                    focusedElement = $("#feedbackLastName");
                } else {
                    $("#feedbackLastName").removeClass("validation-error");
                }
                if ($("#feedbackFirstName").val().trim() === "") {
                    isValidated = false;
                    $("#feedbackFirstName").addClass("validation-error");
                    focusedElement = $("#feedbackFirstName");
                } else {
                    $("#feedbackFirstName").removeClass("validation-error");
                }

                if (isValidated) {
                    $("#modal-feedback .validation-error-message").addClass("hidden");
                } else {
                    $("#modal-feedback .validation-error-message").removeClass("hidden");
                    if (isFocusErrorField && focusedElement.length) {
                        focusedElement.focus();
                    }
                }
                return isValidated;
            }
            $(document).on("submit", "#form-feedback", function (e) {
                e.preventDefault();
                $("#modal-feedback .ajax-error-message").empty();
                if (validateFeedbackForm(true)) {
                    $.ajax({
                        async: true,
                        type: "POST",
                        //url: siteRoot + $("#form-feedback").attr("action"),
                        url: siteRoot + "/Utility/SendFeedback",
                        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                        data: $("#form-feedback").serialize(),
                        dataType: "text",
                        success: function (data) {
                            if (data.indexOf("dm-ui-alert") === -1) {
                                if (data === "success" || data === '"success"') {
                                    clearFeedbackForm();
                                    $("#modal-feedback section.form").hide();
                                    $("#modal-feedback section.success").show();
                                    if ($("body").hasClass("wcag_focuses_on")) {
                                        $("#modal-feedback section.success button").focus();
                                    } else {
                                        $("#modal-feedback .close_icon").focus();
                                    }
                                } else {
                                    $("#modal-feedback .ajax-error-message").html('<div id="divAlertMessage" class="dm-ui-alert dm-ui-alert-error">An error occurred while processing your request. Please try again later or contact the <em>DataManager</em> Support Center.<br>1-877-246-8337 | <a href="mailto:help@riversidedatamanager.com">help@riversidedatamanager.com</a><br><em>DataManager Support Center hours are Monday - Friday from 7:00 AM - 6:00 PM (CST).</em><a href="#" class="dm-ui-alert-close" aria-label="close alert" aria-describedby="divAlertMessage">×</a></div>');
                                }
                            } else {
                                $("#modal-feedback .ajax-error-message").html(data);
                            }
                        },
                        error: function (e) {
                            $("#modal-feedback .ajax-error-message").html('<div id="divAlertMessage" class="dm-ui-alert dm-ui-alert-error">An error occurred while processing your request. Please try again later or contact the <em>DataManager</em> Support Center.<br>1-877-246-8337 | <a href="mailto:help@riversidedatamanager.com">help@riversidedatamanager.com</a><br><em>DataManager Support Center hours are Monday - Friday from 7:00 AM - 6:00 PM (CST).</em><a href="#" class="dm-ui-alert-close" aria-label="close alert" aria-describedby="divAlertMessage">×</a></div>');
                            e.preventDefault();
                        }
                    });
                }
            });
            $(document).on("input", "#feedbackFirstName, #feedbackLastName, #feedbackEmailAddress, #feedbackMessage", function (e) {
                if ($("#modal-feedback .validation-error-message").is(":visible")) {
                    validateFeedbackForm();
                }
            });

            //Elevate collapse/expand hamburger menu handler
            $(document).on("click", ".app-header-section .app-menu-top-item, .app-menu-drawer .close-header-menu-button, .app-menu-wrapper .expand-sidebar-button", function () {
                    let isMenuVisible = $(".app-header-visible").hasClass("has-visible-menu");
                    let isMenuExpanded = $(".app-header-visible").hasClass("has-expanded-menu");
                    let pageContent = $(".body-content");
                    if (isMenuVisible) {
                         pageContent.addClass("has-visible-menu");
                    } else {
                         pageContent.removeClass("has-visible-menu");
                    }
                    if (isMenuExpanded) {
                         pageContent.addClass("has-expanded-menu");
                    } else {
                         pageContent.removeClass("has-expanded-menu");
                    }
            });
        },
    };
}();


//jQuery Plugin
(function ($) {
    $.fn.GenerateRightCardTable = function (options) {
        var settings = $.extend({
            'data': "",
            'class_of_table': ""
        }, options);
        return this.each(function () {
            Dashboard.GenerateRightCardTable(this, settings.data);
        });
    };

    $.fn.GenerateRightCardTable2 = function (options) {
        var settings = $.extend({
            'data': "",
            'class_of_table': ""
        }, options);
        return this.each(function () {
            Dashboard.GenerateRightCardTable2(this, settings.data);
        });
    };

    $.fn.GenerateStanineTable = function (options) {
        var settings = $.extend({
            'data': "",
            'class_of_table': ""
        }, options);
        return this.each(function () {
            Dashboard.GenerateStanineTable(this, settings.data);
        });
    };

    $.fn.GenerateRosterTable = function (options) {
        var settings = $.extend({
            'data': "",
            'class_of_table': ""
        }, options);
        return this.each(function () {
            Dashboard.GenerateRosterTable(this, settings.data);
        });
    };

    //WCAG functions
    $.fn.makeElementNotTabbable = function () {
        this.attr("tabindex", "-1");
        this.blur();
    };
    $.fn.makeElementTabbable = function () {
        if (Dashboard.IsTabNavigationOn()) {
            var rootElement = this.parents(".root-tab-element");
            var rootTabIndex = rootElement.data("tabindex");
            if (rootTabIndex !== undefined && $("body").hasClass("wcag_focuses_on") && rootElement.attr("tabindex") === "-1") {
                this.attr("tabindex", rootTabIndex);
            }
        } else {
            this.attr("tabindex", "0");
            this.blur();
        }
    };
    $.fn.masterMakeInnerRootsTabbable = function (isFocusAtTheEnd) {
        if (Dashboard.IsTabNavigationOn()) {
            this.find("> .root-tab-hidden-content").removeAttr("aria-hidden");
            this.find(".root-tab-element").last().addClass("last-root-tab-element");
            this.find(".root-tab-element").first().addClass("first-root-tab-element");
            var elements = this.find(".root-tab-element");
            if (this.data("tabindex") !== undefined && elements.length) {
                elements.each(function () {
                    $(this).attr("tabindex", $(this).data("tabindex"));
                    //$(this).removeAttr("aria-hidden");
                });
                this.attr("tabindex", "-1");

                if (typeof isFocusAtTheEnd !== "undefined" && isFocusAtTheEnd === true) {
                    $("body").addClass("wcag_focuses_on");
                    Dashboard.AddCssClassToElementWithDelay("body", "wcag_focuses_on", 1); //for NVDA screenreader

                    //elements[0].focus();
                    this.find(".root-tab-element").first().focus();
                    //Dashboard.FocusElementWithDelay(this.find(".root-tab-element").first(), 500);
                }
            }
        }
    };
    $.fn.masterMakeInnerRootsNotTabbable = function () {
        if (Dashboard.IsTabNavigationOn()) {
            this.find("> .root-tab-hidden-content").attr("aria-hidden", "true");
            var elements = this.find(".root-tab-element");
            if (this.data("tabindex") !== undefined && elements.length) {
                this.attr("tabindex", this.data("tabindex"));
                elements.each(function () {
                    $(this).attr("tabindex", "-1");
                    //$(this).attr("aria-hidden", "true");
                });
                this.focus();
            }
        }
    };
    $.fn.rootMakeContentTabbable = function (isFocusAtTheEnd) {
        if (Dashboard.IsTabNavigationOn()) {
            if (this.data("tabindex") !== undefined && $("body").hasClass("wcag_focuses_on")) {
                this.find(".first-tab-element").removeClass("first-tab-element");
                this.find(".last-tab-element").removeClass("last-tab-element");
                this.find(".root-tab-hidden-content").removeAttr("aria-hidden");
                var elements, firstElement;
                var isFilters = false;
                var rootTabIndex = this.data("tabindex");
                //this.removeAttr("tabindex");
                this.attr("tabindex", "-1");
                //this.find(".aria-hidden").removeAttr("aria-hidden");
                elements = this.find("a, .tab");
                if (this.hasClass("filters")) {
                    isFilters = true;
                    elements = this.find("a, .tab, button, [tabindex]:not([role=alert])");
                    this.find(".dm-ui-dropdown-button").first().addClass("first-tab-element");
                    if (this.find(".dashboard-filter.button .dm-ui-button-primary").hasClass("disabled-element")) {
                        this.find(".dm-ui-dropdown-button").last().addClass("last-tab-element");
                    } else {
                        this.find(".dashboard-filter.button .dm-ui-button-primary").addClass("last-tab-element");
                    }
                }
                if (this.hasClass("right-column-card")) {
                    if (this.hasClass("overlayed")) {
                        elements = this.find(".tab:visible:not(.disabled-element)");
                    } else {
                        elements = this.find(".tab:visible, .dashboard-table th.k-header, .dashboard-table td, .dashboard-table tbody th, .dashboard-table input.k-textbox, .dashboard-table .k-widget.k-dropdown, .dashboard-table a.k-pager-nav:not(.k-state-disabled)");
                    }
                }
                if (this.hasClass("stanine-card")) {
                    elements = this.find(".tab:visible, #stanine-table-wrapper.dashboard-table th.k-header, #stanine-table-wrapper.dashboard-table td, #stanine-table-wrapper.dashboard-table tbody th, #stanine-table-wrapper.dropdown-graph-content-wrapper button, [tabindex]:not([role=alert])");
                }
                if (this.hasClass("roster-table-card")) {
                    elements = this.find(".tab:visible, #roster-table-wrapper th.k-header, #roster-table-wrapper td, #roster-table-wrapper tbody th, #roster-table-wrapper input.k-textbox, #roster-table-wrapper .k-widget.k-dropdown, #roster-table-wrapper a.k-pager-nav:not(.k-state-disabled), #roster-top-buttons-wrapper button, #roster-top-buttons-wrapper .dm-ui-dropdown-content [tabindex]:not([role=alert])");
                    //Dashboard.ClearRosterGridActiveCells();
                }
                elements.each(function () {
                    //if ($(this).attr("tabindex-important") !== "true" && $(this).attr("tabindex") !== undefined && !$(this).hasClass("disabled-element")) {
                    if ($(this).attr("tabindex-important") !== "true" && !$(this).hasClass("disabled-element") && !$(this).hasClass("k-state-disabled")) {
                        $(this).attr("tabindex", rootTabIndex);
                        if (typeof firstElement === "undefined") {
                            firstElement = $(this);
                        }
                        if (!isFilters) {
                            if (!$(this).hasClass("dm-ui-dropdown-button") && !$(this).parents(".dm-ui-dropdown-items").length && !$(this).parents(".dm-ui-dropdown-content").length && !$(this).parents(".dm-ui-dropdown-buttons-container").length) {
                                $(this).addClass("tab");
                            }
                        }
                    }
                });
                if (!isFilters) {
                    elements.first().addClass("first-tab-element");
                    elements.last().addClass("last-tab-element");
                }
                if (typeof isFocusAtTheEnd !== "undefined" && isFocusAtTheEnd === true) {
                    if ($("body").hasClass("wcag_focuses_on")) {
                        if (typeof firstElement !== "undefined" && firstElement !== null) {
                            //if (firstElement.text() !== "") {
                            //firstElement.focus();
                            Dashboard.FocusElementWithDelay(firstElement, 100);

                            //$("body").addClass("wcag_focuses_on");
                            //Dashboard.AddCssClassToElementWithDelay("body", "wcag_focuses_on", 1); //for NVDA screenreader
                            //}
                        }
                    }
                }
            }
        }
    };
    $.fn.rootMakeContentNotTabbable = function (isFocusAtTheEnd) {
        if (Dashboard.IsTabNavigationOn()) {
            var self = this;
            this.find(".first-tab-element").removeClass("first-tab-element");
            this.find(".last-tab-element").removeClass("last-tab-element");
            //if (this.data("tabindex") === undefined) {
            if (!this.hasClass("root-tab-element")) {
                var rootElement = this.parents(".root-tab-element");
                if (rootElement.length) {
                    self = rootElement;
                }
            }
            var rootTabIndex = self.data("tabindex");
            if (rootTabIndex !== undefined) {
                self.find(".root-tab-hidden-content").attr("aria-hidden", "true");
                self.attr("tabindex", rootTabIndex);
                //self.find(".aria-hidden").attr("aria-hidden", "true");
                var elements = self.find("a, .tab");
                if (self.hasClass("filters")) {
                    elements = self.find("a, .tab, button, [tabindex]:not([role=alert])");
                }
                if (self.hasClass("right-column-card")) {
                    elements = self.find(".tab:visible, .dashboard-table th.k-header, .dashboard-table td, .dashboard-table tbody th, .dashboard-table input.k-textbox, .dashboard-table .k-widget.k-dropdown, .dashboard-table a.k-pager-nav:not(.k-state-disabled)");
                }
                if (self.hasClass("stanine-card")) {
                    elements = self.find(".tab:visible, #stanine-table-wrapper.dashboard-table th.k-header, #stanine-table-wrapper.dashboard-table td, #stanine-table-wrapper.dashboard-table tbody th, #stanine-table-wrapper.dropdown-graph-content-wrapper button, [tabindex]:not([role=alert])");
                }
                if (self.hasClass("roster-table-card")) {
                    elements = self.find(".tab:visible, #roster-table-wrapper th.k-header, #roster-table-wrapper td, #roster-table-wrapper tbody th, #roster-table-wrapper input.k-textbox, #roster-table-wrapper .k-widget.k-dropdown, #roster-table-wrapper a.k-pager-nav:not(.k-state-disabled), #roster-top-buttons-wrapper button, #roster-top-buttons-wrapper .dm-ui-dropdown-content [tabindex]:not([role=alert])");
                    //Dashboard.ClearRosterGridActiveCells();
                }
                elements.each(function () {
                    //if ($(this).hasClass("k-textbox")) {
                    //    debugger;
                    //}
                    //if ($(this).attr("tabindex-important") !== "true" && $(this).attr("tabindex") !== undefined) {
                    if ($(this).attr("tabindex-important") !== "true") {
                        $(this).attr("tabindex", "-1");
                    }
                });
                if (typeof isFocusAtTheEnd !== "undefined" && isFocusAtTheEnd === true) {
                    $("body").addClass("wcag_focuses_on");
                    self.focus();
                }
            }
        }
    };
})(jQuery);


/*
function capitalizeText(text) {
    return text.toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase() });
}
function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
*/
