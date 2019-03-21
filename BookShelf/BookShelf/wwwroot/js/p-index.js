(function () {
    let actionContainerToggleElement = document.querySelectorAll(".c-bookCard_actionContainerToggle");
  
    for (let i = 0; i < actionContainerToggleElement.length; i++) {
        actionContainerToggleElement[i].addEventListener("click", function () {
            let actionContainerElement = actionContainerToggleElement[i].previousElementSibling;

            if (actionContainerElement.style.display === "inline-block") {
                actionContainerElement.style.display = "none";
                actionContainerToggleElement[i].classList.remove("-cancel");
            }
            else {
                actionContainerElement.style.display = "inline-block";
                actionContainerToggleElement[i].classList.add("-cancel");
            }
        });
    }

})();