<script setup lang="ts">
import 'devextreme/dist/css/dx.fluent.blue.light.css';

import { computed, ref, watch } from "vue";
import CustomPagination from "@/components/ui/ms-pagination/CustomPagination.vue";
import { toast } from "@/services/toast.ts";
import DxDataGrid, {
  DxColumn,
  DxSelection
} from 'devextreme-vue/data-grid';
import DxSelectBox from 'devextreme-vue/select-box';
import { DxPopup, DxToolbarItem } from 'devextreme-vue/popup';
import { type SalaryComponents, salaryComponentsData } from "@/views/ms-salary-components/data.ts";
import { usePagination } from "@/views/ms-salary-components/usePagination.ts";
import SalaryComponentForm from "./SalaryComponentForm.vue";

toast.info('Dang nhap thanh cong', 'Chao mung den voi trang tuyen dung')

const selectedIds = ref<string[]>([]);
const isLoading = ref(false);
const isSlowLoading = ref(false);

const pageSizeOptions = [
  { value: 5, label: "5" },
  { value: 10, label: "10" },
  { value: 15, label: "15" },
  { value: 25, label: "25" },
  { value: 50, label: "50" },
];

const searchKeyword = ref("");
const filteredData = computed(() => {
  if (!searchKeyword.value) return salaryComponentsData;
  const keyword = searchKeyword.value.toLowerCase();
  return salaryComponentsData.filter(item =>
    item.componentId.toLowerCase().includes(keyword) ||
    item.componentName.toLowerCase().includes(keyword) ||
    item.componentType.toLowerCase().includes(keyword)
  );
});

// <editor-fold> desc="Search"
watch(searchKeyword, () => {
  currentPage.value = 1;
});
// </editor-fold>

// Khởi tạo pagination trước khi dùng trong fetch
// Khởi tạo các biến cơ bản từ pagination
const {
  currentPage,
  pageSize,
  handlePageSizeChange,
  paginatedData,
  totalRecords,
  pageInfo: hookPageInfo
} = usePagination(filteredData);

const calculateSTT = (data: SalaryComponents) => {
  const index = filteredData.value.findIndex(item => item.componentId === data.componentId);
  return index + 1;
};

const isConfirmModalOpen = ref(false);
const selectedComponent = ref<any>(null);

const handleActive = (data: any) => {
  selectedComponent.value = data;
  isConfirmModalOpen.value = true;
};

const closeConfirmModal = () => {
  isConfirmModalOpen.value = false;
  selectedComponent.value = null;
};

const confirmActive = () => {
  alert('Đã chuyển trạng thái thành công!');
  closeConfirmModal();
};

const isFormVisible = ref(false);
const formMode = ref<'add' | 'edit' | 'copy'>('add');
const formInitialData = ref<any>(null);

const handleAdd = () => {
  formMode.value = 'add';
  formInitialData.value = null;
  isFormVisible.value = true;
};

const handleCopy = (data: any) => {
  formMode.value = 'copy';
  formInitialData.value = data;
  isFormVisible.value = true;
};

const handleEdit = (data: any) => {
  formMode.value = 'edit';
  formInitialData.value = data;
  isFormVisible.value = true;
};

const closeForm = () => {
  isFormVisible.value = false;
  formInitialData.value = null;
};

const handleSaveForm = (data: any) => {
  console.log('Saved data:', data);
};

const handleDelete = (data: any) => {
  console.log('Delete:', data);
};

</script>

<template>
  <section v-if="!isFormVisible" class="content">
    <!-- Title danh sách -->
    <div class="content_header">
      <div class="content_header_left">
        <div class="content_header_title">Thành phần lương</div>
      </div>
      <div class="content_header_right">
        <button class="misa-btn-outline">
          <div class="mi_icon_system_category"></div>
          <span>Danh mục của hệ thống</span>
        </button>

        <div class="split_button_container">
          <button class="misa-btn-primary-left" @click="handleAdd">
            <div class="mi_icon_add_white"></div>
            Thêm mới
          </button>
          <div class="split_button_arrow">
            <div class="mi_icon_arrow_down_white"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Nội dung bảng -->
    <div class="content_body">
      <div class="content_body_container">
        <!--        Title-->
        <div class="content_body_title">
          <div class="content_body_header_left">
            <div class="content_body_header_left_search">
              <input type="text" class="misa-search-input" v-model="searchKeyword" placeholder="Tìm kiếm"
                style="width: 250px;" />
            </div>
          </div>
          <div class="content_body_header_right">
            <div class="content_body_header_right_filters">
              <DxSelectBox class="misa-selectbox" :items="[{ text: 'Tất cả trạng thái', value: 'all' }]"
                display-expr="text" value-expr="value" value="all" :width="160" />
              <DxSelectBox class="misa-selectbox" :items="[{ text: 'Tất cả đơn vị', value: 'all' }]" display-expr="text"
                value-expr="value" value="all" :width="320" />
            </div>
            <div class="content_body_header_right_icon">
              <div class="mi_icon_filter"></div>
            </div>
            <div class="content_body_header_right_icon">
              <div class="mi_icon_setting"></div>
            </div>
          </div>
        </div>

        <!--        Content table-->
        <div class="content_body_table">
          <div class="table_wrapper">
            <DxDataGrid :data-source="paginatedData" :show-borders="true" :row-alternation-enabled="true"
              key-expr="componentId" v-model:selected-row-keys="selectedIds" :column-auto-width="true">
              <DxSelection mode="multiple" show-check-boxes-mode="always" />
              <DxColumn caption="STT" :calculate-cell-value="calculateSTT" :width="50" alignment="center" />
              <DxColumn data-field="componentId" caption="Mã thành phần" />
              <DxColumn data-field="componentName" caption="Tên thành phần" />
              <DxColumn data-field="appliedFor" caption="Đơn vị áp dụng" />
              <DxColumn data-field="componentType" caption="Loại thành phần" />
              <DxColumn data-field="attribute" caption="Tính chất" />
              <DxColumn data-field="valueType" caption="Kiểu giá trị" />
              <DxColumn data-field="value" caption="Giá trị" />

              <!-- Cột Chức năng -->
              <DxColumn caption="Chức năng" cell-template="actionTemplate" alignment="center" :width="160" fixed
                fixed-position="right" css-class="col-action" />

              <template #actionTemplate="{ data }">
                <div class="action-buttons">
                  <div class="action-btn action-active" @click="handleActive(data.data)" title="Sử dụng">
                    <svg viewBox="0 0 24 24" width="18" height="18" stroke="#2ca01c" stroke-width="2" fill="none"
                      stroke-linecap="round" stroke-linejoin="round">
                      <circle cx="12" cy="12" r="10"></circle>
                      <polyline points="8 12 11 15 16 9"></polyline>
                    </svg>
                  </div>
                  <div class="action-btn action-copy" @click="handleCopy(data.data)" title="Nhân bản">
                    <svg viewBox="0 0 24 24" width="18" height="18" stroke="#5a5a5a" stroke-width="2" fill="none"
                      stroke-linecap="round" stroke-linejoin="round">
                      <rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect>
                      <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path>
                    </svg>
                  </div>
                  <div class="action-btn action-edit" @click="handleEdit(data.data)" title="Sửa">
                    <svg viewBox="0 0 24 24" width="18" height="18" stroke="#5a5a5a" stroke-width="2" fill="none"
                      stroke-linecap="round" stroke-linejoin="round">
                      <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
                      <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
                    </svg>
                  </div>
                  <div class="action-btn action-delete" @click="handleDelete(data.data)" title="Xóa">
                    <svg viewBox="0 0 24 24" width="18" height="18" stroke="#ff4d4f" stroke-width="2" fill="none"
                      stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="3 6 5 6 21 6"></polyline>
                      <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                    </svg>
                  </div>
                </div>
              </template>
            </DxDataGrid>
          </div>
        </div>
        <div class="content_body_footer">
          <!-- Tổng bản ghi và Số bản ghi trên trang -->
          <div class="content_body_footer_left">
            <div class="content_body_footer_total">
              Tổng: <strong id="totalRecords">{{ totalRecords }}</strong> bản ghi
            </div>
          </div>

          <!-- Phân trang và điều hướng -->
          <div class="content_body_footer_right">
            <div class="content_body_footer_pagesize">
              <span class="paging_label">Số bản ghi trên trang</span>
              <div class="page_size_custom_select">
                <DxSelectBox class="misa-selectbox" v-model="pageSize" :items="pageSizeOptions" display-expr="label"
                  value-expr="value" :width="70" @value-changed="handlePageSizeChange()" />
              </div>
            </div>
            <div class="content_body_footer_info">
              <span class="page_info" id="pageInfo">{{ hookPageInfo }}</span>
            </div>
            <div class="content_body_footer_nav">
              <CustomPagination v-model="currentPage" :total="totalRecords" :page-size="pageSize" color="#0070f3" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>

  <!-- Confirm Modal using DevExtreme DxPopup -->
  <DxPopup v-model:visible="isConfirmModalOpen" :width="480" height="auto" title="Chuyển trạng thái"
    :show-close-button="true" :drag-enabled="false">
    <div class="misa-popup-body">
      Bạn có chắc chắn muốn chuyển trạng thái thành phần lương <strong>{{ selectedComponent?.componentName }}</strong>
      sang đang theo dõi không?
    </div>

    <DxToolbarItem toolbar="bottom" location="after" template="cancelBtn" />
    <DxToolbarItem toolbar="bottom" location="after" template="confirmBtn" />

    <template #cancelBtn>
      <button class="misa-btn-cancel" @click="closeConfirmModal">Hủy bỏ</button>
    </template>

    <template #confirmBtn>
      <button class="misa-btn-primary" @click="confirmActive">Đồng ý</button>
    </template>
  </DxPopup>

  <!-- Form component overlay -->
  <SalaryComponentForm v-if="isFormVisible" :mode="formMode" :initial-data="formInitialData" @close="closeForm"
    @save="handleSaveForm" />
</template>

<style scoped src="./style.css"></style>
<style scoped>
.misa-selectbox {
  border: 1px solid #dddde4;
  border-radius: 4px;
}

.misa-selectbox.dx-state-hover {
  border-color: #2ca01c;
}

.content_body_footer_pagesize {
  display: flex;
  align-items: center;
  gap: 8px;
}

.page_size_custom_select {
  width: 70px;
}

.misa-search-input {
  height: 32px;
  padding: 0 12px 0 32px;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  font-family: inherit;
  font-size: 13px;
  color: #111;
  outline: none;
  background-color: #fff;
  transition: border-color 0.2s;

  /* Icon kính lúp trực tiếp làm background cho thẻ input */
  background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%23888' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Ccircle cx='11' cy='11' r='6'%3E%3C/circle%3E%3Cline x1='20' y1='20' x2='15.24' y2='15.24'%3E%3C/line%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: 8px center;
  background-size: 16px 16px;
}

.misa-search-input::placeholder {
  color: #888;
}

.misa-search-input:hover,
.misa-search-input:focus {
  border-color: #2ca01c;
}

/* Styles for Action Column */
.action-buttons {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  visibility: hidden;
}

:deep(.dx-datagrid-rowsview .dx-row:hover) .action-buttons,
:deep(.dx-datagrid-rowsview .dx-row.dx-selection) .action-buttons {
  visibility: visible;
}

.action-btn {
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: opacity 0.2s ease;
}

.action-btn:hover {
  opacity: 0.6;
}

:deep(.dx-datagrid-rowsview .dx-row.dx-state-hover > td) {
  background-color: #f2fdf5 !important;
}

/* Popup Styles */
.misa-popup-body {
  font-size: 15px;
  color: #333;
  line-height: 1.5;
  padding: 8px 0;
}

.misa-btn-cancel {
  padding: 8px 24px;
  border: 1px solid #e0e0e0;
  background: #fff;
  border-radius: 4px;
  color: #111;
  font-weight: 600;
  cursor: pointer;
  font-family: inherit;
  font-size: 14px;
  transition: background-color 0.2s;
}

.misa-btn-cancel:hover {
  background: #f5f5f5;
}

.misa-btn-primary {
  padding: 8px 24px;
  border: 1px solid transparent;
  background: #2ca01c;
  border-radius: 4px;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
  font-family: inherit;
  font-size: 14px;
  transition: background-color 0.2s;
}

.misa-btn-primary:hover {
  background: #248b17;
}
</style>
