(function () {
    'use strict';
    //var elem = $(".username");
    //elem.text = "Edward Kucherniuk Viktorovych";

    //var main = $(".main");
    //main.on("mouseenter", function () {
    //    main.css("background-color", "#888");
    //});
    //main.on("mouseleave", function () {
    //    main.css("background-color", "");
    //});

    //var menuItems = $("ul.menu li a");



    var sidebarAndWrapper = $(".sidebar, .very-main"); // по суті, тут не треба .very-main бо я на флексах
    var icon = $(".sidebarToggle i.fa")
    $(".sidebarToggle").on("click", function () {
        sidebarAndWrapper.toggleClass("hide-sidebar");
        if (sidebarAndWrapper.hasClass("hide-sidebar")) {
            // ВАЖЛИВО: різниця між this та $(this) в тому, що ми отрмуємо в першому випадку HTML елемент, а в другому jQuery і його функціонал !!! 
            icon.removeClass("fa-chevron-left").addClass("fa-chevron-right");
        }
        else {
            icon.removeClass("fa-chevron-right").addClass("fa-chevron-left");
        }
    });
   




})();
