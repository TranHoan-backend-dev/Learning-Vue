import toast from "../components/toast.js";
import { initModal, renderCandidatePage, checkAll, initBulkDelete, handleCandidateSearching, initFilterSidebar, initFileUpload } from "./employee.js";
import { handleSearching } from "./header.js";
import { navigateToAIMarketingPage, navigateToCalendarPage, navigateToCandidatePage, navigateToContactPage, navigateToHiringCampaignPage, navigateToJobPage, navigateToKnowledgePage, navigateToReportPage, navigateToSettingPage, navigateToPotentialPage } from "./sidebar.js";
import { navigateToHiringPostPage } from './sidebar.js'

/**
 * Tải lần đầu sidebar
 */
fetch("component/layout/sidebar.html").then(res => res.text()).then(data => {
    const sidebarContainer = document.getElementById("sidebar");
    sidebarContainer.innerHTML = data;

    const toggleBtn = sidebarContainer.querySelector(".sidebar__toggle");
    if (toggleBtn) {
        toggleBtn.addEventListener("click", () => {
            sidebarContainer.classList.toggle("sidebar__collapsed");
        });
    }

    window.navigateToHiringPostPage = navigateToHiringPostPage;
    window.navigateToCandidatePage = navigateToCandidatePage;
    window.navigateToCalendarPage = navigateToCalendarPage;
    window.navigateToPotentialPage = navigateToPotentialPage;
    window.navigateToHiringCampaignPage = navigateToHiringCampaignPage;
    window.navigateToJobPage = navigateToJobPage;
    window.navigateToAIMarketingPage = navigateToAIMarketingPage;
    window.navigateToContactPage = navigateToContactPage;
    window.navigateToReportPage = navigateToReportPage;
    window.navigateToSettingPage = navigateToSettingPage;
    window.navigateToKnowledgePage = navigateToKnowledgePage;
});

/**
 * Tải lần đầu header
 */
fetch("component/layout/header.html").then(res => res.text()).then(data => {
    document.getElementById('navbar').innerHTML = data
    handleSearching();
});

/**
 * Tải lần đầu trang candidate. Mặc định trang đầu tiên mở ra là candidates.html
 */
fetch("component/candidates.html").then(res => res.text()).then(data => {
    document.getElementById("content").innerHTML = data;
    renderCandidatePage();
    initModal();
    checkAll();
    initBulkDelete();
    handleCandidateSearching();
    initFilterSidebar();
    initFileUpload();
});

/**
 * Tải toast để sẵn sàng sử dụng
 */
fetch("component/toast.html").then(res => res.text()).then(data => {
    document.getElementById("toast").innerHTML = data;
    toast({
        title: "Đăng nhập thành công",
        message: `Chào mừng đến với trang tuyển dụng`,
        type: "success"
    });
});
