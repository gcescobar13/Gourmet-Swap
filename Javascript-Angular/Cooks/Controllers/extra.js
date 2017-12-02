(function () {

    angular
        .module(APP.NAME).controller("RegisterCookExtraController", RegisterCookExtraController);

    RegisterCookExtraController.$inject = ["$state", "$log", "registerCookService", "toastr"];


    function RegisterCookExtraController($state, $log, registerCookService, toastr) {

        vm = this;

        vm.$onInit = _onInit;
        vm.extraContent = {};
        vm.onSaveClicked = _onSaveClicked;
        vm.extraForm;
        vm.hasValidationError = _hasValidationError;
        vm.showValidationErrors = _showValidationErrors;
        vm.showErrorMessage = _showErrorMessage;
        vm.getSource = _getSource;


        var source;


        function _onInit() {

            registerCookService.populateStepFour(populateStepFourSuccess, populateStepFourError);
           
        }


        function _onSaveClicked() {
            if (vm.extraForm.$invalid) {
                toastr.warning('Some information is not entered correctly, please review the validation messages and try again.',
                    'Submission failed.');
                return;
            } else {
                registerCookService.saveStepFour(vm.extraContent, _onSaveSuccess, _onSaveError);
            }
        }


        function _onSaveSuccess(response) {

            $log.log(response);
            toastr.success("Successfully saved.");
            $state.go("cooksProfile.register.payment");

        }


        function _onSaveError(response) {
            $log.log(response);
            if (response && response.error) {
                toastr.error(response.error, "Failed to save.");
            } else {
                toastr.error("Unable to connect with server", "Failed to save.")
            }
        }

        function populateStepFourSuccess(response){
            $log.log(response);
            vm.extraContent = response.data.item;
            vm.input1 = response.data.item.source;
            //toastr.info();
        }

        function populateStepFourError(response) {
          toastr.warning();
      }


//------------------------------------------------- validation -------------------------------------------------------------//


        function _hasValidationError(propertyName) {
            return (vm.extraForm.$submitted || (vm.extraForm[propertyName].$dirty && vm.extraForm[propertyName].$touched))
                && vm.extraForm[propertyName].$invalid;
        }

        function _showValidationErrors(propertyName) {
            return (vm.extraForm.$submitted || (vm.extraForm[propertyName].$dirty && vm.extraForm[propertyName].$touched));
        };

        function _showErrorMessage(propertyName, ruleName) {
            return vm.extraForm[propertyName].$error[ruleName];
        }

        function _getSource(sourceValue, isChecked) {
            if (isChecked) {
                vm.extraContent.source = sourceValue;
            }
           
            return isChecked;
        }



    }





})();