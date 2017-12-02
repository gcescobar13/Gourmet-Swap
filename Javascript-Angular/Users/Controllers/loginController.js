(function () {

    angular.module(APP.NAME)
        .controller('LoginController', LoginController);

    LoginController.$inject = ['$log', '$window', '$stateParams', '$state', 'userService'];

    function LoginController($log, $window, $stateParams, $state, userService) {
        var vm = this;

        vm.loginContent = {};
        vm.loginForm;
        vm.onLoginClicked = _onLoginClicked;

        vm.hasValidationError = _hasValidationError;
        vm.showValidationErrors = _showValidationErrors;
        vm.showErrorMessage = _showErrorMessage;
        vm.registerNewSwapper = _registerNewSwapper;
        vm.registerNewCook = _registerNewCook;
     


        

            //userService.currentUser(_onIsLoggedInSuccess, _onIsLoggedInError)

        

        function _onLoginClicked() {
            
            if (vm.loginForm.$valid) {
                userService.login(vm.loginContent, _onLoginSuccess, _onLoginError);
            } else {
                $log.log("error");
            }
            
        }

        //add
        function _onLoginSuccess(response) {
            $log.log(response);
            $log.log("hi");
            vm.exists = true;
            userService.currentUser(_onIsLoggedInSuccess, _onIsLoggedInError)
        };

        function _onLoginError(response) {
            $log.log(response);
            if (response && response.error) {
                $log.log("Login Failed");
            } else {
                $log.log("Unable to connect with server. Login Failed.");
            }
        };

        function _registerNewSwapper() {
            $state.go('pub.register');
        }

        function _registerNewCook() {
            $state.go('pub.registerCook');
        }

        function _onIsLoggedInSuccess(response) {

            vm.userName = response.data.name;
            

            //var userId = { userId: response.data.id }
            //$state.go('cooksProfile.profile.Menu', userId, { reload: true });
            $state.go('pub', { role: null }, { reload: true });
        }

        function _onIsLoggedInError(response) {
            $log.log("not logged in");
        }

        function _hasValidationError(propertyName) {
            return (vm.loginForm.$submitted || (vm.loginForm[propertyName].$dirty && vm.loginForm[propertyName].$touched))
                && vm.loginForm[propertyName].$invalid;
        }

        function _showValidationErrors(propertyName) {
            return (vm.loginForm.$submitted || (vm.loginForm[propertyName].$dirty && vm.loginForm[propertyName].$touched));
        };

        function _showErrorMessage(propertyName, ruleName) {
            return vm.loginForm[propertyName].$error[ruleName];
        }

    }
})();