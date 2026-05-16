<script setup lang="ts">
import {ref, onMounted, computed, watch} from 'vue';
import SalaryCompositionDataTable from "@/views/ms-salary-compositions/components/SalaryCompositionDataTable.vue";
import salaryCompositionService from "@/services/salaryCompositionService.ts";
import salaryCompositionSystemService from "@/services/salaryCompositionSystemService.ts";
import gridConfigService from "@/services/gridConfigService.ts";
import {getAttributeName, getValueTypeName} from "@/views/ms-salary-compositions/data.ts";

const emit = defineEmits(['back', 'addSystem']);

const isLoading = ref(false);
const searchKeyword = ref("");
const tableData = ref<any[]>([]);
const totalRecords = ref(0);
const currentPage = ref(1);
const pageSize = ref(25);
const selectedIds = ref<string[]>([]);
const systems = ref<any[]>([]);
const selectedSystemId = ref("all");

const fetchSystems = async () => {
  try {
    const response = await salaryCompositionSystemService.getAll();
    if (response.data) {
      systems.value = [
        { salaryComponentSystemId: 'all', salaryComponentSystemName: 'Tất cả thành phần' },
        ...response.data
      ];
    }
  } catch (error) {
    console.error('Lỗi khi lấy danh mục hệ thống, sử dụng mock:', error);
  }
};

const fetchData = async () => {
  isLoading.value = true;
  try {
    const pageable = {
      pageIndex: currentPage.value - 1,
      pageSize: pageSize.value
    };
    
    const columnFilters: any[] = [
      { Column: 'Source', Value: 'Hệ thống' }
    ];

    if (selectedSystemId.value !== 'all') {
      columnFilters.push({ 
        Column: 'SalaryComponentSystemId', 
        Value: selectedSystemId.value 
      });
    }

    const filterRequest = {
      Keyword: searchKeyword.value,
      ColumnFilters: columnFilters
    };

    const response = await salaryCompositionService.getFilter(pageable, filterRequest);
    
    if (response.data && response.data.data) {
      tableData.value = response.data.data.map((item: any) => ({
        componentId: item.salaryComponentId,
        componentCode: item.salaryComponentCode,
        componentName: item.salaryComponentName,
        salaryComponentSystemId: item.salaryComponentSystemId,
        salaryComponentSystemName: item.salaryComponentSystemName,
        attribute: item.attribute,
        valueType: item.valueType,
        value: item.value || '-',
        source: 'Hệ thống'
      }));
      totalRecords.value = response.data.pageable.totalElements;
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
                   (col.columnId === 'salary_component_name' ? 'componentName' : 
                    (col.columnId === 'salary_component_system_name' ? 'salaryComponentSystemName' : 
                     (col.columnId === 'value_type' ? 'valueType' : col.columnId))),
        caption: col.columnName,
        visible: col.isVisible === 1,
        width: col.width,
        isPinned: col.isPinned === 1,
        calculateCellValue: col.columnId === 'attribute' ? (data: any) => getAttributeName(data.attribute) :
                            col.columnId === 'value_type' ? (data: any) => getValueTypeName(data.valueType) : undefined
      }));
    }
  } catch (error) {
    console.error('Lỗi khi tải cấu hình cột hệ thống:', error);
  }
};

onMounted(() => {
  fetchSystems();
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

const handlePageSizeChange = () => {
  currentPage.value = 1;
  fetchData();
};

const columns = ref<any[]>([]);

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
        :system-items="systems"
        v-model:selectedSystemId="selectedSystemId"
        @handlePageSizeChange="handlePageSizeChange"
        @togglePin="togglePin"
        @addSystem="handleAddSystem"
        status-filter=""/>
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
