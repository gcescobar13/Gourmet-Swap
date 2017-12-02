(function () {

    angular.module(APP.NAME)
        .directive('loadToggle', Loader)

    Loader.$inject = ['$http'];

    function Loader($http) {

        var directive = {
            restrict: 'A',
            link: _linkFunc
        };

        return directive;

        function _linkFunc(scope, element, attrs) {
            if (scope.$last) {
                $('.buttonSwitch').bootstrapToggle();
            }
        };



    }

})();