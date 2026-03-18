import randomUuid from "../utils/utils.js"
import toast from "../components/toast.js"
import { data } from "../data/data.js"

let pageIndex = 1;
let displayData = [...data];

/**
 * Khởi tạo modal
 */
export const initModal = () => {
    const modal = document.getElementById("candidateModal");
    const openBtn = document.querySelector(".content__header__action__add");
    const closeBtn = document.getElementById("closeModal");
    const cancelBtn = document.getElementById("cancelModal");

    if (openBtn) {
        openBtn.addEventListener("click", () => {
            modal.style.display = "flex";
        });
    }

    handleCloseBtn(closeBtn, modal);
    handleCancelBtn(cancelBtn, modal);
    closeModalWhenUserClickOutsideModal(modal);

    document.getElementById('addNewCandidate').addEventListener('submit', function (e) {
        e.preventDefault();

        const formData = new FormData(this);
        const candidateData = {
            fullName: formData.get("fullName"),
            dob: formData.get("dob"),
            gender: formData.get("gender"),
            area: formData.get("area"),
            phone: formData.get("phone"),
            email: formData.get("email"),
            address: formData.get("address"),
            educationLevel: formData.get("educationLevel"),
            educationLocation: formData.get("educationLocation"),
            major: formData.get("major"),
            applyDate: formData.get("applyDate"),
            candidateSource: formData.get("candidateSource"),
            recruiter: formData.get("recruiter"),
            collaborator: formData.get("collaborator"),
            quickAddReference: formData.get("quickAddReference") === 'on',
            recentWorkplace: formData.get("recentWorkplace"),
            workplace: formData.get("workplace"),
            startDate: formData.get("startDate"),
            endDate: formData.get("endDate"),
            jobPosition: formData.get("jobPosition"),
            jobDescription: formData.get("jobDescription")
        };
        console.log(candidateData);

        try {
            validateEmail(candidateData.email);
            validatePhone(candidateData.phone);
            validateFullName(candidateData.fullName);
        } catch (error) {
            toast({
                title: "Thêm mới thất bại",
                message: error.message,
                type: "error"
            });
            return;
        }

        displayData.push({
            id: randomUuid(),
            name: candidateData.fullName,
            phone: candidateData.phone,
            email: candidateData.email,
            hiring_campaign: '',
            hiring_position: '',
            hiring_post: '',
            hiring_round: '',
            review: '',
            hiring_at: new Date().toLocaleDateString('vi-VN'),
            status: ''
        });

        renderCandidatePage();

        toast({
            title: "Thành công",
            message: "Ứng viên đã được thêm vào danh sách",
            type: "success"
        });

        closeModal(modal);
    });
};

/**
 * Xác minh email. Email không được trùng với bản ghi đang có
 * @param {*} email Dữ liệu form gửi về
 */
const validateEmail = (email) => {
    if (data.find(item => item.email === email)) {
        throw new Error("Email đã tồn tại");
    }
}

/**
 * Xác minh sdt. Sdt phải đúng format và không trùng
 * @param {*} phone Sdt form gửi về
 */
const validatePhone = (phone) => {
    const regex = /^[0-9]{10}$/
    if (!regex.test(phone)) {
        throw new Error("Sdt không đúng định dạng")
    }
    if (data.find(item => item.phone === phone)) {
        throw new Error("Sdt đã tồn tại")
    }
}

/**
 * Xác minh dữ liệu đầu vào của trường fullName
 * @param {*} fullName Dữ liệu form gửi về
 */
const validateFullName = (fullName) => {
    const regex = /^[\p{L} ]+$/u;
    if (!regex.test(fullName)) {
        throw new Error("Họ tên không được chứa chữ số hoặc ký tự đặc biệt");
    }
}

/**
 * Xử lý việc tạo bảng ứng viên lần đầu. Gán sự kiện cho các nút next và prev,
 * tạo các sự kiện cập nhật bảng, xóa bản ghi, cập nhật bản ghi,...
 */
export const renderCandidatePage = () => {
    const prevBtn = document.getElementById("btnPrevPage")
    const nextBtn = document.getElementById("btnNextPage")
    const totalElements = document.getElementById('totalRecords')
    const pageSize = document.getElementById('pageSizeSelect');
    const pageInfo = document.getElementById('pageInfo');

    localStorage.setItem("data", JSON.stringify(data));

    let size = parseInt(pageSize.value);

    window.updatePagination = () => {
        const numberOfPages = Math.ceil(displayData.length / size);
        if (pageIndex > numberOfPages && numberOfPages > 0) pageIndex = numberOfPages;

        prevBtn.disabled = (pageIndex <= 1);
        nextBtn.disabled = (pageIndex >= numberOfPages || numberOfPages === 0);

        pagination(pageIndex, size, pageInfo);
        totalElements.innerText = displayData.length;
    };

    pageSize.onchange = function () {
        size = parseInt(this.value);
        pageIndex = 1;
        window.updatePagination();
    };

    prevBtn.onclick = function () {
        if (pageIndex > 1) {
            pageIndex--;
            window.updatePagination();
        }
    };

    nextBtn.onclick = function () {
        const numberOfPages = Math.ceil(displayData.length / size);
        if (pageIndex < numberOfPages) {
            pageIndex++;
            window.updatePagination();
        }
    };

    window.updatePagination();

    window.deleteRecord = (id) => {
        const modal = document.getElementById("deleteConfirmModal");
        const closeBtn = document.getElementById("closeDeleteModal");
        const cancelBtn = document.getElementById("cancelDeleteModal");
        const confirmBtn = document.getElementById("confirmDeleteBtn");

        if (modal) {
            modal.style.display = 'flex';
            const idxOriginal = data.findIndex(item => item.id === id);
            if (idxOriginal !== -1) data.splice(idxOriginal, 1);

            const idxDisplay = displayData.findIndex(item => item.id === id);
            if (idxDisplay !== -1) displayData.splice(idxDisplay, 1);

            window.updatePagination();
            localStorage.setItem("data", JSON.stringify(data));

            if (confirmBtn) {
                confirmBtn.addEventListener('click', function () {
                    displayData.filter(item => item.id !== id);
                    updatePagination();

                    modal.style.display = 'none';
                    toast({
                        title: "Xóa ứng viên",
                        message: "Xóa ứng viên thành công",
                        type: "success"
                    })
                })
            }

            handleCancelBtn(cancelBtn, modal);
            handleCloseBtn(closeBtn, modal);
            closeModalWhenUserClickOutsideModal(modal);
        }
    }

    window.updateRecord = (id) => {
        const modal = document.getElementById("updateCandidateModal");
        const closeBtn = document.getElementById("closeUpdateModal");
        const cancelBtn = document.getElementById("cancelUpdateModal");
        const confirmBtn = document.getElementById("confirmUpdateBtn");
        let originalItem = data.find(item => item.id === id);
        console.log(originalItem);

        bindingData(originalItem);

        if (modal) {
            modal.style.display = 'flex';
            console.log(id);
        }
        handleCloseBtn(closeBtn, modal);
        handleCancelBtn(cancelBtn, modal);
        if (confirmBtn) {
            const updateForm = document.getElementById("updateCandidate");
            if (updateForm) {
                updateForm.onsubmit = function (e) {
                    e.preventDefault();

                    const formData = new FormData(this);
                    const candidateData = {
                        fullName: formData.get("fullName"),
                        phone: formData.get("phone"),
                        email: formData.get("email"),
                        dob: formData.get("dob"),
                        gender: formData.get("gender"),
                        area: formData.get("area"),
                        address: formData.get("address"),
                        educationLevel: formData.get("educationLevel"),
                        educationLocation: formData.get("educationLocation"),
                        major: formData.get("major"),
                        applyDate: formData.get("applyDate"),
                        candidateSource: formData.get("candidateSource"),
                        recruiter: formData.get("recruiter"),
                        collaborator: formData.get("collaborator"),
                        quickAddReference: formData.get("quickAddReference") === 'on',
                        recentWorkplace: formData.get("recentWorkplace"),
                        workplace: formData.get("workplace"),
                        startDate: formData.get("startDate"),
                        endDate: formData.get("endDate"),
                        jobPosition: formData.get("jobPosition"),
                        jobDescription: formData.get("jobDescription")
                    };

                    try {
                        if (candidateData.email !== originalItem.email) {
                            validateEmail(candidateData.email, id);
                        }
                        if (candidateData.phone !== originalItem.phone) {
                            validatePhone(candidateData.phone, id);
                        }
                        validateFullName(candidateData.fullName);
                    } catch (error) {
                        toast({
                            title: "Cập nhật thất bại",
                            message: error.message,
                            type: "error"
                        });
                        return;
                    }

                    originalItem.name = candidateData.fullName;
                    originalItem.phone = candidateData.phone;
                    originalItem.email = candidateData.email;

                    window.updatePagination();
                    localStorage.setItem("data", JSON.stringify(data));

                    toast({
                        title: "Thành công",
                        message: "Thông tin ứng viên đã được cập nhật",
                        type: "success"
                    });

                    closeModal(modal);
                };
            }
        }
    }

    const bindingData = (data) => {
        let fullName = document.getElementById('fullName');
        let phone = document.getElementById('phone');
        let email = document.getElementById('email');

        fullName.value = data.name;
        phone.value = data.phone ?? "";
        email.value = data.email ?? "";
    }

    window.viewDetails = (id) => {
        console.log(id);
        const modal = document.getElementById("candidateDetailModal");
        const closeBtn = document.getElementById("closeDetailModal");

        if (modal) {
            const candidate = data.find(c => c.id === id);

            const nameEl = modal.querySelector(".detail-sidebar__name");
            if (nameEl) nameEl.innerText = candidate.name;

            const emailEl = modal.querySelector(".detail_sidebar_label_email");
            if (emailEl) emailEl.innerText = candidate.email;

            modal.style.display = "flex";
            handleCloseBtn(closeBtn, modal)
        }
        closeModalWhenUserClickOutsideModal(modal);
    }
};

/**
 * Xử lý sự kiện đóng modal bằng cách nhấp vào nút X ở đầu modal
 * @param {*} closeBtn Nút đóng ở đầu modal
 * @param {*} modal Modal bất kỳ
 */
const handleCloseBtn = (closeBtn, modal) => {
    if (closeBtn) {
        closeBtn.addEventListener('click', function () {
            closeModal(modal);
        })
    }
}

/**
 * Xử lý sự kiện đóng modal bằng cách nhấp vào nút hủy ở chân modal
 * @param {*} cancelBtn Nút hủy ở chân modal
 * @param {*} modal Modal bất kỳ
 */
const handleCancelBtn = (cancelBtn, modal) => {
    if (cancelBtn) {
        cancelBtn.addEventListener('click', function () {
            closeModal(modal);
        })
    }
}

/**
 * Xử lý việc đóng modal
 * @param {*} modal Modal bất kỳ
 */
const closeModal = (modal) => {
    modal.style.display = "none";
};

/**
 * Xử lý việc đóng modal khi người dùng click vào vùng bên ngoài của modal
 * @param {*} modal Modal bất kỳ
 */
const closeModalWhenUserClickOutsideModal = (modal) => {
    window.addEventListener("click", (event) => {
        if (event.target == modal) {
            closeModal(modal);
        }
    });
}

/**
 * Xử lý phân trang và binding các bản ghi vào bảng
 * @param {*} pageIndex Chỉ số trang hiện tại
 * @param {*} size Cỡ bảng
 * @param {*} pageInfo Thông tin về chỉ số bản ghi trong trang hiện tại
 */
const pagination = (pageIndex, size, pageInfo) => {
    const tableBody = document.getElementById('table__body');
    let start = (pageIndex - 1) * size;
    let end = start + size;
    const currentPageData = displayData.slice(start, end);

    const displayEnd = Math.min(end, displayData.length);
    const displayStart = displayData.length > 0 ? start + 1 : 0;
    pageInfo.innerText = `Từ ${displayStart} - ${displayEnd} bản ghi`;

    tableBody.innerHTML = currentPageData.map((d, index) => `
        <tr>
            <td class="col-checkbox"><input type="checkbox" class="item-checkbox" data-id="${d.id}"></td>
            <td class="col-name stt">${start + index + 1}</td>
            <td class="col-name">${d.name}</td>
            <td class="col-phone">${d.phone ? d.phone : "--"}</td>
            <td class="col-email" title="${d.email}">${d.email ? d.email : "--"}</td>
            <td class="col-email">${d.hiring_campaign ? d.hiring_campaign : "--"}</td>
            <td class="col-email">${d.hiring_position ? d.hiring_position : "--"}</td>
            <td class="col-email">${d.hiring_post ? d.hiring_post : "--"}</td>
            <td class="col-email">${d.hiring_round ? d.hiring_round : "--"}</td>
            <td class="col-date">${d.hiring_at ? d.hiring_at : "--"}</td>
            <td class="col-email text-center">
                ${[1, 2, 3, 4, 5].map(_ => `<i class="bi bi-star"></i>`).join("")}
            </td>
            <td>
                <div class="table__action">
                    <div class="mi__icon">
                        <button type="button" onclick="updateRecord('${d.id}')" class="mi__icon__edit" title="Sửa ứng viên"></button>
                    </div>
                    <div class="mi__icon">
                        <button type="button" onclick="deleteRecord('${d.id}')" class="mi__icon__delete" title="Xóa ứng viên"></button>
                    </div>
                    <div class="mi__icon">
                        <button type="button" class="mi_icon_view_details" onclick="viewDetails('${d.id}')" title="Xem ứng viên"></button>
                    </div>
                </div>
            </td>
        </tr>
    `).join("");
}

/**
 * Xử lý việc chọn toàn bộ các bản ghi bằng checkbox
 */
export const checkAll = () => {
    const theadCheckbox = document.getElementById('checkAll');
    if (theadCheckbox) {
        theadCheckbox.addEventListener('change', function () {
            const rowCheckboxes = document.querySelectorAll('.item-checkbox');
            rowCheckboxes.forEach(checkbox => {
                checkbox.checked = theadCheckbox.checked;
            });
        });
    }
}

/**
 * Xử lý việc xóa nhiều bản ghi
 */
export const initBulkDelete = () => {
    const bulkDeleteBtn = document.getElementById('bulkDeleteBtn');
    const modal = document.getElementById("deleteConfirmModal");
    const closeBtn = document.getElementById("closeDeleteModal");
    const cancelBtn = document.getElementById("cancelDeleteModal");
    const confirmBtn = document.getElementById("confirmDeleteBtn");

    if (bulkDeleteBtn && modal) {
        bulkDeleteBtn.addEventListener('click', () => {
            const selectedCheckboxes = document.querySelectorAll('.item-checkbox:checked');
            if (selectedCheckboxes.length > 0) {
                modal.style.display = "flex";
            }
        });
        if (confirmBtn) {
            confirmBtn.addEventListener('click', function () {
                const selectedCheckboxes = document.querySelectorAll('.item-checkbox:checked');
                const selectedIds = Array.from(selectedCheckboxes).map(cb => cb.dataset.id);

                if (selectedIds.length === 0) return;

                selectedIds.forEach(id => {
                    const index = data.findIndex(item => item.id === id);
                    if (index !== -1) {
                        data.splice(index, 1);
                    }
                });
                displayData = data; // Update displayData after bulk delete

                localStorage.setItem("data", JSON.stringify(data));

                toast({
                    title: "Thành công",
                    message: `Đã xóa ${selectedIds.length} bản ghi`,
                    type: "success"
                });

                renderCandidatePage();

                // Reset checkbox head
                const theadCheckbox = document.getElementById('checkAll');
                if (theadCheckbox) theadCheckbox.checked = false;
                closeModal(modal);
            })
        }

        handleCancelBtn(cancelBtn, modal);
        handleCloseBtn(closeBtn, modal);
    }
}

/**
 * Xử lý tìm kiếm các ứng viên theo name, email và phone
 */
export const handleCandidateSearching = () => {
    const searchField = document.querySelector('.content__body__header__right__search__input');
    if (!searchField) return;

    searchField.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            const keyword = this.value.trim().toLowerCase();

            // Lọc dữ liệu và gán vào displayData
            displayData = data.filter(item =>
                (item.name && item.name.toLowerCase().includes(keyword)) ||
                (item.email && item.email.toLowerCase().includes(keyword)) ||
                (item.phone && item.phone.includes(keyword))
            );

            pageIndex = 1; // Quay về trang 1
            if (window.updatePagination) {
                window.updatePagination();
            }
        }
    })
}

// xử lý sự kiện liên quan tới bộ lọc
export const initFilterSidebar = () => {
    const filterSidebar = document.getElementById("filterSidebar");
    const filterOverlay = document.getElementById("filterOverlay");
    const openFilterBtn = document.getElementById("openFilterBtn");
    const closeFilterBtn = document.querySelector("#filterSidebar .close__btn");
    const applyFilterBtn = document.getElementById("applyFilterBtn");
    const clearFilterBtn = document.getElementById("clearFilterBtn");

    if (openFilterBtn && filterSidebar && filterOverlay) {
        openFilterBtn.addEventListener("click", () => {
            filterSidebar.classList.add("active");
            filterOverlay.classList.add("active");
        });
    }

    const closeSidebar = () => {
        if (filterSidebar) filterSidebar.classList.remove("active");
        if (filterOverlay) filterOverlay.classList.remove("active");
    };

    if (closeFilterBtn) closeFilterBtn.addEventListener("click", closeSidebar);
    if (filterOverlay) filterOverlay.addEventListener("click", closeSidebar);
    if (applyFilterBtn) applyFilterBtn.addEventListener("click", closeSidebar);
    if (clearFilterBtn) clearFilterBtn.addEventListener("click", closeSidebar);
};

// Xử lý tải CV
export const initFileUpload = () => {
    const uploadBoxes = document.querySelectorAll('.upload__box');

    uploadBoxes.forEach(box => {
        const input = box.querySelector('.cv_file_input');

        if (!input) return;

        box.addEventListener('click', () => {
            input.click();
        });

        input.addEventListener('change', (e) => {
            const files = e.target.files;
            handleFiles(files, box);
        });

        // Ngăn chặn hành vi mặc định của trình duyệt khi kéo thả
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            box.addEventListener(eventName, (e) => {
                e.preventDefault();
                e.stopPropagation();
            }, false);
        });

        ['dragenter', 'dragover'].forEach(eventName => {
            box.addEventListener(eventName, () => {
                box.classList.add('dragging');
            }, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            box.addEventListener(eventName, () => {
                box.classList.remove('dragging');
            }, false);
        });

        box.addEventListener('drop', (e) => {
            const dt = e.dataTransfer;
            const files = dt.files;
            handleFiles(files, box);
        });
    });

    const handleFiles = (files, box) => {
        if (files.length > 0) {
            const file = files[0];
            const fileName = file.name;
            const fileSize = (file.size / (1024 * 1024)).toFixed(2);

            if (fileSize > 15) {
                toast({
                    title: "Lỗi tải file",
                    message: "Dung lượng file không được vượt quá 15MB",
                    type: "error"
                });
                return;
            }

            const textEl = box.querySelector('.upload__text');
            if (textEl) {
                textEl.innerText = `Đã chọn: ${fileName}`;
                textEl.style.color = "#28a745";
            }

            toast({
                title: "Tải file thành công",
                message: `Đã nhận file: ${fileName}`,
                type: "success"
            });

            console.log("File received:", file);
        }
    };
};
