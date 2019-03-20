(function () {
    "use strict";
    // Створюємо НОВИЙ модуль
    angular.module("app-trips", ['simpleControls', 'ngRoute']) // тут ми визначемо пакет коду для Ангуляр, назвемо для узгодження як app-trips, // а масив це список залежностей, які будуть
    .config(function ($routeProvider) {
            // "/" - означає шлях, який ми шукаємо, так як "/", то це наш рут. Другий параметр - це обєкт, який показує, як виконувати цю маршутизацію
            $routeProvider.when("/", {
                controller: "tripsController", // вкажемо контролер
                controllerAs: "vm", // тут даємо кличку (коротку назву для контролера) як писали в розмітці tripsController as vm 
                templateUrl: "/views/tripsView.html" // шлях, який відображає поточний УРЛ для конкретного Виду - tripsView - створимо його

            }); 

            // Added test comment for the app-trip.js (Test GitHub)
            // створимо ще один маршрут для редагування подорожі, тому почнемо з шляху editor
            $routeProvider.when("/editor/:tripName", {
                controller: "tripEditorController",
                controllerAs: "vm",
                templateUrl: "/views/tripEditorView.html"
            });




            $routeProvider.otherwise({ redirectTo: "/" }) // якщо жоден з шляхів не співпав, то просто відправимо на корінь


        })                            


})();