const toast = ({ title = "", message = "", type = "success", duration = 3000 }) => {
    const main = document.getElementById("toast-container");
    const template = document.getElementById("toast-template");

    if (main && template) {
        const toastElement = template.content.cloneNode(true).querySelector(".toast");

        toastElement.classList.add(`toast--${type}`);

        const icons = {
            success: "bi-check-circle-fill",
            error: "bi-exclamation-circle-fill",
            warning: "bi-exclamation-triangle-fill"
        };
        const icon = icons[type];
        const iconElement = toastElement.querySelector(".toast__icon i");
        if (iconElement) iconElement.classList.add(icon);

        const titleElement = toastElement.querySelector(".toast__title");
        const msgElement = toastElement.querySelector(".toast__msg");
        if (titleElement) titleElement.innerText = title;
        if (msgElement) msgElement.innerText = message;

        const closeBtn = toastElement.querySelector(".toast__close");
        if (closeBtn) {
            closeBtn.onclick = () => {
                toastElement.style.animation = "fadeOut 0.5s ease forwards";
                setTimeout(() => {
                    if (main.contains(toastElement)) main.removeChild(toastElement);
                }, 500);
            };
        }

        setTimeout(() => {
            if (main.contains(toastElement)) {
                toastElement.style.animation = "fadeOut 0.5s ease forwards";
                setTimeout(() => {
                    if (main.contains(toastElement)) main.removeChild(toastElement);
                }, 500);
            }
        }, duration);

        main.appendChild(toastElement);
    }
};

export default toast;
