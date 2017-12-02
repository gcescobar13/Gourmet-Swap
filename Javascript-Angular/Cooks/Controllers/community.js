(function () {

    angular
        .module(APP.NAME).controller("RegisterCookCommunityController", RegisterCookCommunityController);

    RegisterCookCommunityController.$inject = ["$state", "$log", "registerCookService", "toastr"];


    function RegisterCookCommunityController($state, $log, registerCookService, toastr) {

        vm = this;

        vm.communityContent;
        vm.onSaveClicked = _onSaveClicked;
        vm.communityForm;
        vm.hasValidationError = _hasValidationError;
        vm.showValidationErrors = _showValidationErrors;
        vm.showErrorMessage = _showErrorMessage;
        vm.$onInit = _onInit;

        function _onInit() {
            registerCookService.populateStepThree(populateStepThreeSuccess, populateStepThreeError);
        }

        function _onSaveClicked() {
            if (vm.communityForm.$invalid) {
                toastr.warning('Some information is not entered correctly, please review the validation messages and try again.',
                    'Submission failed.');
                return;
            } else {
                registerCookService.saveStepThree(vm.communityContent, _onSaveSuccess, _onSaveError);
            }          
        }
        
        function _onSaveSuccess(response) {
            $log.log(response);
            toastr.success("Successfully saved.");
            $state.go("cooksProfile.register.extra");
        }

        function _onSaveError(response) {
            $log.log(response);
            if (response && response.error) {
                toastr.error(response.error, "Failed to save.");
            } else {
                toastr.error("Unable to connect with server", "Failed to save.")
            }
        }

        function populateStepThreeSuccess(response) {
            $log.log(response);
             vm.communityContent = response.data.item;
        }

        function populateStepThreeError(response) {
            toastr.info("Please fill out the following fields...", "Hello");
        }
        
//----------------------------------------------- validation ---------------------------------//
        function _hasValidationError(propertyName) {
            return (vm.communityForm.$submitted || (vm.communityForm[propertyName].$dirty && vm.communityForm[propertyName].$touched))
                && vm.communityForm[propertyName].$invalid;
        }

        function _showValidationErrors(propertyName) {
            return (vm.communityForm.$submitted || (vm.communityForm[propertyName].$dirty && vm.communityForm[propertyName].$touched));
        };

        function _showErrorMessage(propertyName, ruleName) {
            return vm.communityForm[propertyName].$error[ruleName];
        }



    }





})();