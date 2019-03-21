
(function () {
    let imageInputElement = document.querySelector(".p-create_imageInput");
    let imageContentElement = document.querySelector(".p-create_imageContent");

    imageInputElement.addEventListener("change", function (e) {
        if (e.target.value !== "") {
            fileName = e.target.value.split('\\').pop();
            imageContentElement.innerHTML = fileName;
        }
        else {
            imageContentElement.innerHTML = "No Image Selected";
        }
    });
})();
