(function () {
    "use strict";

    angular.module("simpleControls", []) // так як другий параметр [], то ми створюємо директиву 
        .directive("waitCursor", waitCursor); 
    // directive - як спосіб декорації нашого коду, але також можна використати для завершення елементів
    function waitCursor() {
        return {
            scope: {
                show: "=displayWhen" // може називатись  і по-інакшому
            },
            restrict: "E",
            templateUrl: "/views/waitCursor.html" // тобто це в wwwroot тому потрібно створити views/waitCursor.html
        };
    }


})();