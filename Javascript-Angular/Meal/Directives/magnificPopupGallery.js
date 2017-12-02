(function () {
    angular
        .module(APP.NAME)
        .directive('magnificPopupGallery', MagnificPopupGallery);

    MagnificPopupGallery.$inject = [];

    function MagnificPopupGallery() {
        var directive = {
            restrict: 'A',
            link: _linkFunc
        };

        return directive;

        function _linkFunc(scope, element, attrs) {
            $(element).magnificPopup({
                delegate: '.image',
                type: 'image',
                gallery: {
                    enabled: true
                }
            });

        };
    }

})();