<script setup lang="ts">
import CustomButton from "@/components/ui/ms-button/CustomButton.vue";
import SearchField from "@/components/ui/ms-input/SearchField.vue";
import CustomTable from "@/components/ui/ms-table/CustomTable.vue";
import NormalCheckbox from "@/components/ui/ms-checkbox/NormalCheckbox.vue";

import {computed, onMounted, ref, watch} from "vue";
import CandidateAddEditForm from "@/views/ms-candidate/add-edit-form/CandidateAddEditForm.vue";
import CandidateViewModal from "@/views/ms-candidate/CandidateViewModal.vue";
import {usePagination} from "@/views/ms-candidate/usePagination.ts"
import CustomPagination from "@/components/ui/ms-pagination/CustomPagination.vue";
import CustomSkeleton from "@/components/ui/ms-skeleton/CustomSkeleton.vue";
import CustomSelect from "@/components/ui/ms-select/CustomSelect.vue";
import type {BodyProps} from "@/components/ui/ms-table/model.ts";
import {toast} from "@/services/toast.ts";
import candidateService, {type Candidate, type Pageable, type FilterRequest} from '@/services/candidateService';

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
const selectedCandidateIds = ref<string[]>([]);
const isLoading = ref(false);
const isSlowLoading = ref(false);

const filteredData = ref<any[]>([]);
const totalRecordsServer = ref(0);

// Khởi tạo pagination trước khi dùng trong fetch
// Khởi tạo các biến cơ bản từ pagination
const {
  currentPage,
  pageSize,
  handlePageSizeChange,
} = usePagination(filteredData);

const totalPages = computed(() => {
  return Math.ceil(totalRecordsServer.value / pageSize.value) || 1;
});

// Định nghĩa các lựa chọn cho số bản ghi trên trang
const pageSizeOptions = [
  {value: 5, label: "5"},
  {value: 10, label: "10"},
  {value: 15, label: "15"},
  {value: 25, label: "25"},
  {value: 50, label: "50"},
];

// Hiển thị thông tin phân trang (ví dụ: 1 - 10 trên 134 bản ghi)
const pageInfo = computed(() => {
  const start = totalRecordsServer.value === 0 ? 0 : (currentPage.value - 1) * pageSize.value + 1;
  const end = Math.min(currentPage.value * pageSize.value, totalRecordsServer.value);
  return `${start} - ${end} trên ${totalRecordsServer.value} bản ghi`;
});


// Cập nhật lại tableData để dùng filteredData trực tiếp từ Server
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
        {tdClassName: '', slotName: 'checkbox', id: candidate.id},
        {tdClassName: 'col_name text_left', value: candidate.name || "--"},
        {tdClassName: 'col_phone text_right', value: candidate.phone || "--"},
        {tdClassName: 'col_email text_left', value: candidate.email || "--"},
        {tdClassName: 'col_email text_left', value: candidate.hiringCampaign || "--"},
        {tdClassName: 'col_email text_left', value: candidate.hiringPosition || "--"},
        {tdClassName: 'col_email text_left', value: "--"},
        {tdClassName: 'col_date text_center', value: candidate.hiringAt || "--"},
        {tdClassName: 'col_email text_left', value: candidate.hiringRound || "--"},
        {tdClassName: 'col_email text_center', slotName: 'star'},
        {tdClassName: '', slotName: 'action', id: candidate.id, candidate: candidate}
      ]
  )
});

// <editor-fold> desc="Fetch candidates list"
/**
 * Hàm lấy danh sách ứng viên từ Backend
 */
const fetchCandidates = async () => {
  let skeletonTimeout: any = null;
  try {
    isLoading.value = true;

    // Thiết lập đếm ngược 1 giây trước khi hiện Skeleton
    skeletonTimeout = setTimeout(() => {
      if (isLoading.value) {
        isSlowLoading.value = true;
      }
    }, 1000);

    const pageable: Pageable = {
      pageIndex: currentPage.value - 1, // Chuyển từ 1-indexed sang 0-indexed cho Backend
      pageSize: pageSize.value
    };
    const response = await candidateService.getPaginated(pageable, {keyword: ""});
    if (response && response.data) {
      console.log(response.data)
      // Sửa lỗi mapping: gán candidateId vào id để đồng bộ logic toàn app
      filteredData.value = (response.data.data || []).map((item: any) => ({
        ...item,
        id: item.candidateId
      }));
      totalRecordsServer.value = response.data.pageable?.totalElements || 0;
    }
  } catch (error) {
    console.error("Lỗi khi lấy danh sách ứng viên:", error);
    toast.error("Lỗi", "Không thể lấy dữ liệu từ máy chủ");
  } finally {
    isLoading.value = false;
    isSlowLoading.value = false;
    if (skeletonTimeout) clearTimeout(skeletonTimeout);
  }
};

onMounted(() => {
  fetchCandidates();
});

watch([currentPage, pageSize], () => {
  fetchCandidates();
});

// Đảm bảo không ở trang ảo (ví dụ đang ở trang 10 nhưng đổi pageSize làm tổng số trang chỉ còn 5)
watch(totalPages, (newTotal) => {
  if (currentPage.value > newTotal) {
    currentPage.value = newTotal || 1;
  }
});
// </editor-fold>

// <editor-fold> desc="Save candidate"
/**
 * Hàm thêm mới hoặc cập nhật ứng viên thông qua Service
 */
const handleSaveCandidate = async (data: Candidate) => {
  try {
    isLoading.value = true;
    if (data.candidateId) {
      await candidateService.update(data.candidateId, data);
      toast.success("Thành công", "Cập nhật ứng viên thành công");
    } else {
      await candidateService.add(data);
      toast.success("Thành công", "Thêm mới ứng viên thành công");
    }
    isModalOpen.value = false;
    // Load lại dữ liệu sau khi lưu thành công
    await fetchCandidates();
  } catch (error) {
    console.error("Lỗi khi lưu ứng viên:", error);
    toast.error("Lỗi", "Không thể lưu thông tin ứng viên");
  } finally {
    isLoading.value = false;
  }
};
// </editor-fold>

// <editor-fold> desc="delete many"

// lay toan bo cac id dang co trong trang
const currentPageIds = computed(() =>
    filteredData.value.map((candidate: any) => candidate.id)
)

// kiem tra xem tat ca cac checkbox da duoc chon hay chua
const isAllCurrentPageSelected = computed(() => {
  return currentPageIds.value.length > 0 &&
      currentPageIds.value.every(id => selectedCandidateIds.value.includes(id))
})

// kiem tra xem lieu nguoi dung da chon checkbox nao hay chua
const hasSelectedRows = computed(() => selectedCandidateIds.value.length > 0);

const toggleCandidateSelection = (id: string | undefined) => {
  if (!id) return;

  if (selectedCandidateIds.value.includes(id)) {
    // Nếu bản ghi đã được chọn, thì loại bỏ khỏi mảng
    selectedCandidateIds.value = selectedCandidateIds.value.filter(item => item !== id);
  } else {
    // Nếu bản ghi chưa được chọn, thì thêm vào mảng
    selectedCandidateIds.value.push(id)
  }
}

const toggleSelectAllCurrentPage = () => {
  if (isAllCurrentPageSelected.value) {
    // neu tat ca da duoc chon => bo chon tat ca
    selectedCandidateIds.value = selectedCandidateIds
        .value
        .filter(id => !currentPageIds.value.includes(id));
  } else {
    // neu tat ca chua chon het thi tron cai dang co voi tat ca cac ban ghi trong trang
    const merged = new Set([...selectedCandidateIds.value, ...currentPageIds.value])
    selectedCandidateIds.value = [...merged]
  }
}

const handleDeleteMany = () => {
  if (hasSelectedRows.value) {
    currentCandidate.value = null;
    modalMode.value = 'delete'
    isModalOpen.value = true
  }
}
// </editor-fold>

// <editor-fold> desc="Add new + Edit"
const saveCandidate = async (data: any) => {
  console.log('[Candidates.saveCandidate] data: ' + data)
  isModalOpen.value = false;
  if (modalMode.value === "add") {
    await handleSaveCandidate(data);
  }
  if (modalMode.value === "edit") {
    await handleSaveCandidate(data);
  }
}

const handleOpenAddingModal = () => {
  currentCandidate.value = null;
  modalMode.value = 'add'
  isModalOpen.value = true
}

const handleOpenEditModal = async (id: string) => {
  currentCandidate.value = filteredData.value.find(c => c.id === id) || null
  modalMode.value = 'edit'
  isModalOpen.value = true
}
// </editor-fold>

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
  let title = 'Xóa thành công';
  let message = 'Xóa ứng viên thành công';

  if (currentCandidate.value) {
    message = `Xóa ứng viên ${currentCandidate.value.name} thành công`;
    filteredData.value = filteredData.value.filter(c => c.id !== currentCandidate.value.id)
  } else if (selectedCandidateIds.value.length > 0) {
    message = `Xóa ${selectedCandidateIds.value.length} ứng viên thành công`;
    filteredData.value = filteredData.value.filter(
        c => !selectedCandidateIds.value.includes(c.id)
    )
    selectedCandidateIds.value = []
  }

  toast.success(title, message)
  currentCandidate.value = null;
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
        <CustomButton
            class-name="content_header_right_btn_delete"
            :disabled="!hasSelectedRows"
            @click="handleDeleteMany"
            tooltip-location="bottom"
            tooltip-content="Xóa"
        >
          <div class="mi_icon_delete_user"></div>
          Xóa
        </CustomButton>

        <!--        Them-->
        <CustomButton
            variant="flat"
            class-name="content_header_right_btn_delete"
            style="background-color: var(--border-control-hover)"
            @click="handleOpenAddingModal"
            tooltip-location="bottom"
            tooltip-content="Thêm ứng viên"
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
                <NormalCheckbox
                    :model-value="isAllCurrentPageSelected"
                    @update:model-value="toggleSelectAllCurrentPage"
                />
              </template>
              <template #cell-checkbox="{cell}">
                <NormalCheckbox
                    :model-value="selectedCandidateIds.includes(cell.id)"
                    @update:model-value="toggleCandidateSelection(cell.id)"
                />
              </template>
              <template #cell-skeleton>
                <CustomSkeleton height="16px" border-radius="4px"/>
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
              Tổng: <strong id="totalRecords">{{ totalRecordsServer }}</strong> bản ghi
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
                />
              </div>
            </div>
            <div class="content_body_footer_info">
              <span class="page_info" id="pageInfo">{{ pageInfo }}</span>
            </div>
            <div class="content_body_footer_nav">
              <CustomPagination
                  v-model="currentPage"
                  :total="totalRecordsServer"
                  :page-size="pageSize"
                  color="#0070f3"
              />
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

.content_body_footer_pagesize {
  display: flex;
  align-items: center;
  gap: 8px;
}

.page_size_custom_select {
  width: 80px; /* Độ rộng phù hợp cho số bản ghi */
}
</style>
