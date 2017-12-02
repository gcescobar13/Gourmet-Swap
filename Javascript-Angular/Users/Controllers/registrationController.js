(function () {

    angular.module(APP.NAME)
        .controller('RegisterController', RegisterController);

    RegisterController.$inject = ['$log', '$window', '$stateParams', '$state', 'userService', 'toastr'];

    function RegisterController($log, $window, $stateParams, $state, userService, toastr) {
        var vm = this;
        vm.out = {
            $stateParams,
            stateCurrent: $state.current
            , stateParams: $state.params
        };
        vm.registerContent = {};
        vm.registrationForm;
        vm.onRegisterBtnClicked = _onRegisterBtnClicked;
        vm.onRegisterCookBtnClicked = _onRegisterCookBtnClicked;

        vm.hasValidationError = _hasValidationError;
        vm.showValidationErrors = _showValidationErrors;
        vm.showErrorMessage = _showErrorMessage;
        vm.$onInit = _onInit;
        vm.preRegister = _preRegister;

        vm.emailFormat = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
        vm.passwordFormat = /^(?=.*[0-9]).{6,}$/;
        vm.numberFormat = /^\D?(\d{3})\D?\D?(\d{3})\D?(\d{4})$/;
        vm.zipcodeFormat = /^\d{5}$|^\d{5}-\d{4}$/;


        function _onInit() {
            vm.role = $state.params.registerType;
            if (vm.role == "user") {
                vm.registerHeader = "Let's start with the usual!";
            } else if (vm.role == "chef") {
                vm.registerHeader = "Let's gets cooking!";
            } else {
                vm.registerHeader = "Wait? Who are you?";
            }
        };

        function _preRegister() {
            $log.log("In");
            if (vm.role == "user") {
                _onRegisterBtnClicked();
            } else if (vm.role == "chef") {
                _onRegisterCookBtnClicked();
            }
        }

        function _onRegisterBtnClicked() {

            if (vm.registrationForm.$invalid) {
                return;
            } else {
                vm.registerContent.role = 'user';
                userService.addUser(vm.registerContent, _onRegisterSuccess, _onRegisterError);
            }

        }

        //add
        function _onRegisterSuccess(response) {
            $state.go('pub.registerThanks', { status: 'good' });
        };

        function _onRegisterError(response) {
            if (response && response.errors) {
                toastr.error(response.errors);
            } else {
                $state.go('pub.registerThanks', { status: 'bad' });
            }

        };

        function _onRegisterCookBtnClicked() {

            if (vm.registrationForm.$invalid) {
                return;
            } else {
                vm.registerContent.role = 'chef';
                userService.addUser(vm.registerContent, _onRegisterCookSuccess, _onRegisterCookError);
            }

        }

        //add
        function _onRegisterCookSuccess(response) {
            $state.go('pub.registerThanks', { status: 'good' });

        };

        function _onRegisterCookError(response) {
            if (response && response.error) { 
                toastr.error(response.errors);
            } else {
                $state.go('pub.registerThanks', { status: 'bad' });
            }

        };

        //update
        function _onUpdateSuccess(response) {
            $log.log(response);
            $log.log("hi");

        };

        function _onUpdateError(response) {
            $log.log(response);
            if (response && response.error) {
                $log.log("Update Failed");
            } else {
                $log.log("Unable to connect with server. Update Failed.");
            }

        };


        function _hasValidationError(propertyName) {
            return (vm.registrationForm.$submitted || (vm.registrationForm[propertyName].$dirty && vm.registrationForm[propertyName].$touched))
                && vm.registrationForm[propertyName].$invalid;
        }

        function _showValidationErrors(propertyName) {
            return (vm.registrationForm.$submitted || (vm.registrationForm[propertyName].$dirty && vm.registrationForm[propertyName].$touched));
        };

        function _showErrorMessage(propertyName, ruleName) {
            return vm.registrationForm[propertyName].$error[ruleName];
        }

        function getUrlParameter(name) {
            name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
            var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
            var results = regex.exec(location.search);
            return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
        };

    }


})();