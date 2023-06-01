var DmUiLibrary = function () {

    var private = {
        UiSettings: {},

        SessionTimeElapsed: 0,
        SessionCheckInterval: 5,//seconds

        AllAjaxRequests: [],

        isDropdownFocusOut: false,
        isDropdownFocusIn: false,
        objDropdownFocusedIn: {},
        stopScrollingKeys: {},

        AjaxSetup: function () {
            $.ajaxSetup({
                cache: false,
                contentType: 'application/json; charset=utf-8',
                dataType: 'html',
                beforeSend: function (jqXHR) {
                    private.AllAjaxRequests.push(jqXHR);
                },
                complete: function (jqXHR) {
                    var index = private.AllAjaxRequests.indexOf(jqXHR);
                    if (index > -1) {
                        private.AllAjaxRequests.splice(index, 1);
                    }
                }
            });

            $(document).ajaxStart(function () {
                $('#dm-ui-main-page-title').next('.dm-ui-alert').remove();
            });

            $(document).ajaxError(function (event, xhr, settings) {
                if (settings.dataType === 'html')
                    $('#dm-ui-main-page-title').after(xhr.responseText);
                else if (settings.dataType === 'json')
                    $('#dm-ui-main-page-title').after(private.CreateAlertHtml(xhr.responseJSON));
                else
                    alert(xhr.responseText);

                $('.dm-ui-overlay-spinner-container .accessible-message-placeholder').text('Error loading content.');
                $('.dm-ui-overlay-spinner-container').hide();
            });

            $(document).ajaxComplete(function (event, xhr, settings) {
                private.SessionTimeElapsed = 0;
            });
        },
        CreateAlertHtml: function (alertJson) {
            if (typeof alertJson !== "undefined") {
                var html = '<div class="' + alertJson.HtmlClass + '" role="alert">';

                if (alertJson.IsDismissable)
                    html += '<a href="#" class="dm-ui-alert-close" aria-label="close error alert">&times;</a>';

                return html += alertJson.Message + '</div>';
            }
        },
        InitWindowEvents: function () {
            $(window).on('click enterkeyup', function (e) {
                if (!e.target.matches('.dm-ui-dropdown-button, .dm-ui-dropdown-button > .placeholder, .dm-ui-dropdown-button > span, .dm-ui-dropdown-multiselect > .dm-ui-dropdown-content, .dm-ui-dropdown-multiselect > .dm-ui-dropdown-content *')) {
                    private.CollapseExpandedWidgets();
                    $('.dm-ui-multi-select-message').removeClass('dm-ui-error-text').removeClass('dm-ui-success-text').text('');
                }
            });

            $(window).on('escapekeyup', function () {
                private.CollapseExpandedWidgets();//WCAG on ESCAPE keyhas
            });
        },

        StopBrowserScrollingListener1: function (e) {
            private.stopScrollingKeys[e.keyCode] = true;
            switch (e.keyCode) {
                case 38: case 40: // Arrow keys
                case 32: e.preventDefault(); break; // Space
                default: break; // do not block other keys
            }
        },
        StopBrowserScrollingListener2: function (e) {
            private.stopScrollingKeys[e.keyCode] = false;
        },
        StopBrowserScrolling: function () {
            window.addEventListener("keydown", private.StopBrowserScrollingListener1, false);
            window.addEventListener("keyup", private.StopBrowserScrollingListener2, false);
        },
        StartBrowserScrolling: function () {
            window.removeEventListener("keydown", private.StopBrowserScrollingListener1, false);
            window.removeEventListener("keyup", private.StopBrowserScrollingListener2, false);
        },
        CheckIsModalPopupDropdown: function (element) {
            var modal = element.parents(".dm-ui-modal");
            if (modal.length) {
                if (element.hasClass("dm-ui-expanded")) {
                    modal.addClass("without-overflow");
                } else {
                    modal.removeClass("without-overflow");
                }
            }
        },

        InitDropdowns: function () {
            $(document).on("DmUi:updated", ".dm-ui-single-select", function (e) {
                $(this).DmUiRecreateDropdownContentSingleSelection();
            });

            $(document).on("DmUi:updated", ".dm-ui-multi-select", function (e) {
                $(this).DmUiRecreateDropdownContentMultiSelection();
            });

            $(document).on("DmUi:updated", ".dm-ui-group-multi-select", function (e) {
                $(this).DmUiRecreateDropdownContentTreeMultiSelection();
            });

            function collapseDropdown(dropdownWidget) {
                var element = dropdownWidget.find(".dm-ui-dropdown-button");
                if (element.hasClass("dm-ui-expanded")) {
                    private.StartBrowserScrolling();
                    element.removeClass("dm-ui-expanded");
                    var $dropdownContainer = element.next(".dm-ui-dropdown-content");
                    $dropdownContainer.hide();
                    element.attr("aria-expanded", "false");
                    private.ResetMultiSelect(element);
                    private.CheckIsModalPopupDropdown(element);
                }
            }
            function checkIsCollapseDropdown(element) {
                if (!element.is(private.objDropdownFocusedIn) || (private.isDropdownFocusOut && !private.isDropdownFocusIn)) {
                    collapseDropdown(element);
                }
            }
            function stopScrollIfNextDropdownWasFocusedAgainAfterDropdown() {
                private.StopBrowserScrolling();
            }
            $(document).on("focusin", ".dm-ui-dropdown-widget, .dm-ui-checkbox, .dm-ui-radio, .dm-ui-switcher label, .no-spacebar-scrolling", function () {
                private.isDropdownFocusIn = true;
                private.objDropdownFocusedIn = $(this);
                private.StopBrowserScrolling();
                setTimeout(stopScrollIfNextDropdownWasFocusedAgainAfterDropdown, 20);
            });
            $(document).on("focusout", ".dm-ui-dropdown-widget", function () {
                /*
                                private.isDropdownFocusOut = true;
                                private.isDropdownFocusIn = false;
                                setTimeout(checkIsCollapseDropdown, 10, private.objDropdownFocusedIn);
                */
                if (!$(this).find(".dm-ui-dropdown-button").hasClass("dm-ui-expanded")) {
                    private.StartBrowserScrolling();
                }
            });
            $(document).on("focusout", ".dm-ui-checkbox, .dm-ui-radio, .dm-ui-switcher label, .no-spacebar-scrolling", function () {
                private.StartBrowserScrolling();
            });
            $(document).on("keydown", ".dm-ui-dropdown-widget", function (e) {
                if (e.keyCode === 9) {
                    private.isDropdownFocusOut = true;
                    private.isDropdownFocusIn = false;
                    setTimeout(checkIsCollapseDropdown, 10, private.objDropdownFocusedIn);
                }
            });


            $(document).on('click keydown', '.dm-ui-dropdown-button', function (e) {
                var doClick = false;
                var isDownArrow = false;
                var isUpArrow = false;

                if (e.keyCode === undefined && e.originalEvent.clientX <= 0 && e.originalEvent.clientY <= 0) {
                    return;
                }
                if (e.keyCode === 32 || e.keyCode === 13) {
                    e.keyCode = undefined;
                }

                if (e.keyCode) {
                    if (e.keyCode == 38) {
                        e.preventDefault();
                        isUpArrow = true;
                        doClick = true;
                    }
                    else if (e.keyCode == 40) {
                        e.preventDefault();
                        isDownArrow = true;
                        doClick = true;
                    }
                }
                else {
                    doClick = true;
                }

                if (doClick) {
                    if ($(this).parent('.dm-ui-dropdown-widget').hasClass('dm-ui-disabled'))
                        return;

                    if ($(this).hasClass('dm-ui-expanded')) {
                        if (isDownArrow) {
                            //$(this).parent().find("*[tabindex = '0']:visible")[0].focus();
                            $(this).parent().find("*[tabindex]:visible").not("[tabindex = '-1']").not(".dm-ui-dropdown-button")[0].focus();
                        }
                        else {
                            private.CollapseExpandedWidgets();
                        }
                    }
                    else {
                        if (!isUpArrow) {
                            private.CollapseExpandedWidgets(this);
                            $(this).addClass('dm-ui-expanded');
                            $(this).focus();
                            $(this).attr('aria-expanded', 'true');
                            var $dropdownContainer = $(this).next('.dm-ui-dropdown-content');
                            $dropdownContainer.show();
                            $dropdownContainer.find('.dm-ui-hide').hide();
                            if ($(this).parent().hasClass("dm-ui-dropdown-multiselect")) {
                                if ($(this).parent().find("input[type=checkbox]:not(.checkbox_group):checked:first").length) {
                                    $(this).parent().find("input[type=checkbox]:not(.checkbox_group):checked:first").trigger("change");
                                } else {
                                    if ($(this).parent().prev().hasClass("dm-ui-multi-select")) {
                                        $(this).parent().find("input[type=checkbox]:first").trigger("change");
                                    }
                                }
                            }
                        }
                    }
                }
                private.CheckIsModalPopupDropdown($(this));
            });

            $(document).on('keydown', '.dm-ui-li-item', function (e) {
                if (e.keyCode == 38) {
                    if ($(this).prev().length > 0) {
                        $(this).prev().focus();
                    } else {
                        $(this).parent().parent().parent().children("button")[0].focus();
                    }
                }
                else if (e.keyCode == 40) {
                    if ($(this).next().length > 0) {
                        $(this).next().focus();
                    }
                }
            });

            $(document).on("keydown", ".dm-ui-menuitem-checkbox, .group.one-item-selectable .group-title-wrapper, .dm-ui-dropdown-items-group li.group > .arrow, .dm-ui-cancel-button, .dm-ui-apply-button, .dm-ui-select-all, .dm-ui-select-none, .dm-ui-select-all-group, .dm-ui-select-none-group", function (e) {
                if (e.keyCode === 38) {
                    private.FindDropdownPrevTabStop($(this)).focus();
                }
                if (e.keyCode === 40) {
                    private.FindDropdownNextTabStop($(this)).focus();
                }
            });

            $(document).on('click enterkeyup', '.dm-ui-dropdown-singleselect li', function () {
                var $singleselect = $(this).closest('.dm-ui-dropdown-singleselect');

                if ($(this).hasClass('dm-ui-disabled') || $(this).hasClass('dm-ui-selected') && !$singleselect.data('allow-diselection'))
                    return false;

                $(this).attr('aria-checked', true);

                $(this).siblings('li').removeClass('dm-ui-selected');
                $(this).siblings('li').attr('aria-checked', false);

                $(this).toggleClass('dm-ui-selected');

                var isElementSelected = $(this).hasClass('dm-ui-selected');

                var $hdnLabel = $singleselect.children('label').last();
                $hdnLabel.html($(this).text() + ' Selected');

                var selectType = $(this).parent().parent().parent().attr('data-type');

                var btnText = $(this).text();

                if (typeof selectType !== typeof undefined && selectType !== false && selectType.length > 0) {
                    btnText = selectType + ": " + btnText;
                }

                var $button = $singleselect.find('.dm-ui-dropdown-button');
                //$button.text(isElementSelected ? $(this).text() : $button.data('default-text'));
                $button.html(isElementSelected ? btnText : '<span class="placeholder">' + $button.data('default-text') + '</span>');
                //$button.attr('aria-describedby', $(this).attr('id'));

                var $select = $('#' + $singleselect.attr('id').replace('_dm_ui', ''));
                var $selectOptions = $select.children();
                var selectedOptionValue = $(this).data('value');
                $selectOptions.removeAttr("selected");
                if (isElementSelected) {
                    $selectOptions.each(function () {
                        if ($(this).attr("value") === selectedOptionValue) {
                            $(this).prop("selected", true);
                            $(this).attr("selected", "selected");
                            return;
                        }
                    });
                }
                $select.val(isElementSelected ? String($(this).data('value')) : '').trigger('change');
                $button.focus();

                return this;
            });

            $(document).on('click enterkeyup', '.dm-ui-dropdown-multiselect .dm-ui-cancel-button', function () {
                private.CollapseExpandedWidgets();
            });

            $(document).on("change", ".tree-list.dm-ui-dropdown-multiselect .dm-ui-menuitem-checkbox", function () {
                var hiddenApplyButton = $(this).parents(".dm-ui-dropdown-content").first().find(".dm-ui-apply-button");
                hiddenApplyButton.click();
            });

            $(document).on('click enterkeyup', '.dm-ui-dropdown-multiselect .dm-ui-apply-button', function () {
                var $multiselect = $(this).closest('.dm-ui-dropdown-multiselect');

                $multiselect.find('input[type=checkbox]').each(function () {
                    //$(this).data('initially-selected', $(this).is(':checked'));
                    $(this).attr('data-initially-selected', $(this).is(':checked'));
                });

                var values = $multiselect.find('input:checked').map(function (index, elem) {
                    if (!$(this).hasClass('checkbox_group')) {
                        return elem.value;
                    }
                }).get();

                var isToolTip = $multiselect.hasClass("tooltip");
                var tooltiptext = '';

                var hiddenValues = $multiselect.find('input:checked').closest('.dm-ui-hide').length;
                var valuesLength = values.length - hiddenValues;

                var $select = $('#' + $multiselect.attr('id').replace('_dm_ui', ''));
                $select.val(values).trigger('change');

                var $selectOptions = $select.children();
                $selectOptions.each(function () {
                    if ($(this).prop("selected")) {
                        if (isToolTip && !($(this).hasClass("dm-ui-hide"))) {
                            tooltiptext = tooltiptext + "<div class='tooltipitem'>" + $(this).text() + "</div>";
                        }
                        $(this).attr("selected", "selected");
                    } else {
                        $(this).removeAttr("selected");
                    }
                });

                if (isToolTip) {
                    var $tooltip = $multiselect.find('.tooltiptext');

                    if ($tooltip.length > 0) {
                        if (tooltiptext.length > 0) {
                            $tooltip.html(tooltiptext);
                        }
                        else {
                            $tooltip.remove();
                        }
                    }
                    else if (tooltiptext.length > 0) {
                        $multiselect.prepend($("<div class='tooltiptext'>" + tooltiptext + "</div>"));
                    }
                }

                $multiselect.find('.dm-ui-dropdown-content').hide();

                var selectType = $multiselect.attr('data-type');
                var btnText = valuesLength;

                if (typeof selectType !== typeof undefined && selectType !== false && selectType.length > 0) {
                    btnText = btnText + ' ' + selectType;

                    if (valuesLength > 1) {
                        btnText = btnText + (selectType[selectType.length - 1].toUpperCase() == 'S' ? 'es' : 's');
                    }
                }

                var $button = $multiselect.find('.dm-ui-dropdown-button');
                $button.removeClass('dm-ui-expanded');
                $button.attr('aria-expanded', 'false');
                private.StartBrowserScrolling();
                //$button.text(values.length === 0 ? $button.data('default-text') : values.length + ' selected');
                $button.html(valuesLength === 0 ? '<span class="placeholder">' + $button.data('default-text') + '</span>' : btnText + ' selected');
                $button.focus();

                var $indicator = $multiselect.find('.selected-num-indicator');
                $indicator.css('display', valuesLength === 0 ? 'none' : 'inline-block');
                $indicator.text(valuesLength === 0 ? '' : valuesLength);

                var hdnLabel = $multiselect.children('label').last();
                hdnLabel.html($button.text());
                private.CheckIsModalPopupDropdown($(this));
            });

            $(document).on('click enterkeyup', '.dm-ui-dropdown-multiselect .dm-ui-select-all', function () {
                var $multiselect = $(this).closest('.dm-ui-dropdown-multiselect');

                $multiselect.find('li:not(.dm-ui-hide) input[type=checkbox]').not(':disabled').each(function () {
                    $(this).prop('checked', true);
                });

                $multiselect.find('input[type=checkbox]:first').trigger('change');
            });

            $(document).on("click enterkeyup", ".dm-ui-dropdown-multiselect .dm-ui-select-all-group", function () {
                var $multiselect = $(this).closest(".dm-ui-dropdown-multiselect");
                $multiselect.find(".checkbox_group").removeClass("selected_not_all");

                $multiselect.find("li:not(.dm-ui-hide) input[type=checkbox]").not(":disabled").each(function () {
                    $(this).prop("checked", true);
                });

                $multiselect.find('.dm-ui-dropdown-items-group > li.group > .dm-ui-dropdown-items > li input[type=checkbox]').not('.checkbox_group').trigger('change');
            });

            $(document).on('click enterkeyup', '.dm-ui-dropdown-multiselect .dm-ui-select-none', function () {
                var $multiselect = $(this).closest('.dm-ui-dropdown-multiselect');

                $multiselect.find('li:not(.dm-ui-hide) input[type=checkbox]').not(':disabled').each(function () {
                    $(this).prop('checked', false);
                });

                $multiselect.find('input[type=checkbox]:first').trigger('change');
            });

            $(document).on("click enterkeyup", ".dm-ui-dropdown-multiselect .dm-ui-select-none-group", function () {
                var $multiselect = $(this).closest(".dm-ui-dropdown-multiselect");

                $multiselect.find("li:not(.dm-ui-hide) input[type=checkbox]").not(":disabled").each(function () {
                    $(this).prop("checked", false);
                });

                $multiselect.find('.dm-ui-dropdown-items-group > li.group > .dm-ui-dropdown-items > li input[type=checkbox]').not('.checkbox_group').trigger('change');
            });

            $(document).on('change', '.dm-ui-dropdown-multiselect input[type=checkbox]', function () {
                var $multiselect = $(this).closest('.dm-ui-dropdown-multiselect');

                $(this).parent().attr('aria-checked', $(this).prop('checked'));

                $multiselect.find('.dm-ui-multi-select-message').removeClass('dm-ui-error-text').removeClass('dm-ui-success-text').text('');
                $multiselect.find('.dm-ui-apply-button').prop('disabled', false);

                var numSelected = $multiselect.find('li:not(.dm-ui-hide) input:checked').length;
                var numSelectedGroup = $multiselect.find('li:not(.dm-ui-hide) input.checkbox_group:checked').length;
                var numCheckboxes = $multiselect.find('li:not(.dm-ui-hide) input').length;
                numSelected = numSelected - numSelectedGroup;
                numCheckboxes = numCheckboxes - numSelectedGroup;

                var minToSelect = $multiselect.data('min-to-select') ? $multiselect.data('min-to-select') : 0;
                var maxToSelect = $multiselect.data('max-to-select') ? $multiselect.data('max-to-select') : $multiselect.find('.dm-ui-checkbox').length;
                maxToSelect = maxToSelect - numSelectedGroup;

                if (numSelected < minToSelect) {
                    $multiselect.find('.dm-ui-apply-button').prop('disabled', true);
                    $multiselect.find('.dm-ui-multi-select-message').addClass('dm-ui-error-text').text('At least ' + minToSelect + ' option(s) need to be selected to continue.');
                }
                else if (numSelected > maxToSelect) {
                    $multiselect.find('.dm-ui-apply-button').prop('disabled', true);
                    $multiselect.find('.dm-ui-multi-select-message').addClass('dm-ui-error-text').text('Max number of ' + maxToSelect + ' selections has been exceeded.');
                }
                else if (numSelected === minToSelect && minToSelect > 0) {
                    $multiselect.find('.dm-ui-apply-button').prop('disabled', false);
                    //if (minToSelect < numCheckboxes) {
                    //    $multiselect.find('.dm-ui-multi-select-message').addClass('dm-ui-success-text').text('Minimum number of ' + minToSelect + ' selection(s) is currently selected.');
                    //}
                }
                else if (numSelected === maxToSelect) {
                    $multiselect.find('.dm-ui-apply-button').prop('disabled', false);
                    if (maxToSelect < numCheckboxes) {
                        $multiselect.find('.dm-ui-multi-select-message').addClass('dm-ui-success-text').text('Max number of ' + maxToSelect + ' selection(s) has been reached.');
                    }
                }
                //else {
                //    $multiselect.find('.dm-ui-multi-select-message').removeClass('dm-ui-error-text').removeClass('dm-ui-success-text').text('');
                //    $multiselect.find('.dm-ui-apply-button').prop('disabled', false);
                //}
            });

            $(".tree.dm-ui-dropdown-widget .dm-ui-dropdown-button").each(function () {
                private.ResetMultiSelect($(this));
            });
        },
        CollapseExpandedWidgets: function ($clickedButtonInSelectWidget) {
            // close/hide all dm-ui widgets except for when click is on an element in 'excluded elements'
            $('.dm-ui-expanded').not($clickedButtonInSelectWidget).each(function () {
                private.StartBrowserScrolling();
                if ($(this).hasClass("dm-ui-expanded")) {
                    $(this).focus();
                }
                $(this).removeClass('dm-ui-expanded');
                var $dropdownContainer = $(this).next('.dm-ui-dropdown-content');
                $dropdownContainer.hide();
                $(this).attr('aria-expanded', 'false');
                private.ResetMultiSelect(this);
                private.CheckIsModalPopupDropdown($(this));
            });
        },
        CheckHowManyCheckboxesCheckedInGroup: function (innerCheckboxOfGroup) {
            var innerCheckboxesWrapperElement = innerCheckboxOfGroup.parent().parent().parent();
            var totalNum = innerCheckboxesWrapperElement.find(".dm-ui-checkbox").length;
            var counter = 0;
            innerCheckboxesWrapperElement.find(".dm-ui-checkbox input[type=checkbox]").each(function () {
                if ($(this).prop("checked")) {
                    counter++;
                }
            });
            var categoryCheckbox = innerCheckboxesWrapperElement.parent(".group").find("input[type=checkbox].checkbox_group").first();
            categoryCheckbox.removeClass("selected_not_all");
            var counterTreeInside = 0;
            var totalTreeInsideNum = categoryCheckbox.parent().parent().find(".dm-ui-dropdown-items-group input[type=checkbox].checkbox_group").length;
            if (totalTreeInsideNum) {
                categoryCheckbox.parent().parent().find(".dm-ui-dropdown-items-group input[type=checkbox].checkbox_group").each(function () {
                    if ($(this).prop("checked") && !$(this).hasClass("selected_not_all")) {
                        counterTreeInside++;
                    }
                });
            }
            if (counter === totalNum && counterTreeInside === totalTreeInsideNum) {
                categoryCheckbox.prop("checked", true);
            } else if (counter === 0 && counterTreeInside === 0) {
                categoryCheckbox.prop("checked", false);
            } else {
                categoryCheckbox.prop("checked", true);
                categoryCheckbox.addClass("selected_not_all");
            }
            private.CheckTreeBranchAllCheckboxes(categoryCheckbox);
        },
        CheckTreeBranchAllCheckboxes: function (categoryCheckbox) {
            //count group checkboxes & current level checkboxes state & call upper level if exist
            var groupWrapper = categoryCheckbox.parent().parent(".group");
            if (groupWrapper.length) {

                var upperLevelGroup = groupWrapper.parents(".group").first();
                if (upperLevelGroup.length) {

                    var firstUpperLevelGroupCheckbox = upperLevelGroup.find("> .dm-ui-checkbox .checkbox_group");
                    if (firstUpperLevelGroupCheckbox.length) {
                        var numTotalSingleCheckboxesOnThisLevel = upperLevelGroup.find("> .dm-ui-dropdown-items .dm-ui-checkbox").length;
                        var numCheckedSingleCheckboxesOnThisLevel = upperLevelGroup.find("> .dm-ui-dropdown-items .dm-ui-checkbox input[type=checkbox]:checked").length;
                        var numTotalGroupCheckboxesOnThisLevel = upperLevelGroup.find("> .dm-ui-dropdown-items-group > .group .checkbox_group").length;
                        var numCheckedGroupCheckboxesOnThisLevel = upperLevelGroup.find("> .dm-ui-dropdown-items-group > .group .checkbox_group:checked:not(.selected_not_all)").length;
                        var numCheckedNotAllGroupCheckboxesOnThisLevel = upperLevelGroup.find("> .dm-ui-dropdown-items-group > .group .checkbox_group.selected_not_all").length;
                        firstUpperLevelGroupCheckbox.removeClass("selected_not_all");
                        if (numCheckedSingleCheckboxesOnThisLevel === numTotalSingleCheckboxesOnThisLevel && numCheckedGroupCheckboxesOnThisLevel === numTotalGroupCheckboxesOnThisLevel) {
                            firstUpperLevelGroupCheckbox.prop("checked", true);
                        } else if (numCheckedSingleCheckboxesOnThisLevel === 0 && numCheckedGroupCheckboxesOnThisLevel === 0 && numCheckedNotAllGroupCheckboxesOnThisLevel === 0) {
                            firstUpperLevelGroupCheckbox.prop("checked", false);
                        } else {
                            firstUpperLevelGroupCheckbox.prop("checked", true);
                            firstUpperLevelGroupCheckbox.addClass("selected_not_all");
                        }
                        private.CheckTreeBranchAllCheckboxes(firstUpperLevelGroupCheckbox);
                    }
                }
            }
        },
        FindDropdownNextTabStop: function (el) {
            var tabbable = el.parents(".dm-ui-dropdown-widget").find("button, a, [tabindex]:visible");
            var list = Array.prototype.filter.call(tabbable, function (item) { return item.tabIndex >= "0" });
            var index = list.indexOf(el[0]);
            return list[index + 1] || list[index];
        },
        FindDropdownPrevTabStop: function (el) {
            var tabbable = el.parents(".dm-ui-dropdown-widget").find("button, a, [tabindex]:visible");
            var list = Array.prototype.filter.call(tabbable, function (item) { return item.tabIndex >= "0" });
            var index = list.indexOf(el[0]);
            return list[index - 1] || list[0];
        },
        ResetMultiSelect: function ($widgetButton) {
            var $widget = $($widgetButton).closest(".dm-ui-dropdown-widget");
            if ($widget.hasClass("dm-ui-dropdown-multiselect")) {
                $widget.find("input[type=checkbox]").each(function () {
                    //$(this).prop("checked", $(this).data("initially-selected"));
                    if ($(this).attr("data-initially-selected") === "true") {
                        $(this).prop("checked", true);
                        $(this).parent(".dm-ui-checkbox").attr("aria-checked", "true");
                    } else {
                        $(this).prop("checked", false);
                        $(this).parent(".dm-ui-checkbox").attr("aria-checked", "false");
                    }
                });
                $widget.find(".group.one-item-selectable").each(function () {
                    if ($(this).find('input').prop("checked")) {
                        $(this).addClass("selected");
                    } else {
                        $(this).removeClass("selected");
                    }
                });
                if ($widget.find(".dm-ui-dropdown-items-group").length && !$widget.find(".dm-ui-dropdown-items-group .group.one-item-selectable").length) {
                    $widget.find(".group .dm-ui-dropdown-items li:first-child .dm-ui-checkbox input[type=checkbox]").each(function () {
                        private.CheckHowManyCheckboxesCheckedInGroup($(this));
                    });    
                }
            }
        },
        InitFooter: function () {
            //TODO:  THIS IS A BIG FAT PROBLEM.  focusout event fires when going from 1 item in footer to another, thus breaking everything
            //$('.main-footer-container').on('focusout', function() {
            //    private.ResetFooter();
            //});

            //expand/collapse
            $('.footer-sitemap-switch').on('click', function () {
                if ($('.main-footer-container').hasClass('dm-ui-footer-expanded')) {
                    $('.main-footer-container').removeClass('dm-ui-footer-expanded');
                    $('.main-footer-sitemap').addClass('dm-ui-hide');
                    $('#divFooterContent').attr('aria-expanded', false);
                }
                else {
                    $('.main-footer-container').addClass('dm-ui-footer-expanded');
                    $('.main-footer-sitemap').removeClass('dm-ui-hide');
                    $('#divFooterContent').attr('aria-expanded', true);
                }
                $('html, body').animate({ scrollTop: $(document).height() }, 'fast');
            });

            $('.main-footer').on('enterkeyup', function () {
                $('.footer-sitemap-switch').length === 1 ? $('.footer-sitemap-switch').focus() : $('.footer-link:first').focus();
                $('.footer-link').attr('tabindex', '0');
                $(this).attr('tabindex', '-1');
            });

            $('.footer-sitemap-block').on('enterkeyup', function () {
                $(this).find('.footer-sitemap-link:first').focus();
                $('.footer-sitemap-link').attr('tabindex', '0');
                $('.footer-sitemap-block').attr('tabindex', '-1');
            });

            $('.footer-link, .footer-sitemap-block, .footer-sitemap-link').on('escapekeyup', function () {
                $('.main-footer').focus();
                private.ResetFooter();
            });

            $('.footer-sitemap-link').on('keyup', function (e) {
                if (e.keyCode === 38) {
                    var $prevItem = $(this).closest('li').prev('li');
                    if ($prevItem.length > 0) {
                        $prevItem.children('a').focus();
                    }
                }
                else if (e.keyCode === 40) {
                    var $nextItem = $(this).closest('li').next('li');
                    if ($nextItem.length > 0) {
                        $nextItem.children('a').focus();
                    }
                }
            });
        },
        ResetFooter: function () {
            $('.main-footer').attr('tabindex', '0');
            $('.footer-link, .footer-sitemap-link').attr('tabindex', '-1');
            $('.footer-sitemap-block').attr('tabindex', '0');
        },
        InitMiscellaneousHandlers: function () {
            $('.dm-ui-tab-nav .dm-ui-selected a').on('click', function (e) {
                e.preventDefault();
            });

            $('.dm-ui-tab-li').on('keyup click', function (e) {
                var isTabClick = false;

                if (e.keyCode) {
                    if (e.keyCode == 13 || e.keyCode == 32) {
                        isTabClick = true;
                    }
                }
                else {
                    isTabClick = true;
                }

                if (isTabClick) {
                    $('.dm-ui-tab-li').attr('aria-selected', 'false');
                    $(this).attr('aria-selected', 'true');
                    var liLocation = $(this).attr('data-link');
                    window.location.href = liLocation;
                }
            });

            //Alert dismiss button
            $(document).on('click', '.dm-ui-alert-close', function () {
                $(this).parent('.dm-ui-alert').remove();
            });

            $(document).on("keyup", ".dm-ui-checkbox, .dm-ui-radio", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $(this).find("input").trigger("click");
                    if (e.keyCode === 32) {
                        $("body").addClass("wcag_focuses_on");
                    }
                }
            });

            $(document).on('enterkeyup click', '.dm-ui-switcher > label', function (e) {
                $(this).prev('input').trigger('click');
            });

            $(document).on("click", ".dm-ui-checkbox", function () {
                $(this).attr('aria-checked', $(this).find('input[type=checkbox]').prop('checked'));
            });

            $(document).on("change", ".group .dm-ui-dropdown-items .dm-ui-checkbox input[type=checkbox]", function () {
                private.CheckHowManyCheckboxesCheckedInGroup($(this));
            });

            $(document).on("change", ".group.one-item-selectable .dm-ui-dropdown-items .dm-ui-checkbox", function () {
                var id = $(this).find('input').attr("id");
                $(this).parent().parent().find('input').each(function () {
                    if ($(this).attr("id") !== id && $(this).prop("checked")) {
                        $(this).prop("checked", false);
                        $(this).parent().attr("aria-checked", "false");
                    }
                });
                if ($(this).find('input').prop("checked")) {
                    $(this).parent().parent().parent().addClass("selected");
                } else {
                    $(this).parent().parent().parent().removeClass("selected");
                }
            });

            $(document).on("change", ".dm-ui-checkbox .checkbox_group", function () {
                var checkboxGroupState = "" + !$(this).prop("checked");
                $(this).parents(".group").first().find(".dm-ui-dropdown-items .dm-ui-checkbox").each(function () {
                    if ($(this).attr("aria-checked") === checkboxGroupState) {
                        //$(this).find("label").click();
                        $(this).find("> input").click();
                    }
                });
            });

            $(document).on('click enterkeyup', '.dm-ui-dropdown-items-group .group .arrow', function () {
                $(this).toggleClass('opened');
                if ($(this).hasClass('opened')) {
                    $(this).parent('.group').find('> .dm-ui-dropdown-items').show();
                    $(this).attr("aria-expanded", "true");
                } else {
                    $(this).parent('.group').find('> .dm-ui-dropdown-items').hide();
                    $(this).attr("aria-expanded", "false");
                }
            });

            $(document).on("click enterkeyup", ".dm-ui-dropdown-items-group .group .group-title-wrapper", function () {
                var element = $(this).children(".arrow");
                element.toggleClass("opened");
                if (element.hasClass("opened")) {
                    element.parents(".group").find(".dm-ui-dropdown-items").show();
                    $(this).attr("aria-expanded", "true");
                } else {
                    element.parents(".group").find(".dm-ui-dropdown-items").hide();
                    $(this).attr("aria-expanded", "false");
                }
            });

            $('.dm-ui-radio > input[type="radio"]').bind('click', function () {
                $('input[name="' + $(this).attr('name') + '"]').not($(this)).trigger('deselect');
                $(this).parent().attr('aria-checked', 'true');
            });

            $('.dm-ui-radio > input[type="radio"]').bind('deselect', function () {
                $(this).parent().attr('aria-checked', 'false');
            });
        },
        InitModalHandlers: function () {
            $('.dm-ui-modal-first').on('keydown', function (e) {
                if (e.keyCode == 9 && e.shiftKey) {
                    $('.dm-ui-modal-last').first().focus();
                    return false;
                }
            });

            $('.dm-ui-modal-last').on('keydown', function (e) {
                if (e.keyCode == 9 && !e.shiftKey) {
                    $('.dm-ui-modal-first').first().focus();
                    return false;
                }
            });
        },
        InitSessionTimeOut: function () {
            if (!private.UiSettings.TerminateSessionTimeOut && private.UiSettings.SessionTimeOut > 0) {
                $('#session-sign-out-button').on('click', function () {
                    window.location = private.UiSettings.LoginUrl;
                });

                $('#session-continue-button').on('click', function () {
                    private.ResetSessionTimeOut();
                    location.reload();
                });

                $('#session-continue-button').on("keydown",
                    function (e) {
                        if (e.keyCode === 9 && !e.shiftKey) {
                            e.preventDefault();
                            $('#session-sign-out-button').focus();
                        }
                    });

                $('#session-sign-out-button').on("keydown",
                    function (e) {
                        if (e.keyCode === 9 && e.shiftKey) {
                            e.preventDefault();
                            $('#session-continue-button').focus();
                        }
                    });

                setInterval(function () { private.CheckSessionTimeOut(); }, private.SessionCheckInterval * 1000);
            }
        },
        ResetSessionTimeOut: function () {
            //var self = this;

            $.ajax({
                type: 'POST',
                url: private.UiSettings.SiteRoot + private.UiSettings.SessionKeepAliveUrl,
                success: function () {
                    private.SessionTimeElapsed = 0;
                    $('#dm-ui-session-time-out-modal').hide();
                },
                error: function (e) {
                    //self.HideOverlaySpinner();
                    e.preventDefault();
                }
            });
        },
        CheckSessionTimeOut: function () {
            private.SessionTimeElapsed = private.SessionTimeElapsed + 5; //seconds
            var timeRemaining = private.UiSettings.SessionTimeOut - private.SessionTimeElapsed;

            if (timeRemaining <= private.UiSettings.AlertTimeToSessionEnd) {

                var minutesValue = parseInt(timeRemaining / 60);
                var secondsValue = timeRemaining % 60;

                if (minutesValue === 0 && secondsValue === 0)
                    window.location = private.UiSettings.LoginUrl;

                var minutes = minutesValue === 0 ? '' :
                    minutesValue === 1 ? minutesValue + ' minute' :
                        minutesValue + ' minutes';
                var seconds = secondsValue === 0 ? '' : secondsValue + ' seconds';

                $('.session-modal-count-down').html(minutes + ' ' + seconds);

                var isModalVisible = $('#dm-ui-session-time-out-modal').is(':visible');

                $('#dm-ui-session-time-out-modal').show();

                if (!isModalVisible) {
                    $('#session-continue-button').focus();
                }
            }
        },
        InitSearchFieldEvents: function () {
            $(document).on("change", ".dm-ui-search-field input", function (e) {
                if ($(this).val() == "") {
                    $(this).removeClass("serach-field-filled");
                } else {
                    $(this).addClass("serach-field-filled");
                }
            });
        },
        SwitchFocusStatesForWCAG: function () {
            $(document).keyup(function (e) {
                if (e.which === 9 || e.which === 38 || e.which === 40 || e.which === 13) {
                    $("body").addClass("wcag_focuses_on");
                }
            });
            $(document).click(function (e) {
                if (e.originalEvent !== undefined) {
                    if (e.originalEvent.screenX !== 0 && e.originalEvent.screenY !== 0) {
                        if (e.originalEvent.mozInputSource === undefined) {
                            $("body").removeClass("wcag_focuses_on");
                        } else {
                            if (e.originalEvent.mozInputSource === 1) {
                                $("body").removeClass("wcag_focuses_on");
                            }
                        }
                    }
                }
            });
        }
    };

    return {
        GetUiSettings: function () { return private.UiSettings; },

        Init: function (uiSettings) {
            private.UiSettings = uiSettings;

            private.AjaxSetup();
            private.InitWindowEvents();
            private.InitDropdowns();
            private.InitFooter();
            private.InitMiscellaneousHandlers();
            private.InitSessionTimeOut();
            private.SwitchFocusStatesForWCAG();
            private.InitSearchFieldEvents();
        },

        InitMiscHandlers: function () {
            private.InitMiscellaneousHandlers();
        },

        InitFooter: function () {
            private.InitFooter();
        },

        ShowOverlaySpinner: function () {
            $('.dm-ui-overlay-spinner-container').show();
            $('.dm-ui-overlay-spinner-container .accessible-message-placeholder').text('Content is loading');
        },

        HideOverlaySpinner: function () {
            $('.dm-ui-overlay-spinner-container .accessible-message-placeholder').text('Content has loaded.');
            $('.dm-ui-overlay-spinner-container').hide();
        },

        DisplayAlert: function (data, dataType) {
            if (dataType === 'html') {
                $('#dm-ui-main-page-title').after(data);
                $('#dm-ui-main-page-title').next('.dm-ui-alert-success').attr('role', 'alert');
                $('#dm-ui-main-page-title').next('.dm-ui-alert-error').attr('role', 'alert');
            }
            else if (dataType === 'json')
                $('#dm-ui-main-page-title').after(private.CreateAlertHtml(data));
            else
                alert(data);
        },

        DisplayResponseAlert: function (data, dataType) {
            if (dataType === 'html' && data.includes('dm-ui-alert') || dataType === 'json' && data.AlertType) {
                this.DisplayAlert(data, dataType);
                this.HideOverlaySpinner();
                return true;
            }

            return false;
        },

        AbortAllAjaxRequests: function () {
            var cloneAllAjaxRequests = private.AllAjaxRequests.slice();
            cloneAllAjaxRequests.forEach(function (request) {
                request.abort();
            });
            private.AllAjaxRequests.length = 0;
        },

        NoAjaxResetSessionTimeOut: function () {
            private.SessionTimeElapsed = 0;
        },

        RandomStringGenerator: function (num) {
            var result = "";
            var words = 'abcdefghijklmnopqrstuvwxyz0123456789';
            var maxPosition = words.length - 1;
            var position;
            for (var i = 0; i < num; i++) {
                position = Math.floor(Math.random() * maxPosition);
                result = result + words.substring(position, position + 1);
            }
            return result;
        },

        StopBrowserScrolling: function () {
            private.StopBrowserScrolling();
        },

        StartBrowserScrolling: function () {
            private.StartBrowserScrolling();
        }, 

        ResetMultiSelect: function (el) {
            private.ResetMultiSelect(el);
        }
    };
}();