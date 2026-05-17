<script setup lang="ts">
import 'devextreme/dist/css/dx.fluent.blue.light.css';

import {computed, ref, watch} from "vue";
import {toast} from "@/services/toast.ts";
import {type SalaryCompositions, getAttributeName, getValueTypeName} from "@/views/ms-salary-compositions/data.ts";
import salaryCompositionService from "@/services/salaryCompositionService.ts";
import SalaryCompositionForm from "./components/form/SalaryCompositionForm.vue";
import SalaryCompositionPopups from "./components/SalaryCompositionPopups.vue";
import {onMounted} from "vue";
import SalaryCompositionHeader from "@/views/ms-salary-compositions/components/SalaryCompositionHeader.vue";
import SalaryCompositionDataTable from "@/views/ms-salary-compositions/components/SalaryCompositionDataTable.vue";
import SalaryCompositionSystemDirectory from "./components/SalaryCompositionSystemDirectory.vue";
import gridConfigService from "@/services/gridConfigService.ts";
import MSPageLayout from "@/components/layout/ms-page-layout/MSPageLayout.vue";

const selectedIds = ref<string[]>([]);
const isLoading = ref(false);

const searchKeyword = ref("");
const tableData = ref<SalaryCompositions[]>([]);
const totalRecords = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const statusFilter = ref("all");

// <editor-fold> desc="Xu ly logic bang"
// <editor-fold desc="Fetch data"
/**
 * Fetch data tu backend len va trinhh bay du lieu
 */
const fetchData = async () => {
  isLoading.value = true;
  try {
    const pageable = {
      pageIndex: currentPage.value - 1,
      pageSize: pageSize.value
    };
    const filterRequest = {
      ColumnFilters: [] as any[]
    };

    // Log kiểm tra giá trị filter
    console.log("Current statusFilter value:", statusFilter.value);
    // Mac dinh luc dau fetch toan bo cac trang thai
    if (statusFilter.value !== "all" && statusFilter.value !== null && statusFilter.value !== undefined) {
      filterRequest.ColumnFilters.push({
        Column: "Status",
        Value: statusFilter.value.toString(),
        DataType: 0,
        FilterType: 4
      });
    }

    console.log("Sending filterRequest:", JSON.stringify(filterRequest, null, 2));
    const response = await salaryCompositionService.getFilter(pageable, filterRequest, true);
    if (response.data) {
      console.log(response.data)
      // map response body
      mapResponseBody(response.data.data);
      totalRecords.value = response.data.pageable.totalElements;
    }
  } catch (error) {
    console.error('Lỗi khi tải dữ liệu từ backend, sử dụng mock data:', error);
  } finally {
    isLoading.value = false;
  }
};

// <editor-fold> desc="Xu ly phan tim kiem"
/**
 * Xu ly tim kiem voi keyword
 */
const handleSearchByKeyword = async () => {
  try {
    const pageable = {
      pageIndex: currentPage.value - 1,
      pageSize: pageSize.value
    };
    const filterRequest = {
      Keyword: searchKeyword.value,
      ColumnFilters: [] as any[]
    };
    let response = await salaryCompositionService.getFilter(pageable, filterRequest, true);
    if (response.data) {
      mapResponseBody(response.data.data);
    }
  } catch (e: any) {
    toast.error('Co loi xay ra', e.message);
  }
}

/**
 * Theo doi bien searchKeyword. Neu co bat cu thay doi nao, kich hoat hanh vi fetch data
 */
watch(searchKeyword, async () => {
  await handleSearchByKeyword();
})
// </editor-fold>

const mapResponseBody = (items: any[]) => {
  tableData.value = items.map((item: any) => ({
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
  })) as any
}

/**
 * Fetch danh sac cac column cua trang Thanh phan luong
 */
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
    }
  } catch (error) {
    console.error('Lỗi khi tải cấu hình cột:', error);
  }
};

/**
 * Khoi tao lan dau du lieu
 */
onMounted(() => {
  fetchColumns();
  fetchData();
});
// </editor-fold>

/**
 * Theo doi trang thai cua cac bo loc va phan tim kiem
 */
watch([currentPage, pageSize, statusFilter], (newValues, oldValues) => {
  // Nếu statusFilter hoặc searchKeyword thay đổi, reset về trang 1
  if (oldValues && (newValues[2] !== oldValues[2] || newValues[3] !== oldValues[3])) {
    if (currentPage.value !== 1) {
      currentPage.value = 1;
      return;
    }
  }
  fetchData();
});

// reset ve trang 1
const handlePageSizeChange = () => {
  currentPage.value = 1;
  fetchData();
};

/**
 * Tinh toan index cua ban ghi trong trang hien tai
 * VD: Trang 2 => Ban ghi tu 11-20
 */
const pageInfo = computed(() => {
  const start = totalRecords.value > 0 ? (currentPage.value - 1) * pageSize.value + 1 : 0;
  const end = Math.min(currentPage.value * pageSize.value, totalRecords.value);
  return `${start} - ${end} / ${totalRecords.value} bản ghi`;
});
// </editor-fold>

const isConfirmModalOpen = ref(false);
const selectedComposition = ref<any>(null);

// <editor-fold> desc="Xu ly nghiep vu thay doi status cua ban ghi"
/**
 * Xu ly dong mo modal
 * @param data
 */
const handleActive = (data: any) => {
  selectedComposition.value = data;
  isConfirmModalOpen.value = true;
};

/**
 * Xu ly su thay doi trang thai theo doi (Dang theo doi <-> Ngung theo doi)
 * @param data
 */
const handleChangeStatus = async (data: any) => {
  if (!data) return;
  try {
    const newStatus = data.status === 1 ? 0 : 1;
    const requestData = {
      SalaryComponentCode: data.componentCode,
      SalaryComponentName: data.componentName,
      AppliedUnitId: data.appliedUnitId,
      SalaryComponentSystemId: data.salaryComponentSystemId,
      Attribute: data.attribute,
      ValueType: data.valueType,
      Value: data.value === '-' ? null : data.value,
      Status: newStatus,
      Source: data.source
    };

    await salaryCompositionService.update(data.componentId, requestData);
    toast.success('Thành công', `Đã chuyển trạng thái thành công cho ${data.componentName}`);
    await fetchData();
  } catch (e: any) {
    console.error(e);
    toast.error('Có lỗi xảy ra', `Có lỗi xảy ra khi cập nhật trạng thái: ${e.message}`);
  }
  closeConfirmModal();
};
// </editor-fold>

const closeConfirmModal = () => {
  isConfirmModalOpen.value = false;
  selectedComposition.value = null;
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

/**
 * Luu du lieu ve backend
 * @param formData
 * @param stayOpen
 */
const handleSaveForm = async (formData: any, stayOpen = false) => {
  isLoading.value = true;
  try {
    // Map dữ liệu
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
      toast.success(`${content} thành công`, `${content} thành công ${requestData.SalaryComponentName}`);
    } else {
      // Khi sửa, dùng ID thực của bản ghi (nếu có trong formData hoặc state)
      const id = selectedRowId.value || formData.salaryComponentId;
      await salaryCompositionService.update(id, requestData);
      toast.success('Cập nhật thành công', `Cập nhật thành công ${requestData.SalaryComponentName}`);
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

// <editor-fold> desc="Xu ly thao tac xoa"
/**
 * Mo modal xac nhan xoa
 * @param data
 */
const handleDelete = (data: any) => {
  pendingDeleteData.value = data;
  deleteType.value = 'single';
  deleteModalMessage.value = `Bạn có chắc chắn muốn xóa thành phần lương <${data.componentName}> không?`;
  isDeleteModalOpen.value = true;
};

/**
 * Mo modal xac nhan xoa nhieu
 */
const handleDeleteSelected = () => {
  if (selectedIds.value.length === 0) return;
  deleteType.value = 'multiple';
  deleteModalMessage.value = 'Bạn có chắc chắn muốn xóa các thành phần lương đã chọn không?';
  isDeleteModalOpen.value = true;
};

/**
 * Xac nhan xoa. Gui danh sach id can xoa ve backend
 */
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
// </editor-fold>

// <editor-fold> desc="Xu ly nut chevron down 'Them moi'"
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
// </editor-fold>

const addSystemComposition = async (data: any) => {
  debugger;
  try {
    // Map du lieu vao request payload
    const requestData = {
      SalaryComponentCode: data.salaryComponentCode,
      SalaryComponentName: data.salaryComponentName,
      SalaryComponentSystemId: data.salaryComponentSystemId,
      Attribute: data.attribute,
      ValueType: data.valueType,
      Value: data.value === '-' ? null : data.value,
      Status: 1,
      Source: 'Hệ thống',
      AppliedUnitId: data.appliedUnitId,
      IsUsed: true
    };
    debugger;
    await salaryCompositionService.update(data.salaryComponentId, requestData);
    toast.success('Thêm thành công', `Đã thêm thành phần ${data.salaryComponentName} từ hệ thống`);
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
    <MSPageLayout v-if="!isFormVisible">
      <template #header-left>
        <div class="content_header_title">Thành phần lương</div>
      </template>

      <template #header-right>
        <SalaryCompositionHeader
            :is-add-dropdown-visible="isAddDropdownVisible"
            @add="handleAdd"
            @toggle-dropdown="toggleAddDropdown"
            @add-from-system="handleAddFromSystem"
            @open-system="handleAddFromSystem"
        />
      </template>

      <template #body>
        <SalaryCompositionDataTable
            v-model:searchKeyword="searchKeyword"
            v-model:selectedIds="selectedIds"
            v-model:currentPage="currentPage"
            v-model:pageSize="pageSize"
            :table-data="tableData"
            :total-records="totalRecords"
            :columns="columns"
            :page-info="pageInfo"
            v-model:statusFilter="statusFilter"
            @handlePageSizeChange="handlePageSizeChange"
            @handleOpenConfig="handleOpenConfig"
            @togglePin="togglePin"
            @handleActive="handleActive"
            @handleDuplicate="handleDuplicate"
            @handleEdit="handleEdit"
            @handleDelete="handleDelete"
            @deleteSelected="handleDeleteSelected"
        />
      </template>
    </MSPageLayout>

    <!-- Form component overlay -->
    <SalaryCompositionForm
        v-if="isFormVisible"
        :key="formKey"
        :mode="formMode"
        :initial-data="formInitialData"
        @close="closeForm"
        @save="handleSaveForm"
    />

    <!-- Popups (Confirm & Column Config) -->
    <SalaryCompositionPopups
        v-model:isConfirmVisible="isConfirmModalOpen"
        v-model:isConfigVisible="isColumnConfigVisible"
        v-model:isDeleteVisible="isDeleteModalOpen"
        :delete-message="deleteModalMessage"
        v-model:columns="columns"
        :selected-composition="selectedComposition"
        @confirmActive="handleChangeStatus"
        @confirmDelete="confirmDelete"
        @closeConfirm="closeConfirmModal"
        @closeConfig="closeConfig"
    />
  </template>

  <SalaryCompositionSystemDirectory
      v-else
      @back="closeSystemDirectory"
      @addSystem="addSystemComposition"
  />
</template>

<style scoped src="./style.css"></style>
