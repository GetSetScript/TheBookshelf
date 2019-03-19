(function () {
    $.validator.addMethod("fileSize", function (value, element) {
        let twoMB = 2097152;
        return this.optional(element) || element.files[0].size <= twoMB;
    }, "file size must be less than 2mb");

    $.validator.addMethod("fileExtension", function (value, element) {
        return this.optional(element) || value.match(new RegExp(/\.(png|jpeg|jpg)$/i));
    }, "The file extension must be png, jpeg, or jpg");
    
})();

