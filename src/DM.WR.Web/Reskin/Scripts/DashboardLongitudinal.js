var DashboardLongitudinal = function () {

    var private = {
        RosterInitialZeroData: {}, //json object with zero values to initialize Roster before getting new json values
        GainsAnalysisInitialZeroData: {}, //json object with zero values to initialize GainsAnalysis card before getting new json values
        modelTrendAnalysis: {}, //last successfully received Performance Band
        modelSelectedTestEvents: {}, //Selected Test Events in Performance Band card
        modelRoster: {}, //last successfully received Roster
        arrRosterLevelRanges: [],
        minRosterValue: 0,
        maxRosterValue: 0,
        minRosterValueOriginal: 0,
        maxRosterValueOriginal: 0,
        rosterZoomStep: 5,
        rosterZoomMultiplier: 0,
        initialScaleShift: 0,
        LastFocusedElement: {},
        SearchAutocompleteFoundNum: 0,
        LoggedUserLevel: "",
        isTabNavigationOn: false,
        defaultTabIndex: "0",
        isProdEnvironment: true
    };

    var siteRoot = "";
    if (private.isTabNavigationOn) {
        private.defaultTabIndex = "-1";
    }

    return {
        Init: function (isProdEnvironment) {
            var uiSettings = DmUiLibrary.GetUiSettings();
            siteRoot = uiSettings.SiteRoot;

            //Fix for svg href clicks: Set a split prototype on the SVGAnimatedString object which will return the split of the baseVal on this
            SVGAnimatedString.prototype.split = function () { return String.prototype.split.apply(this.baseVal, arguments); };

            private.GainsAnalysisInitialZeroData = getEmptyJsonGainsAnalysis();

            this.InitDebuggingQueries(isProdEnvironment);
            this.AssignSelectGroupsHandlers();
            this.FiltersChangeHandler();
            this.AssignAllEventHandlers();

            this.GetFilters(true);

            DmUiLibrary.HideOverlaySpinner();
            $(".dm-ui-overlay-spinner-container.placed-in-block").css("display", "block");
        },

        InitDebuggingQueries: function (isProdEnvironment) {
            if (typeof isProdEnvironment === "string") {
                //Debugging DIV for GraphQL queries
                if (isProdEnvironment === "False") {
                    private.isProdEnvironment = false;
                    window.addEventListener("click", function (evt) {
                        if (evt.target.nodeName === "BODY" && evt.detail === 3) {
                            if ($("#debug-graphql:visible").length) {
                                $("#debug-graphql").hide();
                            } else {
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

        ClearAllErrorsMessages: function () {
            //remove all errors messages and empty data overlays
            $(".empty-json-overlay").removeClass("empty-json-overlay");
            $(".json-error").removeClass("json-error");
            $(".dm-ui-alert.dm-ui-alert-info").remove();
            $(".undefined").remove();
            $(".debug-warning").empty();
        },

        FiltersChangeHandler: function () {
            var self = this;
            $(document).on("change", ".dm-ui-hidden-input", function () {
                DashboardLongitudinal.ClearAllErrorsMessages();
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
                    url: siteRoot + "/IowaFlexLongitudinal/UpdateFilters",
                    data: data,
                    dataType: "json",
                    beforeSend: function () {
                        //DmUiLibrary.ShowOverlaySpinner();
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
/*
            if ($(".bread-crumbs .location-drill").length > 0) {
                isResetPageDisabled = false;
            }
            if (isResetPageDisabled) {
                if ($("#roster-name").text() === "Student Roster") {
                    isResetPageDisabled = false;
                }
            }
*/
            if (isResetPageDisabled) {
                $(".filters .dashboard-filter select.dm-ui-single-select:not(#TestEvent_Control) option:first-child").each(function () {
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

        GetFilters: function (hideSpinnerOnComplete) {
            Dashboard.ClearErrorWarningShowDashboardContent();

            if (!private.isProdEnvironment) {
                $("#debug-graphql-filters textarea").text("");
                $("#debug-graphql-filters span").text("");
            }

            $.ajax({
                type: "GET",
                dataType: "html",
                url: siteRoot + "/IowaFlexLongitudinal/GetFilters",
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
                                breadCrumbsText += "<span>" + arrOfObjBreadCrumbs[i].Text + "</span>";
                            }
                        }
                        $(".bread-crumbs").html(breadCrumbsText);

                        DashboardLongitudinal.CheckIsResetPageButtonDisabled();

                        if (!private.isProdEnvironment) {
                            $("#debug-graphql-filters textarea").text($("#FiltersGraphqlQuery").text());
                            $("#debug-graphql-filters span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        if (private.isTabNavigationOn) {
                            $(".filters").attr("tabindex", "1");
                            $('.filters [tabindex="0"], .filters a, .filters button').each(function () {
                                $(this).attr("tabindex", "-1");
                            });
                            //$("#modal-dashboard-report-criteria [role=alert]").attr("tabindex", "-1").attr("aria-hidden", "true");
                        }

                        DashboardLongitudinal.GetTrendAnalysis();
                    } else {
                        $("#print-report-center").addClass("empty-json-overlay no-message");
                    }
                },
                beforeSend: function () {
                    //DmUiLibrary.ShowOverlaySpinner();
                },
                complete: function () {
                    if (hideSpinnerOnComplete)
                        DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        ResetPerformanceBand: function () {
            $(".performance-band-blocks-wrapper").removeClass("animated");
            $(".performance-band-blocks-wrapper .block").removeAttr("aria-label");
            $(".performance-band-blocks-wrapper .text").text("0%");
            $(".performance-band-blocks-wrapper .filled").attr("style", "width: 0%;");
            $(".performance-band-blocks-wrapper .title span").html("&nbsp;");
            $(".performance-band-blocks-wrapper .title span").removeAttr("aria-label");
            $(".performance-band-blocks-wrapper").addClass("animated");
        },

        ResetPerformanceSummary: function () {
            $(".performance-band-summary-wrapper .percent").text("0%");
            $(".performance-band-summary-wrapper .percent").attr("aria-label", "0%");
            $(".performance-band-summary-wrapper .trend").removeClass("trend-up");
            $("#dropdown-summary-test-event").empty();
            $("#dropdown-summary-test-event").trigger("DmUi:updated");
            $("#performance-summary-label span").empty();
            $("#performance-summary-label span").removeAttr("aria-label");
        },

        AddMissedValues: function (data) {
            var i;
            var dataCompleted = [
                { "range": 1, "percent": 0 },
                { "range": 2, "percent": 0 },
                { "range": 3, "percent": 0 },
                { "range": 4, "percent": 0 },
                { "range": 5, "percent": 0 }
            ];
            for (i = 0; i < data.length; ++i) {
                if (typeof data[i].range !== "undefined") {
                    dataCompleted[data[i].range - 1] = data[i];
                }
            }
            //dataCompleted.sort(function (a, b) { return a.range > b.range });
            return dataCompleted;
        },

        DrawPerformanceSummary: function () {
            var arrDropdownTestEventSelectedId = [];
            var arrDropdownSummaryTestEventSelectedId = [];

            $(".performance-band-summary-wrapper .trend").removeClass("trend-up");
            $(".performance-band-summary-wrapper .right-part").removeClass("trend-zero");

            $("#dropdown-test-event option:selected").each(function (i) {
                arrDropdownTestEventSelectedId.push(i);
            });
            arrDropdownTestEventSelectedId.reverse();
            //console.log(arrDropdownTestEventSelectedId);

            $("#dropdown-summary-test-event option:selected").each(function () {
                arrDropdownSummaryTestEventSelectedId.push(arrDropdownTestEventSelectedId[$(this).index()]);
            });
            arrDropdownSummaryTestEventSelectedId.reverse();
            //console.log(arrDropdownSummaryTestEventSelectedId);

            if (arrDropdownSummaryTestEventSelectedId.length === 2) {
                var id1 = arrDropdownSummaryTestEventSelectedId[0];
                var id2 = arrDropdownSummaryTestEventSelectedId[1];
                var data = private.modelSelectedTestEvents;
                var value, selector;

                data[id1].values = DashboardLongitudinal.AddMissedValues(data[id1].values);
                data[id2].values = DashboardLongitudinal.AddMissedValues(data[id2].values);
                //console.log(data);

                for (var j = 0; j < 5; ++j) {
                    selector = ".performance-band-summary-wrapper .block:nth-child(" + (j + 1) + ")";
                    value = data[id2].values[j].percent - data[id1].values[j].percent;
                    if (value > 0) {
                        $(selector + " .trend").addClass("trend-up");
                    }
                    if (value === 0) {
                        $(selector + " .right-part").addClass("trend-zero");
                    }
                    $(selector + " .percent").text(value + "%");
                    $(selector + " .percent").attr("aria-label", "Level " + (j + 1) + ": " + value + "%");
                }
                //console.log(private.modelSelectedTestEvents);
            }
        },

        DrawPerformanceBand: function () {
            var data = [];
            var blockIndex, selectorRow, value;
            var ariaLabel;

            DashboardLongitudinal.ResetPerformanceBand();
            $(".performance-band-blocks-wrapper").addClass("animated");

            $("#dropdown-test-event option").each(function (i) {
                if ($(this).prop("selected")) {
                    data.push(private.modelTrendAnalysis.model[i]);
                }
            });
            data.reverse();
            private.modelSelectedTestEvents = data;

            for (var i = 0; i < data.length; ++i) {
                data[i].values = DashboardLongitudinal.AddMissedValues(data[i].values);
                ariaLabel = data[i].name + ": ";
                if (i === 1 && data.length === 2) {
                    blockIndex = 2;
                } else {
                    blockIndex = i;
                }
                for (var j = 0; j < 5; ++j) {
                    if (typeof data[i].values[j] !== "undefined" && typeof data[i].values[j].range !== "undefined") {
                        selectorRow = ".performance-band-blocks-wrapper .block:nth-child(" + (blockIndex + 1) + ") .row:nth-child(" + (data[i].values[j].range) + ")";
                        value = data[i].values[j].percent + "%";
                        $(selectorRow + " .text").text(value);
                        $(selectorRow + " .filled").attr("style", "width: " + value + ";");
                        ariaLabel += " Level " + data[i].values[j].range + " - " + data[i].values[j].percent + "%";
                    } else {
                        ariaLabel += " Level " + data[i].values[j].range + " - 0%";
                    }
                    if (j !== 4) {
                        ariaLabel += ";";
                    }
                }
                $(".performance-band-blocks-wrapper .block:nth-child(" + (blockIndex + 1) + ") .title span").text(data[i].name);
                //$(".performance-band-blocks-wrapper .block:nth-child(" + (blockIndex + 1) + ") .title span").attr("aria-label", data[i].name);
                $(".performance-band-blocks-wrapper .block:nth-child(" + (blockIndex + 1) + ")").attr("aria-label", ariaLabel);
            }

            DashboardLongitudinal.InitDropdownSummaryTestEvent();
        },

        GetTrendAnalysis: function () {
            DashboardLongitudinal.ResetPerformanceBand();
            DashboardLongitudinal.ResetPerformanceSummary();
            DashboardLongitudinal.AddSpinner(".performance-band-card");
            DashboardLongitudinal.AddSpinner(".performance-summary-card");

            $.ajax({
                type: "GET",
                dataType: "json",
                url: siteRoot + "/IowaFlexLongitudinal/GetTrendAnalysis",
                success: function (data) {
                    DashboardLongitudinal.RemoveSpinner(".performance-band-card");
                    DashboardLongitudinal.RemoveSpinner(".performance-summary-card");

                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        private.modelTrendAnalysis = data;

                        if (!private.isProdEnvironment) {
                            $("#debug-graphql-trend-analysis textarea").text(data.graph_ql_query);
                            $("#debug-graphql-trend-analysis span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        var isDisabled, isSelected, selectedNum = 0, disabledNotFounded = 1;
                        var options = "";
                        for (var i = 0; i < data.model.length; ++i) {
                            isDisabled = "";
                            isSelected = "";
                            if (data.model[i].is_default) {
                                isDisabled = ' class="dm-ui-disabled"';
                                selectedNum++;
                                isSelected = ' selected="selected"';
                                disabledNotFounded = 0;
                            } else {
                                if (selectedNum < (3 - disabledNotFounded)) {
                                    selectedNum++;
                                    isSelected = ' selected="selected"';
                                }
                            }
                            options += '<option data-date="' + data.model[i].date + '"' + isDisabled + isSelected + ' value="' + data.model[i].id + '">' + data.model[i].name + "</option>";
                        }
                        $("#dropdown-test-event").html(options);
                        $("#dropdown-test-event option:last-child").trigger("click"); //for hiding dm-ui-multi-select-message
                        $("#dropdown-test-event").trigger("change");
                        $("#dropdown-test-event").trigger("DmUi:updated");

                        //DashboardLongitudinal.DrawPerformanceBand();
                        $(".performance-band-card.root-tab-element").rootMakeContentNotTabbable();
                        $(".performance-summary-card.root-tab-element").rootMakeContentNotTabbable();
                    } else {
                        $(".performance-band-card, .performance-summary-card").addClass("empty-json-overlay json-error");
                    }
                },
                complete: function (jqXhr, textStatus) {
                    if (textStatus === "error") {
                    }
                }
            });

/*
            var data = { "model": [] };
            $("#dropdown-test-event option").each(function (i) {
                if ($(this).prop("selected")) {
                    data.model.push(getSampleJsonPerformanceBand(i));
                }
            });
            private.modelTrendAnalysis = data;

            setTimeout(function () {
                DashboardLongitudinal.DrawPerformanceBand();
            }, 500);
*/
        },

        ResetGainsAnalysis: function () {
            DashboardLongitudinal.DrawChartGainsAnalysis(private.GainsAnalysisInitialZeroData, "init");
            $("#table-gains-analysis").empty();
            $("#legend-gains-analysis").empty();
        },

        GetGainsAnalysis: function (testEventIds) {
            DashboardLongitudinal.ResetGainsAnalysis();

            $.ajax({
                type: "GET",
                dataType: "json",
                url: siteRoot + "/IowaFlexLongitudinal/GetGainsAnalysis?testEventIds=" + testEventIds,
                success: function (data) {
                    if (!private.isProdEnvironment) {
                        $("#debug-graphql-gains-analysis textarea").text(data.graph_ql_query);
                        $("#debug-graphql-gains-analysis span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                    }

                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        DashboardLongitudinal.DrawTableGainsAnalysis(data.model);
                        DashboardLongitudinal.DrawChartGainsAnalysis(data.model);
                    } else {
                        $("#dashboard-right-column").addClass("empty-json-overlay json-error");
                    }
                },
                complete: function (jqXhr, textStatus) {
                    if (textStatus === "error") {
                    }
                }
            });

/*
            var data = getSampleJsonGainsAnalysis();
            $("#dropdown-test-event option").each(function (i) {
                if (!$(this).prop("selected")) {
                    data.values.splice(i, 1);
                }
            });

            setTimeout(function () {
                DashboardLongitudinal.DrawChartGainsAnalysis(data);
                DashboardLongitudinal.DrawTableGainsAnalysis(data);
            }, 500);
*/
        },

        ResetRoster: function () {
            $("#roster-table-wrapper").empty();
            $("#roster-table-wrapper").removeClass();
            $("#roster-selected-test-events-label").empty();
            $("#roster-selected-test-events-label").hide();
            this.addViewElements("#roster-table-wrapper", "init");
        },

        GetRoster: function (testEventIds) {
            var arr, i, j, k;

            DashboardLongitudinal.ResetRoster();
            private.rosterZoomMultiplier = 0;

            $.ajax({
                type: "GET",
                dataType: "json",
                url: siteRoot + "/IowaFlexLongitudinal/GetRoster?testEventIds=" + testEventIds,
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        if (data.roster_level === "building") {
                            $("#roster-name").text("Building Score Comparison");
                            //$("#roster-name").attr("aria-label", "Building Score Comparison");
                            $(".roster-table-card").attr("aria-label", "Building Comparison Table");
                        }
                        if (data.roster_level === "class") {
                            $("#roster-name").text("Class Score Comparison");
                            //$("#roster-name").attr("aria-label", "Class Score Comparison");
                            $(".roster-table-card").attr("aria-label", "Class Comparison Table");
                        }
                        if (data.roster_level === "Student") {
                            $("#roster-name").text("Student Roster");
                            //$("#roster-name").attr("aria-label", "Student Roster");
                            $(".roster-table-card").attr("aria-label", "Students Comparison Table");
                        }

                        var arrTestEvents = [];
                        $("#dropdown-test-event option:selected").each(function () {
                            arrTestEvents[Number($(this).val())] = $(this).text();
                        });
                        for (i = 0; i < data.columns.length; ++i) {
                            if (data.columns[i].id !== null) {
                                data.columns[i].title = arrTestEvents[data.columns[i].id];
                                data.columns[i].title_full = data.columns[i].title;
                            }
                        }

                        private.modelRoster = data;

                        if (!private.isProdEnvironment) {
                            $("#debug-graphql-roster textarea").text(data.graph_ql_query);
                            $("#debug-graphql-roster span").text("Time: " + (new Date()).toTimeString().split(" ")[0]);
                        }

                        if (typeof data.bands !== "undefined" && data.bands !== null) {
                            private.arrRosterLevelRanges = [];
                            for (i = 0; i < data.bands.length; ++i) {
                                arr = data.bands[i].range_band.split(":");
                                private.arrRosterLevelRanges.push({ "begin": Number(arr[0]), "end": Number(arr[1]) });
                            }
                        } else {
                            private.arrRosterLevelRanges = [{ "begin": 0, "end": 0 }, { "begin": 0, "end": 0 }, { "begin": 0, "end": 0 }, { "begin": 0, "end": 0 }, { "begin": 0, "end": 0 }];
                        }
                        //console.log(private.arrRosterLevelRanges);

                        var arrFields = [], min = 999, max = 0, min2, max2;
                        for (k = 0; k < data.columns.length; ++k) {
                            if (data.columns[k].field === "SS0" ||
                                data.columns[k].field === "SS1" ||
                                data.columns[k].field === "SS2") {
                                arrFields.push(data.columns[k].field);
                            }
                        }
                        for (k = 0; k < data.values.length; ++k) {
                            for (j = 0; j < arrFields.length; ++j) {
                                if (data.values[k][arrFields[j]] !== null && data.values[k][arrFields[j]] > max) {
                                    max = data.values[k][arrFields[j]];
                                }
                                if (data.values[k][arrFields[j]] !== null && data.values[k][arrFields[j]] < min) {
                                    min = data.values[k][arrFields[j]];
                                }
                            }
                        }
                        //console.log(max);
                        //console.log(min);
                        if (min === max) {
                            min = private.arrRosterLevelRanges[0].begin;
                            max = private.arrRosterLevelRanges[4].end;
                        }
                        min2 = min - (min % 5);
                        max2 = max + (5 - (max % 5));
                        if (min - min2 < 5) {
                            min2 -= 5;
                        }
                        if (max2 - max < 5) {
                            max2 += 5;
                        }
                        //console.log(max2);
                        //console.log(min2);
                        private.minRosterValue = min2;
                        private.maxRosterValue = max2;
                        private.minRosterValueOriginal = min2; 
                        private.maxRosterValueOriginal = max2;

                        //private.modelRoster = data;
                        DashboardLongitudinal.DrawRoster(private.modelRoster);

                        if (data.roster_type === "compare") {
                            $("#print-student-profile").hide();
                            $("#print-dashboard").addClass("last-tab-element");
                            $(".bread-crumbs + .floating-buttons").removeClass("shifted");
                        } else {
                            $("#print-student-profile").show();
                            $("#print-dashboard").removeClass("last-tab-element");
                            $(".bread-crumbs + .floating-buttons").addClass("shifted");
                        }
                        $("#roster-table-wrapper .k-grid-header tr:first-child th:first-child a.k-link").click();
                        $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable();
                        $("#roster-table-wrapper > table").removeAttr("tabindex"); //if not removed -kendo table will be the last focused element on page
                        $("#roster-table-wrapper > table").removeAttr("role");
                        $("#roster-table-wrapper > table thead th[data-role=columnsorter] a").attr("role", "button");
                        setTimeout(function () {
                            $("#roster-table-wrapper > table td, #roster-table-wrapper > table tbody th").removeAttr("role");
                        }, 1000);
                        $("#roster-table-wrapper .k-pager-sizes span.k-input").attr("aria-hidden", "true");
                        //Dashboard.FocusLastFocusedElement();
                    } else {
                        $(".roster-table-card").addClass("empty-json-overlay json-error");
                    }
                },
                complete: function (jqXhr, textStatus) {
                    if (textStatus === "error") {
                    }
                }
            });
        },

        PercentValueLeft: function (value, beginScale, endScale) {
            value -= 0.5; //correction for middle line of the neighbor values
            return ((value - beginScale) * 100 / (endScale - beginScale)).toFixed(3);
        },

        PercentValueWidth: function (value, beginScale, endScale) {
            value += 1; //correction for middle line of the neighbor values
            return (value * 100 / (endScale - beginScale)).toFixed(3);
        },

        BandLevel: function (data) {
            if (data >= data.private.arrRosterLevelRanges[0].end) {
                return 1;
            }
            if (data >= data.private.arrRosterLevelRanges[1].begin &&
                data >= data.private.arrRosterLevelRanges[1].end) {
                return 2;
            }
            if (data >= data.private.arrRosterLevelRanges[2].begin &&
                data >= data.private.arrRosterLevelRanges[2].end) {
                return 3;
            }
            if (data >= data.private.arrRosterLevelRanges[3].begin &&
                data >= data.private.arrRosterLevelRanges[3].end) {
                return 4;
            }
            if (data >= data.private.arrRosterLevelRanges[4].begin) {
                return 5;
            }
            return 0;
        },

        TruncateString: function (str, num) {
            if (str.length <= num) {
                return str;
            }
            return str.slice(0, num) + "..";
        },

        DrawRoster: function (data) {
            var rosterColumns = [];
            var templateScore, templateChart, headerTemplate;
            var i, j;

            $("#roster-table-wrapper").empty();

            for (i = 0; i < data.columns.length; ++i) {
                if (data.columns[i].title.length <= 9 || i === 0) {
                    headerTemplate = data.columns[i].title;
                } else {
                    headerTemplate = DashboardLongitudinal.TruncateString(data.columns[i].title, 9) + '<span class="tooltiptext" role="tooltip">' + data.columns[i].title + "</span>";
                }
                if (i === 0) {
                    if (data.roster_type === "compare") {
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: headerTemplate, template: '<span data-href="#:link#" class="location-drill" data-node-id="#:node_id#" tabindex="' + private.defaultTabIndex + '" role="button">#:node_name#</span>', filterable: { cell: { suggestionOperator: "contains" } } });
                    } else {
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: headerTemplate, template: '<span class="student-link" data-node-id="#:node_id#" tabindex="' + private.defaultTabIndex + '" role="button">#:node_name#</span>', filterable: { cell: { suggestionOperator: "contains" } } });
                    }
                } else {
                    if (data.columns[i].field === "SS0" ||
                        data.columns[i].field === "SS1" ||
                        data.columns[i].field === "SS2") {
                        //rosterColumns.push({ field: data.columns[i].field, headerTemplate: headerTemplate, template: data.values[data.columns[i].field] });
                        //rosterColumns.push({ field: data.columns[i].field, headerTemplate: headerTemplate, template: "#= (" + data.columns[i].field + " == null) ? '*' : '<i></i>' + " + data.columns[i].field + " #" });
                        templateScore = "# if (" + data.columns[i].field + " == null) { #" +
                            "<span class='wrap'>*</span> \n" +
                            "# } else if (" + data.columns[i].field + " <= " + private.arrRosterLevelRanges[0].end + ") { #" +
                                "<span class='wrap'><i class='level1'></i> #=" + data.columns[i].field + "#</span>\n" + 
                            "# } else if (" + data.columns[i].field + " >= " + private.arrRosterLevelRanges[1].begin + " && " + data.columns[i].field + " <= " + private.arrRosterLevelRanges[1].end + ") { #" +
                                "<span class='wrap'><i class='level2'></i> #=" + data.columns[i].field + "#</span>\n" + 
                            "# } else if (" + data.columns[i].field + " >= " + private.arrRosterLevelRanges[2].begin + " && " + data.columns[i].field + " <= " + private.arrRosterLevelRanges[2].end + ") { #" +
                                "<span class='wrap'><i class='level3'></i> #=" + data.columns[i].field + "#</span>\n" + 
                            "# } else if (" + data.columns[i].field + " >= " + private.arrRosterLevelRanges[3].begin + " && " + data.columns[i].field + " <= " + private.arrRosterLevelRanges[3].end + ") { #" +
                                "<span class='wrap'><i class='level4'></i> #=" + data.columns[i].field + "#</span>\n" + 
                            "# } else if (" + data.columns[i].field + " >= " + private.arrRosterLevelRanges[4].begin + ") { #" +
                                "<span class='wrap'><i class='level5'></i> #=" + data.columns[i].field + "#</span>\n" + 
                            "# } else { #" +
                                "<span class='wrap'>*</span>" +
                            "# } #";
                        //console.log(templateScore);
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: headerTemplate, template: templateScore, filterable: false });
                    } else {
                        rosterColumns.push({ field: data.columns[i].field, headerTemplate: headerTemplate, filterable: false });
                    }
                }
                if (i === data.columns.length - 1) {
                    var levelLeft, levelWidth;
                    var wrapperLeft, wrapperWidth;
                    templateChart = "";
                    for (j = 0; j < 5; ++j) {
                        levelLeft = DashboardLongitudinal.PercentValueLeft(private.arrRosterLevelRanges[j].begin, private.minRosterValue, private.maxRosterValue);
                        levelWidth = DashboardLongitudinal.PercentValueWidth(private.arrRosterLevelRanges[j].end - private.arrRosterLevelRanges[j].begin, private.minRosterValue, private.maxRosterValue);
                        templateChart += '<div class="bg-level' + (j + 1) + '" style="left:' + levelLeft + "%; width:" + levelWidth + '%"></div>\n';
                    }

                    //add square icon
                    templateChart += "# if (" + data.columns[1].field + " == null) {" +
                            "} else if (" + data.columns[1].field + " <= " + private.arrRosterLevelRanges[0].end + ") { #" +
                                "<i class='level1 square' style='left: #= (" + data.columns[1].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" + 
                            "# } else if (" + data.columns[1].field + " >= " + private.arrRosterLevelRanges[1].begin + " && " + data.columns[1].field + " <= " + private.arrRosterLevelRanges[1].end + ") { #" +
                                "<i class='level2 square' style='left: #= (" + data.columns[1].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" +
                            "# } else if (" + data.columns[1].field + " >= " + private.arrRosterLevelRanges[2].begin + " && " + data.columns[1].field + " <= " + private.arrRosterLevelRanges[2].end + ") { #" +
                                "<i class='level3 square' style='left: #= (" + data.columns[1].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" + 
                            "# } else if (" + data.columns[1].field + " >= " + private.arrRosterLevelRanges[3].begin + " && " + data.columns[1].field + " <= " + private.arrRosterLevelRanges[3].end + ") { #" +
                                "<i class='level4 square' style='left: #= (" + data.columns[1].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" + 
                            "# } else if (" + data.columns[1].field + " >= " + private.arrRosterLevelRanges[4].begin + ") { #" +
                                "<i class='level5 square' style='left: #= (" + data.columns[1].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" + 
                            "# } #";

                    //add circle icon
                    if (data.columns.length > 2 && data.columns[2].field === "SS1") {
                        templateChart += "# if (" + data.columns[2].field + " == null) {" +
                            "} else if (" + data.columns[2].field + " <= " + private.arrRosterLevelRanges[0].end + ") { #" +
                                "<i class='level1 circle' style='left: #= (" + data.columns[2].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" +
                            "# } else if (" + data.columns[2].field + " >= " + private.arrRosterLevelRanges[1].begin + " && " + data.columns[2].field + " <= " + private.arrRosterLevelRanges[1].end + ") { #" +
                                "<i class='level2 circle' style='left: #= (" + data.columns[2].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" +
                            "# } else if (" + data.columns[2].field + " >= " + private.arrRosterLevelRanges[2].begin + " && " + data.columns[2].field + " <= " + private.arrRosterLevelRanges[2].end + ") { #" +
                                "<i class='level3 circle' style='left: #= (" + data.columns[2].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" +
                            "# } else if (" + data.columns[2].field + " >= " + private.arrRosterLevelRanges[3].begin + " && " + data.columns[2].field + " <= " + private.arrRosterLevelRanges[3].end + ") { #" +
                                "<i class='level4 circle' style='left: #= (" + data.columns[2].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" +
                            "# } else if (" + data.columns[2].field + " >= " + private.arrRosterLevelRanges[4].begin + ") { #" +
                                "<i class='level5 circle' style='left: #= (" + data.columns[2].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -9px;'></i>\n" +
                            "# } #";
                    }

                    //add star icon
                    if (data.columns.length > 3 && data.columns[3].field === "SS2") {
                        templateChart += "# if (" + data.columns[3].field + " == null) {" +
                            "} else if (" + data.columns[3].field + " <= " + private.arrRosterLevelRanges[0].end + ") { #" +
                                "<i class='level1 star' style='left: #= (" + data.columns[3].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -10px;'></i>\n" +
                            "# } else if (" + data.columns[3].field + " >= " + private.arrRosterLevelRanges[1].begin + " && " + data.columns[3].field + " <= " + private.arrRosterLevelRanges[1].end + ") { #" +
                                "<i class='level2 star' style='left: #= (" + data.columns[3].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -10px;'></i>\n" +
                            "# } else if (" + data.columns[3].field + " >= " + private.arrRosterLevelRanges[2].begin + " && " + data.columns[3].field + " <= " + private.arrRosterLevelRanges[2].end + ") { #" +
                                "<i class='level3 star' style='left: #= (" + data.columns[3].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -10px;'></i>\n" +
                            "# } else if (" + data.columns[3].field + " >= " + private.arrRosterLevelRanges[3].begin + " && " + data.columns[3].field + " <= " + private.arrRosterLevelRanges[3].end + ") { #" +
                                "<i class='level4 star' style='left: #= (" + data.columns[3].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -10px;'></i>\n" +
                            "# } else if (" + data.columns[3].field + " >= " + private.arrRosterLevelRanges[4].begin + ") { #" +
                                "<i class='level5 star' style='left: #= (" + data.columns[3].field + " - " + private.minRosterValue + ") * 100 / " + (private.maxRosterValue - private.minRosterValue) + " #%; margin-left: -10px;'></i>\n" +
                            "# } #";
                    }
                    //console.log(private.minRosterValue);
                    //console.log(private.maxRosterValue);
                    //wrapperLeft = DashboardLongitudinal.PercentValueLeft(private.minRosterValue, private.minRosterValue, private.maxRosterValue);
                    //wrapperWidth = DashboardLongitudinal.PercentValueWidth(private.maxRosterValue - private.minRosterValue, private.minRosterValue, private.maxRosterValue);
                    wrapperLeft = 0;
                    wrapperWidth = 100;
                    templateChart = '<span class="sr-only">Performance band graph of #:node_name#</span><div class="wrapper-scale" style="left: ' + wrapperLeft + "%; width: " + wrapperWidth + '%;">' + templateChart;
                    templateChart += "</div>";
                    //console.log(templateChart);
                    rosterColumns.push({ field: data.columns[i].field, headerTemplate: "<div id='roster-scale-wrapper'><div id='roster-scale'></div></div><span class='scale-minus tab' tabindex='" + private.defaultTabIndex + "' title='Zoom Out' role='button'>-</span> <span class='scale-reset tab' tabindex='" + private.defaultTabIndex + "' title='Zoom Reset' role='button'>Grade " + $("#Grade_Control option:selected").text() + "</span> <span class='scale-plus tab' tabindex='" + private.defaultTabIndex + "' title='Zoom In' role='button'>+</span>", template: templateChart, sortable: false, filterable: false });
                } 
            }

            try {
                $("#roster-table-wrapper").kendoGrid({
                    dataSource: {
                        data: data.values,
                        pageSize: 25
                    },
                    pageable: {
                        input: true,
                        numeric: false,
                        alwaysVisible: true,
                        pageSizes: [5, 10, 25, 50],
                        change: function () {
                            //console.log("pager change event");
                            Dashboard.WcagPagination("#roster-table-wrapper");
                        }
                    },
                    resizable: true,
                    scrollable: false,
                    filterable: {
                        mode: "row"
                    },
                    persistSelection: true, //see parameter in schema->model-> id: "node_id",
                    sortable: {
                        mode: "single",
                        showIndexes: true,
                        allowUnsort: true
                    },
                    sort: function () {
                        //$(".kendo_sorted_column").addClass("grid-sort-filter-icon k-icon");
                    },
                    dataBound: function () {
                        $("#roster-table-wrapper > table tbody tr td:first-child").attr("scope", "row");
                        $("#roster-table-wrapper > table tbody tr td:first-child").each(function () {
                            $(this).replaceWith($(this)[0].outerHTML.replace("<td ", "<th ").replace("</td>", "</th>"));
                        });
                        if (!private.isTabNavigationOn) {
                            $("#roster-table-wrapper > table thead tr th[data-role=\"columnsorter\"]").addClass("tab").attr("tabindex", "0");
                        }
                        //console.log(private.initialScaleShift);
                        setTimeout(function () {
                            Dashboard.WcagPagination("#roster-table-wrapper");
                            DashboardLongitudinal.MakeScalesDraggable();
                            document.getElementById("roster-scale").style.left = Number(private.initialScaleShift) + "px";
                        }, 100);
                    },
                    navigatable: true,
                    navigate: function (e) {
                        Dashboard.setGridCellCurrentState(e.element);
                        $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                        e.element.focus();
                        //private.RosterGridNavigationEvent = true;
                        Wcag.SetRosterGridNavigationEvent(true);
                    },
                    columns: rosterColumns
                    //columns: data.columns
                });
            } catch (e) {
                $("body").append('<div class="kendo-errors">Error RightCardTable=' + e.name + ": " + e.message + " | " + e.stack + "</div>");
            }
            DashboardLongitudinal.moveOutRosterFilters();
            DashboardLongitudinal.insertTooltipsToHeaderOfRosterTable(data.columns);
            $("#roster-table-wrapper thead th").addClass("tooltip");
            DashboardLongitudinal.DrawRosterScale({ min: private.minRosterValue, max: private.maxRosterValue, width: $("#roster-scale-wrapper").width() });
        },

        WcagPagination: function (element) {
            if (typeof $(element) !== "undefined") {
                $(element + " .k-pager-wrap > a.k-pager-nav").removeAttr("aria-disabled");
                $(element + " .k-pager-wrap > a.k-pager-nav.k-state-disabled").attr("aria-disabled", "true");
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

        MakeScalesDraggable: function () {
            var padding;
            var element;
            var level1Offset;
            var level5Offset;
            var cellOffsetLeft;
            var cellOffsetRight;
            var minShiftLeft;
            var maxShiftLeft;

            for (var i = 0; i < document.getElementsByClassName("wrapper-scale").length; ++i) {
                element = document.getElementsByClassName("wrapper-scale")[i];
                level1Offset = cumulativeOffsetLeft(element.getElementsByClassName("bg-level1")[0]);
                level5Offset = cumulativeOffsetLeft(element.getElementsByClassName("bg-level5")[0]) + element.getElementsByClassName("bg-level5")[0].offsetWidth;
                cellOffsetLeft = cumulativeOffsetLeft(element.parentElement);
                cellOffsetRight = cumulativeOffsetLeft(element.parentElement) + element.parentElement.offsetWidth;
                padding = 100;
                //padding = element.parentElement.offsetWidth - 100;
                maxShiftLeft = cellOffsetRight - level1Offset - padding;
                minShiftLeft = cellOffsetLeft - level5Offset + padding;
                dragElement(document.getElementsByClassName("wrapper-scale")[i]);
            }

            function cumulativeOffsetLeft(element) {
                var left = 0;
                do {
                    left += element.offsetLeft || 0;
                    element = element.offsetParent;
                } while (element);
                return left;
            };

            function dragElement(element) {
                var pos1 = 0, pos3 = 0;
                //var pos2 = 0, pos4 = 0;
                element.onmousedown = dragMouseDown;

                function dragMouseDown(e) {
                    e = e || window.event;
                    e.preventDefault();
                    // get the mouse cursor position at startup:
                    pos3 = e.clientX;
                    //pos4 = e.clientY;
                    document.onmouseup = closeDragElement;
                    // call a function whenever the cursor moves:
                    document.onmousemove = elementDrag;
                }

                function elementDrag(e) {
                    e = e || window.event;
                    e.preventDefault();
                    // calculate the new cursor position:
                    pos1 = pos3 - e.clientX;
                    //pos2 = pos4 - e.clientY;
                    pos3 = e.clientX;
                    //pos4 = e.clientY;
/*
                    // set the element's new position:
                    //element.style.top = (element.offsetTop - pos2) + "px";
                    element.style.left = (element.offsetLeft - pos1) + "px";
*/

                    var leftShift = (element.offsetLeft - pos1);
                    if (leftShift > maxShiftLeft) {
                        leftShift = maxShiftLeft;
                    }
                    if (leftShift < minShiftLeft) {
                        leftShift = minShiftLeft;
                    }
                    document.getElementById("roster-scale").style.left = Number(private.initialScaleShift) + Number(leftShift) + "px";
                    leftShift += "px";
                    for (var i = 0; i < document.getElementsByClassName("wrapper-scale").length; ++i) {
                        document.getElementsByClassName("wrapper-scale")[i].style.left = leftShift;
                    }
                }

                function closeDragElement() {
                    // stop moving when mouse button is released:
                    document.onmouseup = null;
                    document.onmousemove = null;
                }
            }
        },

        InitDropdownSummaryTestEvent: function () {
            $("#dropdown-summary-test-event").empty();
            $("#dropdown-test-event option").each(function () {
                if ($(this).prop("selected")) {
                    $("#dropdown-summary-test-event").append($(this)[0].outerHTML);
                }
            });
            $("#dropdown-summary-test-event option").each(function () {
                $(this).attr("selected", "selected");
            });
            if ($("#dropdown-test-event option:selected").length >= 3) {
                $("#dropdown-summary-test-event option:nth-child(2)").prop("selected", false);
            }
            //$("#dropdown-summary-test-event option:first-child").trigger("click"); //for hiding dm-ui-multi-select-message
            $("#dropdown-summary-test-event option").removeClass("dm-ui-disabled");
            $("#dropdown-summary-test-event").trigger("DmUi:updated");
            $("#dropdown-summary-test-event").trigger("change");
        },

        DrawRosterScale: function (data, viewType) {
            this.addViewElements("#roster-scale", viewType);
            $("#roster-scale").RosterScale({
                'data': data
            });
            private.initialScaleShift = document.getElementById("roster-scale").style.left.replace("px", "");
            if (private.initialScaleShift === "") {
                private.initialScaleShift = "0";
            }
/*
            //uncomment for scales in each roster row
            var scale = $("#roster-scale-wrapper").clone();
            scale.find("#roster-scale").addClass("roster-scale").removeAttr("id");
            $("#roster-table-wrapper table tbody td:last-child .wrapper-scale").append(scale.html());
*/
        },

        DrawChartGainsAnalysis: function (data, viewType) {
            this.addViewElements("#chart-gains-analysis", viewType);
            $("#chart-gains-analysis").ChartGainsAnalysis({
                'data': data,
                'loggedUserLevel': private.LoggedUserLevel
            });
        },

        DrawTableGainsAnalysis: function (data) {
            var i, html, isAllValuesNull;
            html = '<table class="dm-ui-table" cellspacing="0" cellpadding="0" border="0">';
            html += "<thead>";
            html += "<th></th>";
            //console.log(data.values);
            for (i = 0; i < data.values.length; ++i) {
                if (data.values[i].da !== null) {
                    private.LoggedUserLevel = "DISTRICT";
                }
                if (data.values[i].sa !== null) {
                    private.LoggedUserLevel = "BUILDING";
                }
                if (data.values[i].ca !== null) {
                    private.LoggedUserLevel = "CLASS";
                }
                if (data.values[i].title.length > 9) {
                    html += '<th class="tooltip">' + DashboardLongitudinal.TruncateString(data.values[i].title, 9) + '<span class="tooltiptext" role="tooltip">' + data.values[i].title + "</span></th>";
                } else {
                    html += "<th>" + data.values[i].title + "</th>";
                }
            }
            html += "<th>Gain</th>";
            html += "</thead>";
            html += "<tbody>";

            isAllValuesNull = true;
            for (i = 0; i < data.values.length; ++i) {
                if (data.values[i].ca !== null) {
                    isAllValuesNull = false;
                    break;
                }
            }
            if (private.LoggedUserLevel === "CLASS" && !isAllValuesNull) {
                html += "<tr>";
                html += "<th>CA</th>";
                for (i = 0; i < data.values.length; ++i) {
                    html += "<td>" + data.values[i].ca + "</td>";
                }
                html += "<td>" + (data.values[data.values.length - 1].ca - data.values[0].ca) + "</td>";
                html += "</tr>";
                $("#legend-gains-analysis").append('<div class="block"><i class="ca"></i><div aria-label="Class Average">Class<br> Average (CA)</div></div>');
            }

            isAllValuesNull = true;
            for (i = 0; i < data.values.length; ++i) {
                if (data.values[i].sa !== null) {
                    isAllValuesNull = false;
                    break;
                }
            }
            if ((private.LoggedUserLevel === "CLASS" || private.LoggedUserLevel === "BUILDING") && !isAllValuesNull) {
                html += "<tr>";
                html += "<th>SA</th>";
                for (i = 0; i < data.values.length; ++i) {
                    html += "<td>" + data.values[i].sa + "</td>";
                }
                html += "<td>" + (data.values[data.values.length - 1].sa - data.values[0].sa) + "</td>";
                html += "</tr>";
                $("#legend-gains-analysis").append('<div class="block"><i class="sa"></i><div aria-label="School Average">School<br> Average (SA)</div></div>');
            }

            isAllValuesNull = true;
            for (i = 0; i < data.values.length; ++i) {
                if (data.values[i].da !== null) {
                    isAllValuesNull = false;
                    break;
                }
            }
            if ((private.LoggedUserLevel === "CLASS" || private.LoggedUserLevel === "BUILDING" || private.LoggedUserLevel === "DISTRICT") && !isAllValuesNull) {
                html += "<tr>";
                html += "<th>DA</th>";
                for (i = 0; i < data.values.length; ++i) {
                    html += '<td aria-label="' + data.values[i].title + ": District Average " + data.values[i].da + '">' + data.values[i].da + "</td>";
                }
                html += '<td aria-label="Gain: District Average ' + (data.values[data.values.length - 1].da - data.values[0].da) + '">' + (data.values[data.values.length - 1].da - data.values[0].da) + "</td>";
                html += "</tr>";
                $("#legend-gains-analysis").append('<div class="block"><i class="da"></i><div aria-label="District Average">District<br> Average (DA)</div></div>');
            }
            html += "</tbody>";
            html += "</table>";
            $("#table-gains-analysis").append(html);
        },

        NoData: function () {
            if ($(".dm-ui-alert.dm-ui-alert-info").length === 0) {
                DmUiLibrary.DisplayAlert({ "Message": "There is no data available for the selection, please re-select your criteria and try again.", "HtmlClass": "dm-ui-alert dm-ui-alert-info", "IsDismissable": true }, "json");
                $(".dm-ui-page-container").hide();
            }
        },

        AddSpinner: function (selector) {
            var style = "";
            if (selector === ".performance-band-card") {
                style = "margin: 23px 0 0 -1px;";
            }
            if (selector === ".performance-summary-card") {
                style = "margin: -35px 0 0 -25px;";
            }
            var spinner = '<div class="spinner" style="' + style + '"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div></div>';
            $(selector).append(spinner);
        },

        RemoveSpinner: function (selector) {
            $(selector + " div.spinner").remove();
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
            if (viewType === "empty") {
                if (selector === "#barChart4" || selector === "#roster-table-wrapper") {
                    $(selector).parents(".section-card").addClass("empty-json-overlay");
                }
            }
            if (viewType === "error") {
                if (selector === "#barChart4" || selector === "#roster-table-wrapper") {
                    $(selector).parents(".section-card").addClass("empty-json-overlay");
                    $(selector).parents(".section-card").addClass("json-error");
                }
            }
        },

        insertTooltipsToHeaderOfRosterTable: function (rosterStructure) {
            var arrTooltips = [];
            var column;
            for (var key in rosterStructure) {
                if (rosterStructure.hasOwnProperty(key)) {
                    column = rosterStructure[key];
                    if (column["title_full"] !== column["title"]) {
                        arrTooltips.push(column["title_full"]);
                    } else {
                        arrTooltips.push("");
                    }
                }
            }

            $("#roster-table-wrapper table thead tr:first-child th").each(function (i) {
                if (typeof arrTooltips[i] != "undefined") {
                    if (arrTooltips[i] !== "") {
                        $(this).addClass("tooltip");
                        $(this).append('<span class="tooltiptext" role="tooltip">' + arrTooltips[i] + "</span>");
                    }
                }
                i++;
            });
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
                            $(this).attr("placeholder", "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name");
                            $(this).attr("aria-label", "Search");
                            $(this).attr("id", "roster-search");
                            $(this).attr("aria-describedby", "wcag-autocomplete-search-instructions");
                            $("#roster-search-field").prepend('<label class="floating-label" id="roster-search-label" for="roster-search">' + "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name" + '</label>');
                            $(this).prev("input").attr("placeholder", "Enter a " + fieldFullTitle.substr(0, fieldFullTitle.indexOf(" ")) + " Name");
                        }
                        $(this).attr("data-full-title", fieldFullTitle);
                        $(this).prev("input").attr("data-full-title", fieldFullTitle);

                        if (private.isTabNavigationOn) {
                            $(this).attr("tabindex", $(this).parents(".root-tab-element").data("tabindex")); //add tabindex for search field
                            $(this).parents(".k-filtercell").find(".k-button-icon").attr("tabindex", $(this).parents(".root-tab-element").data("tabindex")); //add tabindex for search criteria icon
                        }
                        $(this).addClass("tab");
                        $(this).parents(".k-filtercell").find(".k-button-icon").addClass("tab");
                    }
                });
            }
        },

        ZoomRosterGraphs: function () {
            var newMin = private.minRosterValueOriginal + private.rosterZoomStep * private.rosterZoomMultiplier;
            var newMax = private.maxRosterValueOriginal - private.rosterZoomStep * private.rosterZoomMultiplier;
            var currentScrollPosition;
            if (newMin < newMax) {
                private.minRosterValue = newMin;
                private.maxRosterValue = newMax;
                currentScrollPosition = document.documentElement.scrollTop;
                DashboardLongitudinal.DrawRoster(private.modelRoster);
                $("html").stop().animate({
                    scrollTop: currentScrollPosition
                }, 0);
            } else {
                private.rosterZoomMultiplier--;
            }
            if ($("body").hasClass("wcag_focuses_on")) {
                $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
            }
/*
            if (private.rosterZoomMultiplier > 0) {
                DashboardLongitudinal.DrawRosterScale({ min: private.minRosterValue, max: private.maxRosterValue, width: $("#roster-scale-wrapper").width() });
            }
*/
        },

        FocusLastFocusedElement: function () {
            if (!$.isEmptyObject(private.LastFocusedElement)) {
                private.LastFocusedElement.focus();
                $("body").addClass("wcag_focuses_on");
            }
        },

        AssignAllEventHandlers: function () {
            $(document).on("change", "#dropdown-test-event", function () {
                if ($(this).find("option:selected").length < 3) {
                    $(".performance-band-blocks-wrapper .block:nth-child(2)").hide();
                } else {
                    $(".performance-band-blocks-wrapper .block:nth-child(2)").show();
                }
                //DashboardLongitudinal.GetTrendAnalysis();
                DashboardLongitudinal.DrawPerformanceBand();

                var arrSelectedTestEventIds = [];
                $("#dropdown-test-event option:selected").each(function () {
                    arrSelectedTestEventIds.push($(this).val());
                });
                arrSelectedTestEventIds.reverse();
                var selectedTestEventIds = arrSelectedTestEventIds.join(",");
                DashboardLongitudinal.GetGainsAnalysis(selectedTestEventIds);
                DashboardLongitudinal.GetRoster(selectedTestEventIds);

                var defaultTestEventId = $("#dropdown-test-event option.dm-ui-disabled").val();
                var testEvent1 = $("#dropdown-test-event option:selected").last();
                var testEvent2 = $("#dropdown-test-event option:selected").first();
                if (testEvent1.val() === defaultTestEventId) {
                    $("#roster-selected-test-events-label").html('<div aria-label="' + testEvent1.text() + " through " + testEvent2.text() + '"><div class="arrow"></div> <strong>' + testEvent1.text() + "</strong> — " + testEvent2.text() + "</div>");
                } else if (testEvent2.val() === defaultTestEventId) {
                    $("#roster-selected-test-events-label").html('<div aria-label="' + testEvent1.text() + " through " + testEvent2.text() + '"><div class="arrow"></div> ' + testEvent1.text() + " — <strong>" + testEvent2.text() + "</strong></div>");
                } else {
                    $("#roster-selected-test-events-label").html('<div aria-label="' + testEvent1.text() + " through " + testEvent2.text() + '"><div class="arrow"></div> ' + testEvent1.text() + " — " + testEvent2.text() + "</div>");
                }
                $("#roster-selected-test-events-label").show();
            });

            $(document).on("change", "#dropdown-summary-test-event", function () {
                var placeholder = "";
                DashboardLongitudinal.DrawPerformanceSummary();
                $("#dropdown-summary-test-event option:selected").each(function () {
                    if (placeholder !== "") {
                        placeholder = " — " + placeholder;
                    }
                    placeholder = $(this).text() + placeholder;
                });
                setTimeout(function () {
                    $("#dropdown-summary-test-event_dm_ui > button").text(placeholder);
                    $("#performance-summary-label span").text(placeholder);
                    //$("#performance-summary-label span").attr("aria-label", placeholder);
                }, 1);
            });

            $(document).on("keyup", ".scale-minus", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $(this).click();
                    $(".scale-minus").focus();
                }
            });
            $(document).on("click touchstart", ".scale-minus", function () {
                if (private.rosterZoomMultiplier > -30) {
                    //console.log("-");
                    private.rosterZoomMultiplier--;
                    DashboardLongitudinal.ZoomRosterGraphs();
                }
            });
            $(document).on("keyup", ".scale-plus", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $(this).click();
                    //$(".roster-table-card").masterMakeInnerRootsTabbable(true);
                    //DashboardLongitudinal.setGridCellCurrentState(e.element);
                    $(".scale-plus").focus();
                }
            });

            $(document).on("click touchstart", ".scale-plus", function () {
                if (private.rosterZoomMultiplier < 30) {
                    //console.log("+");
                    private.rosterZoomMultiplier++;
                    DashboardLongitudinal.ZoomRosterGraphs();
                }
            });
            $(document).on("keyup", ".scale-reset", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $(this).click();
                    $(".scale-reset").focus();
                }
            });
            $(document).on("click touchstart", ".scale-reset", function () {
                private.rosterZoomMultiplier = 0;
                DashboardLongitudinal.ZoomRosterGraphs();
            });
            $(document).on("click touchstart", "#button-back-to-flex", function (e) {
                e.preventDefault();
                $("#modal-dashboard-save-filter-changes").fadeIn("fast");
                private.LastFocusedElement = $(this);
                $("#modal-dashboard-save-filter-changes .first-tab-element").focus();
                if (!$("body").hasClass("wcag_focuses_on")) {
                    $("#modal-dashboard-save-filter-changes .first-tab-element").blur();
                }
            });
            $(document).on("keydown", "#button-back-to-flex", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    $("#button-back-to-flex").click();
                }
            });
            $(document).on("keyup", "#modal-dashboard-save-filter-changes", function (e) {
                if (e.keyCode === 27) {
                    $("#modal-dashboard-save-filter-changes .close_icon").click();
                }
            });
            $(document).on("click touchstart", "#modal-dashboard-save-filter-changes .close_icon", function () {
                $("#modal-dashboard-save-filter-changes").fadeOut("fast");
                DashboardLongitudinal.FocusLastFocusedElement();
            });
            $(document).on("keydown", "#modal-dashboard-save-filter-changes .close_icon", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    $(this).click();
                }
            });
            $(document).on("click touchstart", "#modal-dashboard-save-filter-changes .dm-ui-button-primary", function () {
                $.ajax({
                    type: "POST",
                    url: siteRoot + "/IowaFlexLongitudinal/PersistLongitudinalFilters",
                    data: JSON.stringify({ persist: true}),
                    dataType: "json",
                    success: function (data) {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType) && data === "Success") {
                            window.location.href = $("#button-back-to-flex").attr("href");
                        } else {
                            $("#modal-dashboard-save-filter-changes").fadeOut("fast", function () { });
                            $(".filters").addClass("empty-json-overlay json-error");
                        }
                    }
                });
            });
            $(document).on("click touchstart", "#modal-dashboard-save-filter-changes .dm-ui-button-secondary", function () {
                $.ajax({
                    type: "POST",
                    url: siteRoot + "/IowaFlexLongitudinal/PersistLongitudinalFilters",
                    data: JSON.stringify({ persist: false }),
                    dataType: "json",
                    success: function (data) {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType) && data === "Success") {
                            window.location.href = $("#button-back-to-flex").attr("href");
                        } else {
                            $("#modal-dashboard-save-filter-changes").fadeOut("fast", function () { });
                            $(".filters").addClass("empty-json-overlay json-error");
                        }
                    }
                });
            });

            $(document).on("click touchstart", ".k-animation-container .k-list-scroller ul li", function () {
                //user clicked on some search autofill list value
                $("#roster-search-label").addClass("visible");
            });
            $(document).on("click touchstart", "#roster-top-buttons-wrapper .k-filtercell button.k-button-icon", function () {
                //Clear Search field button click
                $("#roster-search-label").removeClass("visible");
            });

            //***** Location drill link ajax request *****
            $(document).on("click touchstart", ".location-drill", function (e) {
                e.preventDefault();
                DmUiLibrary.ShowOverlaySpinner();
                DmUiLibrary.AbortAllAjaxRequests(); //abort all Ajax requests
                DashboardLongitudinal.ClearAllErrorsMessages();
                var link = $(this).data("href");
                $.ajax({
                    type: "GET",
                    dataType: "json",
                    url: link,
                    success: function (data) {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            if (data === "success" || data === '"success"') {
                                DashboardLongitudinal.GetFilters(true);
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

            //***** Filters button 'Reset Page' *****
            $(document).on("click touchstart", ".dashboard-filter.button button", function () {
                //$(".bread-crumbs .location-drill:first-child").click();
                $(this).addClass("disabled-element");
                $.ajax({
                    type: "GET",
                    url: siteRoot + "/IowaFlexLongitudinal/ResetFilters",
                    dataType: "json",
                    beforeSend: function () {
                        DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                            DashboardLongitudinal.GetFilters(true);
                        }
                    },
                    error: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
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
                    if (private.isTabNavigationOn && $(".roster-table-card.root-tab-element").attr("tabindex") === "-1") {
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
        }
    };
}();
