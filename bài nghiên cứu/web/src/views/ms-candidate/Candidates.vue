<script setup lang="ts">
import 'devextreme/dist/css/dx.fluent.blue.light.css';

import {computed, ref, watch} from "vue";
import {usePagination} from "@/views/ms-candidate/usePagination.ts"
import CustomPagination from "@/components/ui/ms-pagination/CustomPagination.vue";
import type {BodyProps} from "@/components/ui/ms-table/model.ts";
import {toast} from "@/services/toast.ts";
import DxDataGrid, {
  DxColumn,
  DxSelection
} from 'devextreme-vue/data-grid';
import DxSelectBox from 'devextreme-vue/select-box';
import {salaryComponentsData, type SalaryComponents} from "@/views/ms-candidate/data.ts";
import SearchField from "@/components/ui/ms-input/SearchField.vue";
import CustomSelect from "@/components/ui/ms-select/CustomSelect.vue";

/**
 * Lấy chữ cái viết tắt từ tên
 */
const getInitials = (name: string) => {
  if (!name || name === "--") return "";
  const parts = name.trim().split(' ').filter(p => p.length > 0);
  if (parts.length === 0) return "";
  if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
  return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
};

// Danh sách màu sắc cho avatar
const avatarColors = [
  '#FF5722', '#E91E63', '#9C27B0', '#673AB7', '#3F51B5',
  '#2196F3', '#03A9F4', '#00BCD4', '#009688', '#4CAF50',
  '#8BC34A', '#CDDC39', '#FFEB3B', '#FFC107', '#FF9800', '#795548'
];

// Lấy màu sắc ngẫu nhiên dựa trên tên
const getAvatarColor = (name: string) => {
  if (!name || name === "--") return '#ccc';
  let hash = 0;
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash);
  }
  const index = Math.abs(hash) % avatarColors.length;
  return avatarColors[index];
};

toast.info('Dang nhap thanh cong', 'Chao mung den voi trang tuyen dung')

const components = [
  {iconClassName: "content_body_header_right_filter_icon"},
  {iconClassName: "content_body_header_right_shared_icon"},
  {iconClassName: "content_body_header_right_samset_icon"},
  {iconClassName: "sidebar_menu_item_setting_icon"},
]

const isModalOpen = ref(false)
const modalMode = ref<'add' | 'view' | 'edit' | 'delete'>('add')
const currentCandidate = ref<any>(null)
const selectedIds = ref<string[]>([]);
const isLoading = ref(false);
const isSlowLoading = ref(false);

const pageSizeOptions = [
  {value: 5, label: "5"},
  {value: 10, label: "10"},
  {value: 15, label: "15"},
  {value: 25, label: "25"},
  {value: 50, label: "50"},
];

const searchKeyword = ref("");
const filteredData = computed(() => {
  if (!searchKeyword.value) return salaryComponentsData;
  const keyword = searchKeyword.value.toLowerCase();
  return salaryComponentsData.filter(item =>
      item.componentId.toLowerCase().includes(keyword) ||
      item.componentName.toLowerCase().includes(keyword) ||
      item.componentType.toLowerCase().includes(keyword)
  );
});

// <editor-fold> desc="Search"
watch(searchKeyword, () => {
  currentPage.value = 1;
  // fetchCandidates();
});
// </editor-fold>

// Khởi tạo pagination trước khi dùng trong fetch
// Khởi tạo các biến cơ bản từ pagination
const {
  currentPage,
  pageSize,
  handlePageSizeChange,
  paginatedData,
  totalRecords,
  pageInfo: hookPageInfo
} = usePagination(filteredData);

const totalPages = computed(() => {
  return Math.ceil(totalRecords.value / pageSize.value) || 1;
});

// Cập nhật lại tableData để dùng filteredData trực tiếp từ Server
const formatDate = (dateStr: string | null | undefined) => {
  if (!dateStr) return "--";
  try {
    const d = new Date(dateStr);
    if (isNaN(d.getTime())) return dateStr;
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    return `${day}/${month}/${year}`;
  } catch (e) {
    return dateStr;
  }
};

const calculateSTT = (data: SalaryComponents) => {
  const index = filteredData.value.findIndex(item => item.componentId === data.componentId);
  return index + 1;
};


const tableData = computed<BodyProps[][]>(() => {
  // Chỉ hiển thị Skeleton nếu thời gian tải dữ liệu vượt quá 1 giây
  if (isSlowLoading.value) {
    return Array.from({length: pageSize.value}).map(() =>
        Array.from({length: 12}).map(() => ({
          tdClassName: 'text_center',
          slotName: 'skeleton'
        }))
    );
  }

  return filteredData.value.map((candidate: any, index: number): BodyProps[] =>
      [
        {tdClassName: 'col_checkbox text_center', slotName: 'checkbox', id: candidate.id},
        {tdClassName: 'col_name text_left', value: candidate.name || "--", slotName: 'name'},
        {tdClassName: 'col_phone text_right', value: candidate.phone || "--"},
        {tdClassName: 'col_email text_left', value: candidate.email || "--"},
        {tdClassName: 'col_email text_left', value: candidate.hiringCampaign || "--"},
        {tdClassName: 'col_email text_left', value: candidate.hiringPosition || "--"},
        {tdClassName: 'col_email text_left', value: "--"},
        {tdClassName: 'col_date text_center', value: formatDate(candidate.hiringAt)},
        {tdClassName: 'col_email text_left', value: candidate.hiringRound || "--"},
        {tdClassName: 'col_email text_center', slotName: 'star'},
        {tdClassName: '', slotName: 'action', id: candidate.id, candidate: candidate}
      ]
  )
});

</script>

<template>
  <section class="content">
    <!-- Title danh sách -->
    <div class="content_header">
      <div class="content_header_left">
        <div class="content_header_title">Thành phần lương</div>
      </div>
      <div class="content_header_right">
        <button class="misa-btn-outline">
          <div class="mi_icon_system_category"></div>
          <span>Danh mục của hệ thống</span>
        </button>

        <div class="split_button_container">
          <button class="misa-btn-primary-left">
            <div class="mi_icon_add_white"></div>
            Thêm mới
          </button>
          <div class="split_button_arrow">
            <div class="mi_icon_arrow_down_white"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Nội dung bảng -->
    <div class="content_body">
      <div class="content_body_container">
        <!--        Title-->
        <div class="content_body_title">
          <div class="content_body_header_left">
            <div class="content_body_header_left_search">
              <div class="navbar_left_search_icon"></div>
              <SearchField v-model="searchKeyword" :width="250" placeholder=""/>
            </div>
          </div>
          <div class="content_body_header_right">
            <div class="content_body_header_right_filters">
              <DxSelectBox class="misa-selectbox" :items="[{ text: 'Tất cả trạng thái', value: 'all' }]"
                           display-expr="text" value-expr="value" value="all" :width="160"/>
              <DxSelectBox class="misa-selectbox" :items="[{ text: 'Tất cả đơn vị', value: 'all' }]" display-expr="text"
                           value-expr="value" value="all" :width="320"/>
            </div>
            <div class="content_body_header_right_icon">
              <div class="mi_icon_filter"></div>
            </div>
            <div class="content_body_header_right_icon">
              <div class="mi_icon_setting"></div>
            </div>
          </div>
        </div>

        <!--        Content table-->
        <div class="content_body_table">
          <div class="table_wrapper">
            <DxDataGrid :data-source="paginatedData" :show-borders="true" :row-alternation-enabled="true"
                        key-expr="componentId" v-model:selected-row-keys="selectedIds">
              <DxSelection mode="multiple" show-check-boxes-mode="always"/>
              <DxColumn caption="STT" :calculate-cell-value="calculateSTT" :width="50" alignment="center"/>
              <DxColumn data-field="componentId" caption="Mã thành phần"/>
              <DxColumn data-field="componentName" caption="Tên thành phần"/>
              <DxColumn data-field="appliedFor" caption="Đơn vị áp dụng"/>
              <DxColumn data-field="componentType" caption="Loại thành phần"/>
              <DxColumn data-field="attribute" caption="Tính chất"/>
              <DxColumn data-field="valueType" caption="Kiểu giá trị"/>
              <DxColumn data-field="value" caption="Giá trị"/>
            </DxDataGrid>
          </div>
        </div>
        <div class="content_body_footer">
          <!-- Tổng bản ghi và Số bản ghi trên trang -->
          <div class="content_body_footer_left">
            <div class="content_body_footer_total">
              Tổng: <strong id="totalRecords">{{ totalRecords }}</strong> bản ghi
            </div>
          </div>

          <!-- Phân trang và điều hướng -->
          <div class="content_body_footer_right">
            <div class="content_body_footer_pagesize">
              <span class="paging_label">Số bản ghi trên trang</span>
              <div class="page_size_custom_select">
                <CustomSelect
                    v-model="pageSize"
                    :options="pageSizeOptions"
                    @update:modelValue="handlePageSizeChange"
                    size="sm"
                    hide-error-space/>
              </div>
            </div>
            <div class="content_body_footer_info">
              <span class="page_info" id="pageInfo">{{ hookPageInfo }}</span>
            </div>
            <div class="content_body_footer_nav">
              <CustomPagination v-model="currentPage" :total="totalRecords" :page-size="pageSize" color="#0070f3"/>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped src="./style.css">
.misa-selectbox {
  border: 1px solid #dddde4;
  border-radius: 4px;
}

.misa-selectbox.dx-state-hover {
  border-color: #2ca01c;
}

.content_body_footer_pagesize {
  display: flex;
  align-items: center;
  gap: 8px;
}

.page_size_custom_select {
  width: 120px;
}

.navbar_left_search_icon {
  width: 24px;
  height: 24px;
  -webkit-mask-image: var(--misa-amis-icon-1);
  -webkit-mask-position: -311px -1105px;
  position: absolute;
  top: 14px;
  left: 8px;
  background-color: #c5ccd5;
  z-index: 2;
  pointer-events: none;
}
</style>
