(function () {
  
    angular.module(APP.NAME)
        .factory('recoveryService', recoveryService);

    recoveryService.$inject = ['$log', '$http'];

    function recoveryService($log, $http) {
        
   
        return {
            recoveryRequest: _recoveryRequest
            ,resetRequest: _resetRequest
        }
       
        function _recoveryRequest(data, onSuccess, onError) {
            var settings = {
                url: "/api/users/passwords/requests",
                method: 'POST',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                withCredentials: true,
                data: data
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _resetRequest(data, onSuccess, onError) {
            var settings = {
                url: "/api/users/passwords/recover/" + data.token,
                method: 'POST',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                withCredentials: true,
                data: data
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

    }

})();