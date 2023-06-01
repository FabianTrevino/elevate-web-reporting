var Dashboard = function () {
    var private = {
        isAdaptive: false, //Adaptive or Single Subject Dashboard
        convertToPdfImmediately: true, //print profile narrative generates PDF or HTML
        MaxRosterShownColumnsNum: 3 + 6, //Roster Table Columns: Location or Student Name, NPR, SS + 6 Domains (max)
        RosterType: "",  //last successfully received roster type ("compare" or "students")
        RosterColumnsTypes: {}, //object for Kendo UI Grid, that store the types of columns (Numeric/String)
        EmptyDomainsColumnContent: '<div class="dm-ui-card" tabindex="3" data-tabindex="3"><div class="spinner"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div></div></div>',
        QuantilesInitialZeroData: {}, //json object with zero values to initialize Quantiles or Quartiles Chart before getting new json values
        RosterInitialZeroData: {}, //json object with zero values to initialize Roster before getting new json values
        RosterDropdownOptionsSS: "", //Roster Filters Popup options for dropdown SS
        RosterDropdownOptionsDomain: "", //Roster Filters Popup options for dropdown Domain
        RosterDropdownOptionsCondition: '<option data-alt-value="" value="eq">is equal to</option><option data-alt-value="" value="gt">is greater than</option><option data-alt-value="" value="lt">is less than</option><option data-alt-value="" value="gte">is greater than or equal to</option><option data-alt-value="" value="lte">is less than or equal to</option><option data-alt-value="" value="is_in_between" selected="selected">is in between</option>', //Roster Filters Popup options for dropdown Condition        
        modelSsNprScores: "", //last successfully received Average Standard Score (SS) & National PercentileRank (NPR) of the Average Standard Score
        modelQuartiles: "", //last successfully received Quartiles
        modelTableRoster: "", //last successfully received Roster
        modelPldCards: "", //last successfully received Pld Performance Stage Cards
        modelHierarchy: "", //last successfully Hierarchy
        modelAchievementAbilitySummary: "", //last successfully Achievement & Ability Summary data
        RosterCompareData: "",
        RosterStudentsData: "",
        RosterGridNavigationEvent: false,
        RosterGridKeyEvent: false,
        RosterGridOrderingEvent: false,
        LastFocusedElement: {},
        SearchAutocompleteFoundNum: 0,
        LastRosterTableKeyPressedCode: 0,
        isGradeK1: false,
        isCogatRoster: false,
        isCogatRosterTypeStudents: false,
        isCogatDrill: false,
        cogatRosterSelectedContentScope: "",
        cogatQuantileSecondRequest: false,
        cogatObjGrayscaleDomains: {},
        cogatSelectedQuantile: 0,
        cogatSelectedDomainId: 0,
        cogatSelectedDomainLevel: 0,
        isTabNavigationOn: false,
        defaultTabIndex: "0",
        isProdEnvironment: true
    };

    var siteRoot = "";

    if ($("#IsAdaptive").val() === "True") {
        private.isAdaptive = true;
    }
    if (private.isTabNavigationOn) {
        private.defaultTabIndex = "-1";
    }
    private.QuantilesInitialZeroData = getEmptyJsonQuantiles();
    private.RosterInitialZeroData = getEmptyJsonRoster();

    return {
        getRosterType: function () { return private.RosterType; },

        Init: function (isProdEnvironment) {
            var uiSettings = DmUiLibrary.GetUiSettings();
            siteRoot = uiSettings.SiteRoot;

            //Fix for svg href clicks: Set a split prototype on the SVGAnimatedString object which will return the split of the baseVal on this
            SVGAnimatedString.prototype.split = function () { return String.prototype.split.apply(this.baseVal, arguments); };

            this.InitDebuggingQueries(isProdEnvironment);
            this.AssignSelectGroupsHandlers();
            this.FiltersChangeHandler();
            this.AssignAllEventHandlers();

            this.GetFilters(true);

            DmUiLibrary.HideOverlaySpinner();
            $(".dm-ui-overlay-spinner-container.placed-in-block").css("display", "block");
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
                            if ($("#debug-graphql:visible").length) {
                                //$("#debug-graphql").hide("slide", { direction: "right" }, 1000);
                                $("#debug-graphql").hide();
                            } else {
                                //$("#debug-graphql").show("slide", { direction: "right" }, 1000);
                                $("#debug-graphql").show();
                            }
                        }
                    });
                    $(document).on("click", "#debug-graphql textarea", function () {
                        $("#debug-graphql textarea").blur();
                        $(this).select();
                        document.execCommand("copy");
                    });
                }
            }
        },

        AssignSelectGroupsHandlers: function () {
            $(document).on("change", ".dashboard-filter > select", function () {
                DmUiLibrary.AbortAllAjaxRequests(); //abort all Ajax requests
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
                Dashboard.ClearErrorWarningShowDashboardContent();
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
                    type: "POST",
                    url: siteRoot + "/DashboardIowaFlex/UpdateFilters",
                    data: data,
                    dataType: "json",
                    beforeSend: function () {
                        DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            self.GetFilters(true);
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
            if ($(".bread-crumbs .location-drill").length > 0) {
                isResetPageDisabled = false;
            }
            if (isResetPageDisabled) {
                if ($("#roster-name").text() === "Student Roster") {
                    isResetPageDisabled = false;
                }
            }
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
                if (!private.isTabNavigationOn) {
                    $(".dashboard-filter.button button").attr("tabindex", "-1");
                }
            } else {
                $(".dashboard-filter.button button").removeClass("disabled-element");
                if (!private.isTabNavigationOn) {
                    $(".dashboard-filter.button button").removeAttr("tabindex");
                }
            }
        },

        GetFilters: function (hideSpinnerOnComplete, isCogat) {
            Dashboard.ClearErrorWarningShowDashboardContent();

            $("#longitudinal-button").hide();
            $("#achievement-ability").hide();

            $(".pld-stage-info-card").hide();
            $(".pld-level-info-card").hide();

            if (!private.isCogatDrill && typeof isCogat === "undefined") {
                $("#flex-cogat-diagram").hide();
                $("#achievement-ability").text("Show Achievement / Ability");
                private.isCogatRoster = false;
                private.cogatSelectedQuantile = 0;
                private.cogatSelectedDomainId = 0;
                private.cogatSelectedDomainLevel = 0;
                $(".roster-table-card").removeClass("cogat-roster");
                $(".roster-table-card").removeClass("cogat-score-column-selected-4 cogat-score-column-selected-5 cogat-score-column-selected-6 cogat-score-column-selected-7 cogat-score-column-selected-8 cogat-score-column-selected-9 cogat-score-column-selected-10");
                $("#Score_Control_dm_ui").hide();
                $("#Scope_Control_dm_ui").hide();
                private.cogatRosterSelectedContentScope = "";
                Dashboard.ResetAndHideCogatRosterLabel();
            }

            if (!private.isProdEnvironment) {
                $("#debug-graphql-filters textarea").text("");
                $("#debug-graphql-filters span").text("");
            }

            var url = "/DashboardIowaFlex/GetFilters";
            if (typeof isCogat !== "undefined") {
                url += "?cogat=" + isCogat;
            }

            $.ajax({
                type: "GET",
                dataType: "html",
                url: siteRoot + url,
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        $(".filters .root-tab-hidden-content").html(data);
                        $(".filters .root-tab-hidden-content").append('<div class="dashboard-filter button"><button class="dm-ui-button-primary dm-ui-button-small">Reset Page</button></div>');
                        var breadCrumbsText = data.substr(data.indexOf('id="locations-path"'));
                        breadCrumbsText = breadCrumbsText.substr(breadCrumbsText.indexOf("value=") + 7);
                        breadCrumbsText = breadCrumbsText.substr(0, breadCrumbsText.indexOf('"'));
                        breadCrumbsText = breadCrumbsText.replace(/&quot;/g, '"');
                        var arrOfObjBreadCrumbs = JSON.parse(breadCrumbsText);
                        breadCrumbsText = "";
                        for (var i = 0; i < arrOfObjBreadCrumbs.length; ++i) {
                            if (i > 0) {
                                breadCrumbsText += " &raquo; ";
                            }
                            if (arrOfObjBreadCrumbs[i].Link) {
                                if (private.isTabNavigationOn) {
                                    breadCrumbsText += '<span class="location-drill tab" data-href="' + arrOfObjBreadCrumbs[i].Link + '" tabindex="1" data-tabindex="1" role="button">' + arrOfObjBreadCrumbs[i].Text + "</span>";
                                } else {
                                    breadCrumbsText += '<span class="location-drill tab" data-href="' + arrOfObjBreadCrumbs[i].Link + '" tabindex="0" role="button">' + arrOfObjBreadCrumbs[i].Text + "</span>";
                                }
                            } else {
                                breadCrumbsText += '<span>' + arrOfObjBreadCrumbs[i].Text + "</span>";
                            }
                        }
                        if (typeof isCogat === "undefined") {
                            $(".bread-crumbs").html(breadCrumbsText);
                        }
                        Dashboard.CheckIsResetPageButtonDisabled();

                        if (!private.isProdEnvironment) {
                            $("#debug-graphql-filters textarea").text($("#FiltersGraphqlQuery").text());
                            $("#debug-graphql-filters span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        if (private.isTabNavigationOn) {
                            $(".filters").attr("tabindex", "1");
                            $('.filters [tabindex="0"], .filters a, .filters button').each(function () {
                                $(this).attr("tabindex", "-1");
                            });
                        }

                        if ($("#Grade_Control").val() === "K" || $("#Grade_Control").val() === "1") {
                            private.isGradeK1 = 1;
                        } else {
                            private.isGradeK1 = 0;
                        }
                        Dashboard.SwitchDashboardLabelsToKto1();
                        $("#print-reports-button").hide();
                        $("#debug-graphql-hierarchy").hide();

                        if (typeof isCogat === "undefined") {
                            if (private.isGradeK1) {
                                Dashboard.GetPerformanceScoresKto1();
                                if ($("#has-differentiated-k-to-1-report").val() === "true") {
                                    $("#print-reports-button").show();
                                    $("#debug-graphql-hierarchy").show();
                                    Dashboard.GetDifferentiatedReportHierarchy();
                                }
                            } else {
                                Dashboard.GetQuantiles();
                            }
                            Dashboard.GetRoster();
                            Dashboard.GetDomains();
                            if ($("#achievement-ability").text() === "Hide Achievement / Ability") {
                                Dashboard.GetFilters(true, "1");
                            }
                        }
                    } else {
                        var errorClass = "";
                        if (data.indexOf("currently could not find any data") !== -1) {
                            $(".filters").hide();
                            Dashboard.redrawBarChartVertical4(private.QuantilesInitialZeroData, "empty");
                            Dashboard.redrawTableRoster("empty");
                        } else {
                            $(".filters").addClass("empty-json-overlay no-message");
                            Dashboard.redrawBarChartVertical4(private.QuantilesInitialZeroData, "error");
                            Dashboard.redrawTableRoster("error");
                            errorClass = " json-error";
                        }

                        $("#dashboard-right-column").empty();
                        var ariaLabelText = "Domains";
                        if (private.isGradeK1) {
                            ariaLabelText = "PLD Level Cards";
                        }
                        if (private.isTabNavigationOn) {
                            $("#dashboard-right-column").append('<div class="master-of-roots" tabindex="3" data-tabindex="3" role="region" aria-label="' + ariaLabelText + '"><div class="wrapper root-tab-hidden-content" style="display: none;"><label role="tab" class="nvda-announce" tabindex="-1" tabindex-important="true"></label></div></div>');
                        } else {
                            $("#dashboard-right-column").append('<div class="master-of-roots" role="region" aria-label="' + ariaLabelText + '"><div class="wrapper root-tab-hidden-content" style="display: none;"><label role="tab" class="nvda-announce" tabindex-important="true"></label></div></div>');
                        }
                        $("#dashboard-right-column .wrapper").fadeIn();
                        $("#dashboard-right-column").addClass("empty-json-overlay" + errorClass);

                        data = getEmptyJsonDomains();
                        for (var i = 0; i < data.length; ++i) {
                            var chartId = "blockChart3Bars-" + (i + 1);
                            var block = '<div class="dm-ui-card root-tab-element" id="blockChart3Bars-delayed-' + (i + 1) + '" style="display: none;" data-tabindex="3" role="group" aria-label="' + data[i].title + '"><div class="root-tab-hidden-content"><h2 class="domain-header">' + data[i].title + '</h2><div class="chart3bars-legend">Percent of Students</div><div id="' + chartId + '"></div></div></div>'; //delayed version
                            $("#dashboard-right-column .wrapper").append(block);
                            $("#" + chartId).BarChartVertical3({ 'data': data[0] });
                            $("#" + chartId).prepend('<span class="sr-only">Activating these buttons will cause content on the page to be updated.</span>');
                            $("#blockChart3Bars-delayed-" + (i + 1)).fadeIn();
                            $("#dashboard-right-column").addClass("empty-json-overlay");
                        }

                        $("#print-report-center").addClass("empty-json-overlay no-message");
                    }
                },
                complete: function () {
                    if (hideSpinnerOnComplete)
                        DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        ClearErrorWarningShowDashboardContent: function () {
            $(".dm-ui-page-container").show();
            $(".empty-json-overlay").removeClass("empty-json-overlay");
            $(".json-error").removeClass("json-error");
            $(".dm-ui-alert.dm-ui-alert-info").remove();
            $(".dm-ui-alert.dm-ui-alert-info").remove();
        },

        NoData: function () {
            if ($(".dm-ui-alert.dm-ui-alert-info").length === 0) {
                DmUiLibrary.DisplayAlert({ "Message": "There is no data available for the selection, please re-select your criteria and try again.", "HtmlClass": "dm-ui-alert dm-ui-alert-info", "IsDismissable": true }, "json");
                $(".dm-ui-page-container").hide();
            }
        },

        GrayscaleQuantiles: function (dataRange) {
            dataRange = "" + dataRange;
            $(".quantile-band").each(function () {
                if ($(this).attr("data-range") !== dataRange) {
                    $(this).addClass("grayscale");
                }
            });
            $("#barChart4 .rect-label").each(function () {
                if ($(this).attr("data-range") !== dataRange) {
                    $(this).addClass("grayscale");
                }
            });
            $("#roster-table-wrapper").addClass("color-event-range-" + dataRange);
        },

        GrayscaleDomains: function () {
            var parentCardId = "" + private.cogatObjGrayscaleDomains.parentCardId;
            //var parentCardNum = parentCardId.substr(parentCardId.length - 1).trim();
            var dataRange = "" + private.cogatObjGrayscaleDomains.domainLevel;
            $("#dashboard-right-column .dm-ui-card").each(function () {
                if ($(this).attr("id") !== parentCardId) {
                    $(this).addClass("grayscale");
                } else {
                    $(this).find(".domain-band").each(function () {
                        if ($(this).attr("data-range") !== dataRange) {
                            $(this).addClass("grayscale");
                        }
                    });
                }
            });
            $("#dashboard-right-column .rect-label").each(function () {
                if ($(this).parents(".dm-ui-card").attr("id") !== parentCardId) {
                    $(this).addClass("grayscale");
                } else {
                    if ($(this).attr("data-range") !== dataRange) {
                        $(this).addClass("grayscale");
                    }
                }
            });
            //$("#roster-table-wrapper").addClass("color-domain-num-" + parentCardNum);
            //$("#roster-table-wrapper").addClass("color-domain-range-" + dataRange);
            private.cogatObjGrayscaleDomains = {};
        },

        GetQuantiles: function (getAjaxData, domainId, domainLevel, performanceBand, isRedrawOnlyAverages) {
            if (typeof domainId === "undefined" || domainId === null) {
                domainId = -1;
            }
            if (typeof domainLevel === "undefined" || domainLevel === null) {
                domainLevel = -1;
            }
            if (typeof performanceBand === "undefined" || performanceBand === null) {
                performanceBand = -1;
            }
            if (typeof isRedrawOnlyAverages === "undefined" || isRedrawOnlyAverages === null) {
                isRedrawOnlyAverages = false;
            }

            if (!private.isProdEnvironment) {
                $("#debug-graphql-quantiles textarea").text("");
                $("#debug-graphql-quantiles span").text("");
                $("#debug-graphql-quantiles h4").html("Quantiles: <span></span>");
            }

            var shownModelBarChartQuartiles;
            var totalStudentsNumber = 0;
            var self = this;
            self.redrawAverageStandardScore(getEmptyJsonPieChartScore(), "init");
            self.redrawPieChartScore(getEmptyJsonPieChartScore(), "init");
            if (!isRedrawOnlyAverages) {
                self.redrawBarChartVertical4(private.QuantilesInitialZeroData, "init");
            }

            //$("body").append("typeof shownModelBarChartQuartiles=" + typeof shownModelBarChartQuartiles);
            if (getAjaxData === false && typeof shownModelBarChartQuartiles === "object") {
                if (!DmUiLibrary.DisplayResponseAlert(shownModelBarChartQuartiles, "json")) {
                    self.redrawAverageStandardScore(private.modelSsNprScores);
                    self.redrawPieChartScore(private.modelSsNprScores);
                    self.redrawBarChartVertical4(shownModelBarChartQuartiles);
                    for (var i = 0; i < shownModelBarChartQuartiles.values.length; i++) {
                        totalStudentsNumber += Number(shownModelBarChartQuartiles.values[i]["number"]);
                    }
                } else {
                    if (shownModelBarChartQuartiles.Message === "An error has occurred. (Quartiles)") {
                        self.redrawPieChartScore(getEmptyJsonPieChartScore(), "error");
                        self.redrawBarChartVertical4(private.QuantilesInitialZeroData, "error");
                    } else {
                        self.redrawPieChartScore(getEmptyJsonPieChartScore(), "empty");
                        self.redrawBarChartVertical4(private.QuantilesInitialZeroData, "empty");
                    }
                }
                $("#total-students-num-quartiles").html("Total Number: <span>0</span>");
                $("#total-students-num-quartiles span").text(totalStudentsNumber);
            } else {
                var cogat = "";
                if (private.isCogatRoster) {
                    cogat = "1";
                    if (private.cogatSelectedQuantile) performanceBand = private.cogatSelectedQuantile;
                }
                $.ajax({
                    type: "GET",
                    dataType: "json",
                    url: siteRoot + "/api/Dashboard/GetQuartiles?domainId=" + domainId + "&domainLevel=" + domainLevel + "&performanceBand=" + performanceBand + "&cogat=" + cogat,
                    success: function (data) {
                        if (!private.isProdEnvironment) {
                            $("#debug-graphql-quantiles textarea").text(data.graph_ql_query);
                            $("#debug-graphql-quantiles span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        private.modelQuartiles = data;
                        shownModelBarChartQuartiles = private.modelQuartiles;
                        //$("#average-standard-score").text(data.average_standard_score);
                        $("#test-event-subject-name").text(data.category);
                        //$("#test-event-subject-name").attr("aria-label", data.category);
                        Dashboard.initDropdownsValuesOfRosterSearchPopup(data.category);
                        if (!isRedrawOnlyAverages) {
                            private.modelSsNprScores = {
                                "national_percentile_rank": data.national_percentile_rank,
                                "average_standard_score": data.average_standard_score,
                                "values": data.values
                            };
                        }
                        if (data.nodata === true) {
                            Dashboard.NoData();
                        } else {
                            if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                                if (isRedrawOnlyAverages) {
                                    var onlyAveragesModel = {
                                        "national_percentile_rank": data.national_percentile_rank,
                                        "average_standard_score": data.average_standard_score,
                                        "values": data.values
                                    };
                                    self.redrawAverageStandardScore(onlyAveragesModel);
                                    self.redrawPieChartScore(onlyAveragesModel);
                                } else {
                                    self.redrawAverageStandardScore(private.modelSsNprScores);
                                    self.redrawPieChartScore(private.modelSsNprScores);
                                    self.redrawBarChartVertical4(shownModelBarChartQuartiles);
                                }
                                for (var i = 0; i < shownModelBarChartQuartiles.values.length; i++) {
                                    totalStudentsNumber += Number(shownModelBarChartQuartiles.values[i]["number"]);
                                }

                                if (typeof data.is_longitudinal !== "undefined") {
                                    if (data.is_longitudinal) {
                                        $("#longitudinal-button").show();
                                    }
                                }
                                if (typeof data.is_cogat !== "undefined") {
                                    if (data.is_cogat) {
                                        if (!$("#dashboard-right-column .rect-label.grayscale").length) {
                                            $("#achievement-ability").show();
                                        }
                                    }
                                }
                                if (private.cogatSelectedDomainId) {
                                    //second refresh of the top card to get correct average values
                                    if (!private.cogatQuantileSecondRequest) {
                                        private.cogatQuantileSecondRequest = true;
                                        if (private.cogatSelectedQuantile) {
                                            Dashboard.GetQuantiles(false, private.cogatSelectedDomainId, private.cogatSelectedDomainLevel, private.cogatSelectedQuantile, true);
                                        } else {
                                            Dashboard.GetQuantiles(false, private.cogatSelectedDomainId, private.cogatSelectedDomainLevel, null, true);
                                        }
                                    } else {
                                        private.cogatQuantileSecondRequest = false;
                                    }
                                    Dashboard.GrayscaleQuantiles(private.cogatSelectedQuantile);
                                    private.cogatSelectedQuantile = 0;
                                    private.cogatSelectedDomainId = 0;
                                    private.cogatSelectedDomainLevel = 0;
                                } else if (private.cogatSelectedQuantile) {
                                    Dashboard.GrayscaleQuantiles(private.cogatSelectedQuantile);
                                    private.cogatSelectedQuantile = 0;
                                    private.cogatSelectedDomainId = 0;
                                    private.cogatSelectedDomainLevel = 0;
                                }
                            } else {
                                self.redrawAverageStandardScore(getEmptyJsonPieChartScore(), "empty");
                                self.redrawPieChartScore(getEmptyJsonPieChartScore(), "empty");
                                self.redrawBarChartVertical4(private.QuantilesInitialZeroData, "empty");
                            }
                            $("#total-students-num-quartiles").html("Total Number: <span>0</span>");
                            $("#total-students-num-quartiles span").text(totalStudentsNumber);
                        }
                    },
                    complete: function (jqXhr, textStatus) {
                        if (textStatus === "error") {
                            shownModelBarChartQuartiles = private.QuantilesInitialZeroData;
                            shownModelBarChartQuartiles.AlertType = "2";
                            shownModelBarChartQuartiles.Message = "An error has occurred. (Quartiles)";

                            self.redrawAverageStandardScore(getEmptyJsonPieChartScore(), "error");
                            self.redrawPieChartScore(getEmptyJsonPieChartScore(), "error");
                            self.redrawBarChartVertical4(private.QuantilesInitialZeroData, "error");
                        }
                        $("#total-students-num-quartiles").html("Total Number: <span>0</span>");
                        $("#total-students-num-quartiles span").text(totalStudentsNumber);
                    }
                });
            }
        },

        SwitchDashboardLabelsToKto1: function () {
            if (private.isGradeK1) {
                $("#average-standard-score + .test-event-text").html("Total Number of <br> Students Selected");
                $(".quantiles-header-text").text("Percent of Students in each Performance Stage");
                $(".quantiles-header-text").attr("style", "margin-left: 100px;");
                $("#pie-chart-score").hide();
                $("#pie-chart-score + .test-event-text").hide();
                $(".carded-section.dashboard-page-section > .section-card").attr("aria-label", "Performance Stage Card");
            } else {
                $("#average-standard-score + .test-event-text").text("Average Standard Score");
                $(".quantiles-header-text").text("Percent of Students in each Performance Band");
                $(".quantiles-header-text").attr("style", "");
                $("#pie-chart-score").show();
                $("#pie-chart-score + .test-event-text").show();
                $(".carded-section.dashboard-page-section > .section-card").attr("aria-label", "Group Average Card");
            }
        },

        GetPerformanceScoresKto1: function () {
            if (!private.isProdEnvironment) {
                $("#debug-graphql-quantiles textarea").text("");
                $("#debug-graphql-quantiles span").text("");
                $("#debug-graphql-quantiles h4").html("Performance Stage Card: <span></span>");
            }

            //Dashboard.redrawBarChartVertical4(private.QuantilesInitialZeroData, "init");
            Dashboard.redrawAverageStandardScore(getEmptyJsonPieChartScore(), "init");

            $.ajax({
                type: "GET",
                dataType: "json",
                url: siteRoot + "/DashboardIowaFlex/GetPerformanceScoresKto1",
                success: function (data) {
                    if (!private.isProdEnvironment) {
                        $("#debug-graphql-quantiles textarea").text(data.graph_ql_query);
                        $("#debug-graphql-quantiles span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                    }

                    //data = getSampleJsonPerformanceScoresKto1();

                    $("#test-event-subject-name").text(data.subject);
                    if (data.nodata === true) {
                        Dashboard.NoData();
                    } else {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            private.modelQuartiles = data;
                            Dashboard.redrawAverageStandardScore({
                                "total_number_students_selected": data.totalCount
                            });
                            Dashboard.redrawBarChartVertical4(data);

                            if (typeof data.is_longitudinal !== "undefined") {
                                if (data.is_longitudinal) {
                                    $("#longitudinal-button").show();
                                }
                            }
                            if (typeof data.is_cogat !== "undefined") {
                                if (data.is_cogat) {
                                    $("#achievement-ability").show();
                                }
                            }
                        } else {
                            Dashboard.redrawBarChartVertical4(private.QuantilesInitialZeroData, "empty");
                        }
                    }
                },
                complete: function (jqXhr, textStatus) {
                    if (textStatus === "error") {
                        Dashboard.redrawBarChartVertical4(private.QuantilesInitialZeroData, "error");
                    }
                }
            });
        },

        GetDifferentiatedReportHierarchy: function () {
            if (!private.isProdEnvironment) {
                $("#debug-graphql-hierarchy textarea").text("");
                $("#debug-graphql-hierarchy span").text("");
            }
            private.modelHierarchy = {};
            $("#print-reports-button").addClass("disabled-element");
            $.ajax({
                type: "GET",
                dataType: "json",
                url: siteRoot + "/DashboardIowaFlex/GetDifferentiatedReportHierarchy",
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        if (!private.isProdEnvironment) {
                            $("#debug-graphql-hierarchy textarea").text(data.graph_ql_query);
                            $("#debug-graphql-hierarchy span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        //private.modelHierarchy = getSampleJsonDifferentiatedReportHierarchy();
                        private.modelHierarchy = data;
                        $("#print-reports-button").removeClass("disabled-element");
                    }
                }
            });
        },

        ResetAchievementAbilitySummaryCard: function () {
            $("#flex-cogat-diagram table td").text("").removeClass("data selected tab").removeAttr("tabindex");
            $("#flex-cogat-diagram").removeClass("empty-json-overlay json-error");
        },

        SelectRosterColumnByContentScope: function () {
            var rosterHeaderButton = $(".cogat-roster-header-button[data-score='" + private.cogatRosterSelectedContentScope + "']");
            var columnNumber = rosterHeaderButton.parent().index() + 1;
            $(".cogat-roster").removeClass("cogat-score-column-selected-4 cogat-score-column-selected-5 cogat-score-column-selected-6 cogat-score-column-selected-7 cogat-score-column-selected-8 cogat-score-column-selected-9 cogat-score-column-selected-10");
            if (columnNumber > 0) {
                $(".cogat-roster").addClass("cogat-score-column-selected-" + columnNumber);
                var text = $("#roster-table-wrapper table th:nth-child(" + columnNumber + ") .cogat-roster-header-button").text();
                $("#roster-table-wrapper table tr:first-child th:not(:nth-child(2)) span.sr-only").remove();
                $("#roster-table-wrapper table th:nth-child(" + columnNumber + ")").append('<span class="sr-only">Achievement & Ability Summary is filtered by ' + text + "</span>");
            }
        },

        ChangeCogatShortContentScopeNameToLong: function (contentName) {
            var result = "";
            if (contentName === "V") result = "verbal";
            if (contentName === "Q") result = "quantitative";
            if (contentName === "N") result = "nonVerbal";
            if (contentName === "VQ") result = "compVQ";
            if (contentName === "VN") result = "compVN";
            if (contentName === "QN") result = "compQN";
            if (contentName === "VQN") result = "compVQN";
            return result;
        },

        GetAchievementAbilitySummary: function (isJustCheckingCogat) {
            var url = siteRoot + "/DashboardIowaFlex/GetPerformanceLevelMatrix";
            var domainParams = "";
            var point = {}
            var arrSelectedCogatContentScope = [];
            $("#Scope_Control option:selected").each(function () {
                arrSelectedCogatContentScope.push($(this).val());
            });

            Dashboard.UnlockCogatRosterMatrixDataSelected();

            //url += "?contentType=" + $("#flex-cogat-content-type option:selected").val();
            url += "?contentType=" + $("#flex-cogat-content-type option[selected=selected]").val();

            if (private.cogatRosterSelectedContentScope === "" || $.inArray(private.cogatRosterSelectedContentScope, arrSelectedCogatContentScope) === -1) {
                if ($("#Scope_Control option[value='VQN']:selected").length) {
                    private.cogatRosterSelectedContentScope = "VQN";
                } else {
                    private.cogatRosterSelectedContentScope = $("#Scope_Control option:selected").first().val();
                }
            }
            url += "&contentName=" + Dashboard.ChangeCogatShortContentScopeNameToLong(private.cogatRosterSelectedContentScope);
            $("#roster-cogat-label-scope-value").text(private.cogatRosterSelectedContentScope);

            if ($("#barChart4 .rect-label.grayscale").length) {
                url += "&performanceBand=" + $("#barChart4 a.quantile-band.rect-label:not(.grayscale)").data("range");
            } else if (private.cogatSelectedQuantile) {
                url += "&performanceBand=" + private.cogatSelectedQuantile;
            }
            if ($("#dashboard-right-column .rect-label.grayscale").length) {
                domainParams = $("#dashboard-right-column .domain-band.rect-label:not(.grayscale)").data("adaptive-url");
                domainParams = domainParams.substr(domainParams.indexOf("domainId="));
                url += "&" + domainParams;
            } else if (private.cogatSelectedDomainId) {
                url += "&domainId=" + private.cogatSelectedDomainId + "&domainLevel=" + private.cogatSelectedDomainLevel;
            }
            Dashboard.ResetAchievementAbilitySummaryCard();
            $("#flex-cogat-diagram .spinner").show();
            if (!private.isProdEnvironment) {
                $("#debug-graphql-achievement-ability-summary textarea").text("");
                $("#debug-graphql-achievement-ability-summary span").text("");
            }
            $.ajax({
                type: "GET",
                dataType: "json",
                url: url,
                success: function (data) {
                    $("#flex-cogat-diagram .spinner").hide();
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        if (isJustCheckingCogat) {
                            if (data.dataPoints.length === 0) {
                                $("#achievement-ability").hide();
                            } else {
                                $("#achievement-ability").show();
                            }
                        }
                        if (!private.isProdEnvironment) {
                            $("#debug-graphql-achievement-ability-summary textarea").text(data.graph_ql_query);
                            $("#debug-graphql-achievement-ability-summary span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }
                        private.modelAchievementAbilitySummary = data;
                        data.dataPoints.map(function (x) {
                            point = x.abilityAchievement.split(",");
                            $("#flex-cogat-diagram table tr:nth-child(" + (6 - Number(point[1])) + ") td:nth-child(" + (1 + Number(point[0])) + ")").addClass("data tab").attr("tabindex", "0").html("<span><b>" + x.studCount + "</b></span>");
                        });
                        if (data.dataPoints.length === 0) {
                            $("#flex-cogat-diagram").addClass("empty-json-overlay");
                        }
                    } else {
                        $("#flex-cogat-diagram").addClass("empty-json-overlay json-error");
                    }
                }
            });
            //if (typeof contentName === "undefined") {
            //    Dashboard.GetRoster();
            //}
        },

        GetDomains: function (range) {
            if (!private.isProdEnvironment) {
                $("#debug-graphql-domains textarea").text("");
                $("#debug-graphql-domains span").text("");
                if (private.isGradeK1) {
                    $("#debug-graphql-domains h4").html("PLD Level Cards: <span></span>");
                } else {
                    $("#debug-graphql-domains h4").html("Domains: <span></span>");
                }
            }

            range = (typeof range === "undefined") ? "" : "?range=" + range;
            if (private.cogatSelectedQuantile) range = "?range=" + private.cogatSelectedQuantile;
            $("#dashboard-right-column").empty();

            var flagEmptyData = false;
            var block;
            var chartId;
            var cogat = (range === "") ? "?cogat=" : "&cogat=";
            if (private.isCogatRoster) {
                cogat += "1";
            }
            var url = "/api/Dashboard/GetCards" + range;
            url += cogat;
            if (private.isGradeK1) {
                url = "/DashboardIowaFlex/GetPerformanceDonutsKto1" + range;
            }

            $("#dashboard-right-column").append(private.EmptyDomainsColumnContent);
            $.ajax({
                type: "GET",
                dataType: "json",
                url: siteRoot + url,
                success: function (data) {
                    if (!private.isProdEnvironment) {
                        $("#debug-graphql-domains textarea").text(data.graph_ql_query);
                        $("#debug-graphql-domains span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                    }
                    if (data.cards !== undefined) {
                        data = data.cards;
                    }
                    $("#dashboard-right-column").empty();

                    var ariaLabelText = "Domains";
                    if (private.isGradeK1) {
                        ariaLabelText = "PLD Level Cards";
                    }
                    if (private.isTabNavigationOn) {
                        $("#dashboard-right-column").append('<div class="master-of-roots" tabindex="3" data-tabindex="3" role="region" aria-label="' + ariaLabelText + '"><div class="wrapper root-tab-hidden-content" style="display: none;"><label role="tab" class="nvda-announce" tabindex="-1" tabindex-important="true"></label></div></div>');
                    } else {
                        $("#dashboard-right-column").append('<div class="master-of-roots" role="region" aria-label="' + ariaLabelText + '"><div class="wrapper root-tab-hidden-content" style="display: none;"><label role="tab" class="nvda-announce" tabindex-important="true"></label></div></div>');
                    }
                    if (DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        data = getEmptyJsonDomains();
                        flagEmptyData = true;
                    }
                    for (var i = 0; i < data.length; ++i) {
                        if (i === 0) {
                            $("#dashboard-right-column .wrapper").fadeIn();
                            var totalStudentsNumber = 0;
                            for (var j = 0; j < data[0].values.length; j++) {
                                totalStudentsNumber += Number(data[0].values[j]["number"]);
                            }
                            $("#dashboard-right-column .wrapper").append('<span id="total-students-num-domains" aria-hidden="true">Total Number: <span>' + totalStudentsNumber + "</span></span>");
                        }
                        chartId = "blockChart3Bars-" + (i + 1);
                        var isPldStageHasStudents = false;
                        if (private.isGradeK1) {
                            private.modelPldCards = data;
                            for (var j = 0; j < data[i].values.length; ++j) {
                                if (data[i].values[j].studentCount > 0) {
                                    isPldStageHasStudents = true;
                                    break;
                                }
                            }
                            block = '<div class="dm-ui-card root-tab-element piechart" id="blockChart3Bars-delayed-' + (i + 1) + '" style="display: none;" data-tabindex="3" role="group" aria-label="' + data[i].PLDStage + '"><div class="root-tab-hidden-content"><h2 class="domain-header">' + data[i].PLDStage + '</h2><div id="' + chartId + '"></div></div></div>'; //delayed version
                        } else {
                            block = '<div class="dm-ui-card root-tab-element" id="blockChart3Bars-delayed-' + (i + 1) + '" style="display: none;" data-tabindex="3" role="group" aria-label="' + data[i].title + '"><div class="root-tab-hidden-content"><h2 class="domain-header">' + data[i].title + '</h2><div class="chart3bars-legend">Percent of Students</div><div id="' + chartId + '"></div></div></div>'; //delayed version
                        }
                        if (private.isGradeK1) {
                            if (isPldStageHasStudents) {
                                $("#dashboard-right-column .wrapper").append(block);
                            }
                        } else {
                            $("#dashboard-right-column .wrapper").append(block);
                        }
                        if (private.isGradeK1) {
                            $("#" + chartId).StagePieChart({ 'data': data[i] });
                        } else {
                            $("#" + chartId).BarChartVertical3({ 'data': data[i] });
                        }
                        $("#" + chartId).prepend('<span class="sr-only">Activating these buttons will cause content on the page to be updated.</span>');
                        setTimeout(showBlockWithDelay, 200 * i, i); //delayed version
                        //$("#" + chartId).fadeIn(); //no delay version
                    }

                    if (!$.isEmptyObject(private.cogatObjGrayscaleDomains)) {
                        Dashboard.GrayscaleDomains();
                    }
                }
            });

            function showBlockWithDelay(i) {
                $("#blockChart3Bars-delayed-" + (i + 1)).fadeIn();
                if (flagEmptyData) {
                    $("#dashboard-right-column").addClass("empty-json-overlay");
                }
            }
        },

        WcagFilterRosterByQuantile: function () {
            if ($(".bar-chart-wrapper.quantile .rect-label.grayscale").length) {
                var text = $(".bar-chart-wrapper.quantile .quantile-band").text();
                $("#roster-table-wrapper table th:nth-child(2)").append('<span class="sr-only">filtered by ' + text + "</span>");
            }
        },

        WcagFilterRosterByDomain: function (parentCardNum) {
            var text = $("#blockChart3Bars-delayed-" + parentCardNum + " h2").text() + " > " + $("#blockChart3Bars-delayed-" + parentCardNum + " div.domain-band:not(.grayscale)").text();
            $("#roster-table-wrapper table th:nth-child(" + (3 + Number(parentCardNum)) + ")").append('<span class="sr-only">(filtered by ' + text + ")</span>");
        },

        WcagFilterKto1RosterByPldStageAndLevel: function (obj) {
            if (typeof obj.pldStageName !== "undefined") {
                $("#roster-table-wrapper table th:nth-child(2)").append('<span class="sr-only">filtered by ' + obj.pldStageName + "</span>");
            }
            if (typeof obj.pldLevelNumber !== "undefined") {
                $("#roster-table-wrapper table th:last-child").append('<span class="sr-only">filtered by PLD Level ' + obj.pldLevelNumber + "</span>");
            }
        },

        GetRoster: function (getType, objQuantileClickInfo, objDomainClickInfo) {
            var isPerformanceBandDefined = false;
            var selectedPerformanceBand;

            if (!private.isProdEnvironment) {
                $("#debug-graphql-roster textarea").text("");
                $("#debug-graphql-roster span").text("");
            }

            $(".pld-stage-info-card").hide();
            $(".pld-level-info-card").hide();
            Dashboard.ResetAndHideCogatRosterLabel();

            var url;
            if (typeof getType === "undefined" || getType === null || private.isCogatRoster) {
                if (private.isCogatRoster) {
                    if ($(".bread-crumbs span").length >= 3 || private.cogatSelectedQuantile || private.cogatSelectedDomainId) {
                        private.isCogatRosterTypeStudents = true;
                    }
                    var selectedMatrixCell = $(".table-matrix td.data.selected");

                    if (private.cogatRosterSelectedContentScope !== "" && $("#Scope_Control option[value='" + private.cogatRosterSelectedContentScope + "']:selected").length === 0) {
                        Dashboard.GetAchievementAbilitySummary();
                    }
                    if (private.isCogatDrill) {
                        Dashboard.GetAchievementAbilitySummary();
                        private.isCogatDrill = false;
                    }

                    url = siteRoot + "/DashboardIowaFlex/";
                    if (private.isCogatRosterTypeStudents) {
                        url += "GetCogatStudentRoster";
                        $("#roster-cogat-label-roster-type").text("students");
                    } else {
                        url += "GetCogatLocationRoster";
                        if ($(".bread-crumbs span").length === 1) {
                            $("#roster-cogat-label-roster-type").text("buildings");
                        } else {
                            $("#roster-cogat-label-roster-type").text("classes");
                        }
                    }
                    //var cogatScoreValue = $("#flex-cogat-content-type").val();
                    var cogatScoreValue = $("#Score_Control option:selected").val();
                    url += "?cogatScore=" + cogatScoreValue;
                    $("#roster-cogat-label-score-value").text(cogatScoreValue);
                    $("#roster-cogat-label-scope-value").text(private.cogatRosterSelectedContentScope);
                    
                    if ($("#Scope_Control_dm_ui > button").hasClass("disabled-element")) {
                        url += "&contentName=" + Dashboard.ChangeCogatShortContentScopeNameToLong(private.cogatRosterSelectedContentScope);
                    }

                    if ($("#barChart4 .rect-label.grayscale").length) {
                        selectedPerformanceBand = $("#barChart4 a.quantile-band.rect-label:not(.grayscale)").data("range");
                        url += "&performanceBand=" + selectedPerformanceBand;
                        isPerformanceBandDefined = true;
                    } else if (private.cogatSelectedQuantile) {
                        url += "&performanceBand=" + private.cogatSelectedQuantile;
                    }
                    if ($("#dashboard-right-column .rect-label.grayscale").length) {
                        var domainParams = $("#dashboard-right-column .domain-band.rect-label:not(.grayscale)").data("adaptive-url");
                        domainParams = domainParams.substr(domainParams.indexOf("domainId="));
                        url += "&" + domainParams;
                    } else if (private.cogatSelectedDomainId) {
                        url += "&domainId=" + private.cogatSelectedDomainId + "&domainLevel=" + private.cogatSelectedDomainLevel;
                    }
                    if (selectedMatrixCell.length === 1) {
                        url += "&cogatAbility=" + selectedMatrixCell.data("ability");
                        if (!isPerformanceBandDefined) {
                            url += "&performanceBand=" + selectedMatrixCell.data("achievement");
                        }
                        $("#roster-cogat-label-achievement-value").text("(" + selectedMatrixCell.siblings("th").text().toLowerCase() + ")");
                        $("#roster-cogat-label-ability-value").text("(" + $("#iowa-cogat-column" + selectedMatrixCell.data("ability")).text().toLowerCase() + ")");
                    }
                } else if (private.isGradeK1) {
                    url = siteRoot + "/DashboardIowaFlex/GetRosterKto1";
                    if (typeof objDomainClickInfo !== "undefined" && objDomainClickInfo !== null) {
                        if (typeof objDomainClickInfo.pldStageName !== "undefined" && objDomainClickInfo.pldStageName !== "") {
                            url += "?pldStage=" + objDomainClickInfo.pldStageName;
                        }
                        if (typeof objDomainClickInfo.pldLevelNumber !== "undefined" && objDomainClickInfo.pldLevelNumber !== "") {
                            if (url.indexOf("DashboardIowaFlex/GetRosterKto1?") === -1) {
                                url += "?pldLevel=" + objDomainClickInfo.pldLevelNumber;
                            } else {
                                url += "&pldLevel=" + objDomainClickInfo.pldLevelNumber;
                            }
                        }
                    }
                } else {
                    url = siteRoot + "/api/Dashboard/GetRoster";
                    private.cogatSelectedQuantile = 0;
                    private.cogatSelectedDomainId = 0;
                    private.cogatSelectedDomainLevel = 0;
                }
            } else {
                if (getType.indexOf("/api/") !== -1) {
                    url = getType;
                    if (getType.indexOf("domainId=") === -1) {
                        url += "&domainId=-1";
                    }
                    if (getType.indexOf("domainLevel=") === -1) {
                        url += "&domainLevel=-1";
                    }
                    if (getType.indexOf("performanceBand=") === -1) {
                        url += "&performanceBand=-1";
                    }
                } else {
                    url = siteRoot + "/api/Dashboard/GetRoster?" + getType;
                }
            }

            var self = this;
            $("#roster-table-wrapper").removeClass();
            $("#roster-name").html("");
            //$(".section-card.roster-table-card").attr("aria-label", "");
            $("#roster-legend-wrapper").html("");
            $(".section-card.roster-table-card").removeClass("empty-json-overlay");
            self.redrawTableRoster("init");

            $("#roster-top-info-wrapper").addClass("hidden");
            $.ajax({
                type: "GET",
                dataType: "json",
                url: url,
                success: function (data) {
                    if (!private.isProdEnvironment) {
                        $("#debug-graphql-roster textarea").text(data.graph_ql_query);
                        $("#debug-graphql-roster span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                    }

                    $("#roster-top-info-wrapper").removeClass("hidden");
                    $("#roster-selected-quantile-label").css("opacity", 1);
                    private.modelTableRoster = data;
                    if (data.nodata === true) {
                        Dashboard.NoData();
                    } else {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType) && typeof data.values !== "undefined") {
                            private.RosterType = data.roster_type;
                            //Dashboard.CalculateRosterFontResizingForNumberValues
                            Dashboard.resetInputValuesOfRosterSearchPopup();

                            if (private.RosterType === "compare") {
                                private.RosterCompareData = data;
                            }

                            $("#roster-table-wrapper").empty();
                            $("#roster-table-wrapper").GenerateRosterTable({
                                'data': data,
                                'class_of_table': "dm-ui-table"
                            });
                            $("#roster-table-wrapper table").fadeIn();

                            if (typeof objQuantileClickInfo !== "undefined" && objQuantileClickInfo !== null) {
                                $("#roster-selected-quantile-label").show();
                                $("#roster-table-wrapper").addClass("color-event-range-" + objQuantileClickInfo.dataRange);
                                Dashboard.WcagFilterRosterByQuantile();
                            }

                            if (typeof objDomainClickInfo !== "undefined" && objDomainClickInfo !== null) {
                                $("#roster-selected-domain-label").show();
                                if (private.isGradeK1) {
                                    Dashboard.WcagFilterKto1RosterByPldStageAndLevel(objDomainClickInfo);
                                } else {
                                    if (!private.isCogatRoster) {
                                        $("#roster-table-wrapper").addClass("color-domain-num-" + objDomainClickInfo.parentCardNum);
                                        $("#roster-table-wrapper").addClass("color-domain-range-" + objDomainClickInfo.dataRange);
                                        Dashboard.WcagFilterRosterByDomain(objDomainClickInfo.parentCardNum);
                                        if (objDomainClickInfo.dataRangeBandNpr !== "") {
                                            $("#roster-table-wrapper").addClass("color-event-range-" + objDomainClickInfo.dataRangeBandNpr);
                                        }
                                    }
                                    Dashboard.WcagFilterRosterByQuantile();
                                }
                            }

                            if (private.isGradeK1) {
                                if (data.performance_level_descriptor === null && data.performance_level_statement === null) {
                                    Dashboard.redrawAverageStandardScore({
                                        "total_number_students_selected": private.modelQuartiles.totalCount
                                    });
                                }
                                if (typeof data.performance_level_descriptor !== "undefined" && data.performance_level_descriptor !== null && data.performance_level_descriptor !== "") {
                                    Dashboard.changePldStageCard(data.performance_level_descriptor);
                                }
                                if (typeof data.performance_level_statement !== "undefined" && data.performance_level_statement !== null && data.performance_level_statement !== "") {
                                    Dashboard.changePldLevelCard(data.performance_level_statement);
                                }
                            }
                        } else {
                            self.redrawTableRoster("empty");
                            //$("#print-report-center").addClass("empty-json-overlay no-message");
                        }
                        var focusedElement = $(":focus");
                        $("#roster-table-wrapper .k-grid-header tr:first-child th:first-child a.k-link").click();
                        if (focusedElement.length) {
                            focusedElement.focus();
                        } else {
                            $(".main-header a").first().focus();
                        }
                        $(".section-card.roster-table-card").rootMakeContentNotTabbable();
                        //$(".main-menu-reports.current-page .has-sub-menu").first().focus(); //focus on the menu Reports
                        $("#roster-table-wrapper > table").removeAttr("tabindex"); //if not removed -kendo table will be the last focused element on page
                        $("#roster-table-wrapper > table").removeAttr("role");
                        $("#roster-table-wrapper > table thead th[data-role=columnsorter] a").attr("role", "button");
                        setTimeout(function () {
                            $("#roster-table-wrapper > table td, #roster-table-wrapper > table tbody th").removeAttr("role");
                            if (private.RosterType === "compare") {
                                $("#roster-table-wrapper > table thead tr:first-child th:not(:first-child):not(:nth-child(2)):not(:nth-child(3))").attr("scope", "colgroup");
                            }
                        }, 1000);
                        $("#roster-table-wrapper .k-pager-sizes span.k-input").attr("aria-hidden", "true");
                        Dashboard.FocusLastFocusedElement();
                        Dashboard.CheckIsResetPageButtonDisabled();
                        if (private.isCogatRoster) {
                            Dashboard.SelectRosterColumnByContentScope();
                            if ($("#roster-selected-quantile-label").text() !== "") {
                                $("#roster-selected-cogat-label").addClass("shifted-right1");
                            }
                            if ($("#roster-selected-domain-label").text() !== "") {
                                $("#roster-selected-cogat-label").addClass("shifted-right2");
                            }
                            $("#roster-selected-cogat-label").show();
                            var isMatrixSquareSelected = $(".table-matrix td.data.selected").length;
                            if (isMatrixSquareSelected) {
                                $(".cogat-roster-header-button").addClass("disabled");
                            }
                            if (isPerformanceBandDefined) {
                                $("#roster-table-wrapper").addClass("color-event-range-" + selectedPerformanceBand);
                                Dashboard.WcagFilterRosterByQuantile();
                            }
                        }
                        private.isCogatRosterTypeStudents = false;
                    }
                },
                complete: function (jqXhr, textStatus) {
                    if (textStatus === "error") {
                        private.modelTableRoster = private.RosterInitialZeroData;
                        private.modelTableRoster.AlertType = "2";
                        private.modelTableRoster.Message = "An error has occurred. (Roster)";
                        self.redrawTableRoster("error");
                    }
                }
            });
        },

        setGridCellCurrentState: function (element) {
            /*
                        var innerElementAriaLabelValue = element.find(".kendo-grid-cell").attr("aria-label");
                        console.log("innerElementAriaLabelValue=" + innerElementAriaLabelValue);
                        element.attr("aria-label", innerElementAriaLabelValue);
                        element.find(".kendo-grid-cell").removeAttr("aria-label");
                        element.removeAttr("role");
            */
            if (element !== undefined && element !== null && private.isTabNavigationOn) {
                var grid = $("#roster-table-wrapper").data("kendoGrid");
                grid.current(element);
                element.removeAttr("role");
            }
        },

        GenerateRosterTable: function (element, data) {
            var rosterType = data.roster_type;
            var rosterColumns = [];
            var filterableType = false;

            if (typeof rosterType != "undefined") {
                $("#roster-table-wrapper").removeClass("roster-students");
                $("#roster-table-wrapper").addClass("roster-" + rosterType);

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

                if (rosterType === "compare") {
                    //private.RosterCompareData = data;
                    if ($("#roster-switcher-2:checked").val() === "Number") {
                        rosterColumns = Dashboard.GenerateKendoRosterColumnStructure(data.roster_type, data.columns, "Number");
                        $("#roster-table-wrapper").addClass("font-resizer");
                    } else {
                        $("#roster-table-wrapper").removeClass("font-resizer");
                        rosterColumns = Dashboard.GenerateKendoRosterColumnStructure(data.roster_type, data.columns, "Percent");
                    }

                }

                if (rosterType === "students") {
                    private.RosterStudentsData = data;
                    rosterColumns = Dashboard.GenerateKendoRosterColumnStructure(private.RosterStudentsData.roster_type, private.RosterStudentsData.columns);
                }
            } else {
                var studentNameTemplate;
                if (private.isTabNavigationOn) {
                    studentNameTemplate = "<span data-href='#:link#' class='location-drill tab' tabindex='-1' role='button'>#:node_name#</span>";
                } else {
                    studentNameTemplate = "<span data-href='#:link#' class='location-drill tab' tabindex='0' role='button'>#:node_name#</span>";
                }

                rosterColumns = [
                    /*
                    //column for Kendo checkboxes
                    {
                        selectable: true,
                        width: "36px"
                    },
                    */
                    {
                        field: "node_name",
                        headerTemplate: "Location",
                        template: studentNameTemplate
                    },
                    {
                        field: "SS", filterable: false
                    },
                    {
                        field: "NPR", filterable: false
                    }
                ];
            }

            try {
                var self = this;
                $(element).kendoGrid({
                    dataSource: {
                        data: data.values,
                        schema: {
                            model: {
                                id: "node_id",
                                //fields: {
                                //    node_name: { type: "string" },
                                //    link: { type: "string" },
                                //    M: { type: "number" }
                                //}
                                fields: private.RosterColumnsTypes
                            }
                        },
                        pageSize: 25
                    },

                    resizable: true,
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
                    //dataBound: onDataBound(rosterType),
                    dataBound: function () {
                        $("#roster-table-wrapper > table tbody tr td:first-child").attr("scope", "row");
                        $("#roster-table-wrapper > table tbody tr td:first-child").each(function () {
                            $(this).replaceWith($(this)[0].outerHTML.replace("<td ", "<th ").replace("</td>", "</th>"));
                        });
                        if (!private.isTabNavigationOn) {
                            $("#roster-table-wrapper > table thead tr th[data-role=\"columnsorter\"]").addClass("tab").attr("tabindex", "0");
                        }
                        setTimeout(function () {
                            Dashboard.WcagPagination("#roster-table-wrapper");
                        }, 100);
                    },
                    sortable: {
                        mode: "single",
                        //mode: "multiple",
                        showIndexes: true,
                        allowUnsort: true
                    },
                    //filterable: false,
                    filterable: filterableType,
                    pageable: {
                        input: true,
                        numeric: false,
                        alwaysVisible: true,
                        pageSizes: [5, 10, 25, 50],
                        //pageSizes: [5, 10, 25, 50, 100, 1000],
                        change: function () {
                            //console.log("pager change event");
                            Dashboard.WcagPagination("#roster-table-wrapper");
                        }
                    },
                    sort: function () {
                        //$('.kendo_sorted_column').addClass("grid-sort-filter-icon k-icon");
                    },
                    //editable: true,
                    navigatable: true,
                    navigate: function (e) {
                        if (private.isTabNavigationOn) {
                            Dashboard.setGridCellCurrentState(e.element);
                            $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                            e.element.focus();
                            private.RosterGridNavigationEvent = true;
                        }
                    },
                    /*
                    noRecords: {
                        template: self.noRecordMsg(),
                    },
                    */
                    columns: rosterColumns
                });
                Dashboard.moveOutRosterFilters(rosterType);
                Dashboard.insertTooltipsToHeaderOfRosterTable(data.columns);
                if (data.roster_type === "compare" && data.values.length <= 1) {
                    //$("#roster-top-buttons-wrapper").hide();
                } else {
                    //$("#roster-top-buttons-wrapper").show();
                }
                if (private.isGradeK1) {
                    $("#roster-filters-popup-button").hide();
                } else {
                    $("#roster-filters-popup-button").show();
                }
                Dashboard.FocusLastFocusedElement(private.LastFocusedElement);
            } catch (e) {
                //debugger;
                $("body").append("Error Table Roster=" + e.name + ": " + e.message + " | " + e.stack);
            }
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

        clearK1PerformanceAndDonutsCardSelections: function () {
            $(".pld-performance-stage, .rect-label").removeClass("grayscale");

            $("#dashboard-right-column .piechart").removeClass("grayscale");
            $("#dashboard-right-column .piechart .piechart-link").removeClass("grayscale");
            $("#dashboard-right-column .piechart .piechart-text").removeClass("grayscale");
            $("#dashboard-right-column .piechart .piechart-polyline").removeClass("grayscale");
            $("#roster-selected-domain-label").empty();

            $("#roster-selected-domain-label").empty();
        },

        clearQuantileSelection: function () {
            $(".quantile-band").removeClass("grayscale");
            $("#barChart4 .rect-label").removeClass("grayscale");
            $("#roster-table-wrapper").removeClass("color-event-range-1 color-event-range-2 color-event-range-3 color-event-range-4 color-event-range-5");
            $("#roster-selected-quantile-label").removeAttr("data-range");
            this.clearDomainSelection();
        },

        clearDomainSelection: function () {
            if (private.isGradeK1) {
                this.clearK1PerformanceAndDonutsCardSelections();
            } else {
                $("#dashboard-right-column .dm-ui-card").removeClass("grayscale");
                $("#dashboard-right-column .dm-ui-card .domain-band").removeClass("grayscale");
                $("#dashboard-right-column .rect-label").removeClass("grayscale");
                $("#roster-table-wrapper").removeClass("color-domain-num-1 color-domain-num-2 color-domain-num-3 color-domain-num-4 color-domain-num-5 color-domain-num-6 color-domain-num-7");
                $("#roster-table-wrapper").removeClass("color-domain-range-1 color-domain-range-2 color-domain-range-3");
                $("#roster-selected-domain-label").empty();
            }
        },

        changeRosterLabelNPR: function (dataRangeBand, labelName, dataRange) {
            if (private.isTabNavigationOn) {
                $("#roster-selected-quantile-label").html('<div class="tab" tabindex="4" aria-label="' + labelName.replace("-", " through ").replace(" (", ", ").replace(")", "") + '"><div class="arrow"></div> ' + labelName + '<div class="close tab" tabindex="4" aria-label="Clear Standard Score band filter" role="button">X</div></div>');
            } else {
                $("#roster-selected-quantile-label").html('<div class="tab" aria-label="' + labelName.replace("-", " through ").replace(" (", ", ").replace(")", "") + '"><div class="arrow"></div> ' + labelName + '<div class="close tab" tabindex="0" aria-label="Clear Standard Score band filter" role="button">X</div></div>');
            }

            $("#roster-selected-quantile-label").hide();
            $("#roster-selected-quantile-label").attr("data-range", dataRange);
        },

        applyRosterPopupFilters: function () {
            var objAllFilters = {};
            var objRowFilter, arrRowFilter;
            var field, operator, value, value2, obj, obj2, arr;
            var rowNumber;
            var range1, range2;
            var booleanRowOperator;

            $(".table-report-filters tr:not(.filter-row-disabled) td.column-score select").each(function (i) {
                objRowFilter = new Object();
                arrRowFilter = [];

                rowNumber = $(this).attr("id");
                rowNumber = rowNumber.substr(rowNumber.length - 1).trim();

                operator = $("#dropdown-condition-" + rowNumber).val();
                value = Number($("#input-score-" + rowNumber + "-val1").val());
                value2 = Number($("#input-score-" + rowNumber + "-val2").val());

                booleanRowOperator = $("#dropdown-boolean-" + rowNumber).val();
                if (booleanRowOperator === "") {
                    booleanRowOperator = "and";
                }
                objRowFilter["logic"] = booleanRowOperator;
                obj = new Object();

                if ($(this).prop("disabled", false) && ($(this).val() === "NationalPercentileRank" || $(this).val() === "StandardScore")) {
                    if ($(this).val() === "NationalPercentileRank") {
                        field = "NPR";
                    } else {
                        field = "SS";
                    }
                    if (operator === "is_in_between") {
                        obj["logic"] = "and";
                        arr = [];

                        obj2 = new Object();
                        obj2["field"] = field;
                        obj2["operator"] = "gte";
                        obj2["value"] = value;
                        arr.push(obj2);

                        obj2 = new Object();
                        obj2["field"] = field;
                        obj2["operator"] = "lte";
                        obj2["value"] = value2;
                        arr.push(obj2);

                        obj["filters"] = arr;
                    } else {
                        obj["field"] = field;
                        obj["operator"] = operator;
                        obj["value"] = value;
                    }
                }

                if ($(this).prop("disabled", false) && $(this).val() === "DomainPerformanceLevel") {
                    field = $("#dropdown-content-area-" + rowNumber).val();

                    //Low: 1:36
                    //Mid: 37:63
                    //High: 64:99
                    var arrRanges = [{ "From": 1, "To": 36 }, { "From": 37, "To": 63 }, { "From": 64, "To": 99 }];

                    range1 = Number($("#dropdown-score-range-" + rowNumber).val());
                    range2 = Number($("#dropdown-score-range-" + rowNumber + "-val2").val());

                    if (operator === "is_in_between") {
                        /*
                                                objNewFilter["operator"] = "gte";
                                                objNewFilter["value"] = arrRanges[range1 - 1]["From"];
                                                arrAppliedFilters.push(objNewFilter);
                        
                                                objNewFilter2["field"] = field;
                                                objNewFilter2["operator"] = "lte";
                                                objNewFilter2["value"] = arrRanges[range2 - 1]["To"];
                                                //objNewFilter2["logic"] = "and";
                                                arrAppliedFilters.push(objNewFilter2);
                        */
                    } else {
                        switch (operator) {
                            case "eq":
                                obj["logic"] = "and";
                                arr = [];

                                obj2 = new Object();
                                obj2["field"] = field;
                                obj2["operator"] = "gte";
                                obj2["value"] = arrRanges[range1 - 1]["From"];
                                arr.push(obj2);

                                obj2 = new Object();
                                obj2["field"] = field;
                                obj2["operator"] = "lte";
                                obj2["value"] = arrRanges[range1 - 1]["To"];
                                arr.push(obj2);

                                obj["filters"] = arr;
                                break;
                            case "gt":
                                obj["field"] = field;
                                obj["operator"] = operator;
                                obj["value"] = arrRanges[range1 - 1]["To"];
                                break;
                            case "lt":
                                obj["field"] = field;
                                obj["operator"] = operator;
                                obj["value"] = arrRanges[range1 - 1]["From"];
                                break;
                            case "gte":
                                obj["field"] = field;
                                obj["operator"] = operator;
                                obj["value"] = arrRanges[range1 - 1]["From"];
                                break;
                            case "lte":
                                obj["field"] = field;
                                obj["operator"] = operator;
                                obj["value"] = arrRanges[range1 - 1]["To"];
                                break;
                            //default:
                                //console.log("UNKNOWN CONDITION!");
                        }
                    }
                }

                arrRowFilter.push(obj);
                objRowFilter["filters"] = arrRowFilter;


                if (i === 0) {
                    objAllFilters = objRowFilter;
                }
                if (i === 1) {
                    objAllFilters["filters"].push(objRowFilter);
                }
                if (i === 2) {
                    objAllFilters["filters"][1]["filters"].push(objRowFilter);
                }

            });
            //console.log("AppliedFilters::::::::::::::::");
            //console.log(objAllFilters);
            $("#roster-table-wrapper").data("kendoGrid").dataSource.filter(objAllFilters);

            return false;
        },

        changeRosterLabelSkillNCE: function (dataRange, parentCardId) {
            var domainLabelBegin = $("#" + parentCardId + " .domain-header").text();
            var domainLabelEnd = "";
            if (dataRange === "1") {
                domainLabelEnd = "Low Performers";
            }
            if (dataRange === "2") {
                domainLabelEnd = "Mid Performers";
            }
            if (dataRange === "3") {
                domainLabelEnd = "High Performers";
            }
            if (private.isTabNavigationOn) {
                $("#roster-selected-domain-label").html('<div class="tab" tabindex="4" aria-label="' + domainLabelBegin + ", " + domainLabelEnd + '"><div class="arrow"></div> ' + domainLabelBegin + " > " + domainLabelEnd + '<div class="close tab" tabindex="4" aria-label="Clear Domain band filter" role="button">X</div></div>');
            } else {
                $("#roster-selected-domain-label").html('<div class="tab" aria-label="' + domainLabelBegin + ", " + domainLabelEnd + '"><div class="arrow"></div> ' + domainLabelBegin + " > " + domainLabelEnd + '<div class="close tab" tabindex="0" aria-label="Clear Domain band filter" role="button">X</div></div>');
            }
            $("#roster-selected-quantile-label").css("opacity", 0);
            $("#roster-selected-domain-label").hide();
        },

        changeRosterLabelPldStageLevel: function (stage, level, number, percent) {
            var labelText = "";
            var closeIconText = "Clear PLD filter";
            if (level !== "") {
                labelText = " — " + level;
                closeIconText = "Clear PLD Level filter";
            }
            var multiEnd = ""; 
            if (number > 1) {
                multiEnd = "s";
            }

            var labelHtml;
            if (private.isTabNavigationOn) {
                labelHtml = '<div class="multistring" tabindex="4" aria-label="' + stage + labelText + ". " + number + " Student" + multiEnd + " Selected — " + percent + '%"><div class="arrow"></div> <b>' + stage + labelText + "</b><br><b>" + number + "</b> Student" + multiEnd + " Selected — <b>" + percent + '%</b><div class="close tab" tabindex="4" aria-label="' + closeIconText + '" role="button">X</div></div>';
            } else {
                labelHtml = '<div class="multistring" aria-label="' + stage + labelText + ". " + number + " Student" + multiEnd + " Selected — " + percent + '%"><div class="arrow"></div> <b>' + stage + labelText + "</b><br><b>" + number + "</b> Student" + multiEnd + " Selected — <b>" + percent + '%</b><div class="close tab" tabindex="0" aria-label="' + closeIconText + '" role="button">X</div></div>';            }
            $("#roster-selected-domain-label").html(labelHtml);
            $("#roster-selected-quantile-label").css("opacity", 0);
            $("#roster-selected-domain-label").hide();
        },

        changePldStageCard: function (dataObj) {
            var stageNumber = 0;
            var imgSrc;

            if (dataObj.pldName === "Pre-Emerging") stageNumber = 1;
            if (dataObj.pldName === "Emerging") stageNumber = 2;
            if (dataObj.pldName === "Beginning") stageNumber = 3;
            if (dataObj.pldName === "Transitioning") stageNumber = 4;
            if (dataObj.pldName === "Independent") stageNumber = 5;
            imgSrc = $(".pld-stage-info-card img").attr("src");
            imgSrc = imgSrc.substr(0, imgSrc.indexOf(".png") - 1) + stageNumber + ".png";
            $(".pld-stage-info-card img").attr("src", imgSrc);

            $(".pld-stage-info-card h2").text(dataObj.pldAltName);
            $(".pld-stage-info-card p").html(dataObj.pldDesc);

            $(".pld-stage-info-card").show();
        },

        changePldLevelCard: function (dataObj) {
            var counter = 0;
            var arr;
            var columnClass = ".left-column";

            if (dataObj.canStmt !== "") {
                counter++;
                $(".pld-level-info-card .left-column h2").text(dataObj.iCanDesc);
                $(".pld-level-info-card .left-column ul").empty();
                arr = dataObj.canStmt.split("\n");
                arr.forEach(function (item, i) {
                    $(".pld-level-info-card .left-column ul").append("<li>" + item + "</li>");
                });
            }

            if (counter) {
                columnClass = ".right-column";
            }
            if (dataObj.readyStmt !== "") {
                $(".pld-level-info-card " + columnClass + " h2").text(dataObj.readyDesc);
                $(".pld-level-info-card " + columnClass + " ul").empty();
                arr = dataObj.readyStmt.split("\n");
                arr.forEach(function (item, i) {
                    $(".pld-level-info-card " + columnClass + " ul").append("<li>" + item + "</li>");
                });
            }

            if (dataObj.practiceStmt !== "") {
                $(".pld-level-info-card .right-column h2").text(dataObj.needDesc);
                $(".pld-level-info-card .right-column ul").empty();
                arr = dataObj.practiceStmt.split("\n");
                arr.forEach(function (item, i) {
                    $(".pld-level-info-card .right-column ul").append("<li>" + item + "</li>");
                });
            }

            $(".pld-level-info-card").show();
        },

        ClearAllErrorsMessages: function () {
            //remove all errors messages and empty data overlays
            $(".empty-json-overlay").removeClass("empty-json-overlay");
            $(".json-error").removeClass("json-error");
            $(".dm-ui-alert.dm-ui-alert-info").remove();
            $(".undefined").remove();
            $(".debug-warning").empty();
            $("#roster-selected-quantile-label").empty();
            $("#roster-selected-domain-label").empty();
            //$("#roster-top-buttons-wrapper").empty();
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
            }
        },

        redrawPieChartScore: function (data, viewType) {
            this.addViewElements("#pie-chart-score", viewType);
            $("#pie-chart-score").PieChartScore({
                'data': data,
                'radius': 40
            });
        },

        redrawAverageStandardScore: function (data, viewType) {
            this.addViewElements("#average-standard-score", viewType);
            $("#average-standard-score").AverageStandardScore({
                'data': data
            });
        },

        redrawBarChartVertical4: function (data, viewType) {
            var columnsNumber = data.values.length;
            this.addViewElements("#barChart4", viewType);
            $("#barChart4").BarChartVertical4({
                'data': data,
                'columns_number': columnsNumber
            });
            $("#barChart4").prepend('<span class="sr-only">Activating these buttons will cause content on the page to be updated.</span>');
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

        RefreshAverageStandardScoreAndNPR: function (range) {
            if (typeof range === "undefined") {
                range = 0;
            }
            Dashboard.redrawAverageStandardScore(getEmptyJsonPieChartScore(), "init");
            Dashboard.redrawPieChartScore(getEmptyJsonPieChartScore(), "init");
            if (range === 0) {
                Dashboard.redrawAverageStandardScore(private.modelSsNprScores);
                Dashboard.redrawPieChartScore(private.modelSsNprScores);
            } else {
                for (var i = 0; i < private.modelSsNprScores.values.length; i++) {
                    if (Number(private.modelSsNprScores.values[i]["range"]) === Number(range)) {
                        Dashboard.redrawAverageStandardScore(private.modelSsNprScores.values[i]);
                        Dashboard.redrawPieChartScore(private.modelSsNprScores.values[i]);
                        break;
                    }
                }
            }
        },

        CalculateRosterFontResizingForNumberValues: function () {
            //count font size of table for switcher Number position
            var maxRow = 0, numRow, index, numOfExtraDigits;
            for (var i = 0; i < private.modelTableRoster.values.length; i++) {
                numRow = 0;
                index = 0;
                var values = private.modelTableRoster.values[i];
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

        GenerateKendoRosterColumnStructure: function (rosterType, rosterStructure, valuesType) {
            if (typeof valuesType === "undefined") {
                valuesType = "Percent";
            }

            var kendoColumns = [];

            var firstDomainColumnIndexInRoster = 1;
            if (private.isAdaptive) {
                firstDomainColumnIndexInRoster = 2;
            }
            var kendoColumn, multiColumn, tmpColumn, tmpValue;
            var column;
            var arr;
            var index = 0;
            var key;
            var i;
            var legend;
            var arrLegendShort = [];
            var arrLegendFull = [];
            var rosterTitle = "";
            var ariaRangeTitle;
            private.RosterColumnsTypes = {
                node_name: { type: "string" },
                link: { type: "string" }
            }


            var arrSelectedCogatContentScope = [];
            if (private.isCogatRoster) {
                $("#Scope_Control option:selected").each(function () {
                    arrSelectedCogatContentScope.push($(this).val());
                });
            }

            if (rosterType === "students") {
                $("#print-student-profile").show();
                if (private.isTabNavigationOn) {
                    $("#print-dashboard").removeClass("last-tab-element");
                }
                $("#roster-switcher-wrapper").addClass("hidden");

                //kendoColumns.push({ selectable: true, width: "36px" });
                //kendoColumns.push({ field: "NPR", filterable: false }); //show NPR column for Debug purpose
                if (!private.isAdaptive) {
                    kendoColumns.push({ selectable: true, field: "node_id", filterable: false, sortable: false });  //hidden column with checkboxes
                }

                for (key in rosterStructure) {
                    if (rosterStructure.hasOwnProperty(key)) {
                        index++;
                        if (index >= private.MaxRosterShownColumnsNum && !private.isCogatRoster) {
                            break;
                        }
                        column = rosterStructure[key];
                        kendoColumn = {};

                        if (private.isCogatRoster && index >= 4) {
                            if ($.inArray(column.title, arrSelectedCogatContentScope) === -1) {
                                continue;
                            }
                        }

                        rosterTitle = "Student Roster";

                        if (column["multi"]) {
                            if (index === 2 && !private.isAdaptive) {
                                kendoColumn["field"] = "NPR";
                            } else {
                                if (private.isAdaptive) {
                                    kendoColumn["field"] = column["fields"][0];
                                } else {
                                    kendoColumn["field"] = column["fields"][1];
                                    kendoColumn["field"] = kendoColumn["field"].replace("_score_1", "_SKILL_NCE");
                                }
                            }
                        } else {
                            kendoColumn["field"] = column["field"];
                        }

                        if (index === 2 && !private.isAdaptive) {
                            kendoColumn["title"] = "NPR";
                        } else {
                            if (private.isCogatRoster) {
                                kendoColumn["title"] = column["title"];
                            } else {
                                if (column["title"].length > 1) {
                                    kendoColumn["title"] = column["title"];
                                } else {
                                    kendoColumn["title"] = column["title_full"].substr(0, 3);
                                }
                            }
                        }

                        if (index > 1) {
                            kendoColumn["filterable"] = false;
                        } else {
                            kendoColumn["filterable"] = {
                                cell: {
                                    suggestionOperator: "contains"
                                }
                            };
/*
                            kendoColumn["filterable"] = {ui: function (element) {element.kendoNumericTextBox({format: "n0",decimals: 0});}};
                            kendoColumn["filterable"] = {
                                format: "{0:MM/dd/yyyy HH:mm tt}",
                                ui: "datetimepicker"
                            };
                            kendoColumn["filterable"] = {
                                ui: function (element) {
                                    element.kendoNumericTextBox({
                                        format: "n0"
                                    });
                                }
                            }
*/

                        }
                        if (index >= 2 && index <= 10) {
                            if (private.isGradeK1 && column["field"] === "PLDS0") {
                                private.RosterColumnsTypes[kendoColumn["field"]] = { type: "string" }
                            } else {
                                private.RosterColumnsTypes[kendoColumn["field"]] = { type: "number" }
                            }
                        }

                        if (typeof column["field"] != "undefined") {
                            if (column["field"] === "node_name") {
                                if (private.isCogatRoster) {
                                    kendoColumn["template"] = "<span data-name='Student' data-value='#:node_name#' data-node-id='#:node_id#' data-npr='#:NPR#' data-ss='#:SS#' class='student-link tab no-spacebar-scrolling' tabindex='" + private.defaultTabIndex + "' aria-label='#:node_name#' role='button'>#:node_name#</span>";
                                } else if (private.isGradeK1) {
                                    kendoColumn["template"] = "<span data-name='Student' data-value='#:node_name#' data-node-id='#:node_id#' data-pld-stage='#:PLDS0#' data-pld-level='#:PLDL0#' class='student-link tab no-spacebar-scrolling' tabindex='" + private.defaultTabIndex + "' aria-label='#:node_name#' role='button'>#:node_name#</span>";
                                } else {
                                    kendoColumn["template"] = "<span data-name='Student' data-value='#:node_name#' data-node-id='#:node_id#' data-npr='#:NPR#' data-ss='#:SS#' class='student-link tab no-spacebar-scrolling' tabindex='" + private.defaultTabIndex + "' aria-label='#:node_name#' role='button'>#:node_name#</span>";
                                }
                            }

                            if (private.isCogatRoster && (column["field"] === "v" || column["field"] === "q" || column["field"] === "n" || column["field"] === "vq" || column["field"] === "vn" || column["field"] === "qn" || column["field"] === "vqn")) {
                                kendoColumn["filterable"] = false;
                                kendoColumn["headerTemplate"] = '<span class="cogat-roster-header-button tab" data-score="' + column["title"] + '" tabindex="0" role="button">' + column["title"] + "</span>";
                                tmpValue = column["field"];
                                kendoColumn["columns"] = [
                                    {
                                        "filterable": false,
                                        //"title": "APR",
                                        "title": $("#Score_Control option:selected").val(),
                                        //"template": "#:" + column["field"] +"#",
                                        "template": "#= (" + tmpValue + " == null) ? '*' : " + tmpValue + " #",
                                        "field": column["field"]
                                    }
                                ];
                            } else {
                                if (column["field"] === "NPR" || column["field"] === "SS") {
                                    if (private.isAdaptive) {
                                        //kendoColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + ": #:" + column.field + "#'>#:" + column.field + "#</span>";
                                        kendoColumn["template"] = "#:" + column.field + "#";
                                    }
                                }
                            }
                        }

                        if (typeof column["fields"] != "undefined") {
                            if (index === 2) {
                                kendoColumn["template"] = "#:NPR#";
                            }
                            if (index > 2) {
                                //kendoColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + ": #:" + column.fields[0].replace("_score_0", "_SKILL_NCE") + "#'>#:" + column.fields[0].replace("_score_0", "_SKILL_NCE") + "#</span>";
                                //kendoColumn["template"] = "#:" + column.fields[0].replace("_score_0", "_SKILL_NCE") + "#";
                                tmpValue = column.fields[0].replace("_score_0", "_SKILL_NCE");
                                kendoColumn["template"] = "#= (" + tmpValue + " == null) ? '*' : " + tmpValue + " #";
                            }
                        }

                        //kendoColumn["format"] = "{0:c}";
                        //kendoColumn["format"] = "{0:0.00}";
                        //kendoColumn["format"] = "{0:#,##}";
                        //kendoColumn["format"] = "{0:p}";
                        //kendoColumn["format"] = "n0";
                        //kendoColumn["format"] = "n";
                        //kendoColumn["decimals"] = 3;
                        kendoColumns.push(kendoColumn);

                        if (index >= 2) {
                            arrLegendShort.push(kendoColumn["title"]);
                            arrLegendFull.push(column["title_full"]);
                        }
                    }
                }
                //console.log(kendoColumns);

                //generate LEGEND under the Roster Table (students roster type)
                $("#roster-name").html(rosterTitle);
                //$("#roster-name").attr("aria-label", rosterTitle);
                $(".section-card.roster-table-card").attr("aria-label", "Students Comparison Table");

                legend = '<h3 class="roster-legend-header">Legend</h3>';
                legend += '<dl class="legend">';
                if (private.isGradeK1) {
                    legend += "<dt>PLD:</dt><dd>Performance Level Descriptor</dd>";
                } else {
                    legend += "<dt>SS:</dt><dd>Standard Score</dd>";
                    legend += "<dt>NPR:</dt><dd>National Percentile Rating</dd>";
                    for (i = firstDomainColumnIndexInRoster; i < arrLegendShort.length; i++) {
                        legend += "<dt>" + arrLegendShort[i] + ":</dt><dd>" + arrLegendFull[i] + "</dd>";
                    }
                    if (private.isCogatRoster) {
                        legend += "<dt>" + $("#Score_Control option:selected").val() + ":</dt><dd>" + $("#Score_Control option:selected").text() + "</dd>";
                    } else {
                        legend += "<dt>*</dt><dd>value means that the domain was not taken</dd>";
                    }
                }
                legend += "</dl>";
                $("#roster-legend-wrapper").html(legend);
            }


            if (rosterType === "compare") {
                $("#print-student-profile").hide();
                if (private.isTabNavigationOn) {
                    $("#print-dashboard").addClass("last-tab-element");
                }
                $("#roster-switcher-wrapper").removeClass("hidden");

                for (key in rosterStructure) {
                    if (rosterStructure.hasOwnProperty(key)) {
                        index++;
                        if (index >= private.MaxRosterShownColumnsNum && !private.isCogatRoster) {
                            break;
                        }
                        column = rosterStructure[key];
                        kendoColumn = {};

                        if (private.isCogatRoster && index >= 4) {
                            if ($.inArray(column.title, arrSelectedCogatContentScope) === -1) {
                                continue;
                            }
                        }

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

                        if (column["multi"]) {
                            multiColumn = [];
                            if (valuesType === "Percent") {
                                arr = column["fields_per"];
                            } else {
                                arr = column["fields_num"];
                            }
                            arr.sort();
                            //arr = column["fields_num"];
                            for (i = 0; i < arr.length; i++) {
                                if (i === 3) {
                                    if (arr.length > 4) {
                                        $("#roster-debug-warning").html("Roster multi columns structure is wrong! (<strong>" + arr.length + " columns</strong>)");
                                    }
                                    break;
                                }
                                tmpColumn = {};
                                tmpColumn["filterable"] = false;
                                if (i === 0) {
                                    tmpColumn["title"] = "L";
                                    ariaRangeTitle = "Low";
                                    if (valuesType === "Percent") {
                                        //tmpColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + " (Low): #:" + column.fields_per[0] + "#'>#:" + column.fields_per[0] + "#</span>";
                                        tmpColumn["template"] = "#:" + column.fields_per[0] + "#";
                                    } else {
                                        //tmpColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + " (Low): #:" + column.fields_num[0] + "#'>#:" + column.fields_num[0] + "#</span>";
                                        tmpColumn["template"] = "#:" + column.fields_num[0] + "#";
                                    }
                                }
                                if (i === 1) {
                                    tmpColumn["title"] = "M";
                                    ariaRangeTitle = "Middle";
                                    if (valuesType === "Percent") {
                                        //tmpColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + " (Middle): #:" + column.fields_per[1] + "#'>#:" + column.fields_per[1] + "#</span>";
                                        tmpColumn["template"] = "#:" + column.fields_per[1] + "#";
                                    } else {
                                        //tmpColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + " (Middle): #:" + column.fields_num[1] + "#'>#:" + column.fields_num[1] + "#</span>";
                                        tmpColumn["template"] = "#:" + column.fields_num[1] + "#";
                                    }
                                }
                                if (i === 2) {
                                    tmpColumn["title"] = "H";
                                    ariaRangeTitle = "High";
                                    if (valuesType === "Percent") {
                                        //tmpColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + " (High): #:" + column.fields_per[2] + "#'>#:" + column.fields_per[2] + "#</span>";
                                        tmpColumn["template"] = "#:" + column.fields_per[2] + "#";
                                    } else {
                                        //tmpColumn["template"] = "<span class='kendo-grid-cell' aria-label='" + column["title_full"] + " (High): #:" + column.fields_num[2] + "#'>#:" + column.fields_num[2] + "#</span>";
                                        tmpColumn["template"] = "#:" + column.fields_num[2] + "#";
                                    }
                                }
                                tmpColumn["field"] = arr[i];
                                multiColumn.push(tmpColumn);
                            }
                            kendoColumn["columns"] = multiColumn;
                        } else {
                            if (private.isCogatRoster && (column["field"] === "v" || column["field"] === "q" || column["field"] === "n" || column["field"] === "vq" || column["field"] === "vn" || column["field"] === "qn" || column["field"] === "vqn")) {
                                kendoColumn["filterable"] = false;
                                kendoColumn["headerTemplate"] = '<span class="cogat-roster-header-button tab" data-score="' + column["title"] + '" tabindex="0" role="button">' + column["title"] + "</span>";
                                tmpValue = column["field"];
                                kendoColumn["columns"] = [
                                    {
                                        "filterable": false,
                                        //"title": "APR",
                                        "title": $("#Score_Control option:selected").val(),
                                        //"template": "#:" + column["field"] + "#",
                                        "template": "#= (" + tmpValue + " == null) ? '*' : " + tmpValue + " #",
                                        "field": column["field"]
                                    }
                                ];
                            } else {
                                kendoColumn["field"] = column["field"];
                                kendoColumn["filterable"] = {
                                    cell: {
                                        suggestionOperator: "contains"
                                    }
                                };
                            }
                        }

                        if (private.isCogatRoster) {
                            kendoColumn["title"] = column["title"];
                        } else {
                            if (column["title"].length > 1) {
                                kendoColumn["title"] = column["title"];
                            } else {
                                kendoColumn["title"] = column["title_full"].substr(0, 3);
                            }
                        }

                        if (index > 1) {
                            kendoColumn["filterable"] = false;
                        }
                        if (index === 2 || index === 3) {
                            private.RosterColumnsTypes[kendoColumn["field"]] = { type: "number" }
                        }

                        if (typeof column["field"] != "undefined") {
                            if (column["field"] === "node_name") {
                                kendoColumn["template"] = "<span data-href='#:link#' class='location-drill tab' tabindex='" + private.defaultTabIndex + "' tabindex-important='true' role='button'>#:node_name#</span>";
                            } else {
                                kendoColumn["template"] = "#:" + column["field"] + "#";
                            }
                        }

                        if (column["multi"]) {
                            kendoColumn["headerTemplate"] = kendoColumn["title"];
                        }
                        kendoColumns.push(kendoColumn);

                        if (index >= 2) {
                            arrLegendShort.push(kendoColumn["title"]);
                            arrLegendFull.push(column["title_full"]);
                        }
                    }
                }

                //generate LEGEND under the Roster Table (compare roster type)
                $("#roster-name").html(rosterTitle);
                //$("#roster-name").attr("aria-label", rosterTitle);
                $(".section-card.roster-table-card").attr("aria-label", rosterTitle.replace("Score ", "") + " Table");

                if (private.isGradeK1) {
                    legend = "";
                } else {
                    legend = '<h3 class="roster-legend-header">Legend</h3>';
                    legend += '<dl class="legend">';
                    legend += "<dt>SS:</dt><dd>Average Standard Score for the group</dd>";
                    legend += "<dt>NPR:</dt><dd>National Percentile Rank for the group</dd>";
                    if (!private.isCogatRoster) {
                        if (valuesType === "Percent") {
                            legend += "<dt>Low (L) / Mid (M) / High (H):</dt><dd>Percent of students in each domain performance band</dd>";
                        } else {
                            legend += "<dt>Low (L) / Mid (M) / High (H):</dt><dd>Number of students in each domain performance band</dd>";
                        }
                    }
                    for (i = firstDomainColumnIndexInRoster; i < arrLegendShort.length; i++) {
                        legend += "<dt>" + arrLegendShort[i] + ":</dt><dd>" + arrLegendFull[i] + "</dd>";
                    }
                    if (private.isCogatRoster) {
                        legend += "<dt>" + $("#Score_Control option:selected").val() + ":</dt><dd>" + $("#Score_Control option:selected").text() + "</dd>";
                    }
                    legend += "</dl>";
                }
                $("#roster-legend-wrapper").html(legend);
            }
            //console.log(kendoColumns);
            return kendoColumns;
        },

        insertTooltipsToHeaderOfRosterTable: function (rosterStructure) {
            var arrTooltips = [];
            var column;
            var cardIndex = 0;
            for (var key in rosterStructure) {
                if (rosterStructure.hasOwnProperty(key)) {
                    cardIndex++;
                    if (cardIndex >= private.MaxRosterShownColumnsNum && !private.isCogatRoster) {
                        break;
                    }
                    if (!private.isAdaptive && private.RosterType === "students" && cardIndex === 2) {
                        arrTooltips.push("National Percentile Rank");
                    } else {
                        column = rosterStructure[key];
                        if (column["title_full"] !== column["title"]) {
                            arrTooltips.push(column["title_full"]);
                            //arr_legend_short = column["title"];
                        } else {
                            arrTooltips.push("");
                        }
                    }
                }
            }

            var i = 0;
            $("#roster-table-wrapper table thead tr:first-child th").each(function () {
                if (typeof arrTooltips[i] != "undefined") {
                    if (arrTooltips[i] !== "") {
                        $(this).addClass("tooltip");
                        $(this).append('<span class="tooltiptext" role="tooltip">' + arrTooltips[i] + "</span>");
                    }
                }
                i++;
            });
            if (private.isCogatRoster) {
                $("#roster-table-wrapper table thead tr:nth-child(2) th").each(function () {
                    $(this).addClass("tooltip");
                    $(this).append('<span class="tooltiptext" role="tooltip">' + $("#Score_Control option:selected").text() + "</span>");
                });
            }
        },

        resetInputValuesOfRosterSearchPopup: function () {
            $("#modal-dashboard-report-criteria .dm-ui-alert-error").remove();
            $("#input-score-1-val1, #input-score-1-val2, #input-score-2-val1, #input-score-2-val2, #input-score-3-val1, #input-score-3-val2").val("");
        },

        initDropdownsValuesOfRosterSearchPopup: function (subject) {
            var nameTestEvent = subject;
            /*
            if (private.RosterType === "students") {
                nameTestEvent = private.RosterStudentsData.columns[1]["title_full
            } else {
                nameTestEvent = private.RosterCompareData.columns[1]["title_full"];
            }*/
            private.RosterDropdownOptionsSS = '<option value="' + nameTestEvent + '">' + nameTestEvent + "</option>";
            $("#dropdown-content-area-1").empty();
            $("#dropdown-content-area-1").append(private.RosterDropdownOptionsSS);
            $("#dropdown-content-area-1").trigger("DmUi:updated");

            private.RosterDropdownOptionsDomain = "";
            /*
            var domainTitle, domainField;
            for (var i = 2; i < private.RosterStudentsData.columns.length; i++) {
                if (i > 7) break;
                domainTitle = private.RosterStudentsData.columns[i]["title_full"];
                domainField = private.RosterStudentsData.columns[i]["fields"][1];
                domainField = domainField.replace("_score_1", "_SKILL_NCE");
                private.RosterDropdownOptionsDomain += '<option value="' + domainField + '">' + domainTitle + "</option>";
            }
            */
            $("#dropdown-content-area-2").empty();
            //$("#dropdown-content-area-2").append(private.RosterDropdownOptionsDomain);
            $("#dropdown-content-area-2").append(private.RosterDropdownOptionsSS);
            $("#dropdown-content-area-2").trigger("DmUi:updated");
            $("#dropdown-content-area-3").empty();
            //$("#dropdown-content-area-3").append(private.RosterDropdownOptionsDomain);
            $("#dropdown-content-area-3").append(private.RosterDropdownOptionsSS);
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
            if ($("#roster-table-wrapper .k-grid-header .k-filter-row").length) {
                $("#roster-search-field").empty();
                $("#roster-search-field").append($("#roster-table-wrapper .k-grid-header .k-filter-row th .k-filtercell"));

                $("#roster-search-field input.k-input").each(function (i) {
                    if ($(this).attr("aria-label") != null) {
                        if (private.RosterType === "students") {
                            if (i === 0) {
                                fieldFullTitle = private.RosterStudentsData.columns[i]["title_full"];
                            }
                            if (i > 0) {
                                fieldFullTitle = private.RosterStudentsData.columns[i / 2]["title_full"] + " score";
                            }
                        } else {
                            if (i === 0) {
                                fieldFullTitle = private.RosterCompareData.columns[i]["title_full"];
                            }
                            if (i === 2) {
                                fieldFullTitle = private.RosterCompareData.columns[i - 1]["title_full"] + " score";
                            }
                        }
                        if (i === 0) {
                            if (private.isAdaptive) {
                                $(this).attr("placeholder", "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name");
                                $(this).attr("aria-label", "Search");
                                $(this).attr("id", "roster-search");
                                $(this).attr("aria-describedby", "wcag-autocomplete-search-instructions");
                                $("#roster-search-field").prepend('<label class="floating-label" id="roster-search-label" for="roster-search">' + "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name" + '</label>');
                                $(this).prev("input").attr("placeholder", "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name");
                            } else {
                                $(this).attr("placeholder", "Enter a " + fieldFullTitle);
                                $(this).prev("input").attr("placeholder", "Enter a " + fieldFullTitle);
                            }
                        }
                        $(this).attr("data-full-title", fieldFullTitle);
                        $(this).prev("input").attr("data-full-title", fieldFullTitle);

                        if (private.isTabNavigationOn) {
                            $(this).attr("tabindex", $(this).parents(".root-tab-element").data("tabindex")); //add tabindex for search field
                            $(this).parents(".k-filtercell").find(".k-dropdown-operator").attr("tabindex", $(this).parents(".root-tab-element").data("tabindex")); //add tabindex for search criteria icon
                        }
                        $(this).addClass("tab");
                        $(this).parents(".k-filtercell").find(".k-dropdown-operator").addClass("tab");
                    }
                });
                var resetRosterButtonDisabledClass = "disabled-element";
                var tabIndexResetButton = "-1";
                if ($("#roster-selected-quantile-label .arrow").length > 0 || $("#roster-selected-domain-label .arrow").length > 0) {
                    resetRosterButtonDisabledClass = "";
                    if (!private.isTabNavigationOn) {
                        tabIndexResetButton = "0";
                    }
                }
                if ($("#modal-dashboard-print-students").is(":visible")) {
                    $("#modal-dashboard-print-students").fadeOut("fast", function () { });
                }
                //$("#roster-top-buttons-wrapper").append('<span id="roster-filters-popup-button" class="tab no-spacebar-scrolling" tabindex="' + private.defaultTabIndex + '" aria-label="Select Report Criteria" role="button"></span>');
                //$("#roster-top-buttons-wrapper").append('<span id="roster-reset-button" class="dm-ui-button-primary dm-ui-button-small tab no-spacebar-scrolling ' + resetRosterButtonDisabledClass + '" tabindex="' + tabIndexResetButton + '" title="Reset" role="button"></span>');
                $("#roster-filters-popup-button").attr("tabindex", private.defaultTabIndex);
                $("#roster-reset-button").attr("tabindex", tabIndexResetButton);
                if (resetRosterButtonDisabledClass === "disabled-element") {
                    $("#roster-reset-button").addClass("disabled-element");
                } else {
                    $("#roster-reset-button").removeClass("disabled-element");
                }

                //clear empty filter cells
                $("#roster-search-field .k-filter-row th").each(function () {
                    if (!$(this).text().trim()) {
                        $(this).remove();
                    }
                });
            }
        },

        ClearRosterGridActiveCells: function () {
            var activeCellElements = $("#roster-table-wrapper_active_cell");
            if (activeCellElements.length) {
                //console.log("REMOVE active cell:");
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
                if (private.LastFocusedElement[0].id === "roster-filters-popup-button") {
                    $(".roster-table-card").rootMakeContentTabbable();
                    $("#roster-filters-popup-button").focus();
                } else {
                    private.LastFocusedElement.focus();
                    //$("body").addClass("wcag_focuses_on");
                }
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
        },

        PostPdfProfileNarrative: function (params) {
            var newPopupTarget = "print_preview_popup" + DmUiLibrary.RandomStringGenerator(5);
            var formId = "#print_profile_narrative_form";
            if (private.isGradeK1) formId = "#print_profile_narrative_form_k1";
            params = params.split("=");
            $(formId).empty();
            $(formId).append("<input type=\"hidden\" name=\"" + params[0] + '" value="' + params[1] + '">');
            $(formId).attr("target", newPopupTarget);
            if (private.isGradeK1) {
                window.open($(formId).attr("action"), newPopupTarget, "width=1000,height=700,left=0,top=5");
            } else {
                window.open($(formId).attr("action"), newPopupTarget, "width=1340,height=700,left=0,top=5");
            }
            $(formId).submit();
        },

        ToggleShowOrHideCogatMatrixCard: function (hideMatrixCard) {
            if (typeof hideMatrixCard === "undefined") {
                hideMatrixCard = false;
            }
            private.cogatSelectedQuantile = 0;
            private.cogatSelectedDomainId = 0;
            private.cogatSelectedDomainLevel = 0;

            if (!hideMatrixCard && $("#achievement-ability").text() === "Show Achievement / Ability") {
                if ($("#barChart4 .rect-label.grayscale").length) {
                    private.cogatSelectedQuantile = $("#barChart4 a.quantile-band.rect-label:not(.grayscale)").data("range");
                    if (private.cogatSelectedQuantile === "undefined") {
                        private.cogatSelectedQuantile = 0;
                    }
                } else {
                    private.cogatSelectedQuantile = 0;
                }

                if ($("#dashboard-right-column .rect-label.grayscale").length) {
                    var element = $("#dashboard-right-column .domain-band.rect-label:not(.grayscale)");
                    var parentCardId = element.parents(".dm-ui-card").attr("id");
                    var domainParams = element.data("adaptive-url");
                    domainParams = domainParams.substr(domainParams.indexOf("domainId="));
                    if (typeof private.cogatSelectedQuantile === "undefined") {
                        private.cogatSelectedQuantile = 0;
                    }

                    domainParams = domainParams.substr(domainParams.indexOf("domainId=") + 9);
                    private.cogatSelectedDomainId = domainParams.substr(0, domainParams.indexOf("&"));
                    private.cogatSelectedDomainLevel = domainParams.substr(domainParams.indexOf("domainLevel=") + 12);

                    private.cogatObjGrayscaleDomains = { parentCardId: parentCardId, domainId: private.cogatSelectedDomainId, domainLevel: private.cogatSelectedDomainLevel};
                } else {
                    private.cogatSelectedDomainId = 0;
                    private.cogatSelectedDomainLevel = 0;
                }

                //$("#roster-selected-quantile-label").empty();
                //$("#roster-selected-domain-label").empty();
                //Dashboard.clearDomainSelection();
                //Dashboard.clearQuantileSelection();

                $("#achievement-ability").text("Hide Achievement / Ability");
                private.isCogatRoster = true;
                if ($(".bread-crumbs span").length >= 3) {
                    private.isCogatRosterTypeStudents = true;
                }
                $(".roster-table-card").addClass("cogat-roster");
                $("#Score_Control_dm_ui").show();
                $("#Scope_Control_dm_ui").show();
                $("#flex-cogat-diagram").show();
                $("#debug-graphql-achievement-ability-summary").show();
                Dashboard.GetFilters(true, "1");
                Dashboard.GetAchievementAbilitySummary();
            } else {
                $("#achievement-ability").text("Show Achievement / Ability");
                private.isCogatRoster = false;
                $(".roster-table-card").removeClass("cogat-roster");
                $(".roster-table-card").removeClass("cogat-score-column-selected-4 cogat-score-column-selected-5 cogat-score-column-selected-6 cogat-score-column-selected-7 cogat-score-column-selected-8 cogat-score-column-selected-9 cogat-score-column-selected-10");
                $("#Score_Control_dm_ui").hide();
                $("#Scope_Control_dm_ui").hide();
                $("#flex-cogat-diagram").hide();
                Dashboard.ResetAndHideCogatRosterLabel();
                $("#debug-graphql-achievement-ability-summary").hide();
                //Dashboard.GetRoster();
                Dashboard.GetFilters(true, "");
            }
            Dashboard.GetQuantiles();
            Dashboard.GetDomains();
        },

        ResetAndHideCogatRosterLabel: function () {
            $("#roster-selected-cogat-label").hide();
            $("#roster-selected-cogat-label").removeClass("shifted-right1 shifted-right2");
            $("#roster-cogat-label-achievement-value, #roster-cogat-label-ability-value, #roster-cogat-label-scope-value, #roster-cogat-label-score-value").text("");
        },

        LockCogatRosterMatrixDataSelected: function () {
            $(".cogat-roster-header-button").addClass("disabled");
            //$("#Scope_Control_dm_ui > button").addClass("disabled-element");
            //$("#Score_Control_dm_ui > button").addClass("disabled-element");
            $("#flex-cogat-content-type_dm_ui > button").addClass("disabled-element");

            var cogatScoreValueRoster = $("#Score_Control option:selected").val();
            //var cogatScoreValueMatrix = $("#flex-cogat-content-type option:selected").val();
            var cogatScoreValueMatrix = $("#flex-cogat-content-type option[selected=selected]").val();
            if (cogatScoreValueRoster !== cogatScoreValueMatrix) {
                $("#Score_Control").val(cogatScoreValueMatrix).trigger("DmUi:updated");
            }
        },

        UnlockCogatRosterMatrixDataSelected: function () {
            $(".cogat-roster-header-button").removeClass("disabled");
            //$("#Scope_Control_dm_ui > button").removeClass("disabled-element");
            //$("#Score_Control_dm_ui > button").removeClass("disabled-element");
            $("#flex-cogat-content-type_dm_ui > button").removeClass("disabled-element");
        },

        AssignAllEventHandlers: function () {
            //***** Print Dashboard ***** 
            $(document).on("click touchstart", "#print-dashboard", function (e) {
                e.preventDefault();
                printDashboard();
            });
            $(document).on("keyup", "#print-dashboard", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    printDashboard();
                }
            });
            function printDashboard() {
                var isRosterSearchValueEntered = false;
                var inputRosterSearch = $("#roster-top-buttons-wrapper span.k-filtercell input.k-input");
                if (inputRosterSearch.val()) {
                    isRosterSearchValueEntered = true;
                    inputRosterSearch.attr("value", inputRosterSearch.val());
                    inputRosterSearch.addClass("value-entered");
                }
                $("#roster-table-wrapper input.k-textbox").attr("value", $("#roster-table-wrapper input.k-textbox").val());
                $("#input").val($(".body-content").html().replace(/href=/g, "data-href=")); //prevent hyperlinks in pdf
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


            //***** Print Students *****
            $(document).on("click touchstart", "#print-student-profile", function (e) {
                appendToPopupAllRosterStudents();
                $("#modal-dashboard-print-students").fadeIn("fast");
                $("#modal-dashboard-print-students .first-tab-element").focus();
                //$("#modal-dashboard-print-students .modal-header").focus();
                //$("#check-all-students-modal").parent().focus();
            });
            $(document).on("keydown", "#print-student-profile", function (e) {
                if (e.keyCode === 13 || e.which === 32) {
                    e.preventDefault();
                    $("#print-student-profile").click();
                }
            });
            $(document).on("click touchstart", "#modal-dashboard-print-students .close_icon, #cancel-dashboard-print-students", function (e) {
                $("#modal-dashboard-print-students").fadeOut("fast");
                $("#print-student-profile").focus();
            });
            $(document).on("keydown", "#modal-dashboard-print-students .close_icon, #cancel-dashboard-print-students", function (e) {
                if (e.keyCode === 13 || e.which === 32) {
                    e.preventDefault();
                    $("#cancel-dashboard-print-students").click();
                }
            });
            $(document).on("keyup", "#modal-dashboard-print-students .close_icon, #cancel-dashboard-print-students", function (e) {
                e.preventDefault(); //FF tab key fix
            });
            $(document).on("keyup", "#modal-dashboard-print-students", function (e) {
                if (e.keyCode === 27) {
                    $("#cancel-dashboard-print-students").click();
                }
            });
            $(document).on("change", "#check-all-students-modal", function () {
                if ($(this).prop("checked")) {
                    $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                        if (!$(this).prop("checked")) {
                            $(this).parents(".dm-ui-checkbox").attr("aria-checked", "true");
                            $(this).prop("checked", true);
                        }
                    });
                    $("#cancel-dashboard-print-students").removeClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-print-students").addClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-print-students").removeClass("disabled-element");
                } else {
                    $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                        if ($(this).prop("checked")) {
                            $(this).parents(".dm-ui-checkbox").attr("aria-checked", "false");
                            $(this).prop("checked", false);
                        }
                    });
                    $("#cancel-dashboard-print-students").addClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-print-students").removeClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-print-students").addClass("disabled-element");
                }
            });
            $(document).on("change", "#modal-dashboard-print-students .students-wrapper input[type=checkbox]", function () {
                var isAllStudentsSelected = true;
                var isNoneStudentsSelected = true;
                $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                    if (!$(this).prop("checked")) {
                        isAllStudentsSelected = false;
                    }
                    if ($(this).prop("checked")) {
                        isNoneStudentsSelected = false;
                    }
                });

                if (isAllStudentsSelected) {
                    $("#check-all-students-modal").parents(".dm-ui-checkbox").attr("aria-checked", "true");
                    $("#check-all-students-modal").prop("checked", true);
                } else {
                    $("#check-all-students-modal").parents(".dm-ui-checkbox").attr("aria-checked", "false");
                    $("#check-all-students-modal").prop("checked", false);
                }

                if (isNoneStudentsSelected) {
                    $("#cancel-dashboard-print-students").addClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-print-students").removeClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-print-students").addClass("disabled-element");
                } else {
                    $("#cancel-dashboard-print-students").removeClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-print-students").addClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-print-students").removeClass("disabled-element");
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
            function appendToPopupAllRosterStudents() {
                $("#modal-dashboard-print-students .students-wrapper").empty();
                var dataSource = $("#roster-table-wrapper").data("kendoGrid").dataSource;
                var filters = dataSource.filter();
                var allData = dataSource.data();
                var query = new kendo.data.Query(allData);
                var filteredDataAllPages = query.filter(filters).data;
                var container = $("#modal-dashboard-print-students .students-wrapper");
                var numTableColumns = 4, i = 0, tmp;
                var isAllStudentsSelected = true;
                var arrSelectedStudentsId = [];
                var rowsNumber = 0;
                filteredDataAllPages.map(function (x) {
                    arrSelectedStudentsId.push(x.node_id);
                });
                tmp = '<table border="0" cellpadding="0" cellspacing="0" role="presentation">';
                var arrStudents = $("#roster-table-wrapper").data("kendoGrid").dataSource.data();
                arrStudents.sort(compareNames);
                arrStudents.map(function (x) {
                    i++;
                    if (i % (numTableColumns) === 1) {
                        tmp += "<tr>";
                    }
                    if ($.inArray(x.node_id, arrSelectedStudentsId) === -1) {
                        tmp += '<td data-col="' + (i - 1) % (numTableColumns) + '"><div aria-checked="false" class="dm-ui-checkbox dm-ui-menuitem-checkbox" role="checkbox" tabindex="0"><input id="checkbox' + x.node_id + '" name="checkbox' + x.node_id + '" type="checkbox" data-node-id="' + x.node_id + '" data-grid-id="' + (i - 1) + '"><label for="checkbox' + x.node_id + '">' + x.node_name + " </label></div></td>";
                        isAllStudentsSelected = false;
                    } else {
                        tmp += '<td data-col="' + (i - 1) % (numTableColumns) + '"><div aria-checked="true" class="dm-ui-checkbox dm-ui-menuitem-checkbox" role="checkbox" tabindex="0"><input checked="checked" id="checkbox' + x.node_id + '" name="checkbox' + x.node_id + '" type="checkbox" data-node-id="' + x.node_id + '" data-grid-id="' + (i - 1) + '"><label for="checkbox' + x.node_id + '">' + x.node_name + " </label></div></td>";
                    }
                    if (i % numTableColumns === 0) {
                        rowsNumber++;
                        tmp += "</tr>";
                    }
                });
                if (rowsNumber === 0 && i < 4) {
                    for (var k = i; k < 4; ++k) {
                        tmp += "<td>&nbsp;</td>";
                    }
                    tmp += "</tr>";
                }
                tmp += "</table>";
                container.append(tmp);
                if (isAllStudentsSelected) {
                    $("#check-all-students-modal").parents(".dm-ui-checkbox").attr("aria-checked", "true");
                    $("#check-all-students-modal").prop("checked", true);
                } else {
                    $("#check-all-students-modal").parents(".dm-ui-checkbox").attr("aria-checked", "false");
                    $("#check-all-students-modal").prop("checked", false);
                }
            }
            function getCurrentClassInfoParameters(i) {
                var params = "";
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
                return params;
            }
            $(document).on("click touchstart", "#apply-dashboard-print-students", function (e) {
                e.preventDefault();
                var params = "";
                var grid = $("#roster-table-wrapper").data("kendoGrid");
                var gridData = grid.dataSource.data();
                var arrSelected = [];
                $("#modal-dashboard-print-students .students-wrapper input[type=checkbox]").each(function () {
                    if ($(this).prop("checked")) {
                        arrSelected.push($(this).data("grid-id"));
                    }
                });
                var studentIds = "";
                arrSelected.forEach(function (item, i) {
                    if (studentIds !== "") {
                        studentIds += ",";
                    }
                    studentIds += gridData[item].node_id;
                });
                params += "input=" + studentIds;
                if (private.convertToPdfImmediately) {
                    Dashboard.PostPdfProfileNarrative(params);
                } else {
                    window.open("Dashboard/StudentProfile?" + params, "_blank").focus();
                }
            });
            $(document).on("click touchstart", ".student-link", function (e) {
                e.preventDefault();
                var params = "input=" + $(this).data("node-id");
                if (private.convertToPdfImmediately) {
                    Dashboard.PostPdfProfileNarrative(params);
                } else {
                    //window.location.href = "/Dashboard/StudentProfile?" + params;
                    window.open("Dashboard/StudentProfile?" + params, "_blank").focus();
                }
            });


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
                if ($(e.target).attr("id") === "roster-table-wrapper_active_cell") {
                    clickedGridElement = $(e.target);
                } else {
                    clickedGridElement = $(e.target).parents("#roster-table-wrapper td, #roster-table-wrapper th");
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
            $(document).on("change", "#roster-table-wrapper .k-pager-sizes select", function () {
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
            $(document).on("keyup", ".root-tab-element", function (e) {
                if ((e.keyCode === 13 || e.keyCode === 32) && ($.inArray("root-tab-element", e.target.classList) !== -1)) {
                    e.stopPropagation();
                    $(this).rootMakeContentTabbable(true);
                }
                if (e.keyCode === 27) {
                    if (!$(this).hasClass("filters")) {
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
            $(document).on("keyup", ".root-tab-element .dm-ui-dropdown-widget .dm-ui-dropdown-buttons-container button", function (e) {
                if (e.keyCode === 27) {
                    $(this).closest(".dm-ui-dropdown-button").focus();
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
                            e.preventDefault();
                            e.stopPropagation();
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
                    $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable(false);
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    var self = $(this);
                    function setFocusBack() {
                        self.focus();
                        $("body").addClass("wcag_focuses_on");
                    }
                    setTimeout(setFocusBack, 100);
                }
            });
            $(document).on("keydown", "#roster-top-buttons-wrapper .k-button.k-button-icon, #apply-dashboard-report-button, #cancel-dashboard-report-button, #modal-dashboard-report-criteria .column-val1 input, #modal-dashboard-report-criteria .column-val2 input", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $("body").removeClass("wcag_focuses_on");
                    $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable(false);
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    function setFocusBack() {
                        if (!$("#modal-dashboard-report-criteria .dm-ui-alert-error").is(":visible")) {
                            $("#roster-filters-popup-button").focus();
                            $("body").addClass("wcag_focuses_on");
                            $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                        }
                    }
                    setTimeout(setFocusBack, 100);
                }
            });

            $(document).on("blur", "#roster-table-wrapper td, #roster-table-wrapper th", function (e) {
                //if (!private.RosterGridNavigationEvent && !private.RosterGridKeyEvent) {
                if (private.isTabNavigationOn) {
                    if (e.relatedTarget !== null && e.relatedTarget.nodeName === "TABLE") {
                        $(e.target).focus();

                        function setFocusBack(element) {
                            if (!private.RosterGridNavigationEvent && !private.RosterGridOrderingEvent) {
                                element.focus();
                            }
                            private.RosterGridOrderingEvent = false;
                        }

                        if ((private.LastRosterTableKeyPressedCode >= 37 && private.LastRosterTableKeyPressedCode <= 40) || private.LastRosterTableKeyPressedCode === 9 || private.LastRosterTableKeyPressedCode === 16 || private.LastRosterTableKeyPressedCode === 13) {
                            setTimeout(setFocusBack, 1, $(e.target)); //for Firefox additional focus event
                        }
                        private.LastRosterTableKeyPressedCode = 0;

                    }
                }
                private.RosterGridNavigationEvent = false;
                private.RosterGridKeyEvent = false;
            });

            $(document).on("keyup", "#roster-table-wrapper td, #roster-table-wrapper th", function (e) {
                private.LastRosterTableKeyPressedCode = e.keyCode;
                private.RosterGridKeyEvent = true;
                if (e.keyCode === 9) {
                    Dashboard.setGridCellCurrentState($(this));
                }
            });
            $(document).on("keydown", "#roster-table-wrapper th.k-header", function (e) {
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
                if ((e.keyCode === 13 || e.keyCode === 32) && private.isTabNavigationOn) {
                    e.preventDefault();
                    var link = $(this).find(".location-drill, .student-link");
                    if (link.length) {
                        link.click();
                    }
                }
            });
            $(document).on("keyup", "#roster-table-wrapper .location-drill, #roster-table-wrapper .student-link", function (e) {
                if ((e.keyCode === 13 || e.keyCode === 32) && !private.isTabNavigationOn) {
                    e.preventDefault();
                    $(this).click();
                }
            });

            //***** Location drill link ajax request *****
            $(document).on("click touchstart", ".location-drill", function (e) {
                e.preventDefault();
                DmUiLibrary.ShowOverlaySpinner();
                DmUiLibrary.AbortAllAjaxRequests(); //abort all Ajax requests
                Dashboard.ClearAllErrorsMessages();
                var link = $(this).data("href");
                if (private.isCogatRoster) {
                    private.isCogatDrill = true;
                    if (link.indexOf("&type=class") !== -1) {
                        private.isCogatRosterTypeStudents = true;
                    }
                }
                $.ajax({
                    type: "GET",
                    dataType: "json",
                    url: link,
                    success: function (data) {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            if (data === "success" || data === '"success"') {
                                //location.reload();
                                Dashboard.GetFilters(true);
                            }
                        }
                    },
                    error: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });
            $(document).on("keydown", ".bread-crumbs .location-drill", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    $(this).click();
                }
            });

            //***** Roster popup filters behavior *****
            $(document).on("change", ".table-report-filters td:first-child select", function () {
                var rowNumber = $(this).attr("id");
                rowNumber = rowNumber.substr(rowNumber.length - 1).trim();
                var dropdownContentArea = $(this).parents("td").next().find("select");
                var dropdownCondition = $(this).parents("td").next().next().find("select");
                var dropdownConditionSelected = dropdownCondition.val();
                dropdownContentArea.empty();
                dropdownCondition.empty();
                dropdownCondition.append(private.RosterDropdownOptionsCondition);
                var selectedValue = $(this).val();
                if (selectedValue === "NationalPercentileRank" || selectedValue === "StandardScore") {
                    dropdownContentArea.append(private.RosterDropdownOptionsSS);
                    dropdownContentArea.prop("disabled", true);
                    $("#wrapper-input-score-" + rowNumber).show();
                    $("#input-score-" + rowNumber + "-val1").prop("disabled", false);
                    //$("#wrapper-dropdown-score-" + rowNumber).hide();
                    $("#dropdown-score-range-" + rowNumber).prop("disabled", true);
                }
                if (selectedValue === "DomainPerformanceLevel") {
                    dropdownContentArea.append(private.RosterDropdownOptionsDomain);
                    dropdownContentArea.prop("disabled", false);
                    dropdownCondition.find("option:last-child").remove();
                    $("#wrapper-input-score-" + rowNumber).hide();
                    $("#input-score-" + rowNumber + "-val1").prop("disabled", true);
                    //$("#wrapper-dropdown-score-" + rowNumber).show();
                    $("#dropdown-score-range-" + rowNumber).prop("disabled", false);
                }
                $("#dropdown-score-range-" + rowNumber).trigger("DmUi:updated");
                dropdownContentArea.trigger("DmUi:updated");
                $("#dropdown-condition-" + rowNumber + " option[value=" + dropdownConditionSelected + "]").prop("selected", true);
                dropdownCondition.trigger("DmUi:updated");
                checkIsSelectedAnyInBetweenConditionOption();
                validateReportFilters();
            });
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
                            //$("#dropdown-content-area-" + rowNumber).prop("disabled", false);
                            $("#dropdown-condition-" + rowNumber).prop("disabled", false);
                            $("#input-score-" + rowNumber + "-val1").prop("disabled", false);
                            $("#dropdown-score-range-" + rowNumber).prop("disabled", false);
                            $("#dropdown-boolean-" + rowNumber).prop("disabled", false);
                        }
                    } else {
                        trNext.addClass("filter-row-disabled");
                        $("#dropdown-score-" + rowNumber).prop("disabled", true);
                        //$("#dropdown-content-area-" + rowNumber).prop("disabled", true);
                        $("#dropdown-condition-" + rowNumber).prop("disabled", true);
                        $("#input-score-" + rowNumber + "-val1").prop("disabled", true);
                        $("#dropdown-score-range-" + rowNumber).prop("disabled", true);
                        $("#dropdown-boolean-" + rowNumber).prop("disabled", true);
                        if (trNext.next().length) {
                            trNext.next().addClass("filter-row-disabled");
                            $("#dropdown-score-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            //$("#dropdown-content-area-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-condition-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#input-score-" + (Number(rowNumber) + 1) + "-val1").prop("disabled", true);
                            $("#input-score-" + (Number(rowNumber) + 1) + "-val2").prop("disabled", true);
                            $("#dropdown-score-range-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-boolean-" + rowNumber + " option").removeAttr("selected");
                            $("#dropdown-boolean-" + rowNumber + " option:first-child").prop("selected", true);
                            $("#dropdown-boolean-" + (Number(rowNumber) + 1)).prop("disabled", true);
                            $("#dropdown-score-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
                            //$("#dropdown-content-area-" + (Number(rowNumber) + 1)).trigger("DmUi:updated");
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
                validateReportFilters();
            });
            $(document).on("change", ".table-report-filters td:nth-child(3) select", function () {
                checkIsSelectedAnyInBetweenConditionOption();
                validateReportFilters();
            });
            $(document).on("keyup", ".table-report-filters tr:not(.filter-row-disabled) td.column-val1 input, .table-report-filters tr:not(.filter-row-disabled) td.column-val2 input, .table-report-filters tr:not(.filter-row-disabled) td:first-child select", function () {
                validateReportFilters();
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
            function validateReportFilters() {
                var numCorrectRows = 0;
                var numTotalRows = $(".table-report-filters tr:not(.filter-row-disabled)").length;
                $(".table-report-filters tr:not(.filter-row-disabled) td:first-child select").each(function () {
                    if (!$(this).prop("disabled") && $(this).val() === "DomainPerformanceLevel") {
                        numCorrectRows++;
                    }
                });
                $(".table-report-filters tr:not(.filter-row-disabled) td.column-val1 input").each(function () {
                    if (!$(this).prop("disabled") && $(this).val() !== "") {
                        if ($.isNumeric($(this).val())) {
                            numCorrectRows++;
                        }
                    }
                });
                if (numCorrectRows === numTotalRows) {
                    $("#apply-dashboard-report-button").prop("disabled", false);
                    $("#cancel-dashboard-report-button").removeClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-report-button").addClass("last-tab-element wcag-modal-last-element");
                } else {
                    $("#apply-dashboard-report-button").prop("disabled", true);
                    $("#cancel-dashboard-report-button").addClass("last-tab-element wcag-modal-last-element");
                    $("#apply-dashboard-report-button").removeClass("last-tab-element wcag-modal-last-element");
                }
            }
            function cancelDashboardReportButtonClick() {
                $(".table-report-filters .k-filtercell > span > .k-button").click();
                $("#modal-dashboard-report-criteria").fadeOut("fast", function () { });
                $("#roster-filters-popup-button").removeClass("applied");
                $("#roster-reset-button").click();
                $("#roster-reset-button").addClass("disabled-element");
                $("#roster-reset-button").makeElementNotTabbable();
                $("#roster-filters-popup-button").focus();
                private.LastFocusedElement = $("#roster-filters-popup-button");
            }
            $(document).on("click touchstart", "#cancel-dashboard-report-button", function () {
                cancelDashboardReportButtonClick();
            });
            $(document).on("keydown", "#modal-dashboard-report-criteria .column-val1 input, #modal-dashboard-report-criteria .column-val2 input", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    $("#apply-dashboard-report-button").click();
                }
            });
            $(document).on("keyup", "body", function (e) {
                if (e.keyCode === 27) {
                    if ($("#modal-dashboard-report-criteria").is(":visible")) {
                        if ($.inArray("dm-ui-li-item", e.target.classList) !== -1) {
                            $(e.target).parents(".dm-ui-dropdown-content").prev().focus();
                        } else {
                            cancelDashboardReportButtonClick();
                        }
                    }
                    if ($("#modal-dashboard-print-reports").is(":visible")) {
                        if (($.inArray("dm-ui-li-item", e.target.classList) !== -1) || ($.inArray("dm-ui-select-all", e.target.classList) !== -1) || ($.inArray("dm-ui-select-none", e.target.classList) !== -1) || ($.inArray("dm-ui-menuitem-checkbox", e.target.classList) !== -1) || ($.inArray("dm-ui-cancel-button", e.target.classList) !== -1) || ($.inArray("dm-ui-apply-button", e.target.classList) !== -1)) {
                            $(e.target).parents(".dm-ui-dropdown-content").prev().focus();
                        } else {
                            $("#cancel-dashboard-print-reports").click();
                        }
                    }
                }
            });
            $(document).on("click touchstart", "#apply-dashboard-report-button", function () {
                Dashboard.applyRosterPopupFilters();
                $("#roster-filters-popup-button").addClass("applied");
                $("#roster-reset-button").removeClass("disabled-element");
                $("#roster-reset-button").makeElementTabbable();

                var dataSource = $("#roster-table-wrapper").data("kendoGrid").dataSource;
                var filters = dataSource.filter();
                var allData = dataSource.data();
                var query = new kendo.data.Query(allData);
                var filteredDataAllPages = query.filter(filters).data;

                $("#modal-dashboard-report-criteria .dm-ui-alert-error").remove();
                if (filteredDataAllPages.length) {
                    $("#modal-dashboard-report-criteria").fadeOut("fast", function () { });
                    $("#roster-filters-popup-button").focus();
                } else {
                    $("#modal-dashboard-report-criteria .table-report-filters").before('<div class="dm-ui-alert dm-ui-alert-error" role="alert"><a href="#" class="dm-ui-alert-close" aria-label="Close alert message" role="button">×</a>No data resulted in your search criteria, please change criteria and try again.</div>');
                }
                $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
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
                //$(".bread-crumbs .location-drill:first-child").click();
                $(this).addClass("disabled-element");
                $.ajax({
                    type: "GET",
                    url: siteRoot + "/api/Dashboard/ResetFilters",
                    //data: data,
                    dataType: "json",
                    beforeSend: function () {
                        DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            $("#roster-selected-quantile-label").empty();
                            $("#roster-selected-domain-label").empty();
                            Dashboard.clearDomainSelection();
                            Dashboard.clearQuantileSelection();

                            Dashboard.GetFilters(true);
                        }
                    },
                    error: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });

            //***** Roster other buttons, filters & popup behavior *****
            $(document).on("keyup", "#roster-reset-button", function (e) {
                if (e.keyCode === 13 || e.which === 32) {
                    $("#roster-reset-button").click();
                }
            });
            $(document).on("click touchstart", "#roster-reset-button", function (e) {
                e.preventDefault();
                $("#roster-filters-popup-button").removeClass("applied");
                $("#roster-reset-button").addClass("disabled-element");
                $("#roster-reset-button").makeElementNotTabbable();
                if (private.isAdaptive) {
                    $("#roster-selected-quantile-label").empty();
                    $("#roster-selected-domain-label").empty();
                    Dashboard.clearDomainSelection();
                    Dashboard.clearQuantileSelection();
                    if (private.isGradeK1) {
                        Dashboard.GetPerformanceScoresKto1();
                    } else {
                        Dashboard.GetQuantiles(false);
                    }
                    if (private.isCogatRoster) {
                        Dashboard.GetAchievementAbilitySummary();
                    }
                    Dashboard.GetRoster();
                    Dashboard.GetDomains();
                } else {
                    $("#roster-selected-domain-label .close").click();
                    $("#roster-selected-quantile-label .close").click();
                    $("#roster-top-buttons-wrapper span.k-filtercell button").click();
                    $("#roster-table-wrapper").data("kendoGrid").dataSource.filter([]);
                }
            });
            function checkIsResetRosterButtonShouldBeEnabledOrNot() {
                if ($("#roster-selected-quantile-label .arrow").length === 0 && $("#roster-selected-domain-label .arrow").length === 0) {
                    $("#roster-reset-button").addClass("disabled-element");
                    $("#roster-reset-button").makeElementNotTabbable();
                }
            }
            $(document).on("click touchstart", "#roster-top-buttons-wrapper span.k-filtercell button", function () {
                checkIsResetRosterButtonShouldBeEnabledOrNot();
            });
            function focusFirstFilterReportCriteria() {
                $("#dropdown-score-1_dm_ui .dm-ui-dropdown-button").focus();
                if (!$("body").hasClass("wcag_focuses_on")) {
                    $("#dropdown-score-1_dm_ui .dm-ui-dropdown-button").blur();
                }
            }
            $(document).on("click touchstart", "#roster-filters-popup-button", function () {
                //$("#cancel-dashboard-report-button").click(); //uncomment if you want to reset selected students in roster every time popup shown
                //$("#apply-dashboard-report-button").prop("disabled", true);
                //$("#dropdown-score-1_dm_ui > button").addClass("first-tab-element wcag-modal-first-element");
                $("#modal-dashboard-report-criteria").fadeIn("fast", function () { });
                setTimeout(focusFirstFilterReportCriteria, 100);
            });
            $(document).on("keydown", "#roster-filters-popup-button", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    $("#roster-filters-popup-button").click();
                }
            });
            function applySearchRosterByName() {
                $("#roster-top-buttons-wrapper .k-filtercell > span > .k-button").each(function () {
                    $("#roster-reset-button").removeClass("disabled-element");
                    $("#roster-reset-button").makeElementTabbable();

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
            $(document).on("click touchstart", "#roster-top-buttons-wrapper .k-filtercell button.k-button-icon", function () {
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



            //***** Select & Unselect Quantile & Domain band *****
            $(document).on("click touchstart keydown", ".quantile-band", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32 || e.which === 1 || e.which === undefined) {
                    //if ((e.keyCode === 13 || e.keyCode === 32 || e.which === 1) && ($.inArray("quantile-band", e.target.classList) !== -1)) {
                    e.preventDefault();
                    e.stopPropagation();

                    if (e.which !== undefined) {
                        private.LastFocusedElement = $(this);
                    }

                    $(this).parents(".root-tab-element").rootMakeContentTabbable();

                    var objQuantileClickInfo = new Object();
                    var dataRange = $(this).attr("data-range");
                    var dataRangeBand = $(this).attr("data-range-band");
                    if (private.isAdaptive) {
                    } else {
                        if (Dashboard.getRosterType() !== "students") {
                            Dashboard.ClearAllErrorsMessages();
                            objQuantileClickInfo["dataRangeBandNpr"] = dataRangeBand;
                            objQuantileClickInfo["dataRange"] = dataRange;
                            Dashboard.GetRoster("type=student", objQuantileClickInfo);
                        }
                    }

                    var isThisQuartileGreyscale = false;
                    var isAnyQuartilesGreyscale = false;
                    if ($(this).hasClass("grayscale")) {
                        isThisQuartileGreyscale = true;
                    }
                    $(".quantile .rect-label").each(function () {
                        if ($(this).hasClass("grayscale")) {
                            isAnyQuartilesGreyscale = true;
                            return false;
                        }
                    });

                    Dashboard.clearQuantileSelection();

                    if (isThisQuartileGreyscale || !isAnyQuartilesGreyscale) {
                        Dashboard.RefreshAverageStandardScoreAndNPR(dataRange);
                        $(".quantile-band").each(function () {
                            if ($(this).attr("data-range") !== dataRange) {
                                $(this).addClass("grayscale");
                            }
                        });
                        $("#barChart4 .rect-label").each(function () {
                            if ($(this).attr("data-range") !== dataRange) {
                                $(this).addClass("grayscale");
                            }
                        });
                        $("#roster-table-wrapper").addClass("color-event-range-" + dataRange);

                        if (private.isCogatRoster) {
                            private.isCogatRosterTypeStudents = true;
                            Dashboard.GetAchievementAbilitySummary();
                        } else {
                            Dashboard.GetAchievementAbilitySummary(true);
                        }

                        //Dashboard.changeRosterLabelNPR(dataRangeBand);
                        Dashboard.changeRosterLabelNPR(dataRangeBand, $(this).data("link-name"), dataRange);
                        if (private.isAdaptive) {
                            //dataRangeBand.replace("-", ":");
                            objQuantileClickInfo["dataRangeBandNpr"] = dataRangeBand;
                            objQuantileClickInfo["dataRange"] = dataRange;
                            Dashboard.GetRoster($(this).data("adaptive-url"), objQuantileClickInfo);
                            Dashboard.GetDomains(dataRange);
                        }
                    } else {
                        $("#roster-selected-quantile-label .close").click();
                    }
                }
            });
            $(document).on("click touchstart keydown", ".domain-band", function (e) {
                private.LastFocusedElement = $(this);
                if ((e.keyCode === 13 || e.keyCode === 32 || e.which === 1 || typeof e.which === "undefined")) {
                    //if ((e.keyCode === 13 || e.keyCode === 32 || e.which === 1) && ($.inArray("domain-band", e.target.classList) !== -1)) {
                    e.preventDefault();
                    e.stopPropagation();

                    $(this).parents(".master-of-roots").masterMakeInnerRootsTabbable();
                    $(this).parents(".root-tab-element").rootMakeContentTabbable();

                    var objDomainClickInfo = new Object();
                    var dataRange = $(this).attr("data-range");
                    var dataRangeBand = $(this).attr("data-range-band");
                    var dataCategory = $(this).attr("data-category");
                    dataCategory = dataCategory.replace(/[^a-zA-Z]/gi, "") + "_SKILL_NCE";
                    //var dataType = $(this).attr("data-type");

                    if (private.isAdaptive) {
                    } else {
                        if (Dashboard.getRosterType() !== "students") {
                            Dashboard.ClearAllErrorsMessages();

                            objDomainClickInfo["dataRangeBandSkillNce"] = dataRangeBand;
                            objDomainClickInfo["dataCategorySkillNce"] = dataCategory;
                            Dashboard.GetRoster("type=student", null, objDomainClickInfo);
                        }
                    }

                    var isThisLinkGreyscale = false;
                    var isThisDomainGreyscale = false;
                    var isAnyDomainsGreyscale = false;
                    var domainsNumber = $("#dashboard-right-column .dm-ui-card").length;
                    var graiscaleBandsNumber = $("#dashboard-right-column .dm-ui-card .domain-band.grayscale").length;

                    if ($(this).hasClass("grayscale")) {
                        isThisLinkGreyscale = true;
                    }
                    if ($(this).parents(".dm-ui-card").hasClass("grayscale")) {
                        isThisDomainGreyscale = true;
                    }
                    $("#dashboard-right-column .dm-ui-card").each(function () {
                        if ($(this).hasClass("grayscale")) {
                            isAnyDomainsGreyscale = true;
                            return false;
                        }
                    });

                    //var selectedQuantile = $("#roster-selected-quantile-label").attr("data-range");
                    var selectedQuantile;
                    if ($("#barChart4 .rect-label.grayscale").length) {
                        selectedQuantile = $("#barChart4 a.quantile-band.rect-label:not(.grayscale)").data("range");
                    }

                    if (isThisLinkGreyscale || (!isAnyDomainsGreyscale && domainsNumber > 1) || (graiscaleBandsNumber === 0 && domainsNumber === 1) || isThisDomainGreyscale) {
                        Dashboard.clearDomainSelection();
                        var parentCardId = $(this).parents(".dm-ui-card").attr("id");
                        var parentCardNum = parentCardId.substr(parentCardId.length - 1).trim();

                        $("#dashboard-right-column .dm-ui-card").each(function () {
                            if ($(this).attr("id") !== parentCardId) {
                                $(this).addClass("grayscale");
                            } else {
                                $(this).find(".domain-band").each(function () {
                                    if ($(this).attr("data-range") !== dataRange) {
                                        $(this).addClass("grayscale");
                                    }
                                });
                            }
                        });
                        $("#dashboard-right-column .rect-label").each(function () {
                            if ($(this).parents(".dm-ui-card").attr("id") !== parentCardId) {
                                $(this).addClass("grayscale");
                            } else {
                                //$(this).find(".domain-band").each(function () {
                                if ($(this).attr("data-range") !== dataRange) {
                                    $(this).addClass("grayscale");
                                }
                                //});
                            }
                        });

                        $("#roster-table-wrapper").addClass("color-domain-num-" + parentCardNum);
                        $("#roster-table-wrapper").addClass("color-domain-range-" + dataRange);

                        Dashboard.changeRosterLabelSkillNCE(dataRange, parentCardId);
                        if (private.isAdaptive) {
                            //dataRangeBand.replace("-", ":");
                            objDomainClickInfo["dataRangeBandSkillNce"] = dataRangeBand;
                            objDomainClickInfo["dataCategorySkillNce"] = dataCategory;
                            objDomainClickInfo["dataRange"] = dataRange;
                            objDomainClickInfo["parentCardNum"] = parentCardNum;

                            var domainId = $(this).data("adaptive-url");
                            domainId = domainId.substr(domainId.indexOf("domainId=") + 9);
                            domainId = domainId.substr(0, domainId.indexOf("&"));

                            if (private.isCogatRoster) {
                                private.isCogatRosterTypeStudents = true;
                                Dashboard.GetAchievementAbilitySummary();
                            } else {
                                Dashboard.GetAchievementAbilitySummary(true);
                            }

                            if (typeof selectedQuantile !== "undefined") {
                                objDomainClickInfo["dataRangeBandNpr"] = selectedQuantile;
                                Dashboard.GetQuantiles(false, domainId, dataRange, selectedQuantile, true);
                                Dashboard.GetRoster($(this).data("adaptive-url") + "&performanceBand=" + selectedQuantile, null, objDomainClickInfo);
                            } else {
                                Dashboard.GetQuantiles(false, domainId, dataRange, null, true);
                                Dashboard.GetRoster($(this).data("adaptive-url"), null, objDomainClickInfo);
                            }
                        }
                    } else {
                        $("#roster-selected-domain-label .close").click();
                        Dashboard.clearDomainSelection();
                    }
                }
            });

            $(document).on("click touchstart", "#roster-selected-quantile-label .close", function () {
                private.cogatSelectedQuantile = 0;
                private.cogatSelectedDomainId = 0;
                private.cogatSelectedDomainLevel = 0;
                if (private.isCogatRoster) {
                    $(".table-matrix td.data").removeClass("selected");
                } else {
                    if (typeof private.modelQuartiles.is_cogat !== "undefined") {
                        if (private.modelQuartiles.is_cogat) {
                            $("#achievement-ability").show();
                        }
                    }
                }
                $("#roster-selected-quantile-label").empty();
                $("#roster-selected-domain-label").empty();
                Dashboard.clearQuantileSelection();
                //$('#roster-table-wrapper').data("kendoGrid").dataSource.filter([]);
                Dashboard.RefreshAverageStandardScoreAndNPR();
                if (private.isAdaptive) {
                    Dashboard.GetRoster();
                    Dashboard.GetDomains();
                }
                if (private.isCogatRoster) {
                    Dashboard.GetAchievementAbilitySummary();
                }
            });
            $(document).on("keydown", "#roster-selected-quantile-label .close", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    e.preventDefault();
                    $(this).click();
                }
            });

            $(document).on("click touchstart", "#roster-selected-domain-label .close", function () {
                private.cogatSelectedDomainId = 0;
                private.cogatSelectedDomainLevel = 0;
                if (private.isCogatRoster) {
                    $(".table-matrix td.data").removeClass("selected");
                } else {
                    if (typeof private.modelQuartiles.is_cogat !== "undefined") {
                        if (private.modelQuartiles.is_cogat) {
                            $("#achievement-ability").show();
                        }
                    }
                }
                Dashboard.clearDomainSelection();
                if (private.isAdaptive) {
                    var selectedQuantile = $("#roster-selected-quantile-label").attr("data-range");
                    if (typeof selectedQuantile !== "undefined") {
                        $(".rect-label").removeClass("grayscale");
                        $('.quantile div.quantile-band[data-range="' + selectedQuantile + '"]').first().click();
                        //$('svg a.quantile-band[data-range="3"]').first().click();
                    } else {
                        Dashboard.GetRoster();
                    }
                } else {
                    //clear filters
                    //$('#roster-table-wrapper').data("kendoGrid").dataSource.filter([]);
                    clearAllRosterFiltersExceptNpr();
                }
                $("#roster-selected-domain-label").empty();
                if (private.isCogatRoster) {
                    Dashboard.GetAchievementAbilitySummary();
                }
            });
            $(document).on("keydown", "#roster-selected-domain-label .close", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    e.preventDefault();
                    $(this).click();
                }
            });

            function clearAllRosterFiltersExceptNpr() {
                var currentFilters, currentFiltersClean = [];
                if ($("#roster-table-wrapper").data("kendoGrid").dataSource.filter() == undefined) {
                    currentFilters = [];
                } else {
                    currentFilters = $("#roster-table-wrapper").data("kendoGrid").dataSource.filter().filters;
                }
                //clear all filters except NPR
                for (var i = 0; i < currentFilters.length; ++i) {
                    if (currentFilters[i]["field"] === "NPR" || currentFilters[i]["logic"] === "or") {
                        currentFiltersClean.push(currentFilters[i]);
                    }
                }
                $("#roster-table-wrapper").data("kendoGrid").dataSource.filter(currentFiltersClean);
            }

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
                    if (private.isTabNavigationOn) {
                        if ($(".roster-table-card.root-tab-element").attr("tabindex") === "-1") {
                            $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                        }
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

            $(document).on("click touchstart keydown", ".pld-performance-stage", function (e) {
                private.LastFocusedElement = $(this);
                if ((e.keyCode === 13 || e.keyCode === 32 || e.which === 1)) {
                    e.preventDefault();
                    e.stopPropagation();

                    $(this).parents(".root-tab-element").rootMakeContentTabbable();

                    var objDomainClickInfo = new Object();
                    var stageName = $(this).attr("data-link-name");
                    var stageNumber = $(this).attr("data-pld-stage-number");
                    var studentsNumber = $(this).attr("data-students-number");
                    var studentsPercent = $(this).attr("data-students-percent");

                    var isThisStageGreyscale = false;
                    var isAnyStageGreyscale = false;
                    if ($(this).hasClass("grayscale")) {
                        isThisStageGreyscale = true;
                    }
                    $(".pld-performance-stage.rect-label").each(function () {
                        if ($(this).hasClass("grayscale")) {
                            isAnyStageGreyscale = true;
                            return false;
                        }
                    });

                    if (isThisStageGreyscale || !isAnyStageGreyscale || $("#roster-selected-domain-label").text().indexOf("PLD Level ") !== -1) {
                        Dashboard.clearDomainSelection();
                        $("#average-standard-score span").text(private.modelQuartiles.values[stageNumber - 1].studentCount);

                        $(".pld-performance-stage, .rect-label").each(function () {
                            if ($(this).attr("data-pld-stage-number") !== stageNumber) {
                                $(this).addClass("grayscale");
                            }
                        });

                        $("#dashboard-right-column .dm-ui-card").each(function () {
                            if ($(this).find("h2").text() !== stageName) {
                                $(this).addClass("grayscale");
                            }
                        });

                        Dashboard.changeRosterLabelPldStageLevel(stageName, "", studentsNumber, studentsPercent);

                        objDomainClickInfo["pldStageName"] = stageName;
                        objDomainClickInfo["pldStageNumber"] = stageNumber;
                        Dashboard.GetRoster(null, null, objDomainClickInfo);

                        $("html , body").stop().animate({
                            scrollTop: $("#blockChart3Bars-delayed-" + stageNumber).offset().top - 60
                        }, 200);
                    } else {
                        $("#roster-selected-domain-label .close").click();
                    }
                }
            });

            $(document).on("click touchstart keydown", ".piechart-link", function (e) {
                private.LastFocusedElement = $(this);
                if ((e.keyCode === 13 || e.keyCode === 32 || e.which === 1)) {
                    e.preventDefault();
                    e.stopPropagation();

                    $(this).parents(".master-of-roots").masterMakeInnerRootsTabbable();
                    $(this).parents(".root-tab-element").rootMakeContentTabbable();

                    var objDomainClickInfo = new Object();
                    var stageName = $(this).attr("data-link-name");
                    var stageNumber = $(this).attr("data-pld-stage-number");
                    var levelNumber = $(this).attr("data-pld-level-number");
                    var studentsNumber = $(this).attr("data-students-number");
                    var studentsPercent = $(this).attr("data-students-percent");

                    var isThisLinkGreyscale = false;
                    var isThisStageHasOtherGreyscale = false;

                    if ($(this).hasClass("grayscale")) {
                        isThisLinkGreyscale = true;
                    }
                    if ($(this).parents(".dm-ui-card").find("td.piechart-link").hasClass("grayscale")) {
                        isThisStageHasOtherGreyscale = true;
                    }

                    //var selectedQuantile = $("#roster-selected-quantile-label").attr("data-range");
                    if (!isThisLinkGreyscale && isThisStageHasOtherGreyscale) {
                        $("#roster-selected-domain-label .close").click();
                    } else {
                        $("#average-standard-score span").text(private.modelPldCards[stageNumber - 1].values[levelNumber - 1].studentCount);
                        Dashboard.clearDomainSelection();
                        var parentCardId = $(this).parents(".dm-ui-card").attr("id");
                        var parentCardNum = parentCardId.substr(parentCardId.length - 1).trim();
                        var levelText = $("#" + parentCardId + " table td:nth-child(" + levelNumber + ")").text();

                        $("#dashboard-right-column .dm-ui-card").each(function () {
                            if ($(this).attr("id") !== parentCardId) {
                                $(this).addClass("grayscale");
                            } else {
                                $(this).find(".piechart-link, .piechart-text, .piechart-polyline").each(function () {
                                    if ($(this).attr("data-pld-level-number") !== levelNumber) {
                                        $(this).addClass("grayscale");
                                    }
                                });
                            }
                        });

                        //grayscale Performance Stage Card
                        $(".pld-performance-stage, .rect-label").removeClass("grayscale");
                        $(".pld-performance-stage, .rect-label").each(function () {
                            if ($(this).attr("data-pld-stage-number") !== stageNumber) {
                                $(this).addClass("grayscale");
                            }
                        });
/*
                        $("#roster-table-wrapper").addClass("color-domain-num-" + parentCardNum);
                        $("#roster-table-wrapper").addClass("color-domain-range-" + stageNumber);
*/
                        Dashboard.changeRosterLabelPldStageLevel(stageName, levelText, studentsNumber, studentsPercent);

                        objDomainClickInfo["pldStageName"] = stageName;
                        objDomainClickInfo["pldStageNumber"] = stageNumber;
                        objDomainClickInfo["pldLevelNumber"] = levelNumber;
                        objDomainClickInfo["parentCardNum"] = parentCardNum;
                        Dashboard.GetRoster(null, null, objDomainClickInfo);
                    }
                }
            });

            //***** Print Reports ***** 
            $(document).on("click touchstart", "#print-reports-button", function (e) {
                e.preventDefault();
                if (!$.isEmptyObject(private.modelHierarchy)) {
                    var optionsPerformanceStages = "";
                    var arrStagesNotNull = [];
                    //var arrStages = ["Pre-Emerging", "Emerging", "Beginning", "Transitioning", "Independent"];
                    var arrStages = [];
                    $("#barChart4 div.pld-performance-stage div").each(function () {
                        arrStages.push($(this).text());
                    });

                    private.modelHierarchy.values.forEach(function (itemStudents) {
                        itemStudents.studentList.forEach(function (item, i) {
                            if (arrStagesNotNull.indexOf(item.pldStage) === -1) {
                                arrStagesNotNull.push(item.pldStage);
                            }
                        });
                    });

                    //arrays intersection
                    arrStages = arrStages.filter(function (item) {
                        return arrStagesNotNull.indexOf(item) !== -1;
                    });

                    /*
                    $("#dashboard-right-column .dm-ui-card h2").each(function () {
                        optionsPerformanceStages += '<option value="' + $(this).text() + '">' + $(this).text() + "</option>";
                    });
                    */
                    arrStages.forEach(function (item, i) {
                        optionsPerformanceStages += '<option value="' + item + '">' + item + "</option>";
                    });

                    $("select#print-performance-stages").html(optionsPerformanceStages).trigger("DmUi:updated");
                    $("#treeStudentsMultiSelectDropdownList").empty().trigger("DmUi:updated").trigger("change");
                    $("#modal-dashboard-print-reports .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox  input").attr("disabled", "disabled"); //'Select All' checkbox diabled
                    $("#modal-dashboard-print-reports .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").addClass("dm-ui-disabled").attr("aria-disabled", "true").attr("tabindex", -1); //'Select All' checkbox disabled
                    $("#modal-dashboard-print-reports").fadeIn("fast");
                    $("#modal-dashboard-print-reports #print-report-type_dm_ui > button").focus();
                }
            });
            $(document).on("keydown", "#print-reports-button", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    $(this).click();
                }
            });
            $(document).on("click touchstart", "#modal-dashboard-print-reports .close_icon, #cancel-dashboard-print-reports", function () {
                $("#modal-dashboard-print-reports").fadeOut("fast");
                $("#print-reports-button").focus();
            });
            $(document).on("change", "#check-all-students-modal-reports", function () {
                if ($(this).prop("checked")) {
                    $("#students-treeview-wrapper .dm-ui-select-all-group").click();
                } else {
                    $("#students-treeview-wrapper .dm-ui-select-none-group").click();
                }
            });
            $(document).on("change", "#treeStudentsMultiSelectDropdownList", function () {
                if ($(this).find("option:not(:selected)").length) {
                    $("#check-all-students-modal-reports").parents(".dm-ui-checkbox").attr("aria-checked", "false");
                    $("#check-all-students-modal-reports").prop("checked", false);
                } else {
                    $("#check-all-students-modal-reports").parents(".dm-ui-checkbox").attr("aria-checked", "true");
                    $("#check-all-students-modal-reports").prop("checked", true);
                }
                if ($(this).find("option:selected").length) {
                    $("#apply-dashboard-print-reports").prop("disabled", false).addClass("last-tab-element wcag-modal-last-element");
                    $("#cancel-dashboard-print-reports").removeClass("last-tab-element wcag-modal-last-element");
                } else {
                    $("#apply-dashboard-print-reports").prop("disabled", true).removeClass("last-tab-element wcag-modal-last-element");
                    $("#cancel-dashboard-print-reports").addClass("last-tab-element wcag-modal-last-element");
                }
            });
            $(document).on("change", "select#print-performance-stages", function (e) {
                var selectedPerformanceStages = [];
                $("select#print-performance-stages option").each(function () {
                    if ($(this).prop("selected")) {
                        selectedPerformanceStages.push($(this).val());
                    }
                });
                createStudentsTreeListStructure(selectedPerformanceStages);
            });

            $(document).on("click touchstart", "#apply-dashboard-print-reports", function (e) {
                var params = "";
                $("#treeStudentsMultiSelectDropdownList option:selected").each(function () {
                    if (params !== "") params += ",";
                    params += $(this).val();
                });
                //params = encodeURIComponent(JSON.stringify(params));

                $("#print_differentiated_profile_form").empty();
                $("#print_differentiated_profile_form").append('<input type="hidden" name="input" value="' + params + '">');

                var newPopupTarget = "print_preview_popup" + DmUiLibrary.RandomStringGenerator(5);
                $("#print_differentiated_profile_form").attr("target", newPopupTarget);
                window.open($("#print_differentiated_profile_form").attr("action"), newPopupTarget, "width=1000,height=700,left=0,top=5");
                $("#print_differentiated_profile_form").submit();
            });

            function createStudentsTreeListStructure(arrSelectedPerformanceStages) {
                if (typeof arrSelectedPerformanceStages !== "undefined" && arrSelectedPerformanceStages !== null) {
                    $("#modal-dashboard-print-reports .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox  input").removeAttr("disabled"); //'Select All' checkbox enabled
                    $("#modal-dashboard-print-reports .filter-wrapper + .dm-ui-checkbox-container > .dm-ui-checkbox").removeAttr("aria-disabled").removeClass("dm-ui-disabled").attr("tabindex", 0); //'Select All' checkbox enabled

/*
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
                    var arrStudents = $("#roster-table-wrapper").data("kendoGrid").dataSource.data();
                    arrStudents.sort(compareNames);
*/

                    var arrStudents;
                    var arrStages = ["", "Pre-Emerging", "Emerging", "Beginning", "Transitioning", "Independent"];

                    var arrTree = [];
                    var className = "";
                    var arrTopFilterSelectedBuildingId = [];
                    var arrTopFilterSelectedClassId = [];
                    var arrTopFilterSelectedStudentId = [];
                    var rosterType = $("label#Label_ChildLocations_Control").text().toUpperCase();

                    if (rosterType === "CLASS") {
                        $(".filters .dashboard-filter #ParentLocations_Control option:selected").each(function () {
                            arrTopFilterSelectedBuildingId.push(Number($(this).val()));
                        });
                        $(".filters .dashboard-filter #ChildLocations_Control option:selected").each(function () {
                            arrTopFilterSelectedClassId.push(Number($(this).val()));
                        });
                    }
                    if (rosterType === "STUDENT") {
                        $(".filters .dashboard-filter #ParentLocations_Control option:selected").each(function () {
                            arrTopFilterSelectedClassId.push(Number($(this).val()));
                        });
                        $(".filters .dashboard-filter #ChildLocations_Control option:selected").each(function () {
                            arrTopFilterSelectedStudentId.push(Number($(this).val()));
                        });
                    }

                    private.modelHierarchy.values.forEach(function (itemStudents) {
                        arrStudents = itemStudents.studentList;
                        arrStudents.map(function (x) {
                            if (arrSelectedPerformanceStages.indexOf(x.pldStage) !== -1) {
                                if (rosterType === "CLASS") {
                                    if ($.inArray(itemStudents.buildingId, arrTopFilterSelectedBuildingId) !== -1 && $.inArray(x.classId, arrTopFilterSelectedClassId) !== -1) {
                                        //if ($("#ChildLocations_Control option:selected").length > 1) { //if we don't want to add class name to the tree structure, if only 1 class is available
                                        if (true) {
                                            className = "|" + x.className;
                                        }
                                        arrTree.push(arrStages.indexOf(x.pldStage) + "|PLD Level " + x.pldLevel + className + "||" + x.studentName + "||" + x.studentId);
                                    }
                                }
                                if (rosterType === "STUDENT") {
                                    if (($.inArray(itemStudents.classId, arrTopFilterSelectedClassId) !== -1 || $.inArray(x.classId, arrTopFilterSelectedClassId) !== -1) && $.inArray(x.studentId, arrTopFilterSelectedStudentId) !== -1) {
                                        if ($("#ParentLocations_Control option:selected").length > 1) {
                                            className = "|" + itemStudents.className;
                                        }
                                        arrTree.push(arrStages.indexOf(x.pldStage) + "|PLD Level " + x.pldLevel + className + "||" + x.studentName + "||" + x.studentId);
                                    }
                                }
                            }
                        });
                    });
                    arrTree.sort();

                    for (var i = 0; i < arrTree.length; ++i) {
                        arrTree[i] = arrStages[arrTree[i].substr(0, 1)] + arrTree[i].substr(1);
                    }

                    var arrTreeSelectOptions = [];
                    var arr;
                    arrTree.map(function (x) {
                        arr = x.split("||");
                        arrTreeSelectOptions.push('<option data-group="' + arr[0] + '" value="' + arr[2] + '" selected="selected">' + arr[1] + "</option>");
                    });

                    $("#treeStudentsMultiSelectDropdownList").empty().append(arrTreeSelectOptions.join("")).trigger("DmUi:updated").trigger("change");
                }
            }

            $(document).on("click touchstart", "#achievement-ability", function () {
                var selectedPerformanceBand;
                var selectedDomainBand;
                var selectedDomainCardId;
                Dashboard.ToggleShowOrHideCogatMatrixCard();
                if ($("#achievement-ability").text() === "Show Achievement / Ability") {
                    if ($("#roster-selected-domain-label").text() !== "") {
                        selectedDomainCardId = $("#dashboard-right-column .domain-band.rect-label:not(.grayscale)").parents(".dm-ui-card").attr("id");
                        selectedDomainBand = $("#dashboard-right-column .domain-band.rect-label:not(.grayscale)").data("range");
                        $("#roster-selected-domain-label").empty();
                        Dashboard.clearDomainSelection();
                        $("#" + selectedDomainCardId + " .domain-band.rect-label.band" + selectedDomainBand).click();
                    } else if ($("#roster-selected-quantile-label").text() !== "") {
                        selectedPerformanceBand = $("#barChart4 a.quantile-band.rect-label:not(.grayscale)").data("range");
                        $("#roster-selected-quantile-label").empty();
                        Dashboard.clearQuantileSelection();
                        $("#barChart4 a.quantile-band.rect-label.band" + selectedPerformanceBand).click();
                    } else {
                        Dashboard.GetRoster();
                    }
                } else {
                    Dashboard.GetRoster();
                }
            });
            $(document).on("keydown", "#achievement-ability", function (e) {
                if (e.keyCode === 13 || e.which === 32) {
                    e.preventDefault();
                    $(this).click();
                }
            });

            $(document).on("click touchstart", ".table-matrix td.data", function () {
                if ($(this).hasClass("selected")) {
                    $(".table-matrix td.data").removeClass("selected");
                    if ($(".bread-crumbs span").length >= 3) {
                        private.isCogatRosterTypeStudents = true;
                    }
                    Dashboard.UnlockCogatRosterMatrixDataSelected();
                } else {
                    $(".table-matrix td.data").removeClass("selected");
                    $(this).addClass("selected");
                    private.isCogatRosterTypeStudents = true;
                    Dashboard.LockCogatRosterMatrixDataSelected();
                }
                Dashboard.GetRoster();
            });

            $(document).on("change", "select#flex-cogat-content-type", function (e) {
                Dashboard.GetAchievementAbilitySummary();
                //Dashboard.GetRoster();
            });

            $(document).on("change", "select#Score_Control", function (e) {
                if ($("#roster-name").text() === "Student Roster") {
                    private.isCogatRosterTypeStudents = true;
                }
                Dashboard.GetRoster();
            });

            $(document).on("change", "select#Scope_Control", function (e) {
                if ($("#roster-name").text() === "Student Roster") {
                    private.isCogatRosterTypeStudents = true;
                }
                Dashboard.GetRoster();
            });

            $(document).on("click touchstart", ".cogat-roster-header-button", function () {
                var columnNumber = $(this).parent().index() + 1;
                $(".cogat-roster").removeClass("cogat-score-column-selected-4 cogat-score-column-selected-5 cogat-score-column-selected-6 cogat-score-column-selected-7 cogat-score-column-selected-8 cogat-score-column-selected-9 cogat-score-column-selected-10");
                $(".cogat-roster").addClass("cogat-score-column-selected-" + columnNumber);
                private.cogatRosterSelectedContentScope = $(this).data("score");
                $("#roster-cogat-label-scope-value").text(private.cogatRosterSelectedContentScope);
                var text = $("#roster-table-wrapper table th:nth-child(" + columnNumber + ") .cogat-roster-header-button").text();
                $("#roster-table-wrapper table tr:first-child th:not(:nth-child(2)) span.sr-only").remove();
                $("#roster-table-wrapper table th:nth-child(" + columnNumber + ")").append('<span class="sr-only">Achievement & Ability Summary is filtered by ' + text + "</span>");
                Dashboard.GetAchievementAbilitySummary();
            });
            $(document).on("keydown", ".cogat-roster-header-button", function (e) {
                if (e.keyCode === 13 || e.which === 32) {
                    e.preventDefault();
                    $(this).click();
                }
            });

            $(document).on("click touchstart", "#roster-selected-cogat-label .close", function () {
                var isMatrixSquareSelected = $(".table-matrix td.data.selected").length;
                if (isMatrixSquareSelected) {
                    $(".table-matrix td.data.selected").click();
                } else {
                    $("#achievement-ability").click();
                }
            });
            $(document).on("keydown", "#roster-selected-cogat-label .close", function (e) {
                if (e.keyCode === 13 || e.which === 32 || e.which === 1) {
                    e.preventDefault();
                    $(this).click();
                }
            });

        }
    };
}();


//jQuery Plugin
(function ($) {
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
            if (this.data("tabindex") !== undefined) {
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
                if (this.hasClass("roster-table-card")) {
                    elements = this.find(".tab, #roster-table-wrapper th.k-header, #roster-table-wrapper td, #roster-table-wrapper tbody th, #roster-table-wrapper input.k-textbox, #roster-table-wrapper .k-widget.k-dropdown, #roster-table-wrapper .k-pager-nav, #roster-legend-wrapper");
                    this.find("#roster-legend-wrapper").addClass("last-tab-element");
                }
                elements.each(function () {
                    if ($(this).attr("tabindex-important") !== "true" && !$(this).hasClass("disabled-element") && !$(this).hasClass("k-state-disabled")) {
                        $(this).attr("tabindex", rootTabIndex);
                        if (typeof firstElement === "undefined") {
                            firstElement = $(this);
                        }
                        if (!isFilters) {
                            $(this).addClass("tab");
                        }
                    }
                });
                if (typeof isFocusAtTheEnd !== "undefined" && isFocusAtTheEnd === true) {
                    if ($("body").hasClass("wcag_focuses_on")) {
                        if (typeof firstElement !== "undefined" && firstElement !== null) {
                            if (firstElement.text() !== "") {
                                //firstElement.focus();
                                Dashboard.FocusElementWithDelay(firstElement, 100);

                                //$("body").addClass("wcag_focuses_on");
                                //Dashboard.AddCssClassToElementWithDelay("body", "wcag_focuses_on", 1); //for NVDA screenreader
                            }
                        }
                    }
                }
            }
        }
    };
    $.fn.rootMakeContentNotTabbable = function (isFocusAtTheEnd) {
        if (Dashboard.IsTabNavigationOn()) {
            var self = this;
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
                if (self.hasClass("roster-table-card")) {
                    elements = self.find(".tab, #roster-table-wrapper th.k-header, #roster-table-wrapper td, #roster-table-wrapper tbody th, #roster-table-wrapper input.k-textbox, #roster-table-wrapper .k-widget.k-dropdown, #roster-table-wrapper .k-pager-nav, #roster-legend-wrapper");
                    self.find("#roster-legend-wrapper").addClass("last-tab-element");
                }
                elements.each(function () {
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
