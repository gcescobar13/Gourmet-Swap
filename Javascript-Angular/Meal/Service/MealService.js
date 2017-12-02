(function () {

    angular.module(APP.NAME).factory("mealService", mealService);
    mealService.$inject = ["$http"];

    function mealService($http) {

        return {            
            mealLocation: _mealLocation,
            getMealPositions: _getMealPositions,
            getMealPositionsV2: _getMealPositionsV2,
            getMealLocationId: _getMealLocationId,
            getComments: _getComments,
            getMealPhotos: _getMealPhotos
        }


        //------------------------------------------------GET--------------------------------------------//

        
        function _getMealLocationId(id, onSuccess, onError) {
            var settings = {
                url: "/api/meals/" + id + "/locations",
                method: "GET",
                cache: false,
                withCredentials: true,
            };
            return $http(settings).then(onSuccess, onError);
        }

        function _getMealPositions(lat, lng, maxDistance, onSuccess, onError) {
            var settings = {
                url: '/api/meals/positions/' + lat + '/' + lng + '/' + maxDistance,
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings).then(onSuccess, onError);
        }


        function _getMealPhotos(mealIds, onSuccess, onError) {
            var settings = {
                url: '/api/meals/photos',
                method: 'POST',
                cache: false,
                content: "application/json; charset=UTF-8",
                data: JSON.stringify(mealIds),
                withCredentials: true
            };
            return $http(settings).then(onSuccess, onError);
        }

        function _mealLocation(locationInfo, onSuccess, onError) {
            var settings = {
                url: "/api/meals/locations",
                method: "POST",
                cache: false,
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(locationInfo),
                withCredentials: true

            };
            return $http(settings).then(onSuccess, onError);
        }

     
        function _getComments(ownerId, onSuccess, onError) {
            var settings = {
                url: '/api/comments/threads/' + ownerId + '/1',
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings).then(onSuccess, onError);
        }


        function _updateMeal(mealInfo, onSuccess, onError) {
            var settings = {
                url: "/api/meals/" + mealInfo.id,
                method: "PUT",
                cache: false,
                content: "application/json; charset=UTF-8",
                data: JSON.stringify(mealInfo),
                withCredentials: true
            };
            return $http(settings).then(onSuccess, onError);
        }
       

        function _getMealPhotos2(mealId, onSuccess, onError) {
            var settings = {
                url: '/api/files/meal/' + mealId,
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

      

    }







})();