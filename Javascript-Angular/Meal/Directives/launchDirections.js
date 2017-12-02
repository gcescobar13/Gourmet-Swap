(function () {

    angular.module(APP.NAME)
        .directive('getDirections', getDirections)

    getDirections.$inject = ['$http'];

    function getDirections($http) {

        var directive = {
            restrict: 'A',
            scope: {
                btnHandler: '&',

            },
            link: _linkFunc
        };

        return directive;

        function _linkFunc(scope, element, attrs) {
            element.on("click", ".directionBtns", _triggerFunc);
            function _triggerFunc(evt) {
                var btnEl = evt.target;
                var lat = btnEl.attributes['data-meal-lat'].value;
                var lng = btnEl.attributes['data-meal-lng'].value;
                scope.btnHandler({ lat: lat, lng: lng });
            }
             
        };



    }

})();