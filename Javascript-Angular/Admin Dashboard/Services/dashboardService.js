(function () {
   
    angular.module(APP.NAME)
        .factory('dashboardService', dashboardService);

    dashboardService.$inject = ['$log', '$http'];

    function dashboardService($log, $http) {
        
        var apiRoute = "/api/dashboard/";
        var svc = {};

        svc.pagedUsers = _pagedUsers;
        
        return svc;

        function _pagedUsers(data, onSuccess, onError) {
            var settings = {
                url: apiRoute + "users",
                method: 'GET',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                withCredentials: true,
                params: data
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

    }

})();