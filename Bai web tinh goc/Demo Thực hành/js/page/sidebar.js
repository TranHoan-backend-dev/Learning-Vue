import { checkAll, handleCandidateSearching, initBulkDelete, initModal, renderCandidatePage } from "./employee.js";

// Xử lý việc chuyển trang

export const navigateToHiringPostPage = (el) => {
    fetch("component/hiring-post.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToCandidatePage = (el) => {
    fetch("component/candidates.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
        renderCandidatePage();
        initModal();
        checkAll();
        initBulkDelete();
        handleCandidateSearching();
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToCalendarPage = (el) => {
    fetch("component/calendar.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToPotentialPage = (el) => {
    fetch("component/hiring-post.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToHiringCampaignPage = (el) => {
    fetch("component/media-campaign.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToJobPage = (el) => {
    fetch("component/job.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToAIMarketingPage = (el) => {
    fetch("component/marketing.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToContactPage = (el) => {
    fetch("component/conversation.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToReportPage = (el) => {
    fetch("component/report.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToSettingPage = (el) => {
    fetch("component/setting.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}
export const navigateToKnowledgePage = (el) => {
    fetch("component/useful-knowledge.html").then(res => res.text()).then(data => {
        document.getElementById("content").innerHTML = data;
    });
    removeOldActiveClassAndAddNewActiveClass(el);
}

/**
 * Bỏ class quy định thuộc tính active của tab cũ để sẵn sàng gán active cho tab mới
 * @param {*} el Tab active cũ cần gỡ
 */
const removeOldActiveClassAndAddNewActiveClass = (el) => {
    document.querySelectorAll(".sidebar__menu__item-active")
        .forEach(el => el.classList.remove("sidebar__menu__item-active"));
    el.classList.add('sidebar__menu__item-active');
}
