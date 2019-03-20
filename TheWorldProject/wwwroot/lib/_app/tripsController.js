//import { Object } from "core-js/library/web/timers";



(function () {

    "use strict";

    // Отримуємо вже існуючий модуль
    angular.module("app-trips") // .module поверне нам обєкт звідки ми зможемо отримати контролер (створити його)
        .controller("tripsController", tripsController); // tripsController - назвемо наш контролер, 2 парам: tripsController - імя функції

    // ця функція є тілом нашого конструктора, зверху ми вказали імя нашого констркутора і функцію, що буде містити його функціонал
    function tripsController($http) {

        // this - це обєкт, що повертає tripsController
        var vm = this;

        // додамо нову властивість, УВАГА: тут JS =)) 
        //vm.Name = "Edward";

        // створимо масив з обєктами
        vm.trips = [];

        vm.newTrip = {}; // для того, щоб заповнювати новими обєктами подорожі, то зробимо його пустим
        vm.errorMessage = "";
        vm.isBusy = true;

        // get -  повертає нам promise (промис - згадай JS), .then - приймає 2 колбека, перший - це якщо успіх, другий, якщо невдача - то виконується
        $http.get("/api/trips") // тобто викличемо метод get контролера trips, який знаходиться за шляхом /api/trips - і отримаємо через цей метод інфу з сервера
            .then(function (response) { // response - відповідь з сервера
                // Success
                // response.data - містить серіалізований з JSON в обєкт дані з сервера
                angular.copy(response.data, vm.trips); // зберегти в vm.trips наш обєкт подорожі
                //vm.isBusy = false; // put it in FINALLY block !!!
            }, function (error) {
                // Failure
                vm.errorMessage = "Failed to load data: " + error;
                //vm.isBusy = false; // put it in FINALLY block !!!

            })
            .finally(function () {
                vm.isBusy = false;
            });

        vm.AddTrip = function () {
            //vm.trips.push({
            //    name: vm.newTrip.name,
            //    created: new Date()
            //});

            //vm.newTrip = {}; // щоб наша форма очистилась

            vm.isBusy = true; // поставимо в процес завантаження
            vm.errorMessage = ""; // очистимо повідомлення помилки  

            $http.post("/api/trips", vm.newTrip) // так як post то воно очікує прийняття обєкта, що відображатиме тіло нашого Trip, і ми передаємо модель vm.newTrip 
                .then(function (response) {
                    // Success
                    vm.trips.push(response.data); // щоб відобразити в таблиці наш обєкт
                    vm.newTrip = {};
                },
                function () {
                    // Failure
                    vm.errorMessage = "Failed to save new trip";
                })
                .finally(function () {
                    vm.isBusy = false;
                });

        }

    }

})();