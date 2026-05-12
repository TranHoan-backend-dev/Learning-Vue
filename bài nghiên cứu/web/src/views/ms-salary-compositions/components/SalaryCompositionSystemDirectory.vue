<script setup lang="ts">
import {ref, onMounted, computed, watch} from 'vue';
import SalaryCompositionDataTable from "@/views/ms-salary-compositions/components/SalaryCompositionDataTable.vue";
import salaryCompositionService from "@/services/salaryCompositionService.ts";
import {toast} from "@/services/toast.ts";
import {systemDirectoryColumns} from "@/views/ms-salary-compositions/data.ts";

const emit = defineEmits(['back', 'addSystem']);

const isLoading = ref(false);
const searchKeyword = ref("");
const tableData = ref<any[]>([]);
const totalRecords = ref(0);
const currentPage = ref(1);
const pageSize = ref(25);
const selectedIds = ref<string[]>([]);

const mockData = [
  {
    salaryComponentId: 'sys-1',
    salaryComponentCode: 'TY_LE_HOAN_THANH_KPI',
    salaryComponentName: 'Tỷ lệ hoàn thành KPI',
    salaryComponentSystemName: 'KPI',
    attribute: 'Khác',
    valueType: 'Phần trăm',
    value: '-',
    source: 'Hệ thống'
  },
  {
    salaryComponentId: 'sys-2',
    salaryComponentCode: 'TY_LE_HOAN_THANH_DOANH_SO',
    salaryComponentName: 'Tỷ lệ hoàn thành doanh số',
    salaryComponentSystemName: 'Doanh số',
    attribute: 'Khác',
    valueType: 'Phần trăm',
    value: '=DOANH_SO_THUC_T...',
    source: 'Hệ thống'
  },
  {
    salaryComponentId: 'sys-3',
    salaryComponentCode: 'TONG_GIO_LAM_THEM_HUONG_LUONG_THU_VIEC',
    salaryComponentName: 'Tổng giờ làm thêm hưởng lương thử việc',
    salaryComponentSystemName: 'Chấm công',
    attribute: 'Khác',
    valueType: 'Số',
    value: '-',
    source: 'Hệ thống'
  },
  {
    salaryComponentId: 'sys-4',
    salaryComponentCode: 'TONG_GIO_LAM_THEM_HUONG_LUONG_KHAC',
    salaryComponentName: 'Tổng giờ làm thêm hưởng lương khác',
    salaryComponentSystemName: 'Chấm công',
    attribute: 'Khác',
    valueType: 'Số',
    value: '-',
    source: 'Hệ thống'
  },
  {
    salaryComponentId: 'sys-5',
    salaryComponentCode: 'TONG_GIO_LAM_THEM_HUONG_LUONG_HOC_VIEC',
    salaryComponentName: 'Tổng giờ làm thêm hưởng lương học việc',
    salaryComponentSystemName: 'Chấm công',
    attribute: 'Khác',
    valueType: 'Số',
    value: '-',
    source: 'Hệ thống'
  },
  {
    salaryComponentId: 'sys-6',
    salaryComponentCode: 'TONG_CONG_HUONG_LUONG_THEO_GIO',
    salaryComponentName: 'Tổng công hưởng lương theo giờ',
    salaryComponentSystemName: 'Chấm công',
    attribute: 'Khác',
    valueType: 'Số',
    value: '-',
    source: 'Hệ thống'
  }
];

const fetchData = async () => {
  isLoading.value = true;
  try {
    const pageable = {
      pageIndex: currentPage.value - 1,
      pageSize: pageSize.value
    };
    const filterRequest = {
      keyword: searchKeyword.value,
      columnFilters: [
        {column: 'Source', value: 'Hệ thống'}
      ]
    };
    const response = await salaryCompositionService.getFilter(pageable, filterRequest);
    if (response.data && response.data.data && response.data.data.length > 0) {
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
      // Sử dụng mock data nếu API trả về trống hoặc lỗi
      tableData.value = mockData.map(item => ({
        ...item,
        componentId: item.salaryComponentId,
        componentCode: item.salaryComponentCode,
        componentName: item.salaryComponentName
      }));
      totalRecords.value = mockData.length;
    }
  } catch (error) {
    // Dự phòng dữ liệu mẫu khi lỗi API
    tableData.value = mockData.map(item => ({
      ...item,
      componentId: item.salaryComponentId,
      componentCode: item.salaryComponentCode,
      componentName: item.salaryComponentName
    }));
    totalRecords.value = mockData.length;
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

onMounted(fetchData);

watch([currentPage, pageSize], fetchData);

watch(searchKeyword, () => {
  currentPage.value = 1;
  fetchData();
});

const handlePageSizeChange = () => {
  currentPage.value = 1;
  fetchData();
};

const columns = ref(systemDirectoryColumns);

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
  emit('addSystem', data);
};
</script>

<template>
  <section class="content">
    <div class="content_header">
      <div class="content_header_left">
        <div class="back_button_container" @click="emit('back')">
          <div class="mi_icon_back"></div>
        </div>
        <div class="content_header_title">Danh mục thành phần lương của hệ thống</div>
      </div>
    </div>

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
        @handlePageSizeChange="handlePageSizeChange"
        @togglePin="togglePin"
        @addSystem="handleAddSystem"/>
  </section>
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

.content {
  display: flex;
  flex-direction: column;
}

:deep(.content_body) {
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.content_header {
  height: 57px !important;
  /* Cố định chiều cao thấp hơn để nâng tiêu đề lên */
  padding-top: 0;
  padding-bottom: 0;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.content_header_left {
  display: flex;
  align-items: center;
}
</style>
