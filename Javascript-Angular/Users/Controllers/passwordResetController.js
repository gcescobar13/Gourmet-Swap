(function () {


    angular.module(APP.NAME)
        .controller('passwordResetController', passwordResetController);

    passwordResetController.$inject = ['$log', 'recoveryService','$stateParams','toastr'];

    function passwordResetController($log, recoveryService, $stateParams,toastr) {
        var vm = this;


        vm.onResetBtnClicked = _onResetBtnClicked;

        function _onResetBtnClicked() {
            vm.resetData.token = $stateParams.token;
            if (vm.resetForm.$valid) {
                $log.log(vm.resetContent);            
                recoveryService.resetRequest(vm.resetData, _resetSuccess, _resetError);
            } else {
                $log.log("error");
            }
        }

        function _resetSuccess(response) {
            $log.log(response);
            toastr.success('Password has been reset!');
        };

        function _resetError(response) {
            $log.log(response);
            if (response && response.error) {
                $log.log("Reset Failed");
            } else {
                $log.log("Unable to connect with server. Reset Failed.");
                toastr.error('Password has already been reset');
            }
        };
    }
})();