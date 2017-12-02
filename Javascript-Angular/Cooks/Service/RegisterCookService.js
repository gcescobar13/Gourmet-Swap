(function () {
    
    angular.module(APP.NAME).factory("registerCookService", RegisterCookService);
    RegisterCookService.$inject = ["$http", "$q"];

    function RegisterCookService($http, $q) {

        return {
            saveStepTwo: _saveStepTwo,         
            currentUser: _currentUser,
            saveStepThree: _saveStepThree,
            saveStepFour: _saveStepFour,
            putCurrentUser: _putCurrentUser,
            getCuisineOptions: _getCuisineOptions,
            getServicesProvided: _getServicesProvided,
            populateStepTwo: _populateStepTwo,
            getPreviousServicesProvided: _GetPreviousServicesProvided,
            getPreviousCuisinesProvided: _GetPreviousCuisinesProvided,
            populateStepThree: _populateStepthree,
            populateStepFour: _populateStepfour,
            completeStripeRegistration: _completeStripeRegistration,
            storeStripeAccount: _storeStripeAccount,
            userLite: _userById,

        };

        function _userById(onSuccess, onError) {
            var settings = {
                url: '/api/cooks/cuisineoptions',
                method: 'GET',
                cache: false,
                // contentType: 'application/json; charset=UTF-8',
                // data: JSON.stringify(cookInfo),
                withCredentials: true

            };
            return $http(settings)
                .then(onSuccess, onError);
        }
        
        function _completeStripeRegistration(authInfo, onSuccess, onError) {
            var settings = {
                url: '/api/stripeRegistration',
                method: 'POST',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(authInfo),
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _storeStripeAccount(account, onSuccess, onError) {
            var settings = {
                url: '/api/storeStripeAccount',
                method: 'POST',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(account),
                withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _getCuisineOptions(onSuccess, onError) {
            var settings = {
                url: '/api/cooks/cuisineoptions'
               , method: 'GET'
               , cache: false
               , withCredentials: true

            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _getServicesProvided(onSuccess, onError) {
            var settings = {
                url: '/api/cooks/servicesProvided'
               , method: 'GET'
               , cache: false
               , withCredentials: true

            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _currentUser(onSuccess, onError) {
            var settings = {
                url: '/api/users/current/full'
               , method: 'GET'
               , cache: false
               , withCredentials: true

            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _populateStepTwo(onSuccess, onError) {
            var settings = {
                url: '/api/cooks/stepTwo/full'
                , method: 'GET'
                , cache: false
                , withCredentials: true

            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _GetPreviousServicesProvided(userId, onSuccess, onError) {
            var settings = {
                url: '/api/cooks/' + userId + '/profile/services'
                , method: 'GET'
                , cache: false
                , withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _GetPreviousCuisinesProvided(userId, onSuccess, onError) {
            var settings = {
                url: '/api/cooks/' + userId + '/profile/cuisines'
                , method: 'GET'
                , cache: false
                , withCredentials: true
            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _populateStepthree(onSuccess, onError) {
            var settings = {
                url: '/api/cooks/stepthree'
                , method: 'GET'
                , cache: false
                , Withcredentials: true
            };
            return $http(settings).then(onSuccess, onError);
        }


        function _populateStepfour(onSuccess, onError) {
            var settings = {
                url: '/api/cooks/stepFour'
                , method: 'GET'
                , cache: false
                , Withcredentials: true
            };
            return $http(settings).then(onSuccess, onError);
        }

//----------------------------------------------- put -----------------------------------------------//

        function _saveStepTwo(cookInfo, onSuccess, onError) {
            var settings = {
                url: '/api/cooks/cooking'
               , method: 'PUT'
               , cache: false
               , contentType: 'application/json; charset=UTF-8'
               , data: JSON.stringify(cookInfo)
               ,  withCredentials: true

            };
            return $http(settings)
                .then(onSuccess, onError);
        }
        
        function _putCurrentUser(userInfo, onSuccess, onError) {
            var settings = {
                url: '/api/cooks/account',
                method: 'PUT',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(userInfo),
                withCredentials: true
            };
            return $http(settings).then(onSuccess, onError);
        }

        function _saveStepThree(cookInfo, onSuccess, onError) {
            var settings = {
                url: '/api/cooks/community',
                method: 'PUT',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(cookInfo),
                withCredentials: true,

            };
            return $http(settings)
                .then(onSuccess, onError);
        }

        function _saveStepFour(cookInfo, onSuccess, onError) {
            var settings = {
                url: '/api/cooks/stepfour',
                method: 'PUT',
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(cookInfo),
                withCredentials: true,

            };
            return $http(settings)
                .then(onSuccess, onError);
        }

      
        
    }

})();