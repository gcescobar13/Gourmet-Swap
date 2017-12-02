(function () {
    angular
        .module(APP.NAME)
        .directive('onToggle', OnToggle);

    OnToggle.$inject = ['userAdminService', 'toastr'];

    function OnToggle(userAdminService, toastr) {
        var status;

        var directive = {
            restrict: 'A',
            scope: {
                userId: '=userId',
                isDisabled: '=isDisabled'
            },
            link: _linkFunc
        };

        return directive;

        
        function _linkFunc(scope, element, attrs) {

            $(element).prop("checked", !scope.isDisabled);

            
            $(element).change(function () {

                if ($(element).prop("checked")) {
                    status = false;
                } else {
                    status = true;
                }
                var data = { id: scope.userId, disabled: status };
                userAdminService.changeUserDisableStatus(data, _onChangeStatusSuccess, _onChangeStatusError);

            });
        };

        function _onChangeStatusSuccess(response) {
            
            if (!status) {
                toastr.success("User enabled.");
            } else {
                toastr.success("User disabled.");
            }
        }

        function _onChangeStatusError(response) {
            toastr.error("Server error.")
        }
    }

})();