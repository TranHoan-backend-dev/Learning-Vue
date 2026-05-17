<script setup lang="ts">
import {computed, ref, onMounted} from "vue";
import DxSelectBox from "devextreme-vue/select-box";
import DxDataGrid, {
  DxColumn,
  DxHeaderFilter,
  DxPaging,
  DxScrolling,
  DxSelection,
  DxSorting
} from "devextreme-vue/data-grid";
import CustomPagination from "@/components/ui/ms-pagination/CustomPagination.vue";
import MSIcon from "@/components/ui/ms-icon/MSIcon.vue";
import {DxTooltip} from "devextreme-vue";
import MSStatusBadge from "@/components/ui/ms-status-badge/MSStatusBadge.vue";
import {
  type DataTableAttributes,
  pageSizeOptions,
  salaryCompositionStatus
} from "@/views/ms-salary-compositions/data.ts";
import organizationService from "@/services/organizationService.ts";

const props = defineProps<DataTableAttributes>();
const organization = ref<any[]>([]);

const emit = defineEmits([
  'update:selectedIds',
  'update:searchKeyword',
  'update:currentPage',
  'update:pageSize',
  'update:selectedSystemId',
  'handlePageSizeChange',
  'handleOpenConfig',
  'togglePin',
  'handleActive',
  'handleDuplicate',
  'handleEdit',
  'handleDelete',
  'deleteSelected',
  'update:statusFilter',
  'addSystem'
]);

/**
 * Phat su kien khi nguoi dung click vao cac checkbox. Truyen mang checkbox active nay ra ngoai
 * component cha
 * @param val
 */
const handleSelectedIdsChange = (val: string[]) => {
  emit('update:selectedIds', val);
};

// <editor-fold> desc="Xu ly phan trang + tim kiem"
/**
 * Phat su kien tim kiem dua tren keyword
 * @param e
 */
const handleSearchChange = (e: any) => {
  emit('update:searchKeyword', e.target.value);
};

/**
 * Phat su kien khi page index thay doi
 * @param val
 */
const handleCurrentPageChange = (val: number) => {
  emit('update:currentPage', val);
};

/**
 * Phat su kien page size thay doi
 * @param e
 */
const handlePageSizeValChange = (e: any) => {
  emit('update:pageSize', e.value);
  emit('handlePageSizeChange');
};
// </editor-fold>

// <editor-fold> desc="Xu ly viec ghim cot"
/*
 * Tinh toan cac cot can ghim
 */
const isFixed = (col: any) => {
  const pinnedIndex = props.columns.findIndex(c => c.isPinned);
  if (pinnedIndex === -1) return false;

  const currentColIndex = props.columns.findIndex(c => c.dataField === col.dataField);
  return currentColIndex !== -1 && currentColIndex <= pinnedIndex;
};

// Phat su kien togglePin
const handleTogglePin = (e: any, dataField: string) => {
  emit('togglePin', e, dataField);
};
// </editor-fold>

/**
 * Tinh toan va cap nhat data khi co thay doi
 */
const fullColumns = computed(() => {
  return props.columns.map((col, index) => ({
    ...col,
    fixed: isFixed(col),
    allowFiltering: false,
    allowSorting: false,
    visibleIndex: index + 1, // Dành vị trí 0 cho Checkbox
    isStt: false
  }));
});

/**
 * Fetch danh sach cac Don vi ap dung
 */
const loadAllOrganization = async () => {
  try {
    let res = await organizationService.getAll();
    if (res.data) {
      // Map lại dữ liệu để khớp với text/value của DxSelectBox
      organization.value = [
        {text: 'Tất cả đơn vị', value: 'all'},
        ...res.data.map((org: any) => ({
          text: org.organizationName,
          value: org.organizationId
        }))
      ];
    }
  } catch (error) {
    console.error("Error loading organizations:", error);
  }
}

onMounted(() => {
  loadAllOrganization();
});
</script>

<template>
  <div class="content_body">
    <div class="content_body_container">
      <!-- Title -->
      <div class="content_body_title">
        <template v-if="selectedIds.length > 0">

          <div class="content_body_header_left" style="display: flex; align-items: center; gap: 24px;">
            <div class="selected-info">
              Đã chọn <strong style="margin: 0 4px">{{ selectedIds.length }}</strong>
              <span class="unselect-link" @click="emit('update:selectedIds', [])">Bỏ chọn</span>
            </div>
            <button class="misa-btn-delete-bulk" @click="emit('deleteSelected')">
              <MSIcon name="trash" color="var(--misa-danger)"/>
              <span>Xóa</span>
            </button>
          </div>

          <div class="content_body_header_right">
          </div>
        </template>

        <template v-else>
          <div class="content_body_header_left">
            <div class="content_body_header_left_search">
              <input
                  type="text"
                  class="misa-search-input"
                  :value="searchKeyword"
                  @input="handleSearchChange"
                  placeholder="Tìm kiếm"
                  style="width: var(--misa-width-standard-control);"
              />
            </div>
          </div>

          <div class="content_body_header_right">
            <div class="content_body_header_right_filters">
              <template v-if="!isSystemMode">
                <!-- Lọc theo trạng thái -->
                <DxSelectBox
                    class="misa-selectbox"
                    :items="salaryCompositionStatus"
                    display-expr="text"
                    value-expr="value"
                    :value="statusFilter"
                    @value-changed="(e) => emit('update:statusFilter', e.value)"
                    :width="160"
                />
                <!-- Lọc theo đơn vi áp dung -->
                <DxSelectBox
                    class="misa-selectbox"
                    :items="organization"
                    display-expr="text"
                    value-expr="value"
                    value="all"
                    :width="320"/>
              </template>

              <template v-else>
                <DxSelectBox
                    class="misa-selectbox"
                    :items="systemItems || []"
                    display-expr="salaryComponentSystemName"
                    value-expr="salaryComponentSystemId"
                    :value="selectedSystemId"
                    @value-changed="(e) => emit('update:selectedSystemId', e.value)"
                    :width="200"
                />
              </template>
            </div>

            <div class="content_body_header_right_icon" @click="emit('handleOpenConfig')">
              <div class="mi_icon_setting"></div>
            </div>
          </div>
        </template>
      </div>

      <!-- Content table -->
      <div class="content_body_table">
        <div class="table_wrapper">
          <DxDataGrid
              :data-source="tableData"
              :show-borders="true"
              :row-alternation-enabled="true"
              :show-column-lines="false"
              :show-row-lines="false"
              key-expr="componentId"
              :selected-row-keys="selectedIds"
              @update:selected-row-keys="handleSelectedIdsChange"
              :column-auto-width="false"
              :allow-column-resizing="true"
              column-resizing-mode="nextColumn"
              width="100%"
              height="100%"
          >
            <DxScrolling
                mode="virtual"
                show-scrollbar="always"
                :use-native="true"
                :scroll-by-content="true"
                :scroll-by-thumb="true"
            />

            <DxPaging :enabled="false"/>

            <!-- Ghim cột Checkbox bên trái -->
            <DxColumn type="selection" :fixed="true" fixed-position="left" :visible-index="0" :width="50"/>
            <DxSelection mode="multiple" show-check-boxes-mode="always"/>

            <DxHeaderFilter :visible="false"/>
            <DxSorting mode="none"/>

            <template v-for="(col, index) in fullColumns" :key="col.dataField || index">
              <DxColumn
                  v-if="col.visible"
                  :data-field="col.dataField"
                  :caption="col.caption"
                  :width="col.width"
                  :alignment="col.alignment"
                  :fixed="col.fixed"
                  :calculate-cell-value="col.calculateCellValue"
                  :cell-template="col.dataField === 'value' ? 'valueTemplate' : (col.isStt ? undefined : col.cellTemplate)"
                  :header-cell-template="col.isStt ? undefined : 'headerTemplate'"
                  :allow-filtering="false"
                  :allow-sorting="false"
                  :visible-index="col.visibleIndex"
              />
            </template>

            <template #valueTemplate="{ data }">
              <div :class="{ 'formula-cell': data.value && data.value.toString().startsWith('=') }" :title="data.value">
                {{ data.value || '-' }}
              </div>
            </template>

            <!-- Cột Chức năng: Ghim phải, hiện khi hover -->
            <DxColumn
                v-if="!isSystemMode"
                caption=""
                cell-template="actionTemplate"
                alignment="right" :width="140"
                :fixed="true"
                fixed-position="right"
                css-class="col-action"
                :allow-filtering="false"
                :allow-sorting="false"
                :visible-index="1000"
            />

            <!-- Cột Thêm (Chỉ hiện trong mode Danh muc hệ thống) -->
            <DxColumn
                v-if="isSystemMode"
                caption=""
                cell-template="addSystemTemplate"
                alignment="right"
                :width="80"
                :fixed="true"
                fixed-position="right"
                css-class="col-action"
                :allow-filtering="false"
                :allow-sorting="false"
                :visible-index="1000"
            />

            <template #headerTemplate="{ data }">
              <div class="header-name-container">
                <span class="column-caption-text">{{ data.column.caption }}</span>
                <div
                    class="pin-icon"
                    :class="{ 'is-pinned': props.columns.find(c => c.dataField === data.column.dataField)?.isPinned }"
                    @click="handleTogglePin($event, data.column.dataField)"
                    title="Ghim cột"
                >
                  <MSIcon name="pin" size="16"/>
                </div>
              </div>
            </template>

            <template #status-cell="{ data }"><MSStatusBadge :status="data.value"/></template>

            <template #addSystemTemplate="{ data }">
              <div class="action-buttons add-system-action">
                <div
                    :id="`add-sys-${data.data.componentId}`"
                    class="action-btn"
                    @click="emit('addSystem', data.data)"
                >
                  <MSIcon name="plus" size="20" color="var(--primary-green)"/>
                  <DxTooltip
                      :target="`#add-sys-${data.data.componentId}`"
                      show-event="mouseenter"
                      hide-event="mouseleave"
                      position="top"
                  >
                    Đưa vào danh sách sử dụng
                  </DxTooltip>
                </div>
              </div>
            </template>

            <template #actionTemplate="{ data }">
              <div class="action-buttons">
                <!-- Nút Sử dụng (Tạm thời để tích xanh cho tất cả bản ghi) -->
                <div
                    :id="`active-${data.data.componentId}`"
                    class="action-btn action-active"
                    @click="emit('handleActive', data.data)"
                >
                  <MSIcon :name="data.data.status ? 'minus-circle' : 'check-circle'" :color="data.data.status ? '#ff8c00' : 'var(--primary-green)'"/>
                  <DxTooltip
                      :target="`#active-${data.data.componentId}`"
                      show-event="mouseenter"
                      hide-event="mouseleave"
                      position="top"
                  >
                    {{ data.data.status ? 'Ngừng theo dõi' : 'Theo dõi' }}
                  </DxTooltip>
                </div>

                <!-- Nút Nhân bản -->
                <div
                    :id="`copy-${data.data.componentId}`"
                    class="action-btn action-copy"
                    @click="emit('handleDuplicate', data.data)"
                >
                  <MSIcon name="copy" color="#5a5a5a"/>
                  <DxTooltip
                      :target="`#copy-${data.data.componentId}`"
                      show-event="mouseenter"
                      hide-event="mouseleave"
                      position="top"
                  >
                    Nhân bản
                  </DxTooltip>
                </div>

                <!-- Nút Sửa -->
                <div
                    :id="`edit-${data.data.componentId}`"
                    class="action-btn action-edit"
                    @click="emit('handleEdit', data.data)"
                >
                  <MSIcon name="edit" color="#5a5a5a"/>
                  <DxTooltip
                      :target="`#edit-${data.data.componentId}`"
                      show-event="mouseenter"
                      hide-event="mouseleave"
                      position="top"
                  >
                    Sửa
                  </DxTooltip>
                </div>

                <!-- Nút Xóa -->
                <div
                    :id="`delete-${data.data.componentId}`"
                    class="action-btn action-delete"
                    @click="emit('handleDelete', data.data)"
                >
                  <MSIcon name="trash" color="var(--misa-danger)"/>
                  <DxTooltip
                      :target="`#delete-${data.data.componentId}`"
                      show-event="mouseenter"
                      hide-event="mouseleave"
                      position="top"
                  >
                    Xóa
                  </DxTooltip>
                </div>
              </div>
            </template>
          </DxDataGrid>
        </div>
      </div>

      <div class="content_body_footer">
        <div class="content_body_footer_left">
          <div class="content_body_footer_total">
            Tổng: <strong id="totalRecords">{{ totalRecords }}</strong> bản ghi
          </div>
        </div>

        <div class="content_body_footer_right">
          <div class="content_body_footer_pagesize">
            <span class="paging_label">Số bản ghi trên trang</span>
            <div class="page_size_custom_select">
              <DxSelectBox
                  class="misa-selectbox"
                  :value="pageSize"
                  :items="pageSizeOptions"
                  display-expr="label"
                  value-expr="value"
                  :width="70"
                  @value-changed="handlePageSizeValChange"
              />
            </div>
          </div>
          <div class="content_body_footer_info">
            <span class="page_info" id="pageInfo">{{ pageInfo }}</span>
          </div>
          <div class="content_body_footer_nav">
            <CustomPagination
                :model-value="currentPage"
                @update:model-value="handleCurrentPageChange"
                :total="totalRecords"
                :page-size="pageSize"
                color="var(--primary-green)"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped src="../style.css"></style>
<style scoped>
.selected-info {
  display: flex;
  align-items: center;
  font-size: var(--misa-font-size-base);
}

.unselect-link {
  color: var(--primary-green);
  cursor: pointer;
  margin-left: 16px;
  font-weight: 500;
}

.unselect-link:hover {
  text-decoration: underline;
}

.misa-btn-delete-bulk {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 0 16px;
  height: var(--misa-control-height);
  border: 1px solid var(--misa-danger);
  border-radius: var(--misa-border-radius);
  background-color: var(--misa-white);
  color: var(--misa-danger);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.misa-btn-delete-bulk:hover {
  background-color: #fff1f0;
}

.dx-datagrid-headers .dx-header-row > td {
  border-bottom: 1px solid var(--misa-border-color) !important;
  border-right: 1px solid var(--misa-border-color) !important;
}

.dx-datagrid-rowsview .dx-data-row > td {
  border-bottom: 1px solid var(--misa-border-color) !important;
  border-right: 1px solid var(--misa-border-color) !important;
  padding: 0 12px !important;
  height: 32px !important;
}

/* Đảm bảo cột STT và cột checkbox cũng có border */
.dx-datagrid-rowsview .dx-data-row > td:first-child {
  border-left: 1px solid var(--misa-border-color) !important;
}

.dx-datagrid-headers .dx-header-row > td:first-child {
  border-left: 1px solid var(--misa-border-color) !important;
}

.formula-cell {
  color: #0070f3;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Cột action cố định theo thiết kế mới */
:deep(.dx-datagrid-rowsview .dx-data-row .col-action) {
  background-color: transparent !important;
  border-left: none !important;
  border-right: none !important;
  z-index: 10;
  pointer-events: auto;
}

/* Đảm bảo nội dung hàng bên dưới hiển thị màu so le */
:deep(.dx-datagrid-rowsview .dx-data-row) {
  background-color: #ffffff;
  height: 32px !important;
}

:deep(.dx-datagrid-rowsview .dx-data-row.dx-row-alt) {
  background-color: #f8f8f8;
}

/* Đảm bảo container chính luôn có màu so le chạy dài ra hết bảng */
:deep(.dx-datagrid-rowsview .dx-datagrid-content) {
  background-image: linear-gradient(#ffffff 50%, #f8f8f8 50%);
  background-size: 100% 64px; /* 32px * 2 rows */
}

/* Chỉ làm ẩn nội dung (nút) bên trong, không làm ẩn cả ô */
:deep(.dx-datagrid-rowsview .dx-data-row .col-action .action-buttons) {
  opacity: 0;
  transition: opacity 0.1s ease;
}

/* Hiện nội dung và màu nền khi hover hàng */
:deep(.dx-datagrid-rowsview .dx-data-row:hover .col-action),
:deep(.dx-datagrid-rowsview .dx-data-row.dx-state-hover .col-action) {
  background-color: #ebf9eb !important;
  pointer-events: auto;
}

:deep(.dx-datagrid-rowsview .dx-data-row:hover .col-action .action-buttons),
:deep(.dx-datagrid-rowsview .dx-data-row.dx-state-hover .col-action .action-buttons) {
  opacity: 1;
}

/* Khi hàng được chọn */
:deep(.dx-datagrid-rowsview .dx-data-row.dx-selection .col-action) {
  background-color: #e5f3ff !important;
  pointer-events: auto;
}

:deep(.dx-datagrid-rowsview .dx-data-row.dx-selection .col-action .action-buttons) {
  opacity: 1;
}

/* Header của cột action - TRONG SUỐT để nhìn xuyên qua khi scroll */
:deep(.dx-datagrid-headers .dx-header-row > td.col-action) {
  background-color: transparent !important;
  border-left: none !important;
  border-right: none !important;
  border-bottom: 1px solid var(--misa-border-color) !important;
}

/* Áp dụng màu xám cho container header để ô action vẫn có màu khi ở vùng trống */
:deep(.dx-datagrid-headers) {
  background-color: #f2f2f2 !important;
}

/* Căn giữa tiêu đề cho các cột khác */
:deep(.dx-datagrid-headers .dx-datagrid-text-content) {
  justify-content: flex-start;
}

.action-buttons {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 12px;
  height: 100%;
}

.add-system-action {
  justify-content: center;
}

.action-btn {
  cursor: pointer !important;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: opacity 0.2s;
}

.action-btn:hover {
  opacity: 0.7;
}
</style>