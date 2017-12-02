(function () {

    angular.module(APP.NAME)
        .directive('loading', Loader) 

    Loader.$inject = ['$http'];

    function Loader($http) {

        var directive = {
            restrict: 'A',
            scope: {
                executeTimeoutFunc: '&'
            },
            link: _linkFunc
        };

        return directive;

        function _linkFunc(scope, element, attrs) {
            if (scope.$parent.$last) {
                scope.executeTimeoutFunc();
            }
        };

    }

})();