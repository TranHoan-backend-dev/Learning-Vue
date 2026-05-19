<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { DxPopup } from 'devextreme-vue/popup';
import DxSelectBox from 'devextreme-vue/select-box';
import DxDataGrid, {
  DxColumn,
  DxSelection,
  DxScrolling,
  DxPaging
} from 'devextreme-vue/data-grid';
import CustomPagination from '@/components/ui/ms-pagination/CustomPagination.vue';
import MSIcon from '@/components/ui/ms-icon/MSIcon.vue';
import salaryCompositionService from '@/services/salaryCompositionService.ts';
import salaryCompositionSystemService from '@/services/salaryCompositionSystemService.ts';
import { getAttributeName, pageSizeOptions } from '@/views/ms-salary-compositions/data.ts';
import { toast } from '@/services/toast.ts';

const emit = defineEmits(['close', 'save']);

const isLoading = ref(false);
const searchKeyword = ref('');
const tableData = ref<any[]>([]);
const totalRecords = ref(0);
const currentPage = ref(1);
const pageSize = ref(25);
const selectedIds = ref<string[]>([]);
const systemCategories = ref<any[]>([
  { salaryComponentSystemId: 'all', salaryComponentSystemName: 'Tất cả' }
]);
const selectedSystemId = ref('all');

// Tải danh mục loại thành phần để lọc
const loadSystemCategories = async () => {
  try {
    const response = await salaryCompositionSystemService.getAll();
    if (response.data) {
      systemCategories.value = [
        { salaryComponentSystemId: 'all', salaryComponentSystemName: 'Tất cả' },
        ...response.data
      ];
      selectedSystemId.value = 'all';
    }
  } catch (error) {
    console.error('Lỗi khi tải danh mục hệ thống:', error);
  }
};

// Tải dữ liệu các thành phần chưa sử dụng từ hệ thống
const fetchData = async () => {
  isLoading.value = true;
  try {
    const pageable = {
      pageIndex: currentPage.value - 1,
      pageSize: pageSize.value
    };

    const filterRequest = {
      Keyword: searchKeyword.value,
      ColumnFilters: [] as any[]
    };

    // Lọc theo Loại thành phần nếu được chọn
    if (selectedSystemId.value && selectedSystemId.value !== 'all') {
      filterRequest.ColumnFilters.push({
        Column: 'SalaryComponentSystemId',
        Value: selectedSystemId.value.toString(),
        DataType: 0,
        FilterType: 4
      });
    }

    const response = await salaryCompositionService.getFilter(pageable, filterRequest, false);

    if (response.data && response.data.data) {
      tableData.value = response.data.data.map((item: any) => ({
        componentId: item.salaryComponentId,
        componentCode: item.salaryComponentCode,
        componentName: item.salaryComponentName,
        salaryComponentSystemId: item.salaryComponentSystemId,
        salaryComponentSystemName: item.salaryComponentSystemName || 'Hệ thống',
        attribute: item.attribute,
        attributeText: getAttributeName(item.attribute),
        valueType: item.valueType,
        value: item.value || '-',
        taxType: item.taxType,
        taxTypeText: item.taxType || '-',
        isUsed: item.isUsed
      }));
      totalRecords.value = response.data.pageable?.totalElements || response.data.data.length;
    }
  } catch (error) {
    console.error('Lỗi khi tải dữ liệu từ hệ thống:', error);
  } finally {
    isLoading.value = false;
  }
};

onMounted(() => {
  loadSystemCategories();
  fetchData();
});

// Theo dõi các bộ lọc để tải lại dữ liệu
watch([currentPage, pageSize, selectedSystemId], () => {
  fetchData();
});

// Theo dõi từ khóa tìm kiếm để reset về trang 1
watch(searchKeyword, () => {
  currentPage.value = 1;
  fetchData();
});

const handlePageSizeChange = (e: any) => {
  pageSize.value = e.value;
  currentPage.value = 1;
};

const pageInfo = computed(() => {
  const start = totalRecords.value > 0 ? (currentPage.value - 1) * pageSize.value + 1 : 0;
  const end = Math.min(currentPage.value * pageSize.value, totalRecords.value);
  return `${start} - ${end}`;
});

const totalPages = computed(() => Math.ceil(totalRecords.value / pageSize.value) || 1);

const selectedRows = computed(() => {
  return tableData.value.filter(item => selectedIds.value.includes(item.componentId));
});

// Xử lý sự kiện click nút Đồng ý để lưu các bản ghi đã chọn
const handleAgree = async () => {
  if (selectedIds.value.length === 0) {
    toast.warning('Thông báo', 'Vui lòng chọn ít nhất một thành phần lương để thêm.');
    return;
  }

  isLoading.value = true;
  try {
    const promises = selectedRows.value.map(async (data) => {
      const requestData = {
        SalaryComponentCode: data.componentCode,
        SalaryComponentName: data.componentName,
        SalaryComponentSystemId: data.salaryComponentSystemId,
        Attribute: data.attribute,
        ValueType: data.valueType,
        Value: data.value === '-' ? null : data.value,
        Status: 1,
        Source: 'Hệ thống',
        AppliedUnitIds: data.appliedUnitIds || [],
        IsUsed: true
      };
      return salaryCompositionService.update(data.componentId, requestData);
    });

    await Promise.all(promises);
    toast.success('Thành công', `Đã thêm thành công ${selectedIds.value.length} thành phần từ hệ thống`);
    emit('save');
    emit('close');
  } catch (error) {
    console.error('Lỗi khi thêm thành phần lương từ hệ thống:', error);
    toast.error('Lỗi', 'Không thể thêm các thành phần lương đã chọn.');
  } finally {
    isLoading.value = false;
  }
};
</script>

<template>
  <DxPopup
      :visible="true"
      @update:visible="val => { if (!val) emit('close'); }"
      :width="1100"
      :height="700"
      :show-title="false"
      :drag-enabled="false"
      class="system-directory-popup-wrapper"
      :wrapper-attr="{ class: 'system-directory-popup-wrapper' }"
  >
    <div class="system-modal-layout">
      <!-- Modal Header -->
      <div class="system-modal-header">
        <div class="system-modal-title">Thêm từ danh mục của hệ thống</div>
        <div class="system-modal-close" @click="emit('close')" title="Đóng">
          <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </div>
      </div>

      <!-- Modal Body -->
      <div class="system-modal-body">
        <!-- Filter bar -->
        <div class="system-modal-filter-bar">
          <div class="search-input-wrapper">
            <input
                type="text"
                class="misa-search-input-modal"
                v-model="searchKeyword"
                placeholder="Tìm kiếm"
            />
            <div class="search-icon-modal">
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#888" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" width="16" height="16">
                <circle cx="11" cy="11" r="6"></circle>
                <line x1="20" y1="20" x2="15.24" y2="15.24"></line>
              </svg>
            </div>
          </div>

          <div class="filter-dropdown-wrapper">
            <span class="filter-dropdown-label">Loại thành phần:</span>
            <DxSelectBox
                v-model:value="selectedSystemId"
                :items="systemCategories"
                display-expr="salaryComponentSystemName"
                value-expr="salaryComponentSystemId"
                :width="180"
                class="misa-selectbox-borderless"
            />
          </div>
        </div>

        <!-- Data Grid -->
        <div class="system-modal-grid-container">
          <div class="grid-loading-overlay" v-if="isLoading">
            <div class="misa-spinner"></div>
          </div>
          
          <DxDataGrid
              :data-source="tableData"
              :show-borders="true"
              :row-alternation-enabled="false"
              :show-column-lines="false"
              :show-row-lines="true"
              key-expr="componentId"
              v-model:selected-row-keys="selectedIds"
              :column-auto-width="false"
              :allow-column-resizing="true"
              column-resizing-mode="widget"
              width="100%"
              height="100%"
              class="misa-system-grid"
          >
            <DxScrolling
                mode="virtual"
                show-scrollbar="always"
                :use-native="true"
                :scroll-by-content="true"
                :scroll-by-thumb="true"
            />

            <DxPaging :enabled="false"/>

            <DxColumn type="selection" :fixed="true" fixed-position="left" :visible-index="0" :width="50"/>
            <DxSelection mode="multiple" show-check-boxes-mode="always"/>

            <DxColumn data-field="componentCode" caption="Mã thành phần" :width="240" />
            <DxColumn data-field="componentName" caption="Tên thành phần" :width="280" />
            <DxColumn data-field="salaryComponentSystemName" caption="Loại thành phần" :width="180" />
            <DxColumn data-field="attributeText" caption="Tính chất" :width="140" />
            <DxColumn data-field="taxTypeText" caption="Chịu thuế" :width="180" />
          </DxDataGrid>
        </div>
      </div>

      <!-- Modal Footer -->
      <div class="system-modal-footer">
        <div class="footer-left">
          Tổng số: <strong class="total-count-text">{{ totalRecords }}</strong>
        </div>
        <div class="footer-right">
          <!-- Page size selector -->
          <div class="pagination-pagesize-container">
            <span class="pagesize-label-text">Số dòng/trang</span>
            <DxSelectBox
                class="misa-pagesize-select"
                :value="pageSize"
                :items="pageSizeOptions"
                display-expr="label"
                value-expr="value"
                :width="70"
                @value-changed="handlePageSizeChange"
            />
          </div>
          <!-- Pagination count info -->
          <div class="pagination-info-text">
            <strong>{{ pageInfo }}</strong>
          </div>
          <!-- Pagination controls -->
          <div class="pagination-navigation-controls">
            <button 
              class="pagination-nav-btn" 
              :class="{ 'disabled': currentPage === 1 }"
              @click="currentPage = 1" 
              title="Trang đầu"
            >
              K
            </button>
            <button 
              class="pagination-nav-btn" 
              :class="{ 'disabled': currentPage === 1 }"
              @click="currentPage = Math.max(1, currentPage - 1)" 
              title="Trang trước"
            >
              &lt;
            </button>
            <button 
              class="pagination-nav-btn" 
              :class="{ 'disabled': currentPage >= totalPages }"
              @click="currentPage = Math.min(totalPages, currentPage + 1)" 
              title="Trang sau"
            >
              &gt;
            </button>
            <button 
              class="pagination-nav-btn" 
              :class="{ 'disabled': currentPage >= totalPages }"
              @click="currentPage = totalPages" 
              title="Trang cuối"
            >
              &gt;|
            </button>
          </div>
        </div>
      </div>

      <!-- Modal Buttons -->
      <div class="system-modal-buttons">
        <button class="btn-misa-modal-cancel" @click="emit('close')">Hủy bỏ</button>
        <button class="btn-misa-modal-agree" @click="handleAgree">Đồng ý</button>
      </div>
    </div>
  </DxPopup>
</template>

<style>
/* Global style block to override the teleported DevExtreme popup internal padding */
.system-directory-popup-wrapper .dx-popup-content,
.dx-popup-content:has(.system-modal-layout) {
  padding: 0 !important;
}
</style>

<style scoped>

.system-modal-layout {
  display: flex;
  flex-direction: column;
  height: 100%;
  box-sizing: border-box;
}

/* Header */
.system-modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 20px 12px 20px;
  border-bottom: none;
  flex-shrink: 0;
}

.system-modal-title {
  font-size: 20px;
  font-weight: 700;
  color: var(--misa-text-title);
}

.system-modal-close {
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  color: #666;
  transition: all 0.2s;
}

.system-modal-close:hover {
  background-color: #f2f2f2;
  color: #111;
}

/* Body */
.system-modal-body {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 0 16px;
  overflow: hidden;
}

/* Filter bar */
.system-modal-filter-bar {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 16px;
  flex-shrink: 0;
  margin-top: 20px;
}

.search-input-wrapper {
  position: relative;
  width: 260px;
}

.misa-search-input-modal {
  width: 100%;
  height: 32px;
  padding: 0 12px 0 32px;
  border: 1px solid #2ca01c;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
  box-sizing: border-box;
}

.misa-search-input-modal::placeholder {
  color: #a0a0a0;
  font-style: italic;
}

.search-icon-modal {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  display: flex;
  align-items: center;
  pointer-events: none;
}

.filter-dropdown-wrapper {
  display: inline-flex;
  align-items: center;
  border: 1px solid #c2c2c2;
  border-radius: 4px;
  padding: 0 10px;
  height: 32px;
  gap: 6px;
}

.filter-dropdown-label {
  font-size: 13px;
  color: #505050;
  white-space: nowrap;
}

:deep(.filter-dropdown-wrapper .dx-selectbox) {
  background-color: transparent !important;
  border: none !important;
  box-shadow: none !important;
}

:deep(.filter-dropdown-wrapper .dx-textbox-input) {
  font-weight: 700 !important;
  color: #111 !important;
  padding-left: 0 !important;
}

/* Grid Container */
.system-modal-grid-container {
  flex: 1;
  position: relative;
  border: 1px solid var(--misa-border-color);
  border-radius: 4px;
  overflow: hidden;
}

.grid-loading-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.6);
  z-index: 100;
  display: flex;
  align-items: center;
  justify-content: center;
}

.misa-spinner {
  width: 32px;
  height: 32px;
  border: 3px solid rgba(40, 167, 69, 0.2);
  border-radius: 50%;
  border-top-color: var(--primary-green);
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

:deep(.misa-system-grid .dx-datagrid-headers .dx-header-row > td) {
  border-bottom: 1px solid var(--misa-border-color) !important;
  border-right: 1px solid var(--misa-border-color) !important;
  background-color: #f2f2f2 !important;
  font-weight: 700;
  color: var(--misa-text-title);
  font-size: 13px;
}

:deep(.misa-system-grid .dx-datagrid-rowsview .dx-data-row > td) {
  border-bottom: 1px solid var(--misa-border-color) !important;
  border-right: none !important;
  padding: 0 12px !important;
  height: 36px !important;
  font-size: 13px;
}

:deep(.misa-system-grid .dx-datagrid-rowsview .dx-data-row:hover) {
  background-color: #ebf9eb !important;
}

/* Footer */
.system-modal-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 20px 12px 20px;
  border-top: none;
  background-color: transparent;
  flex-shrink: 0;
}

.footer-left {
  font-size: 13px;
  color: #333;
}

.total-count-text {
  font-weight: 700;
}

.footer-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

.pagination-pagesize-container {
  display: flex;
  align-items: center;
  gap: 8px;
}

.pagesize-label-text {
  font-size: 13px;
  color: #666;
}

.pagination-info-text {
  font-size: 13px;
  color: #333;
}

.pagination-navigation-controls {
  display: flex;
  align-items: center;
  gap: 16px;
}

.pagination-nav-btn {
  background: transparent;
  border: none;
  color: #666;
  font-size: 13px;
  cursor: pointer;
  padding: 0 4px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  user-select: none;
}

.pagination-nav-btn.disabled {
  color: #d0d0d0;
  cursor: not-allowed;
}

/* Buttons */
.system-modal-buttons {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 12px 16px;
  border-top: 1px solid #e0e0e0;
  background-color: #f4f5f8;
  flex-shrink: 0;
  border-bottom-left-radius: 8px;
  border-bottom-right-radius: 8px;
}

.btn-misa-modal-cancel {
  padding: 0 24px;
  height: 34px;
  border: 1px solid #e0e0e0;
  background-color: #fff;
  border-radius: var(--misa-border-radius);
  font-size: 13px;
  font-weight: 600;
  color: #333;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-misa-modal-cancel:hover {
  background-color: #f5f5f5;
}

.btn-misa-modal-agree {
  padding: 0 24px;
  height: 34px;
  border: none;
  background-color: #2ca01c;
  border-radius: var(--misa-border-radius);
  font-size: 13px;
  font-weight: 600;
  color: #fff;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-misa-modal-agree:hover {
  background-color: #248216;
}
</style>

