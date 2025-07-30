// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    var select = document.querySelector("select[name='placeId']");
    if (select) {
        select.addEventListener("change", function () {
            var selectedOption = this.options[this.selectedIndex].text.toLowerCase();
           var isMain = true;
            if (selectedOption.includes("مطعم") || selectedOption.includes("كافيه")) {
                isMain = false;
            }
            var input = document.getElementById("isMainInput");
            if (input) input.value = isMain;
        });
    }
});


document.addEventListener("DOMContentLoaded", function () {
    const deleteButtons = document.querySelectorAll(".delete-btn, .delete-day-btn");

    deleteButtons.forEach(button => {
        button.addEventListener("click", function () {
            const actionUrl = this.dataset.action || this.getAttribute("data-action");

            if (!actionUrl) return;

            Swal.fire({
                title: 'هل أنت متأكد من الحذف؟',
                text: "لن تتمكن من التراجع بعد ذلك!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'نعم، احذف',
                cancelButtonText: 'إلغاء',
                customClass: {
                    popup: 'swal-narrow-popup',
                    icon: 'swal-small-icon'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    const form = document.createElement("form");
                    form.method = "post";
                    form.action = actionUrl;
                    document.body.appendChild(form);
                    form.submit();
                }
            });
        });
    });
});


document.addEventListener("DOMContentLoaded", function () {
    const placeDropdown = document.getElementById("placeDropdown");
    const isMainInput = document.getElementById("isMainInput");
    const typeFilter = document.getElementById("typeFilter");

    if (placeDropdown && isMainInput) {
        placeDropdown.addEventListener("change", function () {
            const selectedText = this.options[this.selectedIndex].text.toLowerCase();
            const isMain = !(selectedText.includes("مطعم") || selectedText.includes("كافيه"));
            isMainInput.value = isMain;
        });
    }

    if (typeFilter && placeDropdown) {
        typeFilter.addEventListener("change", function () {
            const selectedType = this.value;
            const placeOptions = placeDropdown.querySelectorAll("option");

            placeOptions.forEach(opt => {
                if (!opt.value) {
                    opt.hidden = false;
                } else {
                    opt.hidden = selectedType && opt.dataset.type !== selectedType;
                }
            });

            placeDropdown.selectedIndex = 0;
        });
    }
});
