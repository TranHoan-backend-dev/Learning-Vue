<script setup lang="ts">
import DxSelectBox from "devextreme-vue/select-box";
import DxDataGrid, {DxColumn, DxPaging, DxScrolling, DxSelection} from "devextreme-vue/data-grid";
import CustomPagination from "@/components/ui/ms-pagination/CustomPagination.vue";
import MSIcon from "@/components/ui/ms-icon/MSIcon.vue";
import {DxTooltip} from "devextreme-vue";
import MSStatusBadge from "@/components/ui/ms-status-badge/MSStatusBadge.vue";
import {gridActions, pageSizeOptions} from "@/views/ms-salary-compositions/data.ts";

const props = defineProps<{
  tableData: any[];
  totalRecords: number;
  pageSize: number;
  currentPage: number;
  columns: any[];
  pageInfo: string;
  selectedIds: string[];
  searchKeyword: string;
}>();

const emit = defineEmits([
  'update:selectedIds', 
  'update:searchKeyword', 
  'update:currentPage',
  'update:pageSize',
  'handlePageSizeChange',
  'handleOpenConfig',
  'togglePin',
  'handleActive',
  'handleDuplicate',
  'handleEdit',
  'handleDelete'
]);

const calculateSTT = (data: any) => {
  const index = props.tableData.findIndex(item => item.componentId === data.componentId);
  return (props.currentPage - 1) * props.pageSize + index + 1;
};

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
</script>

<template>
  <div class="content_body">
    <div class="content_body_container">
      <!-- Title -->
      <div class="content_body_title">
        <div class="content_body_header_left">
          <div class="content_body_header_left_search">
            <input 
              type="text" 
              class="misa-search-input" 
              :value="searchKeyword" 
              @input="handleSearchChange"
              placeholder="Tìm kiếm" 
              style="width: 250px;" 
            />
          </div>
        </div>
        <div class="content_body_header_right">
          <div class="content_body_header_right_filters">
            <DxSelectBox 
              class="misa-selectbox" 
              :items="[{ text: 'Tất cả trạng thái', value: 'all' }]"
              display-expr="text" 
              value-expr="value" 
              value="all" 
              :width="160" 
            />
            <DxSelectBox 
              class="misa-selectbox" 
              :items="[{ text: 'Tất cả đơn vị', value: 'all' }]" 
              display-expr="text"
              value-expr="value" 
              value="all" 
              :width="320" 
            />
          </div>
          <div class="content_body_header_right_icon">
            <div class="mi_icon_filter"></div>
          </div>
          <div class="content_body_header_right_icon" @click="emit('handleOpenConfig')">
            <div class="mi_icon_setting"></div>
          </div>
        </div>
      </div>

      <!-- Content table -->
      <div class="content_body_table">
        <div class="table_wrapper">
          <DxDataGrid 
            :data-source="tableData" 
            :show-borders="true" 
            :row-alternation-enabled="true"
            :show-column-lines="true" 
            key-expr="componentId" 
            :selected-row-keys="selectedIds"
            @update:selected-row-keys="handleSelectedIdsChange"
            :column-auto-width="false" 
            :allow-column-resizing="true" 
            column-resizing-mode="widget" 
            width="100%"
            height="100%"
          >
            <DxScrolling mode="standard" show-scrollbar="always" :use-native="true" :scroll-by-content="true"
              :scroll-by-thumb="true" />
            <DxPaging :enabled="false" />
            <DxSelection mode="multiple" show-check-boxes-mode="always" />
            
            <!-- STT -->
            <DxColumn caption="STT" :calculate-cell-value="calculateSTT" :width="50" alignment="center" fixed />

            <template v-for="col in columns" :key="col.dataField">
              <DxColumn 
                v-if="col.visible" 
                :data-field="col.dataField" 
                :caption="col.caption" 
                :width="col.width"
                :fixed="col.dataField === 'componentName' && col.isPinned" 
                :cell-template="col.cellTemplate"
                :header-cell-template="col.dataField === 'componentName' ? 'nameHeaderTemplate' : undefined" 
              />
            </template>

            <!-- Cột Chức năng -->
            <DxColumn 
              caption="Chức năng" 
              cell-template="actionTemplate" 
              alignment="center" 
              :width="160" 
              fixed
              fixed-position="right" 
              css-class="col-action" 
            />

            <template #nameHeaderTemplate="{ data }">
              <div class="header-name-container">
                <span>{{ data.column.caption }}</span>
                <div class="pin-icon"
                  :class="{ 'is-pinned': columns.find(c => c.dataField === 'componentName')?.isPinned }"
                  @click="emit('togglePin', $event)" 
                  title="Ghim cột"
                >
                  <MSIcon name="pin" size="16" />
                </div>
              </div>
            </template>

            <template #status-cell="{ data }">
              <MSStatusBadge :status="data.value" />
            </template>

            <template #actionTemplate="{ data }">
              <div class="action-buttons">
                <template v-for="btn in gridActions" :key="btn.id">
                  <div 
                    :id="`btn-${btn.id}-${data.data.componentId}`"
                    class="action-btn" 
                    :class="btn.class" 
                    @click="btn.id === 'active' ? emit('handleActive', data.data) : 
                            btn.id === 'copy' ? emit('handleDuplicate', data.data) : 
                            btn.id === 'edit' ? emit('handleEdit', data.data) : 
                            emit('handleDelete', data.data)"
                  >
                    <MSIcon :name="btn.icon" :color="btn.color" />
                  </div>
                </template>
              </div>

              <!-- Tooltips -->
              <template v-for="btn in gridActions" :key="`tooltip-${btn.id}-${data.data.componentId}`">
                <DxTooltip
                  :target="`#btn-${btn.id}-${data.data.componentId}`"
                  show-event="dxhoverstart"
                  hide-event="dxhoverend"
                  position="top"
                >
                  <template #content>
                    <p style="margin: 0; font-size: 12px;">{{ btn.title }}</p>
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
              color="#0070f3" 
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped src="../style.css"></style>