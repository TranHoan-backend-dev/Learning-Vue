<script setup lang="ts">
import 'devextreme/dist/css/dx.fluent.blue.light.css';

import { computed, ref, watch } from "vue";
import { toast } from "@/services/toast.ts";
import { type SalaryCompositions, getAttributeName, getValueTypeName } from "@/views/ms-salary-compositions/data.ts";
import salaryCompositionService from "@/services/salaryCompositionService.ts";
import SalaryCompositionForm from "./components/form/SalaryCompositionForm.vue";
import SalaryCompositionPopups from "./components/SalaryCompositionPopups.vue";
import { onMounted } from "vue";
import SalaryCompositionHeader from "@/views/ms-salary-compositions/components/SalaryCompositionHeader.vue";
import SalaryCompositionDataTable from "@/views/ms-salary-compositions/components/SalaryCompositionDataTable.vue";
import SalaryCompositionSystemDirectory from "./components/SalaryCompositionSystemDirectory.vue";
import gridConfigService from "@/services/gridConfigService.ts";
import { mockSalaryCompositions, mockSalaryCompositionColumns } from "./mock.ts";

const selectedIds = ref<string[]>([]);
const isLoading = ref(false);

const searchKeyword = ref("");
const tableData = ref<SalaryCompositions[]>([]);
const totalRecords = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const statusFilter = ref("all");

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

    // Log để kiểm tra giá trị filter
    console.log("Current statusFilter value:", statusFilter.value);

    if (statusFilter.value !== "all" && statusFilter.value !== null && statusFilter.value !== undefined) {
      filterRequest.ColumnFilters.push({
        Column: "Status",
        Value: statusFilter.value.toString(),
        DataType: 0,
        FilterType: 4
      });
    }

    console.log("Sending filterRequest:", JSON.stringify(filterRequest, null, 2));
    const response = await salaryCompositionService.getFilter(pageable, filterRequest);
    if (response.data) {
      console.log(response.data)
      // Backend trả về PagingData { Data: [], Pageable: { TotalRecords: ... } }
      tableData.value = response.data.data.map((item: any) => ({
        componentId: item.salaryComponentId,
        componentCode: item.salaryComponentCode,
        componentName: item.salaryComponentName,
        appliedUnitId: item.appliedUnitId,
        appliedUnitName: item.appliedUnitName,
        salaryComponentSystemId: item.salaryComponentSystemId,
        salaryComponentSystemName: item.salaryComponentSystemName,
        attribute: item.attribute,
        valueType: item.valueType,
        value: item.value || '-',
        status: item.status,
        source: item.source || 'Hệ thống'
      }));
      totalRecords.value = response.data.pageable.totalElements;
    } else {
      useMockData();
    }
  } catch (error) {
    console.error('Lỗi khi tải dữ liệu từ backend, sử dụng mock data:', error);
    useMockData();
  } finally {
    isLoading.value = false;
  }
};

// <editor-fold> desc="Dùng mock data
const useMockData = () => {
  let data = [...mockSalaryCompositions];

  // Áp dụng tìm kiếm
  if (searchKeyword.value) {
    const kw = searchKeyword.value.toLowerCase();
    data = data.filter(item =>
      item.salaryComponentCode.toLowerCase().includes(kw) ||
      item.salaryComponentName.toLowerCase().includes(kw)
    );
  }

  // Áp dụng lọc trạng thái
  if (statusFilter.value !== 'all') {
    data = data.filter(item => item.status === Number(statusFilter.value));
  }

  totalRecords.value = data.length;

  // Áp dụng phân trang
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  tableData.value = data.slice(start, end).map(item => ({
    componentId: item.salaryComponentId,
    componentCode: item.salaryComponentCode,
    componentName: item.salaryComponentName,
    appliedUnitId: item.appliedUnitId,
    appliedUnitName: item.appliedUnitName,
    salaryComponentSystemId: item.salaryComponentSystemId,
    salaryComponentSystemName: item.salaryComponentSystemName,
    attribute: item.attribute,
    valueType: item.valueType,
    value: item.value,
    status: item.status,
    source: item.source
  }));
};

const useMockColumns = () => {
  columns.value = mockSalaryCompositionColumns.map(col => ({
    ...col,
    calculateCellValue: col.dataField === 'attribute' ? (data: any) => getAttributeName(data.attribute) :
      col.dataField === 'value_type' ? (data: any) => getValueTypeName(data.valueType) : undefined
  }));
};
// </editor-fold>

const fetchColumns = async () => {
  try {
    const response = await gridConfigService.getByGridId('SalaryComponentGrid');
    if (response.data && response.data.length > 0) {
      columns.value = response.data.map((col: any) => ({
        dataField: col.columnId === 'salary_component_code' ? 'componentCode' :
          (col.columnId === 'salary_component_name' ? 'componentName' :
            (col.columnId === 'applied_unit_name' ? 'appliedUnitName' :
              (col.columnId === 'salary_component_system_name' ? 'salaryComponentSystemName' :
                (col.columnId === 'value_type' ? 'valueType' :
                  (col.columnId === 'attribute' ? 'attribute' : col.columnId))))),
        caption: col.columnName,
        visible: col.isVisible === 1,
        width: col.width,
        isPinned: col.isPinned === 1,
        cellTemplate: col.columnId === 'status' ? 'status-cell' : undefined,
        calculateCellValue: col.columnId === 'attribute' ? (data: any) => getAttributeName(data.attribute) :
          col.columnId === 'value_type' ? (data: any) => getValueTypeName(data.valueType) : undefined
      }));
    } else {
      useMockColumns();
    }
  } catch (error) {
    console.error('Lỗi khi tải cấu hình cột:', error);
    useMockColumns();
  }
};

onMounted(() => {
  fetchColumns();
  fetchData();
});

watch([currentPage, pageSize, searchKeyword, statusFilter], (newValues, oldValues) => {
  // Nếu statusFilter hoặc searchKeyword thay đổi, reset về trang 1
  if (oldValues && (newValues[2] !== oldValues[2] || newValues[3] !== oldValues[3])) {
    if (currentPage.value !== 1) {
      currentPage.value = 1;
      return;
    }
  }
  fetchData();
});

const handlePageSizeChange = () => {
  currentPage.value = 1;
  fetchData();
};

const pageInfo = computed(() => {
  const start = totalRecords.value > 0 ? (currentPage.value - 1) * pageSize.value + 1 : 0;
  const end = Math.min(currentPage.value * pageSize.value, totalRecords.value);
  return `${start} - ${end} / ${totalRecords.value} bản ghi`;
});

const isConfirmModalOpen = ref(false);
const selectedComposition = ref<any>(null);

const handleActive = (data: any) => {
  selectedComposition.value = data;
  isConfirmModalOpen.value = true;
};

const closeConfirmModal = () => {
  isConfirmModalOpen.value = false;
  selectedComposition.value = null;
};

const confirmActive = () => {
  alert('Đã chuyển trạng thái thành công!');
  closeConfirmModal();
};

const isFormVisible = ref(false);
const formMode = ref<'add' | 'edit' | 'copy'>('add');
const formInitialData = ref<any>(null);
const selectedRowId = ref<string | null>(null);
const formKey = ref(0);

const handleAdd = () => {
  formKey.value = Date.now();
  formMode.value = 'add';
  formInitialData.value = null;
  selectedRowId.value = null;
  isFormVisible.value = true;
};

const handleEdit = (data: any) => {
  formKey.value = Date.now();
  formMode.value = 'edit';
  selectedRowId.value = data.componentId;
  formInitialData.value = {
    ...data,
    componentCode: data.componentCode
  };
  isFormVisible.value = true;
};

const handleDuplicate = (data: any) => {
  formKey.value = Date.now();
  formMode.value = 'copy';
  formInitialData.value = {
    ...data,
    componentCode: data.componentCode
  };
  isFormVisible.value = true;
};

const closeForm = () => {
  isFormVisible.value = false;
  formInitialData.value = null;
};

const handleSaveForm = async (formData: any, stayOpen = false) => {
  isLoading.value = true;
  try {
    // Map dữ liệu từ form sang đúng Model của Backend (PascalCase)
    const requestData = {
      SalaryComponentCode: formData.componentCode,
      SalaryComponentName: formData.componentName,
      AppliedUnitId: formData.appliedUnitId,
      SalaryComponentSystemId: formData.salaryComponentSystemId,
      Attribute: formData.attribute,
      ValueType: formData.valueType,
      Value: formData.valueFormula || formData.quota,
      Status: 1, // Mặc định: Đang sử dụng
      Source: 'Tự thêm'
    };

    if (formMode.value === 'add' || formMode.value === 'copy') {
      await salaryCompositionService.create(requestData);
      var content = formMode.value === 'copy' ? 'Nhân bản' : 'Thêm mới';
      toast.success(`${content} thành công`, `${content} thành công ${requestData.salaryComponentName}`);
    } else {
      // Khi sửa, dùng ID thực của bản ghi (nếu có trong formData hoặc state)
      const id = selectedRowId.value || formData.salaryComponentId;
      await salaryCompositionService.update(id, requestData);
      toast.success('Cập nhật thành công', `Cập nhật thành công ${requestData.salaryComponentName}`);
    }
    if (!stayOpen) {
      isFormVisible.value = false;
      formInitialData.value = null;
      selectedRowId.value = null;
    } else {
      formKey.value = Date.now();
    }
    await fetchData();
  } catch (error) {
    console.error(error);
    toast.error('Lỗi khi lưu dữ liệu', 'Đã có lỗi xảy ra');
  } finally {
    isLoading.value = false;
  }
};

const isDeleteModalOpen = ref(false);
const deleteModalMessage = ref('');
const deleteType = ref<'single' | 'multiple'>('single');
const pendingDeleteData = ref<any>(null);

const handleDelete = (data: any) => {
  pendingDeleteData.value = data;
  deleteType.value = 'single';
  deleteModalMessage.value = `Bạn có chắc chắn muốn xóa thành phần lương <${data.componentName}> không?`;
  isDeleteModalOpen.value = true;
};

const handleDeleteSelected = () => {
  if (selectedIds.value.length === 0) return;
  deleteType.value = 'multiple';
  deleteModalMessage.value = 'Bạn có chắc chắn muốn xóa các thành phần lương đã chọn không?';
  isDeleteModalOpen.value = true;
};

const confirmDelete = async () => {
  isDeleteModalOpen.value = false;
  isLoading.value = true;
  try {
    if (deleteType.value === 'single' && pendingDeleteData.value) {
      await salaryCompositionService.delete(pendingDeleteData.value.componentId);
      toast.success('Xóa thành công', `Đã xóa thành phần lương ${pendingDeleteData.value.componentName}`);
    } else {
      await salaryCompositionService.deleteMany(selectedIds.value);
      toast.success('Xóa thành công', `Đã xóa ${selectedIds.value.length} bản ghi được chọn`);
      selectedIds.value = [];
    }
    fetchData();
  } catch (error) {
    console.error('Lỗi khi xóa:', error);
    toast.error('Lỗi', 'Có lỗi xảy ra khi thực hiện thao tác xóa');
  } finally {
    isLoading.value = false;
    pendingDeleteData.value = null;
  }
};

const isAddDropdownVisible = ref(false);

const toggleAddDropdown = (e: any) => {
  e.stopPropagation();
  isAddDropdownVisible.value = !isAddDropdownVisible.value;
};

// Đóng dropdown khi click ra ngoài
window.addEventListener('click', () => {
  isAddDropdownVisible.value = false;
});

const isSystemDirectoryVisible = ref(false);

const handleAddFromSystem = () => {
  console.log('Switching to System Directory view...');
  isSystemDirectoryVisible.value = true;
  isAddDropdownVisible.value = false;
};

const closeSystemDirectory = () => {
  isSystemDirectoryVisible.value = false;
};

const addSystemComponent = async (data: any) => {
  try {
    // Khi thêm từ hệ thống, ta cần map lại các trường cho đúng với Model của Backend (PascalCase)
    const requestData = {
      SalaryComponentCode: data.componentCode,
      SalaryComponentName: data.componentName,
      SalaryComponentSystemId: data.salaryComponentSystemId,
      Attribute: data.attribute,
      ValueType: data.valueType,
      Value: data.value,
      Status: 1,
      Source: 'Hệ thống'
    };
    await salaryCompositionService.create(requestData);
    toast.success('Thêm thành công', `Đã thêm thành phần ${data.componentName} từ hệ thống`);
    await fetchData();
  } catch (error) {
    console.error(error);
    toast.error('Lỗi', 'Không thể thêm thành phần từ hệ thống');
  }
};

// Column configuration
const columns = ref<any[]>([]);

const isColumnConfigVisible = ref(false);

const togglePin = (e: any, dataField: string) => {
  e.stopPropagation();
  const column = columns.value.find(col => col.dataField === dataField);
  if (column) {
    const newState = !column.isPinned;
    // Reset all pins first to ensure only one column is marked as the "pin point"
    columns.value.forEach(col => {
      col.isPinned = false;
    });
    column.isPinned = newState;
  }
};

const handleOpenConfig = () => {
  isColumnConfigVisible.value = true;
};

const closeConfig = () => {
  isColumnConfigVisible.value = false;
};
toast.success('Đăng nhập thành công', 'Chào mừng đến với hệ thống', 5000);
</script>

<template>
  <template v-if="!isSystemDirectoryVisible">
    <section v-if="!isFormVisible" class="content">
      <!-- Title danh sách -->
      <SalaryCompositionHeader :is-add-dropdown-visible="isAddDropdownVisible" @add="handleAdd"
        @toggle-dropdown="toggleAddDropdown" @add-from-system="handleAddFromSystem"
        @open-system="handleAddFromSystem" />

      <!-- Nội dung bảng -->
      <SalaryCompositionDataTable v-model:searchKeyword="searchKeyword" v-model:selectedIds="selectedIds"
        v-model:currentPage="currentPage" v-model:pageSize="pageSize" :table-data="tableData"
        :total-records="totalRecords" :columns="columns" :page-info="pageInfo" v-model:statusFilter="statusFilter"
        @handlePageSizeChange="handlePageSizeChange" @handleOpenConfig="handleOpenConfig" @togglePin="togglePin"
        @handleActive="handleActive" @handleDuplicate="handleDuplicate" @handleEdit="handleEdit"
        @handleDelete="handleDelete" @deleteSelected="handleDeleteSelected" />
    </section>

    <!-- Form component overlay -->
    <SalaryCompositionForm v-if="isFormVisible" :key="formKey" :mode="formMode" :initial-data="formInitialData"
      @close="closeForm" @save="handleSaveForm" />

    <!-- Popups (Confirm & Column Config) -->
    <SalaryCompositionPopups v-model:isConfirmVisible="isConfirmModalOpen"
      v-model:isConfigVisible="isColumnConfigVisible" v-model:isDeleteVisible="isDeleteModalOpen"
      :delete-message="deleteModalMessage" v-model:columns="columns" :selected-composition="selectedComposition"
      @confirmActive="confirmActive" @confirmDelete="confirmDelete" @closeConfirm="closeConfirmModal"
      @closeConfig="closeConfig" />
  </template>

  <SalaryCompositionSystemDirectory v-else @back="closeSystemDirectory" @addSystem="addSystemComponent" />
</template>

<style scoped src="./style.css"></style>
