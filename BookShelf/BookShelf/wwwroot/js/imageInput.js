(function () {
    let imageContentElement = document.querySelector(".p-addEditBookForm_imageContent");
    let imageInputElement = document.querySelector(".p-addEditBookForm_imageInput");

    imageInputElement.addEventListener("change", function (e) {

        fileName = e.target.value.split('\\').pop();

        imageContentElement.innerHTML = fileName;
    });

    $.validator.addMethod("fileSize", function (value, element) {
        let twoMB = 2097152;
        return this.optional(element) || element.files[0].size <= twoMB;
    }, "file size must be less than 2mb");

    $.validator.addMethod("fileExtension", function (value, element) {
        return this.optional(element) || value.match(new RegExp(/\.(png|jpeg|jpg)$/i));
    }, "The image must have a file extension of png, jpeg, or jpg");
    
})();

