(function ($) {

    /* ----- Functions ----- */
    function selectSingleDropdownOption(self, value) {
        var $selectFancy = $("#" + self.attr("id") + "_dm_ui");
        self.val(value);
        var val = self.val();

        if (value === "") {
            var $selectOptions = self.children();
            $selectOptions.removeAttr("selected");

            $selectFancy.find("li").removeClass("dm-ui-selected");

            var $button = $selectFancy.find(".dm-ui-dropdown-button");
            $button.html('<span class="placeholder">' + $button.data("default-text") + "</span>");
            //$button.removeAttr("aria-describedby");
        }
        else {
            var item = $selectFancy.find('li[data-value="' + val + '"]:first');
            $(item).click();
        }
        return self;
    }

    function selectMultiDropdownOption(self, value) {
        var $selectFancy = $("#" + self.attr("id") + "_dm_ui");

        var $selectOptions = self.children();
        var arrSelectedOptions = [];
        if (value) {
            $selectOptions.each(function () {
                if ($(this).attr("value") === value) {
                    $(this).attr("selected", "selected");
                }
                if ($(this).attr("selected")) {
                    arrSelectedOptions.push($(this).attr("value"));
                }
            });
        }

        var $selectFancyOptions = $selectFancy.find(".dm-ui-dropdown-items li div");
        $selectFancyOptions.each(function () {
            if ($.inArray($(this).find("input").attr("value"), arrSelectedOptions) !== -1) {
                $(this).attr("aria-checked", "true");
                $(this).find("input").attr("checked", "checked");
                $(this).find("input").prop("checked", true);
            } else {
                $(this).attr("aria-checked", "false");
                $(this).find("input").prop("checked", false);
                $(this).find("input").removeAttr("checked");
            }
        });
        $selectFancy.find(".dm-ui-apply-button").click();

        return self;
    }

    $.fn.DmUiApplySelection = function (value) {
        var isMultiSelect = false;
        if (this.attr("multiple")) {
            isMultiSelect = true;
        }
        if (isMultiSelect) {
            selectMultiDropdownOption(this, value);
            if (this.hasClass("tree")) {
                DmUiLibrary.ResetMultiSelect(this.next().find(".dm-ui-dropdown-button").first());
            }
        } else {
            selectSingleDropdownOption(this, value);
        }
    };

    $.fn.DmUiRecreateDropdownContentSingleSelection = function () {
        var $selectFancy = $("#" + this.attr("id") + "_dm_ui");
        var $button = $selectFancy.find(".dm-ui-dropdown-button");
        var selectType = "";
        if (typeof $selectFancy.data("type") !== "undefined") {
            selectType = $selectFancy.data("type");
        }
        var $label = $("#" + "Label_" + this.attr("id"));
        var selectId = this.attr("id");
        var isAnythingSelected = false;
        var $selectOptions = this.children();
        var selectedOptionValue;
        $selectOptions.each(function () {
            if ($(this).prop("selected")) {
                selectedOptionValue = $(this).attr("value");
                if ($(this).text() !== "") {
                    isAnythingSelected = true;
                }
                return;
            }
        });
        var $selectFancyOptions = $selectFancy.find(".dm-ui-dropdown-items");
        $selectFancyOptions.empty();
        $selectOptions.each(function () {
            if ($(this).text() !== "") {
                if ($(this).attr("value") === selectedOptionValue) {
                    $selectFancyOptions.append('<li aria-checked="true" class="dm-ui-selected dm-ui-li-item" data-alt-value="' + $(this).attr("data-alt-value") + '" data-value="' + $(this).attr("value") + '" id="' + DmUiLibrary.RandomStringGenerator(12) + '_dm_ui" role="option" tabindex="0">' + $(this).text() + "</li>");
                } else {
                    $selectFancyOptions.append('<li aria-checked="false" class="dm-ui-li-item" data-alt-value="' + $(this).attr("data-alt-value") + '" data-value="' + $(this).attr("value") + '" id="' + DmUiLibrary.RandomStringGenerator(12) + '_dm_ui" role="option" tabindex="0">' + $(this).text() + "</li>");
                }
            }
        });
        if (isAnythingSelected) {
            var selectedText = $("#" + selectId + " option:selected").text();
            if (selectType !== "") {
                $button.html("<span>" + selectType + ": " + selectedText + "</span>");
            } else {
                $button.html("<span>" + selectedText + "</span>");
            }
        } else {
            $button.html('<span class="placeholder">' + $button.data("default-text") + "</span>");
            //$button.removeAttr("aria-describedby");
        }
        if ($selectOptions.length && !this.prop("disabled")) {
            $selectFancy.removeClass("dm-ui-disabled");
            $label.removeClass("dm-ui-disabled");
            $button.prop("disabled", false);
        } else {
            $selectFancy.addClass("dm-ui-disabled");
            $label.addClass("dm-ui-disabled");
            $button.prop("disabled", true);
        }
    };

    $.fn.DmUiRecreateDropdownContentMultiSelection = function () {
        var $selectFancy = $("#" + this.attr("id") + "_dm_ui");
        var $button = $selectFancy.find(".dm-ui-dropdown-button");
        var selectType = "";
        if (typeof $selectFancy.data("type") !== "undefined") {
            selectType = $selectFancy.data("type");
        }
        var $label = $("#" + "Label_" + this.attr("id"));
        var selectId = this.attr("id");
        var $selectOptions = this.children();
        var arrSelectedOptions = [];
        $selectOptions.each(function () {
            if ($(this).prop("selected")) {
                arrSelectedOptions.push($(this).attr("value"));
            }
        });
        var $selectFancyOptions = $selectFancy.find(".dm-ui-dropdown-items");
        $selectFancyOptions.empty();
        var inputId, inputValue, inputAltValue;
        var numHidden = 0;

        var isToolTip = $selectFancy.hasClass("tooltip");
        var tooltiptext = '';

        $selectOptions.each(function (i) {
            inputId = selectId + "_" + i;
            inputValue = "";
            if (typeof $(this).attr("value") !== "undefined") {
                inputValue = $(this).attr("value");
            }
            inputAltValue = "";
            if (typeof $(this).attr("data-alt-value") !== "undefined") {
                inputAltValue = $(this).attr("data-alt-value");
            }

            var itemHidden = ($(this).attr("class") == "dm-ui-hide");
            var itemDisabled = ($(this).attr("class") == "dm-ui-disabled");

            var liTag = "<li>";
            var checkboxClass = "dm-ui-checkbox dm-ui-menuitem-checkbox";
            var checkBoxDisabled = "";
            var tabIndex = "0";
            var ariaDisabled = "";

            if (itemHidden) {
                liTag = '<li class="dm-ui-hide">';
            }
            else if (itemDisabled) {
                liTag = '<li aria-disabled="true">';
                checkboxClass = checkboxClass + " dm-ui-disabled";
                checkBoxDisabled = ' disabled="true"';
                tabIndex = "-1";
                ariaDisabled = ' aria-disabled="true"';
            }

            if ($.inArray($(this).attr("value"), arrSelectedOptions) !== -1) {
                if (itemHidden) {
                    numHidden += 1;
                }
                if (isToolTip && !itemHidden) {
                    tooltiptext = tooltiptext + "<div class='tooltipitem'>" + $(this).text() + "</div>";
                }

                $selectFancyOptions.append(liTag + '<div aria-checked="true"' + ariaDisabled + ' class="' + checkboxClass + '" role="checkbox" tabindex="' + tabIndex + '"><input checked="checked" data-alt-value="' + inputAltValue + '" data-initially-selected="true"' + checkBoxDisabled + ' id="' + inputId + '" name="' + inputId + '" type="checkbox" value="' + inputValue + '"><label for="' + inputId + '">' + $(this).text() + "</label></div></li>");
            } else {
                $selectFancyOptions.append(liTag + '<div aria-checked="false"' + ariaDisabled + ' class="' + checkboxClass + '" role="checkbox" tabindex="' + tabIndex + '"><input data-alt-value="' + inputAltValue + '" data-initially-selected="false"' + checkBoxDisabled + ' id="' + inputId + '" name="' + inputId + '" type="checkbox" value="' + inputValue + '"><label for="' + inputId + '">' + $(this).text() + "</label></div></li>");
            }
        });

        if (isToolTip) {
            var $tooltip = $selectFancy.find('.tooltiptext');

            if ($tooltip.length > 0) {
                if (tooltiptext.length > 0) {
                    $tooltip.html(tooltiptext);
                }
                else {
                    $tooltip.remove();
                }
            }
            else if (tooltiptext.length > 0) {
                $selectFancy.prepend($("<div class='tooltiptext'>" + tooltiptext + "</div>"));
            }
        }
        var numSelected = $("#" + selectId + " option:selected").length - numHidden;

        if (numSelected) {
            if (selectType !== "" && numSelected >= 2) {
                if (selectType.slice(-1).toUpperCase() === "S") {
                    selectType += "es";
                } else {
                    selectType += "s";
                }
            }
            $button.text(numSelected + " " + selectType + " selected");
        } else {
            $button.html('<span class="placeholder">' + $button.data("default-text") + "</span>");
        }
        $(this).find(".dm-ui-apply-button").click();

        if ($selectOptions.length && !this.prop("disabled")) {
            $selectFancy.removeClass("dm-ui-disabled");
            $label.removeClass("dm-ui-disabled");
            $button.prop("disabled", false);

            var isApplyButtonEnabled = true;
            if (typeof $selectFancy.attr("data-min-to-select")) {
                if (numSelected < $selectFancy.data("min-to-select")) {
                    isApplyButtonEnabled = false;
                }
            }
            if (typeof $selectFancy.attr("data-max-to-select")) {
                if (numSelected > $selectFancy.data("max-to-select")) {
                    isApplyButtonEnabled = false;
                }
            }
            if (isApplyButtonEnabled) {
                $selectFancy.find(".dm-ui-apply-button").removeAttr("disabled");
            }
        } else {
            $selectFancy.addClass("dm-ui-disabled");
            $label.addClass("dm-ui-disabled");
            $button.prop("disabled", true);
        }
    };

    function nodesOfTree(arrNodesSting, isExpandedNode) {
        var treeHtml = "";
        var treeString;
        var arrTree = [];
        var result = "";
        var partialTree = "";
        var nodeStyle = "";
        var nodeArrowClass = " opened";
        var ariaExpanded = "true";
        var isSubNodeNotExist = true;
        if (!isExpandedNode) {
            nodeStyle = "display: none;";
            nodeArrowClass = "";
            ariaExpanded = "false";
        }
        var checkboxGroup = "";
        var inputId = "";
        var groupOptionsId = "";
        var ul;
        for (var s = 0; s < arrNodesSting.length; ++s) {
            treeString = arrNodesSting[s];
            arrTree = treeString.split("|");
            result = "";
            partialTree = "";
            isSubNodeNotExist = true;
            checkboxGroup = "";
            inputId = "";
            groupOptionsId = "";

            for (var i = arrTree.length - 1; i >= 0; --i) {
                partialTree = "";
                for (var j = 0; j <= i; ++j) {
                    partialTree += arrTree[j];
                    if (j < i) {
                        partialTree += "|";
                    }
                }
                inputId = DmUiLibrary.RandomStringGenerator(12);
                groupOptionsId = "dm-ui-group-options-" + DmUiLibrary.RandomStringGenerator(12);
                checkboxGroup = '<div aria-checked="false" class="dm-ui-checkbox dm-ui-menuitem-checkbox" role="checkbox" tabindex="0"><input class="checkbox_group" id="' + inputId + '" name="' + inputId + '" type="checkbox"><label for="' + inputId + '">' + arrTree[i] + "</label></div>";
                isSubNodeNotExist = (treeHtml.indexOf(' data-node="' + partialTree + '"') === -1);
                if (isSubNodeNotExist) {
                    if (i === 0) {
                        if (arrTree.length === 1) {
                            result = checkboxGroup + '<div class="arrow' + nodeArrowClass + '" tabindex="0" aria-expanded="' + ariaExpanded + '" role="button" aria-controls="' + groupOptionsId + '"></div><ul class="dm-ui-dropdown-items" id="' + groupOptionsId + '" style="' + nodeStyle + '" data-node="' + partialTree + '">' + result + '</ul data-end-node="' + partialTree + '">';
                        }
                        else {
                            result = checkboxGroup + '<div class="arrow' + nodeArrowClass + '" tabindex="0" aria-expanded="' + ariaExpanded + '" role="button" aria-controls="' + groupOptionsId + '"></div><ul class="dm-ui-dropdown-items" id="' + groupOptionsId + '" style="' + nodeStyle + '" data-node="' + partialTree + '"><li class="group-inside">' + result + '</li></ul data-end-node="' + partialTree + '">';
                        }
                    }
                    else {
                        ul = '<ul class="dm-ui-dropdown-items-group">';
                        if (i === arrTree.length - 1) {
                            ul += '<li class="group" role="group">' + checkboxGroup + '<div class="arrow' + nodeArrowClass + '" tabindex="0" aria-expanded="' + ariaExpanded + '" role="button" aria-controls="' + groupOptionsId + '"></div><ul class="dm-ui-dropdown-items" id="' + groupOptionsId + '" style="' + nodeStyle + '" data-node="' + partialTree + '">' + result + '</ul data-end-node="' + partialTree + '"></li>';
                        }
                        else {
                            ul += '<li class="group" role="group">' + checkboxGroup + '<div class="arrow' + nodeArrowClass + '" tabindex="0" aria-expanded="' + ariaExpanded + '"` role="button" aria-controls="' + groupOptionsId + '"></div><ul class="dm-ui-dropdown-items" id="' + groupOptionsId + '" style="' + nodeStyle + '" data-node="' + partialTree + '"><li class="group-inside">' + result + '</li></ul data-end-node="' + partialTree + '"></li>';
                        }
                        ul += "</ul>";
                        result = ul;
                    }
                } else {
                    break;
                }
            }
            if (result !== "") {
                if (isSubNodeNotExist) {
                    result = treeHtml + '<li class="group" role="group">' + result + "</li>";
                } else {
                    var beginText = treeHtml.substring(0, treeHtml.indexOf('</ul data-end-node="' + partialTree + '">'));
                    treeHtml = treeHtml.substring(treeHtml.indexOf('</ul data-end-node="' + partialTree + '">'));
                    result = beginText + '<li class="group-inside">' + result + "</li>" + treeHtml;
                }
            }
            treeHtml = result;
        }
        return result;
    }

    $.fn.DmUiRecreateDropdownContentTreeMultiSelection = function () {
        var $selectFancy = $("#" + this.attr("id") + "_dm_ui");
        var isExpanded = $selectFancy.data("expanded");
        if (isExpanded !== true) {
            isExpanded = false;
        }
        var $button = $selectFancy.find(".dm-ui-dropdown-button");
        var selectType = "";
        if (typeof $selectFancy.data("type") !== "undefined") {
            selectType = $selectFancy.data("type");
        }
        var $label = $("#" + "Label_" + this.attr("id"));
        var selectId = this.attr("id");
        var $selectOptions = this.children();
        var arrSelectedOptionsIndex = [];
        var $selectFancyOptions = $selectFancy.find(".dm-ui-dropdown-items-group");
        $selectFancyOptions.empty();
        var selectFancyOptionsString = "";

        var tmpNode;
        var isNodeLongestUnique = false;
        var arrNodes = [];
        var arrUniqueNodes = [];
        var arrLongestUniqueNodes = [];

        var inputId, inputValue, inputAltValue;
        var ulGroup, li;
        var beginText = "";
        var endText = "";
        var searchText = "";
        var strPos = 0;
        var strPos1 = 0;
        var strPos2 = 0;
        var numOfInnerUl = 0;

        //create tree nodes structure
        $selectOptions.each(function (index) {
            tmpNode = $(this).attr("data-group");
            if (typeof tmpNode !== "undefined") {
                arrNodes.push(tmpNode);
                if ($(this).prop("selected")) {
                    arrSelectedOptionsIndex.push(index);
                }
            }
        });
        arrUniqueNodes = $.grep(arrNodes, function (v, k) {
            return $.inArray(v, arrNodes) === k;
        });
        for (var i = 0; i < arrUniqueNodes.length; ++i) {
            tmpNode = arrUniqueNodes[i];
            isNodeLongestUnique = true;
            for (var j = i + 1; j < arrUniqueNodes.length; ++j) {
                if (arrUniqueNodes[j].indexOf(tmpNode + "|") === 0) {
                    isNodeLongestUnique = false;
                    break;
                }
            }
            if (isNodeLongestUnique) {
                arrLongestUniqueNodes.push(tmpNode);
            }
        }
        selectFancyOptionsString = nodesOfTree(arrLongestUniqueNodes, isExpanded);
        $selectFancyOptions.html(selectFancyOptionsString); //to clear </ul data-end-node="">
        selectFancyOptionsString = $selectFancyOptions.html();
        $selectFancyOptions.empty();

        for (var i = 0; i < arrUniqueNodes.length; ++i) {
            ulGroup = "";
            $selectOptions.each(function (index) {
                tmpNode = $(this).attr("data-group");
                if (typeof tmpNode !== "undefined") {
                    if (tmpNode === arrUniqueNodes[i]) {
                        if ($(this).attr("disabled")) {
                            li = '<li class="dm-ui-disabled" aria-disabled="true">';
                        } else {
                            li = '<li>';
                        }
                        inputId = selectId + "_" + index;
                        if (typeof $(this).attr("value") !== "undefined") {
                            inputValue = $(this).attr("value");
                        } else {
                            inputValue = "";
                        }
                        if (typeof $(this).attr("data-alt-value") !== "undefined") {
                            inputAltValue = $(this).attr("data-alt-value");
                        } else {
                            inputAltValue = "";
                        }
                        if (arrSelectedOptionsIndex.indexOf(index) !== -1) {
                            li += '<div aria-checked="true" class="dm-ui-checkbox dm-ui-menuitem-checkbox"role="checkbox" tabindex="0"><input checked="checked" data-alt-value="' + inputAltValue + '" data-initially-selected="true" id="' + inputId + '" name="' + inputId + '" type="checkbox" value="' + inputValue + '"><label for="' + inputId + '">' + $(this).text() + "</label></div></li>";
                        } else {
                            li += '<div aria-checked="false" class="dm-ui-checkbox dm-ui-menuitem-checkbox" role="checkbox" tabindex="0"><input data-alt-value="' + inputAltValue + '" data-initially-selected="false" id="' + inputId + '" name="' + inputId + '" type="checkbox" value="' + inputValue + '"><label for="' + inputId + '">' + $(this).text() + "</label></div></li>";
                        }
                        ulGroup += li;
                    }
                }
            });

            numOfInnerUl = 0;
            searchText = 'data-node="' + arrUniqueNodes[i] + '">';
            strPos = selectFancyOptionsString.indexOf(searchText) + searchText.length;
            strPos1 = selectFancyOptionsString.indexOf("<ul", strPos);
            strPos2 = selectFancyOptionsString.indexOf("</ul>", strPos);
            while ((strPos1 < strPos2) && (strPos1 !== -1)) {
                numOfInnerUl++;
                strPos = strPos1 + 3;
                strPos1 = selectFancyOptionsString.indexOf("<ul", strPos);
            }
            for (var j = 1; j <= numOfInnerUl + 1; ++j) {
                strPos = selectFancyOptionsString.indexOf("</ul>", strPos) + 5;
            }
            strPos = strPos - 5;
            beginText = selectFancyOptionsString.substring(0, strPos);
            endText = selectFancyOptionsString.substring(strPos);
            selectFancyOptionsString = beginText + ulGroup + endText;
        }
        document.getElementById(this.attr("id") + "_dm_ui").getElementsByClassName("dm-ui-dropdown-items-group")[0].insertAdjacentHTML('afterBegin', selectFancyOptionsString);
        delete selectFancyOptionsString;

        var numSelected = $("#" + selectId + " option:selected").length;
        if (numSelected) {
            if (selectType !== "" && numSelected >= 2) {
                if (selectType.slice(-1).toUpperCase() === "S") {
                    selectType += "es";
                } else {
                    selectType += "s";
                }
            }
            $button.text(numSelected + " " + selectType + " selected");
        } else {
            $button.html('<span class="placeholder">' + $button.data("default-text") + "</span>");
        }
        $(this).find(".dm-ui-apply-button").click();

        if ($selectOptions.length && !this.prop("disabled")) {
            $selectFancy.removeClass("dm-ui-disabled");
            $label.removeClass("dm-ui-disabled");
            $button.prop("disabled", false);

            var isApplyButtonEnabled = true;
            if (typeof $selectFancy.attr("data-min-to-select")) {
                if (numSelected < $selectFancy.data("min-to-select")) {
                    isApplyButtonEnabled = false;
                }
            }
            if (typeof $selectFancy.attr("data-max-to-select")) {
                if (numSelected > $selectFancy.data("max-to-select")) {
                    isApplyButtonEnabled = false;
                }
            }
            if (isApplyButtonEnabled) {
                $selectFancy.find(".dm-ui-apply-button").removeAttr("disabled");
            }
        } else {
            $selectFancy.addClass("dm-ui-disabled");
            $label.addClass("dm-ui-disabled");
            $button.prop("disabled", true);
        }
        DmUiLibrary.ResetMultiSelect($button);
    };


    /* ----- Events ----- */

    $.event.special.enterkeyup = {
        delegateType: "keyup",
        bindType: "keyup",
        handle: function (event) {
            if (event.keyCode === 13 || event.keyCode === 32)
                return event.handleObj.handler.apply(this, arguments);
        }
    };

    $.event.special.escapekeyup = {
        delegateType: "keyup",
        bindType: "keyup",
        handle: function (event) {
            if (event.keyCode === 27)
                return event.handleObj.handler.apply(this, arguments);
        }
    };

    $.event.special.tabkeydown = {
        delegateType: "keydown",
        bindType: "keydown",
        handle: function (event) {
            if (event.keyCode === 9)
                return event.handleObj.handler.apply(this, arguments);
        }
    };

    $.event.special.shifttabkeydown = {
        delegateType: "keydown",
        bindType: "keydown",
        handle: function (event) {
            if (event.keyCode === 9 && event.shiftKey)
                return event.handleObj.handler.apply(this, arguments);
        }
    };

}(jQuery));