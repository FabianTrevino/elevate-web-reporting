var Options = function () {

    var siteRoot = '';
    var focusId = 0;

    return {
        Init: function () {
            var uiSettings = DmUiLibrary.GetUiSettings();
            siteRoot = uiSettings.SiteRoot;

            this.DisplayLocationChangeModal();
            this.InitButtonsHandlers();
            this.OptionsChangeHandler();
            this.AssignSelectGroupsHandlers();

            this.GetOptions(true);
            this.ValidateReportName();
        },

        AssignSelectGroupsHandlers: function () {
            $(document).on('change', '.option-group > select', function () {

                var result = $.map($(this).find('option:selected'), function (elem) {
                    return elem.value;
                });

                $(this).siblings('input.dm-ui-hidden-input').val(JSON.stringify(result)).trigger('change');
            });
        },

        AssignCheckboxGroupHandler: function (checkboxesSelector, hiddenInputSelector) {
            $(checkboxesSelector).on('change', function () {
                var selections = [];

                $(checkboxesSelector).each(function () {
                    selections.push($(this).prop('checked'));
                });

                $(hiddenInputSelector).val(JSON.stringify(selections)).trigger('change');
            });
        },

        AssignPiledOptionsGroupHandler: function (applyButtonSelector, cancelButtonSelector, modalSelector, hiddenInputSelector) {

            $(cancelButtonSelector).on('click', function () {
                $(modalSelector).addClass('dm-ui-hide');

                $(modalSelector).find('.dm-ui-dropdown-singleselect').each(function () {
                    $(this).DmUiApplySelection($(this).data('original-value'));
                });
            });

            $(applyButtonSelector).on('click', function () {
                var selections = [];

                $(modalSelector).find('select').each(function () {
                    if ($(this).val() !== '')
                        selections.push($(this).val());
                });

                $(modalSelector).addClass('dm-ui-hide');

                $(hiddenInputSelector).val(JSON.stringify(selections)).trigger('change');
            });

        },
        ValidateReportName: function () {
            $(document).on("keyup", "#report-name", function (e) {
                if ($(this).val().indexOf("/") != -1 || $(this).val().indexOf("*") != -1
                    || $(this).val().indexOf("<") != -1 || $(this).val().indexOf(">") != -1 ||
                    $(this).val().indexOf("|") != -1 || $(this).val().indexOf("?") != -1
                    || $(this).val().indexOf("\\") != -1 || $(this).val().indexOf("\"") != -1 ||
                    $(this).val().indexOf("+") != -1 || $(this).val().indexOf(":") != -1
                    || $(this).val().indexOf("#") != -1 || $(this).val().indexOf("%") != -1
                    || $(this).val().indexOf(",") != -1) {
                    $(this).addClass('invalid dm-ui-error');
                    $("#ok-run-in-background-button").prop("disabled", true);
                    $(".dm-ui-error-text").show();
                } else {
                    $(this).removeClass('invalid dm-ui-error');
                    $("#ok-run-in-background-button").prop("disabled", false);
                }
            });
        },
        AssignLongitudinalTestAdminsGroupHandler: function (applyButtonSelector, cancelButtonSelector, modalSelector, hiddenInputSelector) {
            var $allCheckboxes = $(modalSelector).find('input[type=checkbox]');
            var $checkboxes = $allCheckboxes.not(':first');

            $checkboxes.on('change', function () {
                var $dropdown = $('#' + $(this).data('grade-level-id'));

                var numSelected = $checkboxes.filter(':checked').length;
                if (numSelected < 4) {
                    $checkboxes.each(function () {
                        $(this).prop('disabled', false);
                        $(this).parent('.dm-ui-checkbox').removeClass('dm-ui-disabled');
                    });
                }
                else {
                    $checkboxes.not(':checked').each(function () {
                        $(this).prop('disabled', true);
                        $(this).parent('.dm-ui-checkbox').addClass('dm-ui-disabled');
                    });
                }

                if ($(this).is(':checked')) {
                    $dropdown.removeClass('dm-ui-disabled');

                    //1. Determine if Longitudinal Types is set to 'Consecutive' OR 'Same'
                    var longitudinalTypesValue = $('#LongitudinalTypes_Control').val();

                    //2. Grab 'Grade Num' from closest enabled dropdown above
                    var index = parseFloat($(this).data('index'));
                    var $prevCheckbox = $allCheckboxes.filter(function () {
                        var thisIndex = parseFloat($(this).data('index'));
                        return thisIndex < index && $(this).is(':checked');
                    }).last();
                    var prevGradeNum = parseFloat($('#' + $prevCheckbox.data('grade-level-id')).find('.dm-ui-selected').data('alt-value'));

                    //3. Get selection candidate
                    var $elemToSelect = $dropdown.find('li').filter(function () {
                        var thisGradeNum = parseFloat($(this).data('alt-value'));
                        //If selected Longitudinal Type is Consecutive (CB) - select first smaller grade num
                        if (longitudinalTypesValue === 'CB' && prevGradeNum > thisGradeNum)
                            return true;
                        //If selected Longitudinal Type is Same (SB) - select first same OR smaller grade num
                        else if (longitudinalTypesValue === 'SB' && prevGradeNum >= thisGradeNum)
                            return true;

                        return false;
                    }).first();

                    //4.  If we did not find the right candidate for selection, just select the first one
                    if ($elemToSelect.length === 1)
                        $dropdown.DmUiApplySelection($elemToSelect.data('value'));
                    else
                        $dropdown.DmUiApplySelection($dropdown.find('li:first').data('value'));
                }
                else {
                    $dropdown.DmUiApplySelection('');
                    $dropdown.addClass('dm-ui-disabled');
                }
            });

            $(cancelButtonSelector).on('click', function () {
                $(modalSelector).addClass('dm-ui-hide');

                $checkboxes.each(function () {
                    $(this).prop('checked', $(this).data('original-selection'));
                    var isOriginallyDisabled = $(this).data('original-disabled');
                    $(this).prop('disabled', isOriginallyDisabled);
                    if (isOriginallyDisabled)
                        $(this).parent('.dm-ui-checkbox').addClass('dm-ui-disabled');
                    else
                        $(this).parent('.dm-ui-checkbox').removeClass('dm-ui-disabled');
                });

                $(modalSelector).find('.dm-ui-dropdown-singleselect').each(function () {
                    $(this).DmUiApplySelection($(this).data('original-value'));
                    if ($(this).data('original-disabled'))
                        $(this).addClass('dm-ui-disabled');
                    else
                        $(this).removeClass('dm-ui-disabled');
                });
            });

            $('#LongitudinalTestAdministrationsModal .dm-ui-modal section .left-column .dm-ui-checkbox-container .dm-ui-checkbox').on('keydown', function (e) {
                if (e.keyCode == 9 && e.shiftKey) {
                    var firstItem = $('#LongitudinalTestAdministrationsModal .dm-ui-modal section .left-column .dm-ui-checkbox-container .dm-ui-checkbox:tabbable').first();

                    if ($(this).is(firstItem)) {
                        $(applyButtonSelector).focus();
                        return false;
                    }
                }
            });

            $(applyButtonSelector).on('keydown', function (e) {
                if (e.keyCode == 9 && !e.shiftKey) {
                    $('#LongitudinalTestAdministrationsModal .dm-ui-modal section .left-column .dm-ui-checkbox-container .dm-ui-checkbox:tabbable').first().focus();
                    return false;
                }
            });

            $(applyButtonSelector).on('click', function () {
                var selections = [];

                $(modalSelector).find('input[type=checkbox]:checked').each(function () {
                    var gradeLevelValue = $('#' + $(this).data('grade-level-id')).prev('select').val();
                    selections.push($(this).attr('id') + ':' + gradeLevelValue);
                });

                $(modalSelector).addClass('dm-ui-hide');

                $(hiddenInputSelector).val(JSON.stringify(selections)).trigger('change');
            });
        },

        AssignScoreFiltersGroupHandler: function (applyButtonSelector, cancelButtonSelector, modalSelector, hiddenInputSelector) {

            $(modalSelector).find('select[id^="score-dd"]').on('change', function () {
                var $contentDropdowns = $(this).closest('.score-filters-row').find('.col-content-area > .dm-ui-dropdown-widget');

                if ($contentDropdowns.length > 1) {
                    $contentDropdowns.hide();
                    if ($(this).val() === 'ISST')
                        $contentDropdowns.filter('[data-key=ISST]').show();
                    else
                        $contentDropdowns.filter('[data-key=regular]').show();

                }
            });

            var toggleVal2Visibility = function ($comparisonSelect) {
                var $colVal2 = $comparisonSelect.closest('.score-filters-row').find('.col-val2');
                $comparisonSelect.val() === 'BETWEEN' ? $colVal2.show() : $colVal2.hide();
            };

            $(modalSelector).find('select[id^="comparison-dd"]').on('change', function () {
                toggleVal2Visibility($(this));
            });

            $(cancelButtonSelector).on('click', function () {
                $(modalSelector).addClass('dm-ui-hide');

                $(modalSelector).find('.dm-ui-dropdown-widget').each(function () {
                    $(this).DmUiApplySelection($(this).data('original-value'));
                    var hiddenSelectId = $(this).attr("id").replace("_dm_ui", "");

                    if ($(this).hasClass('content-area-dropdown'))
                        //$(this).data('originally-visible') === true ? $(this).show() : $(this).hide();
                        $("#" + hiddenSelectId).data('originally-visible') === true ? $(this).show() : $(this).hide();


                    if ($(this).hasClass('comparison-dropdown'))
                        toggleVal2Visibility($(this).prev('select'));
                });

                $(modalSelector).find('.dm-ui-switcher').each(function () {
                    var originalValue = $(this).data('original-value');
                    $(this).find('input[type=radio]').prop('checked', false);
                    $(this).find('input[value="' + originalValue + '"]').prop('checked', true);
                });

                $(modalSelector).find('input[type=text]').each(function () {
                    $(this).val($(this).data('original-value'));
                });
            });

            $(applyButtonSelector).on('keydown', function (e) {
                if (e.keyCode == 9 && !e.shiftKey) {
                    $('.score-filters-row .score-filters-col .dm-ui-dropdown-singleselect .dm-ui-dropdown-button').first().focus();
                    return false;
                }
            });

            $('.score-filters-row .score-filters-col .dm-ui-dropdown-singleselect .dm-ui-dropdown-button').on('keydown', function (e) {
                if (e.keyCode == 9 && e.shiftKey) {
                    var firstButton = $('.score-filters-row .score-filters-col .dm-ui-dropdown-singleselect .dm-ui-dropdown-button').first();

                    if ($(this).is(firstButton)) {
                        $(applyButtonSelector).focus();
                        return false;
                    }
                }
            });

            $(applyButtonSelector).on('click', function () {
                var selections = [];

                $(modalSelector).find('.score-filters-row').not('.head-row').each(function () {
                    var row = '';
                    var concat = $(this).find('.col-concat input:checked').val();
                    if (concat != null)
                        row += '|' + concat + '|';

                    var comparison = $(this).find('.col-comparison select').val();

                    row += $(this).find('.col-score select option:selected').data('alt-value');
                    row += ':' + $(this).find('.col-score select').val();
                    row += ':' + $(this).find('.col-content-area .dm-ui-dropdown-widget:visible').prev('select').val();
                    row += ':' + comparison;
                    row += ':' + $(this).find('.col-val1 input').val();

                    if (comparison === 'BETWEEN')
                        row += ',' + $(this).find('.col-val2 input').val();

                    selections.push(row);
                });

                $(modalSelector).addClass('dm-ui-hide');

                $(hiddenInputSelector).val(JSON.stringify(selections)).trigger('change');
            });

        },

        AssignScoreWarningsGroupHandler: function (applyButtonSelector, cancelButtonSelector, modalSelector, hiddenInputSelector) {

            $(modalSelector).find('select[id^="sequence-dd"]').on('change', function () {
                var $switch = $(this).closest('.score-warnings-row').find('.col-switch > .dm-ui-switcher');
                var val = $(this).val();

                if (val === '')
                    $switch.addClass('dm-ui-hide');
                else
                    $switch.removeClass('dm-ui-hide');
            });

            $(cancelButtonSelector).on('click', function () {
                $(modalSelector).addClass('dm-ui-hide');

                $(modalSelector).find('.col-concat > .dm-ui-switcher').each(function () {
                    $(this).find('input[value=AND]').prop('checked', true);
                    $(this).find('input[value=OR]').prop('checked', false);
                });

                $(modalSelector).find('.dm-ui-dropdown-widget').each(function () {
                    $(this).DmUiApplySelection($(this).data('original-value'));
                });

                $(modalSelector).find('.col-switch > .dm-ui-switcher').each(function () {
                    $(this).find('input[value=Include]').prop('checked', true);
                    $(this).find('input[value=Exclude]').prop('checked', false);
                    $(this).addClass('dm-ui-hide');
                });
            });

            $(applyButtonSelector).on('keydown', function (e) {
                if (e.keyCode == 9 && !e.shiftKey) {
                    $('.score-warnings-row .score-warnings-col .dm-ui-dropdown-singleselect .dm-ui-dropdown-button').first().focus();
                    return false;
                }
            });

            $('.score-warnings-row .score-warnings-col .dm-ui-dropdown-singleselect .dm-ui-dropdown-button').on('keydown', function (e) {
                if (e.keyCode == 9 && e.shiftKey) {
                    var firstButton = $('.score-warnings-row .score-warnings-col .dm-ui-dropdown-singleselect .dm-ui-dropdown-button').first();

                    if ($(this).is(firstButton)) {
                        $(applyButtonSelector).focus();
                        return false;
                    }
                }
            });

            $(applyButtonSelector).on('click', function () {
                var selections = [];

                $(modalSelector).find('.score-warnings-row').not('.head-row').each(function () {
                    var concat = $(this).find('.col-concat input:checked').val();
                    var row = concat != null ? concat : '';
                    row += ':' + $(this).find('.col-dropdown select').val();
                    row += ':' + $(this).find('.col-switch input:checked').val();

                    selections.push(row);
                });

                $(modalSelector).addClass('dm-ui-hide');

                $(hiddenInputSelector).val(JSON.stringify(selections)).trigger('change');
            });
        },

        AssignCustomFieldsGroupHandler: function (applyButtonSelector, cancelButtonSelector, removeAllButtonSelector, draggerImgLink, userTextLength, modalSelector, hiddenInputSelector) {

            var handleGroupingCheckbox = function ($groupingCheckbox) {
                var groupingClass = '.' + $groupingCheckbox.data('items-class');

                var allChecked = $(groupingClass).not(':checked, .cdf-manual-selection').length === 0;
                $groupingCheckbox.prop('checked', allChecked);
                $groupingCheckbox.closest('.dm-ui-checkbox').attr('aria-checked', allChecked);
                var allDisabled = $(groupingClass).not(':disabled').length === 0;
                $groupingCheckbox.prop('disabled', allDisabled);
                if (allDisabled) {
                    $groupingCheckbox.parent('.dm-ui-checkbox').addClass('dm-ui-disabled');
                    $groupingCheckbox.parent('.dm-ui-checkbox').attr('aria-disabled', true);
                }
                else {
                    $groupingCheckbox.parent('.dm-ui-checkbox').removeClass('dm-ui-disabled');
                    $groupingCheckbox.parent('.dm-ui-checkbox').attr('aria-disabled', false);
                }
            };

            $('.cdf-selected-items-drop').sortable({
                axis: 'y',
                cancel: '.cdf-selected-item-body',
                cursor: 'grabbing'
            });

            $('.cdf-grouping-arrow-down').on('enterkeyup click', function () {
                var groupingListId = '#' + $(this).data('grouping-list-id');
                $(groupingListId).slideDown();
                $(this).hide();
                $(this).next('.cdf-grouping-arrow-up').show();
            });

            $('.cdf-grouping-arrow-up').on('enterkeyup click', function () {
                var groupingListId = '#' + $(this).data('grouping-list-id');
                $(groupingListId).slideUp();
                $(this).hide();
                $(this).prev('.cdf-grouping-arrow-down').show();
            });

            $('.cdf-default-item').on('click', function () {
                var groupingCheckboxId = '#' + $(this).closest('.cdf-grouping-items').data('grouping-id');
                handleGroupingCheckbox($(groupingCheckboxId));
            });

            $('.cdf-grouping-checkbox').on('change', function () {
                var selectorClass = '.' + $(this).data('items-class');
                $(selectorClass).not(':disabled, .cdf-manual-selection').prop('checked', $(this).is(':checked'));
                var groupUl = $(this).parent().parent().next().next().next();

                $(groupUl).find('.dm-ui-checkbox').attr('aria-checked', $(this).is(':checked'));
            });

            $('#cdf-select-all-link').on('enterkeyup click', function () {
                $('.cdf-available-items input[type=checkbox]').not(':disabled, .cdf-manual-selection').prop('checked', true);
                $('.cdf-available-items .dm-ui-checkbox-container .dm-ui-checkbox').attr('aria-checked', true);
                $('#divSelectedMessage').text('All elements selected');
            });

            $('#cdf-deselect-all-link').on('enterkeyup click', function () {
                $('.cdf-available-items input[type=checkbox]').not(':disabled').prop('checked', false);
                $('.cdf-available-items .dm-ui-checkbox-container .dm-ui-checkbox').attr('aria-checked', false);
                $('#divSelectedMessage').text('All elements deselected');
            });

            $(removeAllButtonSelector).on('click', function () {
                if ($('.cdf-selected-item').length === 0)
                    return;

                $('.cdf-selected-item').each(function () {
                    var defaultItemId = '#' + $(this).data('value');
                    $(defaultItemId).prop('disabled', false).prop('checked', false);
                    $(defaultItemId).parent('.dm-ui-checkbox').removeClass('dm-ui-disabled');
                    $(defaultItemId).parent('.dm-ui-checkbox').attr('aria-disabled', false);
                });

                $('.cdf-grouping-checkbox').prop('disabled', false).prop('checked', false);
                $('.cdf-grouping-checkbox').parent('.dm-ui-checkbox').removeClass('dm-ui-disabled');
                $('.cdf-grouping-checkbox').parent('.dm-ui-checkbox').attr('aria-disabled', false);

                $('.cdf-selected-items-drop').empty();
            });

            $(document).on('click', '.cdf-remove-individual-button', function () {
                var defaultItemId = '#' + $(this).data('value');
                $(defaultItemId).prop('disabled', false).prop('checked', false);
                $(defaultItemId).parent('.dm-ui-checkbox').removeClass('dm-ui-disabled');
                $(defaultItemId).parent('.dm-ui-checkbox').attr('aria-disabled', false);
                $(this).closest('.cdf-selected-item').remove();

                var groupingId = '#' + $(defaultItemId).closest('.cdf-grouping-items').data('grouping-id');
                handleGroupingCheckbox($(groupingId));
            });

            var handleUserInput = function ($textInput, $widthInput) {
                var $defaultValues = $textInput.closest('.cdf-selected-item').find('.cdf-default-values');

                var defaultText = $textInput.data('default-text');
                var defaultWidth = $widthInput.data('default-width');

                if (defaultText === $textInput.val() && parseInt(defaultWidth) === parseInt($widthInput.val()))
                    $defaultValues.hide();
                else
                    $defaultValues.show();
            };

            $(document).on('keyup change', '.cdf-selected-item-user-text', function () {
                handleUserInput($(this), $(this).siblings('.cdf-selected-item-user-width'));
            });

            $(document).on('keyup change', '.cdf-selected-item-user-width', function () {
                handleUserInput($(this).siblings('.cdf-selected-item-user-text'), $(this));
            });

            $(document).on('blur', '.cdf-selected-item-user-width', function () {
                var val = $(this).val();

                if (isNaN(val))
                    $(this).val($(this).data('default-width'));
                else if (val > userTextLength)
                    $(this).val(userTextLength);

                handleUserInput($(this).siblings('.cdf-selected-item-user-text'), $(this));
            });

            $(document).on('click', '#CustomDataFieldsModal .dm-ui-checkbox', function () {
                $(this).attr('aria-checked', $(this).find('input[type=checkbox]').prop('checked'));
            });

            $(document).on('keydown', '#cdf-add-to-selected-button', function (e) {
                if (e.keyCode == 9) {

                    if (e.shiftKey) {
                        var lastDownArrow = $('.cdf-available-items .cdf-grouping-arrow-down').last();

                        if ($(lastDownArrow).length > 0 && $(lastDownArrow).is(":visible")) {
                            $(lastDownArrow).last().focus();
                        }
                        else {
                            $('.cdf-available-items .cdf-grouping-items .cdf-item .dm-ui-checkbox-container .dm-ui-checkbox').last().focus();
                        }

                    }
                    else {
                        var firstItem = $('.cdf-selected-items-drop .cdf-user-controls .cdf-selected-item-user-text').first();

                        if ($(firstItem).length > 0) {
                            $(firstItem).focus();
                        }
                        else {
                            $('#CustomDataFieldsModal-cancel-button').focus();
                        }
                    }

                    return false;
                }
            });

            $('.cdf-available-items').on('keydown', function (e) {
                if (e.keyCode == 27) {
                    $('#cdf-add-to-selected-button').focus();
                    return false;
                }
            });

            $('.custom-fields-selected-items').on("keydown", function (e) {
                if (e.keyCode == 27) {
                    $('#CustomDataFieldsModal-cancel-button').focus();
                    return false;
                }
            });

            $(document).on('keydown', '#CustomDataFieldsModal-cancel-button', function (e) {
                if (e.keyCode == 9 && e.shiftKey) {
                    $('#cdf-add-to-selected-button').focus();
                    return false;
                }
            });

            $(document).on('keydown', '.cdf-available-items .cdf-grouping-arrow-down', function (e) {
                if (e.keyCode == 9 && !e.shiftKey) {
                    if ($(this).is($(this).parent().children('.cdf-grouping-arrow-down').last())) {
                        $('#cdf-add-to-selected-button').focus();
                        return false;
                    }
                }
            });

            $('.cdf-grouping-arrow').on('keydown', function (e) {
                if (e.keyCode == 32) {
                    $(this).click();
                }
            });

            $(document).on('keydown', '#cdf-select-all-link', function (e) {
                if (e.keyCode == 9 && e.shiftKey) {
                    $('#CustomDataFieldsModal-remove-all-button').focus();
                    return false;
                }
            });

            $(document).on('keydown', '.cdf-available-items .dm-ui-checkbox-container', function (e) {
                if (e.keyCode == 9 && !e.shiftKey) {
                    var container = $('.cdf-available-items').first();

                    if ($(this).parent().prop('nodeName') == 'LI') {
                        var li = $(this).parent();
                        var ul = $(this).parent().parent();

                        if ($(ul).is($(container).find('UL').last())) {
                            if ($(li).is($(ul).children().last())) {
                                $('#cdf-add-to-selected-button').focus();
                                return false;
                            }
                        }
                    }
                }
            });

            $(document).on('keydown', '#CustomDataFieldsModal-remove-all-button', function (e) {
                if (e.keyCode === 9 && !e.shiftKey) {
                    $('#cdf-add-to-selected-button').focus();
                }

            });

            $('#cdf-add-to-selected-button').on('click', function () {

                var itemTemplate =
                    '<div class="cdf-selected-item" data-value="@itemValue">' +
                    '<img class="cdf-dragger" src="@draggerImgLink" />' +
                    '<div class="cdf-selected-item-body">' +
                    '<div class="cdf-user-controls">' +
                    '<input type="text" class="cdf-selected-item-user-text" aria-describedby="divDataFieldLabel" aria-label="@defaultText" value="@defaultText" data-default-text="@defaultText" />' +
                    '<input type="text" class="cdf-selected-item-user-width" aria-describedby="divValueLength" aria-label="@defaultWidth" value="@defaultWidth" data-default-width="@defaultWidth" />' +
                    '<button class="dm-ui-button-primary dm-ui-button-small cdf-remove-individual-button" data-value="@itemValue">Remove</button>' +
                    '</div>' +
                    '<div class="cdf-default-values" style="display:none;">' +
                    '<div class="cdf-selected-item-text">@defaultText</div>' +
                    '<div class="cdf-selected-item-width">@defaultWidth</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';

                $('.cdf-default-item:checked').not(':disabled').each(function () {
                    var newItem = itemTemplate
                        .replace('@draggerImgLink', draggerImgLink)
                        .replace(/@defaultText/g, $(this).data('text'))
                        .replace(/@defaultWidth/g, $(this).data('width'))
                        .replace(/@itemValue/g, $(this).data('value'));
                    $('.cdf-selected-items-drop').append(newItem);
                    $(this).prop('disabled', true);
                    $(this).parent('.dm-ui-checkbox').addClass('dm-ui-disabled');
                    $(this).parent('.dm-ui-checkbox').attr('aria-disabled', true);
                });

                $('.cdf-grouping-checkbox').each(function () {
                    handleGroupingCheckbox($(this));
                });

                $('#divSelectedMessage').text('Selected items added to selected data fields editor');
            });

            $(cancelButtonSelector).on('click', function () {
                $(modalSelector).addClass('dm-ui-hide');

                var placeHolder = $(this).closest('.option-group');

                $.ajax({
                    type: 'POST',
                    url: siteRoot + '/Options/GetGroup',
                    data: JSON.stringify({ groupType: $(hiddenInputSelector).prop('id') }),
                    success: function (data) {
                        $(placeHolder).replaceWith(data);
                    },
                    complete: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });

            $(applyButtonSelector).on('click', function () {
                var selections = [];

                $('.cdf-selected-items-drop .cdf-selected-item').each(function () {
                    var val = $(this).data('value');
                    var text = $(this).find('.cdf-selected-item-user-text').val();
                    var width = $(this).find('.cdf-selected-item-user-width').val();

                    selections.push(val + '~' + text + '~' + width);
                });

                $(modalSelector).addClass('dm-ui-hide');

                $(hiddenInputSelector).val(JSON.stringify(selections)).trigger('change');
            });
        },

        AssignPerformanceBandsGroupHandler: function (applyButtonSelector, cancelButtonSelector, modalSelector, hiddenInputSelector) {

            $(cancelButtonSelector).on('click', function () {
                $(modalSelector).addClass('dm-ui-hide');

                $(modalSelector).find('input[type=text]').each(function () {
                    $(this).val($(this).data('original-value'));
                });
            });

            $(applyButtonSelector).on('keydown', function (e) {
                if (e.keyCode == 9 && !e.shiftKey) {
                    $('#inputBand0').focus();
                    return false;
                }
            });

            $('#inputBand0').on('keydown', function (e) {
                if (e.keyCode == 9 && e.shiftKey) {
                    $(applyButtonSelector).focus();
                    return false;
                }
            });

            $(applyButtonSelector).on('click', function () {
                var selections = [];

                $(modalSelector).find('.band-row').not('.band-row-heading').each(function () {
                    var lowValue = $(this).find('.band-low-value input').val();
                    var highValue = $(this).find('.band-high-value input').val();
                    var $category = $(this).find('.band-category input');

                    selections.push($category.data('color') + ':' + $category.val() + ',' + lowValue + ',' + highValue);
                });

                $(modalSelector).addClass('dm-ui-hide');

                $(hiddenInputSelector).val(JSON.stringify(selections)).trigger('change');
            });

        },

        OptionsChangeHandler: function () {
            var self = this;

            $(document).on('change', '.dm-ui-hidden-input', function () {
                focusId = this.id;

                var groupType = this.id;
                var values = $(this).val();
                var data = JSON.stringify({ groupType: groupType, values: JSON.parse(values) });

                $.ajax({
                    type: 'POST',
                    url: siteRoot + '/Options/UpdateOptions',
                    data: data,
                    dataType: 'json',
                    beforeSend: function () {
                        DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType))
                            self.GetOptions(true);
                    },
                    error: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });
        },

        GetOptions: function (hideSpinnerOnComplete) {
            $.ajax({
                type: 'GET',
                url: siteRoot + '/Options/GetOptions',
                success: function (data) {
                    if (data.indexOf('<div class="dm-ui-alert') === 0) {
                        $('.options-placeholder').empty();
                        DmUiLibrary.DisplayAlert(data, this.dataType);
                    }
                    else {
                        $('.options-placeholder').html(data);

                        //for COVID reports
                        $('#DisplayType_Control_dm_ui li[data-value=EGSR]').html($('#DisplayType_Control_dm_ui li[data-value=EGSR]').text());
                        $('#DisplayOptions_Control_dm_ui li[data-value=IEGSSA]').html($('#DisplayOptions_Control_dm_ui li[data-value=IEGSSA]').text());

                        $('#current-location-name').html($('#location-name').val());

                        if (focusId > 0) {
                            var $triggeredInput = $('#' + focusId);
                            $triggeredInput.siblings('.dm-ui-dropdown-widget').find('.dm-ui-dropdown-button').focus();
                            $triggeredInput.siblings('.launch-group-modal').focus();
                        }

                        if ($('#run-in-foreground').val() === 'true')
                            $('#run-report-button').show();
                        else
                            $('#run-report-button').hide();

                        if ($('#invalid-group-alert').length > 0)
                            DmUiLibrary.DisplayAlert(JSON.parse($('#invalid-group-alert').val()), 'json');
                    }
                },
                complete: function () {
                    if (hideSpinnerOnComplete)
                        DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        ResetOptions: function () {
            var self = this;

            $.ajax({
                type: 'POST',
                url: siteRoot + '/Options/ResetOptions',
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function () {
                    self.GetOptions(true);
                },
                error: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        DisplayLocationChangeModal: function () {
            var self = this;

            $('#change-main-location-link').on('click enterkeyup', function () {
                $.ajax({
                    type: 'GET',
                    url: siteRoot + '/Options/DisplayLocationChangeModal',
                    beforeSend: function () {
                        DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        $('#modal-placeholder').html(data);
                        $('#modal-placeholder').find('button:first').focus();

                        $('#location-change-cancel-button').on('click', function () {
                            $('#modal-placeholder').empty();
                        });

                        $('#location-change-apply-button').on('keydown', function (e) {
                            if (e.keyCode == 9 && !e.shiftKey) {
                                $('#location-change-dropdown_dm_ui .dm-ui-dropdown-button').focus();
                                return false;
                            }
                        });

                        $('#location-change-dropdown_dm_ui .dm-ui-dropdown-button').on('keydown', function (e) {
                            if (e.keyCode == 9 && e.shiftKey) {
                                $('#location-change-apply-button').focus();
                                return false;
                            }
                        });

                        $('#location-change-apply-button').on('click', function () {
                            self.SwitchLocation();
                        });
                    },
                    complete: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });
        },

        SwitchLocation: function () {
            $.ajax({
                type: 'POST',
                url: siteRoot + '/Options/SwitchLocation',
                data: JSON.stringify({ id: $('#location-change-dropdown').val() }),
                beforeSend: function () {
                    $('#modal-placeholder').empty();
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function () {
                    window.location = siteRoot + '/Options';
                }
            });
        },

        InitButtonsHandlers: function () {
            var self = this;

            $(document).on('click enterkeyup', '.launch-group-modal', function () {

                $('#' + $(this).data('modal-id')).removeClass('dm-ui-hide');

                if ($(this).data('modal-id') == 'PerformanceBandsModal') {
                    $('#inputBand0').focus();
                }
                else if ($(this).data('modal-id') == 'CustomDataFieldsModal') {
                    $('#cdf-select-all-link').focus();
                }
                else if ($(this).data('modal-id') == 'LongitudinalTestAdministrationsModal') {
                    $('#LongitudinalTestAdministrationsModal .dm-ui-modal section .left-column .dm-ui-checkbox-container .dm-ui-checkbox:tabbable').first().focus();
                }
                else {
                    $('#' + $(this).data('modal-id')).find('button:first').focus();
                }

                if ($(this).data('modal-id') == 'PopulationFiltersModal') {
                    $('.dm-ui-modal button:first').on('keydown', function (e) {
                        if (e.keyCode == 9 && e.shiftKey) {
                            $('.dm-ui-modal .dm-ui-button-primary').focus();
                            return false;
                        }
                    });

                    $('.dm-ui-modal .dm-ui-button-primary').on('keydown', function (e) {
                        if (e.keyCode == 9 && !e.shiftKey) {
                            $('.dm-ui-modal button:first').focus();
                            return false;
                        }
                    });
                }
            });

            $('#reset-criteria-button').on('click', function () {
                self.ResetOptions();
            });

            $('#save-criteria-button').on('click', function () {
                Criteria.DisplaySaveCriteriaModal();
            });

            $('#run-report-in-background').on('click', function () {
                Criteria.DisplayRunInBackgroundModal();
            });

            $('#run-report-button').on('click', function () {
                Common.RunReport(false);
            });

            //Edit Mode buttons
            $('#save-changes-button').on('click', function () {
                Criteria.UpdateCriteria($('#criteriaId').val(), $('#criteria-name-text').val(), $('#criteria-description-text').val());
            });

            $('#save-as-copy-button').on('click', function () {
                Criteria.DisplaySaveCriteriaModal(function () {
                    $('#criteria-name').val($('#criteria-name-text').val() + ' - Copy');
                    $('#criteria-summary').val($('#criteria-description-text').val());
                });
            });

            //Multimeasure buttons
            $(document).on('enterkeyup click', '#add-column-button', function () {
                self.ManipulateMultimeasureColumns(siteRoot + '/Options/AddMultimeasureColumn');
            });

            $(document).on('enterkeyup click', '#delete-column-button', function () {
                self.ManipulateMultimeasureColumns(siteRoot + '/Options/DeleteMultimeasureColumn');
            });

            $(document).on('enterkeyup click', '.go-to-column-link', function () {
                self.ManipulateMultimeasureColumns(
                    siteRoot + '/Options/GoToMultimeasureColumn',
                    JSON.stringify({ columnNumber: $(this).data('go-to-column') })
                );
            });

            $(document).on('click', '#exit-edit-mode-button', function () {
                Criteria.DisplayUnsavedChangesModal();
            });
        },

        ClearEditMode: function (callback) {
            $.ajax({
                type: 'POST',
                url: siteRoot + '/Options/ClearEditMode',
                success: function () {
                    callback();
                }
            });
        },

        ManipulateMultimeasureColumns: function (url, data) {
            var self = this;

            $.ajax({
                type: 'POST',
                url: url,
                data: data,
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function () {
                    self.GetOptions(true);
                }
            });
        }
    };
}();