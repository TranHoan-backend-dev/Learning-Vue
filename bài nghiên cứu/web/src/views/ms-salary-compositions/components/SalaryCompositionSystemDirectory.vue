<script setup lang="ts">
import {ref, onMounted, computed, watch} from 'vue';
import SalaryCompositionDataTable from "@/views/ms-salary-compositions/components/SalaryCompositionDataTable.vue";
import salaryCompositionService from "@/services/salaryCompositionService.ts";
import gridConfigService from "@/services/gridConfigService.ts";
import {getAttributeName, getValueTypeName} from "@/views/ms-salary-compositions/data.ts";
import MSPageLayout from "@/components/layout/ms-page-layout/MSPageLayout.vue";
import {toast} from "@/services/toast.ts";

const emit = defineEmits(['back']);

const isLoading = ref(false);
const searchKeyword = ref("");
const tableData = ref<any[]>([]);
const totalRecords = ref(0);
const currentPage = ref(1);
const pageSize = ref(25);
const selectedIds = ref<string[]>([]);
const systems = ref<any[]>([]);
const selectedSystemId = ref("all");
const columns = ref<any[]>([]);

const fetchData = async () => {
  isLoading.value = true;
  try {
    const pageable = {
      pageIndex: currentPage.value - 1,
      pageSize: pageSize.value
    };

    const filterRequest = {
      Keyword: searchKeyword.value,
      ColumnFilters: []
    };

    const response = await salaryCompositionService.getFilter(pageable, filterRequest, false);

    if (response.data && response.data.data) {
      tableData.value = response.data.data.map((item: any) => ({
        ...item,
        componentId: item.salaryComponentId,
        componentCode: item.salaryComponentCode,
        salaryComponentSystemName: item.salaryComponentName,
        categoryName: 'Hệ thống',
        source: 'Hệ thống'
      }));
      totalRecords.value = response.data.pageable?.totalElements || response.data.data.length;
    }
  } catch (error) {
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

const fetchColumns = async () => {
  try {
    const response = await gridConfigService.getByGridId('SalaryComponentSystemGrid');
    if (response.data && response.data.length > 0) {
      columns.value = response.data.map((col: any) => ({
        dataField: col.columnId === 'salary_component_code' ? 'componentCode' :
            (col.columnId === 'componentName' ? 'salaryComponentSystemName' :
                (col.columnId === 'value_type' ? 'valueType' : col.columnId)),
        caption: col.columnName,
        visible: col.isVisible === 1,
        width: col.width,
        isPinned: col.isPinned === 1,
        calculateCellValue: col.columnId === 'attribute' ? (data: any) => getAttributeName(data.attribute) :
            col.columnId === 'value_type' ? (data: any) => getValueTypeName(data.valueType) : undefined
      }))
    }
  } catch (error) {
    console.error('Lỗi khi tải cấu hình cột hệ thống:', error);
  }
};

onMounted(() => {
  fetchColumns();
  fetchData();
});

watch(selectedSystemId, () => {
  currentPage.value = 1;
  fetchData();
});

watch([currentPage, pageSize], fetchData);

watch(searchKeyword, () => {
  currentPage.value = 1;
  fetchData();
});

/**
 * Xử ly su thay doi cua co bang
 */
const handlePageSizeChange = () => {
  currentPage.value = 1;
  fetchData();
};

const pageInfo = computed(() => {
  const start = totalRecords.value > 0 ? (currentPage.value - 1) * pageSize.value + 1 : 0;
  const end = Math.min(currentPage.value * pageSize.value, totalRecords.value);
  return `${start} - ${end} / ${totalRecords.value} bản ghi`;
});

const togglePin = (e: any, dataField: string) => {
  e.stopPropagation();
  const column = columns.value.find(col => col.dataField === dataField);
  if (column) {
    column.isPinned = !column.isPinned;
  }
};

const handleAddSystem = (data: any) => {
  addSystemComposition(data);
  fetchData();
};

/**
 * Xu ly viec chuyen doi trang thai is_used cua ban ghi
 * @param data
 */
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
      AppliedUnitIds: data.appliedUnitIds,
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
</script>

<template>
  <MSPageLayout>
    <template #header-left>
      <div class="back_button_container" @click="emit('back')">
        <div class="mi_icon_back"></div>
      </div>
      <div class="content_header_title">Danh mục thành phần lương của hệ thống</div>
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
          :is-system-mode="true"
          :system-items="systems"
          v-model:selectedSystemId="selectedSystemId"
          @handlePageSizeChange="handlePageSizeChange"
          @togglePin="togglePin"
          @addSystem="handleAddSystem"
          status-filter=""/>
    </template>
  </MSPageLayout>
</template>

<style scoped src="../style.css"></style>
<style scoped>
.back_button_container {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  margin-right: 8px;
  border-radius: 4px;
}

.back_button_container:hover {
  background-color: #ebedf0;
}

.mi_icon_back {
  width: 24px;
  height: 24px;
  background-image: url(https://amisplatform.misacdn.net/apps/recruit/assets/images/ICON.svg);
  background-position: -70px -798px;
  background-repeat: no-repeat;
}

</style>
