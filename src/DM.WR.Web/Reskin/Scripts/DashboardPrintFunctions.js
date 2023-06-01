var Print = function () {

    return {
        Init: function () {
            this.AssignAllEventHandlers();
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
                postPdfProfileNarrative(params);
            });
            $(document).on("click touchstart", ".student-link", function (e) {
                e.preventDefault();
                var params = "input=" + $(this).data("node-id");
                postPdfProfileNarrative(params);
            });
            function postPdfProfileNarrative(params) {
                //var 2) pdf conversion with POST request
                var newPopupTarget = "print_preview_popup" + DmUiLibrary.RandomStringGenerator(5);
                params = params.split("=");
                $("#print_profile_narrative_form").empty();
                $("#print_profile_narrative_form").append("<input type=\"hidden\" name=\"" + params[0] + '" value="' + params[1] + '">');
                $("#print_profile_narrative_form").attr("target", newPopupTarget);
                //window.open("", newPopupTarget, "width=1000,height=600");
                window.open($("#print_profile_narrative_form").attr("action"), newPopupTarget, "width=1340,height=700,left=0,top=5");
                $("#print_profile_narrative_form").submit();
            }
        }
    };
}();
