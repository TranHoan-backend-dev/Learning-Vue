<script setup lang="ts">
import CustomButton from "@/components/ui/ms-button/CustomButton.vue";
import SearchField from "@/components/ui/ms-input/SearchField.vue";
import CustomTable from "@/components/ui/ms-table/CustomTable.vue";
import NormalCheckbox from "@/components/ui/ms-checkbox/NormalCheckbox.vue";

import {data as candidateData} from "@/assets/data/data";
import {computed, ref} from "vue";
import CandidateAddEditForm from "@/views/ms-candidate/add-edit-form/CandidateAddEditForm.vue";
import CandidateViewModal from "@/views/ms-candidate/CandidateViewModal.vue";
import {usePagination} from "@/views/ms-candidate/usePagination.ts"

const components = [
  {iconClassName: "content_body_header_right_filter_icon"},
  {iconClassName: "content_body_header_right_shared_icon"},
  {iconClassName: "content_body_header_right_samset_icon"},
  {iconClassName: "sidebar_menu_item_setting_icon"},
]
const filteredData = ref([...candidateData]);

const {
  currentPage,
  pageSize,
  totalRecords,
  totalPages,
  paginatedData,
  pageInfo,
  handlePageSizeChange,
  handleNextPage,
  handlePrevPage
} = usePagination(filteredData);

const tableData = computed(() => {
  return paginatedData.value.map((candidate: any, index: number) =>
      [
        {tdClassName: 'col_checkbox text_center', slotName: 'checkbox'},
        {tdClassName: 'col_name text_center', value: ((currentPage.value - 1) * pageSize.value + index + 1).toString()},
        {tdClassName: 'col_name text_left', value: candidate.name || "--"},
        {tdClassName: 'col_phone text_right', value: candidate.phone || "--"},
        {tdClassName: 'col_email text_left', value: candidate.email || "--"},
        {tdClassName: 'col_email text_left', value: candidate.hiring_campaign || "--"},
        {tdClassName: 'col_email text_left', value: candidate.hiring_position || "--"},
        {tdClassName: 'col_email text_left', value: candidate.hiring_round || "--"},
        {tdClassName: 'col_date text_center', value: candidate.hiring_at || "--"},
        {tdClassName: 'col_email text_left', value: candidate.review || "--"},
        {tdClassName: 'col_email text_center', slotName: 'star'},
        {tdClassName: '', slotName: 'action', id: candidate.id, candidate: candidate}
      ]
  )
});

const isModalOpen = ref(false)
const modalMode = ref<'add' | 'view' | 'edit' | 'delete'>('add')
const currentCandidate = ref<any>(null)

const saveCandidate = (data: any) => {
  console.log('Saving candidate:', data);
  // Implementation for saving/updating goes here
  isModalOpen.value = false;
}

const handleDeleteMany = () => {

}

const handleOpenAddingModal = () => {
  currentCandidate.value = null;
  modalMode.value = 'add'
  isModalOpen.value = true
}

const handleOpenEditModal = (id: string) => {
  currentCandidate.value = filteredData.value.find(c => c.id === id) || null
  modalMode.value = 'edit'
  isModalOpen.value = true
}

const handleOpenViewModal = (id: string) => {
  currentCandidate.value = filteredData.value.find(c => c.id === id) || null
  console.log(currentCandidate.value)
  modalMode.value = 'view'
  isModalOpen.value = true
}

// <editor-fold> desc="handle deleting"
const handleOpenDeleteModal = (id: string) => {
  currentCandidate.value = filteredData.value.find(c => c.id === id) || null
  console.log(currentCandidate)
  modalMode.value = 'delete'
  isModalOpen.value = true
}

const confirmDeleting = () => {
  console.log('confirm deleting');
  if (currentCandidate.value) {
    filteredData.value = filteredData.value.filter(c => c.id !== currentCandidate.value.id)
  }
  currentCandidate.value = null
  isModalOpen.value = false
}
// </editor-fold>
</script>

<template>
  <section class="content">
    <!-- Title danh sách -->
    <div class="content_header">
      <div class="content_header_left">
        <div class="content_header_title">Ứng viên</div>
      </div>
      <div class="content_header_right">
        <!--        Xoa-->
        <CustomButton class-name="content_header_right_btn_delete">
          <div class="mi_icon_delete_user"></div>
          Xóa
        </CustomButton>

        <!--        Them-->
        <CustomButton
            variant="flat"
            class-name="content_header_right_btn_delete"
            style="background-color: var(--border-control-hover)"
            @click="handleOpenAddingModal"
        >
          <div class="content_header_action_add_icon_left"></div>
          <div class="content_header_action_add_icon_text">Thêm ứng viên</div>
        </CustomButton>
      </div>
    </div>

    <!-- Nội dung bảng -->
    <div class="content_body">
      <div class="content_body_container">
        <!--        Title-->
        <div class="content_body_title">
          <div class="content_body_header_left"></div>
          <div class="content_body_header_right">
            <div class="content_body_header_right_search">
              <div class="content_body_header_right_search_icon"></div>
              <SearchField placeholder="ứng viên..." :width="300"/>
            </div>
            <div
                class="content_body_header_right_icon"
                v-for="(component, index) in components"
                :key="index"
            >
              <div :class="component.iconClassName"></div>
            </div>
          </div>
        </div>

        <!--        Content table-->
        <div class="content_body_table">
          <div class="table_wrapper">
            <CustomTable
                :header-props="[
                  {className: 'col_checkbox text_center', slotName: 'checkbox'},
                  {className: 'col_name text_center', value: 'STT'},
                  {className: 'col_name text_left', value: 'Họ và Tên'},
                  {className: 'col_phone text_right', value: 'Số điện thoại'},
                  {className: 'col_email text_left', value: 'Email'},
                  {className: 'col_email text_left', value: 'Chiến dịch tuyển dụng'},
                  {className: 'col_email text_left', value: 'Vị trí tuyển dụng'},
                  {className: 'col_email text_left', value: 'Tin tuyển dụng'},
                  {className: 'col_date text_center', value: 'Ngày ứng tuyển'},
                  {className: 'col_email text_left', value: 'Vòng tuyển dụng'},
                  {className: 'col_email text_center', value: 'Đánh giá', slotName: 'star'},
                  {className: 'col_action text_center', value: 'Hành động', slotName: 'action'},
                ]"
                :body-props="tableData"
            >
              <template #header-checkbox>
                <NormalCheckbox/>
              </template>
              <template #cell-checkbox>
                <NormalCheckbox/>
              </template>
              <template #cell-star>
                <i v-for="i in 5" :key="i" class="bi bi-star"></i>
              </template>
              <template #cell-action="{ cell }">
                <div class="table_action">
                  <div class="mi_icon">
                    <button
                        class="mi_icon_edit"
                        title="Sửa ứng viên"
                        @click="handleOpenEditModal(cell.id!)"
                    />
                  </div>
                  <div class="mi_icon">
                    <button
                        class="mi_icon_delete"
                        title="Xóa ứng viên"
                        @click="handleOpenDeleteModal(cell.id!)"
                    />
                  </div>
                  <div class="mi_icon">
                    <button
                        class="mi_icon_view_details"
                        title="Xem ứng viên"
                        @click="handleOpenViewModal(cell.id!)"
                    />
                  </div>
                </div>
              </template>
            </CustomTable>
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
              <select id="pageSizeSelect" class="page_size_select" v-model="pageSize" @change="handlePageSizeChange">
                <option :value="5">5</option>
                <option :value="10">10</option>
                <option :value="15">15</option>
                <option :value="25">25</option>
                <option :value="50">50</option>
              </select>
            </div>
            <div class="content_body_footer_info">
              <span class="page_info" id="pageInfo">{{ pageInfo }}</span>
            </div>
            <div class="content_body_footer_nav">
              <button type="button" class="btn_page" id="btnPrevPage" title="Trang trước" @click="handlePrevPage" :disabled="currentPage <= 1">
                <div class="icon_prev"></div>
              </button>
              <button type="button" class="btn_page" id="btnNextPage" title="Trang sau" @click="handleNextPage" :disabled="currentPage >= totalPages">
                <div class="icon_next"></div>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <v-dialog v-model="isModalOpen" :max-width="modalMode === 'view' ? '1200px' : '800px'">
      <!-- Modal Form (Add/Edit) -->
      <v-card v-if="modalMode === 'add' || modalMode === 'edit'">
        <div class="modal__header d-flex justify-space-between align-center px-6 py-4">
          <h3 class="font-weight-bold text-h6 m-0">{{ modalMode === 'add' ? 'Thêm mới' : 'Chỉnh sửa' }} ứng viên</h3>
          <span class="close__btn cursor-pointer" @click="isModalOpen = false" style="font-size: 24px; line-height: 1;">&times;</span>
        </div>
        <CandidateAddEditForm
            :model-value="currentCandidate"
            :mode="modalMode"
            @save="saveCandidate"
            @cancel="isModalOpen = false"
        />
      </v-card>

      <!-- Modal View Details -->
      <v-card v-else-if="modalMode === 'view'" style="position: relative;">
        <CandidateViewModal
            :candidate="currentCandidate"
            @close="isModalOpen = false"
            @edit="handleOpenEditModal(currentCandidate)"
        />
        <span class="detail-modal__close" @click="isModalOpen = false"
              style="position: absolute; right: 20px; top: 10px; z-index: 100; font-size: 30px; cursor: pointer;">&times;</span>
      </v-card>

      <!-- Modal Delete -->
      <v-card v-else-if="modalMode === 'delete'" class="delete-confirmation-card">
        <div class="modal__header d-flex justify-space-between align-center px-6">
          <h3 class="font-weight-bold text-h6 m-0">Xác nhận</h3>
          <span class="close__btn cursor-pointer" @click="isModalOpen = false" style="font-size: 24px; line-height: 1;">&times;</span>
        </div>
        <v-card-text class="pa-8">
          <div class="d-flex flex-column text-center gap-16">
            <div class="confirm__icon"></div>
            <div>
              <p style="font-weight: 500; font-size: 14px; margin-bottom: 0;">Bạn có chắc chắn muốn xóa bản ghi đã chọn
                không?</p>
            </div>
          </div>
        </v-card-text>
        <v-card-actions class="pa-6 pt-0 justify-end">
          <CustomButton size="large" type="button" class="btn__cancel" @click="isModalOpen = false">Không</CustomButton>
          <CustomButton size="large" type="button" class="btn__delete" @click="confirmDeleting" color="error">Xóa
          </CustomButton>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </section>
</template>

<style scoped src="./style.css"></style>
<style scoped>
:deep(.text_left) {
  text-align: left !important;
}

:deep(.text_center) {
  text-align: center !important;
}

:deep(.text_right) {
  text-align: right !important;
}
</style>
<!-- <script setup lang="ts">
import CustomButton from "@/components/ui/ms-button/CustomButton.vue";
import SearchField from "@/components/ui/ms-input/SearchField.vue";
import CustomTable from "@/components/ui/ms-table/CustomTable.vue";
import NormalCheckbox from "@/components/ui/ms-checkbox/NormalCheckbox.vue";
import CandidateForm from "./CandidateForm.vue";

import {data as initialData} from "@/assets/data/data";
import {computed, ref} from "vue";

const searchQuery = ref('');
const candidates = ref([...initialData]);

const filteredCandidates = computed(() => {
  if (!searchQuery.value) return candidates.value;
  const lowerQuery = searchQuery.value.toLowerCase().trim();
  return candidates.value.filter(c =>
      (c.name && c.name.toLowerCase().includes(lowerQuery)) ||
      (c.email && c.email.toLowerCase().includes(lowerQuery)) ||
      (c.phone && c.phone.includes(lowerQuery))
  );
});

const isSelectAll = computed({
  get: () => filteredCandidates.value.length > 0 && selectedIds.value.length === filteredCandidates.value.length,
  set: (val: boolean) => {
    if (val) {
      selectedIds.value = filteredCandidates.value.map(c => c.id);
    } else {
      selectedIds.value = [];
    }
  }
});

const deleteSelected = () => {
  if (selectedIds.value.length === 0) return;
  candidates.value = candidates.value.filter(c => !selectedIds.value.includes(c.id));
  selectedIds.value = [];
}

const isModalOpen = ref(false);
const modalMode = ref<'add' | 'edit' | 'view'>('add');
const currentCandidate = ref<any>(null);

const openAddModal = () => {
  modalMode.value = 'add';
  currentCandidate.value = null;
  isModalOpen.value = true;
};

const openEditModal = (id: string) => {
  modalMode.value = 'edit';
  currentCandidate.value = candidates.value.find(c => c.id === id);
  isModalOpen.value = true;
};

const openViewModal = (id: string) => {
  modalMode.value = 'view';
  currentCandidate.value = candidates.value.find(c => c.id === id);
  isModalOpen.value = true;
};

const deleteSingle = (id: string) => {
  candidates.value = candidates.value.filter(c => c.id !== id);
  selectedIds.value = selectedIds.value.filter(sid => sid !== id);
}

const saveCandidate = (data: any) => {
  if (modalMode.value === 'add') {
    candidates.value.push({...data, id: Date.now().toString()});
  } else if (modalMode.value === 'edit') {
    const index = candidates.value.findIndex(c => c.id === currentCandidate.value.id);
    if (index !== -1) candidates.value[index] = {...candidates.value[index], ...data};
  }
  isModalOpen.value = false;
}

const components = [
  {iconClassName: "content_body_header_right_filter_icon"},
  {iconClassName: "content_body_header_right_shared_icon"},
  {iconClassName: "content_body_header_right_samset_icon"},
  {iconClassName: "sidebar_menu_item_setting_icon"},
]

const tableData = computed(() => {
  return filteredCandidates.value.map((candidate: any, index: number) =>
    [
      {tdClassName: 'col_checkbox text_center', slotName: 'checkbox', id: candidate.id},
      {tdClassName: 'col_name stt', value: (index + 1).toString()},
      {tdClassName: 'col_name text_left', value: candidate.name || "--"},
      {tdClassName: 'col_phone text_right', value: candidate.phone || "--"},
      {tdClassName: 'col_email text_left', value: candidate.email || "--"},
      {tdClassName: 'col_email text_left', value: candidate.hiring_campaign || "--"},
      {tdClassName: 'col_email text_left', value: candidate.hiring_position || "--"},
      {tdClassName: 'col_email text_left', value: candidate.hiring_round || "--"},
      {tdClassName: 'col_date text_center', value: candidate.hiring_at || "--"},
      {tdClassName: 'col_email text_left', value: candidate.review || "--"},
      {tdClassName: 'col_email text_center', slotName: 'star'},
      {tdClassName: '', slotName: 'action', id: candidate.id}
    ]
  )
});
</script>

<template>
  <section class="content"> -->
<!-- Title danh sách -->
<!--    <div class="content_header">-->
<!--      <div class="content_header_left">-->
<!--        <div class="content_header_title">Ứng viên</div>-->
<!--      </div>-->
<!--      <div class="content_header_right">-->
<!--        &lt;!&ndash;        Xoa&ndash;&gt;-->
<!--        <CustomButton-->
<!--            class-name="content_header_right_btn_delete"-->
<!--            :disabled="selectedIds.length === 0"-->
<!--            @click="deleteSelected"-->
<!--        >-->
<!--          <div class="mi_icon_delete_user"></div>-->
<!--          Xóa-->
<!--        </CustomButton>-->

<!--        Them-->
<!--        <CustomButton-->
<!--            variant="flat"-->
<!--            class-name="content_header_right_btn_delete"-->
<!--            style="background-color: var(&#45;&#45;border-control-hover)"-->
<!--            @click="openAddModal"-->
<!--        >-->
<!--          <div class="content_header_action_add_icon_left"></div>-->
<!--          <div class="content_header_action_add_icon_text">Thêm ứng viên</div>-->
<!--        </CustomButton>-->
<!--      </div>-->
<!--    </div>-->

<!-- Nội dung bảng -->
<!--    <div class="content_body">-->
<!--      <div class="content_body_container">-->
<!--        Title-->
<!--        <div class="content_body_title">-->
<!--          <div class="content_body_header_left"></div>-->
<!--          <div class="content_body_header_right">-->
<!--            <div class="content_body_header_right_search">-->
<!--              <div class="content_body_header_right_search_icon"></div>-->
<!--              <SearchField placeholder="ứng viên..." :width="300"/>-->
<!--            </div>-->
<!--            <div-->
<!--                class="content_body_header_right_icon"-->
<!--                v-for="(component, index) in components"-->
<!--                :key="index"-->
<!--            >-->
<!--              <div :class="component.iconClassName"></div>-->
<!--            </div>-->
<!--          </div>-->
<!--        </div>-->

<!--        Content table-->
<!--        <div class="content_body_table">-->
<!--          <div class="table_wrapper">-->
<!--            <CustomTable-->
<!--                :header-props="[-->
<!--                  {className: 'col_checkbox text_center', slotName: 'checkbox'},-->
<!--                  {className: 'col_name text_center', value: 'STT'},-->
<!--                  {className: 'col_name text_left', value: 'Họ và Tên'},-->
<!--                  {className: 'col_phone text_right', value: 'Số điện thoại'},-->
<!--                  {className: 'col_email text_left', value: 'Email'},-->
<!--                  {className: 'col_email text_left', value: 'Chiến dịch tuyển dụng'},-->
<!--                  {className: 'col_email text_left', value: 'Vị trí tuyển dụng'},-->
<!--                  {className: 'col_email text_left', value: 'Tin tuyển dụng'},-->
<!--                  {className: 'col_date text_center', value: 'Ngày ứng tuyển'},-->
<!--                  {className: 'col_email text_left', value: 'Vòng tuyển dụng'},-->
<!--                  {className: 'col_email text_center', value: 'Đánh giá', slotName: 'star'},-->
<!--                  {className: 'col_action text_center', value: 'Hành động', slotName: 'action'},-->
<!--                ]"-->
<!--                :body-props="tableData"-->
<!--            >-->
<!--              <template #header-checkbox>-->
<!--                <NormalCheckbox v-model="isSelectAll"/>-->
<!--              </template>-->
<!--              <template #cell-checkbox="{ cell }">-->
<!--                <NormalCheckbox-->
<!--                    :value="cell.id"-->
<!--                    v-model="selectedIds"-->
<!--                />-->
<!--              </template>-->
<!--              <template #cell-star>-->
<!--                <i v-for="i in 5" :key="i" class="bi bi-star"></i>-->
<!--              </template>-->
<!--              <template #cell-action="{ cell }">-->
<!--                <div class="table_action">-->
<!--                  <div class="mi_icon">-->
<!--                    <button class="mi_icon_edit" title="Sửa ứng viên" @click="openEditModal(cell.id)"/>-->
<!--                  </div>-->
<!--                  <div class="mi_icon">-->
<!--                    <button class="mi_icon_delete" title="Xóa ứng viên" @click="deleteSingle(cell.id)"/>-->
<!--                  </div>-->
<!--                  <div class="mi_icon">-->
<!--                    <button class="mi_icon_view_details" title="Xem ứng viên" @click="openViewModal(cell.id)"/>-->
<!--                  </div>-->
<!--                </div>-->
<!--              </template>-->
<!--            </CustomTable>-->
<!--          </div>-->
<!--        </div>-->
<!--      </div>-->
<!--    </div>-->
<!-- Modal Thêm/Sửa/Xem -->
<!--    <v-dialog v-model="isModalOpen" max-width="800px">-->
<!--      <v-card>-->
<!--        <v-card-title class="px-6 py-4 border-b border-gray-100 font-bold text-lg">-->
<!--          {{ modalMode === 'add' ? 'Thêm mới' : modalMode === 'edit' ? 'Chỉnh sửa' : 'Chi tiết' }} ứng viên-->
<!--        </v-card-title>-->
<!--        <v-card-text class="pt-4 px-6 pb-2">-->
<!--          <CandidateForm-->
<!--            v-model="currentCandidate"-->
<!--            :mode="modalMode"-->
<!--            @save="saveCandidate"-->
<!--            @cancel="isModalOpen = false"-->
<!--          />-->
<!--        </v-card-text>-->
<!--      </v-card>-->
<!--    </v-dialog>-->
<!--  </section>-->
<!--</template>-->

<!--<style scoped src="./style.css"></style>-->
<!--<style scoped>-->
<!--:deep(.text_left) {-->
<!--  text-align: left !important;-->
<!--}-->
<!--:deep(.text_center) {-->
<!--  text-align: center !important;-->
<!--}-->
<!--:deep(.text_right) {-->
<!--  text-align: right !important;-->
<!--}-->
<!--</style>-->
