var DmUiMainMenu = function () {

    var p = {
        Key: Object.freeze({
            'LEFT': 37,
            'UP': 38,
            'RIGHT': 39,
            'DOWN': 40,
            'TAB': 9
        }),
        MoveRight: function ($nextItem) {
            var $itemToFocus = $nextItem.length === 0 ? $('.main-menu > li:first > a') : $nextItem.children('a');
            $itemToFocus.focus();
        },
        MoveLeft: function ($prevItem) {
            var $itemToFocus = $prevItem.length === 0 ? $('.main-menu > li:last > a') : $prevItem.children('a');
            $itemToFocus.focus();
        },
        ResetSelection: function () {
            $('.main-menu .has-sub-menu').attr('aria-expanded', 'false');


            $('.main-menu .main-menu-arrow svg use').attr('xlink:href', '#base-arrow-down');
            $('.main-menu .main-menu-arrow-up').removeClass('main-menu-arrow-up').addClass('main-menu-arrow-down');


            $('.main-menu .selected-item').removeClass('selected-item');
            $('.main-menu .sub-menu').hide();
        }
    };

    return {
        Init: function () {

            if ($('.main-menu-container').length === 0)
                return false;

            $(window).on('click', function (e) {

                var classes = e.target.className.split(' ');

                var found = false; var i = 0;
                while (i < classes.length && !found) {
                    if (classes[i] == 'sub-menu' || classes[i] == 'has-sub-menu' || classes[i] == 'main-menu-arrow') found = true;
                    else ++i;
                }

                if (!found) {
                    p.ResetSelection();
                }
            });

            /*$(window).on('click', function (e) {
                if (!e.target.matches('.sub-menu a, .has-sub-menu, .main-menu-arrow')) {
                    p.ResetSelection();
                }
            });*/

            $(window).on('escapekeyup', function () {
                p.ResetSelection();
            });

            $('.main-menu-container a').on('keydown', function (e) {//prevent page from scrolling
                if (e.keyCode === p.Key.UP || e.keyCode === p.Key.DOWN)
                    e.preventDefault();
            });

            $('.has-sub-menu').on('click', function (e) {
                //e.preventDefault();
                
                var isOpen = $(this).parent('li').hasClass('selected-item');
                var isSubMenu = $(this).parents(".sub-menu").length;

                if (!isSubMenu) {
                    p.ResetSelection();
                }

                if (!isOpen) {
                    $(this).attr('aria-expanded', 'true');


                    $(this).find('.main-menu-arrow svg use').attr('xlink:href', '#base-arrow-up');
                    $(this).find('.main-menu-arrow-down').removeClass('main-menu-arrow-down').addClass('main-menu-arrow-up');


                    $(this).parent('li').addClass('selected-item');
                    //$(this).next('.sub-menu').show();
                    $(this).next('.sub-menu').fadeIn(50);
                } else {
                    if (isSubMenu) {
                        $(this).find('.main-menu-arrow-up').removeClass('main-menu-arrow-up').addClass('main-menu-arrow-down');
                        $(this).parent('li').removeClass('selected-item');
                        $(this).next('.sub-menu').hide();

                        $(this).attr('aria-expanded', 'false');
                        //$(this).find('.main-menu-arrow svg use').attr('xlink:href', '#base-arrow-down');
                    }
                }
            });

            $('.main-menu > li > a').on('keyup', function (e) {
                if (e.keyCode === p.Key.LEFT) {
                    p.MoveLeft($(this).closest('li').prev('li'));
                }
                else if (e.keyCode === p.Key.RIGHT) {
                    p.MoveRight($(this).closest('li').next('li'));
                }
                else if (e.keyCode === p.Key.UP) {
                    if ($(this).hasClass('has-sub-menu'))
                        $(this).next('.sub-menu').find('a:last').focus();
                }
                else if (e.keyCode === p.Key.DOWN) {
                    if ($(this).hasClass('has-sub-menu'))
                        $(this).next('.sub-menu').find('a:first').focus();
                }
            });

            $('.main-menu .sub-menu a').on('keyup', function (e) {
                if (e.keyCode === p.Key.UP) {
                    if ($(this).parent().parent().prev().hasClass("has-sub-menu") && $(this).parent().index() === 0) {
                        $(this).parent().parent().prev().focus();
                    } else if ($(this).parent().prev().hasClass("selected-item")) {
                        $(this).parent().prev().find(".sub-menu").find("li:last > a").focus();
                    } else {
                        var $prevItem = $(this).closest("li").prev("li");
                        $prevItem.length === 0 ? $(this).closest(".sub-menu").find("li:last > a").focus() : $prevItem.children("a").focus();
                    }
                }
                else if (e.keyCode === p.Key.DOWN) {
                    if ($(this).parent().parent().prev().hasClass("has-sub-menu") && $(this).parent("li").is(":last-child") && $(this).parents("li.selected-item").first().next().length && $(this).parents("li.selected-item").attr("class") === "selected-item") {
                        $(this).parents("li.selected-item").first().next("li").find("a").focus();
                    }
                    else {
                        if ($(this).hasClass("has-sub-menu") && $(this).attr("aria-expanded") === "true") {
                            $(this).next(".sub-menu").find("li:first > a").focus();
                        } else {
                            var $nextItem = $(this).closest('li').next('li');
                            $nextItem.length === 0 ? $(this).closest('.sub-menu').find('li:first > a').focus() : $nextItem.children('a').focus();
                        }
                    }
                }
                else if (e.keyCode === p.Key.RIGHT) {
                    //p.MoveRight($(this).closest('.main-menu-item').next('li'));
                    p.MoveRight($(this).closest("ul.main-menu > li").next("li"));
                }
                else if (e.keyCode === p.Key.LEFT) {
                    //p.MoveLeft($(this).closest('.main-menu-item').prev('li'));
                    p.MoveLeft($(this).closest("ul.main-menu > li").prev("li"));
                }
            });
        }
    };
}();