(function () {
    let imageContentElement = document.querySelector(".p-edit_imageContent");
    let imageInputElement = document.querySelector(".p-edit_imageInput");
    let imageContainerElement = document.querySelector(".p-edit_imageContainer");
    let imageNoteElement = document.querySelector(".p-edit_note");
    let changeImageInputElement = document.querySelector(".p-edit_changeImageInput");

    imageInputElement.addEventListener("change", function (e) {
        if (e.target.value !== "") {
            fileName = e.target.value.split('\\').pop();
            imageContentElement.innerHTML = fileName;
        }
        else {
            imageContentElement.innerHTML = "No Image Selected";
        }
    });

    changeImageInputElement.addEventListener("change", function () {
        if (changeImageInputElement.checked) {
            imageContainerElement.style.display = "block";
            imageNoteElement.style.display = "block";
        }
        else {
            imageContainerElement.style.display = "none";
            imageNoteElement.style.display = "none";
        }
    });

})();