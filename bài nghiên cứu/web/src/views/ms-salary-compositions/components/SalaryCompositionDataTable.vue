<script setup lang="ts">
import {computed} from "vue";
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
  gridActions,
  pageSizeOptions,
  salaryCompositionStatus
} from "@/views/ms-salary-compositions/data.ts";

const props = defineProps<DataTableAttributes>();

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
  'update:statusFilter'
]);

const handleSelectedIdsChange = (val: string[]) => {
  emit('update:selectedIds', val);
};

const handleSearchChange = (e: any) => {
  emit('update:searchKeyword', e.target.value);
};

const handleCurrentPageChange = (val: number) => {
  emit('update:currentPage', val);
};

const handlePageSizeValChange = (e: any) => {
  emit('update:pageSize', e.value);
  emit('handlePageSizeChange');
};

const isFixed = (col: any) => {
  const pinnedIndex = props.columns.findIndex(c => c.isPinned);
  if (pinnedIndex === -1) return false;

  const currentColIndex = props.columns.findIndex(c => c.dataField === col.dataField);
  return currentColIndex !== -1 && currentColIndex <= pinnedIndex;
};

const handleTogglePin = (e: any, dataField: string) => {
  emit('togglePin', e, dataField);
};

const fullColumns = computed(() => {
  return props.columns.map((col, index) => ({
    ...col,
    fixed: isFixed(col),
    allowFiltering: false,
    allowSorting: false,
    visibleIndex: index,
    isStt: false
  }));
});
// TODO: mau #ff4d4f bi lap lai hoi nhieu
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
              <MSIcon name="trash" color="#ff4d4f"/>
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
                  type="text" class="misa-search-input"
                  :value="searchKeyword" @input="handleSearchChange"
                  placeholder="Tìm kiếm" style="width: 250px;"/>
            </div>
          </div>

          <div class="content_body_header_right">
            <div class="content_body_header_right_filters">
              <template v-if="!isSystemMode">
                <DxSelectBox
                    class="misa-selectbox"
                    :items="salaryCompositionStatus"
                    display-expr="text"
                    value-expr="value"
                    :value="statusFilter"
                    @value-changed="(e) => emit('update:statusFilter', e.value)"
                    :width="160"/>
                <DxSelectBox
                    class="misa-selectbox"
                    :items="[{ text: 'Tất cả đơn vị', value: 'all' }]"
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
              :show-row-lines="false" key-expr="componentId"
              :selected-row-keys="selectedIds"
              @update:selected-row-keys="handleSelectedIdsChange"
              :column-auto-width="false"
              :allow-column-resizing="true"
              column-resizing-mode="widget"
              width="100%"
              height="100%">
            <DxScrolling
                mode="standard"
                show-scrollbar="always"
                :use-native="true"
                :scroll-by-content="true"
                :scroll-by-thumb="true"/>

            <DxPaging :enabled="false"/>
            <DxSelection mode="multiple" show-check-boxes-mode="always"/>
            <DxHeaderFilter :visible="false"/>
            <DxSorting mode="none"/>

            <template v-for="col in fullColumns" :key="col.dataField">
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
                  :visible-index="col.visibleIndex"/>
            </template>

            <template #valueTemplate="{ data }">
              <div :class="{ 'formula-cell': data.value && data.value.toString().startsWith('=') }" :title="data.value">
                {{ data.value || '-' }}
              </div>
            </template>

            <!-- Cột Chức năng (Ẩn trong mode Hệ thống) -->
            <DxColumn
                v-if="!isSystemMode"
                caption="Chức năng"
                cell-template="actionTemplate"
                alignment="center"
                :width="160"
                fixed fixed-position="right"
                css-class="col-action"
                :allow-filtering="false"
                :allow-sorting="false"
                :visible-index="1000"/>

            <!-- Cột Thêm (Chỉ hiện trong mode Hệ thống) -->
            <DxColumn
                v-if="isSystemMode"
                caption=""
                cell-template="addSystemTemplate"
                alignment="center"
                :width="50"
                fixed fixed-position="right"
                :visible-index="1000"/>

            <template #headerTemplate="{ data }">
              <div class="header-name-container">
                <span class="column-caption-text">{{ data.column.caption }}</span>
                <div class="pin-icon"
                     :class="{ 'is-pinned': columns.find(c => c.dataField === data.column.dataField)?.isPinned }"
                     @click="handleTogglePin($event, data.column.dataField)" title="Ghim cột">
                  <MSIcon name="pin" size="16"/>
                </div>
              </div>
            </template>

            <template #status-cell="{ data }">
              <MSStatusBadge :status="data.value"/>
            </template>

            <template #addSystemTemplate="{ data }">
              <div :id="`add-sys-${data.data.componentId}`" class="add-system-btn"
                   @click="emit('addSystem', data.data)">
                <MSIcon name="check-circle" size="20" color="#2ca01c"/>
              </div>
              <DxTooltip
                  :target="`#add-sys-${data.data.componentId}`"
                  show-event="dxhoverstart"
                  hide-event="dxhoverend"
                  position="top">
                <template #content>
                  <p class="p_content">Đưa vào danh sách sử dụng</p>
                </template>
              </DxTooltip>
            </template>

            <template #actionTemplate="{ data }">
              <div class="action-buttons">
                <template v-for="btn in gridActions" :key="btn.id">
                  <div
                      class="action-btn"
                      :id="`btn-${btn.id}-${data.data.componentId}`"
                      :class="btn.class"
                      @click="btn.id === 'active' ? emit('handleActive', data.data) :
                        btn.id === 'copy' ? emit('handleDuplicate', data.data) :
                        btn.id === 'edit' ? emit('handleEdit', data.data) :
                        emit('handleDelete', data.data)">
                    <MSIcon :name="btn.icon" :color="btn.color"/>
                  </div>
                </template>
              </div>

              <!-- Tooltips -->
              <template v-for="btn in gridActions" :key="`tooltip-${btn.id}-${data.data.componentId}`">
                <DxTooltip
                    :target="`#btn-${btn.id}-${data.data.componentId}`"
                    show-event="dxhoverstart"
                    hide-event="dxhoverend"
                    position="top">
                  <template #content>
                    <p class="p_content">{{ btn.title }}</p>
                  </template>
                </DxTooltip>
              </template>
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
              <DxSelectBox class="misa-selectbox" :value="pageSize" :items="pageSizeOptions" display-expr="label"
                           value-expr="value" :width="70" @value-changed="handlePageSizeValChange"/>
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
                :page-size="pageSize" color="var(--primary-green)"/>
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
  font-size: 14px;
}

.unselect-link {
  color: #2ca01c;
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
  height: 36px;
  border: 1px solid #ff4d4f;
  border-radius: 4px;
  background-color: #fff;
  color: #ff4d4f;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.misa-btn-delete-bulk:hover {
  background-color: #fff1f0;
}

.dx-datagrid-headers .dx-header-row > td {
  border-bottom: 1px solid #e0e0e0 !important;
  border-right: 1px solid #e0e0e0 !important;
}

.dx-datagrid-rowsview .dx-data-row > td {
  border-bottom: 1px solid #e0e0e0 !important;
  border-right: 1px solid #e0e0e0 !important;
}

/* Đảm bảo cột STT và cột checkbox cũng có border */
.dx-datagrid-rowsview .dx-data-row > td:first-child {
  border-left: 1px solid #e0e0e0 !important;
}

.dx-datagrid-headers .dx-header-row > td:first-child {
  border-left: 1px solid #e0e0e0 !important;
}

.formula-cell {
  color: #0070f3;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Căn giữa tiêu đề cho cột Chức năng */
:deep(.dx-datagrid-headers .col-action .dx-datagrid-text-content) {
  justify-content: center !important;
}

.p_content {
  margin: 0;
  font-size: 12px;
}
</style>