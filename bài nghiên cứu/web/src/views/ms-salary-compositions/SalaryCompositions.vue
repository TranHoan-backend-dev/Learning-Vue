<script setup lang="ts">
import 'devextreme/dist/css/dx.fluent.blue.light.css';

import {computed, ref, watch} from "vue";
import {toast} from "@/services/toast.ts";
import {type SalaryCompositions} from "@/views/ms-salary-compositions/data.ts";
import salaryCompositionService from "@/services/salaryCompositionService.ts";
import SalaryCompositionForm from "./components/form/SalaryCompositionForm.vue";
import SalaryCompositionPopups from "./components/SalaryCompositionPopups.vue";
import {onMounted} from "vue";
import SalaryCompositionHeader from "@/views/ms-salary-compositions/components/SalaryCompositionHeader.vue";
import SalaryCompositionDataTable from "@/views/ms-salary-compositions/components/SalaryCompositionDataTable.vue";
import SalaryCompositionSystemDirectory from "./components/SalaryCompositionSystemDirectory.vue";

const selectedIds = ref<string[]>([]);
const isLoading = ref(false);

const searchKeyword = ref("");
const tableData = ref<SalaryCompositions[]>([]);
const totalRecords = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);

const fetchData = async () => {
  isLoading.value = true;
  try {
    const pageable = {
      pageIndex: currentPage.value - 1,
      pageSize: pageSize.value
    };
    const filterRequest = {
      keyword: searchKeyword.value,
      columnFilters: []
    };
    const response = await salaryCompositionService.getFilter(pageable, filterRequest);
    if (response.data) {
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
    }
  } catch (error) {
    toast.error('Lỗi khi tải dữ liệu', 'Đã có lỗi xảy ra');
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

onMounted(() => {
  fetchData();
});

watch([currentPage, pageSize], () => {
  fetchData();
});

watch(searchKeyword, () => {
  currentPage.value = 1;
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

const handleAdd = () => {
  formMode.value = 'add';
  formInitialData.value = null;
  selectedRowId.value = null;
  isFormVisible.value = true;
};

const handleEdit = (data: any) => {
  formMode.value = 'edit';
  selectedRowId.value = data.componentId;
  formInitialData.value = {
    ...data,
    componentId: data.componentCode // Map componentCode to componentId for Form
  };
  isFormVisible.value = true;
};

const handleDuplicate = (data: any) => {
  formMode.value = 'copy';
  formInitialData.value = {
    ...data,
    componentId: data.componentCode // Map componentCode to componentId for Form
  };
  isFormVisible.value = true;
};

const closeForm = () => {
  isFormVisible.value = false;
  formInitialData.value = null;
};

const handleSaveForm = async (formData: any) => {
  isLoading.value = true;
  try {
    // Map dữ liệu từ form sang DTO của Backend
    const requestData = {
      salaryComponentCode: formData.componentId,
      salaryComponentName: formData.componentName,
      appliedUnitId: formData.appliedUnitId,
      salaryComponentSystemId: formData.salaryComponentSystemId,
      attribute: formData.attribute,
      valueType: formData.valueType,
      value: formData.valueFormula,
      status: 1, // Mặc định: Đang sử dụng
      source: 'Tự thêm'
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
    isFormVisible.value = false;
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
    // When adding from system, we create a new component for the user
    const requestData = {
      ...data,
      salaryComponentId: undefined, // Let backend generate new ID
      source: 'Hệ thống'
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
const columns = ref([
  {dataField: 'componentCode', caption: 'Mã thành phần', visible: true, width: 150, isPinned: false},
  {dataField: 'componentName', caption: 'Tên thành phần', visible: true, width: 250, isPinned: false},
  {dataField: 'appliedUnitName', caption: 'Đơn vị áp dụng', visible: true, width: 200, isPinned: false},
  {dataField: 'salaryComponentSystemName', caption: 'Loại thành phần', visible: true, width: 150, isPinned: false},
  {dataField: 'attribute', caption: 'Tính chất', visible: true, width: 120, isPinned: false},
  {dataField: 'valueType', caption: 'Kiểu giá trị', visible: true, width: 120, isPinned: false},
  {dataField: 'value', caption: 'Giá trị', visible: true, width: 200, isPinned: false},
  {dataField: 'source', caption: 'Nguồn tạo', visible: true, width: 150, isPinned: false},
  {dataField: 'status', caption: 'Trạng thái', visible: true, width: 150, isPinned: false, cellTemplate: 'status-cell'},
]);

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
      <SalaryCompositionHeader
          :is-add-dropdown-visible="isAddDropdownVisible"
          @add="handleAdd"
          @toggle-dropdown="toggleAddDropdown"
          @add-from-system="handleAddFromSystem"
          @open-system="handleAddFromSystem"/>

      <!-- Nội dung bảng -->
      <SalaryCompositionDataTable
          v-model:searchKeyword="searchKeyword"
          v-model:selectedIds="selectedIds"
          v-model:currentPage="currentPage"
          v-model:pageSize="pageSize"
          :table-data="tableData"
          :total-records="totalRecords"
          :columns="columns"
          :page-info="pageInfo"
          @handlePageSizeChange="handlePageSizeChange"
          @handleOpenConfig="handleOpenConfig"
          @togglePin="togglePin"
          @handleActive="handleActive"
          @handleDuplicate="handleDuplicate"
          @handleEdit="handleEdit"
          @handleDelete="handleDelete"
          @deleteSelected="handleDeleteSelected"/>
    </section>

    <!-- Form component overlay -->
    <SalaryCompositionForm v-if="isFormVisible" :mode="formMode" :initial-data="formInitialData" @close="closeForm"
                           @save="handleSaveForm"/>

    <!-- Popups (Confirm & Column Config) -->
    <SalaryCompositionPopups v-model:isConfirmVisible="isConfirmModalOpen"
                             v-model:isConfigVisible="isColumnConfigVisible"
                             v-model:isDeleteVisible="isDeleteModalOpen" :delete-message="deleteModalMessage"
                             v-model:columns="columns"
                             :selected-composition="selectedComposition" @confirmActive="confirmActive"
                             @confirmDelete="confirmDelete"
                             @closeConfirm="closeConfirmModal" @closeConfig="closeConfig"/>
  </template>

  <SalaryCompositionSystemDirectory
      v-else
      @back="closeSystemDirectory"
      @addSystem="addSystemComponent"
  />
</template>

<style scoped src="./style.css"></style>
