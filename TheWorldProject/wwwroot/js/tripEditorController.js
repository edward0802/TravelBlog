(function () {
    "use strict";

    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);


    function tripEditorController($routeParams, $http) {
        var vm = this;

        // $routeParams - дозволяє нам додавати імя наш параметр маршутизації tripName до нашого обєкта vm
        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.errorMessage = "";
        vm.isBusy = true;
        vm.newStop = {};

        var url = "/api/trips/" + vm.tripName + "/stops"

        $http.get(url).then(function (response) {
            // Success
            angular.copy(response.data, vm.stops);
            _showMap(vm.stops);
        },
            function (error) {
                vm.errorMessage = "Failed to load stops"
            })
            .finally(function () {
                vm.isBusy = false;
            });


        
        vm.addStop = function () {
            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post(url, vm.newStop)
                .then(function (response) {
                    vm.stops.push(response.data);
                    _showMap(vm.stops);
                    vm.newStop = {}; // очистити форму, і щоб повторно випадково не додати те саме значення
                },
                function (error) {
                    vm.errorMessage = "Can't add new Stop...";
                })
                .finally(function () {
                    vm.isBusy = false;
                });
        } 


    }

    function _showMap(stops) {

        if (stops && stops.length > 0) {

            var mapStops = _.map(stops, function (item) {
                // item - відображає 1 обєкт з нашого масиву stops, тобто одну зупинку
                // тут ми отримаємо код, який поверне правильні структуру коду, яку очікує наша бібліотека travelMap
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info: item.name
                }
            });

            // Show Map
            travelMap.createMap({
                stops: mapStops, // ОСЬ ТУТ mapStops вставимо
                selector: "#map", // для того, щоб знайти місце на сторінці, куди вставити нашу карту, воно вставить в блок де #map
                currentStop: (mapStops.length - 1), // ГЛЮЧИЛО, БО ТУТ БУЛО зупинка під номером 1, а в нас в масиві лише 1 ЗУПИНКА, тобто номер - 0 !!!!
                initialZoom: 3 // розмір мапи 
            });
        }

    }





})();