(function () {

    angular
        .module(APP.NAME).controller("CookMessagesController", CookMessagesController);

    CookMessagesController.$inject = ["$state", "$log", "registerCookService", "toastr", "userService", 'cookServices'];


    function CookMessagesController($state, $log, registerCookService, toastr, userService, cookServices) {

        var vm = this;
        vm.$onInit = _onInit;
        vm.messages;
        vm.submitReply = _submitReply;
        vm.replyBtnClick = _replyBtnClick;





        function _onInit() {
            userService.cookMessages(_onGetMessagesSuccess, _onGetMessagesError);
            vm.hideForm = true;
            vm.hideList = false;

        }

        function _submitReply() {
            vm.pvtReplyData;
            vm.pvtReplyData.parentId = vm.replyInfo.id
            vm.pvtReplyData.ownerTypeId = vm.replyInfo.ownerTypeId
            vm.pvtReplyData.ownerId = vm.replyInfo.ownerId
            debugger
            cookServices.submitReply(vm.pvtReplyData, _sendReplySuccess, _sendReplyError)
        }

        function _replyBtnClick(currentMessage) {
            vm.replyInfo = currentMessage;

            debugger
            vm.hideForm = false;
            vm.hideList = true;




        }

        function _sendReplySuccess(response) {
            toastr.success('Reply Sent!')
        }

        function _sendReplyError(response) {
            toastr.error('Failed to send!')
        }

        function _onGetMessagesSuccess(response) {
            $log.log(response.data.items);
            vm.messages = response.data.items;
            toastr.success("Successfully loaded messages.");
        }

        function _onGetMessagesError(response) {
            $log.log(response);
            if (response && response.error) {
                toastr.error(response.error, "Failed to load messages.");
            } else {
                toastr.error("Unable to connect with server", "Failed to load messages.")
            }
        }





    }

})();