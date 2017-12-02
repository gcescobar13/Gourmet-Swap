(function () {

    angular.module(APP.NAME)
        .controller("DashboardController", DashboardController);

    DashboardController.$inject = ["$state", "userAdminService", "toastr", "$scope", "$rootScope", "$log", "Roles", "eventService"];


    function DashboardController($state, userAdminService, toastr, $scope, $rootScope, $log, Roles, eventService) {

        var vm = this;
        vm.$onInit = _onInit;
        vm.itemsPerPageOptions = [16, 24, 32];
        vm.onChangeGetUsers = _onChangeGetUsers;
        vm.pagedItemsArray = null;
        vm.onSortClicked = _onSortClicked;
        vm.onRoleClicked = _onRoleClicked;
        vm.onToggle = _onToggle;
        vm.roleOptions = Roles;
        vm.paginationSettings = {
            pageSize: 24,
            sortTypeId: 1,
            pageNum: 1,
            roleId: 0
        };


        eventService.listen("onSearchRequested", _onEventListen);

        vm.sortingOptions = [
            { id: 1, sortType: "Latest" },
            { id: 2, sortType: "Recent" },
            { id: 3, sortType: "Alphabetical" }
        ];

        function _onChangeGetUsers() {

            vm.paginationSettings.pageIndex = vm.paginationSettings.pageNum - 1;

            userAdminService.pagedUsers(vm.paginationSettings, _onGetPagedUsersSuccess, _onGetPagedUsersError);
        }



        function _onGetPagedUsersSuccess(response) {
            vm.pagedList = response.data.item;

            toastr.success("Successful get.");
        }

        function _onGetPagedUsersError(response) {
            toastr.error("error.");
        }


        function _onInit() {
            _onChangeGetUsers();
            $log.log(Roles);
        }

        function _onSortClicked(sortId) {
            vm.paginationSettings.sortTypeId = sortId;
            userAdminService.pagedUsers(vm.paginationSettings, _onGetPagedUsersSuccess, _onGetPagedUsersError);
        }

        function _onRoleClicked(roleId) {
            vm.paginationSettings.roleId = roleId;
            userAdminService.pagedUsers(vm.paginationSettings, _onGetPagedUsersSuccess, _onGetPagedUsersError);
        }

        function _onEventListen(events, data) {
            var searchTerm = data;
            vm.paginationSettings.searchTerm = searchTerm;
            _onChangeGetUsers();
        }

        function _onToggle(userId) {
            
            $log.log("hi");
        }
    }

})();