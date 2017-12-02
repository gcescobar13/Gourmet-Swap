(function () {
    angular
        .module(APP.NAME)
        .directive('onUploadSuccess', UploadSuccess);

    UploadSuccess.$inject = ['toastr'];

    function UploadSuccess(toastr) {
        var directive = {
            restrict: 'A',
            scope: {
                executeFuncOnSuccess: '&',
                mealId: '=mealId'
            },
            link: _linkFunc
        };

        return directive;

        function _linkFunc(scope, element, attrs) {
            $(element).fileinput({
                'uploadUrl': '/api/files/meal/' + scope.mealId,
                uploadAsync: false,
                preferIconicPreview: true,
                previewSettings: {
                    image: { width: "50px", height: "50px" }
                },
                showUploadedThumbs: false
                
            }).on('filebatchuploadsuccess', function (event, data, previewId, index) {
                toastr.success("Successfully upload images.");
                $(element).fileinput('clear');
                scope.executeFuncOnSuccess();
            });
        };
    }

})();