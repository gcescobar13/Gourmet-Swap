(function () {
    //use templateProvider and inject app.config with userService and use userService.isLoggedIN (or .whosloggedIn) to determine if person is login
    // or 2. inject NavController with userService and use .isLogginIN to change the templateUrl of nav through it's contoller
    //or 3. the new state and it's controller that renders when logged in will block nav-public.html from including and it'll include it's own nav html instead.
   
    angular.module(APP.NAME)
        .factory('userService', userService);

    userService.$inject = ['$log', '$http'];

    function userService($log, $http) {
        
        var apiRoute = "/api/users/"
        var svc = {};
        svc.addUser = _addUser;
        svc.updateRegistration = _updateRegistration;
        svc.login = _login;
        svc.logout = _logout;
        svc.currentUser = _currentUser;
        svc.cookMessages = _cookMessages;
        svc.regConfirmation = _regConfirmation;
        svc.getCookStats = _getCookStats;
        svc.setCookLikes = _setCookLikes;
        svc.setCookFollows = _setCookFollows;
        svc.getProfileServiceLikes = _getProfileServiceLikes;
        svc.getProfileServiceFollows = _getProfileServiceFollows;
        svc.getCooksBySearch = _getCooksBySearch;
 
        return svc;

        function _addUser(data, onSuccess, onError) {
            var settings = {
                url: apiRoute + "register/" + data.role, 
                method: 'POST',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                withCredentials: true,
                data: JSON.stringify(data)
            };
            return $http(settings)
                .then(onSuccess, onError);
        }



        function _updateRegistration(data, onSuccess, onError) {
            var settings = {
                url: apiRoute + "register/" + data.id,
                method: 'PUT',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                withCredentials: true,
                data: JSON.stringify(data)
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _login(data, onSuccess, onError) {
            var settings = {
                url: apiRoute + "login",
                method: 'POST',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                withCredentials: true,
                data: JSON.stringify(data)
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _logout(onSuccess, onError) {
            var settings = {
                url: apiRoute + "logout",
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _currentUser(onSuccess, onError) {
            var settings = {
                url: apiRoute + "current",
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _cookMessages(onSuccess, onError) {
            var settings = {
                url: apiRoute + "inbox",
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _regConfirmation(data, onSuccess, onError) {
            var settings = {
                url: apiRoute + "confirmation/" + data,
                method: 'POST',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                withCredentials: true              
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _getCookStats(data, onSuccess, onError) {
            var settings = {
                url: apiRoute + data + "/stats",
                method: 'GET',     
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _setCookLikes(data, onSuccess, onError) {
            var settings = {
                url: apiRoute + data + "/likes",
                method: 'POST',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError)

        }

        function _setCookFollows(data, onSuccess, onError) {
            var settings = {
                url: apiRoute + data + "/follows",
                method: 'POST',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError)

        }

        function _getProfileServiceLikes(onSuccess, onError) {
            var settings = {
                url: "/api/cooks/liking",
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _getProfileServiceFollows(onSuccess, onError) {
            var settings = {
                url: "/api/cooks/following",
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _getCooksBySearch(data, onSuccess, onError) {
            var settings = {
                url: "/api/cooks/searching/" + data,
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

    }

})();